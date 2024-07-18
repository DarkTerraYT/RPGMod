using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMod.Items.Universal
{
    public class MasteryStar2 : ModItem
    {
        public override ItemType Type => ItemType.Unlock;

        public override ItemRarity Rarity => ItemRarity.Pink;

        public override string ItemName => "Ultimate Mastery Star";

        public override bool Universal => true; 

        public override string Description => "Unlocks tower mastery PERMENANTLY";
    }

    internal class MasteryStar2Shop : ShopEntry<MasteryStar2, MasteryStar>
    {
        protected override int Price => 33500;

        public override int UnlockRound => 62;

        public override void OnItemBuy(Game game, InGame inGame, int amount)
        {
            Player.UnlockedMasteryForever = true;
        }
    }
}
