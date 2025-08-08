using HydrateOrDiedrate.Config;
using HydrateOrDiedrate.XSkill;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using XLib.XLeveling;

namespace HydrateOrDiedrate;

public partial class EntityBehaviorThirst
{
    public override string PropertyName() => "thirst";

    // seperate tree for thrist attributes to prevent too many paths from being marked dirty, which would result in full resyncs all the time
    private ITreeAttribute ThirstTree => entity.WatchedAttributes.GetTreeAttribute(thirstTreePath);
    public const string thirstTreePath = "thirst";

    public float MaxThirst
    {
        get => ThirstTree.TryGetFloat("maxThirst") ?? ModConfig.Instance.Thirst.MaxThirst;
        set
        {
            var currentMaxThirst = MaxThirst;
            if (!float.IsFinite(value) || value == currentMaxThirst) return;

            ThirstTree.SetFloat("maxThirst", value);
            entity.WatchedAttributes.MarkPathDirty(thirstTreePath);

            CurrentThirst = (CurrentThirst / currentMaxThirst) * value; // adjust current thirst relative to the new max thirst
        }
    }

    public float CurrentThirst
    {
        get => ThirstTree.TryGetFloat("currentThirst") ?? MaxThirst;
        set
        {
            var maxThirst = MaxThirst;
            var safeValue = GameMath.Clamp(value.GuardFinite(maxThirst), 0, maxThirst);
            if (safeValue == CurrentThirst) return;

            ThirstTree.SetFloat("currentThirst", safeValue);
            UpdateMovementPenalty();
            entity.WatchedAttributes.MarkPathDirty(thirstTreePath);
        }
    }

    public float ThirstRate
    {
        get => ThirstTree.TryGetFloat("thirstRate") ?? ModConfig.Instance.Thirst.ThirstDecayRate;
        set
        {
            var safeValue = value.GuardFinite();
            if(safeValue == ThirstRate) return;

            ThirstTree.SetFloat("thirstRate", safeValue);
            entity.WatchedAttributes.MarkPathDirty(thirstTreePath);
        }
    }

    public float HungerReductionAmount
    {
        get => ThirstTree.GetFloat("hungerReductionAmount");
        set
        {
            var safeValue = Math.Max((float)Math.Ceiling(value.GuardFinite()), 0f);
            if(safeValue == HungerReductionAmount) return;
            
            ThirstTree.SetFloat("hungerReductionAmount", safeValue);
            entity.WatchedAttributes.MarkPathDirty(thirstTreePath);
        }
    }

    public int HydrationLossDelay
    {
        get => ThirstTree.GetInt("hydrationLossDelay");
        set
        {
            if(value == HydrationLossDelay) return;
            ThirstTree.SetInt("hydrationLossDelay", value);
            entity.WatchedAttributes.MarkPathDirty(thirstTreePath);
        }
    }

    private float movementPenalty;
    public float MovementPenalty
    {
        get => movementPenalty;
        set
        {

            float safeValue = GameMath.Clamp(value.GuardFinite(), 0, ModConfig.Instance.Thirst.MaxMovementSpeedPenalty);
            if (movementPenalty == safeValue) return;

            movementPenalty = safeValue;
            entity.Stats.Set("walkspeed", "thirstPenalty", -safeValue, false);
        }
    }

    public void MapLegacyData()
    {
        var attr = entity.WatchedAttributes;
        attr.RemoveAttribute("dromedaryActive");
        
        if(attr.HasAttribute("maxThirst"))
        {
            MaxThirst = attr.GetFloat("maxThirst");
            attr.RemoveAttribute("maxThirst");
        }
        if(attr.HasAttribute("currentThirst"))
        {
            CurrentThirst = attr.GetFloat("currentThirst");
            attr.RemoveAttribute("currentThirst");
        }
        if(attr.HasAttribute("thirstRate"))
        {
            ThirstRate = attr.GetFloat("thirstRate");
            attr.RemoveAttribute("thirstRate");
        }
        if(attr.HasAttribute("hungerReductionAmount"))
        {
            HungerReductionAmount = attr.GetFloat("hungerReductionAmount");
            attr.RemoveAttribute("hungerReductionAmount");
        }
        if(attr.HasAttribute("hydrationLossDelay"))
        {
            HydrationLossDelay = attr.GetInt("hydrationLossDelay");
            attr.RemoveAttribute("hydrationLossDelay");
        }
    }

    private void InitThirstAttributes()
    {
        if(entity.WatchedAttributes.GetTreeAttribute(thirstTreePath) is null)
        {
            entity.WatchedAttributes.SetAttribute(thirstTreePath, new TreeAttribute());
            MapLegacyData();
        }

        if(!XLibSkills.Enabled) RecalculateMaxThirst();
    }

    public void RecalculateMaxThirst()
    {
        if(entity.Api.Side != EnumAppSide.Server) return;
        var newMaxThirst = ModConfig.Instance.Thirst.MaxThirst;

        if(XLibSkills.Enabled) newMaxThirst *= XLibSkills.GetDromedaryModifier(entity.Api, (entity as EntityPlayer)?.Player);

        MaxThirst = newMaxThirst;
    }
}
