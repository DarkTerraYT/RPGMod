global using static RPGMod.RpgUserData;

using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Input;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppAssets.Scripts.Utils;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using MelonLoader.Utils;
using RPGMod;
using RPGMod.Items;
using RPGMod.Items.Universal;
using RPGMod.Quests;
using RPGMod.Quests.Steps;
using RPGMod.Ui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Il2CppAssets.Scripts.Simulation.Simulation;

[assembly: MelonInfo(typeof(RPGMod.RPGMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace RPGMod;

public class RPGMod : BloonsTD6Mod
{
    public static ModQuest? currentQuest;

    public static List<string> TowerIds = new(9999999);

    public static int MaxLevel { get => MaxLevel_; }

    internal static int MaxLevel_ = 65;

    public static string LastLoadedMap = "";

    public static string LastSearch = "";

    public static RectTransform UiRect = new();

    public static RpgGameData currData = RpgGameData.DummyData;

    public static ModHelperPanel? MasteryPanel;

    public static List<TowerModel> BaseTowerModels = [];
    public static List<TowerModel> LeveledTowers = [];
    public static List<TowerModel> ModifiedTowerModels = [];

    internal static Action Blank = () => { };

    public static bool SandboxFlag = false;

    public static string SavePath => Path.Combine(MelonEnvironment.ModsDirectory, "DarkTerra", "RPGMod");
    public static string TowerIconPath => Path.Combine(SavePath, "ModTowerIcons");

    internal static Action Button(Action onClick)
    {
        Action action = new(() => { MenuManager.instance.buttonClickSound.Play(); onClick.Invoke(); });
        return action;
    }

    public override void OnApplicationStart()
    {
        JsonHelper.LoadRpgUserData();
        JsonHelper.SaveRpgUserData();
    }

    public override void OnMainMenu()
    {
        Player.PlayerName = Game.instance.GetPlayerLiNKAccount().DisplayName;
    }

    [HarmonyPatch(typeof(InGame), nameof(InGame.StartMatch))]
    public static class InGame_StartMatch
    {

        [HarmonyPostfix]
        public static void Postfix(InGame __instance, ref bool wasSaveOverwritten)
        {
            SandboxFlag = false;

            bool saveFound = false;

            if (!wasSaveOverwritten)
            {
                foreach (var gameData in JsonHelper.GetSavedRpgGameData())
                {
                    if (__instance.GetGameModel().map.mapName == gameData.MapName)
                    {
                        currData = gameData;
                        saveFound = true;

                        foreach (var item in ModContent.GetContent<ModItem>())
                        {
                            item.LoadAmountFromSave();
                        }

                        if (!currData.GetBoolStat("Mastery").Value)
                        {
                            currData.GetBoolStat("Mastery").Value = Player.UnlockedMasteryForever;
                        }

                        foreach (var data in currData.XPData)
                        {
                            if (Player.UnitedXP.ContainsKey(data.BaseId) && currData.UnitedXPGainedThisGame.ContainsKey(data.BaseId))
                            {
                                data.XP += Player.UnitedXP[data.BaseId] - currData.UnitedXPGainedThisGame[data.BaseId] + Player.UniversalXP - currData.UniversalXPGainedThisGame;
                            }
                        }

                        foreach (var entry in ModContent.GetContent<ShopEntry>())
                        {
                            foreach ((string key, int stock) in currData.Stock)
                            {
                                if (entry.Name == key)
                                {
                                    entry.Stock = stock;
                                }
                            }
                        }

                        break;
                    }
                }
                if (!saveFound)
                {
                    foreach (var item in ModContent.GetContent<ModItem>())
                    {
                        item.Reset();
                    }

                    currData = RpgGameData.Create(__instance);
                    //ModHelper.Log<RPGMod>($"Created Data for map {__instance.GetGameModel().map.mapName} with the {__instance.GetGameModel().gameMode} game mode on {__instance.SelectedDifficulty}");
                }
            }
            else
            {
                foreach (var item in ModContent.GetContent<ModItem>())
                {
                    item.Reset();
                }

                currData = RpgGameData.Create(__instance);
                //ModHelper.Log<RPGMod>($"Reset Data for map {__instance.GetGameModel().map.mapName} with the {__instance.GetGameModel().gameMode} game mode on {__instance.SelectedDifficulty}");
            }
            if (!saveFound)
            {
                foreach (var item in ModContent.GetContent<ModItem>())
                {
                    item.Reset();
                }

                currData = RpgGameData.Create(__instance);
                //ModHelper.Log<RPGMod>($"Reset Data for map {__instance.GetGameModel().map.mapName} with the {__instance.GetGameModel().gameMode} game mode on {__instance.SelectedDifficulty}");
            }

            if (__instance.GetGameModel().gameMode == GameModeType.Sandbox)
            {
                currData.GetBoolStat("Mastery").Value = true;
                SandboxFlag = true;
            }
            else
            {
                SandboxFlag = false;
            }

            Save();

            QuestUI.CreateQuestButton();
            MasteryUI.CreateMasteryButton(currData.XPData);
            BagUI.CreateBagBtn();
            ShopUI.CreateShopBtn();

            //currData.Update();
        }
    }

    public static void Save()
    {
        currData.Round = InGame.instance.bridge.GetCurrentRound();
        List<ItemData> items = [];
        Dictionary<string, int> stock = [];
        foreach (var item in ModContent.GetContent<ModItem>().FindAll(item => !item.Universal))
        {
            items.Add(new(item.ItemName, item.Amount));
        }

        List<ItemData> uniItems = [];
        foreach (var item in ModContent.GetContent<ModItem>().FindAll(item => item.Universal))
        {
            uniItems.Add(new(item.ItemName, item.Amount));
        }

        foreach (var entry in ModContent.GetContent<ShopEntry>())
        {
            stock.Add(entry.Name, entry.Stock);
        }

        if (!SandboxFlag)
        {
            currData.Items = items;
            currData.Stock = stock;
            Player.UniversalItems = uniItems;
            JsonHelper.SaveRpgGameData(currData);
            JsonHelper.SaveRpgUserData();
            ModHelper.Log<RPGMod>("Saved!!");
        }
        else
        {
            ModHelper.Log<RPGMod>("Sandbox!!");
        }
    }

    public override void OnRoundEnd()
    {
        Save();

        string unlockedItems = "Unlocked the item(s): ";

        int itemsFound = 0;

        foreach (var entry in ModContent.GetContent<ShopEntry>())
        {
            if (entry.UnlockRound > 0)
            {
                if (entry.UnlockRound <= InGame.instance.bridge.GetCurrentRound() && !entry.ShowInShop)
                {
                    itemsFound++;
                    if (itemsFound == 1)
                    {
                        unlockedItems += entry.Item.ItemName;
                    }
                    else
                    {
                        unlockedItems += ", " + entry.Item.ItemName;
                    }

                    entry.ShowInShop = true;
                }
            }
        }

        if (itemsFound > 0)
        {
            PopupScreen.instance.SafelyQueue(screen => screen.ShowOkPopup(unlockedItems + "!"));
        }
    }

    public override void OnRoundStart()
    {
        Save();
    }

    public override void OnMatchEnd()
    {
        if (QuestUI.instance != null)
        {
            QuestUI.instance.Close();
        }
        if (MasteryUI.instance != null)
        {
            MasteryUI.instance.Close();
            MasteryPanel = null;
        }
        if (BagUI.instance != null)
        {
            BagUI.instance.Close();
        }
        if (ShopUI.instance != null)
        {
            ShopUI.instance.Close();
        }
    }

    public override void OnTitleScreen()
    {
        List<string> safeBaseTowerIds = Helpers.ValidBaseTowerNames().ToList();
        BaseTowerModels = Game.instance.model.towers.Duplicate().ToList().FindAll(tm => safeBaseTowerIds.Any(id => id == tm.baseId) && !tm.isSubTower);
        LeveledTowers = BaseTowerModels.Duplicate();
        ModifiedTowerModels = BaseTowerModels.Duplicate();

        Player.LoadItemsFromSave();
        var ss1 = ModContent.GetInstance<StarSeal1Shop>();
        if (ss1.Item.Amount > 0)
        {
            MaxLevel_ += 5;
        }
        var ss2 = ModContent.GetInstance<StarSeal2Shop>();
        if (ss2.Item.Amount > 0)
        {
            MaxLevel_ += 5;
        }
        var ss3 = ModContent.GetInstance<StarSeal3Shop>();
        if (ss3.Item.Amount > 0)
        {
            MaxLevel_ += 5;
        }
    }

    public override void OnRestart()
    {
        if (QuestUI.instance != null)
        {
            QuestUI.instance.Close();
        }
        if (MasteryUI.instance != null)
        {
            MasteryUI.instance.Close();
        }
        if (BagUI.instance != null)
        {
            BagUI.instance.Close();
        }
        if (ShopUI.instance != null)
        {
            ShopUI.instance.Close();
        }

        foreach (var item in currData.XPData)
        {
            item.XPToLevelUp = 60;
            item.XP = 0;
            item.Level = 0;
            item.addedLvl50Badge = false;
            item.addedMaxLvlBadge = false;
        }
        foreach (var item in ModContent.GetContent<ModItem>())
        {
            item.Reset();
        }
        foreach (var item in ModContent.GetContent<ShopEntry>())
        {
            item.Reload();
        }

        currData.ResetStats();

        ModHelper.Log<RPGMod>($"Reset Data for map {currData.MapName} with the {currData.ModeName} game mode on {currData.Difficuly}");
        JsonHelper.SaveRpgGameData(currData);

        QuestUI.CreateQuestButton();
        MasteryUI.CreateMasteryButton(currData.XPData);
        BagUI.CreateBagBtn();
        ShopUI.CreateShopBtn();
    }

    [HarmonyPatch(typeof(Simulation), nameof(Simulation.AddCash))]
    public static class Simulation_AddCash
    {
        [HarmonyPostfix]
        public static bool Prefix(ref double c)
        {
            c *= currData.GetNumberStat("CashMulti").Value;

            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Simulation __instance, double c, ref CashType from, ref CashSource source, Tower tower)
        {
            double amount = c;


            if (amount > 0)
            {
                if (source != Simulation.CashSource.CoopTransferedCash | source != Simulation.CashSource.TowerSold)
                {
                    if (from != Simulation.CashType.CoopCash)
                    {
                        if (amount > 0 && currentQuest != null)
                        {
                            foreach (GenerateCash step in currentQuest.ActiveSteps)
                            {
                                if (!step.isFinished)
                                {
                                    step.CashToGenerate -= amount;
                                    if (step.CashToGenerate <= 0)
                                    {
                                        step.Finish();
                                    }
                                }
                            }

                        }
                    }
                }

                if (tower != null)
                {
                    if (source != Simulation.CashSource.TowerSold)
                    {
                        var xpData = currData.XPData.Find(data => data.BaseId == tower.towerModel.baseId);

                        AddXP(tower, 4 * amount / 20);
                    }
                }
            }
        }
    }

    #region Tower Mastery

    public static readonly ModSettingCategory TowerMastery = new("Tower Mastery")
    {
        icon = VanillaSprites.UpgradeBtn,
    };

    public static readonly ModSettingBool AllowRegrow = new(false)
    {
        description = "Do towers gain mastery EXP from regrow bloons (turned off bc of regrow farms), if enabled, exp from regrows will be only 10%",
        category = TowerMastery,
    };

    public static readonly ModSettingDouble XPMutliplier = new(1)
    {
        description = "Mutliplier for all exp given to towers. High values may break the game (you don't want any NAN exp values, do you?)",
        displayName = "XP Multiplier",
        category = TowerMastery,
    };

    public static readonly ModSettingDouble XPCostMultiplier = new(1.5)
    {
        description = "The increase for the amount of xp needed per upgrade. High values may break the game (you don't want any NAN exp values, do you?)",
        displayName = "XP Cost Multiplier",
        min = 1.1f,
        category = TowerMastery,
    };

    public static readonly ModSettingBool AutoUpdate = new(true)
    {
        description = "Whether or not to auto update the mastery menu, shop menu, and bag menu (shop menu and bag menu will still update when you buy/use an item). This may reduce lag.",
        icon = VanillaSprites.RestartIcon
    };

    /// <summary>
    /// Add XP to the given tower
    /// </summary>
    /// <param name="tower">tower to add level to</param>
    /// <param name="amount">xp to add</param>
    /// <returns>new level, 0 if mastery isn't unlocked</returns>
    public static int AddXP(Tower tower, double amount)
    {
        if (currData.GetBoolStat("Mastery").Value)
        {
            TowerXPData xpData = currData.XPData[0];
            foreach (var item in currData.XPData)
            {
                if (item.BaseId == tower.towerModel.baseId)
                {
                    xpData = item;
                    break;
                }
            }

            double total = amount * XPMutliplier * currData.GetNumberStat(RpgGameData.EXPMULTI).Value;

            if (!SandboxFlag)
            {
                if (Player.HasUniversalXP)
                {
                    Player.AddUniversalXP(total * Player.UniversalXPMutliper);
                }
                if (Player.HasUnitedXP)
                {
                    if (Player.UnitedXP.ContainsKey(tower.towerModel.baseId))
                    {
                        Player.UnitedXP[tower.towerModel.baseId] += total * Player.UnitedXPMutliper;
                    }
                    else
                    {
                        Player.UnitedXP.Add(tower.towerModel.baseId, 0);
                        Player.UnitedXP[tower.towerModel.baseId] += total * Player.UnitedXPMutliper;
                    }
                }
            }

            xpData.XP += total;
            xpData.TotalXP += total;

            List<Tower> towers_ = InGame.instance.GetTowers();
            List<Tower> towers = [];

            foreach (var tower_ in towers_)
            {
                if (tower_.towerModel.baseId == tower.towerModel.baseId) { towers.Add(tower_); }
            }

            return xpData.LevelUp(towers);
        }
        else
        {
            return 0;
        }
    }

    [HarmonyPatch(typeof(Bloon), nameof(Bloon.Damage))]
    public static class Bloon_Damage
    {
        [HarmonyPrefix]
        public static void Prefix(Bloon __instance, double totalAmount, Tower tower, Projectile projectile)
        {
            if (tower != null && (!__instance.bloonModel.IsRegrowBloon() || AllowRegrow))
            {
                double multi = totalAmount;
                double xpToAdd = 1;
                if (totalAmount > __instance.health)
                {
                    multi = __instance.health;
                }
                if (__instance.bloonModel.isGrow)
                {
                    xpToAdd /= 10;
                }

                xpToAdd *= multi;

                AddXP(tower, xpToAdd);
            }
            else
            {
                //ModHelper.Warning<RPGMod>("Tower was null.");
            }
        }
    }
    #endregion
}
