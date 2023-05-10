namespace LD53;

class LostAtSea : Scene
{
    public LostAtSea(int width, int height, string id) : base(width, height, id)
    {
    }

    public override void Awake()
    {
        SetClearColor(255, 234, 210, 255);
    }

    public override void Update()
    {
        new Text("Pixuf.ttf", "Looks like you got lost at sea, Traveller")
            .Position(WindowSize / 2)
            .Color(new Vector4(0, 0, 0, 255))
            .Size(8)
            .RenderToTexture()
            .Center(Center.Middle)
            .Render();

        LD.DrawButton(new(WindowSize.X / 2 - ("Return to Main Menu".Length * 4 / 2 + 2), 60), "Return to Main Menu", Mouse.Position, () => {
            SceneHandler.Unload("LostAtSea");
            SceneHandler.Load("MainMenu");
        });

         Draw.Rectangle(new(0, 0, WindowSize.X, WindowSize.Y), new(235, 214, 190, 255));      
    }
}