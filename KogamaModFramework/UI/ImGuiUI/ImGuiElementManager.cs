using System.Collections.Generic;

namespace KogamaModFramework.UI.ImGuiUI;

public static class ImGuiElementManager
{
    private static List<ImGuiElement> elements = new();

    public static void Register(ImGuiElement element)
    {
        elements.Add(element);
    }

    public static void RenderAll()
    {
        foreach (var element in elements)
        {
            element.Render();
        }
    }
}