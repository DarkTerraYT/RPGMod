using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using System;

namespace RPGMod.Items.Unlocks
{
    public class MasteryStar : ModItem
    {
        public override string Icon => GetTextureGUID<RPGMod>("MasteryStar");

        public override ItemType Type => ItemType.Unlock;

        public override ItemRarity Rarity => ItemRarity.Blue;

        public override int Max => 1;

        public override string Description => "Unlocks Tower Mastery";
    }

    public class MasteryStarShop : ShopEntry<MasteryStar>
    {
        protected override int Price => 10000;

        public override bool AutoGiveItem => currData.GetBoolStat("Mastery").Value;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            currData.GetBoolStat("Mastery").Value = true;
        }
    }
}
