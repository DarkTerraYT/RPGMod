using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

namespace RPGMod.Items.Universal
{
    internal class StarsOfUnity : ModItem
    {
        public override ItemType Type => ItemType.Unlock;

        public override ItemRarity Rarity => ItemRarity.Red;

        public override string Description => "5% of ALL tower xp is kept between playthroughs. Note: XP from sandbox games do not count";

        public override int Max => 1;

        public override bool Universal => true;
    }

    internal class StarsOfUnityShop : ShopEntry<StarsOfUnity, MasteryStar2>
    {
        protected override int Price => 500000;

        protected override bool ShowedByDefault => false; 

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            Player.HasUnitedXP = true;

            foreach (var data in currData.XPData)
            {
                Player.AddUnitedXP(data);
            }
        }
    }
}
