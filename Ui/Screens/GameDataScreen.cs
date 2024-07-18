using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.ChallengeEditor;
using Il2CppAssets.Scripts.Unity.UI_New.Settings;
using Il2CppNinjaKiwi.Common;
using Il2CppSystem;
using RPGMod.Items;
using System.Buffers.Text;
using UnityEngine;

namespace RPGMod.Ui.Screens
{
    internal class GameDataScreen : ModGameMenu<SettingsScreen>
    {
        public static RpgGameData Save;

        public static Screen OpenScreen = Screen.TowerMastery;

        ModHelperPanel MainPanel;

        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            CommonForegroundHeader.SetText("RPG Profile Viewer");

            GameMenu.transform.DestroyAllChildren();

            MainPanel = GameMenu.gameObject.AddModHelperPanel(new Info("ProfileViewer", 3600, 1900));

            CreatePanel(OpenScreen);

            return false;
        }

        public void CreatePanel(Screen screen)
        {
            var panel = MainPanel.AddPanel(new("Background_Panel", 3300, 1700), VanillaSprites.MainBGPanelBlue);

            var scroll = panel.AddScrollPanel(new("Scroll", 2600, 1000), RectTransform.Axis.Horizontal, VanillaSprites.BlueInsertPanelRound, 50, 50);

            if (screen == Screen.TowerMastery)
            {
                var text = panel.AddText(new("Title", 0, 1500, 500, 300), "Tower Mastery");
                text.Text.fontSizeMax = 150;
                text.Text.enableAutoSizing = true;

                foreach (var data in Save.XPData)
                {
                    scroll.AddScrollContent(XpPanel(data));
                }
            }
            else if (screen == Screen.Items)
            {
                var text = panel.AddText(new("Title", 0, 1500, 500, 300), "Items");
                text.Text.fontSizeMax = 150;
                text.Text.enableAutoSizing = true;

                foreach (var item in Save.LoadItemsFromSave())
                {
                    scroll.AddScrollContent(ItemPanel(item));
                }
            }
            else if(screen == Screen.Quests)
            {
                var text = panel.AddText(new("Title", 0, 1500, 500, 300), "Quests");
                text.Text.fontSizeMax = 150;
                text.Text.enableAutoSizing = true;
            }
        }

        public ModHelperPanel XpPanel(TowerXPData data)
        {
            ModHelperPanel panel = ModHelperPanel.Create(new("Panel_RPGMod_XPData_" + data.BaseId, 0, 0, 800, 600), VanillaSprites.BlueInsertPanel);

            var towerIcon = ModContent.GetSpriteReference<RPGMod>("MissingIcon");

            if (Game.instance.model.GetTowerFromId(data.BaseId) != null)
            {
                towerIcon = Game.instance.model.GetTowerFromId(data.BaseId).icon;
            }
            var icon = panel.AddImage(new("Icon", 0, 0, 400), towerIcon.GUID);

            var id = panel.AddText(new("Text_Id", 0, panel.RectTransform.rect.bottom - 80, 480, 150), data.BaseId.Spaced(), 60);

            var xpText = panel.AddText(new("Text_XP", 0, panel.RectTransform.rect.top + 160, 800, 150), $"{Math.Round(data.XP, 1)} / {data.XPToLevelUp} EXP", 50);
            xpText.Text.color = Color.green;

            if (data.Level == MaxLevel)
            {
                xpText.SetText($"{Math.Round(data.XP, 1)} EXP");
            }

            var levelText = panel.AddText(new("Text_Level", 0, panel.RectTransform.rect.top + 80, 760, 150), $"Level {data.Level}", 50);
            if (data.Level == 0)
            {
                levelText.Text.color = Color.black;
            }
            else if (data.Level < 10)
            {
                levelText.Text.color = Color.gray;
            }
            else if (data.Level < 20)
            {
                levelText.Text.color = Color.white;
            }
            else if (data.Level < 30)
            {
                levelText.Text.color = Color.blue;
            }
            else if (data.Level < 40)
            {
                levelText.Text.color = Color.green;
            }
            else if (data.Level < 50)
            {
                levelText.Text.color = Color.red;
            }
            else if (data.Level < 60)
            {
                levelText.Text.color = Color.yellow;
            }
            else if (data.Level > 60)
            {
                levelText.Text.color = new(255, 162, 0);
            }
            if (data.Level >= 50)
            {
                panel.AddImage(new("Image_Level_50_Badge", panel.RectTransform.rect.left + 75, panel.RectTransform.rect.bottom - 75, 150), ModContent.GetSpriteReference<RPGMod>("Lvl50Badge").GUID);
                if (data.Level == MaxLevel)
                {
                    panel.AddImage(new("Image_Level_Max_Badge", panel.RectTransform.rect.right - 75, panel.RectTransform.rect.bottom - 75, 150), ModContent.GetSpriteReference<RPGMod>("MaxLvlBadge").GUID);
                }
                else
                {
                    panel.AddImage(new("Image_Level_Max_Badge", panel.RectTransform.rect.right - 75, panel.RectTransform.rect.bottom - 75, 150), ModContent.GetSpriteReference<RPGMod>("MaxLvlBadgeUnachieved").GUID);
                }
            }
            else
            {
                panel.AddImage(new("Image_Level_50_Badge", panel.RectTransform.rect.left + 75, panel.RectTransform.rect.bottom - 75, 150), ModContent.GetSpriteReference<RPGMod>("Lvl50BadgeUnachieved").GUID);
                panel.AddImage(new("Image_Level_Max_Badge", panel.RectTransform.rect.right - 75, panel.RectTransform.rect.bottom - 75, 150), ModContent.GetSpriteReference<RPGMod>("MaxLvlBadgeUnachieved").GUID);
            }

            return panel;
        }

        public ModHelperPanel ItemPanel(ModItem item)
        {
            var panel = ModHelperPanel.Create(new("Item_Panel_" + item.Id, 950), VanillaSprites.MainBGPanelGrey);

            var icon = panel.AddImage(new("Icon", 0, 125, 450), item.Icon);

            var name = panel.AddText(new("Name", 0, 400, 900, 100), item.ItemName, 90);
            name.Text.fontSizeMax = 90;
            name.Text.fontSizeMin = 30;
            name.Text.enableAutoSizing = true;

            var description = panel.AddText(new("Description", 0, -225, 900, 250), item.Description, 55);
            description.Text.fontSizeMax = 60;
            description.Text.fontSizeMin = 20;
            description.Text.enableAutoSizing = true;

            var count = panel.AddText(new("Count", 0, -400, 900, 100), item.Amount.ToString(), 55);
            count.Text.fontSizeMax = 60;
            count.Text.fontSizeMin = 20;
            count.Text.enableAutoSizing = true;
            count.Text.color = Color.yellow;

            return panel;
        }

        public enum Screen 
        {
            TowerMastery,
            Items,
            Quests
        }
    }
}
