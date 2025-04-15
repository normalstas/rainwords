using Microsoft.Maui.Controls;
using System.Numerics;

namespace rainwords;

public partial class CustomBuild : ContentPage
{
	private readonly IAudioService _audioService;
	public CustomBuild()
	{
		InitializeComponent();
		startgame();
	}

	private void startgame()
	{
		if (Preferences.Default.Get("languagepickcheck", "") == "English")
		{
			backcompl.Text = "Back";
			speedcustom.Text = "Choose a speed";
			entryspeed.Placeholder = "10-100000 the higher the slower";
			pointlb.Text = "Choose how many points per word";
			timelb.Text = "Choose the game time";
			entrytime.Placeholder = "1-10000 min";
			wordslb.Text = "Select the number of letters";
			customplay.Text = "Into the game";
		}

		switch (Preferences.Default.Get("selthemedate", ""))
		{
			case "swhitetheme.png":
				allpagefortheme.BackgroundColor = Colors.White;

				foreach (var x in allpagefortheme.Children.ToList()) { if (x is Button button) { button.Style = (Style)Resources["whitethemebutton"]; } }
				foreach (var x in allpagefortheme.Children.ToList()) { if (x is Label label) { label.Style = (Style)Resources["whitethemelabel"]; } }
				foreach (var x in allpagefortheme.Children.ToList()) { if (x is Entry entry) { entry.Style = (Style)Resources["whitethemeentry"]; entry.TextColor = Colors.Black; } }
				wordsell.Style = (Style)Resources["whitethemepicker"];

				break;
			case "spinktheme.png":
				allpagefortheme.BackgroundColor = Colors.Pink;

				foreach (var x in allpagefortheme.Children.ToList()) { if (x is Button button) { button.Style = (Style)Resources["pinkthemebutton"]; } }
				foreach (var x in allpagefortheme.Children.ToList()) { if (x is Label label) { label.Style = (Style)Resources["pinkthemelabel"]; } }
				foreach (var x in allpagefortheme.Children.ToList()) { if (x is Entry entry) { entry.Style = (Style)Resources["pinkthemeentry"]; entry.TextColor = Colors.White; } }
				wordsell.Style = (Style)Resources["pinkthemepicker"];
				break;
			case "sblacktheme.png":
				allpagefortheme.BackgroundColor = Colors.Black;

				foreach (var x in allpagefortheme.Children.ToList()) { if (x is Button button) { button.Style = (Style)Resources["blackthemebutton"]; } }
				foreach (var x in allpagefortheme.Children.ToList()) { if (x is Label label) { label.Style = (Style)Resources["blackthemelabel"]; } }
				foreach (var x in allpagefortheme.Children.ToList()) { if (x is Entry entry) { entry.Style = (Style)Resources["blackthemeentry"]; entry.TextColor = Colors.White; } }
				wordsell.Style = (Style)Resources["blackthemepicker"];
				break;
			default:
				break;
		}
	}

	private async void backcompl_Clicked(object sender, EventArgs e)
	{
		entryspeed.Unfocus();
		entrypoint.Unfocus();
		entrytime.Unfocus();
		await Navigation.PopModalAsync();
	}

	private void entryspeed_TextChanged(object sender, TextChangedEventArgs e)
	{
		var entr = sender as Entry;
		if (entr == null) return;
		switch (entr.Placeholder)
		{
			case "10-100000 чем выше тем медленнее":
				if (entr.Text.Length == 0 || entr.Text.Length > 100000) entr.TextColor = Colors.Red;
				else entr.TextColor = Colors.Black;
				break;
			case "1-10000":
				if (entr.Text.Length == 0 || entr.Text.Length > 10000) entr.TextColor = Colors.Red;
				else entr.TextColor = Colors.Black;
				break;
			case "1-10000 мин":
				if (entr.Text.Length == 0 || entr.Text.Length > 10000) entr.TextColor = Colors.Red;
				else entr.TextColor = Colors.Black;
				break;
			default:
				break;
		}
	}

	private void wordsell_SelectedIndexChanged(object sender, EventArgs e)
	{
		switch (wordsell.Title)
		{
			case "5":
				Data.compl = 0;
				break;
			case "7":
				Data.compl = 1;
				break;
			case "9":
				Data.compl = 2;
				break;
			default:
				break;
		}
	}

	private async void customplay_Clicked(object sender, EventArgs e)
	{
		Data.musplay = true;
		entryspeed.Unfocus();
		entrypoint.Unfocus();
		entrytime.Unfocus();
		Data.timecsm = int.Parse(entrytime.Text);
		Data.speedcsm = uint.Parse(entryspeed.Text);
		Data.pointcsm = int.Parse(entrypoint.Text);
		MainPage page = new MainPage(_audioService);
		await Navigation.PushModalAsync(page);
	}
}