namespace Curiosity.Domain;


public static class CommandFactory
{
    public static Dictionary<char, ICommand> _definitions = new Dictionary<char, ICommand>();

    static CommandFactory() 
    {
        _definitions.Add('L', new TurnLeftCommand());
        _definitions.Add('R', new TurnRightCommand());
        _definitions.Add('F', new ForwardCommand());
    }

    public static ICommand Create(char commandText)
    {
        if (!_definitions.TryGetValue(commandText, out ICommand? command))
            throw new NotSupportedException($"The command '{commandText}' is not supported");

        return command;
    }
}
