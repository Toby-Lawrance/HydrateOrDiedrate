using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace HydrateOrDiedrate.Config.Patching;

public class PatchCollection<T> where T : PatchBase
{
    public int Priority { get; set; }

    public T[] Patches { get; set; } = Array.Empty<T>();

    public void ApplyPatches(IEnumerable<CollectibleObject> collectibles)
    {
        for (int i = 0; i < Patches.Length; i++) Patches[i].PreCompile();

        foreach (var collectible in collectibles)
        {
            for (int i = 0; i < Patches.Length; i++)
            {
                T patch = Patches[i];
                if(!patch.Target.IsMatch(collectible)) continue;

                patch.Apply(collectible);
            }
        }
    }

    public void MergeMissing(PatchCollection<T> otherCollection)
    {
        var missingPatches = otherCollection.Patches
            .Where(patch => !Array.Exists(Patches, existingPatch => existingPatch.Code == patch.Code))
            .ToList();
        
        Patches = Patches.Append(missingPatches);
    }

    public static PatchCollection<T> GetMerged(ICoreAPI api, string path, PatchCollection<T> defaultConfig)
    {
        PatchCollection<T> result;
        try
        {
            result = api.LoadModConfig<PatchCollection<T>>(path);
            if(result is null)
            {
                result = defaultConfig;
            }
            else result.MergeMissing(defaultConfig);

            api.StoreModConfig(result, path);
        }
        catch(Exception ex)
        {
            api.Logger.Error(ex);
            result = defaultConfig;
        }
        return result;
    }
}
