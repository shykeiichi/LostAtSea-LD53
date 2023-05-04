namespace LD53;

class PirateController : Component
{
    Transform transform;
    MainScene main;
    float shoot = 0;

    public override void Awake()
    {
        transform = Get<Transform>();
        var rand = new Random();
        transform.Position = Vector2.Add(transform.Position, new(((float)rand.NextDouble() - 0.5f) * 100, ((float)rand.NextDouble() - 0.5f) * 100));
        main = SceneHandler.Get<MainScene>();
    }

    public override void Sleep()
    {

    }

    public override void Update(double dt)
    {
        Entity player = main.Entities.First((e) => e.TryGet<PlayerController>(out PlayerController c));
        float dist = Helpers.PointDistance(
            player.Get<Transform>().Position,
            transform.Position
        );

        if(dist < 50)
        {
            float angleDiff = Helpers.AngleDifference(
                Helpers.PointDirection(transform.Position, player.Get<Transform>().Position) + 90,
                transform.Angle
            );
            if(main.Entities.Min((e) =>
                Helpers.PointDistance(
                    Helpers.LengthDir(30f * (float)dt, transform.Angle - angleDiff * (float)dt),
                    e.Get<Transform>().Position
                )
            ) < 50)
            {
                transform.Angle -= (angleDiff * (float)dt) * 4;
            }
            else
            {
                transform.Angle -= angleDiff * (float)dt;
            }
            

            if(Math.Abs(angleDiff) < 15)
            {
                Console.WriteLine(shoot);
                shoot += 10f * (float)dt;
                if(shoot > 30)
                {
                    shoot = 0;
                    Get<BoatShooterController>().Shoot();
                }
            }
        }
        else if(dist < 150)
        {
            transform.Angle -= Helpers.AngleDifference(
                Helpers.PointDirection(transform.Position, player.Get<Transform>().Position),
                transform.Angle
            ) * (float)dt;
            transform.Position += Helpers.LengthDir(30f * (float)dt, transform.Angle);
        }
    }

    public override void Render()
    {
        
    }
}