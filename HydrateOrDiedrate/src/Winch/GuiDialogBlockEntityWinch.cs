using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace HydrateOrDiedrate.winch;

public class GuiDialogBlockEntityWinch : GuiDialogBlockEntity
{
    protected override double FloatyDialogPosition => 0.75;

    public GuiDialogBlockEntityWinch(string DialogTitle, InventoryBase Inventory, BlockPos BlockEntityPosition, ICoreClientAPI capi) : base(DialogTitle, Inventory, BlockEntityPosition, capi)
    {
        if (IsDuplicate) return;

        capi.World.Player.InventoryManager.OpenInventory(Inventory);
        SetupDialog();
    }

    private void SetupDialog()
    {
        ElementBounds winchBounds = ElementBounds.Fixed(0.0, 0.0, 100.0, 100.0);
        ElementBounds inputSlotBounds = ElementStdBounds.SlotGrid(EnumDialogArea.CenterMiddle, 0.0, 15.0, 1, 1);
        ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
        bgBounds.BothSizing = ElementSizing.FitToChildren;
        bgBounds.WithChildren(winchBounds);

        ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog
            .WithAlignment(EnumDialogArea.CenterMiddle)
            .WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding, 0.0);

        ClearComposers();

        SingleComposer = capi.Gui.CreateCompo($"blockentitywinch{BlockEntityPosition}", dialogBounds)
            .AddShadedDialogBG(bgBounds, true, 5.0, 0.75f)
            .AddDialogTitleBar(DialogTitle, OnTitleBarClose, null, null)
                .BeginChildElements(bgBounds)
                .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[1], inputSlotBounds, "inputSlot")
                .EndChildElements()
                .Compose(true);
    }

    private void SendInvPacket(object p) => capi.Network.SendBlockEntityPacket(BlockEntityPosition.X, BlockEntityPosition.Y, BlockEntityPosition.Z, p);

    private void OnTitleBarClose() => TryClose();

    public override void OnGuiClosed()
    {
        SingleComposer.GetSlotGrid("inputSlot").OnGuiClosed(capi);
        base.OnGuiClosed();
    }
}