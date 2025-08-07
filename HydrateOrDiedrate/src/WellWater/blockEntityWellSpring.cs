using HydrateOrDiedrate.Config;
using System;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace HydrateOrDiedrate.wellwater
{
    public class BlockEntityWellSpring : BlockEntity
    {
        private ICoreServerAPI sapi;
        private AquiferManager _aquiferManager;
        private int updateIntervalMs = 500;
        private double accumulatedWater = 0.0;
        private double lastInGameTime = -1.0;

        private string cachedRingMaterial;
        private bool canPlaceToConfiguredLevel;
        private int partialValidatedHeight;
        private const double MaxDailyOutput = 70.0;
        private const double MinimumDailyOutput = 0.0;
        
        private string lastWaterType;
        private double lastDailyLiters ;
        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            sapi = api as ICoreServerAPI;
            if (sapi != null)
            {
                _aquiferManager = HydrateOrDiedrateModSystem.AquiferManager;
                if (_aquiferManager == null) return;
                int chunkX = Pos.X / GlobalConstants.ChunkSize;
                int chunkY = Pos.Y / GlobalConstants.ChunkSize;
                int chunkZ = Pos.Z / GlobalConstants.ChunkSize;
                ChunkPos3D chunkCoord = new ChunkPos3D(chunkX, chunkY, chunkZ);

                _aquiferManager.AddWellspringToChunk(chunkCoord, Pos);

                cachedRingMaterial = CheckBaseRingMaterial(sapi.World.BlockAccessor, Pos);
                int validatedLevels = CheckColumnForMaterial(sapi.World.BlockAccessor, Pos, cachedRingMaterial);
                partialValidatedHeight = validatedLevels;

                if (cachedRingMaterial == "brick" && validatedLevels >= ModConfig.Instance.GroundWater.WellwaterDepthMaxClay)
                {
                    canPlaceToConfiguredLevel = true;
                }
                else if (cachedRingMaterial == "stonebrick" && validatedLevels >= ModConfig.Instance.GroundWater.WellwaterDepthMaxStone)
                {
                    canPlaceToConfiguredLevel = true;
                }

                RegisterGameTickListener(OnTick, updateIntervalMs);
                RegisterGameTickListener(OnPeriodicShaftCheck, 30000);
            }
        }
        public override void OnBlockRemoved()
        {
            base.OnBlockRemoved();
            if (_aquiferManager != null)
            {
                _aquiferManager.UnregisterWellspring(Pos);
            }
        }

        private void OnTick(float dt)
        {
            double multiplier = ModConfig.Instance.GroundWater.WellSpringOutputMultiplier;
            var calendar = sapi.World.Calendar;
            double currentInGameTime = calendar.TotalDays;
            if (lastInGameTime < 0)
            {
                lastInGameTime = currentInGameTime;
                return;
            }

            double elapsedDays = currentInGameTime - lastInGameTime;
            if (elapsedDays <= 0)
            {
                return;
            }

            lastInGameTime = currentInGameTime;

            int chunkX = Pos.X / GlobalConstants.ChunkSize;
            int chunkY = Pos.Y / GlobalConstants.ChunkSize;
            int chunkZ = Pos.Z / GlobalConstants.ChunkSize;
            var chunkCoord = new ChunkPos3D(chunkX, chunkY, chunkZ);
            bool isMuddy = IsSurroundedBySoil(sapi.World.BlockAccessor, Pos);
            var aquiferData = _aquiferManager.GetAquiferData(chunkCoord);
            if (aquiferData.AquiferRating == 0 & !isMuddy) return;
            var wellsprings = _aquiferManager.GetWellspringsInChunk(chunkCoord);
            if (wellsprings.Count == 0) return;
            
            var nearbyWater = CheckForNearbyGameWater(sapi.World.BlockAccessor, Pos);
            if (isMuddy)
            {
                if (nearbyWater.isFresh || nearbyWater.isSalty)
                {
                    accumulatedWater += 0.001 * multiplier;
                    if (accumulatedWater >= 1.0)
                    {
                        int wholeLiters = (int)Math.Floor(accumulatedWater);
                        accumulatedWater -= wholeLiters;
                        string waterType = nearbyWater.isSalty ? "muddysalt" : "muddy";
                        AddOrPlaceWater(waterType, wholeLiters);
                        accumulatedWater = 0;
                    }

                    return;
                }
                else
                {
                    return;
                }
            }
            double remainingRating = (double)aquiferData.AquiferRating / wellsprings.Count;
            var thisSpring = wellsprings.FirstOrDefault(ws => ws.Position.Equals(Pos));
            if (thisSpring == null) return;

            double dailyLiters = Math.Max(MinimumDailyOutput, (remainingRating * MaxDailyOutput / 100.0)) * multiplier;
            lastDailyLiters = dailyLiters;
            MarkDirty(true);
            double litersThisTick = dailyLiters * elapsedDays;
            accumulatedWater += litersThisTick;
            if (accumulatedWater >= 1.0)
            {
                int wholeLiters = (int)Math.Floor(accumulatedWater);
                accumulatedWater -= wholeLiters;
                string waterType = aquiferData.IsSalty ? "salt" : "fresh";
                lastWaterType = waterType;
                MarkDirty(true);
                AddOrPlaceWater(waterType, wholeLiters);
            }
        }

        private void OnPeriodicShaftCheck(float dt)
        {
            string oldRingMaterial = cachedRingMaterial;
            bool oldCanPlace = canPlaceToConfiguredLevel;
            cachedRingMaterial = CheckBaseRingMaterial(sapi.World.BlockAccessor, Pos);
            int validatedLevels = CheckColumnForMaterial(sapi.World.BlockAccessor, Pos, cachedRingMaterial);
            partialValidatedHeight = validatedLevels;
            MarkDirty(true);
            canPlaceToConfiguredLevel = false;
            if (cachedRingMaterial == "brick" && validatedLevels >= ModConfig.Instance.GroundWater.WellwaterDepthMaxClay) canPlaceToConfiguredLevel = true;
            else if (cachedRingMaterial == "stonebrick" && validatedLevels >= ModConfig.Instance.GroundWater.WellwaterDepthMaxStone) canPlaceToConfiguredLevel = true;
            if (oldRingMaterial != cachedRingMaterial || oldCanPlace != canPlaceToConfiguredLevel) MarkDirty(true);
        }

        private void AddOrPlaceWater(string waterType, int litersToAdd)
        {
            var blockAccessor = sapi.World.BlockAccessor;
            bool isMuddy = IsSurroundedBySoil(blockAccessor, Pos);

            if (waterType == "muddy" || waterType == "muddysalt")
            {
                BlockPos firstPos = Pos.UpCopy(1);
                Block firstBlock = blockAccessor.GetBlock(firstPos);
                if (IsBlockingBlock(firstBlock))
                {
                    accumulatedWater = 0.0;
                    return;
                }
                if (firstBlock?.Code?.Path.StartsWith($"wellwater{waterType}") == true)
                {
                    var existingBE = blockAccessor.GetBlockEntity<BlockEntityWellWaterData>(firstPos);
                    if (existingBE != null)
                    {
                        int maxVolume = 9;
                        if (existingBE.Volume >= maxVolume)
                        {
                            accumulatedWater = 0.0;
                            return;
                        }
                        else
                        {
                            int addedVolume = Math.Min(maxVolume - existingBE.Volume, litersToAdd);
                            existingBE.Volume += addedVolume;
                            accumulatedWater = 0.0;
                            return;
                        }
                    }
                }
                bool skipPlacementCheck = isMuddy;
                string blockPath = firstBlock?.Code?.Path;
                bool isAir = blockPath == "air";
                bool isSpreading = blockPath?.StartsWith($"wellwater{waterType}-spreading-") == true;
                bool isNatural = blockPath?.StartsWith($"wellwater{waterType}-natural-") == true;

                if ((isAir || isSpreading) && !isNatural &&
                    (skipPlacementCheck || IsValidPlacement(blockAccessor, firstPos)))
                {
                    string blockCode = $"hydrateordiedrate:wellwater{waterType}-natural-still-1";
                    Block waterBlock = sapi.World.GetBlock(new AssetLocation(blockCode));
                    if (waterBlock != null)
                    {
                        blockAccessor.SetBlock(waterBlock.BlockId, firstPos);
                        blockAccessor.TriggerNeighbourBlockUpdate(firstPos);
                        var newBE = blockAccessor.GetBlockEntity<BlockEntityWellWaterData>(firstPos);
                        if (newBE != null)
                        {
                            int maxVolume = 9;
                            int volumeToSet = Math.Min(litersToAdd, maxVolume);
                            newBE.Volume = volumeToSet;
                        }
                    }
                }
                accumulatedWater = 0.0;
                return;
            }
            int maxDepth = DetermineMaxDepthBasedOnCached(cachedRingMaterial, partialValidatedHeight);
            int leftoverLiters = litersToAdd;
            for (int i = 0; i < maxDepth && leftoverLiters > 0; i++)
            {
                BlockPos currentPos = Pos.UpCopy(i + 1);
                Block currentBlock = blockAccessor.GetBlock(currentPos);
                if (IsBlockingBlock(currentBlock))
                {
                    break;
                }

                if (currentBlock?.Code?.Path.StartsWith($"wellwater{waterType}") == true)
                {
                    var existingBE = blockAccessor.GetBlockEntity<BlockEntityWellWaterData>(currentPos);
                    if (existingBE != null)
                    {
                        int maxVolume = isMuddy ? 9 : 70;
                        int availableCapacity = maxVolume - existingBE.Volume;
                        if (availableCapacity > 0)
                        {
                            int addedVolume = Math.Min(availableCapacity, leftoverLiters);
                            existingBE.Volume += addedVolume;
                            leftoverLiters -= addedVolume;
                        }
                    }
                }
            }

            for (int i = 0; i < maxDepth && leftoverLiters > 0; i++)
            {
                BlockPos currentPos = Pos.UpCopy(i + 1);
                Block currentBlock = blockAccessor.GetBlock(currentPos);
                if (IsBlockingBlock(currentBlock))
                {
                    break;
                }

                bool skipPlacementCheck = isMuddy;
                string blockPath = currentBlock?.Code?.Path;
                bool isAir = blockPath == "air";
                bool isSpreading = blockPath?.StartsWith($"wellwater{waterType}-spreading-") == true;
                bool isNatural = blockPath?.StartsWith($"wellwater{waterType}-natural-") == true;

                if ((isAir || isSpreading) && !isNatural &&
                    (skipPlacementCheck || IsValidPlacement(blockAccessor, currentPos)))
                {
                    string blockCode = $"hydrateordiedrate:wellwater{waterType}-natural-still-1";
                    Block waterBlock = sapi.World.GetBlock(new AssetLocation(blockCode));
                    if (waterBlock != null)
                    {
                        blockAccessor.SetBlock(waterBlock.BlockId, currentPos);
                        blockAccessor.TriggerNeighbourBlockUpdate(currentPos);
                        var newBE = blockAccessor.GetBlockEntity<BlockEntityWellWaterData>(currentPos);
                        if (newBE != null)
                        {
                            int maxVolume = isMuddy ? 9 : 70;
                            int volumeToSet = Math.Min(leftoverLiters, maxVolume);
                            newBE.Volume = volumeToSet;
                            leftoverLiters -= volumeToSet;
                        }
                    }
                }
            }

            if (leftoverLiters > 0)
            {
                accumulatedWater = 0.0;
            }
        }

        private bool IsBlockingBlock(Block block) => block?.Code is not null && block.Code.Path != "air"  && !block.Code.Path.Contains("wellwater");
        
        private int DetermineMaxDepthBasedOnCached(string ringMat, int validatedLevels)
        {
            int baseDepth = ModConfig.Instance.GroundWater.WellwaterDepthMaxBase;
            if (validatedLevels <= 0 || ringMat == "none") return baseDepth;
            if (ringMat == "brick")
            {
                int clayMax = ModConfig.Instance.GroundWater.WellwaterDepthMaxClay;
                int partialDepth = Math.Min(validatedLevels, clayMax);
                return Math.Max(baseDepth, partialDepth);
            }
            else if (ringMat == "stonebrick")
            {
                int stoneMax = ModConfig.Instance.GroundWater.WellwaterDepthMaxStone;
                int partialDepth = Math.Min(validatedLevels, stoneMax);
                return Math.Max(baseDepth, partialDepth);
            }
            return baseDepth;
        }

        private string CheckBaseRingMaterial(IBlockAccessor blockAccessor, BlockPos pos)
        {
            //TODO do more efficient walk
            Block[] neighbors = new Block[]
            {
                blockAccessor.GetBlock(pos.NorthCopy()),
                blockAccessor.GetBlock(pos.EastCopy()),
                blockAccessor.GetBlock(pos.SouthCopy()),
                blockAccessor.GetBlock(pos.WestCopy())
            };

            bool allBrick = Array.TrueForAll(neighbors, b => b?.Code?.Domain == "game" && b.Code.Path.StartsWith("brick"));
            if (allBrick) return "brick";

            bool allStone = Array.TrueForAll(neighbors, b => b?.Code?.Domain == "game" && b.Code.Path.StartsWith("stonebrick"));
            if (allStone) return "stonebrick";

            return "none";
        }

        private int CheckColumnForMaterial(IBlockAccessor blockAccessor, BlockPos basePos, string ringMaterial)
        {
            if (ringMaterial == "none")
            {
                return 0;
            }
            int maxCheck;
            if (ringMaterial == "brick")
            {
                maxCheck = ModConfig.Instance.GroundWater.WellwaterDepthMaxClay;
            }
            else if (ringMaterial == "stonebrick")
            {
                maxCheck = ModConfig.Instance.GroundWater.WellwaterDepthMaxStone;
            }
            else
            {
                return 0;
            }
            int validatedHeight = 0;
            for (int level = 1; level <= maxCheck; level++)
            {
                BlockPos checkPos = basePos.UpCopy(level);
                Block[] neighbors = new Block[]
                {
                    blockAccessor.GetBlock(checkPos.NorthCopy()),
                    blockAccessor.GetBlock(checkPos.EastCopy()),
                    blockAccessor.GetBlock(checkPos.SouthCopy()),
                    blockAccessor.GetBlock(checkPos.WestCopy())
                };
                if (ringMaterial == "brick")
                {
                    bool allBrick = neighbors.All(b =>
                        b?.Code?.Domain == "game" && b.Code.Path.StartsWith("brick")
                    );
                    if (!allBrick)
                    {
                        break;
                    }
                    validatedHeight++;
                }
                else if (ringMaterial == "stonebrick")
                {
                    bool allStone = neighbors.All(b =>
                        b?.Code?.Domain == "game" && b.Code.Path.StartsWith("stonebrick")
                    );
                    if (!allStone)
                    {
                        break;
                    }

                    validatedHeight++;
                }
            }
            return validatedHeight;
        }

        private bool IsSurroundedBySoil(IBlockAccessor blockAccessor, BlockPos pos)
        {
            Block[] blocks = new Block[]
            {
                blockAccessor.GetBlock(pos.NorthCopy()),
                blockAccessor.GetBlock(pos.EastCopy()),
                blockAccessor.GetBlock(pos.SouthCopy()),
                blockAccessor.GetBlock(pos.WestCopy())
            };

            return blocks.Any(b =>
                b?.Code?.Path.StartsWith("soil-") == true ||
                b?.Code?.Path.StartsWith("sand-") == true ||
                b?.Code?.Path.StartsWith("gravel-") == true
            );
        }
        private bool IsValidPlacement(IBlockAccessor blockAccessor, BlockPos pos)
        {
            Block northBlock = blockAccessor.GetBlock(pos.NorthCopy());
            Block eastBlock  = blockAccessor.GetBlock(pos.EastCopy());
            Block southBlock = blockAccessor.GetBlock(pos.SouthCopy());
            Block westBlock  = blockAccessor.GetBlock(pos.WestCopy());

            bool northSolid = northBlock != null && northBlock.SideSolid[BlockFacing.SOUTH.Index];
            bool eastSolid  = eastBlock  != null && eastBlock.SideSolid[BlockFacing.WEST.Index];
            bool southSolid = southBlock != null && southBlock.SideSolid[BlockFacing.NORTH.Index];
            bool westSolid  = westBlock  != null && westBlock.SideSolid[BlockFacing.EAST.Index];

            return northSolid && eastSolid && southSolid && westSolid;
        }
        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            accumulatedWater     = tree.GetDouble("accumulatedWater", 0.0);
            lastDailyLiters      = tree.GetDouble("lastDailyLiters", lastDailyLiters);
            cachedRingMaterial   = tree.GetString("cachedRingMaterial", cachedRingMaterial);
            partialValidatedHeight = tree.GetInt("partialValidatedHeight", partialValidatedHeight);
            lastWaterType        = tree.GetString("lastWaterType", lastWaterType);
            lastInGameTime       = tree.GetDouble("lastInGameTime", lastInGameTime);
            if (worldForResolving.Api.Side == EnumAppSide.Server)
            {
                _aquiferManager = HydrateOrDiedrateModSystem.AquiferManager;
            }
        }
        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetDouble("accumulatedWater", accumulatedWater);
            tree.SetDouble("lastDailyLiters", lastDailyLiters);
            tree.SetString("cachedRingMaterial", cachedRingMaterial);
            tree.SetString("lastWaterType", lastWaterType ?? "");
            tree.SetInt("partialValidatedHeight", partialValidatedHeight);
            tree.SetDouble("lastInGameTime", lastInGameTime);
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            base.GetBlockInfo(forPlayer, dsc);
            string description = Lang.Get("hydrateordiedrate:block-wellspring-description");
            dsc.AppendLine(description);
        }

        private (bool isSalty, bool isFresh) CheckForNearbyGameWater(IBlockAccessor blockAccessor, BlockPos centerPos)
        {
            bool saltyFound = false;
            bool freshFound = false;
            for (int dx = -3; dx <= 2; dx++)
            {
                for (int dy = -3; dy <= 2; dy++)
                {
                    for (int dz = -3; dz <= 2; dz++)
                    {
                        if (dx == 0 && dy == 0 && dz == 0) continue;

                        BlockPos checkPos = centerPos.AddCopy(dx, dy, dz);
                        Block checkBlock = blockAccessor.GetBlock(checkPos);

                        if (checkBlock?.Code?.Domain == "game")
                        {
                            if (checkBlock.Code.Path.StartsWith("saltwater-"))
                            {
                                saltyFound = true;
                            }
                            else if (checkBlock.Code.Path.StartsWith("water-") 
                                     || checkBlock.Code.Path.StartsWith("boilingwater-"))
                            {
                                freshFound = true;
                            }
                        }

                        if (saltyFound && freshFound) return (true, true);
                    }
                }
            }
            return (saltyFound, freshFound);
        }

        public string GetWaterType() => lastWaterType;

        public double GetCurrentOutputRate() => lastDailyLiters;

        public int GetRetentionDepth() => DetermineMaxDepthBasedOnCached(cachedRingMaterial, partialValidatedHeight);
    }
}