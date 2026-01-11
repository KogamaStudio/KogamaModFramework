using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KogamaModFramework.UI.ImGuiUI;

public static class GUIUtils
{
    public static string RemoveIdentifier(string label)
    {
        int idx = label.IndexOf("##");
        return idx == -1 ? label : label.Substring(0, idx);
    }

    public static float CalcButtonWidth(string label)
    {
        return (ImGui.CalcTextSize(RemoveIdentifier(label)) + ImGui.GetStyle().FramePadding * 2).X;
    }

    public static bool InputFloat(string label, ref float value)
    {
        ImGui.PushID(label);
        ImGui.Text(RemoveIdentifier(label));
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        bool result = ImGui.InputFloat(string.Empty, ref value);
        ImGui.PopID();
        return result;
    }
}