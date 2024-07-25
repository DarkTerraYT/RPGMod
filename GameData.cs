using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Newtonsoft.Json;
using RPGMod.Items;
using RPGMod.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGMod
{
    public class NumberStat(string name, double defaultValue, double value)
    {
        public string Name => name;
        [JsonIgnore]
        public double DefaultValue = defaultValue;

        public double Value = value;
    }

    public class BoolStat(string name, bool defaultValue, bool value)
    {
        public string Name => name;
        [JsonIgnore]
        public bool DefaultValue = defaultValue;

        public bool Value = value;
    }

    public class RpgUserData
    {

        private static RpgUserData Dummy = new()
        {
            PlayerName = "Player"
        };

        public static RpgUserData Player = Dummy;

        public List<ItemData> UniversalItems = [];
        public Dictionary<string, double> UnitedXP = new();

        [JsonIgnore]
        public List<RpgGameData> Saves { get => JsonHelper.GetSavedRpgGameData(); }

        public string PlayerName = "";

        [JsonIgnore]
        public Color NameColour = Color.white;

        float R = 1;
        float G = 1;
        float B = 1;

        public void UpdateColor(Color color)
        {
            NameColour = color;
            R = color.r;
            G = color.g;
            B = color.b;
        }

        public double UniversalXP { get => universalXP; }
        private double universalXP = 0;

        public bool UnlockedMasteryForever = false;

        public bool HasUniversalXP = false;
        public bool HasUnitedXP = false;

        /// <summary>
        /// Rate that the player gains universal xp
        /// </summary>
        public double UniversalXPMutliper = 0.01;
        /// <summary>
        /// Rate that the player gains united xp
        /// </summary>
        public double UnitedXPMutliper = 0.05;

        public void AddUnitedXP(TowerXPData data)
        {
            AddUnitedXP(data.BaseId, data.TotalXP);
        }

        public void AddUniversalXP(double amount)
        {
            universalXP += amount;
            currData.UniversalXPGainedThisGame += amount;
        }

        internal void SetUnitedXP(Dictionary<string, double> unitedXP)
        {
            UnitedXP = unitedXP;
        }

        public void AddUnitedXP(string baseId, double amount)
        {
            if (!UnitedXP.ContainsKey(baseId))
            {
                UnitedXP.Add(baseId, amount * UnitedXPMutliper);
                if (currData.UnitedXPGainedThisGame.ContainsKey(baseId))
                {
                    currData.UnitedXPGainedThisGame[baseId] += amount * UnitedXPMutliper;
                }
                else
                {
                    currData.UnitedXPGainedThisGame.Add(baseId, amount * UnitedXPMutliper);
                }
            }
            else
            {
                UnitedXP[baseId] += amount * UnitedXPMutliper;
                if (currData.UnitedXPGainedThisGame.ContainsKey(baseId))
                {
                    currData.UnitedXPGainedThisGame[baseId] += amount * UnitedXPMutliper;
                }
                else
                {
                    currData.UnitedXPGainedThisGame.Add(baseId, amount * UnitedXPMutliper);
                }
            }
        }

        internal void SetUniversalXP(double amount)
        {
            universalXP = amount;
        }

        public double GetUnitedXP(string baseId)
        {
            if (!UnitedXP.ContainsKey(baseId))
            {
                UnitedXP.Add(baseId, 0);
                return UnitedXP[baseId];
            }

            return UnitedXP[baseId];
        }

        public List<ModItem> LoadItemsFromSave()
        {
            var list = new List<ModItem>();

            foreach (var itemData in UniversalItems)
            {
                var item = ModItem.GetItemWithName(itemData.ID);

                if (item != null)
                {
                    //ModHelper.Log<RPGMod>($"Player had {item.Amount} of {item.ItemName}");
                    item.SetAmount(itemData.Amount);
                    //ModHelper.Log<RPGMod>($"Player now has {item.Amount} of {item.ItemName}");
                    list.Add(item);
                }
            }

            return list;
        }

        public List<ModItem> LoadItemsFromSave(out Dictionary<ItemData, ModItem> items)
        {
            items = new();

            var list = new List<ModItem>();

            foreach (var itemData in UniversalItems)
            {
                var item = ModItem.GetItemWithName(itemData.ID);

                if (item != null)
                {
                    //ModHelper.Log<RPGMod>($"Player had {item.Amount} of {item.ItemName}");
                    item.SetAmount(itemData.Amount);
                    //ModHelper.Log<RPGMod>($"Player now has {item.Amount} of {item.ItemName}");
                    list.Add(item);
                    items.Add(itemData, item);
                }
            }

            return list;
        }


        public void UpdateItemData(ModItem item)
        {
            if(item.Universal)
            {
                ItemData itemData = UniversalItems.Find(data => data.ID == item.ItemName);
                itemData.Amount = item.Amount;
            }
            else
            {
                ModHelper.Log<RPGMod>("Tried to add a normal item to player's universal items!");
            }
        }
    }
    public class RpgGameData(string mapName, string modeName, string difficuly, List<TowerXPData> xpData, int round = 0, bool ignoreSave = false)
    {
        public const string EXPMULTI = "ExpMulti";
        public const string CASHMULTI = "CashMulti";

        public string MapName = mapName;
        public string ModeName = modeName;
        public string Difficuly = difficuly;
        public int Round = round;
        public List<TowerXPData> XPData = xpData;
        public List<ItemData> Items = [];
        public Dictionary<string, int> Stock = [];

        public List<NumberStat> NumberStats = [new("Stars", 0, 0), new("CashMulti", 1, 1), new("ExpMulti", 1, 1)];
        public List<BoolStat> BoolStats = [new("Mastery", false, false)];

        public double UniversalXPGainedThisGame = 0;
        public Dictionary<string, double> UnitedXPGainedThisGame = [];

        public void Update()
        {
            LeveledTowers = BaseTowerModels.Duplicate().FindAll(tower => !tower.isSubTower);

            foreach (var data in XPData)
            {
                foreach (var tower in LeveledTowers)
                {
                    if (tower.baseId == data.BaseId)
                    {
                        for (int i = 1; i < data.Level; i++)
                        {
                            TowerXPData.ApplyLevel(tower, i);
                        }
                    }
                }
            }

            ModifiedTowerModels = LeveledTowers.Duplicate();

            foreach (var item in ModContent.GetContent<ModItem>().FindAll(item_ => item_.ApplyMethod != ModItem.ItemApplyMethod.Never))
            {
                item.ApplyToTowers(ModifiedTowerModels);
            }

            Game.instance.model.towers = ModifiedTowerModels.ToIl2CppReferenceArray();

            foreach(var tts in InGame.instance.bridge.GetAllTowers().ToList())
            {
                tts.tower.UpdateRootModel(ModifiedTowerModels.Find(tm => tm.name == tts.tower.towerModel.name));
            }
        }

        public static RpgGameData Create(string map, string gameMode, string diff, List<TowerXPData> xpData)
        {
            try
            {
                RpgGameData data = new(map, gameMode, diff, xpData);

                data.GetBoolStat("Mastery").Value = Player.UnlockedMasteryForever;

                currData.Update();
                return data;
            }
            catch (Exception e)
            {
                ModHelper.Error<RPGMod>("Failed to create RpgGameData.");
                ModHelper.Error<RPGMod>(e);
                return null;
            }
        }

        public static RpgGameData Create(string map, string gameMode, string diff)
        {
            return Create(map, gameMode, diff, TowerXPData.CreateTowerXPData(Game.instance.model.towers.ToList().FindAll(tower => !tower.isSubTower)));
        }

        public static RpgGameData Create(InGame inGame)
        {
            return Create(inGame, TowerXPData.CreateTowerXPData(Game.instance.model.towers.ToList().FindAll(tower => !tower.isSubTower)));
        }

        public static RpgGameData Create(InGame inGame, List<TowerXPData> xpData)
        {
            return Create(inGame.GetGameModel().map.mapName, inGame.GetGameModel().gameMode, inGame.GetGameModel().difficultyId, xpData);
        }

        public NumberStat GetNumberStat(string name)
        {
            return NumberStats.Find(x => x.Name == name);
        }

        public void ResetNumberStat(string name)
        {
            var numberStat = GetNumberStat(name);

            ResetNumberStat(numberStat);
        }

        public void ResetStats()
        {
            ResetNumberStats();
            ResetBoolStats();
        }

        public void ResetNumberStats()
        {
            foreach (var item in NumberStats)
            {
                item.Value = item.DefaultValue;
            }
        }

        public void ResetBoolStats()
        {
            foreach (var item in BoolStats)
            {
                item.Value = item.DefaultValue;
            }
        }

        public void ResetNumberStat(NumberStat stat)
        {
            stat.Value = stat.DefaultValue;
        }

        public BoolStat GetBoolStat(string name)
        {
            return BoolStats.Find(x => x.Name == name);
        }

        public void ResetBoolStat(BoolStat stat)
        {
            stat.Value = stat.DefaultValue;
        }

        public void ResetBoolStat(string name)
        {
            var boolStat = GetBoolStat(name);

            ResetBoolStat(boolStat);
        }

        [JsonIgnore]
        public readonly bool IgnoreSave = ignoreSave;

        //public bool QuestsUnlocked = false;

        public int GetItemCountFromSave(string itemName)
        {
            foreach (var item in Items)
            {
                if (item.ID == itemName)
                {
                    return item.Amount;
                }
            }

            var item_ = ModContent.GetContent<ModItem>().Find(item => item.ItemName == itemName);

            if (item_ == null)
            {
                return 0;
            }
            else
            {
                return item_.StartAmount;
            }
        }

        public static int GetItemCountFromSave(RpgGameData saveData, string itemName)
        {
            return saveData.GetItemCountFromSave(itemName);
        }

        public List<ModItem> LoadItemsFromSave()
        {
            var list = new List<ModItem>();

            foreach (var itemData in Items)
            {
                var item = ModItem.GetItemWithName(itemData.ID);

                if (item != null)
                {
                    //ModHelper.Log<RPGMod>($"Player had {item.Amount} of {item.ItemName}");
                    item.SetAmount(itemData.Amount);
                    //ModHelper.Log<RPGMod>($"Player now has {item.Amount} of {item.ItemName}");
                    list.Add(item);
                }
            }

            return list;
        }

        public List<ModItem> LoadItemsFromSave(out Dictionary<ItemData, ModItem> items)
        {
            items = new();

            var list = new List<ModItem>();

            foreach (var itemData in Items)
            {
                var item = ModItem.GetItemWithName(itemData.ID);

                if (item != null)
                {
                    //ModHelper.Log<RPGMod>($"Player had {item.Amount} of {item.ItemName}");
                    item.SetAmount(itemData.Amount);
                    //ModHelper.Log<RPGMod>($"Player now has {item.Amount} of {item.ItemName}");
                    list.Add(item);
                    items.Add(itemData, item);
                }
            }

            return list;
        }

        public static RpgGameData DummyData = new("", "", "", [], 0, false);

        public void ModifyItemData(string id, int amount)
        {
            var savedItem = Items.Find(iD => iD.ID == id);

            if (savedItem != null)
            {
                savedItem.Amount = amount;
            }
            else
            {
                Items.Add(new(id, amount));
            }
        }
        public void ModifyItemData(ModItem item)
        {
            ModifyItemData(item.ItemName, item.Amount);
        }
    }

    public class ItemData(string id, int amount)
    {
        public string ID = id;
        public int Amount = amount;
    }


    public class TowerXPData(string baseId, double startXP = 0)
    {
        public string BaseId = baseId;
        public double XP = startXP; // Double to allow numbers over a centillion (10^300)
        public double TotalXP = startXP;
        public int Level = 0;
        public double XPToLevelUp = 75; // Same reasoning as XP
        public bool addedLvl50Badge = false;
        public bool addedMaxLvlBadge = false;

        public string Name => BaseId.Spaced();

        public static int CalculateLevel(double xp)
        {
            double xpToLevelUp = 75;
            int level = 0;
            while (xp >= xpToLevelUp)
            {
                if (level >= MaxLevel)
                {
                    level = MaxLevel;
                    break;
                }
                level++;
                xp -= xpToLevelUp;
                xpToLevelUp *= XPCostMultiplier;
                xpToLevelUp = Math.Truncate(xpToLevelUp);
            }

            return level;
        }

        public static List<TowerXPData> CreateTowerXPData(List<TowerModel> towers, bool ignoreHeroCheck = false, bool ignoreBaseTowerCheck = false)
        {
            List<TowerXPData> list = [];

            if (!ignoreBaseTowerCheck)
            {
                towers = towers.FindAll(tower => tower.baseId == tower.name);
            }
            foreach (var tower in towers)
            {
                if ((tower.towerSet == Il2CppAssets.Scripts.Models.TowerSets.TowerSet.Hero && InGame.instance.SelectedHero == tower.baseId) || ignoreHeroCheck
                    || tower.towerSet != Il2CppAssets.Scripts.Models.TowerSets.TowerSet.Hero)
                {
                    if (!ignoreBaseTowerCheck)
                    {
                        if (!Player.UnitedXP.ContainsKey(tower.baseId))
                        {
                            Player.UnitedXP.Add(tower.baseId, 0);
                        }
                    }

                    TowerIds.Add(tower.baseId);
                    var xpData = new TowerXPData(tower.baseId, Player.UnitedXP[tower.baseId] + Player.UniversalXP);
                    if (!ignoreBaseTowerCheck)
                    {
                        xpData.LevelUp(Game.instance.model.towers.ToList().FindAll(tower_ => tower_.baseId == tower.baseId));
                    }
                    list.Add(new(tower.baseId));
                }
            }
            return list;
        }

        public static void ApplyLevel(TowerModel towerModel, int level)
        {
            var attackModels = towerModel.GetAttackModels();
            var weaponModels = towerModel.GetWeapons();

            towerModel.range += 1.75f;
            foreach (var attackModel in attackModels)
            {
                attackModel.range += 1.75f;
            }
            foreach (var weaponModel in weaponModels)
            {
                if (level.NoRemainder(5))
                    weaponModel.rate -= 0.05f;
                foreach (var projectile in weaponModel.GetDescendants<ProjectileModel>().ToList())
                {
                    if (level.NoRemainder(3))
                        projectile.pierce += 1f;
                    foreach (var damageModel in projectile.GetDescendants<DamageModel>().ToList())
                    {
                        if (level >= 25)
                        {
                            damageModel.immuneBloonProperties = Il2Cpp.BloonProperties.None;
                        }

                        if (level.NoRemainder(7))
                        {
                            damageModel.damage += 1f;

                            foreach (var damageModifier in damageModel.GetDescendants<DamageModifierForTagModel>().ToList())
                            {
                                damageModifier.damageAddative += 1f;
                                if (level.NoRemainder(10))
                                    damageModifier.damageMultiplier += 0.1f;
                            }
                            foreach (var damageModifier in damageModel.GetDescendants<DamageModifierForBloonTypeModel>().ToList())
                            {
                                damageModifier.damageAdditive += 1f;
                                if (level.NoRemainder(10))
                                    damageModifier.damageMultiplier += 0.1f;
                            }
                            foreach (var damageModifier in damageModel.GetDescendants<DamageModifierForBloonStateModel>().ToList())
                            {
                                damageModifier.damageAdditive += 1f;
                                if (level.NoRemainder(10))
                                    damageModifier.damageMultiplier += 0.1f;
                            }
                            foreach (var damageModifier in damageModel.GetDescendants<DamageModifierForModifiersModel>().ToList())
                            {
                                damageModifier.damageAddative += 0.2f;
                                if (level.NoRemainder(10))
                                    damageModifier.damageMultiplier += 0.1f;
                            }
                        }
                    }
                }
            }

            if (level.NoRemainder(4))
            {
                if (towerModel.HasDescendant<CashPerBananaFarmInRangeModel>())
                {
                    towerModel.GetDescendants<CashPerBananaFarmInRangeModel>().ForEach(mod => { mod.baseCash *= 1.05f; mod.cash *= 1.05f; });
                }
                if (towerModel.HasDescendant<CashModel>())
                {
                    towerModel.GetDescendants<CashModel>().ForEach(mod =>
                    {
                        if (mod.salvage < 1)
                        {
                            if (mod.salvage * 1.05f > 1)
                            {
                                mod.salvage = 1;
                            }
                            else
                            {
                                mod.salvage *= 1.05f;
                            }
                        }

                        mod.minimum *= 1.05f;
                        mod.maximum *= 1.05f;
                    });
                }
                if (towerModel.HasDescendant<ImfLoanModel>())
                {
                    towerModel.GetDescendants<ImfLoanModel>().ForEach(mod =>
                    {
                        float newAmount = mod.amount * 1.05f;

                        if (mod.imfLoanCollection != null)
                        { mod.imfLoanCollection.amount -= newAmount - mod.amount; }
                        mod.amount = newAmount;
                    });
                }
                if (towerModel.HasBehavior<PerRoundCashBonusTowerModel>())
                {
                    towerModel.GetBehaviors<PerRoundCashBonusTowerModel>().ForEach(mod => mod.cashPerRound *= 1.05f);
                }
            }

            if (level >= 12)
            {
                towerModel.GetDescendants<FilterInvisibleModel>().ForEach(mod => mod.isActive = false);
            }

            if (level.NoRemainder(3))
            {
                foreach (var ability in towerModel.GetAbilities())
                {
                    if (ability.cooldownSpeedScale - 0.05f > 0)
                    {
                        ability.cooldownSpeedScale -= 0.05f;
                    }
                }
            }
        }


        public int LevelUp(List<Tower> towers)
        {
            int oldLevel = Level;

            while (XP >= XPToLevelUp)
            {
                if (Level >= MaxLevel)
                {
                    Level = MaxLevel;
                    break;
                }
                Level++;
                XP -= XPToLevelUp;
                XPToLevelUp *= XPCostMultiplier;
                XPToLevelUp = Math.Truncate(XPToLevelUp);
            }

            if (oldLevel < Level)
            {
                currData.Update();
            }

                if (MasteryPanel != null)
            { MasteryUI.Update(this, MasteryPanel); }
            
            return Level;
        }

        public int LevelUp(List<TowerModel> towers)
        {
            while (XP >= XPToLevelUp)
            {
                if (Level >= MaxLevel)
                {
                    Level = MaxLevel;
                    break;
                }
                Level++;
                XP -= XPToLevelUp;
                XPToLevelUp *= XPCostMultiplier;
                XPToLevelUp = Math.Truncate(XPToLevelUp);

                foreach (var tower in towers)
                {
                    ApplyLevel(tower, Level);
                }
            }

            //currData.Update();

            if (MasteryPanel != null)
            { MasteryUI.Update(this, MasteryPanel); }
            
            return Level;
        }
    }
}
