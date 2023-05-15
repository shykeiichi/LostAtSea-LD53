namespace LD53;

class PauseMenu : Scene
{
    public PauseMenu(int width, int height, string id) : base(width, height, id)
    {
    }

    public override void Awake()
    {
        SetClearColor(255, 234, 210, 100);
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


        LD.DrawButton(new(WindowSize.X / 2 - (4 * 2 + 2), 60), "Resume", Mouse.Position, () => {
            SceneHandler.Unload("PauseMenu");
        });

        LD.DrawButton(new(WindowSize.X / 2 - (4 * 2 + 2), 72), "Quit", Mouse.Position, () => {
            SceneHandler.Unload("PauseMenu");
            SceneHandler.Unload("MainScene");
            SceneHandler.Load("MainMenu");
        });

        Draw.Rectangle(new(0, 0, WindowSize.X, WindowSize.Y), new(235, 214, 190, 255));
    }
}