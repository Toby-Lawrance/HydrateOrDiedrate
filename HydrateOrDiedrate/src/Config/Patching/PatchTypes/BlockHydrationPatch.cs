using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace HydrateOrDiedrate.Config.Patching.PatchTypes;

public class BlockHydrationPatch : HydrationPatchBase
{
    [JsonProperty("BlockCode")]
    public override string Code { get; set; }

    [JsonProperty(Attributes.IsBoiling)]
    public bool IsBoiling { get; set; }

    [JsonProperty(Attributes.HungerReduction)]
    public int HungerReduction { get; set; }

    [JsonProperty(Attributes.Healing)]
    public int Healing { get; set; }

    public override void Apply(CollectibleObject collectible, float value)
    {

        //TODO: this really should just be: `collectible.Attributes.Token["hydration"] = value;` why does the original code have this weird logic?
        var hydrationByType = collectible.Attributes.Token["hydrationByType"] ??= new JObject();
        hydrationByType["*"] = value;

        collectible.Attributes.Token[Attributes.IsBoiling] = IsBoiling;
        collectible.Attributes.Token[Attributes.HungerReduction] = HungerReduction;
        collectible.Attributes.Token[Attributes.Healing] = Healing;
    }

    public static PatchCollection<BlockHydrationPatch> GenerateDefaultPatchCollection() => new()
    {
        Priority = 5,
        Patches = new BlockHydrationPatch[]
        {
            new BlockHydrationPatch
            {
                Code = "boilingwater*",
                Value = 600,
                IsBoiling = true
            },
            new BlockHydrationPatch
            {
                Code = "water*",
                Value = 600,
                HungerReduction = 100
            },
            new BlockHydrationPatch
            {
                Code = "wellwaterfresh*",
                Value = 750
            },
            new BlockHydrationPatch
            {
                Code = "wellwatersalt*",
                Value = -600,
                HungerReduction = 100
            },
            new BlockHydrationPatch
            {
                Code = "wellwatermuddy*",
                Value = 600,
                HungerReduction = 50
            },
            new BlockHydrationPatch
            {
                Code = "wellwatertainted*",
                Value = 750,
                HungerReduction = 400,
                Healing = -5
            },
            new BlockHydrationPatch
            {
                Code = "wellwaterpoisoned*",
                Value = 750,
                Healing = -20
            },
            new BlockHydrationPatch
            {
                Code = "wellwatermuddysalt*",
                Value = -600,
                HungerReduction = 50
            },
            new BlockHydrationPatch
            {
                Code = "wellwatertaintedsalt*",
                Value = -600,
                HungerReduction = 400,
                Healing = -5
            },
            new BlockHydrationPatch
            {
                Code = "wellwaterpoisonedsalt*",
                Value = -600,
                Healing = -20
            },
            new BlockHydrationPatch
            {
                Code = "saltwater*",
                Value = -600,
                HungerReduction = 100
            },
            new BlockHydrationPatch
            {
                Code = "distilledwater*",
                Value = 600
            }
        }
    };
}
