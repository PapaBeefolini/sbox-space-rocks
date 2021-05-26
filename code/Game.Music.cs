using Sandbox;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceRocks
{
	public partial class Game
	{
		static readonly SoundEvent Beat1 = new( "sounds/beat1.vsnd" )
		{
			Volume = 0.9f,
			DistanceMax = 5000.0f
		};

		static readonly SoundEvent Beat2 = new( "sounds/beat2.vsnd" )
		{
			Volume = 0.9f,
			DistanceMax = 5000.0f
		};

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

				PlaySound( Beat1.Name );

				await Task.DelaySeconds( musicTime );

				PlaySound( Beat2.Name );

				musicTime -= 0.01f;
				musicTime = MathX.Clamp(musicTime, 0.2f, 1.0f);
			}
		}
		void StopMusic()
		{
			isPlaying = false;
		}
	}
}
