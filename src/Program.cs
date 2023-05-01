namespace LD53;

class Program
{
    static void Main(string[] args)
    {
        Game.Initialize("Delivery LD53", 1920, 1080);

        SceneHandler.Register(new MainScene(280, 157, "Main")
            .SetRelativeWindowSize(0, 0, 1, 1)
            .SetAlwaysAtBack(true)
        );

        SceneHandler.Register(new Map(280, 157, "Map")
            .SetRelativeWindowSize(0, 0, 1f, 1f)
        );

        SceneHandler.Register(new LostAtSea(280, 157, "LostAtSea")
            .SetRelativeWindowSize(0, 0, 1f, 1f)
        );

        SceneHandler.Register(new MainMenu(280, 157, "MainMenu")
            .SetRelativeWindowSize(0, 0, 1f, 1f)
        );

        SceneHandler.Register(new EditorInspector(280, 157, "EditorInspector")
            .SetRelativeWindowSize(0f, 0, 0.2f, 1)
            .SetAlwaysAtBack(true)
            .SetAlwaysRebuildTexture(true)
        );
        
        SceneHandler.Register(new Editor(280, 157, "Editor")
            .SetRelativeWindowSize(0.2f, 0, 1, 1)
            .SetAlwaysAtBack(true)
            .SetAlwaysRebuildTexture(true)
        );

        if(args.Length > 0)
        {
            SceneHandler.Load("EditorInspector");
            SceneHandler.Load("Editor");
        } else {
            SceneHandler.Load("MainMenu");
            // SceneHandler.Load("Map");
        }

        Game.Run();
    }
}