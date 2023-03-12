using System.Numerics;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;

namespace MeadowFlyer.Shared.Rendering;

/// <summary>
/// Renderer that lifts the algorithm from https://github.com/s-macke/VoxelSpace
/// </summary>
public class SMackeRenderer : IRenderer
{
    private readonly IGraphicsDisplay _display;

    public SMackeRenderer(IGraphicsDisplay display)
    {
        _display = display;
    }

    public int ScreenWidth => _display.Width;
    public int ScreenHeight => _display.Height;

    public void Render(Camera camera, MapData map)
    {
        const int height = 100;
        const int scaleHeight = 120;
        const int horizon = 120;
        _display.Fill(Color.CornflowerBlue);
        
        var angleInRadians = camera.DirectionAngleDegrees * Math.PI / 180;
        var sinAngle = Math.Sin(angleInRadians);
        var cosAngle = Math.Cos(angleInRadians);

        var yBuffer = new ushort[ScreenWidth];
        for (var x = 0; x < yBuffer.Length; x++)
        {
            yBuffer[x] = (ushort)(ScreenHeight - 1);
        }
        
        // Draw front to back (low z to high z)
        var deltaZ = 1f;
        var z = 1f;
        while (z < camera.VisibleDistance)
        {
            // Find a line on the map based on a field of view of 90 degrees (hardcoded)
            var pointLeft = new Point(
                (int)(-cosAngle * z - sinAngle * z) + camera.Position.X,
                (int)(sinAngle * z - cosAngle * z) + camera.Position.Y);

            var pointRight = new Point(
                (int)(cosAngle * z - sinAngle * z) + camera.Position.X,
                (int)(-sinAngle * z - cosAngle * z) + camera.Position.Y);
            
            // Segment the line
            var deltaX = (pointRight.X - pointLeft.X) / (float) ScreenWidth;
            var deltaY = (pointRight.Y - pointLeft.Y) / (float) ScreenWidth;
            
            // Raster line
            var point = new Vector2(pointLeft.X, pointLeft.Y);
            for (var column = 0; column < ScreenWidth; column++)
            {
                var samplePoint = map.NormalizeCoordinates(new Point((int)point.X, (int)point.Y));
                var heightValue = map.GetHeightAt(samplePoint);
                var colorValue = map.GetColorAt(samplePoint);
                var heightOnScreen = (height - heightValue) / z * scaleHeight + horizon;
                if (heightOnScreen < yBuffer[column])
                {
                    for (var row = yBuffer[column]; row > heightOnScreen; row--)
                    {
                        const int bytesPerPixel = 2;
                        var index = row * _display.Width * bytesPerPixel + column * bytesPerPixel;
                        _display.PixelBuffer.Buffer[index] = colorValue.Item1;
                        _display.PixelBuffer.Buffer[index + 1] = colorValue.Item2;
                    }
                    
                    yBuffer[column] = (ushort) heightOnScreen;
                }

                point = new Vector2(point.X + deltaX, point.Y + deltaY);
            }
            
            // Go to the next line and increase step size as you get far away
            z += deltaZ;
            deltaZ += 0.2f;
        }

        _display.Show();
    }
}