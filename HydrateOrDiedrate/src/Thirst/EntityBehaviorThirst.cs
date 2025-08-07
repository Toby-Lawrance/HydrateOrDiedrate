using HydrateOrDiedrate.Config;
using HydrateOrDiedrate.Thirst;
using System;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace HydrateOrDiedrate;

public partial class EntityBehaviorThirst : EntityBehavior
{
    public EntityBehaviorThirst(Entity entity) : base(entity) { }
    public override void Initialize(EntityProperties properties, JsonObject attributes)
    {
        base.Initialize(properties, attributes);
        InitThirstAttributes();
        UpdateMovementPenalty();
    }

    private void UpdateMovementPenalty()
    {
        float currentThirst = CurrentThirst;
        var threshold = ModConfig.Instance.Thirst.MovementSpeedPenaltyThreshold;
        if(threshold < 0 || currentThirst > threshold)
        {
            MovementPenalty = 0;
        }
        else if(currentThirst <= 0)
        {
            MovementPenalty = ModConfig.Instance.Thirst.MaxMovementSpeedPenalty;
        }
        else if(threshold > 0)
        {
            MovementPenalty = ModConfig.Instance.Thirst.MaxMovementSpeedPenalty * (1 - (currentThirst / threshold));
        }
        else MovementPenalty = 0;
    }

    public override void OnEntityReceiveDamage(DamageSource damageSource, ref float damage)
    {
        //On respawn, reset thirst to a percentage of max thirst
        if (damageSource.Source == EnumDamageSource.Revive && damageSource.Type == EnumDamageType.Heal)
        {
            CurrentThirst = ModConfig.Instance.Thirst.ThirstPercentageOnRespawn * MaxThirst;
            HungerReductionAmount = 0;
        }
    }

    private void ApplyThirstDamage(float thirstDamage)
    {
        if(thirstDamage <= 0) return;
        entity.ReceiveDamage(new DamageSource
        {
            Source = EnumDamageSource.Internal,
            Type = EnumDamageType.Injury
        }, thirstDamage);
    }

    public void ModifyThirst(float amount, float hydLossDelay = 0)
    {
        CurrentThirst += amount.GuardFinite();

        int hydrationLossDelay = HydrationLossDelay;
        if (hydLossDelay > hydrationLossDelay)
        {
            HydrationLossDelay = (int)Math.Floor(hydLossDelay);
        }
    }
    
    public const float thirstInterval = 10f; // seconds
    private float thirstDelta;

    public const float thirstDamageInterval = 10f; // seconds
    private float thirstDamageDelta;

    public const float fastInterval = 2f; // seconds
    private float fastDelta;

    private long lastMoveMs;

    public override void OnGameTick(float deltaTime)
    {
        if (entity.World.Side != EnumAppSide.Server || !ModConfig.Instance.Thirst.Enabled || !entity.Alive || entity is not EntityPlayer playerEntity) return;
        if (playerEntity.Player?.WorldData.CurrentGameMode != EnumGameMode.Survival) return;

        fastDelta += deltaTime;
        if (fastDelta >= fastInterval)
        {
            var controls = playerEntity.Controls;
            if (controls.TriesToMove || controls.Jump || controls.LeftMouseDown || controls.RightMouseDown)
            {
                lastMoveMs = entity.World.ElapsedMilliseconds;
            }


            UpdateHydrationLossDelay(fastDelta);
            fastDelta = 0f;
        }

        if(HydrationLossDelay > 0) return;

        thirstDelta += deltaTime;
        if (thirstDelta >= thirstInterval)
        {
            HandleAccumulatedThirstDecay(playerEntity, thirstDelta);
            thirstDelta = 0;
        }

        if (CurrentThirst > 0) return;

        thirstDamageDelta += deltaTime;
        if (thirstDamageDelta >= thirstDamageInterval)
        {
            ApplyThirstDamage(ModConfig.Instance.Thirst.ThirstDamage);
            thirstDamageDelta -= thirstDamageInterval;
        }
    }

    private const float DefaultSpeedOfTime = 60f;
    private const float DefaultCalendarSpeedMul = 0.5f;

    public float CalculateMultiplierPerGameSecond()
    {
        float currentSpeedOfTime = entity.World.Calendar.SpeedOfTime.GuardFinite(DefaultSpeedOfTime);
        float currentCalendarSpeedMul = entity.World.Calendar.CalendarSpeedMul.GuardFinite(DefaultCalendarSpeedMul);

        float multiplierPerGameSec = Util.GuardFinite((currentSpeedOfTime / DefaultSpeedOfTime) * (currentCalendarSpeedMul / DefaultCalendarSpeedMul));
        if (multiplierPerGameSec <= 0) multiplierPerGameSec = 1f;

        return multiplierPerGameSec;
    }

    private void UpdateHydrationLossDelay(float elapsedSeconds)
    {
        int currentDelay = HydrationLossDelay;
        if(currentDelay <= 0) return;

        HydrationLossDelay = Math.Max(0, currentDelay - (int)(CalculateMultiplierPerGameSecond() * elapsedSeconds));
    }

    private void HandleAccumulatedThirstDecay(EntityPlayer player, float deltaTime)
    {
        var thirstRate = ThirstRate = CalculateThirstRate(player);

        thirstRate *= CalculateMultiplierPerGameSecond();

        ModifyThirst(-thirstRate * deltaTime * 0.1f);
    }

    private float CalculateThirstRate(EntityPlayer player)
    {
        var config = ModConfig.Instance.Thirst;
        float thirstDecayRate = config.ThirstDecayRate;

        var controls = player.Controls;
        if (controls.Sprint)
        {
            thirstDecayRate *= config.SprintThirstMultiplier;
        }

        foreach(var modifier in player.SidedProperties.Behaviors.OfType<IThirstRateModifier>())
        {
            var newThirstDecayRate = modifier.OnThirstRateCalculate(thirstDecayRate);
            
            if (float.IsFinite(newThirstDecayRate)) thirstDecayRate = newThirstDecayRate;
        }

        thirstDecayRate = Math.Min(thirstDecayRate, config.ThirstDecayRate * config.ThirstDecayRateMax);


        if (entity.World.ElapsedMilliseconds - lastMoveMs > 3000L) thirstDecayRate *= config.IdleThirstModifier;

        return thirstDecayRate.GuardFinite(config.ThirstDecayRate);
    }

}
