using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace HydrateOrDiedrate.Hot_Weather;

public static class CoolingManager
{
    public static float GetMaxCooling(ItemStack itemStack)
    {
        var attributes = itemStack?.Collectible.Attributes;
        if (attributes is null) return 0f;

        return attributes.Token.Value<float>(Attributes.Cooling).GuardFinite();
    }
}
