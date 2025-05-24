using System.Diagnostics;

namespace rainwords;
public partial class Settings : ContentPage
{
	private readonly IAudioService _audioService;
	public Settings(IAudioService audioService)
	{
		var stopwatch = Stopwatch.StartNew();
		InitializeComponent();
		stopwatch.Stop();
		Console.WriteLine($"Settings loaded in {stopwatch.ElapsedMilliseconds} ms");
		_audioService = audioService;
		if (Preferences.Default.Get("swsongs", false) == false)
		{
			songsel_switch.IsToggled = false;
		}
		else
		{
			songsel_switch.IsToggled = true;
		}
		//songsel_switch.IsToggled = _audioService.IsMusicEnabled;
		string[] datetheme = new string[] { "whitetheme.png", "pinktheme.png", "blacktheme.png" };
		var currentTheme = Preferences.Default.Get("selthemedate", "");
		for (int i = 0; i < datetheme.Length; i++)
		{
			var imgbtncreate = new ImageButton
			{
				CornerRadius = 20,
				WidthRequest = 51,
				HeightRequest = 51,
				BackgroundColor = Colors.Transparent,
				Source = datetheme[i],


			};
			if ("s" + datetheme[i] == currentTheme)
			{
				imgbtncreate.Source = "s" + datetheme[i];

			}
			Grid.SetColumn(imgbtncreate, i);
			imgbtncreate.Clicked += Image_Clicked;
			theme.Children.Add(imgbtncreate);
			startgame();
		}


	}

	private async void LanguageSwitch(object sender, EventArgs e)
	{
		var language = ((Button)sender).CommandParameter.ToString();
		Preferences.Default.Set("languagepickcheck", language);
		rustrue.Text = language == "Русский" ? ">" : "";
		entrue.Text = language == "English" ? ">" : "";

		startgame();
	}

	private void song_Toggled(object sender, ToggledEventArgs e)
	{
		_audioService.IsMusicEnabled = e.Value;
		var language = Preferences.Default.Get("languagepickcheck", "");
		if (songsel_switch.IsToggled)
		{
			if (e.Value && Application.Current.MainPage is Menu)
				_audioService.PlayMenuMusic();
			if (e.Value && Application.Current.MainPage is MainPage)
				_audioService.PlayGameMusic();
			if (language == "Русский") { songsel.Text = "Выключить звук"; }
			if (language == "English") { songsel.Text = "Turn off the sound"; }
			Preferences.Default.Set("swsongs", true);
		}
		else
		{
			if (!e.Value)
				_audioService.StopAllMusic();
			if (language == "Русский") { songsel.Text = "Включить звук"; }
			if (language == "English") { songsel.Text = "Turn on the sound"; }
			Preferences.Default.Set("swsongs", false);

		}

	}

	private async void Button_Clicked(object sender, EventArgs e)
	{
		mainsettings.IsEnabled = false;
		await Navigation.PushModalAsync(new Menu(_audioService), animated: false);
	}

	void startgame()
	{
		var language = Preferences.Default.Get("languagepickcheck", "");
		var swanim = Preferences.Default.Get("swanim", true);
		var swsongs = Preferences.Default.Get("swsongs", true);
		if (swanim)
		{
			if (language == "Русский") { animsel.Text = "Выключить анимации и эффекты"; }
			if (language == "English") { animsel.Text = "Turn off animations and effects"; }
			animsel_switch.IsToggled = true;
		}
		else
		{
			if (language == "Русский") { animsel.Text = "Включить анимации и эффекты"; }
			if (language == "English") { animsel.Text = "Enable animations and effects"; }
			animsel_switch.IsToggled = false;
		}
		if (swsongs)
		{
			if (language == "Русский") { songsel.Text = "Выключить звук"; }
			if (language == "English") { songsel.Text = "Turn off the sound"; }
			songsel_switch.IsToggled = true;
		}
		else
		{
			if (language == "Русский") { songsel.Text = "Включить звук"; }
			if (language == "English") { songsel.Text = "Turn on the sound"; }
			songsel_switch.IsToggled = false;
		}

		if (language == "English")
		{
			rustrue.Text = "";
			entrue.Text = ">";
		}
		else
		{
			rustrue.Text = ">";
			entrue.Text = "";
		}

		if (language == "English")
		{
			sellang.Text = "Choose a language";
			exit.Text = "Save and exit";
			selecttheme.Text = "choose a theme";
		}
		if (language == "Русский")
		{
			sellang.Text = "Выбрать язык";
			exit.Text = "Сохранить и выйти";
			selecttheme.Text = "Выбрать тему";

		}




		mas = Preferences.Default.Get("selthemedate", "");
		switch (mas)
		{
			case "swhitetheme.png":
				mainsettings.BackgroundColor = Colors.White;
				var whiteLabelStyle = (Style)Resources["whitethemelabel"];
				foreach (var label in mainsettings.Children.OfType<Label>()) { { label.Style = whiteLabelStyle; ; } }

				sellang.Style = whiteLabelStyle;
				songsel.Style = whiteLabelStyle;
				selecttheme.Style = whiteLabelStyle; 
				animsel.Style = whiteLabelStyle;
				exit.Style = (Style)Resources["whitethemebutton"];
				rustrue.Style = whiteLabelStyle;
				entrue.Style = whiteLabelStyle;
				lben.Style = whiteLabelStyle;
				lbrus.Style = whiteLabelStyle;

				break;
			case "spinktheme.png":
				mainsettings.BackgroundColor = Colors.Pink;
				var pinkLabelStyle = (Style)Resources["pinkthemelabel"];
				sellang.Style = pinkLabelStyle;
				songsel.Style = pinkLabelStyle;
				selecttheme.Style = pinkLabelStyle;
				animsel.Style = pinkLabelStyle;
				exit.Style = (Style)Resources["pinkthemebutton"];
				rustrue.Style = pinkLabelStyle;
				entrue.Style = pinkLabelStyle;
				lben.Style = pinkLabelStyle;
				lbrus.Style = pinkLabelStyle;
				break;
			case "sblacktheme.png":
				mainsettings.BackgroundColor = Colors.Black;
				var blackLabelStyle = (Style)Resources["blackthemelabel"];
				sellang.Style = blackLabelStyle;
				songsel.Style = blackLabelStyle;
				selecttheme.Style = blackLabelStyle;
				animsel.Style = blackLabelStyle;
				exit.Style = (Style)Resources["blackthemebutton"];
				rustrue.Style = blackLabelStyle;
				entrue.Style = blackLabelStyle;
				lben.Style = blackLabelStyle;
				lbrus.Style = blackLabelStyle;
				break;
			default:
				break;
		}
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
							if (y is ImageButton btndel && btndel.Source.ToString().Remove(0, 6) == mas)
							{
								btndel.Source = mas.Remove(0, 1);
							}
						}

						mas = "s" + imagePath;
						imagebutton.Source = mas;
						Preferences.Default.Set("selthemedate", imagebutton.Source.ToString().Remove(0, 6));
						startgame();
					}


				}
			}

		}

	}

	private void anim_Toggled(object sender, ToggledEventArgs e)
	{
		var language = Preferences.Default.Get("languagepickcheck", "");
		if (animsel_switch.IsToggled)
		{
			if (language == "Русский") { animsel.Text = "Выключить анимации и эффекты"; }
			if (language == "English") { animsel.Text = "Turn off animations and effects"; }
			Preferences.Default.Set("swanim", true);
		}
		else
		{
			if (language == "Русский") { animsel.Text = "Включить анимации и эффекты"; }
			if (language == "English") { animsel.Text = "Enable animations and effects"; }
			Preferences.Default.Set("swanim", false);

		}
	}

}