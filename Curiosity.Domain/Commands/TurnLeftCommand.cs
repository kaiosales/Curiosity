namespace Curiosity.Domain;

public class TurnLeftCommand: ICommand
{
    public void Execute(ICommandReceiver receiver)
    {
        receiver.Turn(Orientation.Counterclockwise);
    }

    public override string ToString()
    {
        return "L";
    }
}
