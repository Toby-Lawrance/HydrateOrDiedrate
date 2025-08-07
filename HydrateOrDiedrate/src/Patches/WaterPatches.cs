using System.Collections.Generic;
using HydrateOrDiedrate.Config;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.ServerMods.NoObf;

namespace HydrateOrDiedrate;

public static class WaterPatches
{
    public static void PrepareWaterSatietyPatches(ICoreAPI api)
    {
        ApplySatietyPatch(api, "game:itemtypes/liquid/waterportion.json", ModConfig.Instance.Satiety.WaterSatiety);
        ApplySatietyPatch(api, "game:itemtypes/liquid/saltwaterportion.json", ModConfig.Instance.Satiety.SaltWaterSatiety);
        ApplySatietyPatch(api, "game:itemtypes/liquid/boilingwaterportion.json", ModConfig.Instance.Satiety.BoilingWaterSatiety);
        ApplySatietyPatch(api, "hydrateordiedrate:itemtypes/liquid/rainwaterportion.json", ModConfig.Instance.Satiety.RainWaterSatiety);
        ApplySatietyPatch(api, "hydrateordiedrate:itemtypes/liquid/distilledwaterportion.json", ModConfig.Instance.Satiety.DistilledWaterSatiety);
        ApplySatietyPatch(api, "hydrateordiedrate:itemtypes/liquid/boiledwaterportion.json", ModConfig.Instance.Satiety.BoiledWaterSatiety);
        ApplySatietyPatch(api, "hydrateordiedrate:itemtypes/liquid/boiledrainwaterportion.json", ModConfig.Instance.Satiety.BoiledRainWaterSatiety);
    }

    private static void ApplySatietyPatch(ICoreAPI api, string jsonFilePath, float satietyValue)
    {
        var ensureNutritionProps = new JsonPatch
        {
            Op = EnumJsonPatchOp.AddMerge,
            Path = "/attributes/waterTightContainerProps/nutritionPropsPerLitre",
            Value = new JsonObject(new JObject()),
            File = new AssetLocation(jsonFilePath),
            Side = EnumAppSide.Server
        };

        var patchSatiety = new JsonPatch
        {
            Op = EnumJsonPatchOp.AddMerge,
            Path = "/attributes/waterTightContainerProps/nutritionPropsPerLitre/satiety",
            Value = new JsonObject(JToken.FromObject(satietyValue)),
            File = new AssetLocation(jsonFilePath),
            Side = EnumAppSide.Server
        };

        var patchFoodCategory = new JsonPatch
        {
            Op = EnumJsonPatchOp.AddMerge,
            Path = "/attributes/waterTightContainerProps/nutritionPropsPerLitre/foodcategory",
            Value = new JsonObject(JToken.FromObject("NoNutrition")),
            File = new AssetLocation(jsonFilePath),
            Side = EnumAppSide.Server
        };

        int applied = 0, notFound = 0, errorCount = 0;
        ModJsonPatchLoader patchLoader = api.ModLoader.GetModSystem<ModJsonPatchLoader>();
        patchLoader.ApplyPatch(0, new AssetLocation("hydrateordiedrate:ensureNutritionProps"), ensureNutritionProps, ref applied, ref notFound, ref errorCount);
        patchLoader.ApplyPatch(0, new AssetLocation("hydrateordiedrate:dynamicsatietypatch"), patchSatiety, ref applied, ref notFound, ref errorCount);
        patchLoader.ApplyPatch(0, new AssetLocation("hydrateordiedrate:dynamicfoodcategorypatch"), patchFoodCategory, ref applied, ref notFound, ref errorCount);
    }

    public static void PrepareWellWaterSatietyPatches(ICoreAPI api)
    {
        var wellWaterSatietyValues = new Dictionary<string, float>
        {
            { "fresh", ModConfig.Instance.Satiety.WellWaterFreshSatiety },
            { "salt", ModConfig.Instance.Satiety.WellWaterSaltSatiety },
            { "muddy", ModConfig.Instance.Satiety.WellWaterMuddySatiety },
            { "tainted", ModConfig.Instance.Satiety.WellWaterTaintedSatiety },
            { "poisoned", ModConfig.Instance.Satiety.WellWaterPoisonedSatiety },
            { "muddysalt",  ModConfig.Instance.Satiety.WellWaterMuddySaltSatiety },
            { "taintedsalt", ModConfig.Instance.Satiety.WellWaterTaintedSaltSatiety },
            { "poisonedsalt", ModConfig.Instance.Satiety.WellWaterPoisonedSaltSatiety }
        };

        foreach (var kvp in wellWaterSatietyValues)
        {
            ApplyWellWaterSatietyPatch(api, "hydrateordiedrate:itemtypes/liquid/wellwaterportion.json", kvp.Key, kvp.Value);
        }
    }

    private static void ApplyWellWaterSatietyPatch(ICoreAPI api, string jsonFilePath, string waterType, float satietyValue)
    {
        int applied = 0, notFound = 0, errorCount = 0;
        ModJsonPatchLoader patchLoader = api.ModLoader.GetModSystem<ModJsonPatchLoader>();
        var ensureNutritionProps = new JsonPatch
        {
            Op = EnumJsonPatchOp.AddMerge,
            Path = $"/attributesByType/*-{waterType}/waterTightContainerProps/nutritionPropsPerLitre",
            Value = new JsonObject(new JObject()),
            File = new AssetLocation(jsonFilePath),
            Side = EnumAppSide.Server
        };

        var patchSatiety = new JsonPatch
        {
            Op = EnumJsonPatchOp.AddMerge,
            Path = $"/attributesByType/*-{waterType}/waterTightContainerProps/nutritionPropsPerLitre/satiety",
            Value = new JsonObject(JToken.FromObject(satietyValue)),
            File = new AssetLocation(jsonFilePath),
            Side = EnumAppSide.Server
        };

        var patchFoodCategory = new JsonPatch
        {
            Op = EnumJsonPatchOp.AddMerge,
            Path = $"/attributesByType/*-{waterType}/waterTightContainerProps/nutritionPropsPerLitre/foodcategory",
            Value = new JsonObject(JToken.FromObject("NoNutrition")),
            File = new AssetLocation(jsonFilePath),
            Side = EnumAppSide.Server
        };

        patchLoader.ApplyPatch(0, new AssetLocation($"hydrateordiedrate:wellwaterensure-{waterType}"), ensureNutritionProps, ref applied, ref notFound, ref errorCount);
        patchLoader.ApplyPatch(0, new AssetLocation($"hydrateordiedrate:wellwatersatiety-{waterType}"), patchSatiety, ref applied, ref notFound, ref errorCount);
        patchLoader.ApplyPatch(0, new AssetLocation($"hydrateordiedrate:wellwaterfoodcat-{waterType}"), patchFoodCategory, ref applied, ref notFound, ref errorCount);
    }
    public static void PrepareWaterPerishPatches(ICoreAPI api)
    {
        if (!ModConfig.Instance.PerishRates.Enabled)
        {
            RemoveWaterPerishPatches(api);
            return;
        }

        float rainWaterFreshFreshHours = ModConfig.Instance.PerishRates.RainWaterFreshHours;
        float rainWaterFreshTransitionHours = ModConfig.Instance.PerishRates.RainWaterTransitionHours;
        float boiledWaterFreshFreshHours = ModConfig.Instance.PerishRates.BoiledWaterFreshHours;
        float boiledWaterFreshTransitionHours = ModConfig.Instance.PerishRates.BoiledWaterFreshHours;
        float boiledRainWaterFreshFreshHours = ModConfig.Instance.PerishRates.BoiledRainWaterFreshHours;
        float boiledRainWaterFreshTransitionHours = ModConfig.Instance.PerishRates.BoiledRainWaterTransitionHours;
        float distilledWaterFreshFreshHours = ModConfig.Instance.PerishRates.DistilledWaterFreshHours;
        float distilledWaterFreshTransitionHours = ModConfig.Instance.PerishRates.DistilledWaterTransitionHours;

        ApplyPerishPatch(api, "hydrateordiedrate:itemtypes/liquid/rainwaterportion.json", rainWaterFreshFreshHours, rainWaterFreshTransitionHours);
        ApplyPerishPatch(api, "hydrateordiedrate:itemtypes/liquid/distilledwaterportion.json", distilledWaterFreshFreshHours, distilledWaterFreshTransitionHours);
        ApplyPerishPatch(api, "hydrateordiedrate:itemtypes/liquid/boiledwaterportion.json", boiledWaterFreshFreshHours, boiledWaterFreshTransitionHours);
        ApplyPerishPatch(api, "hydrateordiedrate:itemtypes/liquid/boiledrainwaterportion.json", boiledRainWaterFreshFreshHours, boiledRainWaterFreshTransitionHours);
    }

    private static void RemoveWaterPerishPatches(ICoreAPI api)
    {
        string[] jsonFiles = new string[]
        {
            "hydrateordiedrate:itemtypes/liquid/rainwaterportion.json",
            "hydrateordiedrate:itemtypes/liquid/distilledwaterportion.json",
            "hydrateordiedrate:itemtypes/liquid/boiledwaterportion.json",
            "hydrateordiedrate:itemtypes/liquid/boiledrainwaterportion.json"
        };

        ModJsonPatchLoader patchLoader = api.ModLoader.GetModSystem<ModJsonPatchLoader>();
        foreach (var jsonFilePath in jsonFiles)
        {
            int applied = 0, notFound = 0, errorCount = 0;
            var removePatch = new JsonPatch
            {
                Op = EnumJsonPatchOp.Remove,
                Path = "/transitionableProps",
                File = new AssetLocation(jsonFilePath),
                Side = EnumAppSide.Server
            };
            patchLoader.ApplyPatch(0, new AssetLocation("hydrateordiedrate:removeperishpatch"), removePatch, ref applied, ref notFound, ref errorCount);
        }
    }

    public static void PrepareWellWaterPerishPatches(ICoreAPI api)
    {
        if (!ModConfig.Instance.PerishRates.Enabled)
        {
            RemoveWellWaterPerishPatches(api);
            return;
        }

        var wellWaterPerishRateValues = new Dictionary<string, (float fresh, float transition)>
        {
            {
                "fresh",
                (
                    ModConfig.Instance.PerishRates.WellWaterFreshFreshHours,
                    ModConfig.Instance.PerishRates.WellWaterFreshTransitionHours
                )
            },
            {
                "salt",
                (
                    ModConfig.Instance.PerishRates.WellWaterSaltFreshHours,
                    ModConfig.Instance.PerishRates.WellWaterSaltTransitionHours
                )
            },
            {
                "muddy",
                (
                    ModConfig.Instance.PerishRates.WellWaterMuddyFreshHours,
                    ModConfig.Instance.PerishRates.WellWaterMuddyTransitionHours
                )
            },
            {
                "tainted",
                (
                    ModConfig.Instance.PerishRates.WellWaterTaintedFreshHours,
                    ModConfig.Instance.PerishRates.WellWaterTaintedTransitionHours
                )
            },
            {
                "poisoned",
                (
                    ModConfig.Instance.PerishRates.WellWaterPoisonedFreshHours,
                    ModConfig.Instance.PerishRates.WellWaterPoisonedTransitionHours
                )
            },
            {
                "muddysalt",
                (
                    ModConfig.Instance.PerishRates.WellWaterMuddySaltFreshHours,
                    ModConfig.Instance.PerishRates.WellWaterMuddySaltTransitionHours
                )
            },
            {
                "taintedsalt",
                (
                    ModConfig.Instance.PerishRates.WellWaterTaintedSaltFreshHours,
                    ModConfig.Instance.PerishRates.WellWaterTaintedSaltTransitionHours
                )
            },
            {
                "poisonedsalt",
                (
                    ModConfig.Instance.PerishRates.WellWaterPoisonedSaltFreshHours,
                    ModConfig.Instance.PerishRates.WellWaterPoisonedSaltTransitionHours
                )
            }
        };

        foreach (var kvp in wellWaterPerishRateValues)
        {
            ApplyWellWaterPerishRatePatch(api, "hydrateordiedrate:itemtypes/liquid/wellwaterportion.json", kvp.Key, kvp.Value.fresh, kvp.Value.transition);
        }
    }

    private static void RemoveWellWaterPerishPatches(ICoreAPI api)
    {
        ModJsonPatchLoader patchLoader = api.ModLoader.GetModSystem<ModJsonPatchLoader>();
        int applied = 0, notFound = 0, errorCount = 0;
        var removePatch = new JsonPatch
        {
            Op = EnumJsonPatchOp.Remove,
            Path = "/transitionablePropsByType",
            File = new AssetLocation("hydrateordiedrate:itemtypes/liquid/wellwaterportion.json"),
            Side = EnumAppSide.Server
        };

        patchLoader.ApplyPatch(0, new AssetLocation("hydrateordiedrate:removewellwaterperishpatch"), removePatch, ref applied, ref notFound, ref errorCount);
    }

    private static void ApplyPerishPatch(ICoreAPI api, string jsonFilePath, float freshHours, float transitionHours)
    {
        var patchFreshHours = new JsonPatch
        {
            Op = EnumJsonPatchOp.AddMerge,
            Path = "/transitionableProps/0/freshHours",
            Value = new JsonObject(JToken.FromObject(new JObject{ ["avg"] = freshHours })),
            File = new AssetLocation(jsonFilePath),
            Side = EnumAppSide.Server
        };
    
        var patchTransitionHours = new JsonPatch
        {
            Op = EnumJsonPatchOp.AddMerge,
            Path = "/transitionableProps/0/transitionHours",
            Value = new JsonObject(JToken.FromObject(new JObject{ ["avg"] = transitionHours })),
            File = new AssetLocation(jsonFilePath),
            Side = EnumAppSide.Server
        };

        int applied = 0, notFound = 0, errorCount = 0;
        ModJsonPatchLoader patchLoader = api.ModLoader.GetModSystem<ModJsonPatchLoader>();
        patchLoader.ApplyPatch(0, new AssetLocation("hydrateordiedrate:dynamicfreshhourspatch"), patchFreshHours, ref applied, ref notFound, ref errorCount);
        patchLoader.ApplyPatch(0, new AssetLocation("hydrateordiedrate:dynamictransitionhourspatch"), patchTransitionHours, ref applied, ref notFound, ref errorCount);
    }

    private static void ApplyWellWaterPerishRatePatch(ICoreAPI api, string jsonFilePath, string waterType, float freshHours, float transitionHours)
    {
        int applied = 0, notFound = 0, errorCount = 0;
        ModJsonPatchLoader patchLoader = api.ModLoader.GetModSystem<ModJsonPatchLoader>();
        var patchFreshHours = new JsonPatch
        {
            Op = EnumJsonPatchOp.AddMerge,
            Path = $"/transitionablePropsByType/*-{waterType}/0/freshHours",
            Value = new JsonObject(JToken.FromObject(new JObject { ["avg"] = freshHours })),
            File = new AssetLocation(jsonFilePath),
            Side = EnumAppSide.Server
        };

        var patchTransitionHours = new JsonPatch
        {
            Op = EnumJsonPatchOp.AddMerge,
            Path = $"/transitionablePropsByType/*-{waterType}/0/transitionHours",
            Value = new JsonObject(JToken.FromObject(new JObject { ["avg"] = transitionHours })),
            File = new AssetLocation(jsonFilePath),
            Side = EnumAppSide.Server
        };
        patchLoader.ApplyPatch(0, new AssetLocation($"hydrateordiedrate:wellwaterfreshperishrate-{waterType}"), patchFreshHours, ref applied, ref notFound, ref errorCount);
        patchLoader.ApplyPatch(0, new AssetLocation($"hydrateordiedrate:wellwatertransitionperishrate-{waterType}"), patchTransitionHours, ref applied, ref notFound, ref errorCount);
    }
}