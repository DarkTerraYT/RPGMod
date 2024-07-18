using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using RPGMod.Items.Universal;

namespace RPGMod.Items.Relics
{
    public class ExpNecklace : ModItem
    {
        public override ItemType Type => ItemType.Relic;

        public override string ItemName => "EXP Necklace";

        public override ItemRarity Rarity => ItemRarity.Purple;

        public override string Description => "Provides 25% Higher EXP Gain";

        public override int Max => 1;
    }

    public class ExpNecklaceShop : ShopEntry<ExpNecklace>
    {
        protected override int Price => 25000;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            currData.GetNumberStat(RpgGameData.EXPMULTI).Value += .25;
        }
    }
}
