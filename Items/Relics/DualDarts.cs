using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMod.Items.Relics
{
    internal class DualDarts : ModItem
    {
        public override ItemType Type => ItemType.Relic;

        public override ItemRarity Rarity => ItemRarity.Green;

        public override string Description => "All currently placed, and all future dart monkeys shoot one more dart.";

        protected override void ModifyTowerModels(List<TowerModel> towerModels)
        {
            foreach(TowerModel model in towerModels)
            {
                if(model.baseId == TowerType.DartMonkey)
                {
                    foreach(var weapon in model.GetWeapons())
                    {
                        if(weapon.emission.GetType() == typeof(ArcEmissionModel))
                        {
                            var emission = weapon.emission.Cast<ArcEmissionModel>();
                            emission.Count += 1;
                        }
                        else
                        {
                            var emission = new ArcEmissionModel("ArcEmissionModel_", 2, 0, 5, weapon.emission.behaviors, false, false);
                            weapon.emission = emission;
                        }
                    }
                }
            }
        }
    }

    internal class DualDartsShop : ShopEntry<DualDarts>
    {
        protected override int Price => 2150;
    }
}
