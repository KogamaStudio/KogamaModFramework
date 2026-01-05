using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using Il2Cpp;
using MelonLoader;

namespace KogamaModFramework.UI;

public static class CanvasUI
{
    public static Canvas CreateCanvas()
    {
        var canvasGO = new GameObject("ModCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<GraphicRaycaster>();
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        return canvas;
    }

    public static Button CreateButton(Canvas canvas, string text, Vector2 pos, Vector2 size, Font font = null, int fontSize = 28, System.Action onClick = null)
    {
        var buttonGO = new GameObject("Button");
        buttonGO.transform.SetParent(canvas.transform, false);

        var rectTransform = buttonGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = pos;
        rectTransform.sizeDelta = size;

        var image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.31f, 0.31f, 0.31f);

        //shadow
        float shadowHeight = size.y * (1f / 7f);

        var shadowGO = new GameObject("Shadow");
        shadowGO.transform.SetParent(buttonGO.transform, false);

        var shadowRect = shadowGO.AddComponent<RectTransform>();
        shadowRect.anchoredPosition = new Vector2(0, -(size.y / 2f + shadowHeight / 2f));
        shadowRect.sizeDelta = new Vector2(size.x, shadowHeight);

        var shadowImage = shadowGO.AddComponent<Image>();
        shadowImage.color = new Color(0.23f, 0.23f, 0.23f);
        //end of shadow
        
        var button = buttonGO.AddComponent<Button>();
        button.targetGraphic = image;

        //colors
        var colors = button.colors;
        colors.highlightedColor = new Color(1.05f, 1.05f, 1.05f);
        colors.pressedColor = new Color(1.05f, 1.05f, 1.05f);
        button.colors = colors;
        //end of colors

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);

        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var textComp = textGO.AddComponent<Text>();
        textComp.text = text;
        textComp.font = font ?? Resources.FindObjectsOfTypeAll<Font>().FirstOrDefault(f => f.name == "OpenSans-Regular");
        textComp.fontSize = fontSize;
        textComp.color = Color.white;
        textComp.alignment = TextAnchor.MiddleCenter;

        if (onClick != null)
        {
            button.onClick.AddListener(onClick);
        }

        return button;
    }

    public static Image CreatePanel(Canvas canvas, Vector2 pos, Vector2 size, Color? color = null)
    {
        var panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvas.transform, false);
        var rectTransform = panelGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = pos;
        rectTransform.sizeDelta = size;
        var image = panelGO.AddComponent<Image>();
        image.color = color ?? new Color(0.19f, 0.23f, 0.27f);
        return image;
    }
}
