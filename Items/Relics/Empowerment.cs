using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMod.Items.Relics
{
    internal class Empowerment : ModItem
    {
        public override ItemType Type => ItemType.Relic;

        public override ItemRarity Rarity => ItemRarity.Blue;

        public override string Description => "Magic monkeys shoot 10% faster, and have 1 more pierce";

        protected override void ModifyTowerModels(List<TowerModel> towerModels)
        {
            foreach (var model in towerModels)
            {
                if(model.towerSet == Il2CppAssets.Scripts.Models.TowerSets.TowerSet.Magic)
                {
                    foreach(var weapon in model.GetDescendants<WeaponModel>().ToList())
                    {
                        weapon.rate *= 0.9f;
                        foreach(var projectile in weapon.GetDescendants<ProjectileModel>().ToList())
                        {
                            projectile.pierce++;
                        }
                    }
                }
            }
        }
    }
}
