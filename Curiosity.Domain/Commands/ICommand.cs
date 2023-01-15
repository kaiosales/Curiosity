namespace Curiosity.Domain;

public interface ICommand
{
    void Execute(ICommandReceiver receiver);
}
