using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using RPGMod.Quests.Steps;

namespace RPGMod.Quests
{
    internal class CashQuest : ModQuest
    {
        public override int Chance => 100;

        public override string DisplayName => "Generate Cash";

        public override string Description => $"Generate {ActiveSteps[0]} cash";

        public override string RewardDesc => "25% faster towers.";

        public override QuestStep[] OriginalSteps => [GetInstance<GenerateCash>()];

        public override void Reward()
        {
            foreach (var tower in InGame.instance.GetTowers())
            {
                foreach (var weapon in tower.towerModel.GetWeapons())
                {
                    weapon.rate *= 0.75f;
                }
            }
        }
    }
}
