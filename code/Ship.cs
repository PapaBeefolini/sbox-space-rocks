using Sandbox;
using System;

namespace SpaceRocks
{
	partial class Ship : Player
	{
		public static Action<Ship> ShipExploded;

		static readonly SoundEvent ShootSound = new( "sounds/fire.vsnd" )
		{
			Volume = 0.8f,
			DistanceMax = 5000.0f
		};

		static readonly SoundEvent ThrustSound = new( "sounds/thrust.vsnd" )
		{
			Volume = 0.6f,
			DistanceMax = 5000.0f
		};

		///////////////////////

		public TimeSince TimeSinceShoot { get; private set; }
		public TimeSince TimeSinceThrust { get; private set; }

		public override void Respawn()
		{
			SetModel( "models/ship.vmdl" );
			Game.SetBaseModelShit( this );

			PhysicsBody.LinearDamping = 1.5f;
			PhysicsBody.AngularDamping = 8.0f;

			Controller = new ShipController();
			Camera = new ShipCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
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
				PlaySound( ThrustSound.Name );
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

			if ( Host.IsServer )
				new Bullet( Position + Rotation.Backward * 50, Rotation.Backward * 2000 );

			PlaySound( ShootSound.Name );
		}

		public override void OnKilled()
		{
			base.OnKilled();

			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;

			Game.CreateEffects( 3, 2.0f, Position, "models/line.vmdl" );
			Game.CreateEffects( 2, 1.0f, Position, "models/line.vmdl" );

			PlaySound( Asteroid.SoundBangLarge.Name );

			ShipExploded?.Invoke(this);
		}

		public override void StartTouch( Entity other )
		{
			if ( !IsServer )
				return;

			if ( other is Asteroid )
			{
				TakeDamage( DamageInfo.Generic( 1000 ) );
				((Asteroid)other).BlowTheFuckUp();
			}
		}

		public override void CreateHull()
		{
			// Don't need this
		}
	}
}
