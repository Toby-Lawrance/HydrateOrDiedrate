using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace HydrateOrDiedrate.patches;

[HarmonyPatch(typeof(BlockCookingContainer), "DoSmelt")]
public static class BlockCookingContainerPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.VeryLow)]
    public static bool Prefix(IWorldAccessor world, ISlotProvider cookingSlotsProvider, ItemSlot inputSlot, ItemSlot outputSlot, BlockCookingContainer __instance)
    {
        ItemStack[] stacks = __instance.GetCookingStacks(cookingSlotsProvider, true);
        CookingRecipe recipe = __instance.GetMatchingCookingRecipe(world, stacks);

        if (recipe == null)
        {
            return true;
        }
        if (recipe.CooksInto?.ResolvedItemstack?.Collectible?.Code?.ToString()?.Contains("hydrateordiedrate:") == true)
        {
            int quantityServings = recipe.GetQuantityServings(stacks);
            ItemStack resolvedItemstack = recipe.CooksInto.ResolvedItemstack;
            ItemStack outstack = resolvedItemstack?.Clone();
            if (outstack != null)
            {
                outstack.StackSize *= quantityServings;
                stacks = new ItemStack[] { outstack };
            }
            ItemStack outputStack = new ItemStack(__instance);
            outputStack.Collectible.SetTemperature(world, outputStack, BlockCookingContainer.GetIngredientsTemperature(world, stacks), true);
            TransitionableProperties cookedPerishProps = recipe.PerishableProps.Clone();
            cookedPerishProps.TransitionedStack.Resolve(world, "cooking container perished stack", true);

            ICoreAPI api = world.Api;

            CollectibleObject.CarryOverFreshness(api, cookingSlotsProvider.Slots, stacks, cookedPerishProps);
            for (int i = 0; i < cookingSlotsProvider.Slots.Length; i++)
            {
                cookingSlotsProvider.Slots[i].Itemstack = (i == 0) ? stacks[0] : null;
            }
            inputSlot.Itemstack = outputStack;
            return false;
        }
        return true;
    }
}