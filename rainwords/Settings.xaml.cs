namespace rainwords;

public partial class Settings : ContentPage
{
	public Settings()
	{
		InitializeComponent();

		string[] datetheme = new string[] { "whitetheme.png", "pinktheme.png", "blacktheme.png" };
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
			if (Preferences.Default.Get("selthemedate", "") == null)
			{
				imgbtncreate.Source = "swhitetheme.png";
				Preferences.Default.Set("selthemedate", imgbtncreate.Source.ToString().Remove(0, 6));
				Preferences.Default.Set("languagepickcheck", "Русский");
				Preferences.Default.Set("sweff", true);
				Preferences.Default.Set("swanim", true);
			}
			if ("s" + datetheme[i] == mas)
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
		var btn = sender as Button;
		if (btn == null) return;
		if (btn.CommandParameter.ToString() == "en")
		{
			enbtn.Margin = new Thickness(20, 0);
			rusbtn.Margin = new Thickness(0);
			rustrue.Text = "";
			entrue.Text = ">";
			Preferences.Default.Set("languagepickcheck", "English");
		}
		else
		{
			rusbtn.Margin = new Thickness(20, 0);
			enbtn.Margin = new Thickness(0);
			rustrue.Text = ">";
			entrue.Text = "";
			Preferences.Default.Set("languagepickcheck", "Русский");
		}
		startgame();
	}

	private void song_Toggled(object sender, ToggledEventArgs e)
	{
		if (songsel_switch.IsToggled)
		{
			if (Preferences.Default.Get("languagepickcheck", "") == "Русский") { songsel.Text = "Выключить звук"; }
			if (Preferences.Default.Get("languagepickcheck", "") == "English") { songsel.Text = "Turn off the sound"; }
			Preferences.Default.Set("swsongs", true);
		}
		else
		{
			if (Preferences.Default.Get("languagepickcheck", "") == "Русский") { songsel.Text = "Включить звук"; }
			if (Preferences.Default.Get("languagepickcheck", "") == "English") { songsel.Text = "Turn on the sound"; }
			Preferences.Default.Set("swsongs", false);

		}

	}

	private async void Button_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new Menu());
	}

	void startgame()
	{
		if (Preferences.Default.Get("swanim", true))
		{
			if (Preferences.Default.Get("languagepickcheck", "") == "Русский") { animsel.Text = "Выключить анимации и эффекты"; }
			if (Preferences.Default.Get("languagepickcheck", "") == "English") { animsel.Text = "Turn off animations and effects"; }
			animsel_switch.IsToggled = true;
		}
		else
		{
			if (Preferences.Default.Get("languagepickcheck", "") == "Русский") { animsel.Text = "Включить анимации и эффекты"; }
			if (Preferences.Default.Get("languagepickcheck", "") == "English") { animsel.Text = "Enable animations and effects"; }
			animsel_switch.IsToggled = false;
		}
		if (Preferences.Default.Get("swsongs", true))
		{
			if (Preferences.Default.Get("languagepickcheck", "") == "Русский") { songsel.Text = "Выключить звук"; }
			if (Preferences.Default.Get("languagepickcheck", "") == "English") { songsel.Text = "Turn off the sound"; }
			songsel_switch.IsToggled = true;
		}
		else
		{
			if (Preferences.Default.Get("languagepickcheck", "") == "Русский") { songsel.Text = "Включить звук"; }
			if (Preferences.Default.Get("languagepickcheck", "") == "English") { songsel.Text = "Turn on the sound"; }
			songsel_switch.IsToggled = false;
		}

		if (Preferences.Default.Get("languagepickcheck", "") == "English")
		{
			rustrue.Text = "";
			entrue.Text = ">";
		}
		else
		{
			rustrue.Text = ">";
			entrue.Text = "";
		}

		if (Preferences.Default.Get("languagepickcheck", "") == "English")
		{
			sellang.Text = "Choose a language";
			exit.Text = "Save and exit";
			selecttheme.Text = "choose a theme";
		}
		if (Preferences.Default.Get("languagepickcheck", "") == "Русский")
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

				foreach (var x in mainsettings.Children.ToList()) { if (x is Label label) { label.Style = (Style)Resources["whitethemelabel"]; } }

				sellang.Style = (Style)Resources["whitethemelabel"];
				songsel.Style = (Style)Resources["whitethemelabel"];
				selecttheme.Style = (Style)Resources["whitethemelabel"];
				animsel.Style = (Style)Resources["whitethemelabel"];
				exit.Style = (Style)Resources["whitethemebutton"];
				rustrue.Style = (Style)Resources["whitethemelabel"];
				entrue.Style = (Style)Resources["whitethemelabel"];
				lben.Style = (Style)Resources["whitethemelabel"];
				lbrus.Style = (Style)Resources["whitethemelabel"];

				break;
			case "spinktheme.png":
				mainsettings.BackgroundColor = Colors.Pink;

				sellang.Style = (Style)Resources["pinkthemelabel"];
				songsel.Style = (Style)Resources["pinkthemelabel"];
				selecttheme.Style = (Style)Resources["pinkthemelabel"];
				animsel.Style = (Style)Resources["pinkthemelabel"];
				exit.Style = (Style)Resources["pinkthemebutton"];
				rustrue.Style = (Style)Resources["pinkthemelabel"];
				entrue.Style = (Style)Resources["pinkthemelabel"];
				lben.Style = (Style)Resources["pinkthemelabel"];
				lbrus.Style = (Style)Resources["pinkthemelabel"];
				break;
			case "sblacktheme.png":
				mainsettings.BackgroundColor = Colors.Black;
				sellang.Style = (Style)Resources["blackthemelabel"];
				songsel.Style = (Style)Resources["blackthemelabel"];
				selecttheme.Style = (Style)Resources["blackthemelabel"];
				animsel.Style = (Style)Resources["blackthemelabel"];
				exit.Style = (Style)Resources["blackthemebutton"];
				rustrue.Style = (Style)Resources["blackthemelabel"];
				entrue.Style = (Style)Resources["blackthemelabel"];
				lben.Style = (Style)Resources["blackthemelabel"];
				lbrus.Style = (Style)Resources["blackthemelabel"];
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
		if (animsel_switch.IsToggled)
		{
			if (Preferences.Default.Get("languagepickcheck", "") == "Русский") { animsel.Text = "Выключить анимации и эффекты"; }
			if (Preferences.Default.Get("languagepickcheck", "") == "English") { animsel.Text = "Turn off animations and effects"; }
			Preferences.Default.Set("swanim", true);
		}
		else
		{
			if (Preferences.Default.Get("languagepickcheck", "") == "Русский") { animsel.Text = "Включить анимации и эффекты"; }
			if (Preferences.Default.Get("languagepickcheck", "") == "English") { animsel.Text = "Enable animations and effects"; }
			Preferences.Default.Set("swanim", false);

		}
	}

}