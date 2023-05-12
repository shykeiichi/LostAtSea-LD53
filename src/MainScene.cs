namespace LD53;

class MainScene : Scene
{
    public List<Dictionary<Vector2, Tile>> Islands = new();
    public List<(int, Vector2)> Map = new();
    public Dictionary<string, LoadTile> Tiles = new();

    public List<(string, Vector2)> Cities = new();
    public int? selectedCity = null;
    public int startCity = 0;

    public List<Entity> Pirates = new();

    public ulong StartTimer = 0;

    public Entity Boat;

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

        Boat = new Entity(this).Name("Player").Add(new BoatRenderer()).Add(new BoatShooterController()).Add(new PlayerController());
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
                    if(jsonMap != null)
                    {
                        if(jsonMap[i][j].TileID != "")
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
                Vector2 pos = new(rand.Next(-2500, 2500), rand.Next(-2500, 2500));
                
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

        // Pirates.Add(new Entity(this).Add(new BoatRenderer()).Add(new BoatShooterController()).Add(new PirateController()));
        // RegisterEntity(Pirates.Last());
        // Pirates.Add(new Entity(this).Add(new BoatRenderer()).Add(new BoatShooterController()).Add(new PirateController()));
        // RegisterEntity(Pirates.Last());
        // Pirates.Add(new Entity(this).Add(new BoatRenderer()).Add(new BoatShooterController()).Add(new PirateController()));
        // RegisterEntity(Pirates.Last());

        // RegisterEntity(new Entity(this).Add(new DeliveryCompleted()));
    }

    public override void Sleep()
    {
        
    }

    public override void Update()
    {
        for(var j = 0; j < Map.Count; j++)
        {
            foreach(var i in Islands[Map[j].Item1])
            {
                if(Helpers.PointDistance(new(i.Key.X * 16 - Map[j].Item2.X, i.Key.Y * 16 - Map[j].Item2.Y), Camera.Offset) < WindowSize.X + 64) 
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
    }
}

class HUD : Scene
{
    MainScene main;

    Texture BWOff = new("Images/HUD/ButtonW0.png");
    Texture BWOn = new("Images/HUD/ButtonW1.png");

    Texture BAOff = new("Images/HUD/ButtonA0.png");
    Texture BAOn = new("Images/HUD/ButtonA1.png");

    Texture BDOff = new("Images/HUD/ButtonD0.png");
    Texture BDOn = new("Images/HUD/ButtonD1.png");

    Texture BEOn = new("Images/HUD/ButtonE1.png");

    public Texture BMOff = new("Images/HUD/ButtonM0.png");
    public Texture BMOn = new("Images/HUD/ButtonM1.png");

    public Texture BESCOff = new("Images/HUD/ButtonESC0.png");
    public Texture BESCOn = new("Images/HUD/ButtonESC1.png");

    int BMPressed = 0;
    int BWPressed = 0;
    int BAPressed = 0;
    int BDPressed = 0;

    public HUD(int width, int height, string id) : base(width, height, id)
    {
    }

    public override void Awake()
    {
        main = SceneHandler.Get<MainScene>();
        SetClearColor(0, 0, 0, 0);
        SetCaptureMouse(false);
    }

    public override void Update()
    {
        if(Helpers.PointDistance(main.Boat.Get<Transform>().Position, main.Cities[main.selectedCity.Value].Item2) < 50)
        {
            BEOn.Position(new(WindowSize.X / 2, WindowSize.Y - 16)).Center(Center.BottomMiddle).Depth(30000).Render();

            new Text("Pixuf.ttf", "Deliver Package")
                .Size(8)
                .Color(new Vector4(255, 255, 255, 255))
                .RenderToTexture()
                .Destroy(true)
                .Center(Center.BottomRight)
                .Depth(30000)
                .Position(new(WindowSize.X / 2 - 6, WindowSize.Y - 18))
                .Render();

            if(Keyboard.Pressed(Key.E))
            {
                Random rand = new();
                main.Boat.Get<PlayerController>().Gold += 100;
                var newCity = rand.Next(main.Cities.Count);

                Debug.Log("Distance " + Helpers.PointDistance(main.Cities[main.selectedCity.Value].Item2, main.Cities[newCity].Item2));
                Debug.Log("Took " + (SDL_GetTicks64() - main.StartTimer));
                
                main.StartTimer = SDL_GetTicks64();
                main.selectedCity = newCity;

                RegisterEntity(new Entity(this).Add(new DeliveryCompleted()));
            }
        }
        
        string value = $"Deliver your package to${main.Cities[main.selectedCity.Value].Item1}";
        Draw.Box(new(2, WindowSize.Y - 10, value.Length * 4 + 4, 8), new(235, 214, 190, 255), 4999);
        LD.DrawSmallFont(new(4, WindowSize.Y - 8), value);

        if(Keyboard.Pressed(Key.M))
        {
            SceneHandler.Load("Map");
        }   

        new Text("Pixuf.ttf", $"{SceneHandler.Get<MainScene>().Boat.Get<PlayerController>().Gold}")
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
        
        if(GlobalKeyboard.Pressed(Key.W))
            BWPressed++;

        if(GlobalKeyboard.Pressed(Key.A))
            BAPressed++;

        if(GlobalKeyboard.Pressed(Key.D))
            BDPressed++;

        if((BWPressed >= 1) && (BAPressed >= 1) && (BDPressed >= 1))
            if(GlobalKeyboard.Pressed(Key.M))
                BMPressed++;

        if((BWPressed < 1) || (BAPressed < 1) || (BDPressed < 1))
        {    
            if(!GlobalKeyboard.Down(Key.W))
                BWOn.Position(new(WindowSize.X / 2, WindowSize.Y - 32)).Center(Center.BottomMiddle).Depth(30000).Render();
            else
                BWOff.Position(new(WindowSize.X / 2, WindowSize.Y - 32)).Center(Center.BottomMiddle).Depth(30000).Render();

            new Text("Pixuf.ttf", "Forward")
                .Size(8)
                .Color(new Vector4(255, 255, 255, 255))
                .RenderToTexture()
                .Destroy(true)
                .Center(Center.BottomRight)
                .Position(new(WindowSize.X / 2 - 6, WindowSize.Y - 34))
                .Depth(30000)
                .Render();

            if(!GlobalKeyboard.Down(Key.A))
                BAOn.Position(new(WindowSize.X / 2 - 13, WindowSize.Y - 16)).Center(Center.BottomMiddle).Depth(30000).Render();
            else
                BAOff.Position(new(WindowSize.X / 2 - 13, WindowSize.Y - 16)).Center(Center.BottomMiddle).Depth(30000).Render();

            new Text("Pixuf.ttf", "Turn Left")
                .Size(8)
                .Color(new Vector4(255, 255, 255, 255))
                .RenderToTexture()
                .Destroy(true)
                .Center(Center.BottomRight)
                .Position(new(WindowSize.X / 2 - 19, WindowSize.Y - 18))
                .Depth(30000)
                .Render();

            if(!GlobalKeyboard.Down(Key.D))
                BDOn.Position(new(WindowSize.X / 2 + 13, WindowSize.Y - 16)).Center(Center.BottomMiddle).Depth(30000).Render();
            else
                BDOff.Position(new(WindowSize.X / 2 + 13, WindowSize.Y - 16)).Center(Center.BottomMiddle).Depth(30000).Render();

            new Text("Pixuf.ttf", "Turn Right")
                .Size(8)
                .Color(new Vector4(255, 255, 255, 255))
                .RenderToTexture()
                .Destroy(true)
                .Center(Center.BottomLeft)
                .Position(new(WindowSize.X / 2 + 20, WindowSize.Y - 18))
                .Depth(30000)
                .Render();

        } else if(BMPressed < 1) {
            if(!GlobalKeyboard.Down(Key.M))
                BMOn.Position(new(WindowSize.X / 2, WindowSize.Y - 16)).Center(Center.BottomMiddle).Depth(30000).Render();
            else
                BMOff.Position(new(WindowSize.X / 2, WindowSize.Y - 16)).Center(Center.BottomMiddle).Depth(30000).Render();

            new Text("Pixuf.ttf", "Open Map")
                .Size(8)
                .Color(new Vector4(255, 255, 255, 255))
                .RenderToTexture()
                .Destroy(true)
                .Center(Center.BottomRight)
                .Depth(30000)
                .Position(new(WindowSize.X / 2 - 6, WindowSize.Y - 18))
                .Render();
        }
    }
}