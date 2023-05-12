namespace LD53;

class CannonBallController : Component
{
    float angle, speed;
    Vector2 pos;
    ulong spawned;

    Transform transform;
    
    Texture t = new("Images/CannonBall/CannonBall_0.png");

    public CannonBallController(Vector2 position, float angle, float speed)
    {
        this.pos = position;
        this.angle = angle;
        this.speed = speed;
    }

    public override void Awake()
    {
        transform = Get<Transform>();
        transform.Position = pos;
        spawned = SDL_GetTicks64();
    }

    public override void Update()
    {
        transform.Position += Helpers.LengthDir(speed * DeltaTime, angle);

        t.Position(transform.Position).Depth(-(int)transform.Position.Y - 8).Render();

        if(SDL_GetTicks64() - spawned > 10000)
        {
            ParentScene.RemoveEntity(ParentEntity);
        }
    }
}

