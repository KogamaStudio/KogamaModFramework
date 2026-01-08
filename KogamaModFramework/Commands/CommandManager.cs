
namespace KogamaModFramework.Commands;

public static class CommandManager
{
    private static Dictionary<string, Command> commands = new();
    public static bool Enabled = false;
    public static void Initialize()
    {
        State.InitializeAllPatches();
        Enabled = true;
    }
    public static void Register(Command cmd)
    {
        commands[cmd.Name.ToLower()] = cmd;
    }

    public static void Execute(string cmdName, string[] args)
    {
        if (commands.TryGetValue(cmdName.ToLower(), out var cmd)) 
            cmd.Execute(args);
    }

    public static bool HasCommand(string cmdName)
    {
        return commands.ContainsKey(cmdName.ToLower());
    }
}

