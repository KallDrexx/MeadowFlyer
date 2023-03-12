using MeadowFlyer.Shared;
using MeadowMgTestEnvironment;
using Microsoft.Xna.Framework.Input;

var environment = new TestEnvironment(240, 240);
var app = new App(environment.Display, Environment.CurrentDirectory);

environment.BindKey(Keys.Left,
    () => app.InputManager.ButtonPushed(Constants.Left),
    () => app.InputManager.ButtonReleased(Constants.Left));

environment.BindKey(Keys.Right,
    () => app.InputManager.ButtonPushed(Constants.Right),
    () => app.InputManager.ButtonReleased(Constants.Right));

app.Run();