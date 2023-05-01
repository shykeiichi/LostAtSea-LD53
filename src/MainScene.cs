namespace LD53;

class MainScene : Scene
{
    double rectAngle = 0f;

    public List<Dictionary<Vector2, Tile>> Islands = new();
    public List<(int, Vector2)> Map = new();
    public Dictionary<string, LoadTile> Tiles = new();

    public List<(string, Vector2)> Cities = new();
    public int? selectedCity = null;
    public int startCity = 0;

    public ulong StartTimer = 0;

    public Entity Boat;
    bool ShowOpenMap = true;

    public MainScene(int width, int height, string id) : base(width, height, id)
    {
    }

    public override void Awake()
    {
        SetClearColor(140, 171, 161, 255);

        Islands = new();
        Map = new();
        Tiles = new();
        Cities = new();
        selectedCity = null;
        startCity = 0;
        ShowOpenMap = true;

        Boat = new Entity(this).Add(new BoatController());
        Camera.SetTarget(Boat);
        Camera.SetLag(100);
        RegisterEntity(Boat);
    
        for(var idx = 0; idx < 7; idx++)
        {
            var jsonMap = JsonConvert.DeserializeObject<List<List<Tile>>>(File.ReadAllText($"Maps/map{idx}.json"));

            Islands.Add(new());
            for(var i = 0; i < 16; i++)
            {
                for(var j = 0; j < 16; j++)
                {
                    if(jsonMap[i][j].TileID != null)
                    {
                        if(!Tiles.ContainsKey(jsonMap[i][j].TileID))
                        {
                            Tiles.Add(jsonMap[i][j].TileID, new LoadTile() {
                                tex = new Texture(jsonMap[i][j].TileID)
                            });
                        }
                        Islands[Islands.Count - 1].Add(new(i, j), jsonMap[i][j]);
                    }
                }
            }
        }

        var TownNames = File.ReadAllLines("TownNames");

        Random rand = new();

        Vector2 pos1 = new(110, 165);
        Map.Add((0, pos1));

        Islands[Map.Last().Item1].Where((e) => e.Value.IsCity).ToList().ForEach((e) => {
            Cities.Add((
                TownNames[rand.Next(TownNames.Length)],
                e.Key * 16 - pos1 
            ));
        });

        startCity = 1;

        for(var i = 0; i < 38; i++)
        {
            bool next = false;
            while(!next)
            {
                Vector2 pos = new(rand.Next(-3000, 3000), rand.Next(-3000, 3000));
                
                float closest = 999999999999999;

                foreach(var j in Map)
                {
                    if(Helpers.PointDistance(pos, j.Item2) < closest)
                    {
                        closest = Helpers.PointDistance(pos, j.Item2);
                    }
                }
                
                if(closest > 325)
                {
                    Map.Add((rand.Next(0, Islands.Count), pos));

                    // Islands[Islands.Count - 1].Where((e) => e.Value.IsCity).ToList().ForEach((e) => {
                    //     Cities.Add((e.Key))
                    // });

                    Islands[Map.Last().Item1].Where((e) => e.Value.IsCity).ToList().ForEach((e) => {
                        Cities.Add((
                            TownNames[rand.Next(TownNames.Length)],
                            e.Key * 16 - pos 
                        ));
                    });

                    next = true;
                }
            }
        }

        Debug.RegisterCommand("getislands", (args) => {
            foreach(var i in Map)
            {
                Debug.Log(i.Item2);
            }
        });

        Debug.RegisterCommand("getcity", (args) => {
            if(args.Length > 0)
            {
                Debug.Log(Cities.First((e) => e.Item1.ToString() == args[0]));
            }
        });
        
        Debug.RegisterCommand("getselectedcity", (args) => {
            Debug.Log(selectedCity.Value.ToString() + " " + Cities[selectedCity.Value].ToString());
        });

        selectedCity = rand.Next(Cities.Count);
        StartTimer = SDL_GetTicks64();

        // RegisterEntity(new Entity(this).Add(new DeliveryCompleted()));
    }

    public override void Sleep()
    {
        
    }

    public override void Update(double dt)
    {
        rectAngle += 10 * Game.GetDeltaTime();
    }

    public override void Render()
    {
        for(var j = 0; j < Map.Count; j++)
        {
            foreach(var i in Islands[Map[j].Item1])
            {
                if(Helpers.PointDistance(new(i.Key.X * 16 - Map[j].Item2.X, i.Key.Y * 16 - Map[j].Item2.Y), Camera.Offset) < WindowSize.X + 20) 
                {
                    Tiles[i.Value.TileID].tex
                        .Position(i.Key.X * 16 - Map[j].Item2.X, i.Key.Y * 16 - Map[j].Item2.Y)
                        .Depth(-(int)i.Key.Y * 16 - 32 + (int)Map[j].Item2.Y)
                        .Render();
                }
            }
        }

        int idx = -1;
        foreach(var i in Cities)
        {
            idx++;
            // Draw.Text(i.Item2, "Pixuf.ttf", i.Item1, 8, new(255, 255, 255, 255), 5000);
            if(Helpers.PointDistance(Camera.Offset, i.Item2) < WindowSize.X + 10)
            {
                new Text("Pixuf.ttf", i.Item1)
                    .Position(i.Item2)
                    .Size(8)
                    .Color(new Vector4(255, 255, 255, 255))
                    .Depth(5000)
                    .RenderToTexture()
                    .Destroy(true)
                    .Depth(5000)
                    .GetRect(out Vector4 rec)
                    .Render();

                if(idx == selectedCity)
                {
                    new Texture("Images/Marker.png")
                        .Center(Center.BottomMiddle)
                        .Position(Vector2.Add(i.Item2, new (rec.Z / 2, -3)))
                        .Destroy(false)
                        .Depth(4998)
                        .Render();
                }
            }
        }

        if(Helpers.PointDistance(Boat.Get<Transform>().Position, Cities[selectedCity.Value].Item2) < 50)
        {
            string value = "Press E to deliver package";
            Draw.Box(new(2 + Camera.Offset.X, WindowSize.Y - 10 + Camera.Offset.Y, value.Length * 4 + 4, 8), new(255, 255, 255, 255), 4999);
            LD.DrawSmallFont(new(4 + Camera.Offset.X, WindowSize.Y - 8 + Camera.Offset.Y), value);

            if(Keyboard.Pressed(Key.E))
            {
                Random rand = new();
                Boat.Get<BoatController>().Gold += 100;
                var newCity = rand.Next(Cities.Count);

                Debug.Log("Distance " + Helpers.PointDistance(Cities[selectedCity.Value].Item2, Cities[newCity].Item2));
                Debug.Log("Took " + (SDL_GetTicks64() - StartTimer));
                
                StartTimer = SDL_GetTicks64();
                selectedCity = newCity;

                RegisterEntity(new Entity(this).Add(new DeliveryCompleted()));
            }
        }
        else
        {
            string value = $"Deliver your package to${Cities[selectedCity.Value].Item1}";
            Draw.Box(new(2 + Camera.Offset.X, WindowSize.Y - 10 + Camera.Offset.Y, value.Length * 4 + 4, 8), new(235, 214, 190, 255), 4999);
            LD.DrawSmallFont(new(4 + Camera.Offset.X, WindowSize.Y - 8 + Camera.Offset.Y), value);
        }

        // new Texture("Images/Arrow.png")
        //     .Angle(Helpers.PointDirection(Boat.Get<Transform>().Position, Cities[selectedCity.Value].Item2))
        //     .Position(Vector2.Add(Boat.Get<Transform>().Position,  new(20, 0)))
        //     .Center(Center.Middle)
        //     .Render();

        // new Rectangle(new(Mouse.Position, 20, 20))
        //     .Color(new(231, 130, 132, 255))
        //     .Fill(true)
        //     .RenderToTexture()
        //     .Destroy(true)
        //     .Center(Center.Middle)
        //     .Render();

        // new Circle(Mouse.Position, 20)
        //     .Color(new(231, 130, 132, 255))
        //     .Fill(true)
        //     .RenderToTexture()
        //     .Destroy(true)
        //     .Center(Center.Middle)
        //     .Render();

        if(ShowOpenMap)
            Draw.Text(new(1 + Camera.Offset.X, 1 + Camera.Offset.Y), "Pixuf.ttf", "Press (M) To Open Map", 8, new(255, 255, 255, 255), 5000);
        if(Keyboard.Pressed(Key.M))
        {
            ShowOpenMap = false;
            // SceneHandler.Unload("Main");
            SceneHandler.Load("Map");
        }

        new Text("Pixuf.ttf", $"{Boat.Get<BoatController>().Gold}")
            .Color(new Vector4(255, 255, 255, 255))
            .Size(8)
            .RenderToTexture()
            .Destroy(true)
            .GetRect(out Vector4 rect)
            .Position(Vector2.Add(new(WindowSize.X - rect.Z - 2, 2), Camera.Offset))
            .Render();
        
        new Texture("Images/Gold.png")
            .Position(Vector2.Add(new(WindowSize.X - rect.Z - 8, 2), Camera.Offset))
            .Render();
            
    }
}

