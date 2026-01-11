#nullable disable

using System.Numerics;
using System.Runtime.InteropServices;
using ClickableTransparentOverlay;
using Il2CppInterop.Runtime;
using ImGuiNET;
using MelonLoader;

namespace KogamaModFramework.UI.ImGuiUI;

public class KogamaModOverlay : Overlay
{
    private IntPtr _handle;
    private const int defaultWidth = 400;
    private const int defaultHeight = 500;
    private const ImGuiWindowFlags compatibilityFlags = ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize;

    private readonly string _windowName;
    private bool _windowHidden = false;

    public KogamaModOverlay(string windowName) : base(windowName)
    {
        _windowName = windowName;
    }

    protected override Task PostInitialized()
    {
        IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
        _handle = FindWindow(null, _windowName);
        IntPtr gameHandle = FindWindow(null, "KoGaMa");

        if (gameHandle != IntPtr.Zero)
        {
            SetParent(_handle, gameHandle);
            SetWindowLong(_handle, GWL_STYLE, WS_CHILD | WS_VISIBLE);
        }

        VSync = true;
        CompatibilityMode = true;
        return Task.CompletedTask;
    }

    protected override void Render()
    {
        if (!IsGameFocused())
        {
            if (CompatibilityMode && !_windowHidden)
            {
                ShowWindow(_handle, 0);
                _windowHidden = true;
            }
            return;
        }
        else
        {
            if (CompatibilityMode && _windowHidden)
            {
                ShowWindow(_handle, 4);
                _windowHidden = false;
            }
        }

        ImGui.SetNextWindowSize(new Vector2(defaultWidth, defaultHeight), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowPos(Vector2.Zero, ImGuiCond.FirstUseEver);

        ImGui.Begin(_windowName, CompatibilityMode ? compatibilityFlags : ImGuiWindowFlags.None);

        RenderContent();

        if (CompatibilityMode)
        {
            if (ImGui.IsWindowAppearing())
            {
                Vector2 windowSize = ImGui.GetWindowSize();
                Size = new((int)windowSize.X, (int)windowSize.Y);
                ImGui.SetWindowPos(Vector2.Zero);
            }
            ImGui.SetWindowSize(new Vector2(Size.Width, Size.Height));
        }

        ImGui.End();
    }

    protected virtual void RenderContent()
    {
        ImGuiElementManager.RenderAll();
    }

    public void ToggleVisibility()
    {
    }

    private bool IsGameFocused()
    {
        IntPtr foregroundWindow = GetForegroundWindow();
        return foregroundWindow == FindWindow(null, "KoGaMa") || foregroundWindow == _handle || foregroundWindow == IntPtr.Zero;
    }

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string className, string windowName);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private const int GWL_STYLE = -16;
    private const int WS_CHILD = 0x40000000;
    private const int WS_VISIBLE = 0x10000000;
}