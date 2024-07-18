using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.ChallengeEditor;
using Il2CppAssets.Scripts.Unity.UI_New.Settings;
using Il2CppNinjaKiwi.Common;
using RPGMod.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RPGMod.Ui.Screens
{
    internal class UniversalItemsScreen : ModGameMenu<SettingsScreen>
    {
        public static ModHelperPanel MainPanel;

        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            CommonForegroundHeader.SetText("RPG Profile Viewer");

            GameMenu.transform.DestroyAllChildren();

            MainPanel = GameMenu.gameObject.AddModHelperPanel(new Info("ProfileViewer", 3600, 1900));

            CreateMenu();
            return false;
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

        public void CreateMenu()
        {
            var panel = MainPanel.AddPanel(new("Panel", 0, -150, 3300, 1700), VanillaSprites.MainBGPanelBlue);

            var scroll = panel.AddScrollPanel(new("Scroll", 0, 0, 2600, 1000), UnityEngine.RectTransform.Axis.Horizontal, VanillaSprites.BlueInsertPanelRound, 50, 50);

            foreach (var item in Player.LoadItemsFromSave().FindAll(item_ => item_.Amount > 0 || item_.AlwaysShowInProfileViewer && !item_.Hidden))
            {
                scroll.AddScrollContent(ItemPanel(item));
            }
        }
    }
}
