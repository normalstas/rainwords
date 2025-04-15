using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace rainwords
{
	public class AudioService
	{
		private IAudioPlayer _musicPlayer;
		public async Task InitializeMusic()
		{
			using var stream = await FileSystem.OpenAppPackageFileAsync("bgmusictwo.mp3");
			_musicPlayer = AudioManager.Current.CreatePlayer(stream);
			_musicPlayer.Loop = true;
			_musicPlayer.Volume = 1;
		}

		public async Task InitializeMusic2()
		{
			using var stream = await FileSystem.OpenAppPackageFileAsync("bgmusinone.mp3");
			_musicPlayer = AudioManager.Current.CreatePlayer(stream);
			_musicPlayer.Loop = true;
		}
		public void MuteMusic()
		{
			_musicPlayer?.Stop();
		}
		public void UnmuteMusic()
		{
			_musicPlayer.Play();
		}
	}
}
