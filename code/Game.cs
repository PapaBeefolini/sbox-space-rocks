global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using System;
global using System.Linq;
global using System.Threading.Tasks;

namespace SpaceRocks;

public partial class GameMgr : GameManager
{
	public static GameMgr Instance
	{
		get => Current as GameMgr;
	}

	[Net] public Vector2 PlaySpace { get; private set; } = new( 1700, 2200 );
	[Net] public Color GameColor { get; private set; } = Color.White;
	[Net] public int GameScore { get; private set; } = 0;
	[Net] public int GameHighScore { get; private set; } = 0;
	[Net] public int GameLives { get; private set; } = 3;

	public GameMgr()
	{
		if ( Game.IsServer )
		{
			GameColor = Color.Random;
			new UI.GameUI();

			// World Border
			ModelEntity border = new ModelEntity();
			border.SetModel( "models/border.vmdl" );
			border.Position = new Vector3( -1528, 0, 8 );
			border.Rotation = Rotation.FromAxis( Vector3.Up, 270 );
			border.RenderColor = GameColor;

			Asteroid.AsteroidExploded += AsteroidExploded;
			Ship.ShipExploded += ShipExploded;

			Game.PhysicsWorld.Gravity = 0;

			NewGame();
		}
	}

	private void NewGame()
	{
		if ( !Game.IsServer )
			return;

		var bullets = All.OfType<Bullet>();
		foreach ( Bullet bullet in bullets )
		{
			bullet.Delete();
		}

		var asteroids = All.OfType<Asteroid>();
		foreach ( Asteroid asteroid in asteroids )
		{
			asteroid.Delete();
		}

		_ = PlayMusic();
		GameScore = 0;
		GameLives = 3;
		_ = SpawnAsteroids(0);

		foreach ( var cl in Game.Clients )
		{
			var player = new Ship();
			cl.Pawn = player;
			player.Respawn();
		}
	}

	private async Task GameOver()
	{
		if ( !Game.IsServer )
			return;

		StopMusic();
		ClientShowGameOver();
		await Task.DelaySeconds( 3 );
		ClientHideGameOver();
		NewGame();
	}

	private async Task Respawn( IClient cl )
	{
		if ( !Game.IsServer ) 
			return;

		await Task.DelaySeconds( 1 );

		var player = new Ship();
		cl.Pawn = player;
		player.Respawn();
	}

	[ClientRpc]
	private void ClientShowGameOver()
	{
		UI.GameOver.Current.Show();
	}

	[ClientRpc]
	private void ClientHideGameOver()
	{
		UI.GameOver.Current.Hide();
	}

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );
		(cl.Pawn as Ship)?.FrameSimulate( cl );
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Game.IsServer )
		{
			// World Bounds Shit, ghetto but who cares x
			foreach ( Entity ent in All )
			{
				if ( ent.Tags.Has( "effect" ) )
					continue;

				ent.Rotation = Rotation.FromYaw( ent.Rotation.Yaw() );
				ent.Position = ent.Position.WithZ( 0 );

				if ( ent.Position.x > PlaySpace.x )
				{
					ent.Position = new Vector3( -PlaySpace.x, ent.Position.y, 0 );
					ent.ResetInterpolation();
				}
				else if ( ent.Position.x < -PlaySpace.x )
				{
					ent.Position = new Vector3( PlaySpace.x, ent.Position.y, 0 );
					ent.ResetInterpolation();
				}

				if ( ent.Position.y > PlaySpace.y )
				{
					ent.Position = new Vector3( ent.Position.x, -PlaySpace.y, 0 );
					ent.ResetInterpolation();
				}
				else if ( ent.Position.y < -PlaySpace.y )
				{
					ent.Position = new Vector3( ent.Position.x, PlaySpace.y, 0 );
					ent.ResetInterpolation();
				}
			}
		}
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		Ship player = new Ship();
		client.Pawn = player;

		player.Respawn();
	}

	private async Task SpawnAsteroids(float delay)
	{
		await Task.DelaySeconds( delay );

		float d = 1500;
		Vector3[] spawns = { Vector3.Forward * d, Vector3.Backward * d, Vector3.Right * d, Vector3.Left * d };

		for ( int i = 0; i < spawns.Length; i++ )
		{
			new Asteroid( spawns[i], Asteroid.AsteroidSize.Large );
		}
	}

	private void AsteroidExploded(Asteroid asteroid)
	{
		if ( !Game.IsServer )
			return;

		int score = 0;

		switch ( asteroid.Size )
		{
			case Asteroid.AsteroidSize.Large:
				score = 5;
				break;
			case Asteroid.AsteroidSize.Medium:
				score = 3;
				break;
			case Asteroid.AsteroidSize.Small:
				score = 2;
				break;
			default:
				break;
		}

		GameScore += score;
		if ( GameHighScore < GameScore )
			GameHighScore = GameScore;

		// Check if they all blew up
		if ( All.OfType<Asteroid>().ToArray().Length - 1 <= 0 )
			_ = SpawnAsteroids(3);
	}

	private void ShipExploded( Ship ship )
	{
		if ( !Game.IsServer )
			return;

		GameLives--;

		if ( GameLives <= 0 )
			_ = GameOver();
		else
			_ = Respawn( ship.Client );
	}

	public static void CreateEffects( int amount, float time, Vector3 position, string model = "models/bullet.vmdl" )
	{
		for ( int i = 0; i < amount; i++ )
		{
			ModelEntity effect = new ModelEntity();
			effect.SetModel( model );
			SetBaseModelShit( effect );

			effect.Position = position;
			effect.Rotation = Rotation.FromYaw( Game.Random.Float( -360, 360 ) );
			effect.Tags.Add( "effect" );

			effect.PhysicsBody.Velocity = new Vector3( Game.Random.Float( -500, 500 ), Game.Random.Float( -500, 500 ), 0 );
			effect.PhysicsBody.AngularVelocity = new Vector3( 0, 0, Game.Random.Float( -5, 5 ) );

			effect.DeleteAsync( time );
		}
	}

	public static void SetBaseModelShit( ModelEntity ent )
	{
		ent.RenderColor = Instance.GameColor;

		ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		ent.Tags.Add( "solid" );

		ent.PhysicsBody.GravityScale = 0;
		ent.PhysicsBody.LinearDrag = 0;
		ent.PhysicsBody.AngularDrag = 0;
		
		ent.PhysicsBody.DragEnabled = false;
		ent.EnableSolidCollisions = true;
	}
}
