namespace SpaceRocks;

public partial class GameMgr
{
	bool isPlaying = false;

	async Task PlayMusic()
	{
		if ( isPlaying )
			return;

		isPlaying = true;

		float musicTime = 1.0f;

		while ( isPlaying )
		{
			await Task.DelaySeconds( musicTime );

			Sound.FromScreen( "sounds/beat1.sound" );

			await Task.DelaySeconds( musicTime );

			Sound.FromScreen( "sounds/beat2.sound" );

			musicTime -= 0.01f;
			musicTime = MathX.Clamp(musicTime, 0.2f, 1.0f);
		}
	}
	void StopMusic()
	{
		isPlaying = false;
	}
}
