namespace LD53;

class MainMenu : Scene
{
    public MainMenu(int width, int height, string id) : base(width, height, id)
    {
    }

    public override void Awake()
    {
        SetClearColor(255, 234, 210, 255);
    }

    public override void Sleep()
    {

    }

    public override void Update()
    {
        new Texture("Images/Logo.png")
            .Position(new(WindowSize.X / 2, 30))
            .Center(Center.Middle)
            .Render();


        LD.DrawButton(new(WindowSize.X / 2 - (4 * 2 + 2), 60), "Play", Mouse.Position, () => {
            SceneHandler.Unload("MainMenu");
            SceneHandler.Get("Main").Entities.Clear();
            SceneHandler.Get("Main").AwakeCall();
            SceneHandler.Load("Main");

            SceneHandler.Get("HUD").Entities.Clear();
            SceneHandler.Get("HUD").AwakeCall();
            SceneHandler.Load("HUD");

            SceneHandler.Get<InspectorScene>().SelectedScene = "Main";
        });

        Draw.Rectangle(new(0, 0, WindowSize.X, WindowSize.Y), new(235, 214, 190, 255));
    }
}