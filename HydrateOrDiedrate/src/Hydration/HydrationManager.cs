using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace HydrateOrDiedrate;

public static class HydrationManager
{
    private const string HydrationAttributeKey = "hydration";

    private static readonly Dictionary<AssetLocation, Vintagestory.API.Common.Func<ItemStack, float>> CustomHydrationEvaluators = new Dictionary<AssetLocation, Vintagestory.API.Common.Func<ItemStack, float>>();

    [Obsolete("Use RegisterHydrationEvaluator(AssetLocation code, Vintagestory.API.Common.Func<ItemStack, float> evaluator) instead")]
    public static void RegisterHydrationEvaluator(string collectibleCode, Vintagestory.API.Common.Func<ItemStack, float> evaluator) => RegisterHydrationEvaluator(new AssetLocation(collectibleCode), evaluator);
    
    public static void RegisterHydrationEvaluator(AssetLocation code, Vintagestory.API.Common.Func<ItemStack, float> evaluator)
    {
        if(code is null) throw new ArgumentNullException(nameof(code));
        else if (code.IsWildCard) throw new ArgumentException("Code may not be a wildcard", nameof(code));

        CustomHydrationEvaluators[code] = evaluator ?? throw new ArgumentNullException(nameof(code));
    }

    public static float GetHydration(ItemStack itemStack)
    {
        var collectible = itemStack?.Collectible;
        if (collectible is null) return 0f;

        if (CustomHydrationEvaluators.TryGetValue(collectible.Code, out var evaluator)) return evaluator(itemStack);

        return collectible.Attributes?.Token.Value<int>(HydrationAttributeKey) ?? 0f;
    }
}