using Meadow.Foundation.Graphics;

namespace MeadowFlyer.Shared;

public class Camera
{
    public Point Position { get; set; }
    public int FovDegrees { get; set; } = 90;
    public int VisibleDistance { get; set; } = 300;
    public float DirectionAngleDegrees { get; set; }
}