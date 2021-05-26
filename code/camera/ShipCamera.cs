using Sandbox;

namespace SpaceRocks
{
	public class ShipCamera : Camera
	{
		float distance = 2500;

		public override void Update()
		{
			Entity pawn = Local.Pawn;
			if ( pawn == null ) return;

			Pos = Vector3.Up * distance;
			Rot = Rotation.From( 90, 0, 0 );

			FieldOfView = 90;

			Viewer = null;
		}
	}
}
