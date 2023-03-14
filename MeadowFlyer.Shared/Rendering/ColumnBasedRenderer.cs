using System.Numerics;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;

namespace MeadowFlyer.Shared.Rendering;

/// <summary>
/// Renderer based on rendering one ray casted column at a time
/// </summary>
public class ColumnBasedRenderer : IRenderer
{
    private readonly IGraphicsDisplay _display;
    private readonly MicroGraphics _microGraphics;
    public int ScreenWidth => _display.Width;
    public int ScreenHeight => _display.Height;

    public ColumnBasedRenderer(IGraphicsDisplay display)
    {
        _display = display;
        _microGraphics = new MicroGraphics(display);
    }

    public void Render(Camera camera, MapData map)
    {
        _display.Clear();

        // Precalculate some trig values
        var directionInRadians = camera.DirectionAngleDegrees * (Math.PI / 180);
        var cosDirection = Math.Cos(directionInRadians);
        var sinDirection = Math.Sin(directionInRadians);
        var halfFovRadians = camera.FovDegrees / 2f * (Math.PI / 180);
        var tanHalfFov = Math.Tan(halfFovRadians);

        var (farLeft, farRight) = CalculateLine(camera, 
            camera.VisibleDistance, 
            cosDirection, 
            sinDirection, 
            tanHalfFov);

        var (nearLeft, nearRight) = CalculateLine(camera,
            25,
            cosDirection,
            sinDirection,
            tanHalfFov);
        
        _microGraphics.DrawLine(farLeft.X, farLeft.Y, farRight.X, farRight.Y, Color.White);
        _microGraphics.DrawLine(nearLeft.X, nearLeft.Y, nearRight.X, nearRight.Y, Color.White);
        _microGraphics.DrawLine(farLeft.X, farLeft.Y, camera.Position.X, camera.Position.Y, Color.White);
        _microGraphics.DrawLine(farRight.X, farRight.Y, camera.Position.X, camera.Position.Y, Color.White);

        _display.Show();
    }

    private static (Point, Point) CalculateLine(Camera camera,
        int distance,
        double cosDirection,
        double sinDirection,
        double tanHalfFov)
    {
        var opposite = tanHalfFov * distance;
        var leftX = -opposite;
        var leftY = -distance;
        var rotatedLeftX = (int)Math.Round(leftX * cosDirection - leftY * sinDirection);
        var rotatedLeftY = (int)Math.Round(leftX * sinDirection + leftY * cosDirection);

        var rightX = opposite;
        var rightY = -distance;
        var rotatedRightX = (int)Math.Round(rightX * cosDirection - rightY * sinDirection);
        var rotatedRightY = (int)Math.Round(rightX * sinDirection + rightY * cosDirection);

        rotatedLeftX += camera.Position.X;
        rotatedLeftY += camera.Position.Y;
        rotatedRightX += camera.Position.X;
        rotatedRightY += camera.Position.Y;

        return (new Point(rotatedLeftX, rotatedLeftY), new Point(rotatedRightX, rotatedRightY));
    }

    private void RenderColumn(
        Point farPlaneLeft,
        int columnIndex,
        Vector2 farPlaneColumnDelta,
        MapData currentMap,
        Camera camera)
    {
        var far = new Point(
            (int)Math.Round(farPlaneLeft.X + columnIndex * farPlaneColumnDelta.X),
            (int)Math.Round(farPlaneLeft.Y + columnIndex * farPlaneColumnDelta.Y));

        var delta = new Vector2(
            (far.X - camera.Position.X) / (float)ScreenHeight,
            (far.Y - camera.Position.Y) / (float)ScreenHeight);

        var cameraPosition = camera.Position;
        var colorMap = currentMap.ColorMap;
        var heightMap = currentMap.HeightMap;

        var row = 0;
        while (row < ScreenHeight)
        {
            var sampleX = (int)(cameraPosition.X + row * delta.X);
            var sampleY = (int)(cameraPosition.Y + row * delta.Y);

            while (sampleX < 0) sampleX += colorMap.Width;
            while (sampleX >= colorMap.Width) sampleX -= colorMap.Width;
            while (sampleY < 0) sampleY += colorMap.Height;
            while (sampleY >= colorMap.Height) sampleY -= colorMap.Height;

            var colorIndex = sampleY * colorMap.Width * 2 + sampleX * 2;
            var heightIndex = sampleY * colorMap.Width + sampleX;
            var sampleHeight = heightMap.Buffer[heightIndex];
            var sampleTopRow = (int)((50 - sampleHeight) / (float)(row + 1) * 120 + 120);

            if (sampleTopRow <= row)
            {
                row++;
                continue;
            }

            while (row < sampleTopRow)
            {
                var displayIndex = (ScreenHeight - row - 1) * _display.Width * 2 + columnIndex * 2;
                _display.PixelBuffer.Buffer[displayIndex] = colorMap.Buffer[colorIndex];
                _display.PixelBuffer.Buffer[displayIndex + 1] = colorMap.Buffer[colorIndex + 1];

                row++;
            }
        }
    }
}