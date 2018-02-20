using System;
using System.Collections.Generic;
using Modding;

namespace BonfireMod
{
    [Serializable]
    public class BonfireModSettings : IModSettings
    {
        public int StrengthStat { get => GetInt(1); set => SetInt(value); }
        public int DexterityStat { get => GetInt(1); set => SetInt(value); }
        public int IntelligenceStat { get => GetInt(1); set => SetInt(value); }
        public int ResilienceStat { get => GetInt(1); set => SetInt(value); }
        public int WisdomStat { get => GetInt(1); set => SetInt(value); }
        public int LuckStat { get => GetInt(1); set => SetInt(value); }
        public int StrengthIncrease { get => GetInt(0); set => SetInt(value); }
        public int DexterityIncrease { get => GetInt(0); set => SetInt(value); }
        public int IntelligenceIncrease { get => GetInt(0); set => SetInt(value); }
        public int ResilienceIncrease { get => GetInt(0); set => SetInt(value); }
        public int WisdomIncrease { get => GetInt(0); set => SetInt(value); }
        public int LuckIncrease { get => GetInt(0); set => SetInt(value); }
        public int GeoToLvUp { get => GetInt(0); set => SetInt(value); }
        public int CurrentLv { get => GetInt(1); set => SetInt(value); }
        public int SpentGeo { get => GetInt(0); set => SetInt(value); }
        public int Respec { get => GetInt(1); set => SetInt(value); }
        public int FreeLevels { get => GetInt(0); set => SetInt(value); }
        public int GeoLevels { get => GetInt(0); set => SetInt(value); }
        public int RL3Levels { get => GetInt(0); set => SetInt(value); }
        public int RL4Levels { get => GetInt(0); set => SetInt(value); }
        public int TotalFreeLevels { get => GetInt(0); set => SetInt(value); }
        public int RelicLevels { get => GetInt(0); set => SetInt(value); }
        public int SpentFreeLevels { get => GetInt(0); set => SetInt(value); }
        public int TotalGeoLevels { get => GetInt(1); set => SetInt(value); }
        public int TotalSpentGeo { get => GetInt(0); set => SetInt(value); }
        public int SpentGeoLevels { get => GetInt(0); set => SetInt(value); }

        public Dictionary<string, string> BossRewards;
        public void FillBossRewards()
        {
            if (this.BossRewards == null && BonfireMod.Instance.BossRush)
            {
                BossRewards = new Dictionary<string, string>()
                {
                    { "killedBigFly", "trinket4" },
                    { "killedFalseKnight", "trinket4" },
                    { "killedMegaMossCharger", "trinket4" },
                    { "hornet1Defeated", "trinket4" },
                    { "killedMawlek", "trinket4" },
                    { "defeatedMantisLords", "trinket4" },
                    { "mageLordDefeated", "trinket4" },
                    { "killedMegaBeamMiner", "trinket4" },
                    { "killedMegaJellyfish", "trinket4" },
                    { "killedGhostXero", "trinket4" },
                    { "killedGhostAladar", "trinket4" },
                    { "killedGhostMarmu", "trinket4" },
                    { "killedTraitorLord", "trinket4" },
                    { "killedFlukeMother", "trinket4" },
                    { "defeatedDungDefender", "trinket4" },
                    { "killedGhostHu", "trinket4" },
                    { "killedGhostGalien", "trinket4" },
                    { "killedMimicSpider", "trinket4" },
                    { "killedGhostNoEyes", "trinket4" },
                    { "killedJarCollector", "trinket4" },
                    { "newDataLobsterLancer", "trinket4" },
                    { "killedInfectedKnight", "trinket4" },
                    { "newDataBlackKnight", "trinket4" },
                    { "newDataMegaBeamMiner", "trinket4" },
                    { "hornetOutskirtsDefeated", "trinket4" },
                    { "killedGhostMarkoth", "trinket4" },
                    { "killedHollowKnight", "trinket4" },
                    { "mageLordDreamDefeated", "trinket4" },
                    { "infectedKnightDreamDefeated", "trinket4" },
                    { "falseKnightDreamDefeated", "trinket4" },
                    { "killedGreyPrince", "trinket4" },
                    { "killedWhiteDefender", "trinket4" },
                    { "killedFinalBoss", "trinket4" }
                };
                BonfireMod.Instance.Log("Boss Rewards list filled!");
            }
        }

        public static BonfireModSettings _instance;
    }
}
