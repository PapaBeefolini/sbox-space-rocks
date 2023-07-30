namespace SpaceRocks.UI;

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
		highScoreLabel.Style.FontColor = GameMgr.Instance.GameColor;
		highScoreLabel.Text = "Highscore: " + GameMgr.Instance.GameHighScore;

		scoreLabel.Style.FontColor = GameMgr.Instance.GameColor;
		scoreLabel.Text = "Score: " + GameMgr.Instance.GameScore;

		livesLabel.Style.FontColor = GameMgr.Instance.GameColor;
		livesLabel.Text = "Lives: " + GameMgr.Instance.GameLives;
	}
}
