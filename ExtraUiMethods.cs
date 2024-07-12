using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using RPGMod.Items;
using System.Collections.Generic;
using UnityEngine;
namespace RPGMod
{
    [RegisterTypeInIl2Cpp(false)]
    internal class ExtraUiMethods : MonoBehaviour
    {
        public static void Locked(string itemName, bool notImplemented = false)
        {
            if (PopupScreen.instance != null)
            {
                List<string> ItemNames = [];
                foreach (var item in ModContent.GetContent<ShopEntry>())
                {
                    ItemNames.Add(item.Item.ItemName);
                }

                if (notImplemented)
                {
                    PopupScreen.instance.ShowOkPopup("This feature is coming out later!");
                }
                else if (!ItemNames.Contains(itemName))
                {
                    PopupScreen.instance.ShowOkPopup("This item is coming to the shop later!");
                }
                else if (ItemNames.Contains(itemName))
                {
                    PopupScreen.instance.ShowOkPopup($"Buy the {itemName} item from the shop first!");
                }
            }
        }
    }
}
