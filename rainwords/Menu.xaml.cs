using IntelliJ.Lang.Annotations;
using Microsoft.Maui.Controls;
using System.Numerics;

namespace rainwords;

public partial class Menu : ContentPage
{
	public Menu()
	{
		InitializeComponent();
		startgame();
	}
	MainPage page = new MainPage();
	string mas;
	private void Play_Clicked(object sender, EventArgs e)
	{
		
		btnlist.IsVisible = false;
		btnlist.IsEnabled = false;
		complex1.IsVisible = true;
		complex1.IsEnabled = true;
	}

	private async void Setting_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new Settings());
		
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
		
		switch (button.Text)
		{
			case "Продолжить":
				await Navigation.PushModalAsync(page);
				break;
			case "Легкая":
				complexmenu = 0;
				Data.compl = complexmenu;
				page = new MainPage();
				await Navigation.PushModalAsync(page);

				contin.IsVisible = true;

				complex1.IsVisible = false;
				complex1.IsEnabled = false;

				btnlist.IsVisible = true;
				btnlist.IsEnabled = true;

				
				break;
			case "Easy":
				complexmenu = 0;
				Data.compl = complexmenu;
				page = new MainPage();
				await Navigation.PushModalAsync(page);

				contin.IsVisible = true;

				complex1.IsVisible = false;
				complex1.IsEnabled = false;

				btnlist.IsVisible = true;
				btnlist.IsEnabled = true;


				break;
			case "Средняя":
				complexmenu = 1;
				Data.compl = complexmenu;
				page = new MainPage();
				await Navigation.PushModalAsync(page);

				contin.IsVisible = true;

				complex1.IsVisible = false;
				complex1.IsEnabled = false;

				btnlist.IsVisible = true;
				btnlist.IsEnabled = true;

				break;
			case "Сложная":
				complexmenu = 2;
				Data.compl = complexmenu;
				page = new MainPage();
				await Navigation.PushModalAsync(page);

				contin.IsVisible = true;

				complex1.IsVisible = false;
				complex1.IsEnabled = false;

				btnlist.IsVisible = true;
				btnlist.IsEnabled = true;

				break;
			default:
				break;
		}
		
	}

	private void back_menu(object sender, EventArgs e)
	{
		complex1.IsVisible = false;
		complex1.IsEnabled = false;
		btnlist.IsVisible = true;
		btnlist.IsEnabled = true;
	}
	
	void startgame()
	{
		//if (Preferences.Default.Get("swsongs", true))
		//{
			
		//	Preferences.Default.Set("swsongscheck", true);
		//}
		//else
		//{
			
		//	Preferences.Default.Set("swsongscheck", false);
		//}


		//if (Preferences.Default.Get("languagepick", "") == "Русский")
		//{


		//	Preferences.Default.Set("languagepickcheck", "Русский");
		//	play.Text = "Играть";
		//	setting.Text = "Настройки";
		//	exit.Text = "Выход";
		//	confirmationexit.Text = "Точно выйти? Ваша игра не будет сохранена";
		//	exitconf.Text = "Да";
		//	non.Text = "Нет";
		//	back.Text = "Назад";
		//	selectcomplex.Text = "Выберите сложность";
		//	contin.Text = "Продолжить";
		//	easy.Text = "Легкая";
		//	average.Text = "Средняя";
		//	hard.Text = "Сложная";
		//}
		if (Preferences.Default.Get("languagepickcheck", "") == "English")
		{
			//Preferences.Default.Set("languagepickcheck", "English");
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
		}

		switch (Preferences.Default.Get("selthemedate", ""))
		{
			case "swhitetheme.png":
				allpagefortheme.BackgroundColor = Colors.White;

				foreach (var x in btnlist.Children.ToList()) { if (x is Button button) { button.Style = (Style)Resources["whitethemebutton"]; } }
				foreach (var x in complex1.Children.ToList()) { if (x is Button button) { button.Style = (Style)Resources["whitethemebutton"]; } }
				exitconf.Style = (Style)Resources["whitethemebutton"];
				non.Style = (Style)Resources["whitethemebutton"];


				confirmationexit.Style = (Style)Resources["whitethemelabel"];
				selectcomplex.Style = (Style)Resources["whitethemelabel"];

				break;
			case "spinktheme.png":
				allpagefortheme.BackgroundColor = Colors.Pink;

				foreach (var x in btnlist.Children.ToList()) { if (x is Button button) { button.Style = (Style)Resources["pinkthemebutton"]; } }
				foreach (var x in complex1.Children.ToList()) { if (x is Button button) { button.Style = (Style)Resources["pinkthemebutton"]; } }
				exitconf.Style = (Style)Resources["pinkthemebutton"];
				non.Style = (Style)Resources["pinkthemebutton"];


				confirmationexit.Style = (Style)Resources["pinkthemelabel"];
				selectcomplex.Style = (Style)Resources["pinkthemelabel"];


				break;
			case "sblacktheme.png":
				allpagefortheme.BackgroundColor = Colors.Black;

				foreach (var x in btnlist.Children.ToList()) { if (x is Button button) { button.Style = (Style)Resources["blackthemebutton"]; } }
				foreach (var x in complex1.Children.ToList()) { if (x is Button button) { button.Style = (Style)Resources["blackthemebutton"]; } }
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