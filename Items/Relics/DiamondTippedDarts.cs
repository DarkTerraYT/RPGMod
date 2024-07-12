using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMod.Items.Relics
{
    internal class DiamondTippedDarts : ModItem
    {
        public override ItemType Type => ItemType.Relic;

        public override ItemRarity Rarity => ItemRarity.Blue;

        public override string Description => "Increases all damage by 30% (Applied after levels)";

        public override int Max => 2;

        public override ItemApplyMethod ApplyMethod => ItemApplyMethod.PostLevels;

        protected override void ModifyTowerModels(List<TowerModel> towerModels)
        {
            foreach (TowerModel model in towerModels)
            {
                foreach (var dmgModel in model.GetDescendants<DamageModel>().ToList())
                {
                    dmgModel.damage *= 1.3f;
                }
            }
        }
    }

    internal class DTDShop : ShopEntry<DiamondTippedDarts>
    {
        protected override int Price => 5650;

        public override float CostMultiplier => 5;
    }
}
