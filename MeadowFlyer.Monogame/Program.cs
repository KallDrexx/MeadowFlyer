using MeadowFlyer.Shared;
using MeadowMgTestEnvironment;

var environment = new TestEnvironment(240, 240);
var app = new App(environment.Display, Environment.CurrentDirectory);

app.Run();