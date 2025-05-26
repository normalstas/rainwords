using IntelliJ.Lang.Annotations;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.Maui.Controls.Application;
namespace rainwords;
public partial class Settings : ContentPage
{
	private readonly IAudioService _audioService;
	private string _currentTheme;
	private Dictionary<string, ImageButton> _themeButtons = new();
	private Dictionary<string, Style> _cachedStyles = new();
	private Label _sellang, _songsel, _selecttheme, _animsel, _rustrue, _entrue, _lben, _lbrus;
	private Button _exit;
	private Microsoft.Maui.Controls.Switch _songselSwitch, _animselSwitch;
	public Settings(IAudioService audioService)
	{
		var stopwatch = Stopwatch.StartNew();
		InitializeComponent();
		CacheUIElements();
		_audioService = audioService;
		_currentTheme = Preferences.Default.Get("selthemedate", "");
		LoadThemeButtons();
		// �������������� ��������� UI
		UpdateLanguageUI();
		UpdateThemeUI();
		stopwatch.Stop();
		Console.WriteLine($"Settings loaded in {stopwatch.ElapsedMilliseconds} ms");
		var language = Preferences.Default.Get("languagepickcheck", "");

		// ��������� ������ ����������� ��������
		_rustrue.Text = language == "�������" ? ">" : "";
		_entrue.Text = language == "English" ? ">" : "";

	}

	private void CacheUIElements()
	{
		_sellang = sellang;
		_songsel = songsel;
		_selecttheme = selecttheme;
		_animsel = animsel;
		_rustrue = rustrue;
		_entrue = entrue;
		_lben = lben;
		_lbrus = lbrus;
		_exit = exit;
		_songselSwitch = songsel_switch;
		_animselSwitch = animsel_switch;

		// ����������� ������
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

		// ���������� �������
		if (language == "English")
		{
			_sellang.Text = "Choose a language";
			_exit.Text = "Save and exit";
			_selecttheme.Text = "choose a theme";
			_songsel.Text = swsongs ? "Turn off the sound" : "Turn on the sound";
			_animsel.Text = swanim ? "Turn off animations and effects" : "Enable animations and effects";
		}
		else
		{
			_sellang.Text = "������� ����";
			_exit.Text = "��������� � �����";
			_selecttheme.Text = "������� ����";
			_songsel.Text = swsongs ? "��������� ����" : "�������� ����";
			_animsel.Text = swanim ? "��������� �������� � �������" : "�������� �������� � �������";
		}

		// ���������� ��������������
		_animselSwitch.IsToggled = swanim;
		_songselSwitch.IsToggled = swsongs;
	}

	private async void LanguageSwitch(object sender, EventArgs e)
	{
		var language = ((Button)sender).CommandParameter.ToString();
		Preferences.Default.Set("languagepickcheck", language);

		// ��������� ������ ����������� ��������
		_rustrue.Text = language == "�������" ? ">" : "";
		_entrue.Text = language == "English" ? ">" : "";

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

			_songsel.Text = language == "�������" ? "��������� ����" : "Turn off the sound";
			Preferences.Default.Set("swsongs", true);
		}
		else
		{
			if (!e.Value)
				_audioService.StopAllMusic();

			_songsel.Text = language == "�������" ? "�������� ����" : "Turn on the sound";
			Preferences.Default.Set("swsongs", false);
		}

	}
	private void UpdateThemeUI()
	{
		switch (_currentTheme)
		{
			case "swhitetheme.png":
				ApplyTheme(Colors.White, "whiteLabel", "whiteButton");
				break;
			case "spinktheme.png":
				ApplyTheme(Colors.Pink, "pinkLabel", "pinkButton");
				break;
			case "sblacktheme.png":
				ApplyTheme(Colors.Black, "blackLabel", "blackButton");
				break;
		}
	}

	private async void Button_Clicked(object sender, EventArgs e)
	{
		mainsettings.IsEnabled = false;
		await Navigation.PushModalAsync(new Menu(_audioService), animated: false);
	}

	private void ApplyTheme(Color backgroundColor, string labelStyleKey, string buttonStyleKey)
	{
		mainsettings.BackgroundColor = backgroundColor;

		var labelStyle = _cachedStyles[labelStyleKey];
		var buttonStyle = _cachedStyles[buttonStyleKey];

		_sellang.Style = labelStyle;
		_songsel.Style = labelStyle;
		_selecttheme.Style = labelStyle;
		_animsel.Style = labelStyle;
		_exit.Style = buttonStyle;
		_rustrue.Style = labelStyle;
		_entrue.Style = labelStyle;
		_lben.Style = labelStyle;
		_lbrus.Style = labelStyle;
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
			if (imagePath == mas)
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
					}


				}
			}

		}

	}

	private void anim_Toggled(object sender, ToggledEventArgs e)
	{
		var language = Preferences.Default.Get("languagepickcheck", "");

		if (_animselSwitch.IsToggled)
		{
			_animsel.Text = language == "�������" ?
				"��������� �������� � �������" : "Turn off animations and effects";
			Preferences.Default.Set("swanim", true);
		}
		else
		{
			_animsel.Text = language == "�������" ?
				"�������� �������� � �������" : "Enable animations and effects";
			Preferences.Default.Set("swanim", false);
		}
	}

}