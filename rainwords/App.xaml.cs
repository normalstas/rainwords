using Plugin.Maui.Audio;

namespace rainwords
{
	public partial class App : Application
	{
		IAudioService _audioService;
		public App()
		{
			InitializeComponent();

			MainPage = new AppShell();


			Current.RequestedThemeChanged += (s, a) => { };

		}

		protected override Window CreateWindow(IActivationState activationState)
		{
			var window = base.CreateWindow(activationState);

			window.Deactivated += OnWindowDeactivated; // При сворачивании
			window.Activated += OnWindowActivated;     // При разворачивании

			return window;
		}

		private void OnWindowDeactivated(object sender, EventArgs e)
		{
			// Пауза при сворачивании
			PauseGame();
		}

		private void OnWindowActivated(object sender, EventArgs e)
		{
			// Возобновление при разворачивании (если не на паузе)
			if (!isManuallyPaused)
			{
				ResumeGame();
			}
		}

		private bool isManuallyPaused = false;
		public static event Action GamePaused;
		public static event Action GameResumed;

		public void PauseGame()
		{
			GamePaused?.Invoke();
		}

		public void ResumeGame()
		{
			GameResumed?.Invoke();
		}
	}
}
