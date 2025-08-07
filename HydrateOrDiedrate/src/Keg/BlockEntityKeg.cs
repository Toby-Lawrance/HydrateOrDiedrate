using HydrateOrDiedrate.Config;
using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace HydrateOrDiedrate.Keg;

public class BlockEntityKeg : BlockEntityLiquidContainer
{
    public float MeshAngle;

    public override string InventoryClassName => "keg";

    public override void Initialize(ICoreAPI api)
    {
        base.Initialize(api);

        UpdateBlockRelatedStats();
    }
    
    private void UpdateBlockRelatedStats()
    {
        if (inventory is null) return;

        var isTapped = Block.Code.Path == "kegtapped";
        
        //Note: sadly TakeLocked is not fully respected by liquid container code so we still need to overwrite some other methods
        inventory.TakeLocked = !isTapped;

        inventory.TransitionableSpeedMulByType ??= new System.Collections.Generic.Dictionary<EnumTransitionType, float>();

        inventory.TransitionableSpeedMulByType[EnumTransitionType.Perish] = isTapped
            ? ModConfig.Instance.Containers.SpoilRateTapped
            : ModConfig.Instance.Containers.SpoilRateUntapped;
    }

    public override void OnExchanged(Block block)
    {
        base.OnExchanged(block);
        UpdateBlockRelatedStats();
    }

    public BlockEntityKeg()
    {
        inventory = new InventoryGeneric(1, null, null, null)
        {
            BaseWeight = 1.0f,
            OnGetSuitability = GetSuitability,
        };
    }

    private float GetSuitability(ItemSlot sourceSlot, ItemSlot targetSlot, bool isMerge)
    {
        if (targetSlot == inventory[0] && inventory[0].StackSize > 0)
        {
            ItemStack currentStack = inventory[0].Itemstack;
            ItemStack testStack = sourceSlot.Itemstack;
            if (currentStack.Collectible.Equals(currentStack, testStack, GlobalConstants.IgnoredStackAttributes)) return -1;
        }

        return (isMerge ? (inventory.BaseWeight + 3) : (inventory.BaseWeight + 1)) +
               (sourceSlot.Inventory is InventoryBasePlayer ? 1 : 0);
    }

    public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
    {
        base.FromTreeAttributes(tree, worldForResolving);
        MeshAngle = tree.GetFloat("meshAngle", MeshAngle);
    }

    public override void ToTreeAttributes(ITreeAttribute tree)
    {
        base.ToTreeAttributes(tree);
        tree.SetFloat("meshAngle", MeshAngle);
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
    {
        tessThreadTesselator.TesselateBlock(Block, out MeshData mesh);
        Vec3f rotationOrigin = new(0.5f, 0.5f, 0.5f);
        mesh.Rotate(rotationOrigin, 0f, MeshAngle, 0f);
        mesher.AddMeshData(mesh);

        return true;
    }

    public void DropContents(IPlayer byPlayer)
    {
        if (Api is not ICoreServerAPI coreServerAPI) return;

        if (!Inventory.Empty)
        {
            StringBuilder stringBuilder = new($"{byPlayer?.PlayerName} broke container {Block.Code} at {Pos} dropped: ");
            foreach (ItemSlot item in Inventory)
            {
                if (item.Itemstack != null)
                {
                    stringBuilder.Append(item.Itemstack.StackSize).Append("x ").Append(item.Itemstack.Collectible?.Code).Append(", ");
                }
            }

            coreServerAPI.Logger.Audit(stringBuilder.ToString());
        }

        Inventory.DropAll(Pos.ToVec3d().Add(0.5, 0.5, 0.5));
    }

    public override void OnBlockBroken(IPlayer byPlayer = null)
    {
        //Base method would drop contents which is not always intended, that is now handled by the block.
        foreach (BlockEntityBehavior behavior in Behaviors)
        {
            behavior.OnBlockBroken(byPlayer);
        }
    }
}
