using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Moq;

namespace Curiosity.Domain.Tests;

[ExcludeFromCodeCoverage]
public class TransmitterTest
{
    [Fact]
    public void Should_instantiate()
    {
        var receiver = new Mock<IReceiver>();

        var sut = new Transmitter(receiver.Object);

        Assert.NotNull(sut);
    }

    [Fact]
    public void Should_throw_ArgumentNullException_if_plateau_is_null()
    {
        var receiver = new Mock<IReceiver>(); 
        var sut = new Transmitter(receiver.Object);

        Assert.Throws<ArgumentNullException>("plateau", () => sut.Init(null));
    }

    [Fact]
    public void Should_throw_ArgumentException_if_plateau_is_empty_string()
    {
        var receiver = new Mock<IReceiver>(); 
        var sut = new Transmitter(receiver.Object);

        Assert.Throws<ArgumentException>("plateau", () => sut.Init(""));
    }

    [Fact]
    public void Should_throw_FormatException_if_plateau_format_dont_match()
    {
        var receiver = new Mock<IReceiver>(); 
        var sut = new Transmitter(receiver.Object);

        Assert.Throws<FormatException>(() => sut.Init("abc"));
    }

    [Fact]
    public void Should_throw_ArgumentException_if_plateau_width_is_less_than_1()
    {
        var receiver = new Mock<IReceiver>(); 
        var sut = new Transmitter(receiver.Object);

        Assert.Throws<ArgumentException>("plateau", () => sut.Init("0x1"));
    }

    [Fact]
    public void Should_throw_ArgumentException_if_plateau_height_is_less_than_1()
    {
        var receiver = new Mock<IReceiver>(); 
        var sut = new Transmitter(receiver.Object);

        Assert.Throws<ArgumentException>("plateau", () => sut.Init("1x0"));
    }

    public static IEnumerable<object[]> PlateauData(){
        yield return new object[] { "1x1", new Size(1, 1) };
        yield return new object[] { "5x5", new Size(5, 5) };
        yield return new object[] { "10x10", new Size(10, 10) };
        yield return new object[] { "3x7", new Size(3, 7) };
        yield return new object[] { "6x2", new Size(6, 2) };
        yield return new object[] { "30x100", new Size(30, 100) };
    }

    [Theory]
    [MemberData(nameof(PlateauData))]
    public void Should_init_receiver_with_size(string plateau, Size expected)
    {
        var receiver = new Mock<IReceiver>(); 
        receiver.Setup(m => m.Init(It.IsAny<Size>()));

        var sut = new Transmitter(receiver.Object);

        sut.Init(plateau);

        receiver.Verify(m => m.Init(It.Is<Size>(actual => expected == actual)), Times.Once);
    }

    [Fact]
    public void Should_throw_ArgumentNullException_if_input_is_null()
    {
        var receiver = new Mock<IReceiver>(); 
        var sut = new Transmitter(receiver.Object);

        Assert.Throws<ArgumentNullException>("input", () => sut.Send(null));
    }

    [Fact]
    public void Should_throw_ArgumentException_if_input_is_empty_string()
    {
        var receiver = new Mock<IReceiver>(); 
        var sut = new Transmitter(receiver.Object);

        Assert.Throws<ArgumentException>("input", () => sut.Send(""));
    }

    [Fact]
    public void Should_throw_ArgumentOutOfRangeException_if_input_lenght_is_bigger_than_10()
    {
        var capacity = 101;
        var receiver = new Mock<IReceiver>(); 
        var sut = new Transmitter(receiver.Object);

        Assert.Throws<ArgumentOutOfRangeException>("input", () => 
            sut.Send(new string(Enumerable.Repeat('A', capacity).ToArray()))
        );
    }

    [Theory]
    [InlineData("1", "The command '1' is not supported")]
    [InlineData("x", "The command 'X' is not supported")]
    [InlineData("?", "The command '?' is not supported")]
    [InlineData("+", "The command '+' is not supported")]
    [InlineData(" ", "The command ' ' is not supported")]
    public void Should_throw_NotSupportedException_if_input_contains_invalid_chars(string command, string expected)
    {
        var receiver = new Mock<IReceiver>(); 
        var sut = new Transmitter(receiver.Object);

        var exception = Assert.Throws<NotSupportedException>(() => sut.Send(command));

        Assert.Equal(expected, exception.Message);
    }

    [Fact]
    public void Should_create_command_array_with_a_single_item()
    {
        var expected = new ICommand[] { new TurnLeftCommand() };
        var receiver = new Mock<IReceiver>();
        receiver.Setup(m => m.Execute(It.IsAny<ICommand[]>()));

        var sut = new Transmitter(receiver.Object);

        sut.Send("L");

        receiver.Verify(m => m.Execute(It.Is<ICommand[]>(actual => AreListsEqual(expected, actual))), Times.Once);
    }
    

    public static IEnumerable<object[]> CommandData(){
        yield return new object[] { "L", new ICommand[] { new TurnLeftCommand() } };
        yield return new object[] { "R", new ICommand[] { new TurnRightCommand() } };
        yield return new object[] { "F", new ICommand[] { new ForwardCommand() } };
        yield return new object[] { "FFRFLFLF", new ICommand[] { 
            new ForwardCommand(),
            new ForwardCommand(),
            new TurnRightCommand(),
            new ForwardCommand(),
            new TurnLeftCommand(),
            new ForwardCommand(),
            new TurnLeftCommand(),
            new ForwardCommand()
        }};
    }

    [Theory]
    [MemberData(nameof(CommandData))]
    public void Should_create_command_array(string command, ICommand[] expected)
    {
        var receiver = new Mock<IReceiver>();
        receiver.Setup(m => m.Execute(It.IsAny<ICommand[]>()));

        var sut = new Transmitter(receiver.Object);

        sut.Send(command);

        receiver.Verify(m => m.Execute(It.Is<ICommand[]>(actual => AreListsEqual(expected, actual))), Times.Once);
    }

    private static bool AreListsEqual(IEnumerable<ICommand> expected, IEnumerable<ICommand> actual)
    {
        return Enumerable.SequenceEqual(expected, actual, new CommandComparer());
    }

    class CommandComparer : IEqualityComparer<ICommand>
    {
        public bool Equals(ICommand? x, ICommand? y)
        {
            if (y == null && x == null)
                return true;
            else if (x == null || y == null)
                return false;
            else if(x.GetType() == y.GetType())
                return true;
            else
                return false;
        }

        public int GetHashCode(ICommand obj)
        {
            return this.GetType().GetHashCode();
        }
    }
}
