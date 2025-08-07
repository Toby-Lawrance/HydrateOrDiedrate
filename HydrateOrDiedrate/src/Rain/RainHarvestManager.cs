using HydrateOrDiedrate.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace HydrateOrDiedrate
{
    public struct ChunkPos
    {
        public int X;
        public int Y;
        public int Z;

        public ChunkPos(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static ChunkPos FromBlockPos(BlockPos pos)
        {
            return new ChunkPos(pos.X / 32, pos.Y / 32, pos.Z / 32);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChunkPos)) return false;
            ChunkPos other = (ChunkPos)obj;
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
    }

    public class RainHarvesterManager
    {
        private ICoreServerAPI serverAPI;
        private Dictionary<BlockPos, RainHarvesterData> activeHarvesters;
        private Dictionary<ChunkPos, Dictionary<BlockPos, RainHarvesterData>> inactiveHarvestersByChunk;
        private long tickListenerId;
        private int globalTickCounter = 1;
        private bool enableParticleTicking;

        public RainHarvesterManager(ICoreServerAPI api)
        {
            serverAPI = api;
            activeHarvesters = new Dictionary<BlockPos, RainHarvesterData>();
            inactiveHarvestersByChunk = new Dictionary<ChunkPos, Dictionary<BlockPos, RainHarvesterData>>();

            enableParticleTicking = ModConfig.Instance.Rain.EnableRainGathering;

            if (enableParticleTicking)
            {
                tickListenerId = api.Event.RegisterGameTickListener(ScheduleTickProcessing, 2000);
                api.Event.RegisterGameTickListener(OnInactiveHarvesterCheck, 10000);
            }
        }

        public void RegisterHarvester(BlockPos position, RainHarvesterData data)
        {
            float rainIntensity = data.GetRainIntensity();
            if (IsRainyAndOpenToSky(data, rainIntensity))
            {
                activeHarvesters[position] = data;
            }
            else
            {
                ChunkPos chunkKey = ChunkPos.FromBlockPos(position);
                if (!inactiveHarvestersByChunk.TryGetValue(chunkKey, out var chunkDict))
                {
                    chunkDict = new Dictionary<BlockPos, RainHarvesterData>();
                    inactiveHarvestersByChunk[chunkKey] = chunkDict;
                }
                chunkDict[position] = data;
            }
        }

        public void UnregisterHarvester(BlockPos position)
        {
            activeHarvesters.Remove(position);
            ChunkPos chunkKey = ChunkPos.FromBlockPos(position);
            if (inactiveHarvestersByChunk.TryGetValue(chunkKey, out var chunkDict))
            {
                chunkDict.Remove(position);
                if (chunkDict.Count == 0)
                {
                    inactiveHarvestersByChunk.Remove(chunkKey);
                }
            }
        }

        public void OnBlockRemoved(BlockPos position)
        {
            UnregisterHarvester(position);
        }

        public void OnBlockUnloaded(BlockPos position)
        {
            UnregisterHarvester(position);
        }

        private void ScheduleTickProcessing(float deltaTime)
        {
            if (!serverAPI.World.AllOnlinePlayers.Any()) return;
            ProcessTick(deltaTime);
        }

        private void ProcessTick(float deltaTime)
        {
            globalTickCounter = (globalTickCounter % 10) + 1;
            var toRemove = new List<BlockPos>();

            float speedOfTime = serverAPI.World.Calendar.SpeedOfTime;
            float calendarSpeedMul = serverAPI.World.Calendar.CalendarSpeedMul;
            foreach (var kvp in activeHarvesters)
            {
                ProcessHarvesterTick(kvp, deltaTime, speedOfTime, calendarSpeedMul, toRemove);
            }
            foreach (var pos in toRemove)
            {
                if (activeHarvesters.TryGetValue(pos, out var removedHarvester))
                {
                    activeHarvesters.Remove(pos);
                    ChunkPos chunkKey = ChunkPos.FromBlockPos(pos);
                    if (!inactiveHarvestersByChunk.TryGetValue(chunkKey, out var chunkDict))
                    {
                        chunkDict = new Dictionary<BlockPos, RainHarvesterData>();
                        inactiveHarvestersByChunk[chunkKey] = chunkDict;
                    }
                    chunkDict[pos] = removedHarvester;
                }
            }
        }

        private void ProcessHarvesterTick(KeyValuePair<BlockPos, RainHarvesterData> kvp, float deltaTime, float speedOfTime, float calendarSpeedMul, List<BlockPos> toRemove)
        {
            BlockPos position = kvp.Key;
            RainHarvesterData harvesterData = kvp.Value;
            float rainIntensity = harvesterData.GetRainIntensity();

            if (enableParticleTicking)
            {
                harvesterData.OnParticleTickUpdate(deltaTime);
            }

            harvesterData.UpdateCalculatedTickInterval(deltaTime, speedOfTime, calendarSpeedMul, rainIntensity);
            int tickIntervalDifference = harvesterData.calculatedTickInterval - harvesterData.previousCalculatedTickInterval;
            harvesterData.adaptiveTickInterval += tickIntervalDifference;

            if (harvesterData.adaptiveTickInterval <= globalTickCounter &&
                harvesterData.previousCalculatedTickInterval > globalTickCounter)
            {
                harvesterData.adaptiveTickInterval = globalTickCounter + 2;
            }

            if (harvesterData.adaptiveTickInterval > 10)
            {
                harvesterData.adaptiveTickInterval -= 9;
            }

            if (globalTickCounter == harvesterData.adaptiveTickInterval)
            {
                int newAdaptiveTickInterval = harvesterData.adaptiveTickInterval + harvesterData.calculatedTickInterval;
                if (newAdaptiveTickInterval > 10)
                {
                    newAdaptiveTickInterval -= 10;
                }

                harvesterData.adaptiveTickInterval = newAdaptiveTickInterval;
                harvesterData.OnHarvest(rainIntensity);

                if (!IsRainyAndOpenToSky(harvesterData, rainIntensity))
                {
                    toRemove.Add(position);
                }
            }
        }

        private void OnInactiveHarvesterCheck(float deltaTime)
        {
            WeatherSystemServer weatherSys = serverAPI.ModLoader.GetModSystem<WeatherSystemServer>();
            IBlockAccessor blockAccessor = serverAPI.World.BlockAccessor;
            List<BlockPos> toActivate = new List<BlockPos>();
            foreach (var kvp in inactiveHarvestersByChunk)
            {
                ChunkPos chunkKey = kvp.Key;
                double centerX = chunkKey.X * 32 + 16;
                double centerY = chunkKey.Y * 32 + 16;
                double centerZ = chunkKey.Z * 32 + 16;
                Vec3d chunkCenter = new Vec3d(centerX, centerY, centerZ);
                BlockPos centerBlockPos = new BlockPos((int)centerX, (int)centerY, (int)centerZ);
                int distanceToRain = blockAccessor.GetDistanceToRainFall(centerBlockPos, 4, 1);
                if (distanceToRain > 32)
                {
                    continue;
                }
                foreach (var harvesterKvp in kvp.Value)
                {
                    BlockPos pos = harvesterKvp.Key;
                    RainHarvesterData harvesterData = harvesterKvp.Value;
                    if (harvesterData.IsOpenToSky(pos))
                    {
                        toActivate.Add(pos);
                    }
                }
            }
            foreach (var pos in toActivate)
            {
                ChunkPos chunkKey = ChunkPos.FromBlockPos(pos);
                if (inactiveHarvestersByChunk.TryGetValue(chunkKey, out var chunkDict))
                {
                    if (chunkDict.TryGetValue(pos, out var harvesterData))
                    {
                        chunkDict.Remove(pos);
                        if (chunkDict.Count == 0)
                        {
                            inactiveHarvestersByChunk.Remove(chunkKey);
                        }

                        activeHarvesters[pos] = harvesterData;
                    }
                }
            }
        }

        private bool IsRainyAndOpenToSky(RainHarvesterData harvesterData, float rainIntensity)
        {
            return rainIntensity > 0 && harvesterData.IsOpenToSky(harvesterData.BlockEntity.Pos);
        }
    }
}
