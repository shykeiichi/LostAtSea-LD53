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

    public override void Update()
    {
        transform.Position = ParentScene.WindowSize / 2;

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