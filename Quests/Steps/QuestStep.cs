using BTD_Mod_Helper.Api;

namespace RPGMod.Quests.Steps
{
    public abstract class QuestStep : ModContent
    {
        public QuestStep() { }

        public override void Register()
        {
        }

        public bool isFinished = false;

        public ModQuest? Quest = null;

        public void Finish()
        {
            if (!isFinished)
            {
                isFinished = true;

                if (Quest != null)
                {
                    Quest.CheckDone();
                }
            }
        }

        public virtual void OnStart(int level)
        {

        }

        public abstract string QuestId { get; }
    }
}
