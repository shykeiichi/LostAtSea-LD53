using System.Numerics;
using Fjord;
using Fjord.Graphics;
using Fjord.Input;
using Fjord.Scenes;
using Fjord.Ui;
using static SDL2.SDL;
using Newtonsoft.Json;

namespace LD53;

class Tile
{
    public string TileID = "";
    public bool IsCity = false;
}

struct FileFormat
{
    public List<List<Tile>> Map;
}

class Editor : Scene
{
    EditorInspector inspector;
    
    public Dictionary<Vector2, Tile> Tilemap = new();

    public Editor(int width, int height, string id) : base(width, height, id)
    {
    }

    public override void Awake()
    {
        inspector = SceneHandler.Get<EditorInspector>();

        SetClearColor(255, 251, 254, 255);
    }

    public override void Update(double dt)
    {

    }

    public override void Render()
    {
        for(var w = 0; w < 16; w++)
        {
            for(var h = 0; h < 16; h++)
            {
                Draw.Rectangle(new(w * 64, h * 64, 64, 64), new(0, 0, 0, 255));

                if(Tilemap.ContainsKey(new(w, h)))
                {
                    if(Tilemap[new(w, h)].TileID != "")
                    {
                        inspector.Tiles[Tilemap[new(w, h)].TileID].tex
                            .Size(64, 64)
                            .Position(new(w * 64, h * 64))
                            .Alpha(255)
                            .Center(Center.TopLeft)
                            .Render();
                    }
                    if(Tilemap[new(w, h)].IsCity)
                    {
                        new Rectangle(new(w * 64, h * 64, 64, 64))
                            .Fill(true)
                            .Color(new(255, 0, 0, 20))
                            .Render();
                    }
                }

                if(Mouse.Down(MB.Left) && Helpers.PointInside(Mouse.Position, new Vector4(w * 64, h * 64, 64, 64)) && inspector.SelectedTile != null)
                {
                    if(Tilemap.ContainsKey(new(w, h))) 
                    {
                        Tilemap[new(w, h)] = new() {
                            TileID = inspector.SelectedTile,
                            IsCity = Tilemap[new(w, h)].IsCity
                        };
                    } else {
                        Tilemap.Add(new(w, h), new() {
                            TileID = inspector.SelectedTile
                        });
                    }
                }

                if(Mouse.Down(MB.Right) && Helpers.PointInside(Mouse.Position, new Vector4(w * 64, h * 64, 64, 64)))
                {
                    Tilemap.Remove(new(w, h));
                }

                if(Mouse.Pressed(MB.Left) && Helpers.PointInside(Mouse.Position, new Vector4(w * 64, h * 64, 64, 64)) && inspector.SelectedTile == null)
                {
                    if(Tilemap.ContainsKey(new(w, h))) 
                    {
                        Tilemap[new(w, h)] = new() {
                            TileID = Tilemap[new(w, h)].TileID,
                            IsCity = !Tilemap[new(w, h)].IsCity
                        };
                    } else {
                        Tilemap.Add(new(w, h), new() {
                            TileID = "",
                            IsCity = true
                        });
                    }
                }
            }
        }

        if(inspector.SelectedTile != null)
            inspector.Tiles[inspector.SelectedTile].tex.Position( Mouse.Position ).Alpha(100).Center(Center.Middle).Render();
    }
}

struct LoadTile
{
    public Texture tex;
}

class EditorInspector : Scene
{
    public Dictionary<string, LoadTile> Tiles = new();

    public string LoadTileName = "";
    public string LoadTilePath = "";
    
    public string LoadTileFolderPath = "";

    public string AddTileError = "";
    public string AddTilesError = "";

    public string? SelectedTile = null;

    public string SavePath = "";
    public string LoadPath = "";

    public EditorInspector(int width, int height, string id) : base(width, height, id)
    {
    }

    public override void Awake()
    {
        SetClearColor(UiColors.Background);
    }

    public override void Render()
    {
        new UiBuilder(new Vector2(0, 0), Mouse.Position)
            .Container(
                "Tiles",
                new UiBuilder(new Vector2(0, 0), Mouse.Position)
                    .Title(
                        SelectedTile is not null ?
                            "Selected Tile: " + SelectedTile
                        :
                            "No Tile Selected"
                    )
                    .Spacer()
                    .ForEach(Tiles.ToList(), (e) => {
                        return new UiBuilder(new Vector2(0, 0), Mouse.Position)
                            .Title(e.Key)
                            .Image(e.Value.tex)
                            .Button("Select", () => {
                                SelectedTile = e.Key;
                            })
                            .BuildHAlign();
                    })
                    .Build()
            )
            .Container(
                "Mark Towns",
                new UiBuilder()
                    .Button("Equip town marker", () => {
                        SelectedTile = null;
                    })
                    .Build()
            )
            .Container(
                "Load Tile",
                new UiBuilder(new Vector2(0, 0), Mouse.Position)
                    .HAlign(
                        new UiBuilder(new Vector2(0, 0), Mouse.Position)
                            .TextField("ltname", LoadTileName, (e) => LoadTileName = e, (e) => {})
                            .Title("Name")
                            .BuildHAlign()
                    )
                    .HAlign(
                        new UiBuilder(new Vector2(0, 0), Mouse.Position)
                            .TextField("ltpath", LoadTilePath, (e) => LoadTilePath = e, (e) => {})
                            .Title("Path")
                            .BuildHAlign()
                    )
                    .Button("Add Tile", () => {
                        if(LoadTileName == "")
                        {
                            AddTileError = "Tile Name must be set!";
                        }
                        if(LoadTilePath == "")
                        {
                            AddTileError = "Tile Path must be set!";
                        }

                        try 
                        {
                            Tiles.Add(LoadTileName, new LoadTile() {
                                tex = new(LoadTilePath)
                            });

                            AddTileError = "";
                        } catch(Exception e)
                        {
                            AddTileError = e.Message;
                        }
                    })
                    .Text(AddTileError)
                    .Build()
            )
            .Container(
                "Load Tiles",
                new UiBuilder(new Vector2(0, 0), Mouse.Position)
                    .HAlign(
                        new UiBuilder(new Vector2(0, 0), Mouse.Position)
                            .TextField("ltfpath", LoadTileFolderPath, (e) => LoadTileFolderPath = e, (e) => {})
                            .Title("Path")
                            .BuildHAlign()
                    )
                    .Button("Add Tiles", () => {
                        if(LoadTileFolderPath == "")
                        {
                            AddTilesError = "Tile Path must be set!";
                        }

                        try 
                        {
                            foreach(var file in Directory.GetFiles(LoadTileFolderPath)) 
                            {
                                if(Tiles.ContainsKey(file))
                                {
                                    continue;
                                }

                                Tiles.Add(file, new LoadTile() {
                                    tex = new(file)
                                });

                                AddTilesError = "";
                            }
                        } catch(Exception e)
                        {
                            AddTilesError = e.Message;
                        }
                    })
                    .Text(AddTilesError)
                    .Build()
            )
            .Container(
                "Save & Load",
                new UiBuilder()
                    .Title("Load")
                    .HAlign(
                        new UiBuilder()
                            .TextField("loadpath", LoadPath, (e) => LoadPath = e, (e) => {})
                            .Title("Path")
                            .BuildHAlign()
                    )
                    .Button("Load", () => {
                        var jsonMap = JsonConvert.DeserializeObject<List<List<Tile>>>(File.ReadAllText(LoadPath));

                        if(jsonMap != null)
                        {
                            for(var i = 0; i < 16; i++)
                            {
                                for(var j = 0; j < 16; j++)
                                {
                                    if(jsonMap[i][j].TileID != "")
                                    {
                                        if(!Tiles.ContainsKey(jsonMap[i][j].TileID))
                                        {
                                            Tiles.Add(jsonMap[i][j].TileID, new LoadTile() {
                                                tex = new Texture(jsonMap[i][j].TileID)
                                            });
                                        }
                                        SceneHandler.Get<Editor>().Tilemap.Add(new(i, j), jsonMap[i][j]);
                                    }
                                }
                            }
                        }
                    })
                    .Spacer()
                    .Title("Save")
                    .HAlign(
                        new UiBuilder()
                            .TextField("savepath", SavePath, (e) => SavePath = e, (e) => {})
                            .Title("Path")
                            .BuildHAlign()
                    )
                    .Button("Save", () => {
                        List<List<Tile>> Map = new();

                        for(var i = 0; i < 16; i++)
                        {
                            Map.Add(new());
                            for(var j = 0; j < 16; j++)
                            {
                                Map[i].Add(new());
                            }
                        }

                        foreach(var i in SceneHandler.Get<Editor>().Tilemap)
                        {
                            Map[(int)i.Key.X][(int)i.Key.Y] = i.Value;
                        }

                        if(File.Exists(SavePath))
                        {
                            if(File.Exists($"{SavePath}.backup"))
                            {
                                File.Delete($"{SavePath}.backup");
                            }
                            File.Copy(SavePath, $"{SavePath}.backup");
                        }

                        File.WriteAllText($"{SavePath}", JsonConvert.SerializeObject(Map));
                        SavePath = "";
                    })
                    .Build()
            )
            .Render();
    }
}



