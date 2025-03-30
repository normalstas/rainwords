using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Maui.Audio;
namespace rainwords
{
	public class AudioService
	{
		private IAudioPlayer _musicPlayer;
		private bool _isMusicPlaying = false;

		// Инициализация музыки
		public async Task InitializeMusic()
		{
			using var stream = await FileSystem.OpenAppPackageFileAsync("Resources\\Raw\\bgmusinone.mp3");
			_musicPlayer = AudioManager.Current.CreatePlayer(stream);
			_musicPlayer.Loop = true; // Включить зацикливание
		}

		// Переключение музыки (вкл/выкл)
		public void ToggleMusic()
		{
			if (_isMusicPlaying)
			{
				_musicPlayer.Pause();
			}
			else
			{
				_musicPlayer.Play();
			}
			_isMusicPlaying = !_isMusicPlaying;
		}

		// Установка громкости (0.0 - 1.0)
		public void SetVolume(double volume)
		{
			_musicPlayer.Volume = volume;
		}
	}
}
