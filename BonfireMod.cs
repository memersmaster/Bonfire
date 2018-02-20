using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Reflection;
using HutongGames.PlayMaker.Actions;
using Modding;
using UnityEngine.SceneManagement;


namespace BonfireMod
{
    public class BonfireMod : Mod<BonfireModSettings>, IMod, Modding.ILogger, ITogglableMod
    {
        public static BonfireMod Instance;
        private static readonly FieldInfo[] ActionDataFields = typeof(ActionData).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private static readonly FieldInfo ActionSpellData = ActionDataFields.Single(x => x.Name == "byteData");
        public override void Initialize()
        {
            Instance = this;
            Instance.Log("Bonfire Mod initializing!");

            ModHooks.Instance.NewGameHook += SetupGameRefs;
            ModHooks.Instance.SavegameLoadHook += SetupGameRefs;
            ModHooks.Instance.CharmUpdateHook += BenchApply;
            ModHooks.Instance.DoAttackHook += CalculateCrit;
            ModHooks.Instance.SoulGainHook += SoulGain;
            ModHooks.Instance.HeroUpdateHook += MpRegen;
            ModHooks.Instance.BlueHealthHook += BlueHealth;
            ModHooks.Instance.FocusCostHook += FocusCost;
            ModHooks.Instance.SlashHitHook += CritHit;
            ModHooks.Instance.CursorHook += ShowCursor;

            ModHooks.Instance.OnGetEventSenderHook += SpellDamage;
            ModHooks.Instance.AfterTakeDamageHook += ResShield;
            //ModHooks.Instance.SetPlayerBoolHook += LookForBoss;
            ModHooks.Instance.SceneChanged += onSceneLoad;
            //UnityEngine.SceneManagement.SceneManager.sceneLoaded += onSceneLoad;

            Instance.Log("Bonfire Mod v." + GetVersion() + " initialized!");
        }

        public void Unload()
        {
            ModHooks.Instance.NewGameHook -= SetupGameRefs;
            ModHooks.Instance.SavegameLoadHook -= SetupGameRefs;
            ModHooks.Instance.CharmUpdateHook -= BenchApply;
            ModHooks.Instance.DoAttackHook -= CalculateCrit;
            ModHooks.Instance.SoulGainHook -= SoulGain;
            ModHooks.Instance.HeroUpdateHook -= MpRegen;
            ModHooks.Instance.BlueHealthHook -= BlueHealth;
            ModHooks.Instance.FocusCostHook -= FocusCost;
            ModHooks.Instance.SlashHitHook -= CritHit;
            ModHooks.Instance.CursorHook -= ShowCursor;

            ModHooks.Instance.OnGetEventSenderHook -= SpellDamage;
            ModHooks.Instance.AfterTakeDamageHook -= ResShield;
            ModHooks.Instance.SetPlayerBoolHook -= LookForBoss;
            ModHooks.Instance.SceneChanged -= onSceneLoad;

            pd.nailDamage = 5 + 4 * pd.nailSmithUpgrades;
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
            HeroController.instance.ATTACK_DURATION = 0.35f;
            HeroController.instance.ATTACK_DURATION_CH = 0.25f;
            HeroController.instance.ATTACK_COOLDOWN_TIME = 0.41f;
            HeroController.instance.ATTACK_COOLDOWN_TIME_CH = 0.25f;

            Instance.Log("Bonfire Mod disabled!");
        }

        public int ResShield(int hazardType, int damage)
        {
            if (Settings.ResilienceStat > 1 && hazardType == 1)
            {
                if (HitsSinceShielded > 7)
                {
                    HitsSinceShielded = 7;
                }
                float num = UnityEngine.Random.Range(1, 100);
                float iframes = ls.IFrames(Settings.ResilienceStat);
                float multiplier = 0f;
                switch (this.HitsSinceShielded)
                {
                    case 1:
                        multiplier = 10f;
                        break;
                    case 2:
                        multiplier = 20f;
                        break;
                    case 3:
                        multiplier = 30f;
                        break;
                    case 4:
                        multiplier = 50f;
                        break;
                    case 5:
                        multiplier = 70f;
                        break;
                    case 6:
                        multiplier = 80f;
                        break;
                    case 7:
                        multiplier = 90f;
                        break;
                }
                LogDebug($"{num} <= {multiplier} * {iframes}");
                if (multiplier > 0 && num <= multiplier * iframes)
                {
                    this.HitsSinceShielded = 0;
                    hc.carefreeShield.SetActive(true);
                    damage = 0;
                }
                else
                {
                    this.HitsSinceShielded++;
                }
            }
            return damage;
        }

        public GameObject SpellDamage(GameObject go, Fsm pmfsm)
        {


            switch (go.name)
            {
                case "Fireball(Clone)":
                case "Fireball":
                case "Hit L": // Dive
                case "Hit R": // Dive
                case "Q Fall Damage": // Dive
                case "Hit U": // Wraiths
                    break;
                default:
                    return go;

            }
            PlayMakerFSM fsm = FSMUtility.LocateFSM(go, "damages_enemy");
            if (fsm != null)
            {
                Log($"Setting Damage for {go.name} - {fsm.name}");
                FsmInt value = fsm.FsmVariables.GetFsmInt("damageDealt");
                if (value != null)
                {
                    Log($"Spell name: {go.name} - {fsm.name}. Damage: {value.Value}");
                    value.Value = ls.SpellDamage(value.Value, Settings.IntelligenceStat);
                    Log($"Set Damage for {go.name} - {fsm.name} to {value.Value}");
                }
            }
            return go;
        }


        public void ShowCursor()
        {
            if (hc != null && hc.cState != null && GameManager.instance != null)
            {
                Cursor.visible = hc.cState.nearBench || GameManager.instance.isPaused;
                return;
            }

            if (GameManager.instance != null)
                Cursor.visible = GameManager.instance.isPaused;

        }

        public void CritHit(Collider2D otherCollider, GameObject go)
        {
            if (Crit && otherCollider.gameObject.layer == 11)
            {
                hc.shadowRingPrefab.transform.SetScaleX(0.5f);
                hc.shadowRingPrefab.transform.SetScaleY(0.5f);
                UnityEngine.Object.Instantiate(hc.shadowRingPrefab, otherCollider.gameObject.transform.position, go.transform.rotation);
            }
        }

        public float FocusCost() => (float)ls.FocusCost(Settings.IntelligenceStat) / 33f;

        public int BlueHealth() => ls.ExtraMasks(Settings.ResilienceStat);

        public void MpRegen()
        {
            if (hc == null)
                return;

            this.manaRegenTime += Time.deltaTime;
            if (this.manaRegenTime >= 1.11f)
            {
                this.manaRegenTime -= 1.11f;
                hc.AddMPChargeSpa(ls.SoulRegen(Settings.WisdomStat));
            }
        }

        public int SoulGain(int num) => ls.ExtraSoul(Settings.WisdomStat, num);

        public void CalculateCrit()
        {
            pd.nailDamage = this.OldNailDamage;
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
            int num = new System.Random().Next(1, 100);
            this.Crit = (num <= ls.CritChance(Settings.LuckStat));
            if (this.Crit)
            {
                pd.nailDamage = ls.CritDamage(Settings.DexterityStat, pd.nailDamage);
                PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
                spriteFlash.FlashGrimmflame();
                hc.carefreeShield.SetActive(true);
            }
        }

        //public void onSceneLoad(Scene dst, LoadSceneMode lsm)
        public void onSceneLoad(string sceneName)
        {
            if (GameManager.instance.IsGameplayScene() && UIManager.instance.uiState.ToString() == "PLAYING")
            {
                if (hc == null && HeroController.instance != null)
                {
                    hc = HeroController.instance;
                    if (spriteFlash == null)
                    {
                        spriteFlash = hc.GetComponent<SpriteFlash>();
                        Instance.Log("Hero object set. SpriteFlash component gotten.");
                    }
                }
            }
            if (pd == null && PlayerData.instance != null)
                pd = PlayerData.instance;
            int num = 0;
            if (pd.lurienDefeated)
            {
                num++;
            }
            if (pd.hegemolDefeated)
            {
                num++;
            }
            if (pd.monomonDefeated)
            {
                num++;
            }

            GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
            for (int i = 0; i < array.Length; i++)
            {
                GameObject go = array[i];
                PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(go, "health_manager_enemy");
                if (playMakerFSM == null)
                {
                    playMakerFSM = FSMUtility.LocateFSM(go, "health_manager");
                }
                if (playMakerFSM != null)
                {
                    playMakerFSM.FsmVariables.GetFsmInt("Geo Small").Value = ls.IncreaseGeo(playMakerFSM.FsmVariables.GetFsmInt("Geo Small").Value, Settings.LuckStat);
                    playMakerFSM.FsmVariables.GetFsmInt("Geo Small Extra").Value = ls.IncreaseGeo(playMakerFSM.FsmVariables.GetFsmInt("Geo Small Extra").Value, Settings.LuckStat);
                    playMakerFSM.FsmVariables.GetFsmInt("Geo Medium").Value = ls.IncreaseGeo(playMakerFSM.FsmVariables.GetFsmInt("Geo Medium").Value, Settings.LuckStat);
                    playMakerFSM.FsmVariables.GetFsmInt("Geo Med Extra").Value = ls.IncreaseGeo(playMakerFSM.FsmVariables.GetFsmInt("Geo Med Extra").Value, Settings.LuckStat);
                    playMakerFSM.FsmVariables.GetFsmInt("Geo Large").Value = ls.IncreaseGeo(playMakerFSM.FsmVariables.GetFsmInt("Geo Large").Value, Settings.LuckStat);
                    playMakerFSM.FsmVariables.GetFsmInt("Geo Large Extra").Value = ls.IncreaseGeo(playMakerFSM.FsmVariables.GetFsmInt("Geo Large Extra").Value, Settings.LuckStat);
                    //Uncomment this section to print dropped geo amount for all enemies on scene.
                    //BonfireMod.Instance.LogDebug(string.Concat(new object[]
                    //{
                    //"Total geo for " + array[i].name + " : ",
                    //playMakerFSM.FsmVariables.GetFsmInt("Geo Small").Value,
                    //"-",
                    //playMakerFSM.FsmVariables.GetFsmInt("Geo Small Extra").Value,
                    //"-",
                    //playMakerFSM.FsmVariables.GetFsmInt("Geo Medium").Value,
                    //"-",
                    //playMakerFSM.FsmVariables.GetFsmInt("Geo Med Extra").Value,
                    //"-",
                    //playMakerFSM.FsmVariables.GetFsmInt("Geo Large").Value,
                    //"-",
                    //playMakerFSM.FsmVariables.GetFsmInt("Geo Large Extra").Value
                    //}));
                    if (FSMUtility.GetInt(playMakerFSM, "HP") <= 5)
                    {
                        FSMUtility.SetInt(playMakerFSM, "HP", 1);
                    }
                    else
                    {
                        FSMUtility.SetInt(playMakerFSM, "HP", (int)((double)FSMUtility.GetInt(playMakerFSM, "HP") * (1.25 + (double)(num / 3)) * (2.5 / (1.0 + Math.Exp(-0.05 * (double)Settings.CurrentLv)))));
                    }

                }
                PlayMakerFSM fsm = FSMUtility.LocateFSM(go, "damages_enemy");
            }
            Log($"Enemy HP multiplier: {(1.25 + num / 3) * (2.5 / (1.0 + Math.Exp(-0.05 * Settings.CurrentLv)))}");
            //Settings.RL3Levels = pd.trinket3;
            //Settings.RL4Levels = pd.trinket4;
        }

        public void SetupGameRefs()
        {
            if (gm == null)
            {
                gm = GameManager.instance;
                gm.gameObject.AddComponent<LevellingSystem>();
            }
            if (ls == null)
                ls = LevellingSystem.Instance;
            if (pd == null && PlayerData.instance != null)
                pd = PlayerData.instance;
            //BossRush = IsBossRush();
            //Settings.FillBossRewards();
        }

        public void SetupGameRefs(int id)
        {
            if (gm == null)
            {
                gm = GameManager.instance;
                gm.gameObject.AddComponent<LevellingSystem>();
            }
            if (ls == null)
                ls = LevellingSystem.Instance;
            if (pd == null && PlayerData.instance != null)
                pd = PlayerData.instance;
            //BossRush = IsBossRush();
            //Settings.FillBossRewards();
        }

        public void BenchApply(PlayerData pd, HeroController hc)
        {
            pd.nailDamage = LevellingSystem.Instance.NailDamage(Settings.StrengthStat);
            OldNailDamage = pd.nailDamage;
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
            //Settings.RL3Levels = pd.trinket3;
            //Settings.RL4Levels = pd.trinket4;
            HeroController.instance.ATTACK_DURATION = 0.35f / LevellingSystem.Instance.AttackSpeed(Settings.DexterityStat);
            HeroController.instance.ATTACK_DURATION_CH = 0.25f / LevellingSystem.Instance.AttackSpeed(Settings.DexterityStat);
            HeroController.instance.ATTACK_COOLDOWN_TIME = 0.41f / LevellingSystem.Instance.AttackSpeed(Settings.DexterityStat);
            HeroController.instance.ATTACK_COOLDOWN_TIME_CH = 0.25f / LevellingSystem.Instance.AttackSpeed(Settings.DexterityStat);
            LogDebug("Attack Duration: " + HeroController.instance.ATTACK_DURATION +
                ". Attack Duration CH: " + HeroController.instance.ATTACK_DURATION_CH +
                ". Attack Cooldown: " + HeroController.instance.ATTACK_COOLDOWN_TIME +
                ". Attack Cooldown CH: " + HeroController.instance.ATTACK_COOLDOWN_TIME_CH);
        }

        public void SetupNewModData()
        {
            Settings.CurrentLv = 1;
            Settings.StrengthIncrease = 0;
            Settings.DexterityIncrease = 0;
            Settings.IntelligenceIncrease = 0;
            Settings.ResilienceIncrease = 0;
            Settings.WisdomIncrease = 0;
            Settings.LuckIncrease = 0;
            Settings.SpentGeo = 0;
            Settings.TotalSpentGeo = 0;
            Settings.Respec = 1;
            //Settings.FreeLevels = 0;
            //Settings.SpentFreeLevels = 0;
            //Settings.TotalFreeLevels = 0;
            Settings.GeoLevels = 0;
            Settings.TotalGeoLevels = 1;
            Settings.SpentGeoLevels = 0;
            //Settings.RL3Levels = 0;
            //Settings.RL4Levels = 0;
            //Settings.RelicLevels = 0;
            //Settings.FillBossRewards();
            Settings.StrengthStat = 1;
            Settings.DexterityStat = 1;
            Settings.IntelligenceStat = 1;
            Settings.ResilienceStat = 1;
            Settings.WisdomStat = 1;
            Settings.LuckStat = 1;
            Log("Set up new player data.");
        }


        public void Reset(int save)
        {
            SetupNewModData();
            Log("Reset player data.");
        }

        public bool IsBossRush()
        {
            int num = 0;
            foreach (string mod in ModHooks.Instance.LoadedMods)
                if (mod == "BossRush")
                    num++;
            if (num == 1)
                Instance.Log("Boss Rush loaded, allowing level up on pause menu.");
            else
                Instance.Log("Boss Rush not loaded.");
            return num == 1;
        }

        public void LookForBoss(string target, bool val)
        {
            if (!BossRush)
                return;
            foreach (KeyValuePair<string, string> boss in Settings.BossRewards)
                if (target == nameof(boss))
                    PlayerData.instance.SetIntInternal(boss.Value, PlayerData.instance.GetIntInternal(boss.Value) + 1);
            PlayMakerFSM.BroadcastEvent("TRINK 4");

        }

        //public static BonfireSettings Settings;
        public override string GetVersion() => "1.1.3.1";
        public int OldNailDamage { get; set; } = 5;
        public int HitsSinceShielded { get; set; } = 0;
        public bool Crit { get; set; } = false;
        public bool BossRush { get; set; } = false;
        public float manaRegenTime;
        public static GameManager gm;
        public LevellingSystem ls;
        public static PlayerData pd;
        public static HeroController hc;
        public static SpriteFlash spriteFlash;
        public float VanillaSlashDuration;
        public float VanillaSlashDurationCH;
        public float VanillaSlashCooldown;
        public float VanillaSlashCooldownCH;
        public int FireballCloneDamage { get; set; } = 15;
        public int FireballDamage { get; set; } = 15;
        public int HitLDamage { get; set; } = 35;
        public int HitRDamage { get; set; } = 30;
        public int QFallDamage { get; set; } = 15;

    }
}
