using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpaceRocks.UI
{
	public partial class Hud : Panel
	{
		Label highScoreLabel;
		Label scoreLabel;
		Label livesLabel;

		public Hud()
		{
			highScoreLabel = Add.Label( "", "text" );
			scoreLabel = Add.Label( "", "text" );
			livesLabel = Add.Label( "", "text" );
		}

		public override void Tick()
		{
			highScoreLabel.Style.FontColor = Game.Instance.GameColor;
			highScoreLabel.Text = "Highscore: " + Game.Instance.GameHighScore;

			scoreLabel.Style.FontColor = Game.Instance.GameColor;
			scoreLabel.Text = "Score: " + Game.Instance.GameScore;

			livesLabel.Style.FontColor = Game.Instance.GameColor;
			livesLabel.Text = "Lives: " + Game.Instance.GameLives;
		}
	}
}
