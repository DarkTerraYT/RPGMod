namespace RPGMod.Quests.Steps
{
    internal class CompleteRounds : QuestStep
    {
        public override string QuestId => "CompleteRounds1";

        public int RoundsToBeat = 5;

        public override void OnStart(int level)
        {

        }
    }
}
