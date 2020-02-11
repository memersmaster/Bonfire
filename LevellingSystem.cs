using System;
using GlobalEnums;
using HutongGames.PlayMaker;
using UnityEngine;
using Modding;

namespace BonfireMod
{
    public class LevellingSystem : MonoBehaviour
    {
        public LevellingSystem() { }

        public static LevellingSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LevellingSystem();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private static GUIStyle labelStyle;

        private static GUIStyle buttonStyle;

        public bool CanLevelUp()
        {
            return (HeroController.instance.cState.nearBench)&&(gotFreeLevel || BonfireMod.Instance.Settings.GeoToLvUp + BonfireMod.Instance.Settings.SpentGeo <= PlayerData.instance.geo);
        }
        public void OnGUI()
        {
            if (GameManager.instance == null) return;
            if (!(GameManager.instance.gameState == GameState.PLAYING || GameManager.instance.gameState == GameState.PAUSED) || InInventory()) return;

            if (trajanBold == null || trajanNormal == null)
            {
                foreach (Font font in Resources.FindObjectsOfTypeAll<Font>())
                {
                    if (font != null && font.name == "TrajanPro-Bold")
                    {
                        trajanBold = font;
                    }
                    if (font != null && font.name == "TrajanPro-Regular")
                    {
                        trajanNormal = font;
                    }
                }
            }
            if (HeroController.instance.cState.nearBench || GameManager.instance.isPaused)
            {
                GUI.enabled = true;
                if (labelStyle == null)
                    labelStyle = new GUIStyle(GUI.skin.label)
                    {
                        font = trajanNormal,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 15
                    };
                if (buttonStyle == null)
                    buttonStyle = new GUIStyle(GUI.skin.button)
                    {
                        font = trajanBold,
                        fontStyle = FontStyle.Normal,
                        fontSize = 15,
                        alignment = TextAnchor.MiddleCenter
                    };
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
                GUI.color = Color.white;
                BonfireMod.Instance.Settings.RelicLevels = BonfireMod.Instance.Settings.TotalFreeLevels + BonfireMod.Instance.Settings.SpentFreeLevels;
                BonfireMod.Instance.Settings.CurrentLv = BonfireMod.Instance.Settings.StrengthStat + BonfireMod.Instance.Settings.DexterityStat + BonfireMod.Instance.Settings.LuckStat + BonfireMod.Instance.Settings.ResilienceStat + BonfireMod.Instance.Settings.WisdomStat + BonfireMod.Instance.Settings.IntelligenceStat + BonfireMod.Instance.Settings.SpentGeoLevels + BonfireMod.Instance.Settings.SpentFreeLevels - 5;
                if (BonfireMod.Instance.Settings.CurrentLv == 1)
                    BonfireMod.Instance.Settings.TotalGeoLevels = 1;
                BonfireMod.Instance.Settings.GeoLevels = BonfireMod.Instance.Settings.TotalGeoLevels + BonfireMod.Instance.Settings.SpentGeoLevels;
                BonfireMod.Instance.Settings.GeoToLvUp = (int)(Math.Pow((double)BonfireMod.Instance.Settings.GeoLevels, 2.0) + (double)(10 * BonfireMod.Instance.Settings.GeoLevels) + 50.0);
                BonfireMod.Instance.Settings.FreeLevels = BonfireMod.Instance.Settings.RL3Levels + BonfireMod.Instance.Settings.RL4Levels;
                gotFreeLevel = !(BonfireMod.Instance.Settings.FreeLevels == 0);
                string geoToLevelUp = BonfireMod.Instance.Settings.GeoToLvUp.ToString();
                string totalInt = (BonfireMod.Instance.Settings.IntelligenceStat + BonfireMod.Instance.Settings.IntelligenceIncrease).ToString();
                string totalStr = (BonfireMod.Instance.Settings.StrengthStat + BonfireMod.Instance.Settings.StrengthIncrease).ToString();
                string totalDex = (BonfireMod.Instance.Settings.DexterityStat + BonfireMod.Instance.Settings.DexterityIncrease).ToString();
                string totalLck = (BonfireMod.Instance.Settings.LuckStat + BonfireMod.Instance.Settings.LuckIncrease).ToString();
                string totalRes = (BonfireMod.Instance.Settings.ResilienceStat + BonfireMod.Instance.Settings.ResilienceIncrease).ToString();
                string totalWsdm = (BonfireMod.Instance.Settings.WisdomStat + BonfireMod.Instance.Settings.WisdomIncrease).ToString();
                string nailDamage = NailDamage(BonfireMod.Instance.Settings.StrengthIncrease + BonfireMod.Instance.Settings.StrengthStat).ToString();
                string attackSpeed = AttackSpeed(BonfireMod.Instance.Settings.DexterityIncrease + BonfireMod.Instance.Settings.DexterityStat).ToString();
                string extraMasks = ExtraMasks(BonfireMod.Instance.Settings.ResilienceStat + BonfireMod.Instance.Settings.ResilienceIncrease).ToString();
                string extraSoul = ExtraSoul(BonfireMod.Instance.Settings.WisdomStat + BonfireMod.Instance.Settings.WisdomIncrease, 11).ToString();
                string critChance = CritChance(BonfireMod.Instance.Settings.LuckStat + BonfireMod.Instance.Settings.LuckIncrease).ToString();
                string critDamage = CritDamage(BonfireMod.Instance.Settings.DexterityIncrease + BonfireMod.Instance.Settings.DexterityStat, 100).ToString();
                string geoIncrease = (5 * (BonfireMod.Instance.Settings.LuckStat + BonfireMod.Instance.Settings.LuckIncrease - 1)).ToString();
                string focusCost = FocusCost(BonfireMod.Instance.Settings.IntelligenceStat + BonfireMod.Instance.Settings.IntelligenceIncrease).ToString();
                string soulRegen = SoulRegen(BonfireMod.Instance.Settings.WisdomStat + BonfireMod.Instance.Settings.WisdomIncrease).ToString();
                string spellDamage = SpellDamage(100, BonfireMod.Instance.Settings.IntelligenceStat + BonfireMod.Instance.Settings.IntelligenceIncrease).ToString();
                string expectedHits;
                if (BonfireMod.Instance.Settings.ResilienceStat + BonfireMod.Instance.Settings.ResilienceIncrease > 1)
                {
                    expectedHits = ExpectedHits(BonfireMod.Instance.Settings.ResilienceStat + BonfireMod.Instance.Settings.ResilienceIncrease).ToString();
                }
                else
                {
                    expectedHits = "0";
                }
                string applyText;
                if (BonfireMod.Instance.Settings.RL3Levels <= 0)
                {
                    if (BonfireMod.Instance.Settings.RL4Levels <= 0)
                    {
                        if (BonfireMod.Instance.Settings.SpentFreeLevels > 0)
                        {
                            applyText = string.Concat("Apply (", BonfireMod.Instance.Settings.SpentGeo, " geo and ", BonfireMod.Instance.Settings.SpentFreeLevels, " relics)");
                        }
                        else
                        {
                            applyText = "Apply (" + BonfireMod.Instance.Settings.SpentGeo + " geo)";
                        }
                    }
                    else
                    {
                        applyText = BonfireMod.Instance.Settings.RL4Levels + " Free Levels!\n(Arcane Egg)";
                    }
                }
                else
                {
                    applyText = BonfireMod.Instance.Settings.RL3Levels + " Free Levels!\n(King's Idol)";
                }
                GUILayout.BeginArea(new Rect(20f, (float)(Screen.height / 4), 530f, 500f));
                GUILayout.Label("Level Up", labelStyle);
                GUILayout.BeginHorizontal("box");
                GUILayout.BeginVertical("box");
                if (GUILayout.Button(new GUIContent("Strength: " + totalStr), buttonStyle, GUILayout.Height(40f), GUILayout.Width(160f)) && CanLevelUp() && HeroController.instance.cState.nearBench)
                {
                    IncreaseStat("Strength");
                }
                if (GUILayout.Button(new GUIContent("Dexterity: " + totalDex), buttonStyle, GUILayout.Height(40f), GUILayout.Width(160f)) && CanLevelUp() && HeroController.instance.cState.nearBench)
                {
                    IncreaseStat("Dexterity");
                }
                if (GUILayout.Button(new GUIContent("Intelligence: " + totalInt), buttonStyle, GUILayout.Height(40f), GUILayout.Width(160f)) && CanLevelUp() && HeroController.instance.cState.nearBench)
                {
                    IncreaseStat("Intelligence");
                }
                if (GUILayout.Button(new GUIContent("Resilience: " + totalRes), buttonStyle, GUILayout.Height(40f), GUILayout.Width(160f)) && CanLevelUp() && HeroController.instance.cState.nearBench)
                {
                    IncreaseStat("Resilience");
                }
                if (GUILayout.Button(new GUIContent("Wisdom: " + totalWsdm), buttonStyle, GUILayout.Height(40f), GUILayout.Width(160f)) && CanLevelUp() && HeroController.instance.cState.nearBench)
                {
                    IncreaseStat("Wisdom");
                }
                if (GUILayout.Button(new GUIContent("Luck: " + totalLck), buttonStyle, GUILayout.Height(40f), GUILayout.Width(160f)) && CanLevelUp() && HeroController.instance.cState.nearBench)
                {
                    IncreaseStat("Luck");
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical("box");
                GUILayout.Label(new GUIContent("Nail Damage: " + nailDamage), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.Label(new GUIContent("Slash Speed: " + attackSpeed), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.Label(new GUIContent("Spell Damage: " + spellDamage + "%"), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.Label(new GUIContent("Extra Masks: " + extraMasks), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.Label(new GUIContent("SOUL Gained: " + extraSoul), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.Label(new GUIContent("Crit Chance: " + critChance + "%"), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.EndVertical();
                GUILayout.BeginVertical("box");
                GUILayout.Label(new GUIContent(""), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.Label(new GUIContent("Crit Damage: " + critDamage + "%"), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.Label(new GUIContent("Focus Cost: " + focusCost), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.Label(new GUIContent("Hit Resistance: " + expectedHits + "%"), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.Label(new GUIContent("SOUL Regen: " + soulRegen), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.Label(new GUIContent("Geo Increase: " + geoIncrease + "%"), labelStyle, GUILayout.Height(40f), GUILayout.Width(160f));
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Label(new GUIContent("Current Level: " + BonfireMod.Instance.Settings.CurrentLv.ToString()), labelStyle);
                GUILayout.Label(new GUIContent("Geo to Level Up: " + geoToLevelUp), labelStyle);
                GUILayout.BeginHorizontal("box");
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button(new GUIContent(applyText), buttonStyle, GUILayout.Height(40f), GUILayout.Width(258f)) && HeroController.instance.cState.nearBench)
                {
                    ApplyLevel();
                }
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = Color.white;
                if (GUILayout.Button(new GUIContent("Cancel"), buttonStyle, GUILayout.Height(40f), GUILayout.Width(258f)) && HeroController.instance.cState.nearBench)
                {
                    BonfireMod.Instance.Settings.SpentGeo = 0;
                    BonfireMod.Instance.Settings.StrengthIncrease = 0;
                    BonfireMod.Instance.Settings.DexterityIncrease = 0;
                    BonfireMod.Instance.Settings.WisdomIncrease = 0;
                    BonfireMod.Instance.Settings.ResilienceIncrease = 0;
                    BonfireMod.Instance.Settings.IntelligenceIncrease = 0;
                    BonfireMod.Instance.Settings.LuckIncrease = 0;
                    BonfireMod.Instance.Settings.RL3Levels = PlayerData.instance.trinket3;
                    BonfireMod.Instance.Settings.RL4Levels = PlayerData.instance.trinket4;
                    BonfireMod.Instance.Settings.FreeLevels = BonfireMod.Instance.Settings.RL3Levels + BonfireMod.Instance.Settings.RL4Levels;
                    BonfireMod.Instance.Settings.SpentFreeLevels = 0;
                    BonfireMod.Instance.Settings.SpentGeoLevels = 0;
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal("box");
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button(new GUIContent("Respec (" + BonfireMod.Instance.Settings.Respec.ToString() + "  Rancid Egg)"), buttonStyle, GUILayout.Height(40f), GUILayout.Width(522f)) && PlayerData.instance.rancidEggs >= BonfireMod.Instance.Settings.Respec && HeroController.instance.cState.nearBench)
                {
                    Respec();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            if (!HeroController.instance.cState.nearBench)
            {
                BonfireMod.Instance.Settings.SpentGeo = 0;
                BonfireMod.Instance.Settings.StrengthIncrease = 0;
                BonfireMod.Instance.Settings.DexterityIncrease = 0;
                BonfireMod.Instance.Settings.WisdomIncrease = 0;
                BonfireMod.Instance.Settings.ResilienceIncrease = 0;
                BonfireMod.Instance.Settings.IntelligenceIncrease = 0;
                BonfireMod.Instance.Settings.LuckIncrease = 0;
                BonfireMod.Instance.Settings.RL3Levels = PlayerData.instance.trinket3;
                BonfireMod.Instance.Settings.RL4Levels = PlayerData.instance.trinket4;
                BonfireMod.Instance.Settings.FreeLevels = BonfireMod.Instance.Settings.RL3Levels + BonfireMod.Instance.Settings.RL4Levels;
                BonfireMod.Instance.Settings.SpentFreeLevels = 0;
                BonfireMod.Instance.Settings.SpentGeoLevels = 0;
            }
        }

        public static bool InInventory()
        {
            BonfireMod.Instance.Log("Checking inventory open");
            GameObject gameObject = GameObject.FindGameObjectWithTag("Inventory Top");
            if (gameObject == null) return false;
            Modding.Logger.Log($"Inventory object {gameObject}");
            PlayMakerFSM component = FSMUtility.LocateFSM(gameObject, "Inventory Control");
            if (component == null) return false;
            Modding.Logger.Log($"Inventory control fsm {component}");
            FsmBool fsmBool = component.FsmVariables.GetFsmBool("Open");
            Modding.Logger.Log($"Inventory open {fsmBool}");
            return fsmBool != null && fsmBool.Value;
        }


        public int NailDamage(int totalStr) => (int)Math.Round((double)(5 + 4 * PlayerData.instance.nailSmithUpgrades) * Math.Pow(1.25, Math.Log((double)totalStr, 2.0)));


        public int ExtraMasks(int totalRes) => (int)Math.Round(-0.4 + 2.6 * Math.Log((double)totalRes));


        public float FocusCost(int totalInt) => (float)Math.Round(34.0 * Math.Exp(-0.01 * ((double)totalInt + 1.0)));


        public int CritChance(int totalLck) => (int)Math.Round(6.5 * Math.Log(totalLck));


        public int DroppedGeo(int totalLck) => 1 + totalLck / 20;


        public float AttackSpeed(int totalDex) => (float)Math.Round(2.7 / (1.0 + 1.82 * Math.Exp(-0.08 * (double)totalDex)) - 0.01, 2);


        public void Respec()
        {
            PlayerData.instance.AddGeo(BonfireMod.Instance.Settings.TotalSpentGeo);
            HeroController.instance.AddGeoToCounter(BonfireMod.Instance.Settings.TotalSpentGeo);
            PlayerData.instance.trinket3 += BonfireMod.Instance.Settings.TotalFreeLevels;
            PlayMakerFSM.BroadcastEvent("TRINK 3");
            BonfireMod.Instance.Settings.StrengthStat = 1;
            BonfireMod.Instance.Settings.DexterityStat = 1;
            BonfireMod.Instance.Settings.ResilienceStat = 1;
            BonfireMod.Instance.Settings.WisdomStat = 1;
            BonfireMod.Instance.Settings.IntelligenceStat = 1;
            BonfireMod.Instance.Settings.LuckStat = 1;
            BonfireMod.Instance.Settings.StrengthIncrease = 0;
            BonfireMod.Instance.Settings.DexterityIncrease = 0;
            BonfireMod.Instance.Settings.IntelligenceIncrease = 0;
            BonfireMod.Instance.Settings.ResilienceIncrease = 0;
            BonfireMod.Instance.Settings.WisdomIncrease = 0;
            BonfireMod.Instance.Settings.LuckIncrease = 0;
            BonfireMod.Instance.Settings.SpentGeo = 0;
            BonfireMod.Instance.Settings.TotalSpentGeo = 0;
            BonfireMod.Instance.Settings.FreeLevels = 0;
            BonfireMod.Instance.Settings.TotalFreeLevels = 0;
            BonfireMod.Instance.Settings.RL3Levels = 0;
            BonfireMod.Instance.Settings.GeoLevels = 0;
            BonfireMod.Instance.Settings.TotalGeoLevels = 1;
            BonfireMod.Instance.Settings.SpentGeoLevels = 0;
            BonfireMod.Instance.Settings.RelicLevels = 0;
            BonfireMod.Instance.Settings.CurrentLv = 1;
            PlayerData.instance.rancidEggs -= BonfireMod.Instance.Settings.Respec;
            BonfireMod.Instance.Settings.Respec += 1;
            PlayerData.UpdateBlueHealth();
        }


        public void IncreaseStat(string stat)
        {
            if (stat == "Strength")
            {
                BonfireMod.Instance.Settings.StrengthIncrease++;
            }
            else if (stat == "Dexterity")
            {
                BonfireMod.Instance.Settings.DexterityIncrease++;
            }
            else if (stat == "Intelligence")
            {
                BonfireMod.Instance.Settings.IntelligenceIncrease++;
            }
            else if (stat == "Resilience")
            {
                BonfireMod.Instance.Settings.ResilienceIncrease++;
            }
            else if (stat == "Wisdom")
            {
                BonfireMod.Instance.Settings.WisdomIncrease++;
            }
            else
            {
                BonfireMod.Instance.Settings.LuckIncrease++;
            }
            if (BonfireMod.Instance.Settings.RL3Levels <= 0)
            {
                if (BonfireMod.Instance.Settings.RL4Levels <= 0)
                {
                    BonfireMod.Instance.Settings.SpentGeo += BonfireMod.Instance.Settings.GeoToLvUp;
                    BonfireMod.Instance.Settings.SpentGeoLevels++;
                }
                else
                {
                    BonfireMod.Instance.Settings.RL4Levels--;
                    BonfireMod.Instance.Settings.SpentFreeLevels++;
                }
            }
            else
            {
                BonfireMod.Instance.Settings.RL3Levels--;
                BonfireMod.Instance.Settings.SpentFreeLevels++;
            }
        }
        public void ApplyLevel()
        {
            PlayerData.instance.TakeGeo(BonfireMod.Instance.Settings.SpentGeo);
            PlayerData.instance.trinket3 = BonfireMod.Instance.Settings.RL3Levels;
            PlayerData.instance.trinket4 = BonfireMod.Instance.Settings.RL4Levels;
            HeroController.instance.geoCounter.TakeGeo(BonfireMod.Instance.Settings.SpentGeo);
            BonfireMod.Instance.Settings.TotalSpentGeo += BonfireMod.Instance.Settings.SpentGeo;
            BonfireMod.Instance.Settings.SpentGeo = 0;
            BonfireMod.Instance.Settings.TotalGeoLevels += BonfireMod.Instance.Settings.SpentGeoLevels;
            BonfireMod.Instance.Settings.SpentGeoLevels = 0;
            BonfireMod.Instance.Settings.TotalFreeLevels += BonfireMod.Instance.Settings.SpentFreeLevels;
            BonfireMod.Instance.Settings.SpentFreeLevels = 0;
            BonfireMod.Instance.Settings.StrengthStat += BonfireMod.Instance.Settings.StrengthIncrease;
            BonfireMod.Instance.Settings.DexterityStat += BonfireMod.Instance.Settings.DexterityIncrease;
            BonfireMod.Instance.Settings.ResilienceStat += BonfireMod.Instance.Settings.ResilienceIncrease;
            BonfireMod.Instance.Settings.WisdomStat += BonfireMod.Instance.Settings.WisdomIncrease;
            BonfireMod.Instance.Settings.IntelligenceStat += BonfireMod.Instance.Settings.IntelligenceIncrease;
            BonfireMod.Instance.Settings.LuckStat += BonfireMod.Instance.Settings.LuckIncrease;
            HeroController.instance.CharmUpdate();
            PlayerData.instance.UpdateBlueHealth();
            PlayMakerFSM.BroadcastEvent("UPDATE BLUE HEALTH");
            BonfireMod.Instance.Log("Level up applied: " + BonfireMod.Instance.Settings.StrengthIncrease + " Strength, " + BonfireMod.Instance.Settings.DexterityIncrease +
                " Dexterity, " + BonfireMod.Instance.Settings.IntelligenceIncrease + " Intelligence, " + BonfireMod.Instance.Settings.ResilienceIncrease + " Resilience, " +
                BonfireMod.Instance.Settings.WisdomIncrease + " Wisdom and " + BonfireMod.Instance.Settings.LuckIncrease + " Luck.");
            BonfireMod.Instance.Settings.StrengthIncrease = 0;
            BonfireMod.Instance.Settings.DexterityIncrease = 0;
            BonfireMod.Instance.Settings.WisdomIncrease = 0;
            BonfireMod.Instance.Settings.ResilienceIncrease = 0;
            BonfireMod.Instance.Settings.IntelligenceIncrease = 0;
            BonfireMod.Instance.Settings.LuckIncrease = 0;
            PlayerData.UpdateBlueHealth();
        }


        public int IncreaseGeo(int droppedGeo, int totalLck) => (int)(droppedGeo * (1f + (totalLck - 1) / 20f));


        public int CritDamage(int totalDex, int nailDamage) => (int)(nailDamage * (1.2 + Math.Log(totalDex)));


        public int SpellDamage(int baseDamage, int totalInt) => (int)Math.Round(baseDamage * Math.Pow(1.25, Math.Log(totalInt, 2.0)));


        public int ExtraSoul(int totalWsdm, int baseSoul) => (int)Math.Round(baseSoul + 5.0 * Math.Log(totalWsdm));


        public int SoulRegen(int totalWsdm) => (int)Math.Round(0.32 + 0.68 * Math.Log(totalWsdm));


        public float IFrames(int totalRes) => (float)(3.25 / (1.0 + 2.4 * Math.Exp(-0.07 * (totalRes - 1))));


        public float IFramesChance(int totalRes, int hitsTaken)
        {
            if (hitsTaken > 7)
            {
                hitsTaken = 7;
            }
            switch (hitsTaken)
            {
                case 1:
                    return 0.1f * IFrames(totalRes);
                case 2:
                    return (1f - 0.1f * IFrames(totalRes)) * 0.2f * IFrames(totalRes);
                case 3:
                    return (1f - 0.1f * IFrames(totalRes)) * (1f - 0.2f * IFrames(totalRes)) * 0.3f * IFrames(totalRes);
                case 4:
                    return (1f - 0.1f * IFrames(totalRes)) * (1f - 0.2f * IFrames(totalRes)) * (1f - 0.3f * IFrames(totalRes)) * 0.5f * IFrames(totalRes);
                case 5:
                    return (1f - 0.1f * IFrames(totalRes)) * (1f - 0.2f * IFrames(totalRes)) * (1f - 0.3f * IFrames(totalRes)) * (1f - 0.5f * IFrames(totalRes)) * 0.7f * IFrames(totalRes);
                case 6:
                    return (1f - 0.1f * IFrames(totalRes)) * (1f - 0.2f * IFrames(totalRes)) * (1f - 0.3f * IFrames(totalRes)) * (1f - 0.5f * IFrames(totalRes)) * (1f - 0.7f * IFrames(totalRes)) * 0.8f * IFrames(totalRes);
                case 7:
                    return (1f - 0.1f * IFrames(totalRes)) * (1f - 0.2f * IFrames(totalRes)) * (1f - 0.3f * IFrames(totalRes)) * (1f - 0.5f * IFrames(totalRes)) * (1f - 0.7f * IFrames(totalRes)) * (1f - 0.8f * IFrames(totalRes)) * 0.9f * IFrames(totalRes);
                default:
                    return 0f;
            }
        }


        public float ExpectedHits(int totalRes)
        {
            float num = 0f;
            for (int i = 1; i < 8; i++)
            {
                num += (float)(i + 1) * IFramesChance(totalRes, i);
            }
            return (float)Math.Round((double)(100f / num));
        }



        //public static BonfireMod BonfireMod;


        //public BonfireModSettings BonfireModSettings;


        public static LevellingSystem _instance;


        public PlayerData PlayerData;


        public GameManager GameManager;


        public bool levellingUp;


        public HeroController HeroController;


        public PlayMakerFSM PlayMakerFSM;


        public Font trajanBold;


        public Font trajanNormal;


        public bool gotFreeLevel;

        public int freeLevelsSpent;

        public int geoToLvUp;

        public GameObject inventory;
    }

}
