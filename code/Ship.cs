namespace SpaceRocks;

partial class Ship : ModelEntity
{
	public static Action<Ship> ShipExploded;

	[ClientInput] public Vector3 MoveDirection { get; set; }
	
	public float CameraDistance { get; set; } = 2500f;

	///////////////////////

	public TimeSince TimeSinceShoot { get; private set; }
	public TimeSince TimeSinceThrust { get; private set; }


	public void Respawn()
	{
		Health = 100;
		Position = 0;
		SetModel( "models/ship.vmdl" );
		GameMgr.SetBaseModelShit( this );

		PhysicsBody.LinearDamping = 1.5f;
		PhysicsBody.AngularDamping = 8.0f;

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	public void Move(float forward, float left)
	{
		// Thrust go brrrr
		Vector3 thrust = PhysicsBody.Transform.Rotation.Backward * MathX.Clamp( forward, 0, 1 ) * 4000 * 10000;
		Vector3 torque = PhysicsBody.Transform.Rotation.Up * left * 6000 * 400000;

		PhysicsBody.ApplyForce( thrust * Time.Delta );
		PhysicsBody.ApplyTorque( torque * Time.Delta );

		// Thruster "effects"
		if ( thrust != Vector3.Zero )
		{
			SetBodyGroup( "ship", 0 );
			if ( TimeSinceThrust < 0.1f )
				return;
			SetBodyGroup( "ship", 1 );
			TimeSinceThrust = 0;
			Sound.FromScreen( "sounds/thrust.sound" );
		}
		else
		{
			SetBodyGroup( "ship", 0 );
		}
	}

	public void Shoot()
	{
		if ( TimeSinceShoot < 0.2f )
			return;
		TimeSinceShoot = 0;

		if ( Game.IsServer )
			new Bullet( Position + Rotation.Backward * 50, Rotation.Backward * 2000 );

		Sound.FromScreen( "sounds/fire.sound" );
	}

	public override void OnKilled()
	{
		base.OnKilled();

		EnableAllCollisions = false;
		EnableDrawing = false;

		GameMgr.CreateEffects( 3, 2.0f, Position, "models/line.vmdl" );
		GameMgr.CreateEffects( 2, 1.0f, Position, "models/line.vmdl" );

		Sound.FromScreen( "sounds/banglarge.sound" );

		ShipExploded?.Invoke(this);
	}

	public override void Simulate( IClient cl )
	{
		if ( Health <= 0 )
			return;

		Move( MoveDirection.x, MoveDirection.y );

		if ( Input.Pressed( "jump" ) )
			Shoot();
	}

	public override void BuildInput()
	{
		base.BuildInput();

		MoveDirection = Input.AnalogMove;
	}

	public override void FrameSimulate( IClient cl )
	{
		Camera.Position = Vector3.Up * CameraDistance;
		Camera.Rotation = Rotation.From( 90, 0, 0 );

		Camera.FieldOfView = 90;

		Camera.FirstPersonViewer = null;
	}
}
