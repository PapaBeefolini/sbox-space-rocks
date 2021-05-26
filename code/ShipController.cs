using Sandbox;

namespace SpaceRocks
{
	public partial class ShipController : BasePlayerController
	{
		public override void Simulate()
		{
			Ship ship = Pawn as Ship;

			ship.Move( Input.Forward, Input.Left );

			if ( Input.Pressed( InputButton.Jump ) )
				ship.Shoot();
		}
	}
}
