using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;

namespace RPGMod.Ui
{
    [RegisterTypeInIl2Cpp(false)]
    public class MasteryUI : MonoBehaviour
    {
        public static MasteryUI? instance;

        public static bool masteryMenuOpen = false;

        public static bool unitedXPMenu = false;

        private static List<string> AddedIds = new(999999)
        {

        };

        public static void CreateMasteryButton(List<TowerXPData> data)
        {
            var panel = UiRect.gameObject.AddModHelperPanel(new("Panel_RPGMod_Quests", UiRect.rect.right - 135, UiRect.rect.bottom - 750, 200), ModContent.GetSpriteReference<RPGMod>("empty-1x1").GUID);
            instance = panel.AddComponent<MasteryUI>();
            if (currData.GetBoolStat("Mastery").Value)
            {
                var openButton = panel.AddButton(new("Button_Mastery", 0, 0, 200), ModContent.GetSpriteReference<RPGMod>("TowerMasteryBtn").GUID, new Action(() =>
                {
                    instance.Close();
                    if (QuestUI.instance != null)
                    {
                        QuestUI.instance.Close();
                    }
                    if (BagUI.instance != null)
                    {
                        BagUI.instance.Close();
                    }
                    if (ShopUI.instance != null)
                    {
                        ShopUI.instance.Close();
                    }

                    instance.OpenMasteryMenu(data);
                }));
            }
            else
            {
                var openButton = panel.AddButton(new("Button_Mastery", 0, 0, 200), ModContent.GetSpriteReference<RPGMod>("TowerMasteryBtnOff").GUID, new Action(() =>
                {
                    ExtraUiMethods.Locked("Mastery Star");
                }));
            }

        }

        public ModHelperPanel OpenMasteryMenu(List<TowerXPData> data, string search = "")
        {
            AddedIds.Clear();

            if (instance != null)
            {
                instance.Close();
            }

            masteryMenuOpen = true;

            var panel = UiRect.gameObject.AddModHelperPanel(new("Panel_RPGMod_Mastery_Panel", 0, 150, 840, 1550), VanillaSprites.BrownInsertPanel);
            var panel_ = panel.AddScrollPanel(new("Panel_RPGMod_Mastery_Scroll", 0, -150, 840, 1400), RectTransform.Axis.Vertical, VanillaSprites.BrownInsertPanel, 50, 20);
            instance = panel.AddComponent<MasteryUI>();

            MasteryPanel = panel;

            /*var filterBtn = panel.AddButton(new("Button_Filter", panel.RectTransform.UiRect.left, panel.RectTransform.UiRect.top, 150), VanillaSprites.BlueBtnCircleSmall, new Action(() =>
            {
                // Filter
            }));
            var filterImg = filterBtn.AddImage(new("Image_Filter", InfoPreset.FillParent), ModContent.GetSpriteReference<RPGMod>("FilterIcon").GUID);*/

            var text = ModHelperText.Create(new("Title_", 0, 1150, 840, 200), "Tower Mastery", 100);


            if (!unitedXPMenu && Player.HasUniversalXP)
            {
                ModHelperPanel universalXP = ModHelperPanel.Create(new("Panel_Universal_XP", 800, 600), VanillaSprites.MainBgPanelParagon);

                universalXP.AddImage(new("Image_Universal_XP", 0, 100, 600), ModContent.GetTextureGUID<RPGMod>("UniversalXP"));
                var text_ = universalXP.AddText(new("Text", 0, -200, 600), $"{(decimal)Player.UniversalXP} Universal XP", 70);
                text_.Text.fontSizeMax = 70;
                text_.Text.enableAutoSizing = true;

                text_.Text.colorGradient = new(new(1, 0, 0), new(1, 1, 0), new(1, 1, 0), new(0, 1, 0));
                text_.Text.enableVertexGradient = true;

                panel_.AddScrollContent(universalXP);
            }

            if (Player.HasUnitedXP)
            {
                ModHelperButton switchTab = panel.AddButton(new("Button_Switch", panel.RectTransform.rect.left, panel.RectTransform.rect.top - 75, 150), VanillaSprites.RestartBtn, new Action(() =>
                {
                    unitedXPMenu = !unitedXPMenu;
                    instance.OpenMasteryMenu(data, search);
                }));

                if (unitedXPMenu)
                {
                    switchTab.AddText(new("Text", 0, -100, 80), "Mastery XP", 45).Text.color = Color.green;
                }
                else
                {
                    switchTab.AddText(new("Text", 0, -100, 80), "United XP", 45).Text.color = new(1, 0.55f, 0);
                }
            }

            foreach (var data_ in data)
            {
                if (data_.Name.ToLower().Contains(search.ToLower()))
                {
                    panel_.AddScrollContent(CreateXPDataPanel(data_));
                    AddedIds.Add(data_.BaseId);
                }
            }

            if (data.Count == 0)
            {
                panel_.AddText(new("Empty_Text", 0, 0, 840, 640), "There are somehow no towers in this list?");
            }
            else if (AddedIds.Count < 0)
            {
                panel_.AddText(new("Empty_Text", 0, 0, 840, 640), "No tower names found containing \"" + search + "\"!", 100);
            }
            panel_.Add(text);

            var backBtn = panel.AddButton(new("Button_Back", panel.RectTransform.rect.right, panel.RectTransform.rect.top - 75, 150), VanillaSprites.BackBtn, new Action(() =>
            {
                instance.Close();
                CreateMasteryButton(data);
                if (QuestUI.instance == null)
                {
                    QuestUI.CreateQuestButton();
                }
                if (ShopUI.instance == null)
                {
                    ShopUI.CreateShopBtn();
                }
                if (BagUI.instance == null)
                {
                    BagUI.CreateBagBtn();
                }
            }));

            string newSearch = search;

            var searchButton = panel.AddButton(new("Button_Search", -340, 665, 130), VanillaSprites.BlueBtn, new Action(() =>
            {
                if (newSearch != search && instance != null)
                {
                    instance.Close();
                    instance.OpenMasteryMenu(data, newSearch);
                    LastSearch = newSearch;
                }
            }));
            var searchIcon = searchButton.AddImage(new("Image_Search", 0, 0, 80), VanillaSprites.SearchIcon);
            var searchInputField = panel.AddInputField(new("Input_Search", 60, 665, 650, 110), search, VanillaSprites.GreyInsertPanel,
                new Action<string>(input => newSearch = input)).Text.Text.fontSizeMin -= 10;

            return panel;
        }

        public static void Update(TowerXPData data, ModHelperPanel panel)
        {
            if (masteryMenuOpen && AutoUpdate)
            {
                if (InGame.instance != null)
                {
                    if (AddedIds.Contains(data.BaseId))
                    {
                        var panel_ = panel.GetDescendent<ModHelperScrollPanel>("Panel_RPGMod_Mastery_Scroll");

                        var scroll = panel_.ScrollContent;
                        if (!unitedXPMenu)
                        {
                            if (Player.HasUniversalXP)
                            {
                                var uniXP = scroll.GetDescendent<ModHelperPanel>("Panel_Universal_XP");
                                uniXP.GetDescendent<ModHelperText>("Text").SetText($"{Math.Round(Player.UniversalXP, 2)} Universal XP");
                            }


                            var info = scroll.GetDescendent<ModHelperPanel>("Panel_RPGMod_XPData_" + data.BaseId);

                            var xpText = info.GetDescendent<ModHelperText>("Text_XP");

                            xpText.Text.color = Color.green;

                            if (data.Level >= MaxLevel)
                            {
                                xpText.SetText($"{Math.Round(data.XP, 1)} EXP");
                            }
                            else
                            {
                                xpText.SetText($"{Math.Round(data.XP, 1)} / {data.XPToLevelUp} EXP");
                            }

                            var levelText = info.GetDescendent<ModHelperText>("Text_Level");

                            levelText.SetText("Level " + data.Level.ToString());

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
                            var lvl50Badge = info.GetDescendent<ModHelperImage>("Image_Level_50_Badge");
                            var lvlMaxBadge = info.GetDescendent<ModHelperImage>("Image_Level_Max_Badge");

                            if (data.Level >= MaxLevel / 2)
                            {
                                lvl50Badge.Image.SetSprite(ModContent.GetSpriteReference<RPGMod>("Lvl50Badge"));
                            }
                            if (data.Level >= MaxLevel)
                            {
                                lvlMaxBadge.Image.SetSprite(ModContent.GetSpriteReference<RPGMod>("MaxLvlBadge"));
                            }
                        }
                        else
                        {
                            var info = scroll.GetDescendent<ModHelperPanel>("Panel_RPGMod_XPData_" + data.BaseId);

                            var xpText = info.GetDescendent<ModHelperText>("Text_XP");
                            xpText.SetText($"{Math.Round(Player.UnitedXP[data.BaseId], 2)} United XP");

                            var levelText = info.GetDescendent<ModHelperText>("Text_Level");
                            int level = TowerXPData.CalculateLevel(Player.UnitedXP[data.BaseId]);
                            levelText.SetText($"Default Level: {level}");

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
                                levelText.Text.color = new(255, 162, 0);
                            }
                        }
                    }
                }
            }
        }

        private static ModHelperPanel CreateXPDataPanel(TowerXPData data)
        {
            ModHelperPanel panel = ModHelperPanel.Create(new("Panel_RPGMod_XPData_" + data.BaseId, 0, 0, 800, 600), VanillaSprites.BlueInsertPanel);

            var towerIcon = ModContent.GetSpriteReference<RPGMod>("MissingIcon");

            if (Game.instance.model.GetTowerFromId(data.BaseId) != null)
            {
                towerIcon = Game.instance.model.GetTowerFromId(data.BaseId).icon;
            }


            var icon = panel.AddImage(new("Icon", 0, 0, 400), towerIcon.GUID);

            var id = panel.AddText(new("Text_Id", 0, panel.RectTransform.rect.bottom - 80, 480, 150), data.BaseId.Spaced(), 60);

            if (!unitedXPMenu)
            {
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
                if (data.Level >= MaxLevel / 2)
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
            }
            else
            {
                var xpText = panel.AddText(new("Text_XP", 0, panel.RectTransform.rect.top + 160, 800, 150), $"{Math.Round(Player.UnitedXP[data.BaseId], 2)} United XP", 50);
                xpText.Text.color = new(1, 0.55f, 0);
                int level = TowerXPData.CalculateLevel(Player.UnitedXP[data.BaseId]);
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
            }

            return panel;
        }

        public void Close()
        {
            if (gameObject)
            {
                Destroy(gameObject);
                masteryMenuOpen = false;
                MasteryPanel = null;
            }
        }
    }
}
