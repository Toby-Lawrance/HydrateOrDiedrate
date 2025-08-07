using System;
using System.Linq;
using HarmonyLib;
using HydrateOrDiedrate.Commands;
using HydrateOrDiedrate.Config;
using HydrateOrDiedrate.encumbrance;
using HydrateOrDiedrate.Hot_Weather;
using HydrateOrDiedrate.HUD;
using HydrateOrDiedrate.Keg;
using HydrateOrDiedrate.patches;
using HydrateOrDiedrate.wellwater;
using HydrateOrDiedrate.winch;
using HydrateOrDiedrate.XSkill;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;
using HydrateOrDiedrate.Config.Patching;
using HydrateOrDiedrate.Config.Patching.PatchTypes;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;
using System.Collections.Generic;
using Vintagestory.API.Datastructures;
using Newtonsoft.Json.Linq;

namespace HydrateOrDiedrate;

public class HydrateOrDiedrateModSystem : ModSystem
{
    public const string HarmonyID = "com.chronolegionnaire.hydrateordiedrate";
    public const string NetworkChannelID = "hydrateordiedrate";

    private ICoreServerAPI _serverApi;
    private ICoreClientAPI _clientApi;

    private HudElementThirstBar _thirstHud;
    private HudElementHungerReductionBar _hungerReductionHud;
    private WaterInteractionHandler _waterInteractionHandler;
    private Harmony harmony;
    public static AquiferManager AquiferManager { get; private set; }

    private RainHarvesterManager rainHarvesterManager;
    private DrinkHudOverlayRenderer hudOverlayRenderer;

    private long customHudListenerId;
    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        ConfigManager.EnsureModConfigLoaded(api);

        if (!Harmony.HasAnyPatches(HarmonyID))
        {
            harmony = new Harmony(HarmonyID);
            harmony.PatchAll();
        }
    }

    public override void AssetsLoaded(ICoreAPI api)
    {
        base.AssetsLoaded(api);

        if(api.Side == EnumAppSide.Client) return; //This data is decided by the server and synced over to client automatically
        
        WaterPatches.PrepareWaterSatietyPatches(api);
        WaterPatches.PrepareWellWaterSatietyPatches(api);
        WaterPatches.PrepareWaterPerishPatches(api);
        WaterPatches.PrepareWellWaterPerishPatches(api);
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        base.AssetsFinalize(api);
        if(api.Side == EnumAppSide.Client) return; //This data is decided by the server and synced over to client automatically
        
        EntityProperties playerEntity = api.World.GetEntityType(new AssetLocation("game", "player"));
        var HoDbehaviors = new List<JsonObject>(3);

        //Forcibly insert behaviors to ensure they are present //TODO most of these are only really needed for th server but some are on client as well for now for accessibility
        if (ModConfig.Instance.LiquidEncumbrance.Enabled) HoDbehaviors.Add(new(new JObject { ["code"] =  "liquidencumbrance" }));
        if (ModConfig.Instance.Thirst.Enabled) HoDbehaviors.Add(new(new JObject { ["code"] =  "thirst" }));
        if (ModConfig.Instance.HeatAndCooling.HarshHeat) HoDbehaviors.Add(new(new JObject { ["code"] =  "bodytemperaturehot" }));

        playerEntity.Server.BehaviorsAsJsonObj = playerEntity.Server.BehaviorsAsJsonObj.Append(HoDbehaviors.ToArray());
        
        playerEntity.Client.BehaviorsAsJsonObj = playerEntity.Client.BehaviorsAsJsonObj.Append(HoDbehaviors.ToArray());

        //TODO does this even do anything when HarshHeat is disabled?
        PatchCollection<CoolingPatch>.GetMerged(api, "HoD.AddCooling.json", CoolingPatch.GenerateDefaultPatchCollection()).ApplyPatches(api.World.Items);

        if (ModConfig.Instance.Thirst.Enabled)
        {
            PatchCollection<ItemHydrationPatch>.GetMerged(api, "HoD.AddItemHydration.json", ItemHydrationPatch.GenerateDefaultPatchCollection()).ApplyPatches(api.World.Items);
            
            PatchCollection<BlockHydrationPatch>.GetMerged(api, "HoD.AddBlockHydration.json", BlockHydrationPatch.GenerateDefaultPatchCollection()).ApplyPatches(api.World.Blocks);
        }

        foreach (var block in api.World.Blocks)
        {
            if (block is BlockLiquidContainerTopOpened || block is BlockBarrel || block is BlockGroundStorage)
            {
                EnsureRainHarvesterBehaviorPresent(block);
            }
        }
    }

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        
        api.RegisterBlockClass("BlockKeg", typeof(BlockKeg));
        api.RegisterBlockEntityClass("BlockEntityKeg", typeof(BlockEntityKeg));
        api.RegisterItemClass("ItemKegTap", typeof(ItemKegTap));
        
        api.RegisterBlockClass("BlockTun", typeof(BlockTun));
        api.RegisterBlockEntityClass("BlockEntityTun", typeof(BlockEntityTun));
        
        api.RegisterCollectibleBehaviorClass("BehaviorPickaxeWellMode", typeof(BehaviorPickaxeWellMode));
        api.RegisterCollectibleBehaviorClass("BehaviorShovelWellMode", typeof(BehaviorShovelWellMode));

        api.RegisterBlockEntityClass("BlockEntityWellWaterData", typeof(BlockEntityWellWaterData));
        api.RegisterBlockBehaviorClass("BlockBehaviorWellWaterFinite", typeof(BlockBehaviorWellWaterFinite));
        api.RegisterBlockClass("BlockWellSpring", typeof(BlockWellSpring));
        api.RegisterBlockEntityClass("BlockEntityWellSpring", typeof(BlockEntityWellSpring));
        
        api.RegisterBlockClass("BlockWinch", typeof(BlockWinch));
        api.RegisterBlockEntityClass("BlockEntityWinch", typeof(BlockEntityWinch));

        if (ModConfig.Instance.LiquidEncumbrance.Enabled) api.RegisterEntityBehaviorClass("liquidencumbrance", typeof(EntityBehaviorLiquidEncumbrance));
        if (ModConfig.Instance.Thirst.Enabled) api.RegisterEntityBehaviorClass("thirst", typeof(EntityBehaviorThirst));
        if (ModConfig.Instance.HeatAndCooling.HarshHeat) api.RegisterEntityBehaviorClass("bodytemperaturehot", typeof(EntityBehaviorBodyTemperatureHot)); //TODO does this even do anything when thirst is disabled?

        _waterInteractionHandler = new WaterInteractionHandler(api);

        if (api.ModLoader.IsModEnabled("xlib") || api.ModLoader.IsModEnabled("xlibpatch"))
        {
            XLibSkills.Initialize(api);
        }

        api.RegisterBlockEntityBehaviorClass("RainHarvester", typeof(RegisterRainHarvester));

        if (api.ModLoader.IsModEnabled("BetterProspecting"))
        {
            BetterProspectingAquiferPatch.Apply(api);
        }
        if (api.ModLoader.IsModEnabled("smoothdigestion"))
        {
            EntityBehaviorSDHungerPatch.Apply(api);
        }
    }
    
    public override void StartServerSide(ICoreServerAPI api)
    {
        _serverApi = api;
        base.StartServerSide(api);

        var serverChannel = api.Network.RegisterChannel(NetworkChannelID)
            .RegisterMessageType<DrinkProgressPacket>()
            .RegisterMessageType<WellSpringBlockPacket>()
            .SetMessageHandler<WellSpringBlockPacket>(WellSpringBlockPacketReceived);
        
        AquiferManager = new AquiferManager(api);
        _waterInteractionHandler.Initialize(serverChannel);
        rainHarvesterManager = new RainHarvesterManager(_serverApi);
        api.Event.PlayerDisconnect += _waterInteractionHandler.OnPlayerDisconnect;
        api.Event.RegisterGameTickListener(CheckPlayerInteraction, 100);

        ThirstCommands.Register(api);
        AquiferCommands.Register(api);
    }


    public override void StartClientSide(ICoreClientAPI api)
    {
        _clientApi = api;
        base.StartClientSide(api);

        api.Network.RegisterChannel(NetworkChannelID)
            .RegisterMessageType<DrinkProgressPacket>()
            .RegisterMessageType<WellSpringBlockPacket>()
            .SetMessageHandler<DrinkProgressPacket>(OnDrinkProgressReceived);

        hudOverlayRenderer = new DrinkHudOverlayRenderer(api);
        api.Event.RegisterRenderer(hudOverlayRenderer, EnumRenderStage.Ortho, "drinkoverlay");
        if (ModConfig.Instance.Thirst.Enabled)
        {
            customHudListenerId = api.Event.RegisterGameTickListener(CheckAndInitializeCustomHud, 20);
        }
        if(api.ModLoader.IsModEnabled("configlib")) ConfigLibCompatibility.Init(api);
    }

    public RainHarvesterManager GetRainHarvesterManager()
    {
        return rainHarvesterManager;
    }

    private void OnDrinkProgressReceived(DrinkProgressPacket msg)
    {
        if (hudOverlayRenderer == null) return;
        hudOverlayRenderer.ProcessDrinkProgress(msg.Progress, msg.IsDrinking, msg.IsDangerous);
    }


    //TODO: there should be a better way to do this, no?
    private void CheckAndInitializeCustomHud(float dt)
    {
        var vanillaHudStatbar = GetVanillaStatbarHud();

        if (vanillaHudStatbar != null && vanillaHudStatbar.IsOpened())
        {

            _thirstHud = new HudElementThirstBar(_clientApi);
            _clientApi.Event.RegisterGameTickListener(_thirstHud.OnGameTick, 1000);
            _clientApi.Gui.RegisterDialog(_thirstHud);

            _hungerReductionHud = new HudElementHungerReductionBar(_clientApi);
            _clientApi.Event.RegisterGameTickListener(_hungerReductionHud.OnGameTick, 1000);
            _clientApi.Gui.RegisterDialog(_hungerReductionHud);

            _clientApi.Event.UnregisterGameTickListener(customHudListenerId);
        }
    }

    private HudStatbar GetVanillaStatbarHud()
    {
        foreach (var hud in _clientApi.Gui.OpenedGuis)
        {
            if (hud is HudStatbar statbar)
            {
                return statbar;
            }
        }

        return null;
    }

    public void CheckPlayerInteraction(float dt)
    {
        foreach (IServerPlayer player in _serverApi.World.AllOnlinePlayers)
        {
            if(player.ConnectionState != EnumClientState.Playing) continue;
            _waterInteractionHandler.CheckShiftRightClickBeforeInteractionForPlayer(dt, player);
        }
    }

    private static void EnsureRainHarvesterBehaviorPresent(Block block)
    {
        block.BlockEntityBehaviors ??= Array.Empty<BlockEntityBehaviorType>();

        if (Array.Exists(block.BlockEntityBehaviors, b => b.Name == "RainHarvester")) return;
        
        block.BlockEntityBehaviors = block.BlockEntityBehaviors.Append(new BlockEntityBehaviorType
        {
            Name = "RainHarvester",
            properties = null
        });
    }

    private void WellSpringBlockPacketReceived(IServerPlayer sender, WellSpringBlockPacket packet)
    {
        if (_serverApi?.World == null) return;
        IBlockAccessor accessor = _serverApi.World.GetBlockAccessor(true, true, false);
        accessor.ExchangeBlock(packet.BlockId, packet.Position);
        accessor.SpawnBlockEntity("BlockEntityWellSpring", packet.Position, null);
    }
    public override void Dispose()
    {
        _thirstHud?.Dispose();
        _hungerReductionHud?.Dispose();
        
        ConfigManager.UnloadModConfig();
        harmony?.UnpatchAll(HarmonyID);

        base.Dispose();
    }
}