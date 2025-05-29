using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Numerics;

namespace rainwords;

public partial class Menu : ContentPage
{
	private readonly IAudioService _audioService;
	private MainPage page;
	private bool _isInitialized;
	private bool _musicda = true;
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
		back.Text = "<";
		stopwatch.Stop();
		Console.WriteLine($"Settings loaded in {stopwatch.ElapsedMilliseconds} ms");
		App.GamePaused += PauseMenu;
		App.GameResumed += ResumeMenu;
	}

	private void PauseMenu()
	{
		_audioService.StopMenuMusic();
		_musicda = false;
	}

	private void ResumeMenu()
	{
		if(!_musicda) _audioService.StartMenuMusic();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		// ќтписка от событий при уходе со страницы
		App.GamePaused -= PauseMenu;
		App.GameResumed -= ResumeMenu;
	}
	private void InitializeDefaults()
	{
		var currentTheme = Preferences.Default.Get("selthemedate", "");
		if (string.IsNullOrEmpty(currentTheme))
		{
			Preferences.Default.Set("selthemedate", "swhitetheme.png");
			Preferences.Default.Set("languagepickcheck", "–”—— »…");
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
			if (language == "ENGLISH")
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
		confirmationexitone.Text = "Definitely get out?";
		confirmationexittwo.Text = "Your game will not be saved!";
		exitconf.Text = "Yes";
		non.Text = "No";
		selectcomplex.Text = "DIFFICULTY";
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
		string backgroundColor = "bgwhite.jpeg";
		Color BackInfor = Colors.White;
		Color StrokeInfo = Colors.Black;
		switch (theme)
		{
			case "swhitetheme.png":
				buttonStyleKey = "whitethemebutton";
				labelStyleKey = "whitethemelabel";
				backgroundColor = "bgwhite.jpeg";
				break;
			case "spinktheme.png":
				buttonStyleKey = "pinkthemebutton";
				labelStyleKey = "pinkthemelabel";
				backgroundColor = "bgpink.jpeg";
				BackInfor = Colors.HotPink;
				break;
			case "sblacktheme.png":
				buttonStyleKey = "blackthemebutton";
				labelStyleKey = "blackthemelabel";
				backgroundColor = "bgblack.jpeg";
				BackInfor = Colors.Black;
				StrokeInfo = Colors.White;
				break;
		}

		if (!string.IsNullOrEmpty(buttonStyleKey))
		{
			bg.Source = backgroundColor;
			var buttonStyle = (Style)Resources[buttonStyleKey];
			var labelStyle = (Style)Resources[labelStyleKey];
			confirmation.BackgroundColor = BackInfor;
			iamstroke.Stroke = StrokeInfo;
			foreach (var button in buttons)
			{
				button.Style = buttonStyle;
			}
			back.Style = labelStyle;
			confirmationexitone.Style = labelStyle;
			confirmationexittwo.Style = labelStyle;
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
		back.IsVisible = true;
		btnlist.IsVisible = false;
		btnlist.IsEnabled = false;
		complex1.IsVisible = true;
		complex1.IsEnabled = true;
		ApplyTheme();
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
			play.IsEnabled = false;
			setting.IsEnabled = false;
			exit.IsEnabled = false;
			confirmation.IsVisible = true;
		}
		else
		{
			Application.Current.Quit();
		}
	}
	private void exitconf_Clicked(object sender, EventArgs e) => Application.Current.Quit();

	private void cansel(object sender, EventArgs e)
	{
		play.IsEnabled = true;
		setting.IsEnabled = true;
		exit.IsEnabled = true;
		confirmation.IsVisible = false;
	}

		async private void buttoncomplex(object sender, EventArgs e)
	{
		int complexmenu;
		var button = sender as Button;
		btnlist.IsEnabled = complex1.IsEnabled = false;
		switch (button.CommandParameter)
		{
			case "playnext":
				back.IsVisible = false;
				Data.musplay = true;
				await Navigation.PushModalAsync(page, animated: false);
				break;
			case "playeasy":
				Data.musplay = true;
				complex1.IsEnabled = false;
				back.IsVisible = false;
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
				back.IsVisible = false;
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
				back.IsVisible = false;
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
				back.IsVisible = false;
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
		back.IsVisible = false;
		complex1.IsVisible = false;
		complex1.IsEnabled = false;
		btnlist.IsVisible = true;
		btnlist.IsEnabled = true;
		ApplyTheme();
	}
}