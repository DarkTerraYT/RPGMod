using BTD_Mod_Helper;
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
using System.Linq;
using UnityEngine;
using static RPGMod.Items.ModItem;

namespace RPGMod.Ui
{
    [RegisterTypeInIl2Cpp(false)]
    public class ShopUI : MonoBehaviour
    {
        static ModHelperPanel? BuyPanel;
        
        public static ShopUI? instance;

        static List<ModHelperButton> ItemButtons = [];

        public static void CreateShopBtn()
        {
            var panel = UiRect.gameObject.AddModHelperPanel(new("Button_Panel_RPGMod_Shop", UiRect.rect.right - 135, UiRect.rect.bottom - 300, 200), ModContent.GetSpriteReference<RPGMod>("empty-1x1").GUID);
            instance = panel.AddComponent<ShopUI>();

            var btn = panel.AddButton(new("Btn_Shop", InfoPreset.FillParent), ModContent.GetTextureGUID<RPGMod>("ShopBtn"), new Action(() =>
            {
                instance.CreateShopMenu();
            }));
        }

        Dictionary<ItemRarity, string> RartiyNames = new()
        {
            [ItemRarity.Basic] = "",
            [ItemRarity.Green] = "Green",
            [ItemRarity.Blue] = "Blue",
            [ItemRarity.Purple] = "Purple",
            [ItemRarity.Pink] = "Pink",
            [ItemRarity.Red] = "Red"
        };

        public ModHelperPanel CreateShopMenu()
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
            if (BagUI.instance != null)
            {
                BagUI.instance.Close();
            }

            ModHelperPanel panel = UiRect.gameObject.AddModHelperPanel(new("Panel_RPGMod_Shop", 0, 0, 1900, 1400), VanillaSprites.BrownInsertPanel);
            ModHelperPanel titlePanel = panel.AddPanel(new("Panel_Title", 0, 725, 850, 200), VanillaSprites.BluePanelSmall);
            ModHelperText title = titlePanel.AddText(new("Text_Title", InfoPreset.FillParent), "Ye' Old Shoppe", 100);

            instance = panel.AddComponent<ShopUI>();

            var backBtn = panel.AddButton(new("Button_Back", panel.RectTransform.rect.right, panel.RectTransform.rect.top, 150), VanillaSprites.BackBtn, new Action(() =>
            {
                instance.Close();
                CreateShopBtn();
                if (QuestUI.instance == null)
                {
                    QuestUI.CreateQuestButton();
                }
                if (MasteryUI.instance == null)
                {
                    MasteryUI.CreateMasteryButton(currData.XPData);
                }
                if (BagUI.instance == null)
                {
                    BagUI.CreateBagBtn();
                }
            }));

            ModHelperScrollPanel scroll = panel.AddScrollPanel(new("Panel_RPGMod_Shop_Contents", 0, 0, 1600, 1200), RectTransform.Axis.Vertical, VanillaSprites.BrownInsertPanelDark, 30, 100);

            List<ShopEntry> items_ = ModContent.GetContent<ShopEntry>().FindAll(entry => (entry.Item.Amount < entry.Item.Max || entry.Item.Max == -1) && entry.Stock != -1 && entry.Stock > 0 && entry.ShowInShop);
            List<ShopEntry> items = [];

            foreach(var entry in items_)
            {
                if (entry.RequiredItems.All(item => item.Amount > 0) || entry.RequiredItems.Count == 0)
                {
                    items.Add(entry);
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

                        if (InGame.instance.GetCash() < item.Cost)
                        {
                            var btn = panel_.AddButton(new("Item_Button", x, 0, 150), ModContent.GetTextureGUID<RPGMod>("ItemBox" + RartiyNames[item.Item.Rarity] + "Dark"), new Action(() =>
                            {
                                BuyPanel = instance.OpenBuyPanel(item, panel);
                            }));

                            var ico = btn.AddImage(new("Item_Icon" + item, InfoPreset.FillParent), item.Icon);

                            var text = panel_.AddText(new("Text_Price_" + item.Name, x, -75, 100, 150), item.Cost.ToString());
                            text.Text.color = new(255, 0, 0);
                        }
                        else
                        {
                            var ico = panel_.AddButton(new("Image_", x, 0, 150), ModContent.GetTextureGUID<RPGMod>("ItemBox" + RartiyNames[item.Item.Rarity]), new Action(() =>
                            {
                                BuyPanel = instance.OpenBuyPanel(item, panel);
                            }));

                            var btn = ico.AddImage(new("Button_Buy_" + item, InfoPreset.FillParent), item.Icon);

                            var text = panel_.AddText(new("Text_Price_" + item.Name, x, -75, 100, 150), item.Cost.ToString());
                            text.Text.color = new(0, 255, 0);
                        }

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


        public void Close()
        {
            if (gameObject)
            {
                Destroy(gameObject);
            }
        }

        private ModHelperPanel OpenBuyPanel(ShopEntry item, ModHelperPanel shopPanel)
        {
            if(BuyPanel != null)
            {
                Destroy(BuyPanel.gameObject);
                BuyPanel = null;
            }

            shopPanel.RectTransform.position = new(1035, shopPanel.RectTransform.position.y, shopPanel.RectTransform.position.z);

            var panel = shopPanel.AddPanel(new("Panel_Info_" + item.Name, -1350, 0, 800, 1400), VanillaSprites.GreyInsertPanel);
            var closeButton = panel.AddButton(new("Button_Exit", 0, -700, 150), VanillaSprites.RedBtnSquare, new Action(() =>
            {
                shopPanel.RectTransform.position = new(835, shopPanel.RectTransform.position.y, shopPanel.RectTransform.position.z);
                panel.gameObject.Destroy();

                BuyPanel = null;
            }));
            var x = closeButton.AddText(new("X", InfoPreset.FillParent), "X", 80);
            x.Text.outlineColor = new Color(0.5f, 0, 0);
            x.Text.color = new(0.85f, 0, 0);

            var icon = panel.AddImage(new("Icon_Item", 0, 300, 500), item.Icon);

            var name = panel.AddText(new("Text_ItemName", 0, 600, 700, 150), item.Item.ItemName, 100);
            name.Text.fontSizeMax = 100;
            name.Text.enableAutoSizing = true;

            if(item.Item.Universal)
            {
                var universalTextIcon = panel.AddImage(new("Icon_Universal", 0, 450, 700, 88.2278800101f /* Perfect ratio! (7.934:1) */), ModContent.GetTextureGUID<RPGMod>("UniversalText"));
            }

            var description = panel.AddText(new("Text_Desc", 0, 0, 700, 600), item.Description, 60);
            description.Text.fontSizeMax = 60;
            description.Text.enableAutoSizing = true;

            int amount = 1;

            if (item.Item.Max > 1 | item.Stock > 1)
            {
                var amountText = panel.AddText(new("Text_Amount", 0, -250, 350, 100), amount.ToString());
                amountText.Text.enableAutoSizing = true;

                var buyButton = panel.AddButton(new("Button_Buy", 0, -400, 500, 200), VanillaSprites.GreenBtnLong, new Action(() =>
                {
                    if (amount > item.Item.Max - item.Item.Amount && item.Item.Max != -1)
                    {
                        amount = item.Item.Max - item.Item.Amount;
                    }
                    amountText.SetText(amount.ToString());

                    item.Buy(Game.instance, InGame.instance, amount);

                    if (BuyPanel != null)
                    {
                        Destroy(BuyPanel.gameObject);
                        BuyPanel = null;
                    }
                    if(item.Item.Max <= item.Item.Amount)
                    {
                        instance?.CreateShopMenu();
                    }
                    else if(item.Stock < 1 && item.Stock != -1)
                    {
                        instance?.CreateShopMenu();
                    }
                    else if(item.Stock == 1 && item.Stock != -1)
                    {
                        instance?.OpenBuyPanel(item, shopPanel);
                    }
                    else if(item.Item.Amount == item.Item.Max - 1)
                    {
                        instance?.OpenBuyPanel(item, shopPanel);
                    }

                }));
                var text = buyButton.AddText(new("Text_Buy", InfoPreset.FillParent), "Buy Item(s)", 80);

                int _ = item.Cost;

                for(int i = 1; i < amount; i++)
                {
                    _ = (int)(item.Cost * item.CostMultiplier);
                }

                var totalCost = panel.AddText(new("Text_Cost", 0, -550, 500, 200), "$" + _.ToString());

                totalCost.Text.enableAutoSizing = true;
                totalCost.Text.color = Color.green;
                totalCost.Text.outlineColor = new Color(0, 0.3f, 0);

                var amountButton = panel.AddButton(new("Button_Amount", -250, -300, 100), VanillaSprites.AddMoreBtn, new Action(() =>
                {
                    PopupScreen.instance?.ShowSetValuePopup("Set Amount", "How many would you like to buy?", new Action<int>(input =>
                    {
                        int totalPrice = item.Cost;

                        for(int i = 1; i < input; i++)
                        {
                            totalPrice = (int)(totalPrice * item.CostMultiplier);
                        }

                        if (totalPrice > InGame.instance.bridge.GetCash())
                        {
                            totalPrice = item.Cost;
                            while(totalPrice < InGame.instance.bridge.GetCash())
                            {
                                totalPrice = (int)(totalPrice * item.CostMultiplier);
                                amount++;
                            }
                        }
                        if (input > item.Item.Max - item.Item.Amount && item.Item.Max != -1)
                        {
                            amount = item.Item.Max - item.Item.Amount;
                        }
                        else
                        {
                            amount = input;
                        }

                        amountText.SetText(amount.ToString());
                        totalCost.SetText("$" + totalPrice.ToString());
                    }), 1);
                }));
            }
            else
            {
                var buyButton = panel.AddButton(new("Button_Buy", 0, -400, 500, 200), VanillaSprites.GreenBtnLong, new Action(() =>
                {
                    item.Buy(Game.instance, InGame.instance);
                    instance?.CreateShopMenu();
                }));
                var text = buyButton.AddText(new("Text_Buy", InfoPreset.FillParent), "Buy Item", 80);

                var totalCost = panel.AddText(new("Text_Cost", 0, -550, 500, 200), "$" + (item.Cost).ToString());

                totalCost.Text.enableAutoSizing = true;
                totalCost.Text.color = Color.green;
                totalCost.Text.outlineColor = new Color(0, 0.3f, 0);
            }

            return panel;
        }
    }


}
