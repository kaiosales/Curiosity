using Spectre.Console;
using Curiosity.Domain;
using Spectre.Console.Rendering;
using Point = System.Drawing.Point;
using System.Collections.ObjectModel;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Program p = new Program();
        await p.Start();
    }

    public async Task Start()
    {
        this.WriteWelcome();

        await this.WriteInitialization();

        Rover curiosity = new Rover();
        Transmitter transmitter = new Transmitter(curiosity);

        // // 5X5
        var plateau = this.AskPlateau();

        // // FFRFLFLF
        var commands = this.AskCommands();

        // this.WriteSummary("5X5", "FFRFLFLF");
        this.WriteSummary(plateau, commands);
        
        // transmitter.Init("5x5");
        transmitter.Init(plateau);

        // var telemetry = transmitter.Send("FFRFLFLF");
        var telemetry = transmitter.Send(commands);

        // this.WriteProgress("FFRFLFLF");
        await this.WriteProgress(commands);

        await this.WriteResults(telemetry);

        this.WriteEnd();
    }

    private void WriteWelcome()
    {
        var text = new FigletText("NASA-ish Rover Control")
                        .Centered()
                        .Color(Color.Red);
        var panel = new Panel(text)
                        .Header("WELCOME", Justify.Center)
                        .Border(BoxBorder.Double);

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine("");
        
    }

    private async Task WriteInitialization()
    {
        var writeLogMessage = (string message) => AnsiConsole.MarkupLine($"[grey]LOG:[/] {message}[grey]...[/]");

        await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Default)
            .StartAsync("[yellow]Initializing warp drive[/]", async ctx =>
            {
                // Initialize
                await Task.Delay(1500);
                writeLogMessage("Starting gravimetric field displacement manifold");
                await Task.Delay(500);
                writeLogMessage("Warming up deuterium chamber");
                await Task.Delay(1000);
                writeLogMessage("Generating antideuterium");

                // Warp nacelles
                await Task.Delay(1500);
                ctx.Spinner(Spinner.Known.BouncingBar);
                ctx.Status("[bold blue]Unfolding warp nacelles[/]");
                writeLogMessage("Unfolding left warp nacelle");
                await Task.Delay(1000);
                writeLogMessage("Left warp nacelle [green]online[/]");
                writeLogMessage("Unfolding right warp nacelle");
                await Task.Delay(500);
                writeLogMessage("Right warp nacelle [green]online[/]");

                // Warp bubble
                await Task.Delay(1500);
                ctx.Spinner(Spinner.Known.Star2);
                ctx.Status("[bold blue]Generating warp bubble[/]");
                await Task.Delay(1500);
                ctx.Spinner(Spinner.Known.Star);
                ctx.Status("[bold blue]Stabilizing warp bubble[/]");

                // Safety
                ctx.Spinner(Spinner.Known.Monkey);
                ctx.Status("[bold blue]Performing safety checks[/]");
                writeLogMessage("Enabling interior dampening");
                await Task.Delay(1000);
                writeLogMessage("Interior dampening [green]enabled[/]");

                // Warp!
                await Task.Delay(1500);
                ctx.Spinner(Spinner.Known.Moon);
                writeLogMessage("Preparing for warp");
                await Task.Delay(500);
                for (var warp = 1; warp < 5; warp++)
                {
                    ctx.Status($"[bold blue]Warp {warp}[/]");
                    await Task.Delay(500);
                }
            });

        AnsiConsole.MarkupLine("[bold green]Crusing at Warp 5.8[/]");
        AnsiConsole.WriteLine("");
        AnsiConsole.Write(new Rule($"[bold green]Initialization completed[/]").RuleStyle("grey").LeftJustified());
        AnsiConsole.WriteLine("");
    }

    private string AskPlateau()
    {
        var prompt = new TextPrompt<string>("Enter plateau [green]size[/]:");

        var ret = AnsiConsole.Prompt(prompt);
        AnsiConsole.WriteLine("");

        return ret;
    }

    private string AskCommands()
    {
        var prompt = new TextPrompt<string>("Now, please input [green]commands[/]:");

        var ret = AnsiConsole.Prompt(prompt);
        AnsiConsole.WriteLine("");
        

        return ret;
    }

    private void WriteSummary(string plateau, string commands)
    {
        var table = new Table()
                        .Title("Summary")
                        .Border(TableBorder.DoubleEdge)
                        .HideHeaders();
        table.AddColumn("");
        table.AddColumn(new TableColumn("").RightAligned());

        table.AddRow("Plateau", $"[green]{plateau}[/]");
        table.AddRow("Commands", $"[green]{commands}[/]");

        AnsiConsole.Write(table);
    }

    private async Task WriteProgress(string commands)
    {
        await AnsiConsole.Progress()
            .AutoClear(false)
            .Columns(new ProgressColumn[]
            {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn(),
            })
            .StartAsync(async ctx => 
            {
                var random = new Random(DateTime.Now.Millisecond);

                var task1 = ctx.AddTask("Establishing connection to hover");
                var task2 = ctx.AddTask("Sending commands", autoStart: false).IsIndeterminate();
                var task3 = ctx.AddTask("Processing telemetry", autoStart: false).IsIndeterminate();

                while(!ctx.IsFinished) 
                {
                    task1.Increment(random.NextDouble() * random.Next(2, 10));
                    await Task.Delay(40);
                }

                task2.StartTask();
                task2.IsIndeterminate(false);

                while(!ctx.IsFinished) 
                {
                    task2.Increment(100 / commands.Length);
                    await Task.Delay(250);
                }

                task3.StartTask();
                task3.IsIndeterminate(false);

                while(!ctx.IsFinished) 
                {
                    task3.Increment(100 / commands.Length);
                    await Task.Delay(500);
                }
            });
    }

    private async Task WriteResults(ReadOnlyCollection<Telemetry> telemetry)
    {
        AnsiConsole.Write(new Rule($"[bold green]Results[/]").RuleStyle("grey").LeftJustified());
        AnsiConsole.WriteLine("");
        var last = telemetry.Last();
        AnsiConsole.Markup($"End state: [bold green]{last.Position.X}, {last.Position.Y}, {last.Direction}[/]");
        AnsiConsole.WriteLine("");

        await Task.Delay(500);
        if (AnsiConsole.Confirm("Do you want to preview a simulation?"))
        {
            await this.WriteSimulation(telemetry);
        }
    }
    
    private async Task WriteSimulation(ReadOnlyCollection<Telemetry> telemetry)
    {
        AnsiConsole.Markup($"Rendering simulation:");
        AnsiConsole.WriteLine("");
        for(var i = 0; i < telemetry.Count; i++)
        {
            var snapshot = telemetry[i];
            await this.WriteMap(i < telemetry.Count - 1, snapshot);
        }
        AnsiConsole.WriteLine("");
    }

    private async Task WriteMap(bool clear, Telemetry telemetry)
    {
        var size = new Size(telemetry.Plateau.Width, telemetry.Plateau.Height);

        var createCell = (bool current, string dir) => {

            var grid = new Grid().Centered();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();

            if (!current)
            {
                grid.AddRow(new string[]{" ", "  ", " "});
                grid.AddRow(new string[]{"  ", "  ", "  "});
                grid.AddRow(new string[]{" ", "  ", " "});
            } else {
                grid.AddRow(new Text(" "), new Markup(dir == "North" ? " :up_arrow:" : " "), new Text(" "));
                grid.AddRow(new Markup(dir == "West" ? ":left_arrow:" : " "), new Markup(":robot:"), new Markup(dir == "East" ? ":right_arrow:" : " "));
                grid.AddRow(new Text(" "), new Markup(dir == "South" ? " :down_arrow:" : " "), new Text(" "));
            }

            var p = new Panel(grid);
            p.BorderStyle(new Style(Color.DarkRed));
            p.Width = 10;
            p.Height = 5;
            return p;
        };

        var grid = new Grid().Centered();
        await AnsiConsole.Live(grid)
            .AutoClear(clear)
            .StartAsync(async ctx => 
            {
                var map = new IRenderable[size.Width, size.Height];

                for (var i = 0; i < size.Width; i++)
                    grid.AddColumn(new GridColumn().Width(12));

                for (var i = 0; i < size.Height; i++) {
                    var cells = new List<IRenderable>();
                    for (var j = 0; j < size.Width; j++) {
                        cells.Add(createCell(telemetry.Position.Equals(new Point(j + 1, size.Width - i)), telemetry.Direction));
                    }
                    grid.AddRow(cells.ToArray());
                    ctx.Refresh();
                    await Task.Delay(50);
                }
                await Task.Delay(1000);
            });
    }

    private void WriteEnd()
    {
        AnsiConsole.Write(new Rule($"").RuleStyle("grey"));
        AnsiConsole.Write(new Rule($"End").RuleStyle("grey"));
        AnsiConsole.Write(new Rule($"").RuleStyle("grey"));
        AnsiConsole.WriteLine("");
    }
}