using HydrateOrDiedrate.Config;
using HydrateOrDiedrate.Hot_Weather;
using Vintagestory.API.Common;
using XLib.XLeveling;

namespace HydrateOrDiedrate.XSkill;

public static class XLibSkills
{
    public static bool Enabled { get; internal set; }

    internal static void Initialize(ICoreAPI api)
    {
        XLeveling xleveling = api.ModLoader.GetModSystem<XLeveling>();
        if (xleveling is null) return;

        Skill skill = xleveling.GetSkill("survival", false);
        if (skill is null) return;

        if (ModConfig.Instance.Thirst.Enabled)
        {
            int[] dromedaryValues = [0];
            Ability dromedaryAbility = new Ability("dromedary", "hydrateordiedrate:ability-dromedary", "hydrateordiedrate:abilitydesc-dromedary", 1, 3, dromedaryValues, false);
            if(api.Side == EnumAppSide.Server) dromedaryAbility.OnPlayerAbilityTierChanged += OnDromedaryChanged;
            skill.AddAbility(dromedaryAbility);
        }

        //TODO this should be behind equivelant IF statement no?
        //TODO these values should come from Xlevel config not ours
        //TODO these values should be filled so we can use them in the ability description
        int[] equatidianValues = [0];
        Ability equatidianAbility = new Ability("equatidian", "hydrateordiedrate:ability-equatidian", "hydrateordiedrate:abilitydesc-equatidian", 1, 3, equatidianValues, false);
        equatidianAbility.OnPlayerAbilityTierChanged += OnEquatidianChanged;
        skill.AddAbility(equatidianAbility);
    }

    private static void OnDromedaryChanged(PlayerAbility playerAbility, int oldTier) => playerAbility.PlayerSkill.PlayerSkillSet.Player?.Entity.GetBehavior<EntityBehaviorThirst>()?.RecalculateMaxThirst();

    private static void OnEquatidianChanged(PlayerAbility playerAbility, int oldTier) => playerAbility.PlayerSkill.PlayerSkillSet.Player?.Entity.GetBehavior<EntityBehaviorBodyTemperatureHot>()?.RecalculateCoolingMultiplier();

    public static float GetDromedaryModifier(ICoreAPI api, IPlayer player)
    {
        var playerAbility = api.ModLoader.GetModSystem<XLeveling>()
            ?.IXLevelingAPI.GetPlayerSkillSet(player)
            ?.FindSkill("survival")
            ?.FindAbility("dromedary");

        if (playerAbility is null || playerAbility.Tier < 1) return 1f;

        return 1 + (ModConfig.Instance.XLib.DromedaryMultiplierPerLevel * playerAbility.Tier);
    }

    public static float GetEquatidianModifier(ICoreAPI api, IPlayer player)
    {
        var playerAbility = api.ModLoader.GetModSystem<XLeveling>()
            ?.IXLevelingAPI.GetPlayerSkillSet(player)
            ?.FindSkill("survival")
            ?.FindAbility("equatidian");

        if (playerAbility is null || playerAbility.Tier < 1) return 1f;

        var config = ModConfig.Instance.XLib.EquatidianCoolingMultipliers;
        if (playerAbility.Tier > config.Length)
        {
            return config.Length > 0 ? config[^1] : 1f;
        }
        else return config[playerAbility.Tier - 1];
    }
}
