using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Numerics;

namespace rainwords;

public partial class Menu : ContentPage
{
	private readonly IAudioService _audioService;
	private MainPage page;
	private bool _isInitialized;
	public Menu(IAudioService audioService)
	{

		var stopwatch = Stopwatch.StartNew();
		InitializeComponent();
		_audioService = audioService;
		Task.Run(() =>
		{
			InitializeDefaults();
			InitializeAudio();
			InitializeUI();
		});
		stopwatch.Stop();
		Console.WriteLine($"Settings loaded in {stopwatch.ElapsedMilliseconds} ms");
	}

	private void InitializeDefaults()
	{
		var currentTheme = Preferences.Default.Get("selthemedate", "");
		if (string.IsNullOrEmpty(currentTheme))
		{
			Preferences.Default.Set("selthemedate", "swhitetheme.png");
			Preferences.Default.Set("languagepickcheck", "Русский");
			Preferences.Default.Set("sweff", true);
			Preferences.Default.Set("swanim", true);
		}
	}

	private async Task InitializeAudio()
	{
		var audio = Preferences.Default.Get("swsongs", true);
		if (audio)
		{
			await _audioService.InitializeAsync();
			_audioService.PlayMenuMusic();
		}
		_isInitialized = true;
	}

	private void InitializeUI()
	{
		Dispatcher.Dispatch(() =>
		{
			var language = Preferences.Default.Get("languagepickcheck", "");
			if (language == "English")
			{
				UpdateTextToEnglish();
			}

			ApplyTheme();
			btnlist.IsVisible = true;
		});
	}

	private void UpdateTextToEnglish()
	{
		play.Text = "Play";
		setting.Text = "Setting";
		exit.Text = "Exit";
		confirmationexit.Text = "Are you sure you want to get out? Your game will not be saved";
		exitconf.Text = "Yes";
		non.Text = "No";
		back.Text = "Back";
		selectcomplex.Text = "Choose the difficulty";
		contin.Text = "Continue";
		easy.Text = "Easy";
		average.Text = "Average";
		hard.Text = "Hard";
		custom.Text = "Custom";
	}
	private void ApplyTheme()
	{
		var theme = Preferences.Default.Get("selthemedate", "");
		var buttons = btnlist.Children.OfType<Button>()
			.Concat(complex1.Children.OfType<Button>())
			.Concat(new[] { exitconf, non });

		string buttonStyleKey = "", labelStyleKey = "";
		Color backgroundColor = Colors.White;

		switch (theme)
		{
			case "swhitetheme.png":
				buttonStyleKey = "whitethemebutton";
				labelStyleKey = "whitethemelabel";
				backgroundColor = Colors.White;
				break;
			case "spinktheme.png":
				buttonStyleKey = "pinkthemebutton";
				labelStyleKey = "pinkthemelabel";
				backgroundColor = Colors.Pink;
				break;
			case "sblacktheme.png":
				buttonStyleKey = "blackthemebutton";
				labelStyleKey = "blackthemelabel";
				backgroundColor = Colors.Black;
				break;
		}

		if (!string.IsNullOrEmpty(buttonStyleKey))
		{
			allpagefortheme.BackgroundColor = backgroundColor;

			var buttonStyle = (Style)Resources[buttonStyleKey];
			var labelStyle = (Style)Resources[labelStyleKey];

			foreach (var button in buttons)
			{
				button.Style = buttonStyle;
			}

			confirmationexit.Style = labelStyle;
			selectcomplex.Style = labelStyle;
		}
	}
	private async Task StartGame()
	{
		if (!_isInitialized) return;

		if (Preferences.Default.Get("swsongs", true))
		{
			_audioService.PlayGameMusic();
		}

		page = new MainPage(_audioService);
		await Navigation.PushModalAsync(page, animated: false);
	}
	private async void Play_Clicked(object sender, EventArgs e)
	{
		btnlist.IsVisible = false;
		btnlist.IsEnabled = false;
		complex1.IsVisible = true;
		complex1.IsEnabled = true;
	}

	private async void Setting_Clicked(object sender, EventArgs e)
	{
		btnlist.IsEnabled = false;
		await Navigation.PushModalAsync(new Settings(_audioService), animated: false);
		
	}


	private void Exit_Clicked(object sender, EventArgs e)
	{
		if (contin.IsVisible)
		{
			confirmation.IsVisible = true;
		}
		else
		{
			Application.Current.Quit();
		}
	}
	private void exitconf_Clicked(object sender, EventArgs e)
	{
		Application.Current.Quit();
	}
	private void cansel(object sender, EventArgs e) => confirmation.IsVisible = false;

	async private void buttoncomplex(object sender, EventArgs e)
	{
		int complexmenu;
		var button = sender as Button;
		btnlist.IsEnabled = complex1.IsEnabled = false;
		switch (button.CommandParameter)
		{
			case "playnext":
				Data.musplay = true;
				await Navigation.PushModalAsync(page, animated: false);
				break;
			case "playeasy":
				Data.musplay = true;
				complex1.IsEnabled = false;
				complexmenu = 0;
				Data.compl = complexmenu;
				Data.timecsm = 5;
				Data.speedcsm = 10000;
				Data.pointcsm = 20;
				await StartGame();
				//page = new MainPage(_audioService);
				//await Navigation.PushModalAsync(page);
				contin.IsVisible = true;
				break;
			case "playaverage":
				Data.musplay = true;
				complex1.IsEnabled = false;
				complexmenu = 1;
				Data.compl = complexmenu;
				Data.timecsm = 3;
				Data.speedcsm = 10000;
				Data.pointcsm = 20;
				await StartGame();
				//page = new MainPage(_audioService);
				//await Navigation.PushModalAsync(page);
				contin.IsVisible = true;
				break;
			case "playhard":
				Data.musplay = true;
				complex1.IsEnabled = false;
				complexmenu = 2;
				Data.compl = complexmenu;
				Data.timecsm = 2;
				Data.speedcsm = 10000;
				Data.pointcsm = 20;
				await StartGame();
				//page = new MainPage();
				//await Navigation.PushModalAsync(page);
				break;
			case "playcustom":
				CustomBuild f = new CustomBuild(_audioService);
				await Navigation.PushModalAsync(f);
				break;
			default:
				break;
		}

		complex1.IsVisible = false;
		complex1.IsEnabled = false;

		btnlist.IsVisible = true;
		btnlist.IsEnabled = true;

	}

	private void back_menu(object sender, EventArgs e)
	{
		complex1.IsVisible = false;
		complex1.IsEnabled = false;
		btnlist.IsVisible = true;
		btnlist.IsEnabled = true;
	}
}