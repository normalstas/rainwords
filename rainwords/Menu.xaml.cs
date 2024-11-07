using System.Numerics;

namespace rainwords;

public partial class Menu : ContentPage
{
	public Menu()
	{
		InitializeComponent();
	}

	private void Play_Clicked(object sender, EventArgs e)
	{
		
		btnlist.IsVisible = false;
		btnlist.IsEnabled = false;
		complex1.IsVisible = true;
		complex1.IsEnabled = true;
	}

	private void Setting_Clicked(object sender, EventArgs e)
	{

	}

	private void Exit_Clicked(object sender, EventArgs e)
	{

	}

	async private void buttoncomplex(object sender, EventArgs e)
	{
		int complexmenu;
		var button = sender as Button;
		
		switch (button.Text)
		{
			case "Продолжить":
				await Navigation.PushModalAsync(new MainPage());
				break;
			case "Легкая":
				await Navigation.PushModalAsync(new MainPage());
				complexmenu = 0;
				Data.compl = complexmenu;

				contin.IsVisible = true;

				complex1.IsVisible = false;
				complex1.IsEnabled = false;

				btnlist.IsVisible = true;
				btnlist.IsEnabled = true;

				
				break;
			case "Средняя":
				await Navigation.PushModalAsync(new MainPage());
				complexmenu = 1;
				Data.compl = complexmenu;

				contin.IsVisible = true;

				complex1.IsVisible = false;
				complex1.IsEnabled = false;

				btnlist.IsVisible = true;
				btnlist.IsEnabled = true;

				break;
			case "Сложная":
				await Navigation.PushModalAsync(new MainPage());
				complexmenu = 2;
				Data.compl = complexmenu;

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
}