namespace SpaceRocks;

public partial class Bullet : ModelEntity
{
	public Bullet() { }

	public Bullet( Vector3 position, Vector3 velocity)
	{
		Position = position;
		PhysicsBody.Velocity = velocity;
	}

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/bullet.vmdl" );
		GameMgr.SetBaseModelShit( this );

		DeleteAsync( 1.25f );
	}
}
