using System;
using UnityEngine;

namespace KogamaModFramework.UI.ImGuiUI;

public static class OverlayManager
{
    private static KogamaModOverlay _overlay;
    private static bool _initialized = false;
    private static bool _shutdownHookRegistered = false;

    public static void Initialize(KogamaModOverlay overlay)
    {
        if (!_shutdownHookRegistered)
        {
            Application.quitting += (Action)OnApplicationQuit;
            _shutdownHookRegistered = true;
        }

        if (_initialized)
        {
            throw new InvalidOperationException("OverlayManager is already initialized");
        }

        _overlay = overlay;
        Task.Factory.StartNew(() => _overlay.Start());
        _initialized = true;
    }

    private static void OnApplicationQuit()
    {
        if (_initialized && _overlay != null)
        {
            try
            {
                _overlay.Close();
            }
            catch { }
        }
        _initialized = false;
    }

    public static void ToggleVisibility()
    {
        if (!_initialized)
            throw new InvalidOperationException("OverlayManager not initialized. Call Initialize first.");

        _overlay.ToggleVisibility();
    }

    public static void Shutdown()
    {
        if (_initialized && _overlay != null)
        {
            try
            {
                _overlay.Close();
            }
            catch { }
        }
        _initialized = false;
    }

    public static KogamaModOverlay GetOverlay()
    {
        if (!_initialized)
            throw new InvalidOperationException("OverlayManager not initialized");

        return _overlay;
    }

    public static bool IsInitialized => _initialized;
}