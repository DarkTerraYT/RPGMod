using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using RPGMod.Ui;
using System;

namespace RPGMod.Items.Consumables
{
    public class HeartCandy : ModItem
    {
        public override ItemType Type => ItemType.Consumable;

        public override ItemRarity Rarity => ItemRarity.Basic;

        public override string Icon => GetTextureGUID<RPGMod>("HeartCandy");

        public override string Description => "Heals 10 lives, respects max life count.";

        private bool PopupOpen = false;

        public override int StartAmount => 5;

        public override void OnItemUse(Game game, InGame inGame, int amount)
        {
            if(inGame.GetHealth() >= inGame.GetMaxHealth())
            {
                inGame.SetHealth(inGame.GetMaxHealth());
                if (!PopupOpen)
                {
                    PopupScreen.instance.SafelyQueue(screen => { screen.ShowOkPopup("You've already reached max health!", new Action(() => { ChangeAmount(amount); PopupOpen = false; if (BagUI.InfoPanel != null) { BagUI.InfoPanel.GetDescendent<ModHelperText>("Text_Amount").SetText(Amount.ToString()); } })); PopupOpen = true; });
                }
            }
            else if(inGame.GetHealth() + 10 * amount > inGame.GetMaxHealth())
            {
                int amountToUse = (int)(inGame.GetMaxHealth() - inGame.GetHealth()) / 10;
                inGame.SetHealth(inGame.GetMaxHealth());
                ChangeAmount(amount - amountToUse);
            }
            else 
            {
                inGame.AddHealth(10 * amount);
            }
        }
    }

    public class HeartCandyShop : ShopEntry<HeartCandy>
    {
        protected override int Price => 800;
    }
}