using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMod.Items.Universal
{
    internal class StarSeal1 : ModItem
    {
        public override ItemType Type => ItemType.Relic;

        public override ItemRarity Rarity => ItemRarity.Red;

        public override int Max => 1;

        public override bool Universal => true;

        public override string Description => "Increases level cap from 65 to 70";

        public override string ItemName => "Star Badge";

        public override bool Hidden => GetInstance<StarSeal2>().Amount > 0;
    }
    internal class StarSeal2 : ModItem
    {
        public override ItemType Type => ItemType.Relic;

        public override ItemRarity Rarity => ItemRarity.Red;

        public override int Max => 1;

        public override bool Universal => true;

        public override string Description => "Increases level cap from 70 to 75";

        public override string ItemName => "Star Seal";

        public override bool Hidden => GetInstance<StarSeal3>().Amount > 0;
    }
    internal class StarSeal3 : ModItem
    {
        public override ItemType Type => ItemType.Relic;

        public override ItemRarity Rarity => ItemRarity.Red;

        public override bool Universal => true;

        public override int Max => 1;

        public override string Description => "Increases level cap from 75 to 80";

        public override string ItemName => "Star Insigna";

        public override bool Hidden => GetInstance<StarSeal3>().Amount == 0;
    }

    internal class StarSeal1Shop : ShopEntry<StarSeal1>
    {
        protected override int Price => 350000;
        public override int UnlockRound => 150;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            MaxLevel_ += 5;
        }
    }
    internal class StarSeal2Shop : ShopEntry<StarSeal2, StarSeal1>
    {
        protected override int Price => 450000;
        public override int UnlockRound => 175;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            MaxLevel_ += 5;
        }
    }
    internal class StarSeal3Shop : ShopEntry<StarSeal3, StarSeal2>
    {
        protected override int Price => 550000;
        public override int UnlockRound => 200;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            MaxLevel_ += 5;
        }
    }
}
