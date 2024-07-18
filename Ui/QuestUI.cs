using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using System;
using UnityEngine;

namespace RPGMod.Ui
{
    [RegisterTypeInIl2Cpp(false)]
    public class QuestUI : MonoBehaviour
    {
        public static QuestUI? instance;

        public void Close()
        {
            if (gameObject)
            {
                Destroy(gameObject);
            }
        }

        public static void CreateQuestButton()
        {
            UiRect = InGame.instance.mapRect;

            if (QuestUI.instance != null)
            {
                QuestUI.instance.Close();
            }

            var panel = UiRect.gameObject.AddModHelperPanel(new("Panel_RPGMod_Quests", UiRect.rect.right - 135, UiRect.rect.bottom - 975, 200), ModContent.GetSpriteReference<RPGMod>("empty-1x1").GUID);
            QuestUI.instance = panel.AddComponent<QuestUI>();


            var button = panel.AddButton(new("Button_Quests", 0, 0, 200), ModContent.GetSpriteReference<RPGMod>("QuestBtnOff").GUID, new Action(() =>
            {
                /*QuestUI.instance.Close();

                if (MasteryUI.instance != null)
                {
                    MasteryUI.instance.Close();
                }

                float questPanelW = 550;
                float questPanelH = 815;

                float panelW = questPanelW * 3.5f;
                float panelH = questPanelH * 1.2f;

                var panel_ = UiRect.gameObject.AddModHelperPanel(new("Panel_Quests", 0, 0, panelW, panelH), VanillaSprites.BrownInsertPanel);
                QuestUI.instance = panel_.AddComponent<ExtraUiMethods>();

                var button = panel_.AddButton(new("Btn_Back", panel_.RectTransform.UiRect.right, panel_.RectTransform.UiRect.top, 200), VanillaSprites.BackBtn, new Action(() =>
                {
                    if (MasteryUI.instance == null)
                    {
                        CreateMasteryButton(currData.XPData);
                    }
                    CreateQuestButton();
                }));

                for (int i = 0; i < 3; i++)
                {
                    float x = -questPanelW + (i * questPanelW) + (55 * i);

                    System.Random rand = new();
                    int num = rand.Next(0, 101);
                    foreach (var quest in ModContent.GetContent<ModQuest>())
                    {
                        if (num <= quest.Chance)
                        {
                            var questPanel = panel_.AddPanel(new("QuestPanel_" + quest.DisplayName, x, 0, questPanelW, questPanelH), VanillaSprites.GreyInsertPanel);

                            var icon = questPanel.AddImage(new("Icon_", 0, questPanel.RectTransform.UiRect.bottom - 80, 200), quest.Icon);

                            var name = questPanel.AddText(new("Text_Name", 0, questPanel.RectTransform.UiRect.bottom - 230, questPanelW, 100), quest.DisplayName)
                        }
                    }
                }
                */

                ExtraUiMethods.Locked("Quest Scroll", true);
            }));
        }
    }
}
