using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    internal class StarsOfUnityShop : ShopEntry<StarsOfUnity>
    {
        protected override int Price => 340000;

        public override int UnlockRound => 95;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            RpgUserData.HasUnitedXP = true;

            foreach(var data in currData.XPData)
            {
                RpgUserData.AddUnitedXP(data);
            }
        }
    }
}
