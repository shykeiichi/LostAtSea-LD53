namespace LD53;

class Map : Scene
{
    MainScene main;

    Vector2 MapOffset = new();
    Vector2 TempMapOffset = new();

    Vector2 MouseStart = new();

    float zoom = 8;

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

    public override void Update(double dt)
    {
        if(Mouse.Pressed(MB.Left))
        {
            MouseStart = new(Mouse.Position.X, Mouse.Position.Y);
        }

        if(Mouse.Down(MB.Left))
        {
            TempMapOffset = (MouseStart + MapOffset) - (Mouse.Position);
        } else {
            TempMapOffset = MapOffset;
        }

        if(Mouse.Released(MB.Left))
        {
            MapOffset = (MouseStart + MapOffset) - (Mouse.Position);
        }

        if(Keyboard.Down(Key.W))
        {
            MapOffset.Y -= 400f * (float)dt;
        }
        if(Keyboard.Down(Key.S))
        {
            MapOffset.Y += 400f * (float)dt;
        }

        if(Keyboard.Down(Key.A))
        {
            MapOffset.X -= 400f * (float)dt;
        }
        if(Keyboard.Down(Key.D))
        {
            MapOffset.X += 400f * (float)dt;
        }
        
        if(Keyboard.Pressed(Key.M) || Keyboard.Pressed(Key.ESCAPE))
        {
            // SceneHandler.Load("Main");
            SceneHandler.Unload("Map");
        }
    }

    public override void Render()
    {   
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
    }
}