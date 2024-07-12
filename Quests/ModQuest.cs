using BTD_Mod_Helper.Api;
using RPGMod.Quests.Steps;

namespace RPGMod.Quests
{
    public abstract class ModQuest : NamedModContent
    {
        public ModQuest() { }

        public override void Register()
        {
            ActiveSteps = OriginalSteps;
        }

        public abstract int Chance { get; }

        public abstract string RewardDesc { get; }

        public abstract void Reward();

        public float FinishPercent = 0;

        public virtual string Icon => GetSpriteReference<RPGMod>("QuestIcon").GUID;

        public bool CheckDone()
        {
            bool done = true;
            foreach (var step in ActiveSteps)
            {
                if (!step.isFinished)
                {
                    done = false;
                }
            }
            if (done)
            {
                State = QuestState.FINISHED;
            }
            return done;
        }

        public virtual bool Unique => false;

        public int TimesShown = 0;

        public virtual QuestStep[] OriginalSteps => [GetInstance<GenerateCash>()];

        public QuestStep[] ActiveSteps = [];

        public QuestState State = QuestState.UNACTIVE;

        public enum QuestState
        {
            UNACTIVE, // Can be rolled
            ACTIVE, // Currently active
            FINISHED, // Quest is finished and can be turned in
        }

        public struct QuestInfo(string name)
        {
            public string Name = name;
        }
    }
}
