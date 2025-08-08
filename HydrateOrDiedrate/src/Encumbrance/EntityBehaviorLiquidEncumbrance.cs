using HydrateOrDiedrate.Config;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace HydrateOrDiedrate.encumbrance
{
    public class EntityBehaviorLiquidEncumbrance(Entity entity) : EntityBehavior(entity)
    {
        private int _tickCounter = 0;
        private float _currentPenaltyAmount = 0f;
        private bool _isPenaltyApplied = false;

        //TODO why not just bind on inventory slot changes?
        public override void OnGameTick(float deltaTime)
        {
            if (!entity.Alive || entity is not EntityPlayer player) return;

            var currentGameMode = player.Player?.WorldData?.CurrentGameMode;
            if (currentGameMode == EnumGameMode.Creative || !ModConfig.Instance.LiquidEncumbrance.Enabled)
            {
                RemoveMovementSpeedPenalty();
                return;
            }

            if (currentGameMode == EnumGameMode.Survival)
            {
                _tickCounter++;
                if (_tickCounter >= 30)
                {
                    CheckInventoryForEncumbrance();
                    _tickCounter = 0;
                }
            }
        }

        private void CheckInventoryForEncumbrance()
        {
            var player = entity as EntityPlayer;
            if (player?.Player == null) return;

            var inventoryManager = player.Player.InventoryManager;
            if (inventoryManager == null) return;

            bool isEncumbered = false;
            
            var backpackInventory = inventoryManager.GetOwnInventory("backpack");
            if (backpackInventory != null)
            {
                isEncumbered = CheckInventorySlots(backpackInventory);
            }
            
            var hotbarInventory = inventoryManager.GetOwnInventory("hotbar");
            if (!isEncumbered && hotbarInventory != null)
            {
                isEncumbered = CheckInventorySlots(hotbarInventory);
            }

            var mouseSlot = inventoryManager.MouseItemSlot;
            if (!isEncumbered && mouseSlot != null && mouseSlot.Itemstack != null)
            {
                if (mouseSlot.Itemstack.Block is BlockLiquidContainerBase)
                {
                    float totalLitres = GetTotalLitresInStack(mouseSlot.Itemstack);
                    if (totalLitres > ModConfig.Instance.LiquidEncumbrance.EncumbranceLimit)
                    {
                        isEncumbered = true;
                    }
                }
            }

            if (isEncumbered)
            {
                ApplyMovementSpeedPenalty(ModConfig.Instance.LiquidEncumbrance.EncumbranceMovementSpeedDebuff);
            }
            else
            {
                RemoveMovementSpeedPenalty();
            }
        }

        private bool CheckInventorySlots(IInventory inventory)
        {
            foreach (var slot in inventory)
            {
                if (slot?.Itemstack == null) continue;

                var attributes = slot.Itemstack.Collectible?.Attributes;
                if (attributes == null) continue;

                var isLiquidContainer = slot.Itemstack.Block is BlockLiquidContainerBase;
                if (isLiquidContainer)
                {
                    float totalLitresInStack = GetTotalLitresInStack(slot.Itemstack);

                    if (totalLitresInStack > ModConfig.Instance.LiquidEncumbrance.EncumbranceLimit)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private float GetTotalLitresInStack(ItemStack itemStack)
        {
            if (itemStack.Block is not BlockLiquidContainerBase block) return 0f;

            float litresPerContainer = block.GetCurrentLitres(itemStack);
            
            int stackSize = itemStack.StackSize;

            return litresPerContainer * stackSize;
        }
        private void ApplyMovementSpeedPenalty(float penaltyAmount)
        {
            if (_currentPenaltyAmount == penaltyAmount && _isPenaltyApplied) return;

            _currentPenaltyAmount = penaltyAmount;
            UpdateWalkSpeed();
            _isPenaltyApplied = true;
        }

        private void RemoveMovementSpeedPenalty()
        {
            if (_currentPenaltyAmount == 0f && !_isPenaltyApplied) return;

            _currentPenaltyAmount = 0f;
            UpdateWalkSpeed();
            _isPenaltyApplied = false;
        }

        private void UpdateWalkSpeed()
        {
            entity.Stats.Set("walkspeed", "liquidEncumbrancePenalty", -_currentPenaltyAmount, true);
        }

        public override string PropertyName() => "liquidencumbrance";

        public override void OnEntityDeath(DamageSource damageSourceForDeath)
        {
            base.OnEntityDeath(damageSourceForDeath);

            if(entity.Api is not ICoreServerAPI serverApi) return;
            
            //TODO: This code doesn't seem logical, figure out why it was added and refactor it.
            serverApi.Event.EnqueueMainThreadTask(() => 
            {
                if (!entity.HasBehavior<EntityBehaviorLiquidEncumbrance>())
                {
                    entity.AddBehavior(new EntityBehaviorLiquidEncumbrance(entity));
                }
            }, "AddLiquidEncumbranceBehavior");
        }
    }
}
