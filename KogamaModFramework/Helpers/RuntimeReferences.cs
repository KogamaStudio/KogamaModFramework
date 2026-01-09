using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppMV.Common;

namespace KogamaModFramework.Helpers;

internal static class RuntimeReferences
{
    //thanks kogamatools

    private static DesktopEditModeController _controller;
    private static EditorStateMachine _editorStateMachine;

    internal static DesktopEditModeController DesktopEditModeController
    {
        get
        {
            if (_controller == null && MVGameControllerBase.GameMode == MVGameMode.Edit)
                _controller = MVGameControllerBase.EditModeUI.Cast<DesktopEditModeController>();
            return _controller;
        }
    }

    internal static EditorStateMachine EditorStateMachine
    {
        get
        {
            if (_editorStateMachine == null)
                _editorStateMachine = DesktopEditModeController?.EditModeStateMachine;
            return _editorStateMachine;
        }
    }
}