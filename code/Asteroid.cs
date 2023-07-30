namespace SpaceRocks;

public partial class Asteroid : ModelEntity
{
	public static Action<Asteroid> AsteroidExploded;

	public enum AsteroidSize
	{ 
		Large,
		Medium,
		Small
	}
	public AsteroidSize Size { get; private set; } = AsteroidSize.Large;

	public Asteroid() { Initiate(Vector3.Zero, AsteroidSize.Large); }

	public Asteroid( Vector3 position, AsteroidSize size ) { Initiate( position, size ); }

	private void Initiate(Vector3 position, AsteroidSize size)
	{
		if ( !Game.IsServer )
			return;

		Position = position;
		Size = size;

		SetModel( GetAsteroidModel() );
		GameMgr.SetBaseModelShit( this );

		PhysicsBody.Velocity = new Vector3( Game.Random.Float( -500, 500 ), Game.Random.Float( -500, 500 ), 0 );
		PhysicsBody.AngularVelocity = new Vector3( 0, 0, Game.Random.Float( -5, 5 ) );
	}

	public override void StartTouch( Entity other )
	{
		if ( !Game.IsServer )
			return;

		if ( other is Bullet )
		{
			BlowTheFuckUp();
			other.Delete();
		} 
		else if ( other is Ship ship )
		{
			ship.TakeDamage( DamageInfo.Generic( 1000 ) );
			BlowTheFuckUp();
		}
	}

	public void BlowTheFuckUp()
	{
		if ( Size != AsteroidSize.Small )
		{
			for ( int i = 0; i < 3; i++ )
			{
				AsteroidSize babySize = AsteroidSize.Large;

				if ( Size == AsteroidSize.Large )
				{
					babySize = AsteroidSize.Medium;
					GameMgr.CreateEffects( 2, 1.0f, Position );
				}
				else if ( Size == AsteroidSize.Medium )
				{
					babySize = AsteroidSize.Small;
					GameMgr.CreateEffects( 1, 0.5f, Position );
				}

				new Asteroid( Position, babySize );
			}
		}

		GameMgr.CreateEffects( 2, 0.25f, Position );

		Sound.FromScreen( GetBangSound() );

		AsteroidExploded?.Invoke(this);

		Delete();
	}

	readonly string[] models_large =
	{
		"models/asteroid_01.vmdl",
		"models/asteroid_02.vmdl",
		"models/asteroid_03.vmdl",
	};

	readonly string[] models_medium =
	{
		"models/asteroid_01_m.vmdl",
		"models/asteroid_02_m.vmdl",
		"models/asteroid_03_m.vmdl",
	};

	readonly string[] models_small =
	{
		"models/asteroid_01_s.vmdl",
		"models/asteroid_02_s.vmdl",
		"models/asteroid_03_s.vmdl",
	};

	string GetAsteroidModel()
	{
		if ( Size == AsteroidSize.Large )
			return models_large[Game.Random.Int( 0, models_large.Length - 1 )];
		else if ( Size == AsteroidSize.Medium )
			return models_medium[Game.Random.Int( 0, models_medium.Length - 1 )];
		else if ( Size == AsteroidSize.Small )
			return models_small[Game.Random.Int( 0, models_small.Length - 1 )];
		else
			return null;
	}

	string GetBangSound()
	{
		if ( Size == AsteroidSize.Large )
			return "sounds/banglarge.sound";
		else if ( Size == AsteroidSize.Medium )
			return "sounds/bangmedium.sound";
		else if ( Size == AsteroidSize.Small )
			return "sounds/bangsmall.sound";
		else
			return null;
	}
}
