namespace Curiosity.Domain;

public class TurnRightCommand: ICommand
{
    public void Execute(ICommandReceiver receiver)
    {
        receiver.Turn(Orientation.Clockwise);
    }

    public override string ToString()
    {
        return "R";
    }
}
