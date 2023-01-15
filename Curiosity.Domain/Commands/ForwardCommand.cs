namespace Curiosity.Domain;

public class ForwardCommand: ICommand
{
    public void Execute(ICommandReceiver receiver)
    {
        receiver.Move(1);
    }

    public override string ToString()
    {
        return "F";
    }
}
