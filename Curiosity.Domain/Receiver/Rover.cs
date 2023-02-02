using System.Collections.ObjectModel;
using System.Drawing;

namespace Curiosity.Domain;

public class Rover: IReceiver, ICommandReceiver
{
    private Size plateau;
    private IDirectionState direction;
    private Point position;

    private List<Telemetry> telemetry = new List<Telemetry>();

    public Rover()
    {
        this.direction = new NorthState();
        this.position = new Point(1, 1);
    }

    public void Init(Size plateau)
    {
        this.plateau = plateau;
    }

    public void Move(int units)
    {
        var delta = this.direction switch
        {
            NorthState => new Point(0, 1 * units),
            EastState => new Point(1 * units, 0),
            SouthState => new Point(0, -1 * units),
            WestState => new Point(-1 * units, 0),
            _ => new Point(0, 0)
        };

        var target = new Point(this.position.X, this.position.Y);
        target.Offset(delta);

        if (!ValidateBounderies(target))
            throw new ArgumentOutOfRangeException(nameof(units));

        this.position = target;
    }

    public void Turn(Orientation orientation)
    {
        this.direction = this.direction.Turn(orientation);
    }

    public void Execute(ICommand[] buffer)
    {
        if (this.plateau.IsEmpty)
            throw new InvalidOperationException("Plateau has not been initialized yet");

        foreach(ICommand command in buffer)
        {
            command.Execute(this);
            this.telemetry.Add(new Telemetry(this.plateau, this.position, this.direction.ToString()));
        }
    }

    public ReadOnlyCollection<Telemetry> State()
    {
        if (this.telemetry.Count == 0)
            this.telemetry.Add(new Telemetry(this.plateau, this.position, this.direction.ToString()));

        return this.telemetry.AsReadOnly();
    }

    private bool ValidateBounderies(Point target)
    {
        if (target.X < 1 || target.Y < 1)
            return false;

        if (target.X > this.plateau.Width || target.Y > this.plateau.Height)
            return false;

        return true;
    }
}