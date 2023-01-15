using System.Collections.ObjectModel;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Curiosity.Domain;

public class Transmitter
{
    private static Regex mapRegex = new Regex(@"(\d+)x(\d+)", RegexOptions.Compiled);
    private const int CAPACITY = 100;
    private Queue<ICommand> buffer = new Queue<ICommand>(CAPACITY);
    private readonly IReceiver receiver;


    public Transmitter(IReceiver receiver)
    {
        ArgumentNullException.ThrowIfNull(receiver, nameof(receiver));

        this.receiver = receiver;
    }

    public void Init(string plateau)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(plateau, nameof(plateau));

        var match = mapRegex.Match(plateau);
        if (match == null || !match.Success)
            throw new FormatException();

        var width = Int32.Parse(match.Groups[1].Value);
        var height = Int32.Parse(match.Groups[2].Value);

        if (width < 1 || height < 1)
            throw new ArgumentException("", nameof(plateau));

        var size = new Size(width, height);
        this.receiver.Init(size);
    }

    public ReadOnlyCollection<Telemetry> Send(string input)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(input, nameof(input));

        if (input.Length > CAPACITY)
            throw new ArgumentOutOfRangeException(nameof(input));

        for (var i = 0; i < input.Length; i++) 
        {
            var commandText = Char.ToUpper(input[i]);
            var command = CommandFactory.Create(commandText);
            this.buffer.Enqueue(command);
        }

        this.receiver.Execute(this.buffer.ToArray());
        this.buffer.Clear();

        return this.receiver.State();
    }
}
