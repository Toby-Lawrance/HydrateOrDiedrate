using HydrateOrDiedrate.Config;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace HydrateOrDiedrate.Keg;

//TODO: The 2 keg blocks really should be variants of each other to better link them together (just don't feel like touching the remapper right now)
//TODO: Maybe remove the block interaction help that suggests we can take liquids if the block is not tapped
public class BlockKeg : BlockLiquidContainerBase
{
    public const float requiredActionTime = (100f / 50f) - 0.1f;
    public override float CapacityLitres => ModConfig.Instance.Containers.KegCapacityLitres;
    
    public override bool AllowHeldLiquidTransfer => false;
    public override bool CanDrinkFrom => false; 
    

    
    //TODO: Rework this... currently implementation can respond weirdly when multiple people try to do the interaction at the same time
    private float playNextSound = 0.7f;

    //Note: CanDrinkFrom does not fully prevent you from drinking, unsure why but this is why we are overiding the Eat methods
    protected override void tryEatBegin(ItemSlot slot, EntityAgent byEntity, ref EnumHandHandling handling, string eatSound = "eat", int eatSoundRepeats = 1) => handling = EnumHandHandling.PreventDefaultAction;
    
    protected override bool tryEatStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, ItemStack spawnParticleStack = null) => false;
    
    protected override void tryEatStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity)
    {
        //Not edible
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        if (byPlayer.InventoryManager.ActiveTool == EnumTool.Axe)
        {
            playNextSound = 0.7f;

            KegAnimations.StartAxeAnimation(byPlayer.Entity.AnimManager);
            KegAnimations.PlayChoppingSound(world, byPlayer, blockSel);
            return true;
        }
        
        if(Code.Path != "kegtapped")
        {
            ItemStack heldItem = byPlayer.InventoryManager.ActiveHotbarSlot?.Itemstack;
            if (heldItem?.Collectible is ItemKegTap && byPlayer.Entity.LeftHandItemSlot?.Itemstack?.Collectible?.Tool == EnumTool.Hammer)
            {
                playNextSound = 0.7f;
                KegAnimations.StartTappingAnimation(byPlayer.Entity.AnimManager);
                KegAnimations.PlayTappingSound(world, byPlayer, blockSel);
                return true;
            }

            //Side path for when the block is not tapped and we are interacting with an item that can accept liquids
            if(heldItem?.Collectible is ILiquidSink liquidSink)
            {
                //Return since since we can't take any more (which means it might try taking liquid if liquidSink can accept liquids and is not full)
                if(IsFull(blockSel.Position)) return false;

                //Return if LiquidSink can also provides liquid but it currently does not hold any, to prevent it from taking instead
                if(liquidSink is ILiquidSource && liquidSink.GetCurrentLitres(heldItem) == 0) return false;
            }
        }
        
        return base.OnBlockInteractStart(world, byPlayer, blockSel);
    }

    public override bool OnBlockInteractStep(float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        if (byPlayer.InventoryManager.ActiveTool == EnumTool.Axe)
        {
            if (secondsUsed >= playNextSound)
            {
                KegAnimations.PlayChoppingSound(world, byPlayer, blockSel);
                playNextSound += 0.65f;
            }

            return secondsUsed < requiredActionTime;
        }

        if (Code.Path != "kegtapped" && byPlayer.InventoryManager.ActiveHotbarSlot?.Itemstack?.Collectible is ItemKegTap && byPlayer.Entity.LeftHandItemSlot?.Itemstack?.Collectible?.Tool == EnumTool.Hammer)
        {
            if (secondsUsed >= playNextSound)
            {
                KegAnimations.PlayTappingSound(world, byPlayer, blockSel);
                playNextSound += 0.65f;
            }

            return secondsUsed < requiredActionTime;
        }

        return base.OnBlockInteractStep(secondsUsed, world, byPlayer, blockSel);
    }

    public override void OnBlockInteractStop(float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        //Finish chopping action
        if(byPlayer.InventoryManager.ActiveTool == EnumTool.Axe)
        {
            if(secondsUsed >= requiredActionTime)
            {
                world.BlockAccessor.SetBlock(0, blockSel.Position);
                DropKegItems(world, blockSel.Position);
            }

            KegAnimations.StopAxeAnimation(byPlayer.Entity.AnimManager);
            return;
        }

        //finish tapping animation
        if (Code.Path != "kegtapped" && byPlayer.InventoryManager.ActiveHotbarSlot?.Itemstack?.Collectible is ItemKegTap && byPlayer.Entity.LeftHandItemSlot?.Itemstack?.Collectible?.Tool == EnumTool.Hammer)
        {
            if(secondsUsed >= requiredActionTime)
            {
                Block tappedKegBlock = world.GetBlock(new AssetLocation("hydrateordiedrate", "kegtapped"));

                //Exchange block so that BlockEntity does not get removed
                world.BlockAccessor.ExchangeBlock(tappedKegBlock.BlockId, blockSel.Position);

                ItemSlot activeHotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;
                if (activeHotbarSlot.Itemstack?.Collectible is ItemKegTap)
                {
                    activeHotbarSlot.TakeOut(1);
                    activeHotbarSlot.MarkDirty();
                }

            }

            KegAnimations.StopTappingAnimation(byPlayer.Entity.AnimManager);
            return;
        }
        
        base.OnBlockInteractStop(secondsUsed, world, byPlayer, blockSel);
    }

    public override bool OnBlockInteractCancel(float secondsUsed, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, EnumItemUseCancelReason cancelReason)
    {
        if(base.OnBlockInteractCancel(secondsUsed,world, byPlayer, blockSel, cancelReason))
        {
            KegAnimations.StopAllAnimations(byPlayer.Entity.AnimManager);
            return true;
        }

        return false;
    }

    private void DropKegItems(IWorldAccessor world, BlockPos position)
    {
        var random = world.Rand;
        for (int i = 0; i < 2; i++)
        {
            if (random.NextDouble() < ModConfig.Instance.Containers.KegIronHoopDropChance)
            {
                world.SpawnItemEntity(new ItemStack(world.GetItem(new AssetLocation("game","hoop-iron"))), position.ToVec3d());
            }
        }

        if (Code.Path == "kegtapped" && random.NextDouble() < ModConfig.Instance.Containers.KegTapDropChance)
        {
            world.SpawnItemEntity(new ItemStack(world.GetItem(new AssetLocation("hydrateordiedrate", "kegtap"))), position.ToVec3d());
        }
    }

    public override bool DoPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ItemStack byItemStack)
    {
        bool flag = base.DoPlaceBlock(world, byPlayer, blockSel, byItemStack);

        if (flag && world.BlockAccessor.GetBlockEntity(blockSel.Position) is BlockEntityKeg blockEntityKeg)
        {
            float playerYaw = byPlayer.Entity.Pos.Yaw;
            float snappedYaw = (float)(Math.Round(playerYaw / Constants.SnapAngle) * Constants.SnapAngle);
            blockEntityKeg.MeshAngle = snappedYaw;
            blockEntityKeg.MarkDirty(true, null);
        }

        return flag;
    }

    public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
    {
        var beh = world.BlockAccessor.GetBlockEntity<BlockEntityKeg>(pos);
        if(beh is not null)
        {
            var contentCode = beh.GetContent()?.Collectible.Code;

            //Drop content if we don't drop with liquid or if the content has become rotten
            if(!ModConfig.Instance.Containers.KegDropWithLiquid || (contentCode is not null && contentCode.Domain == "game" && contentCode.Path == "rot"))
            {
                beh.DropContents(byPlayer);
            }
        }

        base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
    }
}
