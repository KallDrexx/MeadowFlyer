using System.Numerics;
using Meadow.Foundation.Graphics;

namespace MeadowFlyer.Shared.Rendering;

/// <summary>
/// Renderer based on rendering one ray casted column at a time
/// </summary>
public class ColumnBasedRenderer : IRenderer
{
    private readonly IGraphicsDisplay _display;
    public int ScreenWidth => _display.Width;
    public int ScreenHeight => _display.Height;

    public ColumnBasedRenderer(IGraphicsDisplay display)
    {
        _display = display;
    }

    public void Render(Camera camera, MapData map)
    {
        _display.Clear();
        
        var (left, right) = CalculateFarPlane(camera);
        // should we have a near plane?

        var farPlaneColumnDelta = new Vector2(
            (right.X - left.X) / (float)ScreenWidth,
            (right.Y - left.Y) / (float)ScreenWidth);
        
        for (var column = 0; column < ScreenWidth; column++)
        {
            RenderColumn(left, column, farPlaneColumnDelta, map, camera);
        }
        
        _display.Show();
    }
    
    private (Point, Point) CalculateFarPlane(Camera camera)
    {
        var directionInRadians = camera.DirectionAngleDegrees * (Math.PI / 180);
        var cosDirection = Math.Cos(directionInRadians);
        var sinDirection = Math.Sin(directionInRadians);
        
        var halfFovRadians = camera.FovDegrees / 2f * (Math.PI / 180);
        var opposite = (int)(Math.Tan(halfFovRadians) * camera.VisibleDistance);

        var leftX = -opposite;
        var leftY = -camera.VisibleDistance;

        var rotatedLeftX = (int)Math.Round(leftX * cosDirection - leftY * sinDirection);
        var rotatedLeftY = (int)Math.Round(leftY * cosDirection + leftX * sinDirection);

        var rightX = opposite;
        var rightY = -camera.VisibleDistance;

        var rotatedRightX = (int)Math.Round(rightX * cosDirection - rightY * sinDirection);
        var rotatedRightY = (int)Math.Round(rightY * cosDirection + rightX * sinDirection);

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