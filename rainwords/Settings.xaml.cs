

namespace rainwords;

public partial class Settings : ContentPage
{
	public Settings()
	{
		InitializeComponent();
		startgame();


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
			if (mas == null)
			{
				imgbtncreate.Source = "swhitetheme.png";
				Preferences.Default.Set("selthemedate", imgbtncreate.Source.ToString().Remove(0, 6));

			}
			if ("s" + datetheme[i] == mas)
			{
				imgbtncreate.Source = "s" + datetheme[i];

			}
			Grid.SetColumn(imgbtncreate, i);
			imgbtncreate.Clicked += Image_Clicked;
			theme.Children.Add(imgbtncreate);
		}


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
	private void language_SelectedIndexChanged(object sender, EventArgs e)
	{
		Preferences.Default.Set("languagepickcheck", language.SelectedItem.ToString());
		startgame();

	}


	void startgame()
	{
		

		if (Preferences.Default.Get("swsongs", true))
		{
			if (Preferences.Default.Get("languagepickcheck", "") == "Русский") { songsel.Text = "Выключить звук"; }
			if (Preferences.Default.Get("languagepickcheck", "") == "English") { songsel.Text = "Turn off the sound"; }
			
		}
		else
		{
			if (Preferences.Default.Get("languagepickcheck", "") == "Русский") { songsel.Text = "Включить звук"; }
			if (Preferences.Default.Get("languagepickcheck", "") == "English") { songsel.Text = "Turn on the sound"; }
			

		}

		language.Title = Preferences.Default.Get("languagepickcheck", "");
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

				//foreach (var x in mainsettings.Children.ToList() ) { if (x is Label label) { label.Style = (Style)Resources["whitethemelabel"]; } }

				sellang.Style = (Style)Resources["whitethemelabel"];
				songsel.Style = (Style)Resources["whitethemelabel"];
				selecttheme.Style = (Style)Resources["whitethemelabel"];

				exit.Style = (Style)Resources["whitethemebutton"];
				language.TitleColor = Colors.Black;
				//songsel_switch.Style = (Style)Resources["whitethemeswitch"];

				language.Style = (Style)Resources["whitethemepicker"];
				break;
			case "spinktheme.png":
				mainsettings.BackgroundColor = Colors.Pink;

				sellang.Style = (Style)Resources["pinkthemelabel"];
				songsel.Style = (Style)Resources["pinkthemelabel"];
				selecttheme.Style = (Style)Resources["pinkthemelabel"];

				exit.Style = (Style)Resources["pinkthemebutton"];
				language.TitleColor = Colors.White;
				language.Style = (Style)Resources["pinkthemepicker"];
				break;
			case "sblacktheme.png":
				mainsettings.BackgroundColor = Colors.Black;

				sellang.Style = (Style)Resources["blackthemelabel"];
				songsel.Style = (Style)Resources["blackthemelabel"];
				selecttheme.Style = (Style)Resources["blackthemelabel"];

				exit.Style = (Style)Resources["blackthemebutton"];
				language.TitleColor = Colors.White;
				language.Style = (Style)Resources["blackthemepicker"];


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
}