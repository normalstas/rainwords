using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace rainwords;

public partial class CustomBuild : ContentPage
{
	private readonly IAudioService _audioService;
	private bool _isInitialized;
	public CustomBuild(IAudioService audioService)//все тоже самое сверху как и у остальных
	{
		_audioService = audioService;
		InitializeComponent();
		Task.Run(() =>//асинхронно запускам чтобы без задержек было
		{
			InitializeAudio();
			Dispatcher.Dispatch(startgame);//выполнение в главном потоке
		});
		backcompl.Text = "<";
		App.GamePaused += PauseMenu; //подписываемся на событие чтобы при сворачивании приложения останавливалась музыка
		App.GameResumed += ResumeMenu;//продолжалась
	}

	private void PauseMenu()
	{
		_audioService.StopMenuMusic();
	}

	private void ResumeMenu()
	{
		_audioService.StartMenuMusic();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		// отписка от событий при уходе со страницы
		App.GamePaused -= PauseMenu;
		App.GameResumed -= ResumeMenu;
	}
	private async void InitializeAudio()
	{
		if (_isInitialized) return;

		if (Preferences.Default.Get("swsongs", true))
		{
			await _audioService.InitializeAsync();
			_audioService.PlayMenuMusic();
		}
		else
		{
			_audioService.IsMusicEnabled = false;
		}
		_isInitialized = true;
	}

	private void startgame()
	{
		allpagefortheme.IsEnabled = true;

		var landuage = Preferences.Default.Get("languagepickcheck", "");
		if (landuage == "ENGLISH")//переводим
		{
			//backcompl.Text = "Back";
			speedcustom.Text = "SPEED";
			entryspeed.Placeholder = "10-100000";
			pointlb.Text = "POINTS";
			timelb.Text = "TIME";
			entrytime.Placeholder = "1-10000 min";
			wordslb.Text = "NUMBER OF LETTERS";
			customplay.Text = "GAME";
			titlelb.Text = "CUSTOM MODE";
		}
		var theme = Preferences.Default.Get("selthemedate", "");
		if (string.IsNullOrEmpty(theme)) return;

		var themePrefix = theme.Replace("stheme.png", "").Replace("s", "").Replace(".png", "");
		ApplyTheme(themePrefix);//определяем темы для элементов 
	}

	private void ApplyTheme(string themePrefix)
	{//создаем темы для всех элементов
		allpagefortheme.BackgroundColor = themePrefix switch
		{
			"whitetheme" => Colors.White,
			"pinktheme" => Colors.HotPink,
			"blacktheme" => Colors.Black,
			_ => allpagefortheme.BackgroundColor
		};

		foreach (var child in allpagefortheme.Children)
		{
			switch (child)
			{
				case Button button:
					button.Style = (Style)Resources[$"{themePrefix}button"];
					break;
				case Label label:
					label.Style = (Style)Resources[$"{themePrefix}label"];
					break;
				case Entry entry:
					entry.Style = (Style)Resources[$"{themePrefix}entry"];
					entry.TextColor = themePrefix == "blacktheme" ? Colors.White : Colors.Black;
					break;
				case Frame frame:
					frame.BackgroundColor = themePrefix == "blacktheme" ? Colors.Black : Colors.White;
					if (themePrefix == "pinktheme") frame.Background = new SolidColorBrush(Color.FromArgb("#D5156B"));
					//frame.BackgroundColor = themePrefix == "pinktheme" ? Colors.HotPink : Colors.Black;
					break;
				case Border border:
					border.Stroke = themePrefix == "blacktheme" ? Colors.White : Colors.Black;
					break;
			}
			
		}
		//entry.Style = (Style)Resources[$"{themePrefix}entry"];
		//entry.TextColor = themePrefix == "blacktheme" ? Colors.White : Colors.Black;
		switch (themePrefix)
		{
			case "blacktheme":
				entryspeed.TextColor = Colors.White;
				entrypoint.TextColor = Colors.White;
				entrytime.TextColor = Colors.White;
				entrycount.TextColor = Colors.White;
				break;
			case "pinktheme":
				entryspeed.TextColor = Colors.Black;
				entrypoint.TextColor = Colors.Black;
				entrytime.TextColor = Colors.Black;
				entrycount.TextColor = Colors.Black;
				break;
			case "whitetheme":
				entryspeed.TextColor = Colors.Black;
				entrypoint.TextColor = Colors.Black;
				entrytime.TextColor = Colors.Black;
				entrycount.TextColor = Colors.Black;
				break;
			default:
				break;
		}
		backcompl.TextColor = themePrefix == "blacktheme" ? Colors.White : Colors.Black;
		titlelb.TextColor = themePrefix == "blacktheme" ? Colors.White : Colors.Black; 
	}

	private async void backcompl_Clicked(object sender, EventArgs e)
	{
		UnfocusAll();//закрываем клаву
		await Navigation.PopModalAsync(animated: false);//назад
	}

	private void entryspeed_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (sender is not Entry entry) return;

		var isValid = entry.Placeholder switch
		{
			"10-100000 чем выше тем медленнее" or
			"1-10000" or
			"1-10000 мин" => !string.IsNullOrEmpty(entry.Text) && entry.Text.Length <= 10000,
			_ => true
		};
		entry.TextColor = isValid ? (allpagefortheme.BackgroundColor == Colors.White ? Colors.Black : Colors.White) : Colors.Red;//ошибка
	}

	private async void customplay_Clicked(object sender, EventArgs e)
	{
		//запускаем игру
		if (!ValidateInputs()) return;

		allpagefortheme.IsEnabled = false;
		Data.musplay = true;
		UnfocusAll();
		Data.timecsm = int.Parse(entrytime.Text);
		Data.speedcsm = uint.Parse(entryspeed.Text);
		Data.pointcsm = int.Parse(entrypoint.Text);

		var page = new MainPage(_audioService);
		await Navigation.PushModalAsync(page, animated: false);
		allpagefortheme.IsEnabled = true;
	}

	private void UnfocusAll()
	{
		entryspeed.Unfocus();
		entrypoint.Unfocus();
		entrytime.Unfocus();
		entrycount.Unfocus();
	}

	private bool ValidateInputs()
	{
		return !string.IsNullOrEmpty(entrytime.Text) ||
			   !string.IsNullOrEmpty(entryspeed.Text) ||//если ничего не написали 
			   !string.IsNullOrEmpty(entrypoint.Text);
	}
}