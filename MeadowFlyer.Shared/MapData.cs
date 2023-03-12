using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;

namespace MeadowFlyer.Shared;

public class MapData
{
    public BufferRgb565 ColorMap { get; }
    public BufferGray8 HeightMap { get; }

    public MapData(string fileRoot, string colorMapName, string heightMapName)
    {
        ColorMap = LoadColorBitmap(Path.Combine(fileRoot, colorMapName));
        HeightMap = LoadGrayScale(Path.Combine(fileRoot, heightMapName));
    }

    public Point NormalizeCoordinates(Point point)
    {
        var x = point.X;
        var y = point.Y;
        
        // Normalize x and y to always be within bounds, wrapping if outside
        while (x < 0) x += ColorMap.Width;
        while (x >= ColorMap.Width) x -= ColorMap.Width;
        while (y < 0) y += ColorMap.Height;
        while (y >= ColorMap.Height) y -= ColorMap.Height;

        return new Point(x, y);
    }

    public (byte, byte) GetColorAt(Point point)
    {
        const int bytesPerPixel = 2;
        
        // Pre-normalized point expected
        var index = point.Y * ColorMap.Width * bytesPerPixel + point.X * bytesPerPixel;
        return (ColorMap.Buffer[index], ColorMap.Buffer[index + 1]);
    }

    public byte GetHeightAt(Point point)
    {
        const int bytesPerPixel = 1;
        
        // Pre-normalized point expected
        var index = point.Y * HeightMap.Width * bytesPerPixel + point.X * bytesPerPixel;
        return HeightMap.Buffer[index];
    }
    
    private static BufferRgb565 LoadColorBitmap(string filePath)
    {
        Console.WriteLine($"Attempting to LoadBitmapFile: {filePath}");
        
        try
        {
            var img = Image.LoadFromFile(filePath);
            Console.WriteLine($"Color mode: {img.DisplayBuffer.ColorMode}");

            // Always make sure that the texture is formatted in the same color mode as the display
            var imgBuffer = new BufferRgb565(img.Width, img.Height);
            imgBuffer.WriteBuffer(0, 0, img.DisplayBuffer);
            Console.WriteLine($"{filePath} loaded to buffer of type {imgBuffer.GetType()}");
            return imgBuffer;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Failed to load {filePath}: The file should be a 24bit bmp, in the root " +
                                 $"directory with BuildAction = Content, and Copy if Newer!", exception);
            
            throw;
        }
    }
    
    private static BufferGray8 LoadGrayScale(string filePath)
    {
        Console.WriteLine($"Attempting to LoadBitmapFile: {filePath}");
        
        try
        {
            var img = Image.LoadFromFile(filePath);
            Console.WriteLine($"Color mode: {img.DisplayBuffer.ColorMode}");

            // Always make sure that the texture is formatted in the same color mode as the display
            var imgBuffer = new BufferGray8(img.Width, img.Height);
            imgBuffer.WriteBuffer(0, 0, img.DisplayBuffer);
            Console.WriteLine($"{filePath} loaded to buffer of type {imgBuffer.GetType()}");
            return imgBuffer;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Failed to load {filePath}: The file should be a 24bit bmp, in the root " +
                                 $"directory with BuildAction = Content, and Copy if Newer!", exception);
            
            throw;
        }
    }
}