using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Reflection;
using Modding;
using UnityEngine.SceneManagement;


namespace BonfireMod
{
    public class BonfireMod : Mod, ITogglableMod, ILocalSettings<BonfireModSettings>
    {
        public static BonfireMod Instance;

        public BonfireModSettings Settings = new BonfireModSettings();
        public void OnLoadLocal(BonfireModSettings s) => Settings = s;
        public BonfireModSettings OnSaveLocal() => Settings;

        public override void Initialize()
        {
            Instance = this;
            Instance.LogDebug("Bonfire Mod initializing!");

            ModHooks.NewGameHook += SetupGameRefs;
            ModHooks.SavegameLoadHook += SetupGameRefs;
            ModHooks.CharmUpdateHook += BenchApply;
            ModHooks.SoulGainHook += SoulGain;
            ModHooks.HeroUpdateHook += MpRegen;
            On.PlayerData.UpdateBlueHealth += PlayerData_UpdateBlueHealth;
            ModHooks.FocusCostHook += FocusCost;
            ModHooks.SlashHitHook += CritHit;
            ModHooks.CursorHook += ShowCursor;
            ModHooks.HitInstanceHook += SetDamages;
            ModHooks.AfterTakeDamageHook += ResShield;
            ModHooks.HeroUpdateHook += Instance_HeroUpdateHook;
            ModHooks.OnEnableEnemyHook += Instance_OnEnableEnemyHook;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            Instance.LogDebug("Bonfire Mod v." + GetVersion() + " initialized!");
        }

        private void PlayerData_UpdateBlueHealth(On.PlayerData.orig_UpdateBlueHealth orig, PlayerData self)
        {
            orig(self);
            self.SetInt("healthBlue", self.GetInt("healthBlue") + ls.ExtraMasks(Settings.ResilienceStat));
        }

        private bool Instance_OnEnableEnemyHook(GameObject enemy, bool isAlreadyDead)
        {
            HealthManager hm = enemy.GetComponent<HealthManager>();
            if (hm != null && hm.hp < 5000 && !isAlreadyDead)
            {
                LogDebug($@"Vanilla HP for {enemy.name} = {hm.hp}");
                hm.hp *= (int)((1.25 + (double)Dreamers / 3) * (2.5 / (1.0 + Math.Exp(-0.05 * Settings.CurrentLv))));
                LogDebug($@"Bonfire HP for {enemy.name} = {hm.hp}");

                hm.SetGeoSmall(ls.IncreaseGeo(GetGeo("small", hm), Settings.LuckStat));
                hm.SetGeoMedium(ls.IncreaseGeo(GetGeo("medium", hm), Settings.LuckStat));
                hm.SetGeoLarge(ls.IncreaseGeo(GetGeo("large", hm), Settings.LuckStat));
            }
            return isAlreadyDead;
        }

        private void Instance_HeroUpdateHook()
        {
            if (GameManager.instance.inputHandler.inputActions.attack.WasPressed)
            {
                critRoll = UnityEngine.Random.Range(1, 100);
            }
        }

        public void Unload()
        {
            ModHooks.NewGameHook -= SetupGameRefs;
            ModHooks.SavegameLoadHook -= SetupGameRefs;
            ModHooks.CharmUpdateHook -= BenchApply;
            ModHooks.SoulGainHook -= SoulGain;
            ModHooks.HeroUpdateHook -= MpRegen;
            On.PlayerData.UpdateBlueHealth -= PlayerData_UpdateBlueHealth;
            ModHooks.FocusCostHook -= FocusCost;
            ModHooks.SlashHitHook -= CritHit;
            ModHooks.CursorHook -= ShowCursor;
            ModHooks.HitInstanceHook -= SetDamages;
            ModHooks.AfterTakeDamageHook -= ResShield;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

            Instance.LogDebug("Bonfire Mod disabled!");
        }

        private HitInstance SetDamages(Fsm owner, HitInstance hit)
        {
            bool isSpell;

            switch (hit.Source.name)
            {
                case "Fireball(Clone)":
                case "Fireball":
                case "Hit L": // Dive
                case "Hit R": // Dive
                case "Q Fall Damage": // Dive
                case "Hit U": // Wraiths
                    isSpell = true;
                    break;
                default:
                    isSpell = false;
                    break;
            }

            if (isSpell)
            {
                LogDebug($"[Vanilla] Spell name: {hit.Source.name} - {hit.Source}. Damage: {hit.DamageDealt}");
                hit.DamageDealt = ls.SpellDamage(hit.DamageDealt, Settings.IntelligenceStat);
                LogDebug($"[Bonfire] Spell name: {hit.Source.name} - {hit.Source}. Damage: {hit.DamageDealt}");
            }

            if (hit.Source.name.Contains("lash"))
            {
                LogDebug($@"[Vanilla] Damage for {hit.Source.name} = {hit.DamageDealt}");
                hit.DamageDealt = ls.NailDamage(Settings.StrengthStat);
                LogDebug($@"[Bonfire] Damage for {hit.Source.name} = {hit.DamageDealt}");
                LogDebug($@"Crit chance: {ls.CritChance(Settings.LuckStat)}. Rolled {critRoll}.");
                Crit = (critRoll <= ls.CritChance(Settings.LuckStat));
                if (Crit)
                {                    
                    hit.DamageDealt = ls.CritDamage(Settings.DexterityStat, hit.DamageDealt);
                    LogDebug($@"[Crit] Damage for {hit.Source.name} = {hit.DamageDealt}");
                    spriteFlash.FlashGrimmflame();
                    hc.carefreeShield.SetActive(true);
                }
            }

            return hit;
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (GameManager.instance.IsGameplayScene() && UIManager.instance.uiState.ToString() == "PLAYING")
            {
                if (hc == null && HeroController.instance != null)
                {
                    hc = HeroController.instance;
                    if (spriteFlash == null)
                    {
                        spriteFlash = hc.GetComponent<SpriteFlash>();
                        Instance.LogDebug("Hero object set. SpriteFlash component gotten.");
                    }
                }
            }

            if (pd == null && PlayerData.instance != null)
                pd = PlayerData.instance;

            Dreamers = 0;
            if (pd.lurienDefeated)
            {
                Dreamers++;
            }
            if (pd.hegemolDefeated)
            {
                Dreamers++;
            }
            if (pd.monomonDefeated)
            {
                Dreamers++;
            }
        }

        int GetGeo(string size, HealthManager enemy)
        {
            FieldInfo fi = enemy.GetType().GetField(size + "GeoDrops", BindingFlags.NonPublic | BindingFlags.Instance);
            object geo = fi.GetValue(enemy);
            int ret = geo == null ? 0 : (int)geo;
            return ret;
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
                switch (HitsSinceShielded)
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
                    HitsSinceShielded = 0;
                    hc.carefreeShield.SetActive(true);
                    damage = 0;
                }
                else
                {
                    HitsSinceShielded++;
                }
            }
            return damage;
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

        public int BlueHPRestored() => ls.ExtraMasks(Settings.ResilienceStat);
        
        public void MpRegen()
        {
            if (HeroController.instance != null && PlayerData.instance != null)
            {
                try
                {
                    if (manaRegenTime > 0)
                    {
                        manaRegenTime -= Time.deltaTime;
                    }
                    else
                    {
                        LogDebug($@"Recovering MP!");
                        manaRegenTime = 1.11f;
                        HeroController.instance.AddMPChargeSpa(ls.SoulRegen(Settings.WisdomStat));
                    }
                }
                catch { }
            }
        }

        public int SoulGain(int num) => ls.ExtraSoul(Settings.WisdomStat, num);

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
            if (GameManager.instance.IsGameplayScene() && UIManager.instance.uiState.ToString() == "PLAYING")
            {
                if (hc == null && HeroController.instance != null)
                {
                    hc = HeroController.instance;
                    if (spriteFlash == null)
                    {
                        spriteFlash = hc.GetComponent<SpriteFlash>();
                        Instance.LogDebug("Hero object set. SpriteFlash component gotten.");
                    }

                    hc.gameObject.AddComponent<LevellingSystem>();
                }
            }
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
            if (GameManager.instance.IsGameplayScene() && UIManager.instance.uiState.ToString() == "PLAYING")
            {
                if (hc == null && HeroController.instance != null)
                {
                    hc = HeroController.instance;
                    if (spriteFlash == null)
                    {
                        spriteFlash = hc.GetComponent<SpriteFlash>();
                        LogDebug("Hero object set. SpriteFlash component gotten.");
                    }
                    hc.gameObject.AddComponent<LevellingSystem>();
                }
            }
        }

        public void BenchApply(PlayerData pd, HeroController hc)
        {
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
            Settings.GeoLevels = 0;
            Settings.TotalGeoLevels = 1;
            Settings.SpentGeoLevels = 0;
            Settings.StrengthStat = 1;
            Settings.DexterityStat = 1;
            Settings.IntelligenceStat = 1;
            Settings.ResilienceStat = 1;
            Settings.WisdomStat = 1;
            Settings.LuckStat = 1;
            LogDebug("Set up new player data.");
        }


        public void Reset(int save)
        {
            SetupNewModData();
            LogDebug("Reset player data.");
        }

        public bool IsBossRush()
        {
            int num = 0;
            foreach (string mod in ModHooks.LoadedModsWithVersions.Keys)
                if (mod == "BossRush")
                    num++;
            if (num == 1)
                Instance.LogDebug("Boss Rush loaded, allowing level up on pause menu.");
            else
                Instance.LogDebug("Boss Rush not loaded.");
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
        
        public override string GetVersion() => "2.0.0.0";
        public int HitsSinceShielded { get; set; } = 0;
        public int Dreamers;
        public bool Crit { get; set; } = false;
        public int critRoll;
        public bool BossRush { get; set; } = false;
        public float manaRegenTime;
        public static GameManager gm;
        public LevellingSystem ls;
        public static PlayerData pd;
        public static HeroController hc;
        public static SpriteFlash spriteFlash;
    }
}
