using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMod.Items.Universal
{
    internal class MasteryStar : ModItem
    {
        public override ItemType Type => ItemType.Unlock;

        public override ItemRarity Rarity => ItemRarity.Blue;

        public override int Max => 1;

        public override string Description => "Unlocks tower mastery.";
    }

    internal class MasteryStarShop : ShopEntry<MasteryStar>
    {
        protected override int Price => 3500;

        public override int UnlockRound => 23;

        protected override int Stock_ => 1;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            currData.GetBoolStat("Mastery").Value = true;
        }
    }
}
