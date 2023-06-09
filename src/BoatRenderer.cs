namespace LD53;

class BoatRenderer : Component
{
    // Texture t = new("Images/Boat/boat_00.png");
    Texture[] t = new Texture[13];
    Transform transform;
    public double Dt = 0;
    MainScene main;

    public List<Vector3> v = new();

    public override void Awake()
    {
        transform = Get<Transform>();

        for(var i = 0; i < t.Length; i++)
        {
            t[i] =  new($"Images/Boat/boat_{(i.ToString().Length > 1 ? i : "0" + i.ToString())}.png");
            t[i].Center(Center.Middle);
        }

        main = SceneHandler.Get<MainScene>();
    }

    public override void Sleep()
    {

    }

    public override void Update()
    {
        for(var i = 0; i < t.Length; i++)
        {
            if(i < t.Length - 6 || Keyboard.Down(Key.W))
            {
                t[i].Position(Vector2.Subtract(transform.Position, new(0, i))).Angle(transform.Angle).Depth(-(int)transform.Position.Y).Render();
            } else 
            {
                t[i].Position(
                    Vector2.Add(
                        Vector2.Subtract(
                            transform.Position, 
                            new(0, i)
                        ),
                        Helpers.LengthDir((MathF.Sin((SDL_GetTicks64() / 200) + (ulong)(i)) - 0.5) * 1, 45)
                        
                    )
                )
                .Angle(transform.Angle)
                .Depth(-(int)transform.Position.Y)
                .Render();
            }
        }

        List<int> PopList = new();

        for(var i = 0; i < v.Count; i++)
        {
            var circ = v[i];
            new Circle(new(circ.X, circ.Y), 5)
                .Color(new(255, 255, 255, 255))
                .Fill(true)
                .Depth((int)-(int)circ.Y - 32)
                .Render();
        
            new Circle(new(circ.X, circ.Y), circ.Z)
                .Color(new(140, 171, 161, 255))
                .Fill(true)
                .Depth(-(int)circ.Y - 32)
                .Render();

            v[i] = Vector3.Add(v[i], new(0, 0, 15 * DeltaTime));

            if(v[i].Z > 5)
                PopList.Add(i);
        }

        PopList.Sort();
        PopList.Reverse();

        foreach(var i in PopList)
        {
            v.RemoveAt(i);
        }
    }
}