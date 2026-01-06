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
    private static List<(RectTransform rect, Image shadow, float height, Vector2 originalPos)> activeButtons = new();
    public static Canvas CreateCanvas()
    {
        var canvasGO = new GameObject("ModCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
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

        activeButtons.Add((rectTransform, shadowImage, shadowHeight, pos));

        if (onClick != null)
        {
            button.onClick.AddListener(onClick);
        }

        return button;
    }

    private static Button currentPressedButton = null;
    private static bool effectLost = false;

    public static void UpdateButtonEffect(Button button)
    {
        var rect = button.GetComponent<RectTransform>();
        var shadow = rect.Find("Shadow")?.GetComponent<Image>();
        if (shadow == null) return;

        var (_, _, height, originalPos) = activeButtons.FirstOrDefault(b => b.rect == rect);
        Vector2 localMousePos = rect.InverseTransformPoint(Input.mousePosition);
        float shadowH = rect.sizeDelta.y * (1f / 7f);
        bool shadowHit = localMousePos.x >= -rect.sizeDelta.x / 2f &&
                         localMousePos.x <= rect.sizeDelta.x / 2f &&
                         localMousePos.y < -(rect.sizeDelta.y / 2f) &&
                         localMousePos.y > -(rect.sizeDelta.y / 2f + shadowH);
        bool mouseOver = rect.rect.Contains(localMousePos) || shadowHit;

        if (Input.GetMouseButtonDown(0) && mouseOver)
        {
            currentPressedButton = button;
            effectLost = false;
        }

        if (currentPressedButton == button && !mouseOver)
            effectLost = true;

        bool shouldPress = currentPressedButton == button && Input.GetMouseButton(0) && mouseOver && !effectLost;

        rect.anchoredPosition = shouldPress ? originalPos + new Vector2(0, -height) : originalPos;
        shadow.gameObject.SetActive(!shouldPress);

        if (!Input.GetMouseButton(0))
        {
            currentPressedButton = null;
            effectLost = false;
        }
    }

    public static Button CreateCloseButton(Canvas canvas, Vector2 pos, Vector2 size, System.Action onClick = null)
    {
        var buttonGO = new GameObject("CloseButton");
        buttonGO.transform.SetParent(canvas.transform, false);

        var rectTransform = buttonGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = pos;
        rectTransform.sizeDelta = size;

        var image = buttonGO.AddComponent<Image>();
        image.color = new Color(0f, 0.51f, 0.97f);

        var button = buttonGO.AddComponent<Button>();
        button.targetGraphic = image;

        var iconGO = new GameObject("Icon");
        iconGO.transform.SetParent(buttonGO.transform, false);
        var iconRect = iconGO.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(128*0.4f, 128*0.4f);

        var iconImage = iconGO.AddComponent<Image>();
        var iconTexture = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == "icon_x");
        if (iconTexture != null)
            iconImage.sprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), Vector2.zero);

        if (onClick != null)
            button.onClick.AddListener(onClick);

        return button;
    }

    public static Text CreateText(Canvas canvas, string text, Vector2 pos, int fontSize = 28, Font font = null, TextAnchor alignment = TextAnchor.MiddleCenter)
    {
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(canvas.transform, false);

        var rectTransform = textGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = pos;
        rectTransform.sizeDelta = new Vector2(500, 100);

        var textComp = textGO.AddComponent<Text>();
        textComp.text = text;
        textComp.font = font ?? Resources.FindObjectsOfTypeAll<Font>().FirstOrDefault(f => f.name == "OpenSans-Regular");
        textComp.fontSize = fontSize;
        textComp.color = Color.white;
        textComp.alignment = alignment;

        return textComp;
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

    public static Image CreateImage(Canvas canvas, Texture2D texture, Vector2 pos, Vector2 size)
    {
        var imageGO = new GameObject("Image");
        imageGO.transform.SetParent(canvas.transform, false);

        var rectTransform = imageGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = pos;
        rectTransform.sizeDelta = size;

        var image = imageGO.AddComponent<Image>();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        return image;
    }
}
