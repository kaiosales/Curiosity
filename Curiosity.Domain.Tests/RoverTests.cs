using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Moq;

namespace Curiosity.Domain.Tests;

[ExcludeFromCodeCoverage]
public class HoverTest
{
    [Fact]
    public void Should_instantiate()
    {
        var sut = new Rover();

        Assert.NotNull(sut);
    }

    [Fact]
    public void Should_have_correct_initial_state()
    {
        var expected = new Telemetry[] { new Telemetry(new Size(1, 1), new Point(1, 1), Direction.NORTH) };

        var sut = new Rover();
        sut.Init(new Size(1, 1));
        var actual = sut.State();

        Assert.True(Enumerable.SequenceEqual(expected, actual));
    }

    [Theory]
    [InlineData(1, Direction.EAST)]
    [InlineData(2, Direction.SOUTH)]
    [InlineData(3, Direction.WEST)]
    [InlineData(4, Direction.NORTH)]
    public void Should_face_after_turn_clockwise(int turns, Direction direction)
    {
        var expected = new Telemetry[] { new Telemetry(new Size(1, 1), new Point(1, 1), direction) };

        var sut = new Rover();
        sut.Init(new Size(1, 1));

        for (var i = 0; i < turns; i++)
        {
            sut.Turn(Orientation.Clockwise);
        }

        var actual = sut.State();

        Assert.True(Enumerable.SequenceEqual(expected, actual));
    }

    [Theory]
    [InlineData(1, Direction.WEST)]
    [InlineData(2, Direction.SOUTH)]
    [InlineData(3, Direction.EAST)]
    [InlineData(4, Direction.NORTH)]
    public void Should_face_after_turn_Counterclockwise(int turns, Direction direction)
    {
        var expected = new Telemetry[] { new Telemetry(new Size(1, 1), new Point(1, 1), direction) };

        var sut = new Rover();
        sut.Init(new Size(1, 1));

        for (var i = 0; i < turns; i++)
        {
            sut.Turn(Orientation.Counterclockwise);
        }

        var actual = sut.State();

        Assert.True(Enumerable.SequenceEqual(expected, actual));
    }

    [Theory]
    [InlineData(new [] { Orientation.Clockwise, Orientation.Counterclockwise, Orientation.Clockwise }, Direction.EAST)]
    public void Should_face_after_turn(Orientation[] orientations, Direction direction)
    {
        var expected = new Telemetry[] { new Telemetry(new Size(1, 1), new Point(1, 1), direction) };

        var sut = new Rover();
        sut.Init(new Size(1, 1));

        foreach(var orientation in orientations)
        {
            sut.Turn(orientation);
        }

        var actual = sut.State();

        Assert.True(Enumerable.SequenceEqual(expected, actual));
    }

    [Fact]
    public void Should_throw_ArgumentOutOfRangeException_if_move_out_of_bounds_to_west()
    {
        var expected = new Telemetry[] { new Telemetry(new Size(1, 1), new Point(1, 1), Direction.WEST) };

        var sut = new Rover();
        sut.Init(new Size(1, 1));
        sut.Turn(Orientation.Counterclockwise);

        Assert.Throws<ArgumentOutOfRangeException>("units", () => sut.Move(1));

        var actual = sut.State();

        Assert.True(Enumerable.SequenceEqual(expected, actual));
    }

    [Fact]
    public void Should_throw_ArgumentOutOfRangeException_if_move_out_of_bounds_to_south()
    {
        var expected = new Telemetry[] { new Telemetry(new Size(1, 1), new Point(1, 1), Direction.SOUTH) };

        var sut = new Rover();
        sut.Init(new Size(1, 1));
        sut.Turn(Orientation.Clockwise);
        sut.Turn(Orientation.Clockwise);

        Assert.Throws<ArgumentOutOfRangeException>("units", () => sut.Move(1));
        
        var actual = sut.State();

        Assert.True(Enumerable.SequenceEqual(expected, actual));
    }

    [Fact]
    public void Should_throw_ArgumentOutOfRangeException_if_move_out_of_bounds_to_east()
    {
        var expected = new Telemetry[] { new Telemetry(new Size(1, 1), new Point(1, 1), Direction.EAST) };

        var sut = new Rover();
        sut.Init(new Size(1, 1));
        sut.Turn(Orientation.Clockwise);

        Assert.Throws<ArgumentOutOfRangeException>("units", () => sut.Move(1));
        
        var actual = sut.State();

        Assert.True(Enumerable.SequenceEqual(expected, actual));
    }

    [Fact]
    public void Should_throw_ArgumentOutOfRangeException_if_move_out_of_bounds_to_north()
    {
        var expected = new Telemetry[] { new Telemetry(new Size(1, 1), new Point(1, 1), Direction.NORTH) };

        var sut = new Rover();
        sut.Init(new Size(1, 1));

        Assert.Throws<ArgumentOutOfRangeException>("units", () => sut.Move(1));
        
        var actual = sut.State();

        Assert.True(Enumerable.SequenceEqual(expected, actual));
    }

    [Fact]
    public void Should_throw_InvalidOperationException_if_execute_commands_without_init_plateau()
    {
        var sut = new Rover();

        Assert.Throws<InvalidOperationException>(() => sut.Execute(new ICommand[] { new Mock<ICommand>().Object }));
    }
}