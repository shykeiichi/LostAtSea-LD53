namespace LD53;

class Map : Scene
{
    MainScene main;

    Vector2 MapOffset = new();
    Vector2 TempMapOffset = new();

    Vector2 MouseStart = new();

    float zoom = 8;

    int BMPressed = 0;
    int BESCPressed = 0;

    int BPanPressed = 0;
    int BScrollPressed = 0;

    Texture BPanOff = new("Images/HUD/ButtonPan0.png");
    Texture BPanOn = new("Images/HUD/ButtonPan1.png");

    Texture BScroll = new("Images/HUD/ButtonScroll.png");

    public Map(int width, int height, string id) : base(width, height, id)
    {
    }

    public override void Awake()
    {
        SetClearColor(255, 234, 210, 255);
        main = SceneHandler.Get<MainScene>();
        MapOffset = new(-140, -78);
    }

    public override void Sleep()
    {

    }

    public override void Update()
    {
        if(Mouse.Pressed(MB.Left))
        {
            MouseStart = new(Mouse.Position.X, Mouse.Position.Y);
        }

        if(Mouse.Released(MB.Left))
        {
            MapOffset = (MouseStart + MapOffset) - (Mouse.Position);
        }

        if(Mouse.Down(MB.Left))
        {
            TempMapOffset = (MouseStart + MapOffset) - (Mouse.Position);
        } else {
            TempMapOffset = MapOffset;
        }

        if(Keyboard.Down(Key.W))
        {
            MapOffset.Y -= 400f * DeltaTime;
        }
        if(Keyboard.Down(Key.S))
        {
            MapOffset.Y += 400f * DeltaTime;
        }

        if(Keyboard.Down(Key.A))
        {
            MapOffset.X -= 400f * DeltaTime;
        }
        if(Keyboard.Down(Key.D))
        {
            MapOffset.X += 400f * DeltaTime;
        }
        
        if(Keyboard.Pressed(Key.M))
        {
            BMPressed++;
        }

        if(Keyboard.Pressed(Key.ESCAPE))
        {
            BESCPressed++;
        }

        if(Keyboard.Pressed(Key.M) || Keyboard.Pressed(Key.ESCAPE))
        {
            // SceneHandler.Load("Main");
            SceneHandler.Unload("Map");
        }

        // LD.DrawSmallFont(new(0, 0), "asd");

        // foreach(var i in main.Map)
        // {
        //     // Draw.Text(i.Item2 - MapOffset, "Pixuf.ttf", i.Item2.ToString(), 8, new(0, 0, 0, 255));
        //     LD.DrawSmallFont(i.Item2 - MapOffset, i.Item2.ToString());
        //     foreach(var j in main.Islands)
        //     {
        //         foreach(var k in j.ToList())
        //         {
        //             Draw.Box(new(i.Key.X * 16 - Map[j].Item2.X, i.Key.Y * 16 - Map[j].Item2.Y, 1, 1), new(179, 165, 85, 255));
        //         }
        //     }
        // }

        // Console.WriteLine($"{Mouse.Released(MB.Left)} {Mouse.Down(MB.Left)}");
   
        Draw.Box(new(2, 2, 35, 40), new(235, 214, 190, 255), 10000);

        Draw.Box(new(4, 4, 4, 4), new(179, 165, 85, 255), 10000);
        LD.DrawSmallFont(new(12, 4), "Land", 10000);

        Draw.Box(new(4, 12, 4, 4), new(100, 81, 59, 255), 10000);
        LD.DrawSmallFont(new(12, 12), "City", 10000);

        Draw.Box(new(4, 20, 4, 4), new(176, 95, 102, 255), 10000);
        LD.DrawSmallFont(new(12, 20), "Start", 10000);

        new Texture("Images/Marker.png")
            .Center(Center.BottomMiddle)
            .Position(6, 28)
            .Center(Center.TopMiddle)
            .Destroy(false)
            .Depth(10002)
            .Render();
        LD.DrawSmallFont(new(12, 32), "Dest.", 10000);

        for(var j = 0; j < main.Map.Count; j++)
        {
            foreach(var i in main.Islands[main.Map[j].Item1])
            {
                // main.Tiles[i.Value.TileID].tex
                //     .Position((i.Key.X * 16 - main.Map[j].Item2.X) / zoom - MapOffset.X, (i.Key.Y * 16 - main.Map[j].Item2.Y) / zoom - MapOffset.Y)
                //     .GetRect(out Vector4 rect)
                //     .Size(rect.Z / zoom, rect.W / zoom)
                //     .Render();

                new Rectangle(
                    new((i.Key.X * 16 - main.Map[j].Item2.X) / zoom - TempMapOffset.X, (i.Key.Y * 16 - main.Map[j].Item2.Y) / zoom - TempMapOffset.Y,
                        16 / zoom + 2,
                        16 / zoom + 2)
                )
                    .Color(new(179, 165, 85, 255))
                    .Fill(true)
                    .Render();

                // Draw.Box(new(i.Key.X * 16 - main.Map[j].Item2.X - MapOffset.X, i.Key.Y * 16 - main.Map[j].Item2.Y - MapOffset.Y, 1, 1), new(255, 255, 255), 5000);

                // if(i.Value.IsCity)
                // {
                //     Draw.Box(rect, new(255, 0, 0, 100));
                // }
            }
        }

        for(var idx = 0; idx < main.Cities.Count; idx++)
        {
            var i = main.Cities[idx];
            
            if(zoom < 12)
            {
                LD.DrawSmallFont((i.Item2 / zoom) - TempMapOffset, $"{i.Item1}");
            }

            bool selected = main.startCity == idx;

            new Rectangle(
                new((i.Item2 / zoom) - TempMapOffset,
                    16 / zoom + 2 + (selected ? 2 : 0),
                    16 / zoom + 2 + (selected ? 2 : 0))
            )
                .Color(
                    main.startCity == idx ?
                        new(176, 95, 102, 255)
                    :
                        new(100, 81, 59, 255)
                )
                .Fill(true)
                .Render();

            if(main.selectedCity == idx)
            {
                new Texture("Images/Marker.png")
                    .Center(Center.BottomMiddle)
                    .Position((i.Item2 / zoom) - TempMapOffset)
                    .Destroy(false)
                    .Depth(4998)
                    .Render();
            }
        }

        if(Mouse.Pressed(MB.ScrollUp))
        {
            zoom += 1f;
        }
        if(Mouse.Pressed(MB.ScrollDown))
        {
            zoom -= 1f;
        }

        zoom = Math.Clamp(zoom, 1, 32);

        LD.DrawButton(new(3, WindowSize.Y - 11), "Reset Map Position", Mouse.Position, () => {
            MapOffset = new(-140, -78);
        });

        Draw.Rectangle(new(0, 0, WindowSize.X, WindowSize.Y), new(235, 214, 190, 255));     

        if(BScrollPressed > 5)
        {
            if(Mouse.Pressed(MB.Left))
            {
                BPanPressed++;
            }
        }

        if(Mouse.Pressed(MB.ScrollUp) || Mouse.Pressed(MB.ScrollDown))
        {
            BScrollPressed++;
        }

        if(BScrollPressed < 5)
        {
            BScroll.Position(new(WindowSize.X / 2, WindowSize.Y - 3)).Center(Center.BottomMiddle).Depth(30000).Render();

            new Text("Pixuf.ttf", "Scroll To Zoom")
                .Size(8)
                .Color(new Vector4(0, 0, 0, 255))
                .RenderToTexture()
                .Destroy(true)
                .Center(Center.BottomRight)
                .Position(new(WindowSize.X / 2 - 7, WindowSize.Y - 4))
                .Depth(30000)
                .GetRect(out Vector4 rect)
                .Render(); 

            Draw.Box(Vector4.Add(rect, new(-2, -2, 3, 3)), new(235, 214, 190, 255), 29999);     
        }
        else if(BPanPressed < 1)
        {
            if(!Mouse.Down(MB.Left))
                BPanOn.Position(new(WindowSize.X / 2, WindowSize.Y - 3)).Center(Center.BottomMiddle).Depth(30000).Render();
            else
                BPanOff.Position(new(WindowSize.X / 2, WindowSize.Y - 3)).Center(Center.BottomMiddle).Depth(30000).Render();

            new Text("Pixuf.ttf", "Pan Around")
                .Size(8)
                .Color(new Vector4(0, 0, 0, 255))
                .RenderToTexture()
                .Destroy(true)
                .Center(Center.BottomRight)
                .Position(new(WindowSize.X / 2 - 7, WindowSize.Y - 4))
                .Depth(30000)
                .GetRect(out Vector4 rect)
                .Render(); 

            Draw.Box(Vector4.Add(rect, new(-2, -2, 3, 3)), new(235, 214, 190, 255), 29999);     
        } 
        else if((BMPressed < 1) && (BESCPressed < 1))
        {
            SceneHandler.Get<HUD>().BMOn.Position(new(WindowSize.X / 2, WindowSize.Y - 3)).Center(Center.BottomMiddle).Depth(30000).Render();

            SceneHandler.Get<HUD>().BESCOn.Position(new(WindowSize.X / 2 + 18, WindowSize.Y - 3)).Center(Center.BottomLeft).Depth(30000).Render();

            new Text("Pixuf.ttf", "Close Map")
                .Size(8)
                .Color(new Vector4(0, 0, 0, 255))
                .RenderToTexture()
                .Destroy(true)
                .Center(Center.BottomRight)
                .Position(new(WindowSize.X / 2 - 7, WindowSize.Y - 4))
                .Depth(30000)
                .GetRect(out Vector4 rect1)
                .Render(); 

            new Text("Pixuf.ttf", "or")
                .Size(8)
                .Color(new Vector4(0, 0, 0, 255))
                .RenderToTexture()
                .Destroy(true)
                .Center(Center.BottomLeft)
                .Position(new(WindowSize.X / 2 + 9, WindowSize.Y - 4))
                .Depth(30000)
                .GetRect(out Vector4 rect2)
                .Render(); 

            Draw.Box(Vector4.Add(rect1, new(-2, -2, 3, 3)), new(235, 214, 190, 255), 29999);  
            Draw.Box(Vector4.Add(rect2, new(-2, -2, 3, 3)), new(235, 214, 190, 255), 29999);        
        }
    }
}