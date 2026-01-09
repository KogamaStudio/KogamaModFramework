using HarmonyLib;
using Il2Cpp;
using KogamaModFramework.Helpers;
using KogamaModFramework.Operations;
using MelonLoader;
using UnityEngine.Events;

namespace KogamaModFramework.UI;

[HarmonyPatch]
public static class ContextMenuManager
{
    //thanks kogamatools

    private static readonly List<MenuItem> menuItems = new();

    public static void AddButton(string buttonName, Func<MVWorldObjectClient, bool> condition, Action<MVWorldObjectClient> action)
    {
        menuItems.Add(new MenuItem(condition, buttonName, action));
    }

    private static int lastMenuInstanceId = -1;

    [HarmonyPatch(typeof(ContextMenu), "AddButton")]
    [HarmonyPrefix]
    private static bool OnAddButton(string buttonText, UnityAction onClickCallback, ContextMenu __instance)
    {
        int currentMenuId = __instance.GetInstanceID();
        if (lastMenuInstanceId == currentMenuId) return true;

        lastMenuInstanceId = currentMenuId;

        MVWorldObjectClient wo = RuntimeReferences.DesktopEditModeController?.contextMenuController?.selectedWorldObject;
        if (wo == null) return true;

        foreach (var item in menuItems)
        {
            if (item.Condition(wo))
            {
                CreateButton(item.ButtonName, () => item.Action(wo), __instance);
            }
        }
        return false;
    }


    private static void CreateButton(string buttonText, Action onClickCallback, ContextMenu __instance)
    {
        var btn = UnityEngine.Object.Instantiate(__instance.contextMenuButtonPrefab);
        btn.Initialize(buttonText, onClickCallback + __instance.Pop);
        btn.transform.SetParent(__instance.transform, false);
    }

    private class MenuItem
    {
        public Func<MVWorldObjectClient, bool> Condition { get; }
        public string ButtonName { get; }
        public Action<MVWorldObjectClient> Action { get; }

        public MenuItem(Func<MVWorldObjectClient, bool> condition, string buttonName, Action<MVWorldObjectClient> action)
        {
            Condition = condition;
            ButtonName = buttonName;
            Action = action;
        }
    }
}