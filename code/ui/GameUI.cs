using Sandbox.UI;

namespace SpaceRocks.UI
{
	public partial class GameUI : Sandbox.HudEntity<RootPanel>
	{
		public GameUI()
		{
			if ( !IsClient )
				return;

			RootPanel.StyleSheet.Load( "/ui/Hud.scss" );

			//RootPanel.AddChild<NameTags>();
			//RootPanel.AddChild<CrosshairCanvas>();
			ChatBox chat = RootPanel.AddChild<ChatBox>();
			chat.Style.FontFamily = "VCR_OSD_MONO_1.001";
			RootPanel.AddChild<VoiceList>();
			//RootPanel.AddChild<KillFeed>();
			//RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
			RootPanel.AddChild<Hud>();
			RootPanel.AddChild<GameOver>();
		}
	}
}
