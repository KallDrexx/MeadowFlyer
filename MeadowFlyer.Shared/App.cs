using Meadow.Foundation.Graphics;

namespace MeadowFlyer.Shared;

public class App
{
    private readonly Renderer _renderer;
    private readonly string _mapPath;
    private readonly Camera _camera;

    public App(IGraphicsDisplay graphicsDisplay, string fileRoot)
    {
        _camera = new Camera();
        _renderer = new Renderer(new MicroGraphics(graphicsDisplay), _camera);
        _mapPath = Path.Combine(fileRoot, "maps");
    }
    
    public void Run()
    {
        var currentMap = new MapData(_mapPath, "C1W.bmp", "D1.bmp");
        _camera.Position = new Point(_renderer.ScreenWidth / 2, _renderer.ScreenHeight / 2);
        _camera.DirectionAngleDegrees = 180;
        
        while (true)
        {
            _renderer.Render(currentMap);
        }
    }
}