using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMod.Items.Consumables
{
    internal class TLCSpawn : ModItem
    {
        public override ItemType Type => ItemType.Consumable;

        public override ItemRarity Rarity => ItemRarity.Purple;

        public override string ItemName => "Cash Pendant";

        public override string Description => "Spawns the Titan of Lots of Cash (TLC), who has 1 million health, but gives you $50000! (works with double cash)";

        public override void OnItemUse(Game game, InGame inGame, int amount)
        {
            base.OnItemUse(game, inGame, amount);
        }
    }

    internal class TLCSpawnShop : ShopEntry<TLCSpawn>
    {
        protected override int Price => 10000;
    }

    internal class TLC : ModBloon
    {
        public override string BaseBloon => BloonType.Bad;

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.maxHealth = 1000000;
            bloonModel.leakDamage = 0;

            bloonModel.GetBehavior<DistributeCashModel>().cash = 500000;

            bloonModel.RemoveAllChildren();
            SpawnBloonsActionModel spawnCashBloons = new("SpawnBloonsActionModel_SpawnCashBloons", "SpawnCashBloons", BloonID<CashBloon>(), 3, 1, 10, 1, 15, new(0), new(0), 0, false, "RPGMod-TLC");
            TimeTriggerModel trigger = new("TimeTriggerModel_TLC", 15, false, new([spawnCashBloons.actionId]));

            bloonModel.AddBehavior(trigger);
        }
    }

    internal class CashBloon : ModBloon
    {
        public override string BaseBloon => BloonType.Red;

        public override IEnumerable<string> DamageStates => ["CashBloon", "CashBloonDamage1", "CashBloonDamage2", "CashBloonDamage3", "CashBloonDamage4", "CashBloonDamage5",];

        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            bloonModel.speed = 85;
            bloonModel.leakDamage = 0;
            bloonModel.GetBehavior<DistributeCashModel>().cash = 10;
            bloonModel.maxHealth = 100;

            bloonModel.GetBehavior<DisplayModel>().scale = 3;

            bloonModel.disallowCosmetics = true;
        }
    }

    internal class TLCDisplay : ModBloonDisplay<TLC>
    {
        public override string BaseDisplay => GetBloonDisplay("Moab");

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, "TLCStandard");
        }
    }
    internal class TLCDisplay1 : ModBloonDisplay<TLC>
    {
        public override string BaseDisplay => GetBloonDisplay("Moab");

        public override int Damage => 1;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, "TLCDamage1");
        }
    }
    internal class TLCDisplay2 : ModBloonDisplay<TLC>
    {
        public override string BaseDisplay => GetBloonDisplay("Moab");

        public override int Damage => 2;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, "TLCDamage2");
        }
    }
    internal class TLCDisplay3 : ModBloonDisplay<TLC>
    {
        public override string BaseDisplay => GetBloonDisplay("Moab");

        public override int Damage => 3;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, "TLCDamage3");
        }
    }
    internal class TLCDisplay4 : ModBloonDisplay<TLC>
    {
        public override string BaseDisplay => GetBloonDisplay("Moab");

        public override int Damage => 4;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, "TLCDamage4");
        }
    }
}
