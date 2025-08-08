using HydrateOrDiedrate.XSkill;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace HydrateOrDiedrate.Hot_Weather;

public partial class EntityBehaviorBodyTemperatureHot : EntityBehavior
{
    private ITreeAttribute TempTree => entity.WatchedAttributes.GetTreeAttribute(tempTreePath);
    public const string tempTreePath = "bodyTemp";

    public float CoolingMultiplier { get; internal set; } = 1f;

    public float Cooling
    {
        get => TempTree?.GetFloat("cooling") ?? 0f;
        set
        {
            var tree = TempTree;
            if(tree is null) return;

            var safeValue = GameMath.Clamp(value.GuardFinite(), 0, float.MaxValue);
            if(safeValue == Cooling) return;

            tree.SetFloat("cooling", safeValue);
            entity.WatchedAttributes.MarkPathDirty(tempTreePath);
        }
    }

    public void InitBodyHeatAttributes()
    {
        MapLegacyData();

        if(!XLibSkills.Enabled) RecalculateCoolingMultiplier();
    }

    private void MapLegacyData()
    {
        var attr = entity.WatchedAttributes;
        attr.RemoveAttribute("currentCoolingHot");
        attr.RemoveAttribute("adjustedCoolingHot");
    }

    public void RecalculateCoolingMultiplier()
    {
        var newCoolingModifier = 1f;
        if(XLibSkills.Enabled) newCoolingModifier *= XLibSkills.GetEquatidianModifier(entity.Api, (entity as EntityPlayer)?.Player);
        Cooling = newCoolingModifier;
    }
}
