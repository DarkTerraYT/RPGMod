using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

namespace RPGMod.Items.Universal
{
    internal class UniversalStar : ModItem
    {
        public override ItemType Type => ItemType.Unlock;

        public override ItemRarity Rarity => ItemRarity.Red;

        public override string Description => "Unlocks Universal XP. Universal XP is XP that goes on every tower. 1% of xp is earned as universal xp.";

        public override int Max => 1;

        public override bool Universal => true;
    }

    internal class UniversalStarShop : ShopEntry<UniversalStar, StarsOfUnity>
    {
        protected override int Price => 5000000;

        protected override bool ShowedByDefault => false;

        public override int UnlockRound => 119;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            Player.HasUniversalXP = true;
        }
    }
}
