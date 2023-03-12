namespace MeadowFlyer.Shared.Rendering;

public interface IRenderer
{
    int ScreenWidth { get; }
    int ScreenHeight { get; }
    void Render(Camera camera, MapData map);
}