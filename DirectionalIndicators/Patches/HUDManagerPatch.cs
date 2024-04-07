using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace DirectionalIndicators.Patches;

[HarmonyPatch(typeof(HUDManager))]
public class HUDManagerPatch
{
    private static GameObject indicatorContainer;

    public static List<GameObject> indicatorGameObjects = new List<GameObject>();

    [HarmonyPostfix]
    [HarmonyPatch("Awake")]
    private static void Awake_Postfix(ref HUDManager __instance)
    {
        // Setup base indicatorGUI
        GameObject indicatorGUI = new GameObject("IndicatorOutline");
        RectTransform indicatorRectTransform = indicatorGUI.AddComponent<RectTransform>();
        indicatorRectTransform.SetParent(GameObject.Find(Constants.PlayerScreenGUIName).transform, false);
        indicatorRectTransform.sizeDelta = new Vector2(800, 800);
        CircleDrawer.DrawCenterCircle(indicatorRectTransform);

        // container for indicators
        indicatorContainer = new GameObject("IndicatorContainer");
        RectTransform containerTransform = indicatorContainer.AddComponent<RectTransform>();
        containerTransform.SetParent(GameObject.Find(Constants.PlayerScreenGUIName).transform, false);
        containerTransform.sizeDelta = new Vector2(800, 800);

        GameObject template = Constants.baseIndicator();
        for (int i = 0; i < Constants.DefaultIndicatorAmount; i++)
        {
            GameObject newObject = Object.Instantiate(template);
            RectTransform newTransform = newObject.GetComponent<RectTransform>();
            newTransform.SetParent(indicatorContainer.transform, false);
            newTransform.anchoredPosition = Constants.Offscreen;

            indicatorGameObjects.Add(newObject);
        }

        //test indicator from base
        GameObject testIndicator2 = indicatorGameObjects[0];
        RectTransform testTrans2 = testIndicator2.GetComponent<RectTransform>();
        testTrans2.SetParent(indicatorContainer.transform, false);
        testTrans2.anchoredPosition = TrigHelpers.GetVector(120f);
    }

    [HarmonyPostfix]
    [HarmonyPatch("Update")]
    private static void Update_Postfix()
    {
        for (int i = 0; i < Constants.DefaultIndicatorAmount; i++)
        {
            GameObject currentObject = indicatorGameObjects[i];
            RectTransform currentTransform = currentObject.GetComponent<RectTransform>();

            if (i < Plugin.Instance.indicators.Count)
            {
                DirectionalIndicator currentDI = Plugin.Instance.indicators[i];
                
                TextMeshProUGUI currentText = currentObject.GetComponent<TextMeshProUGUI>();
                currentText.text = currentDI.StringRepresentation;

                Transform playerTransform = GameNetworkManager.Instance.localPlayerController.transform;
                currentTransform.anchoredPosition = TrigHelpers.GetVector(TrigHelpers.CalculateAzimuthAngle(playerTransform.forward, currentDI.Pos - playerTransform.position));
            } else
            {
                currentTransform.anchoredPosition = Constants.Offscreen;
            }
        }

    }
}
