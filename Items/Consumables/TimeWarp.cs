using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

namespace RPGMod.Items.Consumables
{
    internal class TimeWarp : ModItem
    {
        public override ItemType Type => ItemType.Consumable;

        public override ItemRarity Rarity => ItemRarity.Blue;

        public override string Description => "Skips 5 Rounds";

        public override int Max => 3;

        public override int StartAmount => 1;

        public override void OnItemUse(Game game, InGame inGame, int amount)
        {
            for(int i = 0; i < amount; i++)
            {
                inGame.bridge.SetRound(inGame.bridge.GetCurrentRound() + 5);
            }
        }
    }

    internal class TimeWarpShop : ShopEntry<TimeWarp>
    {
        protected override int Price => 10000;

        protected override int Stock_ => 2;
    }
}
