global using static RPGMod.RPGMod;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using RPGMod.Items;
using System;
using System.Collections.Generic;
using UnityEngine;
using static RPGMod.Items.ModItem;

namespace RPGMod.Ui
{
    [RegisterTypeInIl2Cpp(true)]
    public class BagUI : MonoBehaviour
    {
        public void Close()
        {
            if (gameObject)
            {
                Destroy(gameObject);

                ItemButtons.Clear();
            }
        }

        public static BagUI? instance = null;

        public static ModHelperPanel? BagPanel = null;

        static List<ModHelperButton> ItemButtons = [];

        internal static ModHelperPanel? InfoPanel = null;

        internal string CurrentItem = "";

        Dictionary<ItemRarity, string> RartiyNames = new()
        {
            [ItemRarity.Basic] = "",
            [ItemRarity.Green] = "Green",
            [ItemRarity.Blue] = "Blue",
            [ItemRarity.Purple] = "Purple",
            [ItemRarity.Pink] = "Pink",
            [ItemRarity.Red] = "Red"
        };

        public static void CreateBagBtn()
        {

            var panel = UiRect.gameObject.AddModHelperPanel(new("Panel_RPGMod_Bag", UiRect.rect.right - 135, UiRect.rect.bottom - 525, 200), ModContent.GetSpriteReference<RPGMod>("empty-1x1").GUID);
            instance = panel.AddComponent<BagUI>();

            var btn = panel.AddButton(new("Btn_Shop", InfoPreset.FillParent), ModContent.GetTextureGUID<RPGMod>("BagBtn"), new Action(() =>
            {
                BagPanel = instance.CreateBagMenu(Game.instance, InGame.instance);
            }));
        }



        public ModHelperPanel CreateBagMenu(Game game, InGame inGame)
        {
            if (MasteryUI.instance != null)
            {
                MasteryUI.instance.Close();
            }
            if (QuestUI.instance != null)
            {
                QuestUI.instance.Close();
            }
            if (instance != null)
            {
                instance.Close();
            }
            if (ShopUI.instance != null)
            {
                ShopUI.instance.Close();
            }

            ModHelperPanel panel = UiRect.gameObject.AddModHelperPanel(new("Panel_RPGMod_Shop", 0, 0, 1900, 1400), VanillaSprites.BrownInsertPanel);
            ModHelperPanel titlePanel = panel.AddPanel(new("Panel_Title", 0, 725, 850, 200), VanillaSprites.BluePanelSmall);
            ModHelperText title = titlePanel.AddText(new("Text_Title", InfoPreset.FillParent), "Items", 100);

            instance = panel.AddComponent<BagUI>();

            var backBtn = panel.AddButton(new("Button_Back", panel.RectTransform.rect.right, panel.RectTransform.rect.top, 150), VanillaSprites.BackBtn, new Action(() =>
            {
                instance.Close();
                CreateBagBtn();
                if (QuestUI.instance == null)
                {
                    QuestUI.CreateQuestButton();
                }
                if (MasteryUI.instance == null)
                {
                    MasteryUI.CreateMasteryButton(currData.XPData);
                }
                if (ShopUI.instance == null)
                {
                    ShopUI.CreateShopBtn();
                }
            }));

            ModHelperScrollPanel scroll = panel.AddScrollPanel(new("Panel_RPGMod_Bag_Contents", 0, 0, 1600, 1200), RectTransform.Axis.Vertical, VanillaSprites.BrownInsertPanelDark, 30, 100);

            List<ModItem> items_ = ModContent.GetContent<ModItem>();
            List<ModItem> items = items_.Duplicate();

            foreach (var item in items_)
            {
                if (item.Amount < 1 | item.Hidden)
                {
                    items.Remove(item);
                }
            }

            int rows = items.Count / 8;

            if (!((double)items.Count).NoRemainder(8))
            {
                rows++;
            }

            int index = -1;

            for (int i = 0; i < rows; i++)
            {
                var panel_ = ModHelperPanel.Create(new("Panel_", 1600, 200), ModContent.GetTextureGUID<RPGMod>("empty-1x1"));

                for (int j = 0; j < 8; j++)
                {
                    int x = -600 + (j * 25) + (j * 150);

                    if ((index + 1) < items.Count)
                    {
                        index++;
                        var item = items[index];

                        var btn = panel_.AddButton(new("ItemButton", x, 0, 150), ModContent.GetTextureGUID<RPGMod>($"ItemBox{RartiyNames[item.Rarity]}"), new Action(() =>
                        {
                            if (InfoPanel != null)
                            {
                                InfoPanel.gameObject.Destroy();
                                InfoPanel = null;
                            }

                            InfoPanel = OpenItemPanel(item, panel, game, inGame);
                            CurrentItem = item.Name;
                        }));

                        btn.AddImage(new("Image_", InfoPreset.FillParent), item.Icon);

                        if (item.Amount > 1)
                        { var text_ = btn.AddText(new("Text_Count_" + item.Name, 0, 75, 100, 150), item.Amount.ToString()); }

                        ItemButtons.Add(btn);
                    }
                    else
                    {
                        panel_.AddImage(new("Image_Empty_ItemBox", x, 0, 150), ModContent.GetTextureGUID<RPGMod>("ItemBoxEmpty"));
                    }
                }

                scroll.AddScrollContent(panel_);
            }

            return panel;
        }

        private ModHelperPanel OpenItemPanel(ModItem item, ModHelperPanel bagPanel, Game game, InGame inGame)
        {
            bagPanel.RectTransform.position = new(1035, bagPanel.RectTransform.position.y, bagPanel.RectTransform.position.z);

            var panel = bagPanel.AddPanel(new("Panel_Info_" + item.Name, -1350, 0, 800, 1400), VanillaSprites.GreyInsertPanel);
            var closeButton = panel.AddButton(new("Button_Exit", 0, -700, 150), VanillaSprites.RedBtnSquare, new Action(() =>
            {
                bagPanel.RectTransform.position = new(835, bagPanel.RectTransform.position.y, bagPanel.RectTransform.position.z);
                panel.gameObject.Destroy();

                InfoPanel = null;
                CurrentItem = "";
            }));
            var x = closeButton.AddText(new("X", InfoPreset.FillParent), "X", 80);
            x.Text.outlineColor = new Color(0.5f, 0, 0);
            x.Text.color = new(0.85f, 0, 0);

            var name = panel.AddText(new("Text_ItemName", 0, 600, 700, 300), item.ItemName, 100);
            name.Text.fontSizeMax = 100;
            name.Text.enableAutoSizing = true;

            var icon = panel.AddImage(new("Icon_Item", 0, 300, 500), item.Icon);

            var description = panel.AddText(new("Text_Desc", 0, 0, 700, 600), item.Description, 60);
            description.Text.fontSizeMax = 60;
            description.Text.enableAutoSizing = true;

            if (item.Type == ItemType.Consumable)
            {
                int amount = 1;

                var amountText = panel.AddText(new("Text_Amount", 0, -250, 350, 100), amount.ToString());
                amountText.Text.enableAutoSizing = true;

                var useButton = panel.AddButton(new("Button_Use", 0, -400, 500, 200), VanillaSprites.GreenBtnLong, new Action(() =>
                {
                    item.OnItemUse(game, inGame, amount);
                    item.ChangeAmount(-amount);
                    ModHelperButton? btn = ItemButtons.Find(button => button.name == "Button_Item_Menu_" + item.Name);
                    if (btn != null)
                    {
                        if (btn.GetDescendent<ModHelperText>("Text_Count_" + item.Name) != null)
                        {
                            btn.GetDescendent<ModHelperText>("Text_Count_" + item.Name).Text.SetText(item.Amount.ToString());
                        }
                        if (item.Amount == 0)
                        {
                            if (instance != null)
                                instance.CreateBagMenu(game, inGame);
                        }
                    }

                    if (amount > item.Amount)
                    {
                        amount = item.Amount;
                        amountText.SetText(amount.ToString());
                    }
                }));
                var text = useButton.AddText(new("Text_Use", InfoPreset.FillParent), "Use Item", 80);


                var amountButton = panel.AddButton(new("Button_Amount", -250, -300, 100), VanillaSprites.AddMoreBtn, new Action(() =>
                {
                    PopupScreen.instance?.ShowSetValuePopup("Set Amount", "How many would you like to use?", new Action<int>(input =>
                    {
                        if (input > item.Amount)
                        {
                            amount = item.Amount;
                        }
                        else 
                        {
                            amount = input;
                        }

                        amountText.SetText(amount.ToString());
                    }), 1);
                }));
            }
            else if (item.Type == ItemType.Equipment)
            {
                var useButton = panel.AddButton(new("Button_Use", 0, -400, 500, 200), VanillaSprites.GreenBtnLong, new Action(() => { item.OnItemUse(game, inGame, 1); item.ChangeAmount(-1); if (item.Amount < 1) { instance?.CreateBagMenu(game, inGame); } }));
                var text = useButton.AddText(new("Text_Use", InfoPreset.FillParent), "Use Item", 80);
            }

            return panel;
        }
    }
}
