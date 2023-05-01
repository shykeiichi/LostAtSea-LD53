namespace LD53;

class CannonBallController : Component
{
    float angle, speed;
    Vector2 pos;

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
    }

    public override void Update(double dt)
    {
        transform.Position += Helpers.LengthDir(speed * (float)dt, angle);
    }

    public override void Render()
    {
        t.Position(transform.Position).Depth(-(int)transform.Position.Y - 8).Render();
    }
}

