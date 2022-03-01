using Sandbox;

namespace SpaceRocks
{
	public class ShipCamera : CameraMode
	{
		float distance = 2500;

		public override void Update()
		{
			Entity pawn = Local.Pawn;
			if ( pawn == null ) return;

			Position = Vector3.Up * distance;
			Rotation = Rotation.From( 90, 0, 0 );

			FieldOfView = 90;

			Viewer = null;
		}
	}
}
