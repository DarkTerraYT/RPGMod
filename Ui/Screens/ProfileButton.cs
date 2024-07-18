using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.Main;
using UnityEngine;
using UnityEngine.UI;

namespace RPGMod.Ui.Screens
{
    internal static class ProfileButton
    {
        public static void Create(MainMenu menu)
        {
            var mainMenuTransform = menu.transform.Cast<RectTransform>();
            var trophyStore = mainMenuTransform.FindChild("Settings");
            var modsButton = trophyStore.Duplicate(mainMenuTransform);

            modsButton.name = "RPG_Profile";
            modsButton.transform.FindChild("Flag").gameObject.SetActive(false);
            modsButton.GetComponentInChildren<Image>().SetSprite(ModContent.GetSpriteReference<RPGMod>("ProfileButton"));
            modsButton.GetComponentInChildren<Button>().SetOnClick(() => { ModGameMenu.Open<ProfileScreen>(); MenuManager.instance.buttonClickSound.Play("ClickSounds"); });

            var matchLocalPosition = modsButton.transform.gameObject.AddComponent<MatchLocalPosition>();
            matchLocalPosition.transformToCopy = trophyStore.transform;
            matchLocalPosition.offset = new Vector3(325, 0);

            var text = ModHelperText.Create(new Info("RoundEditor", 100, 150), "RPG Profile");
            text.transform.GetComponent<NK_TextMeshProUGUI>().enableAutoSizing = true;
            text.transform.SetParent(modsButton.GetComponentInChildren<Button>().transform);

            text.transform.localPosition = new Vector3(150, -275, 0);
            text.transform.SetSiblingIndex(0);
        }

        [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Open))]
        private static class MainMenu_Open
        {
            [HarmonyPostfix]
            public static void Postfix(MainMenu __instance)
            {
                Create(__instance);
            }
        }
        [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.ReOpen))]
        private static class MainMenu_ReOpen
        {
            [HarmonyPostfix]
            public static void Postfix(MainMenu __instance)
            {
                Create(__instance);
            }
        }
    }
}
