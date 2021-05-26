using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpaceRocks.UI
{
	public partial class GameOver : Panel
	{
		public static GameOver Current;

		Label gameOverLabel;

		public GameOver()
		{
			Current = this;

			StyleSheet.Load( "/ui/GameOver.scss" );
			gameOverLabel = Add.Label( "", "text" );
		}

		public void Show()
		{
			gameOverLabel.Style.FontColor = Game.Instance.GameColor;
			gameOverLabel.Text = "Game Over";
			gameOverLabel.Style.Dirty();
		}

		public void Hide()
		{
			gameOverLabel.Text = "";
		}
	}
}
