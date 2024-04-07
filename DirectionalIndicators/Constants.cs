using TMPro;
using UnityEngine;

namespace DirectionalIndicators;

public class Constants
{
    public static readonly int DefaultIndicatorAmount = 10;
    public static readonly int DefaultIndicatorExpireTimeMs = 3 * 1000;
    public static readonly int DefaultIndicatorDebounceTimeMs = 2 * 1000;
    public static readonly int DefaultVisibleSubtitleLines = 3;

    public static readonly Vector2 Offscreen = new Vector2(999, 999);

    // Do not edit
    public static readonly string HtmlLineBreakTag = "<br>";
    public static readonly string PlayerScreenGUIName = "Systems/UI/Canvas/Panel/GameObject/PlayerScreen";

    public static GameObject baseIndicator()
    {
        GameObject indicator = new GameObject("test");
        indicator.AddComponent<RectTransform>();
        TextMeshProUGUI testTextComponent = indicator.AddComponent<TextMeshProUGUI>();

        RectTransform testTransform = testTextComponent.rectTransform;
        testTransform.sizeDelta = new Vector2(100, 100);
        testTransform.anchoredPosition = new Vector2(0,0);

        testTextComponent.alignment = TextAlignmentOptions.Center;
        testTextComponent.fontSize = 14f;
        testTextComponent.text = "!";

        return indicator;
    }
}