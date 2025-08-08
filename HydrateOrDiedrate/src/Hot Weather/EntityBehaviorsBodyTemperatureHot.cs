using HydrateOrDiedrate.Config;
using HydrateOrDiedrate.Thirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace HydrateOrDiedrate.Hot_Weather;

public partial class EntityBehaviorBodyTemperatureHot(Entity entity) : EntityBehavior(entity), IThirstRateModifier
{
    public override string PropertyName() => "bodytemperaturehot";

    public override void Initialize(EntityProperties properties, JsonObject attributes)
    {
        base.Initialize(properties, attributes);
        InitBodyHeatAttributes();
    }

    public float OnThirstRateCalculate(float currentModifier)
    {
        if (!ModConfig.Instance.HeatAndCooling.HarshHeat) return currentModifier;
        UpdateCoolingFactor();
        
        var temperature = entity.World.BlockAccessor.GetClimateAt(entity.ServerPos.AsBlockPos, EnumGetClimateMode.NowValues)?.Temperature.GuardFinite() ?? 0f;
        if (temperature > ModConfig.Instance.HeatAndCooling.TemperatureThreshold)
        {
            float temperatureDifference = temperature - ModConfig.Instance.HeatAndCooling.TemperatureThreshold;
            float expArgument = ModConfig.Instance.HeatAndCooling.HarshHeatExponentialGainMultiplier * temperatureDifference;

            currentModifier += Util.GuardFinite(ModConfig.Instance.HeatAndCooling.ThirstIncreasePerDegreeMultiplier * (float)Math.Exp(expArgument));

            float coolingFactor = Cooling;

            float expCooling = (float)Math.Exp(-0.5f * temperatureDifference);

            float coolingEffect = coolingFactor * (1f / (1f + expCooling.GuardFinite(1f)));

            currentModifier -= Math.Min(coolingEffect.GuardFinite(), currentModifier - ModConfig.Instance.Thirst.ThirstDecayRate);
        }

        return currentModifier;
    }

    public void UpdateCoolingFactor()
    {
        if (entity is not EntityAgent entityAgent) return;
        UpdateRoomInfo();
        
        var behaviorContainer = entityAgent.GetBehavior<EntityBehaviorContainer>();
        if (behaviorContainer is null || behaviorContainer.Inventory is null) return;

        var inventory = behaviorContainer.Inventory;

        int unequippedSlots = 0;
        float finalCooling = 0f;

        for (int i = 0; i < inventory.Count; i++)
        {
            var slot = inventory[i];

            if (slot?.Itemstack is null)
            {
                if (i == 0 || i == 1 || i == 2 || i == 11 || i == 3 || i == 4 || i == 8 || i == 5)
                {
                    unequippedSlots++;
                }
                continue;
            }
            
            var cooling = CustomItemWearableExtensions.GetCooling(slot, entity.World.Api);
            
            finalCooling += (float)Math.Round(cooling, 1, MidpointRounding.AwayFromZero);
        }

        var config = ModConfig.Instance.HeatAndCooling;
        
        finalCooling += unequippedSlots * config.UnequippedSlotCooling;

        if (entity.WatchedAttributes.GetFloat("wetness", 0f) > 0) finalCooling *= config.WetnessCoolingFactor;

        if (inEnclosedRoom) finalCooling *= config.ShelterCoolingFactor;

        finalCooling -= nearbyHeatSourcesStrength * 0.5f;
        
        int sunlightLevel = entity.World.BlockAccessor.GetLightLevel(entity.SidedPos.AsBlockPos, EnumLightLevelType.TimeOfDaySunLight);
        double hourOfDay = entity.World.Calendar?.HourOfDay ?? 0;

        float sunlightCooling = (16 - sunlightLevel) / 16f * config.SunlightCoolingFactor;
        double distanceTo4AM = GameMath.SmoothStep(Math.Abs(GameMath.CyclicValueDistance(4.0, hourOfDay, 24.0) / 12.0));
        double distanceTo3PM = GameMath.SmoothStep(Math.Abs(GameMath.CyclicValueDistance(15.0, hourOfDay, 24.0) / 12.0));

        double diurnalCooling = (0.5 - distanceTo4AM - distanceTo3PM) * config.DiurnalVariationAmplitude;
        finalCooling += (float)(sunlightCooling + diurnalCooling);
        
        Cooling = Math.Max(0, Util.GuardFinite(finalCooling * CoolingMultiplier));
    }

    private bool inEnclosedRoom;
    private float nearbyHeatSourcesStrength;
    private void UpdateRoomInfo()
    {
        var roomRegistry = entity.Api.ModLoader.GetModSystem<RoomRegistry>();
        if (roomRegistry is null) return;

        var tempPos = entity.Pos.AsBlockPos;
        Room room = roomRegistry.GetRoomForPosition(tempPos);
        
        inEnclosedRoom = false;
        nearbyHeatSourcesStrength = 0f;

        if (room is null) return;
        inEnclosedRoom = (room.ExitCount == 0 && room.SkylightCount < room.NonSkylightCount); //TODO why this weird skylight check?

        if (!inEnclosedRoom) return;
        const double proximityPower = 0.875;

        BlockPos min = new(room.Location.X1, room.Location.Y1, room.Location.Z1, tempPos.dimension);
        BlockPos max = new(room.Location.X2, room.Location.Y2, room.Location.Z2, tempPos.dimension);

        var entityPos = tempPos.Copy();
        entity.World.BlockAccessor.WalkBlocks(min, max, (block, x, y, z) =>
        {
            tempPos.Set(x, y, z);
            var blockEntity = entity.World.BlockAccessor.GetBlockEntity(tempPos);
            if (blockEntity is IHeatSource heatSource)
            {
                float factor = Math.Min(
                    1f,
                    9f / (8f + (float)Math.Pow(tempPos.DistanceTo(entityPos.X, entityPos.Y + 0.9f, entityPos.Z), proximityPower))
                );

                nearbyHeatSourcesStrength += heatSource.GetHeatStrength(entity.World, tempPos, entityPos) * factor;
            }
        });

        if (entity.Api.ModLoader.IsModEnabled("medievalexpansion") && IsRoomRefrigerated(room))
        {
            Cooling += ModConfig.Instance.HeatAndCooling.RefrigerationCooling;
        }

        nearbyHeatSourcesStrength = nearbyHeatSourcesStrength.GuardFinite();
    }

    private object RoomRefridgePositionManagerInstance;

    //TODO this reflection should be removed and just use an optional reference to the DLL
    private bool IsRoomRefrigerated(Room room)
    {
        if (room is null) return false;

        RoomRefridgePositionManagerInstance ??= Array.Find(AppDomain.CurrentDomain.GetAssemblies(), a => a.GetName().Name == "medievalexpansion")
            ?.GetType("medievalexpansion.src.busineslogic.RoomRefridgePositionManager")
            ?.GetMethod("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?.Invoke(null, null);

        if (RoomRefridgePositionManagerInstance is null) return false;

        var positionsProperty = RoomRefridgePositionManagerInstance.GetType().GetProperty("RoomRefridgerPosition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (positionsProperty is null) return false;

        if (positionsProperty.GetValue(RoomRefridgePositionManagerInstance) is not IList<BlockPos> positions) return false;

        foreach (var pos in positions)
        {
            if (room.Location.Contains(pos))
            {
                return true;
            }
        }
        return false;
    }
}
