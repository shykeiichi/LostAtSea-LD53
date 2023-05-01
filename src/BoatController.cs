namespace LD53;

class BoatController : Component
{
    // Texture t = new("Images/Boat/boat_00.png");
    Texture[] t = new Texture[13];
    Transform transform;
    float Angle = 0;
    double Dt = 0;
    public int Gold = 0;
    MainScene main;

    List<Vector3> v = new();

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

    public override void Update(double dt)
    {
        Debug.RegisterCommand("getpos", (args) => {
            Debug.Log(transform.Position);
        });

        Dt = dt;

        float mSpeed = 70; // 70

        if(Keyboard.Down(Key.A))
        {
            Angle -= 100f * (float)dt;
        }

        if(Keyboard.Down(Key.D))
        {
            Angle += 100f * (float)dt;
        }

        if(Keyboard.Down(Key.W))
        {
            transform.Position += Helpers.LengthDir(mSpeed * dt, Angle);

            Random rand = new();
            if(rand.NextDouble() > 0.98)
                v.Add(new Vector3(transform.Position + Helpers.LengthDir(10, Angle + 180 + ((rand.NextDouble() - 0.5) * 50)), 1));
        }

        // if(Mouse.Pressed(MB.Left))
        // {
        //     ParentScene.RegisterEntity(new Entity(ParentScene).Add(new CannonBallController(transform.Position, Angle + 75, 100f)));
        //     ParentScene.RegisterEntity(new Entity(ParentScene).Add(new CannonBallController(transform.Position, Angle + 105, 100f)));

        //     ParentScene.RegisterEntity(new Entity(ParentScene).Add(new CannonBallController(transform.Position, Angle + 255, 100f)));
        //     ParentScene.RegisterEntity(new Entity(ParentScene).Add(new CannonBallController(transform.Position, Angle + 285, 100f)));
        // }

        if(transform.Position.X < -3200 || transform.Position.X > 3200 || transform.Position.Y < -3200 || transform.Position.Y > 3200)
        {
            SceneHandler.Unload("Main");
            SceneHandler.Load("LostAtSea");
        }

        for(var j = 0; j < main.Map.Count; j++)
        {
            foreach(var i in main.Islands[main.Map[j].Item1])
            {
                if(Helpers.PointInside(transform.Position, new Vector4(i.Key.X * 16 - main.Map[j].Item2.X, i.Key.Y * 16 - main.Map[j].Item2.Y, 16, 16)))
                {
                    transform.Position += Helpers.LengthDir(1, Angle + 180);
                }
            }
        }
    }

    public override void Render()
    {
        for(var i = 0; i < t.Length; i++)
        {
            t[i].Position(Vector2.Subtract(transform.Position, new(0, i))).Angle(Angle).Depth(-(int)transform.Position.Y).Render();
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

            v[i] = Vector3.Add(v[i], new(0, 0, 15 * (float)Dt));

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