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

		void PlayWinSound();
		void PlayLossSound();

		void PlayExplaSound();

		void StopMenuMusic();

		void StartMenuMusic();
		bool IsMusicEnabled { get; set; }
		bool IsInitialized { get; }
	}

	public class AudioServiceTwo : IAudioService, IDisposable
	{
		private readonly IAudioManager _audioManager;
		private IAudioPlayer _menuMusicPlayer;
		private IAudioPlayer _gameMusicPlayer;
		private IAudioPlayer _winSoundPlayer;
		private IAudioPlayer _lossSoundPlayer;
		private IAudioPlayer _explaSoundPlayer;
		private bool _isInitialized;
		public bool IsInitialized => _isInitialized;
		public bool IsMusicEnabled { get; set; } = true;

		public AudioServiceTwo(IAudioManager audioManager)
		{
			_audioManager = audioManager;
			App.GamePaused += StopMenuMusic;
			App.GameResumed += StartMenuMusic;
		}

		public async Task InitializeAsync()
		{
			if (_isInitialized) return;

			try
			{
				// Асинхронно загружаем аудиофайлы
				var menuStream = await FileSystem.OpenAppPackageFileAsync("bgmusicmenu.mp3");
				_menuMusicPlayer = _audioManager.CreatePlayer(menuStream);

				var gameStream = await FileSystem.OpenAppPackageFileAsync("bgmusicgame.mp3");
				_gameMusicPlayer = _audioManager.CreatePlayer(gameStream);

				var winword = await FileSystem.OpenAppPackageFileAsync("winsound.wav");
				_winSoundPlayer = _audioManager.CreatePlayer(winword);

				var lossword = await FileSystem.OpenAppPackageFileAsync("losssound.wav");
				_lossSoundPlayer = _audioManager.CreatePlayer(lossword);

				var explaword = await FileSystem.OpenAppPackageFileAsync("explasound.wav");
				_explaSoundPlayer = _audioManager.CreatePlayer(explaword);
				_menuMusicPlayer.Loop = true;
				_gameMusicPlayer.Loop = true;
				_winSoundPlayer.Loop = false;
				_lossSoundPlayer.Loop = false;
				_explaSoundPlayer.Loop = false;
				_winSoundPlayer.Volume = Math.Clamp(1.0, 0.0, 1.0);
				_lossSoundPlayer.Volume = Math.Clamp(1.0, 0.0, 1.0);
				_explaSoundPlayer.Volume = Math.Clamp(1.0, 0.0, 1.0);
				_gameMusicPlayer.Volume = Math.Clamp(0.7, 0.0, 1.0);
				_menuMusicPlayer.Volume = Math.Clamp(0.7, 0.0, 1.0);


				_isInitialized = true;
			}
			catch (Exception ex)
			{
				
			}
		}

		public void PlayWinSound()
		{
			if (!IsMusicEnabled || !_isInitialized) return;

			_winSoundPlayer?.Play();
		}

		public void PlayLossSound()
		{
			if (!IsMusicEnabled || !_isInitialized) return;

			_lossSoundPlayer?.Play();
		}

		public void PlayExplaSound()
		{
			if (!IsMusicEnabled || !_isInitialized) return;

			_explaSoundPlayer?.Play();
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
			if (!IsMusicEnabled || !_isInitialized) return;
			_menuMusicPlayer?.Stop();
			_gameMusicPlayer?.Stop();
			_explaSoundPlayer?.Stop();
			_lossSoundPlayer?.Stop();
			_winSoundPlayer?.Stop();
		}

		public void StopMenuMusic()
		{
			if (!IsMusicEnabled || !_isInitialized) return;
			_menuMusicPlayer?.Stop();
		}
		public void StartMenuMusic()
		{
			if (!IsMusicEnabled || !_isInitialized) return;
			_menuMusicPlayer?.Play();
		}

		public void Dispose()
		{
			if (!IsMusicEnabled || !_isInitialized) return;
			_menuMusicPlayer?.Dispose();
			_gameMusicPlayer?.Dispose();
		}

		public void PauseGameMusic()
		{
			if (IsMusicEnabled || !_isInitialized) return;
			_gameMusicPlayer?.Pause();
			_menuMusicPlayer?.Stop();
		}

		public void ResumeGameMusic()
		{
			if (IsMusicEnabled || !_isInitialized) return;
			_gameMusicPlayer?.Play();
			_menuMusicPlayer?.Stop();
		}

		public void OnAppPaused()
		{
			_menuMusicPlayer?.Stop();
			_gameMusicPlayer?.Stop();
			_explaSoundPlayer?.Stop();
			_lossSoundPlayer?.Stop();
			_winSoundPlayer?.Stop();
		}

		
	}
}
