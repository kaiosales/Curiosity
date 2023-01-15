using System.Collections.ObjectModel;
using System.Drawing;

namespace Curiosity.Domain;

public interface IReceiver
{
    void Init(Size plateau);
    void Execute(ICommand[] buffer);
    ReadOnlyCollection<Telemetry> State();
}
