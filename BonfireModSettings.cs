using System;
using System.Collections.Generic;

namespace BonfireMod
{
    [Serializable]
    public class BonfireModSettings
    {
        public int StrengthStat = 1;
        public int DexterityStat = 1;
        public int IntelligenceStat = 1;
        public int ResilienceStat = 1;
        public int WisdomStat = 1;
        public int LuckStat = 1;
        public int StrengthIncrease = 0;
        public int DexterityIncrease = 0;
        public int IntelligenceIncrease = 0;
        public int ResilienceIncrease = 0;
        public int WisdomIncrease = 0;
        public int LuckIncrease = 0;
        public int GeoToLvUp = 0;
        public int CurrentLv = 1;
        public int SpentGeo = 0;
        public int Respec = 1;
        public int FreeLevels = 0;
        public int GeoLevels = 0;
        public int RL3Levels = 0;
        public int RL4Levels = 0;
        public int TotalFreeLevels = 0;
        public int RelicLevels = 0;
        public int SpentFreeLevels = 0;
        public int TotalGeoLevels = 1;
        public int TotalSpentGeo = 0;
        public int SpentGeoLevels = 0;

        public Dictionary<string, string> BossRewards;
        public void FillBossRewards()
        {
            if (BossRewards == null && BonfireMod.Instance.BossRush)
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
