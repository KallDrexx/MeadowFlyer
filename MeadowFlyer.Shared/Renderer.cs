using Meadow.Foundation;
using Meadow.Foundation.Graphics;

namespace MeadowFlyer.Shared;

public class Renderer
{
    private readonly MicroGraphics _display;
    private readonly Camera _camera;

    public int ScreenWidth => _display.Width;
    public int ScreenHeight => _display.Height;

    public Renderer(MicroGraphics display, Camera camera)
    {
        _display = display;
        _camera = camera;
    }

    public void Render(MapData currentMap)
    {
        _display.Clear(Color.Black);
        
        var (left, right) = CalculateFarPlane();
        _display.DrawPixel(_camera.Position.X, _camera.Position.Y, Color.White);
        _display.DrawLine(left.X, left.Y, right.X, right.Y, Color.White);
        _display.DrawLine(left.X, left.Y, _camera.Position.X, _camera.Position.Y, Color.White);
        _display.DrawLine(right.X, right.Y, _camera.Position.X, _camera.Position.Y, Color.White);
        
        _display.Show();
    }

    private (Point, Point) CalculateFarPlane()
    {
        var directionInRadians = _camera.DirectionAngleDegrees * (Math.PI / 180);
        var halfFovRadians = _camera.FovDegrees / 2f * (Math.PI / 180);
        var opposite = (int)(Math.Tan(halfFovRadians) * _camera.VisibleDistance);

        var leftX = -opposite;
        var leftY = -_camera.VisibleDistance;

        var rotatedLeftX = (int)Math.Round(leftX * Math.Cos(directionInRadians) - leftY * Math.Sin(directionInRadians));
        var rotatedLeftY = (int)Math.Round(leftY * Math.Cos(directionInRadians) + leftX * Math.Sin(directionInRadians));

        var rightX = opposite;
        var rightY = -_camera.VisibleDistance;

        var rotatedRightX = (int)Math.Round(rightX * Math.Cos(directionInRadians) - rightY * Math.Sin(directionInRadians));
        var rotatedRightY = (int)Math.Round(rightY * Math.Cos(directionInRadians) + rightX * Math.Sin(directionInRadians));

        rotatedLeftX += _camera.Position.X;
        rotatedLeftY += _camera.Position.Y;
        rotatedRightX += _camera.Position.X;
        rotatedRightY += _camera.Position.Y;

        return (new Point(rotatedLeftX, rotatedLeftY), new Point(rotatedRightX, rotatedRightY));
    }
}