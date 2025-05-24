using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Numerics;

namespace rainwords;

public partial class Menu : ContentPage
{
	private readonly IAudioService _audioService;
	public Menu(IAudioService audioService)
	{

		var stopwatch = Stopwatch.StartNew();
		InitializeComponent();
		stopwatch.Stop();
		Console.WriteLine($"Settings loaded in {stopwatch.ElapsedMilliseconds} ms");
		btnlist.IsVisible = true;
		var currentTheme = Preferences.Default.Get("selthemedate", "");
		if (string.IsNullOrEmpty(currentTheme))
		{
			Preferences.Default.Set("selthemedate", "swhitetheme.png");
			Preferences.Default.Set("languagepickcheck", "Русский");
			Preferences.Default.Set("sweff", true);
			Preferences.Default.Set("swanim", true);
		}
		_audioService = audioService;
		InitializeAudio();
		startgame();
	}
	private async Task InitializeAudio()
	{
		if (Preferences.Default.Get("swsongs", true) == true)
		{
			await Task.Run(() => _audioService.InitializeAsync());
			_audioService.PlayMenuMusic();
		}
	}

	private async Task StartGame()
	{
		if (Preferences.Default.Get("swsongs", true) == true) _audioService.PlayGameMusic();
		page = new MainPage(_audioService);
		await Navigation.PushModalAsync(page);
	}
	MainPage page;
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
	private void cansel(object sender, EventArgs e)
	{
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
				Data.musplay = true;
				await Navigation.PushModalAsync(page);
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

	async Task startgame()
	{
		var language = Preferences.Default.Get("languagepickcheck", "");
		if (language == "English")
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
		var theme = Preferences.Default.Get("selthemedate", "");
		var buttons = btnlist.Children.OfType<Button>().Concat(complex1.Children.OfType<Button>());
		var buttonslist = btnlist.Children.OfType<Button>().Concat(complex1.Children.OfType<Button>());
		switch (theme)
		{
			case "swhitetheme.png":
				allpagefortheme.BackgroundColor = Colors.White;

				foreach (var button in buttons) { { button.Style = (Style)Resources["whitethemebutton"]; } }
				foreach (var button in buttonslist) { { button.Style = (Style)Resources["whitethemebutton"]; } }
				exitconf.Style = (Style)Resources["whitethemebutton"];
				non.Style = (Style)Resources["whitethemebutton"];


				confirmationexit.Style = (Style)Resources["whitethemelabel"];
				selectcomplex.Style = (Style)Resources["whitethemelabel"];

				break;
			case "spinktheme.png":
				allpagefortheme.BackgroundColor = Colors.Pink;

				foreach (var button in buttons) { { button.Style = (Style)Resources["pinkthemebutton"]; } }
				foreach (var button in buttonslist) { { button.Style = (Style)Resources["pinkthemebutton"]; } }
				exitconf.Style = (Style)Resources["pinkthemebutton"];
				non.Style = (Style)Resources["pinkthemebutton"];


				confirmationexit.Style = (Style)Resources["pinkthemelabel"];
				selectcomplex.Style = (Style)Resources["pinkthemelabel"];


				break;
			case "sblacktheme.png":
				allpagefortheme.BackgroundColor = Colors.Black;

				foreach (var button in buttons) {  { button.Style = (Style)Resources["blackthemebutton"]; } }
				foreach (var button in buttonslist) { { button.Style = (Style)Resources["blackthemebutton"]; } }
				exitconf.Style = (Style)Resources["blackthemebutton"];
				non.Style = (Style)Resources["blackthemebutton"];


				confirmationexit.Style = (Style)Resources["blackthemelabel"];
				selectcomplex.Style = (Style)Resources["blackthemelabel"];
				break;
			default:
				break;
		}
	}

}