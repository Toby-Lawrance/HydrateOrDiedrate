using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.Client.NoObf;

namespace HydrateOrDiedrate.Config.Patching;

public abstract class PatchBase
{
    public abstract string Code { get; set; }

    public abstract float Value { get; set; }
    public abstract Dictionary<string, float> ValueByType { get; set; }

    [JsonIgnore]
    public CollectibleTarget Target { get; private set; }

    private (CollectibleTarget target, float value)[] CompiledValues { get; set; }

    public void PreCompile()
    {
        Target ??= new CollectibleTarget(Code);

        if(ValueByType is not null)
        {
            if(ValueByType.TryGetValue("*", out var value)) //Extract the catchAll
            {
                Value = value;
                ValueByType.Remove("*");
            }

            if(ValueByType.Count > 0) CompiledValues = ValueByType.Select(pair =>  (new CollectibleTarget(pair.Key), pair.Value)).ToArray();
        }
    }

    public void Apply(CollectibleObject collectible)
    {
        collectible.Attributes ??= new JsonObject(new JObject()); //Ensure attributes exist

        if(CompiledValues is null)
        {
            Apply(collectible, Value);
            return;
        }

        for (int i = 0; i < CompiledValues.Length; i++)
        {
            (CollectibleTarget target, float value) = CompiledValues[i];
            
            if(!target.IsMatch(collectible)) continue;
            Apply(collectible, value);
            return;
        }
        
        Apply(collectible, Value);
    }

    public abstract void Apply(CollectibleObject collectible, float value);
}
