using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMod.Items.Consumables
{
    internal class LesserTimeWarp : ModItem
    {
        public override ItemType Type => ItemType.Consumable;

        public override ItemRarity Rarity => ItemRarity.Green;

        public override string Description => "Skips 1 Round";

        public override int Max => 3;

        public override int StartAmount => 1;

        public override void OnItemUse(Game game, InGame inGame, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                inGame.bridge.SetRound(inGame.bridge.GetCurrentRound() + 1);
            }
        }
    }

    internal class LesserTimeWarpShop : ShopEntry<LesserTimeWarp>
    {
        protected override int Price => 2000;

        protected override int Stock_ => 2;
    }
}
