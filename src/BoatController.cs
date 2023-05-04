namespace LD53;

class BoatController : Component
{
    // Texture t = new("Images/Boat/boat_00.png");
    Transform transform;
    BoatRenderer br;
    public int Gold = 0;
    MainScene main;

    List<Vector3> v = new();

    public override void Awake()
    {
        transform = Get<Transform>();
        br = Get<BoatRenderer>();
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

        float mSpeed = 70; // 70

        if(Keyboard.Down(Key.A))
        {
            br.Angle -= 100f * (float)dt;
        }

        if(Keyboard.Down(Key.D))
        {
            br.Angle += 100f * (float)dt;
        }

        if(Keyboard.Down(Key.W))
        {
            transform.Position += Helpers.LengthDir(mSpeed * dt, br.Angle);

            Random rand = new();
            if(rand.NextDouble() > 0.98)
                br.v.Add(new Vector3(transform.Position + Helpers.LengthDir(10, br.Angle + 180 + ((rand.NextDouble() - 0.5) * 50)), 1));
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
                    transform.Position += Helpers.LengthDir(1, br.Angle + 180);
                }
            }
        }
    }

    public override void Render()
    {

    }
}