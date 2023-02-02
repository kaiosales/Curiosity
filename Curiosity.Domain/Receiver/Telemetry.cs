using System.Drawing;

namespace Curiosity.Domain;

public readonly record struct Telemetry(Size Plateau, Point Position, string Direction);