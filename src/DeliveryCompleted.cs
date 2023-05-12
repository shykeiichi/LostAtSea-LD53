namespace LD53;

public class DeliveryCompleted : Component
{
    Transform transform;
    ulong Spawned = 0;

    public override void Awake()
    {
        transform = Get<Transform>();
        Spawned = SDL_GetTicks64();
    }

    float easeOutCubic(float x) {
        return (float)(1 - Math.Pow(1 - x, 3));
    }

    public override void Update()
    {

        if(SDL_GetTicks64() - Spawned < 1500)
        {
            transform.Position = new(
                (ParentScene.WindowSize.X / 2 + 100) * easeOutCubic(((float)SDL_GetTicks64() - (float)Spawned) / 1500f) - 100,
                ParentScene.WindowSize.Y / 2
            );
        } else if(SDL_GetTicks64() - Spawned > 2500)
        {
            transform.Position = new(
                (ParentScene.WindowSize.X / 2 + 100) * Math.Abs(1 - easeOutCubic(((float)SDL_GetTicks64() - ((float)Spawned + 2500f)) / 1500f)) - 100,
                ParentScene.WindowSize.Y / 2
            );
        } else 
        {
            transform.Position = new(
                (ParentScene.WindowSize.X / 2 + 100) - 100,
                ParentScene.WindowSize.Y / 2
            );
        }

        if(SDL_GetTicks64() - Spawned > 3000)
        {
            ParentScene.RemoveEntity(ParentEntity);
        }

        new Texture("Images/DeliveryCompleted.png")
            .Center(Center.Middle)
            .Position(transform.Position + ParentScene.Camera.Offset)
            .Destroy(false)
            .Depth(20000)
            .Render();
    }
}