using Meadow.Foundation.Graphics;
using MeadowFlyer.Shared.Rendering;

namespace MeadowFlyer.Shared;

public class App
{
    public InputManager InputManager { get; } = new();
    
    private readonly IRenderer _renderer;
    private readonly string _mapPath;
    private readonly Camera _camera;
    private TimeSpan _timeSinceLastFrame;

    public App(IGraphicsDisplay graphicsDisplay, string fileRoot)
    {
        _camera = new Camera();
        _renderer = new SMackeRenderer(graphicsDisplay);
        _mapPath = Path.Combine(fileRoot, "maps");
    }
    
    public void Run()
    {
        var currentMap = new MapData(_mapPath, "C1W.bmp", "D1.bmp");
        _camera.Position = new Point(_renderer.ScreenWidth / 2, _renderer.ScreenHeight / 2);
        _camera.DirectionAngleDegrees = 180;

        var lastFrameTime = DateTime.Now;
        while (true)
        {
            _timeSinceLastFrame = lastFrameTime - DateTime.Now;
            lastFrameTime = DateTime.Now;
        
            ProcessInput();
            _renderer.Render(_camera, currentMap);
        }
    }

    private void ProcessInput()
    {
        const float anglePerSecond = 90;
        InputManager.Tick();

        if (InputManager.GetButtonState(Constants.Left) == ButtonState.Down)
        {
            _camera.DirectionAngleDegrees -= (float)(anglePerSecond * _timeSinceLastFrame.TotalSeconds);
        }

        if (InputManager.GetButtonState(Constants.Right) == ButtonState.Down)
        {
            _camera.DirectionAngleDegrees += (float)(anglePerSecond * _timeSinceLastFrame.TotalSeconds);
        }
    }
}