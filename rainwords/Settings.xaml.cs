
using System.Data;
using System.Diagnostics;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.Maui.Controls.Application;
namespace rainwords;
public partial class Settings : ContentPage
{
	private readonly IAudioService _audioService;
	private string _currentTheme;
	private Dictionary<string, ImageButton> _themeButtons = new();
	private Dictionary<string, Style> _cachedStyles = new();
	private Label  _songsel, _animsel, _rulb, _enlb, _titlelb, _exit;
	private Button _languagebtbn, _selecthemebtn;
	private Microsoft.Maui.Controls.Switch _songselSwitch, _animselSwitch;
	public Settings(IAudioService audioService)
	{
		var stopwatch = Stopwatch.StartNew();
		InitializeComponent();
		CacheUIElements();
		_audioService = audioService;
		_currentTheme = Preferences.Default.Get("selthemedate", "");
		LoadThemeButtons();
		// Первоначальная настройка UI
		UpdateLanguageUI();
		UpdateThemeUI();
		stopwatch.Stop();
		Console.WriteLine($"Settings loaded in {stopwatch.ElapsedMilliseconds} ms");
		// Обновляем только необходимые элементы
		
		_exit.Text = "<";

		App.GamePaused += PauseMenu;
		App.GameResumed += ResumeMenu;
	}

	private void PauseMenu()
	{
		_audioService.StopMenuMusic();
	}

	private void ResumeMenu()
	{
		_audioService.StartMenuMusic();
	}
	private void CacheUIElements()
	{
		_songsel = songsel;
		_animsel = animsel;
		_enlb = enlb;
		_rulb = rulb;
		_exit = exit;
		_titlelb = titlelb;
		_languagebtbn = languagebtn;
		_selecthemebtn = selecthemebtn;
		_songselSwitch = songsel_switch;
		_animselSwitch = animsel_switch;

		// Кэширование стилей
		_cachedStyles["whiteLabel"] = (Style)Resources["whitethemelabel"];
		_cachedStyles["pinkLabel"] = (Style)Resources["pinkthemelabel"];
		_cachedStyles["blackLabel"] = (Style)Resources["blackthemelabel"];
		_cachedStyles["whiteButton"] = (Style)Resources["whitethemebutton"];
		_cachedStyles["pinkButton"] = (Style)Resources["pinkthemebutton"];
		_cachedStyles["blackButton"] = (Style)Resources["blackthemebutton"];
	}

	private void LoadThemeButtons()
	{
		string[] datetheme = new string[] { "whitetheme.png", "pinktheme.png", "blacktheme.png" };

		for (int i = 0; i < datetheme.Length; i++)
		{
			var imagePath = datetheme[i];
			var selectedImagePath = "s" + imagePath;

			var imgbtncreate = new ImageButton
			{
				CornerRadius = 20,
				WidthRequest = 51,
				HeightRequest = 51,
				BackgroundColor = Colors.Transparent,
				Source = _currentTheme == selectedImagePath ? selectedImagePath : imagePath
			};

			Grid.SetColumn(imgbtncreate, i);
			imgbtncreate.Clicked += Image_Clicked;
			theme.Children.Add(imgbtncreate);

			_themeButtons[imagePath] = imgbtncreate;
		}
	}

	private void UpdateLanguageUI()
	{
		var language = Preferences.Default.Get("languagepickcheck", "");
		var swanim = Preferences.Default.Get("swanim", true);
		var swsongs = Preferences.Default.Get("swsongs", true);
		_enlb.FontFamily = language == "ENGLISH" ? "Kokoro-Regular" : "Kokoro";
		_enlb.FontSize = language == "ENGLISH" ? 20 : 18;
		_rulb.FontFamily = language == "РУССКИЙ" ? "Kokoro-Regular" : "Kokoro";
		_rulb.FontSize = language == "РУССКИЙ" ? 20 : 18;
		
		
		// Обновление текстов
		if (language == "ENGLISH")
		{
			_selecthemebtn.Text = "THEME";
			_languagebtbn.Text = "LANGUAGE";
			_songsel.Text = "SOUND";
			_animsel.Text = "ANIMATIONS";
			_titlelb.Text = "SETTINGS MENU";
		}
		else
		{
			_selecthemebtn.Text = "ТЕМЫ";
			_languagebtbn.Text = "ЯЗЫК";
			_songsel.Text = "ЗВУК";
			_animsel.Text = "АНИМАЦИИ";
			_titlelb.Text = "МЕНЮ НАСТРОЕК";
		}

		// Обновление переключателей
		_animselSwitch.IsToggled = swanim;
		_songselSwitch.IsToggled = swsongs;
	}



	private void lang_vkl(object sender, EventArgs e)
	{
		var language = ((Label)sender).Text.ToString();
		Preferences.Default.Set("languagepickcheck", language);
		_rulb.FontFamily = language == "РУССКИЙ" ? "Kokoro-Regular" : "Kokoro";
		_rulb.FontSize = language == "РУССКИЙ" ? 20 : 18;
		_enlb.FontFamily = language == "ENGLISH" ? "Kokoro-Regular" : "Kokoro";
		_enlb.FontSize = language == "ENGLISH" ? 20 : 18;
		UpdateLanguageUI();
	}

	private void song_Toggled(object sender, ToggledEventArgs e)
	{
		_audioService.IsMusicEnabled = e.Value;
		var language = Preferences.Default.Get("languagepickcheck", "");

		if (_songselSwitch.IsToggled)
		{
			if (e.Value && Application.Current.MainPage is Menu)
				_audioService.PlayMenuMusic();
			if (e.Value && Application.Current.MainPage is MainPage)
				_audioService.PlayGameMusic();
			Preferences.Default.Set("swsongs", true);
		}
		else
		{
			if (!e.Value)
				_audioService.StopAllMusic();
			Preferences.Default.Set("swsongs", false);
		}

	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		// Отписка от событий при уходе со страницы
		App.GamePaused -= PauseMenu;
		App.GameResumed -= ResumeMenu;
	}
	private void UpdateThemeUI()
	{
		switch (_currentTheme)
		{
			case "swhitetheme.png":
				ApplyTheme(Colors.White, "whiteLabel", "whiteButton");
				break;
			case "spinktheme.png":
				ApplyTheme(Colors.HotPink, "pinkLabel", "pinkButton");
				break;
			case "sblacktheme.png":
				ApplyTheme(Colors.Black, "blackLabel", "blackButton");
				break;
		}
	}

	private void selecthemebtn_Clicked(object sender, EventArgs e)
	{
		if (!theme.IsVisible) theme.IsVisible = true;
		else theme.IsVisible = false;
	}

	private async void exit_menu(object sender, EventArgs e)
	{
		mainsettings.IsEnabled = false;
		await Navigation.PushModalAsync(new Menu(_audioService), animated: false);
	}

	private void languagebtn_Clicked(object sender, EventArgs e)
	{
		if (!languageswitch.IsVisible) languageswitch.IsVisible = true;
		else languageswitch.IsVisible = false;
	}

	private void ApplyTheme(Color backgroundColor, string labelStyleKey, string buttonStyleKey)
	{
		mainsettings.BackgroundColor = backgroundColor;

		var labelStyle = _cachedStyles[labelStyleKey];
		var buttonStyle = _cachedStyles[buttonStyleKey];

		_songsel.Style = labelStyle;
		_selecthemebtn.Style = buttonStyle;
		_animsel.Style = labelStyle;
		_exit.Style = labelStyle;
		_titlelb.Style = labelStyle;
		_languagebtbn.Style = buttonStyle;
		_rulb.Style = labelStyle;
		_enlb.Style = labelStyle;
	}
	string mas;
	private void Image_Clicked(object sender, EventArgs e)
	{

		var imagebutton = sender as ImageButton;
		if (imagebutton == null) return;
		var source = imagebutton.Source;

		if (source is FileImageSource fileImageSource)
		{
			string imagePath = fileImageSource.File;
			if (imagePath == _currentTheme)
			{
				return;
			}
			else
			{
				foreach (var x in theme)
				{
					if (x is ImageButton button && button.Source.ToString().Remove(0, 6) == imagePath)
					{
						foreach (var y in theme)
						{
							if (y is ImageButton btndel && btndel.Source.ToString().Remove(0, 6) == _currentTheme)
							{
								btndel.Source = _currentTheme.Remove(0, 1);
							}
						}

						_currentTheme = "s" + imagePath;
						imagebutton.Source = _currentTheme;
						Preferences.Default.Set("selthemedate", imagebutton.Source.ToString().Remove(0, 6));
						UpdateThemeUI();
						return;
					}


				}
			}

		}

	}

	private void anim_Toggled(object sender, ToggledEventArgs e)
	{
		var language = Preferences.Default.Get("languagepickcheck", "");

		if (_animselSwitch.IsToggled) Preferences.Default.Set("swanim", true);
		else Preferences.Default.Set("swanim", false);

	}

}