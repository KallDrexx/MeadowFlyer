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