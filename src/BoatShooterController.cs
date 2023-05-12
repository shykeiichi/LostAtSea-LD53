namespace LD53;

class BoatShooterController : Component
{
    Transform transform;

    public override void Awake()
    {
        transform = Get<Transform>();
    }

    public void Shoot()
    {
        ParentScene.RegisterEntity(new Entity(ParentScene).ExcludeFromInspector(true).Add(new CannonBallController(transform.Position, transform.Angle + 85, 100f)));
        ParentScene.RegisterEntity(new Entity(ParentScene).ExcludeFromInspector(true).Add(new CannonBallController(transform.Position, transform.Angle + 95, 100f)));

        ParentScene.RegisterEntity(new Entity(ParentScene).ExcludeFromInspector(true).Add(new CannonBallController(transform.Position, transform.Angle + 265, 100f)));
        ParentScene.RegisterEntity(new Entity(ParentScene).ExcludeFromInspector(true).Add(new CannonBallController(transform.Position, transform.Angle + 275, 100f)));
    }   
}