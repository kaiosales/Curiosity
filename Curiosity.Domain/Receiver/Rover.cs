using System.Collections.ObjectModel;
using System.Drawing;

namespace Curiosity.Domain;

public class Rover: IReceiver, ICommandReceiver
{
    private Size plateau;
    private Direction direction;
    private Point position;

    private List<Telemetry> telemetry = new List<Telemetry>();

    public Rover()
    {
        this.direction = Direction.NORTH;
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
            Direction.NORTH => new Point(0, 1 * units),
            Direction.EAST => new Point(1 * units, 0),
            Direction.SOUTH => new Point(0, -1 * units),
            Direction.WEST => new Point(-1 * units, 0),
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
        int directionsCount = Enum.GetValues<Direction>().Count();
        int angle = (int)this.direction + (int)orientation;
        int remainder = angle % directionsCount;
        this.direction = (Direction)(remainder < 0 ? remainder + directionsCount : remainder);
    }

    public void Execute(ICommand[] buffer)
    {
        if (this.plateau.IsEmpty)
            throw new InvalidOperationException("Plateau has not been initialized yet");

        foreach(ICommand command in buffer)
        {
            command.Execute(this);
            this.telemetry.Add(new Telemetry(this.plateau, this.position, this.direction));
        }
    }

    public ReadOnlyCollection<Telemetry> State()
    {
        if (this.telemetry.Count == 0)
            this.telemetry.Add(new Telemetry(this.plateau, this.position, this.direction));

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