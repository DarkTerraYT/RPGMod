using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

namespace RPGMod.Items.Relics
{
    internal class SilverPitchfork : ModItem
    {
        public override ItemType Type => ItemType.Relic;

        public override ItemRarity Rarity => ItemRarity.Green;

        public override string Description => "30% Higher Cash Generation (Additive, + 30% from pitchforks)";

        public override int Max => 1;
    }

    internal class SPfShop : ShopEntry<SilverPitchfork>
    {
        protected override int Price => 2000;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            currData.GetNumberStat("CashMulti").Value += .3;
        }
    }

    internal class GoldPitchfork : ModItem
    {
        public override ItemType Type => ItemType.Relic;

        public override ItemRarity Rarity => ItemRarity.Blue;

        public override string Description => "40% Higher Cash Generation (Additive, +70% from pitchforks)";

        public override int Max => 1;
    }

    internal class GPfShop : ShopEntry<GoldPitchfork, SilverPitchfork>
    {
        protected override int Price => 8000;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            currData.GetNumberStat("CashMulti").Value += .4;
        }
    }

    internal class DiamondPitchfork : ModItem
    {
        public override ItemType Type => ItemType.Relic;

        public override ItemRarity Rarity => ItemRarity.Purple;

        public override string Description => "50% Higher Cash Generation (Additive, +120% from pitchforks)";

        public override int Max => 1;
    }

    internal class DPfShop : ShopEntry<DiamondPitchfork, GoldPitchfork>
    {
        protected override int Price => 20000;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            currData.GetNumberStat("CashMulti").Value += .5;
        }
    }
}
