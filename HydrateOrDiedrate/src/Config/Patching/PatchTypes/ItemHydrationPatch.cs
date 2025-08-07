using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;

namespace HydrateOrDiedrate.Config.Patching.PatchTypes;

public class ItemHydrationPatch : HydrationPatchBase
{
    [JsonProperty("ItemName")]
    public override string Code { get; set; }

    public override void Apply(CollectibleObject collectible, float value)
    {
        collectible.Attributes.Token[Attributes.Hydration] = value;
    }

    public static PatchCollection<ItemHydrationPatch> GenerateDefaultPatchCollection() => new()
    {
        Priority = 5,
        Patches = new ItemHydrationPatch[]
        {
            new()
            {
                Code = "game:juiceportion-*",
                ValueByType = new()
                {
                    ["*"] = 750,
                    ["game:juiceportion-cranberry"] = 600,
                    ["game:juiceportion-blueberry"] = 700,
                    ["game:juiceportion-pinkapple"] = 750,
                    ["game:juiceportion-lychee"] = 850,
                    ["game:juiceportion-redcurrant"] = 800,
                    ["game:juiceportion-breadfruit"] = 500,
                    ["game:juiceportion-pineapple"] = 950,
                    ["game:juiceportion-blackcurrant"] = 800,
                    ["game:juiceportion-saguaro"] = 600,
                    ["game:juiceportion-whitecurrant"] = 800,
                    ["game:juiceportion-redapple"] = 900,
                    ["game:juiceportion-yellowapple"] = 900,
                    ["game:juiceportion-apple"] = 900,
                    ["game:juiceportion-cherry"] = 800,
                    ["game:juiceportion-peach"] = 950,
                    ["game:juiceportion-pear"] = 950,
                    ["game:juiceportion-orange"] = 1000,
                    ["game:juiceportion-mango"] = 950,
                    ["game:juiceportion-pomegranate"] = 850,
                    ["game:juiceportion-redgrapes"] = 950,
                    ["game:juiceportion-greengrapes"] = 950
                }
            },
            new()
            {
                Code = "game:fruit-*",
                ValueByType = new()
                {
                    ["*"] = 40,
                    ["game:fruit-cranberry"] = 40,
                    ["game:fruit-blueberry"] = 40,
                    ["game:fruit-pinkapple"] = 30,
                    ["game:fruit-lychee"] = 30,
                    ["game:fruit-redcurrant"] = 40,
                    ["game:fruit-breadfruit"] = 5,
                    ["game:fruit-pineapple"] = 35,
                    ["game:fruit-blackcurrant"] = 40,
                    ["game:fruit-saguaro"] = 15,
                    ["game:fruit-whitecurrant"] = 40,
                    ["game:fruit-redapple"] = 30,
                    ["game:fruit-yellowapple"] = 30,
                    ["game:fruit-cherry"] = 40,
                    ["game:fruit-peach"] = 50,
                    ["game:fruit-pear"] = 35,
                    ["game:fruit-orange"] = 35,
                    ["game:fruit-mango"] = 50,
                    ["game:fruit-pomegranate"] = 40,
                    ["game:fruit-redgrapes"] = 40,
                    ["game:fruit-greengrapes"] = 40
                }
            },
            new()
            {
                Code = "game:bambooshoot",
                Value = 15
            },
            new()
            {
                Code = "game:bread-*",
                ValueByType = new()
                {
                    ["*"] = -50,
                    ["game:bread-spelt"] = -50,
                    ["game:bread-rye"] = -50,
                    ["game:bread-flax"] = -40,
                    ["game:bread-rice"] = -60,
                    ["game:bread-cassava"] = -50,
                    ["game:bread-amaranth"] = -40,
                    ["game:bread-sunflower"] = -50,
                    ["game:bread-spelt-partbaked"] = -40,
                    ["game:bread-rye-partbaked"] = -40,
                    ["game:bread-flax-partbaked"] = -35,
                    ["game:bread-rice-partbaked"] = -50,
                    ["game:bread-cassava-partbaked"] = -40,
                    ["game:bread-amaranth-partbaked"] = -35,
                    ["game:bread-sunflower-partbaked"] = -40,
                    ["game:bread-spelt-perfect"] = -50,
                    ["game:bread-rye-perfect"] = -50,
                    ["game:bread-flax-perfect"] = -40,
                    ["game:bread-rice-perfect"] = -60,
                    ["game:bread-cassava-perfect"] = -50,
                    ["game:bread-amaranth-perfect"] = -40,
                    ["game:bread-sunflower-perfect"] = -50,
                    ["game:bread-spelt-charred"] = -80,
                    ["game:bread-rye-charred"] = -100,
                    ["game:bread-flax-charred"] = -70,
                    ["game:bread-rice-charred"] = -80,
                    ["game:bread-cassava-charred"] = -80,
                    ["game:bread-amaranth-charred"] = -70,
                    ["game:bread-sunflower-charred"] = -80
                }
            },
            new()
            {
                Code = "game:bushmeat-*",
                ValueByType = new()
                {
                    ["*"] = -80,
                    ["game:bushmeat-cooked"] = -70,
                    ["game:bushmeat-cured"] = -90
                }
            },
            new()
            {
                Code = "game:butter",
                Value = -80
            },
            new()
            {
                Code = "game:cheese-*",
                ValueByType = new()
                {
                    ["*"] = -50,
                    ["game:cheese-blue-1slice"] = -60,
                    ["game:cheese-cheddar-1slice"] = -45
                }
            },
            new()
            {
                Code = "game:dough-*",
                ValueByType = new()
                {
                    ["*"] = 5,
                    ["game:dough-spelt"] = 5,
                    ["game:dough-rye"] = 5,
                    ["game:dough-flax"] = 5,
                    ["game:dough-rice"] = 5,
                    ["game:dough-cassava"] = 5,
                    ["game:dough-amaranth"] = 5,
                    ["game:dough-sunflower"] = 5
                }
            },
            new()
            {
                Code = "game:fish-*",
                ValueByType = new()
                {
                    ["*"] = -5,
                    ["game:fish-raw"] = 15,
                    ["game:fish-cooked"] = -20,
                    ["game:fish-cured"] = -70,
                    ["game:fish-smoked"] = -80,
                    ["game:fish-cured-smoked"] = -120
                }
            },
            new()
            {
                Code = "game:grain-*",
                ValueByType = new()
                {
                    ["*"] = -30
                }
            },
            new()
            {
                Code = "game:insect-*",
                ValueByType = new()
                {
                    ["*"] = 10
                }
            },
            new()
            {
                Code = "game:legume-*",
                ValueByType = new()
                {
                    ["*"] = -30
                }
            },
            new()
            {
                Code = "game:pemmican-*",
                ValueByType = new()
                {
                    ["*"] = -150,
                    ["game:pemmican-raw-basic"] = -75,
                    ["game:pemmican-raw-salted"] = -120,
                    ["game:pemmican-dried-basic"] = -150,
                    ["game:pemmican-dried-salted"] = -200
                }
            },
            new()
            {
                Code = "game:pickledlegume-soybean",
                Value = 20
            },
            new()
            {
                Code = "game:pickledvegetable-*",
                ValueByType = new()
                {
                    ["*"] = 30,
                    ["game:pickledvegetable-carrot"] = 30,
                    ["game:pickledvegetable-cabbage"] = 30,
                    ["game:pickledvegetable-onion"] = 10,
                    ["game:pickledvegetable-turnip"] = 15,
                    ["game:pickledvegetable-parsnip"] = 10,
                    ["game:pickledvegetable-pumpkin"] = 40,
                    ["game:pickledvegetable-bellpepper"] = 50,
                    ["game:pickledvegetable-olive"] = 10
                }
            },
            new()
            {
                Code = "game:poultry-*",
                ValueByType = new()
                {
                    ["*"] = -40,
                    ["game:poultry-cooked"] = -50,
                    ["game:poultry-cured"] = -70
                }
            },
            new()
            {
                Code = "game:redmeat-*",
                ValueByType = new()
                {
                    ["*"] = -50,
                    ["game:redmeat-cooked"] = -100,
                    ["game:redmeat-vintage"] = -150,
                    ["game:redmeat-cured"] = -150
                }
            },
            new()
            {
                Code = "game:vegetable-*",
                ValueByType = new()
                {
                    ["*"] = 20,
                    ["game:vegetable-carrot"] = 20,
                    ["game:vegetable-cabbage"] = 40,
                    ["game:vegetable-onion"] = 20,
                    ["game:vegetable-turnip"] = 15,
                    ["game:vegetable-parsnip"] = 15,
                    ["game:vegetable-cookedcattailroot"] = 100,
                    ["game:vegetable-pumpkin"] = 20,
                    ["game:vegetable-cassava"] = 200,
                    ["game:vegetable-cookedpapyrusroot"] = 25,
                    ["game:vegetable-bellpepper"] = 30,
                    ["game:vegetable-olive"] = 15
                }
            },
            new()
            {
                Code = "game:alcoholportion",
                Value = -40
            },
            new()
            {
                Code = "game:boilingwaterportion",
                Value = 600
            },
            new()
            {
                Code = "hydrateordiedrate:boiledwaterportion",
                Value = 600
            },
            new()
            {
                Code = "hydrateordiedrate:boiledrainwaterportion",
                Value = 800
            },
            new()
            {
                Code = "hydrateordiedrate:distilledwaterportion",
                Value = 900
            },
            new()
            {
                Code = "hydrateordiedrate:rainwaterportion",
                Value = 750
            },
            new()
            {
                Code = "game:ciderportion-*",
                ValueByType = new()
                {
                    ["*"] = 500,
                    ["game:ciderportion-cranberry"] = 450,
                    ["game:ciderportion-blueberry"] = 500,
                    ["game:ciderportion-pinkapple"] = 500,
                    ["game:ciderportion-lychee"] = 550,
                    ["game:ciderportion-redcurrant"] = 500,
                    ["game:ciderportion-breadfruit"] = 250,
                    ["game:ciderportion-pineapple"] = 500,
                    ["game:ciderportion-blackcurrant"] = 450,
                    ["game:ciderportion-saguaro"] = 300,
                    ["game:ciderportion-whitecurrant"] = 550,
                    ["game:ciderportion-redapple"] = 500,
                    ["game:ciderportion-yellowapple"] = 500,
                    ["game:ciderportion-cherry"] = 400,
                    ["game:ciderportion-peach"] = 475,
                    ["game:ciderportion-pear"] = 525,
                    ["game:ciderportion-orange"] = 400,
                    ["game:ciderportion-mango"] = 525,
                    ["game:ciderportion-pomegranate"] = 425,
                    ["game:ciderportion-apple"] = 500,
                    ["game:ciderportion-mead"] = 400,
                    ["game:ciderportion-spelt"] = 450,
                    ["game:ciderportion-rice"] = 450,
                    ["game:ciderportion-rye"] = 450,
                    ["game:ciderportion-amaranth"] = 450,
                    ["game:ciderportion-cassava"] = 450,
                    ["game:ciderportion-redgrapes"] = 550,
                    ["game:ciderportion-greengrapes"] = 550
                }
            },
            new()
            {
                Code = "game:honeyportion",
                Value = -75
            },
            new()
            {
                Code = "game:jamhoneyportion",
                Value = -60
            },
            new()
            {
                Code = "game:saltwaterportion",
                Value = -600
            },
            new()
            {
                Code = "game:vinegarportion",
                Value = -50
            },
            new()
            {
                Code = "game:cottagecheeseportion",
                Value = 50
            },
            new()
            {
                Code = "game:milkportion",
                Value = 800
            },
            new()
            {
                Code = "game:waterportion",
                Value = 600
            },
            new()
            {
                Code = "hydrateordiedrate:wellwaterportion-fresh",
                Value = 750
            },
            new()
            {
                Code = "hydrateordiedrate:wellwaterportion-salt",
                Value = -600
            },
            new()
            {
                Code = "hydrateordiedrate:wellwaterportion-muddy",
                Value = 600
            },
            new()
            {
                Code = "hydrateordiedrate:wellwaterportion-tainted",
                Value = 750
            },
            new()
            {
                Code = "hydrateordiedrate:wellwaterportion-poisoned",
                Value = 750
            },
            new()
            {
                Code = "hydrateordiedrate:wellwaterportion-muddysalt",
                Value = -600
            },
            new()
            {
                Code = "hydrateordiedrate:wellwaterportion-taintedsalt",
                Value = -600
            },
            new()
            {
                Code = "hydrateordiedrate:wellwaterportion-poisonedsalt",
                Value = -600
            },
            new()
            {
                Code = "game:mushroom-*",
                ValueByType = new()
                {
                    ["*"] = 0,
                    ["game:mushroom-flyagaric"] = -25,
                    ["game:mushroom-earthball"] = -30,
                    ["game:mushroom-deathcap"] = -35,
                    ["game:mushroom-elfinsaddle"] = -30,
                    ["game:mushroom-jackolantern"] = -25,
                    ["game:mushroom-devilbolete"] = -80,
                    ["game:mushroom-bitterbolete"] = -15,
                    ["game:mushroom-devilstooth"] = -25,
                    ["game:mushroom-golddropmilkcap"] = -20,
                    ["game:mushroom-beardedtooth"] = 10,
                    ["game:mushroom-whiteoyster"] = 10,
                    ["game:mushroom-pinkoyster"] = 10,
                    ["game:mushroom-dryadsaddle"] = 10,
                    ["game:mushroom-tinderhoof"] = 10,
                    ["game:mushroom-chickenofthewoods"] = 10,
                    ["game:mushroom-reishi"] = 5,
                    ["game:mushroom-funeralbell"] = -20,
                    ["game:mushroom-livermushroom"] = 10,
                    ["game:mushroom-pinkbonnet"] = -25,
                    ["game:mushroom-shiitake"] = 5,
                    ["game:mushroom-deerear"] = 5
                }
            },
            new()
            {
                Code = "game:spiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 350,
                    ["game:spiritportion-cranberry"] = 320,
                    ["game:spiritportion-blueberry"] = 360,
                    ["game:spiritportion-pinkapple"] = 380,
                    ["game:spiritportion-lychee"] = 440,
                    ["game:spiritportion-redcurrant"] = 350,
                    ["game:spiritportion-breadfruit"] = 230,
                    ["game:spiritportion-pineapple"] = 400,
                    ["game:spiritportion-blackcurrant"] = 340,
                    ["game:spiritportion-saguaro"] = 260,
                    ["game:spiritportion-whitecurrant"] = 370,
                    ["game:spiritportion-redapple"] = 400,
                    ["game:spiritportion-yellowapple"] = 400,
                    ["game:spiritportion-cherry"] = 330,
                    ["game:spiritportion-peach"] = 400,
                    ["game:spiritportion-pear"] = 450,
                    ["game:spiritportion-orange"] = 370,
                    ["game:spiritportion-mango"] = 450,
                    ["game:spiritportion-pomegranate"] = 390,
                    ["game:spiritportion-apple"] = 400,
                    ["game:spiritportion-mead"] = 200,
                    ["game:spiritportion-spelt"] = 225,
                    ["game:spiritportion-rice"] = 225,
                    ["game:spiritportion-rye"] = 225,
                    ["game:spiritportion-amaranth"] = 225,
                    ["game:spiritportion-cassava"] = 225,
                    ["game:spiritportion-redgrapes"] = 380,
                    ["game:spiritportion-greengrapes"] = 380
                }
            },
            new()
            {
                Code = "alchemy:potionportion-*",
                ValueByType = new()
                {
                    ["*"] = -200,
                    ["alchemy:potionportion-all-medium"] = -100,
                    ["alchemy:potionportion-all-strong"] = -300,
                    ["alchemy:potionportion-alltick-medium"] = -100,
                    ["alchemy:potionportion-alltick-strong"] = -300,
                    ["alchemy:potionportion-archer-medium"] = -100,
                    ["alchemy:potionportion-archer-strong"] = -300,
                    ["alchemy:potionportion-healingeffect-medium"] = -100,
                    ["alchemy:potionportion-healingeffect-strong"] = -300,
                    ["alchemy:potionportion-hungerenhance-medium"] = -100,
                    ["alchemy:potionportion-hungerenhance-strong"] = -300,
                    ["alchemy:potionportion-hungersupress-medium"] = -100,
                    ["alchemy:potionportion-hungersupress-strong"] = -300,
                    ["alchemy:potionportion-hunter-medium"] = -100,
                    ["alchemy:potionportion-hunter-strong"] = -300,
                    ["alchemy:potionportion-looter-medium"] = -100,
                    ["alchemy:potionportion-looter-strong"] = -300,
                    ["alchemy:potionportion-melee-medium"] = -100,
                    ["alchemy:potionportion-melee-strong"] = -300,
                    ["alchemy:potionportion-mining-medium"] = -100,
                    ["alchemy:potionportion-mining-strong"] = -300,
                    ["alchemy:potionportion-poison-medium"] = -100,
                    ["alchemy:potionportion-poison-strong"] = -300,
                    ["alchemy:potionportion-predator-medium"] = -100,
                    ["alchemy:potionportion-predator-strong"] = -300,
                    ["alchemy:potionportion-regen-medium"] = -100,
                    ["alchemy:potionportion-regen-strong"] = -300,
                    ["alchemy:potionportion-scentmask-medium"] = -100,
                    ["alchemy:potionportion-scentmask-strong"] = -300,
                    ["alchemy:potionportion-speed-medium"] = -100,
                    ["alchemy:potionportion-speed-strong"] = -300,
                    ["alchemy:potionportion-vitality-medium"] = -100,
                    ["alchemy:potionportion-vitality-strong"] = -300
                }
            },
            new()
            {
                Code = "alchemy:potionteaportion",
                Value = 300
            },
            new()
            {
                Code = "alchemy:utilitypotionportion-*",
                ValueByType = new()
                {
                    ["*"] = -200,
                    ["alchemy:utilitypotionportion-recall"] = -200,
                    ["alchemy:utilitypotionportion-glow"] = -200,
                    ["alchemy:utilitypotionportion-waterbreathe"] = -200,
                    ["alchemy:utilitypotionportion-nutrition"] = -200,
                    ["alchemy:utilitypotionportion-temporal"] = -200
                }
            },
            new()
            {
                Code = "butchering:bloodportion",
                Value = 50
            },
            new()
            {
                Code = "butcher:smoked-*",
                ValueByType = new()
                {
                    ["*"] = -50,
                    ["butcher:smoked-none-redmeat"] = -60,
                    ["butcher:smoked-cured-redmeat"] = -80,
                    ["butcher:smoked-none-bushmeat"] = -50,
                    ["butcher:smoked-cured-bushmeat"] = -70,
                    ["butcher:smoked-none-fish"] = -50,
                    ["butcher:smoked-cured-fish"] = -80,
                    ["butcher:smoked-none-primemeat"] = -50,
                    ["butcher:smoked-healing-primemeat"] = -80
                }
            },
            new()
            {
                Code = "butchering:sausage-*",
                ValueByType = new()
                {
                    ["*"] = -80,
                    ["butchering:sausage-bloodsausage-raw"] = -50,
                    ["butchering:sausage-bloodsausage-cooked"] = -80,
                    ["butchering:sausage-blackpudding-raw"] = -50,
                    ["butchering:sausage-blackpudding-cooked"] = -80
                }
            },
            new()
            {
                Code = "butchering:primemeat-*",
                ValueByType = new()
                {
                    ["*"] = -100,
                    ["butchering:primemeat-raw"] = -50,
                    ["butchering:primemeat-curedhealing"] = -80,
                    ["butchering:primemeat-cooked"] = -100,
                    ["butchering:primemeat-healing"] = -100
                }
            },
            new()
            {
                Code = "butchering:offal",
                Value = 3
            },
            new()
            {
                Code = "expandedfoods:birchsapportion",
                Value = 500
            },
            new()
            {
                Code = "expandedfoods:breadstarter",
                Value = -75
            },
            new()
            {
                Code = "expandedfoods:brothportion-*",
                ValueByType = new()
                {
                    ["*"] = 400,
                    ["expandedfoods:brothportion-bone"] = 400,
                    ["expandedfoods:brothportion-vegetable"] = 450,
                    ["expandedfoods:brothportion-meat"] = 425,
                    ["expandedfoods:brothportion-fish"] = 420
                }
            },
            new()
            {
                Code = "expandedfoods:clarifiedbrothportion-*",
                ValueByType = new()
                {
                    ["*"] = 600,
                    ["expandedfoods:clarifiedbrothportion-bone"] = 600,
                    ["expandedfoods:clarifiedbrothportion-vegetable"] = 650,
                    ["expandedfoods:clarifiedbrothportion-meat"] = 625,
                    ["expandedfoods:clarifiedbrothportion-fish"] = 620
                }
            },
            new()
            {
                Code = "expandedfoods:dressing-*",
                ValueByType = new()
                {
                    ["*"] = 50
                }
            },
            new()
            {
                Code = "expandedfoods:fishsauce",
                Value = 60
            },
            new()
            {
                Code = "expandedfoods:foodoilportion-*",
                ValueByType = new()
                {
                    ["*"] = -30
                }
            },
            new()
            {
                Code = "expandedfoods:fruitsyrupportion-*",
                ValueByType = new()
                {
                    ["*"] = 100
                }
            },
            new()
            {
                Code = "expandedfoods:lard",
                Value = -5
            },
            new()
            {
                Code = "expandedfoods:maplesapportion",
                Value = 600
            },
            new()
            {
                Code = "expandedfoods:pasteurizedmilkportion",
                Value = 800
            },
            new()
            {
                Code = "expandedfoods:peanutliquid-*",
                ValueByType = new()
                {
                    ["*"] = -5,
                    ["expandedfoods:peanutliquid-paste"] = -5,
                    ["expandedfoods:peanutliquid-butter"] = -30,
                    ["expandedfoods:peanutliquid-sauce"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:potentspiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 170,
                    ["expandedfoods:potentspiritportion-apple"] = 170,
                    ["expandedfoods:potentspiritportion-mead"] = 180,
                    ["expandedfoods:potentspiritportion-spelt"] = 175,
                    ["expandedfoods:potentspiritportion-rice"] = 175,
                    ["expandedfoods:potentspiritportion-rye"] = 175,
                    ["expandedfoods:potentspiritportion-amaranth"] = 175,
                    ["expandedfoods:potentspiritportion-cassava"] = 175
                }
            },
            new()
            {
                Code = "expandedfoods:potentwineportion-*",
                ValueByType = new()
                {
                    ["*"] = 160,
                    ["expandedfoods:potentwineportion-apple"] = 160,
                    ["expandedfoods:potentwineportion-mead"] = 170,
                    ["expandedfoods:potentwineportion-spelt"] = 165,
                    ["expandedfoods:potentwineportion-rice"] = 165,
                    ["expandedfoods:potentwineportion-rye"] = 165,
                    ["expandedfoods:potentwineportion-amaranth"] = 165,
                    ["expandedfoods:potentwineportion-cassava"] = 165
                }
            },
            new()
            {
                Code = "expandedfoods:softresin",
                Value = -10
            },
            new()
            {
                Code = "expandedfoods:soulstormbrew-*",
                ValueByType = new()
                {
                    ["*"] = 100,
                    ["expandedfoods:soulstormbrew-slop"] = 100,
                    ["expandedfoods:soulstormbrew-refinedslop"] = 150,
                    ["expandedfoods:soulstormbrew-basic"] = 200
                }
            },
            new()
            {
                Code = "expandedfoods:soymilk-*",
                ValueByType = new()
                {
                    ["*"] = 600,
                    ["expandedfoods:soymilk-raw"] = 600,
                    ["expandedfoods:soymilk-edible"] = 700
                }
            },
            new()
            {
                Code = "expandedfoods:soysauce",
                Value = -20
            },
            new()
            {
                Code = "expandedfoods:strongspiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 150,
                    ["expandedfoods:strongspiritportion-apple"] = 150,
                    ["expandedfoods:strongspiritportion-mead"] = 160,
                    ["expandedfoods:strongspiritportion-spelt"] = 155,
                    ["expandedfoods:strongspiritportion-rice"] = 155,
                    ["expandedfoods:strongspiritportion-rye"] = 155,
                    ["expandedfoods:strongspiritportion-amaranth"] = 155,
                    ["expandedfoods:strongspiritportion-cassava"] = 155
                }
            },
            new()
            {
                Code = "expandedfoods:strongwineportion-*",
                ValueByType = new()
                {
                    ["*"] = 150,
                    ["expandedfoods:strongwineportion-apple"] = 150,
                    ["expandedfoods:strongwineportion-mead"] = 160,
                    ["expandedfoods:strongwineportion-spelt"] = 155,
                    ["expandedfoods:strongwineportion-rice"] = 155,
                    ["expandedfoods:strongwineportion-rye"] = 155,
                    ["expandedfoods:strongwineportion-amaranth"] = 155,
                    ["expandedfoods:strongwineportion-cassava"] = 155
                }
            },
            new()
            {
                Code = "expandedfoods:treesyrupportion-*",
                ValueByType = new()
                {
                    ["*"] = -20
                }
            },
            new()
            {
                Code = "expandedfoods:vegetablejuiceportion-*",
                ValueByType = new()
                {
                    ["*"] = 150,
                    ["expandedfoods:vegetablejuiceportion-carrot"] = 200,
                    ["expandedfoods:vegetablejuiceportion-cabbage"] = 300,
                    ["expandedfoods:vegetablejuiceportion-onion"] = 150,
                    ["expandedfoods:vegetablejuiceportion-turnip"] = 150,
                    ["expandedfoods:vegetablejuiceportion-parsnip"] = 150,
                    ["expandedfoods:vegetablejuiceportion-pumpkin"] = 150,
                    ["expandedfoods:vegetablejuiceportion-cassava"] = 150,
                    ["expandedfoods:vegetablejuiceportion-bellpepper"] = 150
                }
            },
            new()
            {
                Code = "expandedfoods:yeastwaterportion",
                Value = 50
            },
            new()
            {
                Code = "expandedfoods:yogurt-*",
                ValueByType = new()
                {
                    ["*"] = 50
                }
            },
            new()
            {
                Code = "expandedfoods:acorncoffee",
                Value = 75
            },
            new()
            {
                Code = "expandedfoods:wildfruitsyrupportion-*",
                ValueByType = new()
                {
                    ["*"] = 50
                }
            },
            new()
            {
                Code = "expandedfoods:wildpotentspiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 50
                }
            },
            new()
            {
                Code = "expandedfoods:wildpotentwineportion-*",
                ValueByType = new()
                {
                    ["*"] = 100
                }
            },
            new()
            {
                Code = "expandedfoods:wildstrongspiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 50
                }
            },
            new()
            {
                Code = "expandedfoods:wildstrongwineportion-*",
                ValueByType = new()
                {
                    ["*"] = 50
                }
            },
            new()
            {
                Code = "expandedfoods:wildtreefruitsyrupportion-*",
                ValueByType = new()
                {
                    ["*"] = 50
                }
            },
            new()
            {
                Code = "expandedfoods:wildtreepotentspiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 50
                }
            },
            new()
            {
                Code = "expandedfoods:wildtreepotentwineportion-*",
                ValueByType = new()
                {
                    ["*"] = 100
                }
            },
            new()
            {
                Code = "expandedfoods:wildtreestrongspiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 50
                }
            },
            new()
            {
                Code = "expandedfoods:wildtreestrongwineportion-*",
                ValueByType = new()
                {
                    ["*"] = 100
                }
            },
            new()
            {
                Code = "expandedfoods:agedmeat-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:breadcrumbs-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:candiedfruit-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "expandedfoods:choppedmushroom-*",
                ValueByType = new()
                {
                    ["*"] = -5,
                    ["expandedfoods:choppedmushroom-earthball"] = -4,
                    ["expandedfoods:choppedmushroom-deathcap"] = -25,
                    ["expandedfoods:choppedmushroom-funeralbell"] = -20,
                    ["expandedfoods:choppedmushroom-elfinsaddle"] = -3,
                    ["expandedfoods:choppedmushroom-jackolantern"] = -3,
                    ["expandedfoods:choppedmushroom-devilbolete"] = -5,
                    ["expandedfoods:choppedmushroom-pinkbonnet"] = -10,
                    ["expandedfoods:choppedmushroom-flyagaric"] = -3,
                    ["expandedfoods:choppedmushroom-bitterbolete"] = -1,
                    ["expandedfoods:choppedmushroom-devilstooth"] = -1,
                    ["expandedfoods:choppedmushroom-golddropmilkcap"] = -1
                }
            },
            new()
            {
                Code = "expandedfoods:choppedvegetable-*",
                ValueByType = new()
                {
                    ["*"] = 5,
                    ["expandedfoods:choppedvegetable-cassavadried"] = 5,
                    ["expandedfoods:choppedvegetable-cabbage"] = 8,
                    ["expandedfoods:choppedvegetable-pumpkin"] = 7,
                    ["expandedfoods:choppedvegetable-pickledcarrot"] = 6,
                    ["expandedfoods:choppedvegetable-pickledonion"] = 6,
                    ["expandedfoods:choppedvegetable-pickledparsnip"] = 6,
                    ["expandedfoods:choppedvegetable-pickledturnip"] = 6,
                    ["expandedfoods:choppedvegetable-pickledpumpkin"] = 6,
                    ["expandedfoods:choppedvegetable-papyrusroot"] = 7,
                    ["expandedfoods:choppedvegetable-cattailroot"] = 100,
                    ["expandedfoods:choppedvegetable-bellpepper"] = 7
                }
            },
            new()
            {
                Code = "expandedfoods:cookedchoppedmushroom-*",
                ValueByType = new()
                {
                    ["*"] = -1
                }
            },
            new()
            {
                Code = "expandedfoods:cookedchoppedvegetable-*",
                ValueByType = new()
                {
                    ["*"] = 5
                }
            },
            new()
            {
                Code = "expandedfoods:cookedmushroom-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:cookedveggie-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:dehydratedfruit-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:dryfruit-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:driedseaweed-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:fermentedfish-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:fishnugget-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:gelatin-*",
                ValueByType = new()
                {
                    ["*"] = 1
                }
            },
            new()
            {
                Code = "expandedfoods:gelatinfish-*",
                ValueByType = new()
                {
                    ["*"] = 1
                }
            },
            new()
            {
                Code = "expandedfoods:limeegg-*",
                ValueByType = new()
                {
                    ["*"] = 1
                }
            },
            new()
            {
                Code = "expandedfoods:meatnugget-*",
                ValueByType = new()
                {
                    ["*"] = -1
                }
            },
            new()
            {
                Code = "expandedfoods:peanut-*",
                ValueByType = new()
                {
                    ["*"] = -3
                }
            },
            new()
            {
                Code = "expandedfoods:soyprep-*",
                ValueByType = new()
                {
                    ["*"] = 5
                }
            },
            new()
            {
                Code = "expandedfoods:acornbreadcrumbs-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:acornportion-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:acornpowdered-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:acorns-roasted-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:acornberrybread-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:acornbreadedball-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:acornbreadedfishnugget-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:acornbreadedmeatnugget-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:acornbreadedmushroom-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:acornbreadedvegetable-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:acorndoughball-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:acorndumpling-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:acornhardtack-*",
                ValueByType = new()
                {
                    ["*"] = -25
                }
            },
            new()
            {
                Code = "expandedfoods:acornmuffin-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:acornpasta-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:acornstuffedpepper-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:saltedmeatnugget-*",
                ValueByType = new()
                {
                    ["*"] = -15
                }
            },
            new()
            {
                Code = "expandedfoods:berrybread-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:breadedball-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:breadedfishnugget-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:breadedmeatnugget-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:breadedmushroom-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:breadedvegetable-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:candy-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:dumpling-*",
                ValueByType = new()
                {
                    ["*"] = -3
                }
            },
            new()
            {
                Code = "expandedfoods:fruitbar-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:gozinaki-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:hardtack-*",
                ValueByType = new()
                {
                    ["*"] = -25
                }
            },
            new()
            {
                Code = "expandedfoods:muffin-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:pasta-*",
                ValueByType = new()
                {
                    ["*"] = -3
                }
            },
            new()
            {
                Code = "expandedfoods:pemmican-*",
                ValueByType = new()
                {
                    ["*"] = -25
                }
            },
            new()
            {
                Code = "expandedfoods:pemmicanfish-*",
                ValueByType = new()
                {
                    ["*"] = -25
                }
            },
            new()
            {
                Code = "expandedfoods:plaindoughball-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:sausage-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:sausagefish-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:stuffedpepper-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:sushi-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "expandedfoods:sushiveg-*",
                ValueByType = new()
                {
                    ["*"] = -1
                }
            },
            new()
            {
                Code = "expandedfoods:trailmix-*",
                ValueByType = new()
                {
                    ["*"] = -25
                }
            },
            new()
            {
                Code = "expandedfoods:trailmixvegetarian-*",
                ValueByType = new()
                {
                    ["*"] = -25
                }
            },
            new()
            {
                Code = "expandedfoods:crabnugget-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:snakenugget-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "expandedfoods:pemmicancrab-*",
                ValueByType = new()
                {
                    ["*"] = -25
                }
            },
            new()
            {
                Code = "expandedfoods:pemmicansnake-*",
                ValueByType = new()
                {
                    ["*"] = -25
                }
            },
            new()
            {
                Code = "expandedfoods:sausagecrab-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:sausagesnake-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:herbalcornbreadcrumbs-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:herbalbreadcrumbs-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:wildcandiedfruit-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "expandedfoods:wilddehydratedfruit-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:wilddryfruit-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:gozinakiherb-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:herbalacornbread-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:herbalacornhardtack-*",
                ValueByType = new()
                {
                    ["*"] = -25
                }
            },
            new()
            {
                Code = "expandedfoods:herbalabread-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:sausagecrabherb-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:sausagefishherb-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:sausageherb-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:sausagesnakeherb-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "expandedfoods:wildtreecandiedfruit-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "expandedfoods:wildtreedehydratedfruit-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "expandedfoods:wildtreedryfruit-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "floralzonescaperegion:fruit-*",
                ValueByType = new()
                {
                    ["*"] = 30
                }
            },
            new()
            {
                Code = "floralzonescaperegion:vegetable-*",
                ValueByType = new()
                {
                    ["*"] = 15
                }
            },
            new()
            {
                Code = "floralzonescaribbeanregion:vegetable-*",
                ValueByType = new()
                {
                    ["*"] = 15
                }
            },
            new()
            {
                Code = "floralzonescaribbeanregion:vegetable-nonpalm-*",
                ValueByType = new()
                {
                    ["*"] = 15
                }
            },
            new()
            {
                Code = "floralzonescaribbeanregion:fruit-*",
                ValueByType = new()
                {
                    ["*"] = 10
                }
            },
            new()
            {
                Code = "floralzonescaribbeanregion:flour-*",
                ValueByType = new()
                {
                    ["*"] = -7
                }
            },
            new()
            {
                Code = "floralzonescaribbeanregion:dough-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "floralzonescaribbeanregion:bread-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "floralzonescaribbeanregion:spiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 100
                }
            },
            new()
            {
                Code = "floralzonescaribbeanregion:juiceportion-*",
                ValueByType = new()
                {
                    ["*"] = 50
                }
            },
            new()
            {
                Code = "floralzonescaribbeanregion:ciderportion-*",
                ValueByType = new()
                {
                    ["*"] = 300
                }
            },
            new()
            {
                Code = "floralzonescentralaustralianregion:vegetable-*",
                ValueByType = new()
                {
                    ["*"] = 5
                }
            },
            new()
            {
                Code = "floralzonescentralaustralianregion:legume-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "floralzonescentralaustralianregion:fruit-*",
                ValueByType = new()
                {
                    ["*"] = 10
                }
            },
            new()
            {
                Code = "floralzoneseastasiaticregion:vegetable-*",
                ValueByType = new()
                {
                    ["*"] = 5
                }
            },
            new()
            {
                Code = "floralzoneseastasiaticregion:rawkudzu-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "floralzoneseastasiaticregion:fruit-*",
                ValueByType = new()
                {
                    ["*"] = 10
                }
            },
            new()
            {
                Code = "floralzoneseastasiaticregion:flour-*",
                ValueByType = new()
                {
                    ["*"] = -8
                }
            },
            new()
            {
                Code = "floralzoneseastasiaticregion:dough-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "floralzoneseastasiaticregion:bread-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "floralzoneseastasiaticregion:spiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 100
                }
            },
            new()
            {
                Code = "floralzoneseastasiaticregion:juiceportion-*",
                ValueByType = new()
                {
                    ["*"] = 700
                }
            },
            new()
            {
                Code = "floralzoneseastasiaticregion:ciderportion-*",
                ValueByType = new()
                {
                    ["*"] = 350
                }
            },
            new()
            {
                Code = "floralzonesneozeylandicregion:fruit-*",
                ValueByType = new()
                {
                    ["*"] = 10
                }
            },
            new()
            {
                Code = "floralzonesneozeylandicregion:vegetable-*",
                ValueByType = new()
                {
                    ["*"] = 5
                }
            },
            new()
            {
                Code = "newworldcrops:vegetable-*",
                ValueByType = new()
                {
                    ["*"] = 5
                }
            },
            new()
            {
                Code = "newworldcrops:pickledlegume-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "newworldcrops:pickledlegrain-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "newworldcrops:legume-*",
                ValueByType = new()
                {
                    ["*"] = -3
                }
            },
            new()
            {
                Code = "newworldcrops:grain-*",
                ValueByType = new()
                {
                    ["*"] = -3
                }
            },
            new()
            {
                Code = "newworldcrops:flour-*",
                ValueByType = new()
                {
                    ["*"] = -8
                }
            },
            new()
            {
                Code = "newworldcrops:dough-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "newworldcrops:bread-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "warriordrink:spiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 100
                }
            },
            new()
            {
                Code = "warriordrink:ciderportion-*",
                ValueByType = new()
                {
                    ["*"] = 300
                }
            },
            new()
            {
                Code = "wildcraftfruit:vegetable-*",
                ValueByType = new()
                {
                    ["*"] = 5
                }
            },
            new()
            {
                Code = "wildcraftfruit:nut-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "wildcraftfruit:fruit-*",
                ValueByType = new()
                {
                ["*"] = 10,
                ["wildcraftfruit:fruit-watermelon"] = 50,
                ["wildcraftfruit:fruit-illawarra"] = 15,
                ["wildcraftfruit:fruit-creepingpine"] = 8,
                ["wildcraftfruit:fruit-pandanbits"] = 12,
                ["wildcraftfruit:fruit-maritabits"] = 10,
                ["wildcraftfruit:fruit-crowseye"] = 7,
                ["wildcraftfruit:fruit-kakaha"] = 20,
                ["wildcraftfruit:fruit-flaxlily"] = 10,
                ["wildcraftfruit:fruit-engkala"] = 18,
                ["wildcraftfruit:fruit-kawakawa"] = 15,
                ["wildcraftfruit:fruit-gooseberry"] = 12,
                ["wildcraftfruit:fruit-blackgrape"] = 30,
                ["wildcraftfruit:fruit-redgrape"] = 30,
                ["wildcraftfruit:fruit-whitegrape"] = 30,
                ["wildcraftfruit:fruit-foxgrape"] = 28,
                ["wildcraftfruit:fruit-virgingrape"] = 25,
                ["wildcraftfruit:fruit-gardenstrawberry"] = 18,
                ["wildcraftfruit:fruit-strawberry"] = 18,
                ["wildcraftfruit:fruit-falsestrawberry"] = 15,
                ["wildcraftfruit:fruit-raspberry"] = 20,
                ["wildcraftfruit:fruit-blueraspberry"] = 20,
                ["wildcraftfruit:fruit-brambleberry"] = 16,
                ["wildcraftfruit:fruit-cloudberry"] = 15,
                ["wildcraftfruit:fruit-knyazberry"] = 22,
                ["wildcraftfruit:fruit-bushlawyer"] = 10,
                ["wildcraftfruit:fruit-dogrose"] = 14,
                ["wildcraftfruit:fruit-hawthorn"] = 12,
                ["wildcraftfruit:fruit-rowanberry"] = 12,
                ["wildcraftfruit:fruit-quince"] = 18,
                ["wildcraftfruit:fruit-loquat"] = 16,
                ["wildcraftfruit:fruit-pittedcherry"] = 15,
                ["wildcraftfruit:fruit-apricot"] = 22,
                ["wildcraftfruit:fruit-pittedapricot"] = 22,
                ["wildcraftfruit:fruit-purpleplum"] = 25,
                ["wildcraftfruit:fruit-cherryplum"] = 20,
                ["wildcraftfruit:fruit-sallowthorn"] = 10,
                ["wildcraftfruit:fruit-jujube"] = 18,
                ["wildcraftfruit:fruit-fig"] = 20,
                ["wildcraftfruit:fruit-falseorange"] = 25,
                ["wildcraftfruit:fruit-pittedbreadfruit"] = 15,
                ["wildcraftfruit:fruit-silvernettle"] = 8,
                ["wildcraftfruit:fruit-cucumber"] = 35,
                ["wildcraftfruit:fruit-muskmelon"] = 45,
                ["wildcraftfruit:fruit-mirzamelon"] = 40,
                ["wildcraftfruit:fruit-bryony"] = 10,
                ["wildcraftfruit:fruit-passionfruit"] = 22,
                ["wildcraftfruit:fruit-achacha"] = 18,
                ["wildcraftfruit:fruit-spindle"] = 10,
                ["wildcraftfruit:fruit-ugni"] = 15,
                ["wildcraftfruit:fruit-midyimberry"] = 18,
                ["wildcraftfruit:fruit-munthari"] = 15,
                ["wildcraftfruit:fruit-guajava"] = 20,
                ["wildcraftfruit:fruit-feijoa"] = 22,
                ["wildcraftfruit:fruit-roseapple"] = 25,
                ["wildcraftfruit:fruit-lillypillypink"] = 20,
                ["wildcraftfruit:fruit-lillypillyblue"] = 20,
                ["wildcraftfruit:fruit-lillypillywhite"] = 20,
                ["wildcraftfruit:fruit-beachalmondwhole"] = 12,
                ["wildcraftfruit:fruit-pittedbeachalmond"] = 12,
                ["wildcraftfruit:fruit-bluetongue"] = 18,
                ["wildcraftfruit:fruit-turkscap"] = 14,
                ["wildcraftfruit:fruit-cocoa"] = 15,
                ["wildcraftfruit:fruit-wolfberry"] = 10,
                ["wildcraftfruit:fruit-caperberry"] = 12,
                ["wildcraftfruit:fruit-citron"] = 25,
                ["wildcraftfruit:fruit-lemon"] = 30,
                ["wildcraftfruit:fruit-pomelo"] = 35,
                ["wildcraftfruit:fruit-fingerlime"] = 28,
                ["wildcraftfruit:fruit-kumquat"] = 20,
                ["wildcraftfruit:fruit-lemonaspen"] = 30,
                ["wildcraftfruit:fruit-cashewwhole"] = 8,
                ["wildcraftfruit:fruit-cashewapple"] = 25,
                ["wildcraftfruit:fruit-chinaberry"] = 10,
                ["wildcraftfruit:fruit-pittedchinaberry"] = 10,
                ["wildcraftfruit:fruit-redquandong"] = 20,
                ["wildcraftfruit:fruit-rubysaltbush"] = 12,
                ["wildcraftfruit:fruit-pokeberry"] = 8,
                ["wildcraftfruit:fruit-bunchberry"] = 15,
                ["wildcraftfruit:fruit-cheeseberry"] = 15,
                ["wildcraftfruit:fruit-pineheath"] = 10,
                ["wildcraftfruit:fruit-pricklyheath"] = 10,
                ["wildcraftfruit:fruit-honeypots"] = 12,
                ["wildcraftfruit:fruit-crowberry"] = 10,
                ["wildcraftfruit:fruit-lingonberry"] = 15,
                ["wildcraftfruit:fruit-huckleberry"] = 18,
                ["wildcraftfruit:fruit-kiwi"] = 20,
                ["wildcraftfruit:fruit-honeysuckle"] = 12,
                ["wildcraftfruit:fruit-snowberry"] = 8,
                ["wildcraftfruit:fruit-blackelder"] = 10,
                ["wildcraftfruit:fruit-elderberry"] = 15,
                ["wildcraftfruit:fruit-ivy"] = 10,
                ["wildcraftfruit:fruit-blacknightshade"] = 10,
                ["wildcraftfruit:fruit-blacknightshadeunripe"] = 8,
                ["wildcraftfruit:fruit-bitternightshade"] = 8,
                ["wildcraftfruit:fruit-naranjilla"] = 18,
                ["wildcraftfruit:fruit-husktomato"] = 15,
                ["wildcraftfruit:fruit-beautyberry"] = 15,
                ["wildcraftfruit:fruit-numnum"] = 15,
                ["wildcraftfruit:fruit-seamango"] = 20,
                ["wildcraftfruit:fruit-coralbead"] = 10,
                ["wildcraftfruit:fruit-pilo"] = 15,
                ["wildcraftfruit:fruit-mingimingi"] = 15,
                ["wildcraftfruit:fruit-fractureberry"] = 10
            }
            },
            new()
            {
                Code = "wildcraftfruit:flour-*",
                ValueByType = new()
                {
                    ["*"] = -8
                }
            },
            new()
            {
                Code = "wildcraftfruit:dough-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "wildcraftfruit:bread-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "wildcraftfruit:berrymush-*",
                ValueByType = new()
                {
                    ["*"] = 20
                }
            },
            new()
            {
                Code = "wildcraftfruit:spiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 100
                }
            },
            new()
            {
                Code = "wildcraftfruit:finespiritportion-*",
                ValueByType = new()
                {
                    ["*"] = 300
                }
            },
            new()
            {
                Code = "wildcraftfruit:juiceportion-*",
                ValueByType = new()
                {
                    ["*"] = 800,
                    ["wildcraftfruit:juiceportion-watermelon"] = 1000
                }
            },
            new()
            {
                Code = "wildcraftfruit:ciderportion-*",
                ValueByType = new()
                {
                    ["*"] = 400
                }
            },
            new()
            {
                Code = "wildcraftfruit:fineciderportion-*",
                ValueByType = new()
                {
                    ["*"] = 600
                }
            },
            new()
            {
                Code = "wildcraftherb:root-*",
                ValueByType = new()
                {
                    ["*"] = 2
                }
            },
            new()
            {
                Code = "wildcraftherb:herb-*",
                ValueByType = new()
                {
                    ["*"] = 4
                }
            },
            new()
            {
                Code = "wildcraftfruit:flower-*",
                ValueByType = new()
                {
                    ["*"] = 2
                }
            },
            new()
            {
                Code = "wildcraftfruit:pickledvegetable-*",
                ValueByType = new()
                {
                    ["*"] = 15
                }
            },
            new()
            {
                Code = "wildcraftfruit:sweetwaterportion",
                Value = 650
            },
            new()
            {
                Code = "wildcraftfruit:lemonade-*",
                ValueByType = new()
                {
                    ["*"] = 850
                }
            },
            new()
            {
                Code = "acorns:flour-acorn-*",
                ValueByType = new()
                {
                    ["*"] = -8
                }
            },
            new()
            {
                Code = "acorns:acorn-meal-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "primitivesurvival:trussedrot-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "primitivesurvival:trussedmeat-*",
                ValueByType = new()
                {
                    ["*"] = -5
                }
            },
            new()
            {
                Code = "primitivesurvival:snakemeat-*",
                ValueByType = new()
                {
                    ["*"] = -2
                }
            },
            new()
            {
                Code = "primitivesurvival:smokedmeat-*",
                ValueByType = new()
                {
                    ["*"] = -6
                }
            },
            new()
            {
                Code = "primitivesurvival:jerky-redmeat-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "primitivesurvival:jerky-fish-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "primitivesurvival:jerky-bushmeat-*",
                ValueByType = new()
                {
                    ["*"] = -10
                }
            },
            new()
            {
                Code = "primitivesurvival:curedsmokedmeat-*",
                ValueByType = new()
                {
                    ["*"] = -12
                }
            },
            new()
            {
                Code = "primitivesurvival:crabmeat-*",
                ValueByType = new()
                {
                    ["*"] = -4
                }
            },
            new()
            {
                Code = "ancienttools:saltedmeat-*",
                ValueByType = new()
                {
                    ["*"] = -12
                }
            },
            new()
            {
                Code = "maketea:teaportion-*",
                ValueByType = new()
                {
                    ["*"] = 600
                }
            }
        }
    };
}
