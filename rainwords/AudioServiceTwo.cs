using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rainwords
{
	public interface IAudioService
	{
		Task InitializeAsync();
		void PlayMenuMusic();
		void PlayGameMusic();
		void PauseGameMusic();
		void ResumeGameMusic();
		void StopAllMusic();
		bool IsMusicEnabled { get; set; }
		bool IsInitialized { get; }
	}

	public class AudioServiceTwo : IAudioService, IDisposable
	{
		private readonly IAudioManager _audioManager;
		private IAudioPlayer _menuMusicPlayer;
		private IAudioPlayer _gameMusicPlayer;
		private bool _isInitialized;
		public bool IsInitialized => _isInitialized;
		public bool IsMusicEnabled { get; set; } = true;

		public AudioServiceTwo(IAudioManager audioManager)
		{
			_audioManager = audioManager;
		}

		public async Task InitializeAsync()
		{
			if (_isInitialized) return;

			try
			{
				// Асинхронно загружаем аудиофайлы
				var menuStream = await FileSystem.OpenAppPackageFileAsync("bgmusictwo.mp3");
				_menuMusicPlayer = _audioManager.CreatePlayer(menuStream);

				var gameStream = await FileSystem.OpenAppPackageFileAsync("bgmusinone.mp3");
				_gameMusicPlayer = _audioManager.CreatePlayer(gameStream);

				_menuMusicPlayer.Loop = true;
				_gameMusicPlayer.Loop = true;

				_isInitialized = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка загрузки аудио: {ex.Message}");
			}
		}

		public void PlayMenuMusic()
		{
			if (!IsMusicEnabled || !_isInitialized) return;

			_gameMusicPlayer?.Stop();
			_menuMusicPlayer?.Play();
		}

		public void PlayGameMusic()
		{
			if (!IsMusicEnabled || !_isInitialized) return;

			_menuMusicPlayer?.Stop();
			_gameMusicPlayer?.Play();
		}

		public void StopAllMusic()
		{
			_menuMusicPlayer?.Stop();
			_gameMusicPlayer?.Stop();
		}

		public void Dispose()
		{
			_menuMusicPlayer?.Dispose();
			_gameMusicPlayer?.Dispose();
		}

		public void PauseGameMusic()
		{
			if (!IsMusicEnabled || !_isInitialized) return;
			_gameMusicPlayer?.Pause();
		}

		public void ResumeGameMusic()
		{
			if (!IsMusicEnabled || !_isInitialized) return;
			_gameMusicPlayer?.Play();
		}
	}
}
