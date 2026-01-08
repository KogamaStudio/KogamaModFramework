using HarmonyLib;
using Il2Cpp;

namespace KogamaModFramework.Commands;

[HarmonyPatch]
internal class CommandInterceptor
{
    [HarmonyPatch(typeof(SendMessageControl), "HandleChatCommands")]
    [HarmonyPrefix]
    private static bool HandleChat(string chatMsg)
    {
        if (!CommandManager.Enabled) return true;
        if (chatMsg.StartsWith("/"))
        {
            string[] parts = chatMsg[1..].Split(' ');
            if (CommandManager.HasCommand(parts[0]))
            {
                CommandManager.Execute(parts[0], parts[1..]);
                return false;
            }
        }
        return true;
    }
}

