namespace RPGMod.Quests.Steps
{
    public class GenerateCash : QuestStep
    {
        public double CashToGenerate = 1000;

        public double CashGenerated = 0;

        public override string QuestId => "GenerateCash";

        public override void OnStart(int level)
        {
            CashToGenerate *= level;
        }
    }
}
