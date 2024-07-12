using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Map;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine.UIElements;

namespace RPGMod.Items
{
    public abstract class ModItem : ModContent
    {
        public ModItem() { }

        internal static ModItem? GetItemWithName(string itemName)
        {
            return GetContent<ModItem>().Find(item => item.ItemName == itemName);
        }

        public void ApplyToTowers(List<TowerModel> towerModels)
        {
            for(int i = 0; i < Amount; i++)
            {
                ModifyTowerModels(towerModels);
            }
        }

        public void SetAmount(int value)
        {
            if(value > Max & Max != -1)
            {
                amount_ = Max;
            }
            else
            {
                amount_ = value;
            }
        }

        public void ChangeAmount(int value)
        {
            if(amount_ + value < 0)
            {
                amount_ = 0;
            }
            else
            {
                amount_ += value;
            }
        }

        public override void Register() 
        {
            amount_ = StartAmount;
            currData.ModifyItemData(this);
            ModHelper.Log<RPGMod>(Icon);
        }

        public void Reset()
        {
            amount_ = StartAmount;
            currData.ModifyItemData(this);
        }

        public void LoadAmountFromSave(RpgGameData gameData)
        {
            SetAmount(gameData.GetItemCountFromSave(ItemName));
            gameData.ModifyItemData(this);
        }

        public void LoadAmountFromSave()
        {
            LoadAmountFromSave(currData);
        }

        public virtual string ItemName => Name.Spaced();

        public virtual bool Hidden => false;

        public virtual string Icon => GetTextureGUID<RPGMod>(Name);

        public virtual bool VanillaSprite => false;

        public abstract ItemType Type { get; }

        public abstract ItemRarity Rarity { get; }

        public abstract string Description { get; }

        public virtual int StartAmount => 0;

        protected int amount_ = 0;

        public int Amount { get => amount_; }

        public virtual int Max => -1;

        public virtual bool Universal => false;

        public virtual ItemApplyMethod ApplyMethod => ItemApplyMethod.Never;

        public enum ItemApplyMethod
        {
            Never,
            PreLevels,
            PostLevels
        }

        protected virtual void ModifyTowerModels(List<TowerModel> towerModels)
        {
            
        }

        public virtual void OnItemUse(Game game, InGame inGame, int amount)
        {
            
        }

        public enum ItemType
        {
            Consumable,
            Equipment,
            Relic,
            Unlock,
        }

        public enum ItemRarity
        {
            Basic,
            Green,
            Blue,
            Purple,
            Pink,
            Red
        }

        protected override int Order => (int)Rarity;
    }
}
