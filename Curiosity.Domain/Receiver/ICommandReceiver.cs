using System.Drawing;

namespace Curiosity.Domain;

public interface ICommandReceiver : IReceiver
{
    void Move(int units);
    void Turn(Orientation orientation);
}
