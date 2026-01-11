namespace KogamaModFramework.UI.ImGuiUI;

public abstract class ImGuiElement
{
    public abstract string Name { get; }
    public abstract void Render();
}