using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.ChallengeEditor;
using Il2CppNinjaKiwi.Common;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using System;
using UnityEngine;

namespace RPGMod.Ui.Screens
{
    internal class ProfileScreen : ModGameMenu<ExtraSettingsScreen>
    {
        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            CommonForegroundHeader.SetText("RPG Profile");

            var panelTransform = GameMenu.gameObject.GetComponentInChildrenByName<RectTransform>("Panel");
            var panel = panelTransform.gameObject;
            panel.DestroyAllChildren();

            var mainPanel = panel.AddModHelperPanel(new Info("Panel", 3600, 1900));
            CreateMainPanel(mainPanel);

            return true;
        }

        public void CreateMainPanel(ModHelperPanel mainPanel)
        {
            mainPanel.transform.DestroyAllChildren();

            var panel = mainPanel.AddPanel(new("RPG_Profile", 3300, 1700), VanillaSprites.MainBGPanelBlue);

            var userProfileIcon = panel.AddImage(new("UserProfile", -1450, 700, 250), VanillaSprites.NamedMonkeyIcon);
            var username = panel.AddText(new("Username", -950, 700, 700, 250), Player.PlayerName);
            username.Text.fontSizeMax = 100;
            username.Text.color = Player.NameColour;
            username.Text.enableAutoSizing = true;
            username.Text.alignment = Il2CppTMPro.TextAlignmentOptions.Left;

            /*var unitedXPScroll = panel.AddScrollPanel(new("UnitedXP", 0, -100, 800, 1400), RectTransform.Axis.Vertical, VanillaSprites.BlueInsertPanelRound, 75, 75);
            var unitedXPText = panel.AddImage(new("UnitedXP", 0, 700, 360, 211), GetTextureGUID<RPGMod>("UnitedXP"));
            if (Player.HasUnitedXP)
            {
                foreach (string key in Player.UnitedXP.Keys)
                {
                    try
                    {
                        unitedXPScroll.AddScrollContent(UnitedXpPanel(key));
                    }
                    catch { }
                }
            }
            else
            {
                panel.AddText(new("NotUnlocked", 0, -100, 500), "You haven't unlocked this yet!", 75);
            }*/

            var savesScroll = panel.AddScrollPanel(new("Saves", 1200, -100, 800, 1400), RectTransform.Axis.Vertical, VanillaSprites.BlueInsertPanelRound, 75, 75);
            var savesText = panel.AddText(new("Text_Saves", 1200, 700, 800, 200), "Saves", 150);

            foreach (var save in Player.Saves)
            {
                try
                {
                    savesScroll.AddScrollContent(SavePanel(save));
                }
                catch { }
            }

            if (Player.HasUniversalXP)
            {
                var universalXPIcon = panel.AddImage(new("UniversalXP", -1200, -300, 600), GetTextureGUID<RPGMod>("UniversalXP"));
                var universalXPAmount = panel.AddText(new("Text_UniversalXP", -1200, -600, 600, 200), $"{Math.Round(Player.UniversalXP, 2)} Universal XP", 75);
                universalXPAmount.Text.enableAutoSizing = true;
                universalXPAmount.Text.fontSizeMax = 100;
            }

            var items = panel.AddButton(new("UniversalItems", -1200, 300, 600), GetTextureGUID<RPGMod>("UniversalItemBagButton"), new Action(() =>
            {
                Open<UniversalItemsScreen>();
                MenuManager.instance.buttonClickSound.Play();
            }));
        }

        public static string MapName(string internalName)
        {
            if (internalName != null)
            {
                string name = internalName.Spaced();

                return internalName switch
                {
                    "Tutorial" => "Monkey Meadow",
                    _ => name
                };
            }
            return internalName;
        }

        public ModHelperPanel SavePanel(RpgGameData save)
        {
            var panel = ModHelperPanel.Create(new("Save_" + save.MapName, 700), VanillaSprites.MainBGPanelGrey);
            var mapText = panel.AddText(new("Map", 0, 250, 650, 100), MapName(save.MapName));
            mapText.Text.fontSizeMax = 100;
            mapText.Text.enableAutoSizing = true;

            var modeText = panel.AddText(new("Mode", 0, 150, 650, 50), $"{save.ModeName} on {save.Difficuly}");
            modeText.Text.fontSizeMax = 50;
            modeText.Text.enableAutoSizing = true;
            modeText.Text.alignment = Il2CppTMPro.TextAlignmentOptions.Left;

            var roundText = panel.AddText(new("Round", 0, 80, 650, 50), $"Round {save.Round + 1}");
            roundText.Text.enableAutoSizing = true;
            roundText.Text.alignment = Il2CppTMPro.TextAlignmentOptions.Left;
            roundText.Text.fontSizeMax = 50;

            var masteryButton = panel.AddButton(new("Mastery", -160, -160, 300), GetTextureGUID<RPGMod>("TowerMasteryBtn"), new Action(() => { 
                GameDataScreen.Save = save; GameDataScreen.OpenScreen = GameDataScreen.Screen.TowerMastery; Open<GameDataScreen>();
                MenuManager.instance.buttonClickSound.Play(); }));
            var itemsButton = panel.AddButton(new("Mastery", 160, -160, 300), GetTextureGUID<RPGMod>("BagBtn"), new Action(() => {
                GameDataScreen.Save = save; GameDataScreen.OpenScreen = GameDataScreen.Screen.Items; Open<GameDataScreen>();
                MenuManager.instance.buttonClickSound.Play();
            }));

            return panel;
        }

        public ModHelperPanel UnitedXpPanel(string baseId)
        {
            ModHelperPanel panel = ModHelperPanel.Create(new("Panel_RPGMod_XPData_" + baseId, 0, 0, 800, 600), VanillaSprites.MainBGPanelYellow);
            panel.AddPanel(new("Gradient", InfoPreset.FillParent), VanillaSprites.MainBGPanelHighlightTab);

            var towerIcon = ModContent.GetSpriteReference<RPGMod>("MissingIcon");

            if(Game.instance.model.GetTowerFromId(baseId) != null)
            {
                towerIcon = Game.instance.model.GetTowerFromId(baseId).icon;
            }

            var icon = panel.AddImage(new("Icon", 0, 0, 400), towerIcon.GUID);

            var id = panel.AddText(new("Text_Id", 0, panel.RectTransform.rect.bottom - 80, 480, 150), baseId.Spaced(), 60);

            var xpText = panel.AddText(new("Text_XP", 0, panel.RectTransform.rect.top + 160, 800, 150), $"{Math.Round(Player.UnitedXP[baseId], 2)}", 50);
            xpText.Text.color = new(1, 0.55f, 0);

            int level = TowerXPData.CalculateLevel(Player.UnitedXP[baseId]);

            var levelText = panel.AddText(new("Text_Level", 0, panel.RectTransform.rect.top + 80, 760, 150), $"Default Level: {level}", 50);
            if (level == 0)
            {
                levelText.Text.color = Color.black;
            }
            else if (level < 10)
            {
                levelText.Text.color = Color.gray;
            }
            else if (level < 20)
            {
                levelText.Text.color = Color.white;
            }
            else if (level < 30)
            {
                levelText.Text.color = Color.blue;
            }
            else if (level < 40)
            {
                levelText.Text.color = Color.green;
            }
            else if (level < 50)
            {
                levelText.Text.color = Color.red;
            }
            else if (level < 60)
            {
                levelText.Text.color = Color.yellow;
            }
            else if (level > 60)
            {
                levelText.Text.color = new(1, 0.85f, 0);
            }

            return panel;
        }
    }
}