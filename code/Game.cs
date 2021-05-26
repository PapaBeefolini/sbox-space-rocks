using Sandbox;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceRocks
{
	[Library( "spacerocks", Title = "Space Rocks" )]
	public partial class Game : Sandbox.Game
	{
		public static Game Instance
		{
			get => Current as Game;
		}

		[Net] public Vector2 PlaySpace { get; private set; } = new Vector2( 1700, 2200 );
		[Net] public Color GameColor { get; private set; } = Color.White;
		[Net] public int GameScore { get; private set; } = 0;
		[Net] public int GameHighScore { get; private set; } = 0;
		[Net] public int GameLives { get; private set; } = 3;

		public Game()
		{
			if ( IsServer )
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

				NewGame();
			}
		}

		private void NewGame()
		{
			if ( !IsServer )
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
		}

		private async Task GameOver()
		{
			if ( !IsServer )
				return;

			StopMusic();
			ClientShowGameOver();
			await Task.DelaySeconds( 3 );
			ClientHideGameOver();
			NewGame();
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

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( IsServer )
			{
				// World Bounds Shit, ghetto but who cares x
				foreach ( Entity ent in All )
				{
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

		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			Ship player = new Ship();
			client.Pawn = player;

			player.Respawn();
		}

		public override void DoPlayerNoclip( Client player )
		{
			// Can't noclip
		}

		public override void DoPlayerDevCam( Client player )
		{
			//base.DoPlayerDevCam( player );
			// Can't DevCam
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
			if ( !IsServer )
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
			if ( !IsServer )
				return;

			GameLives--;

			if ( GameLives <= 0 )
				_ = GameOver();
		}

		public static void CreateEffects( int amount, float time, Vector3 position, string model = "models/bullet.vmdl" )
		{
			for ( int i = 0; i < amount; i++ )
			{
				ModelEntity effect = new ModelEntity();
				effect.SetModel( model );
				SetBaseModelShit( effect );

				effect.Position = position;
				effect.Rotation = Rotation.FromYaw( Rand.Float( -360, 360 ) );

				effect.PhysicsBody.Velocity = new Vector3( Rand.Float( -500, 500 ), Rand.Float( -500, 500 ), 0 );
				effect.PhysicsBody.AngularVelocity = new Vector3( 0, 0, Rand.Float( -5, 5 ) );

				effect.DeleteAsync( time );
			}
		}

		public static void SetBaseModelShit( ModelEntity ent )
		{
			ent.RenderColor = Instance.GameColor;

			ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			ent.MoveType = MoveType.Physics;
			ent.CollisionGroup = CollisionGroup.Trigger;

			ent.PhysicsBody.GravityScale = 0;
			ent.PhysicsBody.LinearDrag = 0;
			ent.PhysicsBody.AngularDrag = 0;
			ent.EnableSolidCollisions = false;
		}
	}
}
