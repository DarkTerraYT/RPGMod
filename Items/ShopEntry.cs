using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace RPGMod.Items
{
    /// <summary>
    /// <see cref="ShopEntry{T}"/> without generics.
    /// </summary>
    public abstract class ShopEntry : ModContent
    {
        [JsonIgnore]
        public abstract ModItem Item { get; }

        public virtual bool AutoGiveItem => false;

        public ShopEntry() { }

        public override void Register() 
        {
            Stock = Stock_;
            Cost = Price; 
            ShowInShop = ShowedByDefault;
        }

        public void Reload()
        {
            Cost = Price;

            Item.Reset();

            if(AutoGiveItem)
            {
                Item.ChangeAmount(1);
            }

            Stock = Stock_;

            ShowInShop = ShowedByDefault;
        }

        public int Stock = 0;

        public bool ShowInShop = true;

        protected virtual bool ShowedByDefault => UnlockRound > 0;

        protected virtual int Stock_ => -1;

        public abstract string Icon { get; }

        public abstract string ShopItemName { get; }

        public virtual List<ModItem> RequiredItems => [];
        public abstract string Description { get; }

        protected abstract int Price { get; }

        public int Cost = 0;

        public virtual int UnlockRound => 0;

        public virtual float CostMultiplier => 1;

        public virtual void OnItemBuy(Game game, InGame inGame, int amount)
        {
        
        }

        public void Buy(Game game, InGame inGame, int amount = 1)
        {
            int totalCost = Price;

            for(int i = 1; i < amount; i++)
            {
                totalCost = (int)(totalCost * CostMultiplier);
            }

            if (totalCost > inGame.GetCash())
            {
                PopupScreen.instance.SafelyQueue(screen => screen.ShowOkPopup("You don't have enough to buy this!"));
            }
            else
            {
                Item.ChangeAmount(amount);

                inGame.AddCash(totalCost);

                if (!Item.Universal || (Item.Universal && !SandboxFlag))
                {
                    OnItemBuy(game, inGame, amount);
                }

                currData.ModifyItemData(Item);

                if(Stock_ > 0)
                {
                    Stock -= amount;
                }
            }

        }

        protected override int Order => ((int)Item.Rarity) + 1;
    }

    /// <summary>
    /// Abstract class to use for adding shop entries for mod items
    /// </summary>
    /// <typeparam name="T">Mod Item the shop entry is for</typeparam>
    public abstract class ShopEntry<T> : ShopEntry where T : ModItem
    {
        public ShopEntry() { }

        public override ModItem Item => GetInstance<T>();

        public override string Icon => Item.Icon;

        public override string ShopItemName => Item.ItemName;

        public override string Description => Item.Description;
    }

    /// <summary>
    /// Creates a <see cref="ShopEntry{T}"/> with a single required mod item
    /// </summary>
    /// <typeparam name="T">Mod Item the shop entry is for</typeparam>
    /// <typeparam name="K">Mod item required for this entry to appear in the shop</typeparam>
    public abstract class ShopEntry<T, K> : ShopEntry<T> where T : ModItem where K : ModItem
    {
        public override List<ModItem> RequiredItems => [GetInstance<K>()];
    }
}
