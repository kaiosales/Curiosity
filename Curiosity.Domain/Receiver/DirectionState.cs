namespace Curiosity.Domain;

public interface IDirectionState
{
    IDirectionState Turn(Orientation orientation);
    string ToString();
}

public class NorthState : IDirectionState
{
    public IDirectionState Turn(Orientation orientation) => orientation switch
    {
        Orientation.Clockwise => new EastState(),
        Orientation.Counterclockwise => new WestState(),
        _ => this
    };

    public override string ToString()
    {
        return "North";
    }
}

public class EastState : IDirectionState
{
    public IDirectionState Turn(Orientation orientation) => orientation switch
    {
        Orientation.Clockwise => new SouthState(),
        Orientation.Counterclockwise => new NorthState(),
        _ => this
    };

    public override string ToString()
    {
        return "East";
    }
}

public class SouthState : IDirectionState
{
    public IDirectionState Turn(Orientation orientation) => orientation switch
    {
        Orientation.Clockwise => new WestState(),
        Orientation.Counterclockwise => new EastState(),
        _ => this
    };

    public override string ToString()
    {
        return "South";
    }
}

public class WestState : IDirectionState
{
    public IDirectionState Turn(Orientation orientation) => orientation switch
    {
        Orientation.Clockwise => new NorthState(),
        Orientation.Counterclockwise => new SouthState(),
        _ => this
    };

    public override string ToString()
    {
        return "West";
    }
}