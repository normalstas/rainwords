using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace rainwords;

public partial class CustomBuild : ContentPage
{
	private readonly IAudioService _audioService;
	private bool _isInitialized;
	public CustomBuild(IAudioService audioService)
	{
		_audioService = audioService;
		var stopwatch = Stopwatch.StartNew();
		InitializeComponent();
		Task.Run(() =>
		{
			InitializeAudio();
			Dispatcher.Dispatch(startgame);
		});
		stopwatch.Stop();
		Console.WriteLine($"Settings loaded in {stopwatch.ElapsedMilliseconds} ms");
	}
	private async void InitializeAudio()
	{
		if (_isInitialized) return;

		if (Preferences.Default.Get("swsongs", true))
		{
			await _audioService.InitializeAsync();
			_audioService.PlayMenuMusic();
		}
		else
		{
			_audioService.IsMusicEnabled = false;
		}
		_isInitialized = true;
	}

	private void startgame()
	{
		backcompl.IsEnabled = true;
		entrypoint.IsEnabled = true;
		entryspeed.IsEnabled = true;
		entrytime.IsEnabled = true;
		wordsell.IsEnabled = true;
		customplay.IsEnabled = true;

		var landuage = Preferences.Default.Get("languagepickcheck", "");
		if (landuage == "English")
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
		var theme = Preferences.Default.Get("selthemedate", "");
		if (string.IsNullOrEmpty(theme)) return;

		var themePrefix = theme.Replace("stheme.png", "").Replace("s", "").Replace(".png", "");
		ApplyTheme(themePrefix);
	}

	private void ApplyTheme(string themePrefix)
	{
		allpagefortheme.BackgroundColor = themePrefix switch
		{
			"white" => Colors.White,
			"pink" => Colors.Pink,
			"black" => Colors.Black,
			_ => allpagefortheme.BackgroundColor
		};

		foreach (var child in allpagefortheme.Children)
		{
			switch (child)
			{
				case Button button:
					button.Style = (Style)Resources[$"{themePrefix}button"];
					break;
				case Label label:
					label.Style = (Style)Resources[$"{themePrefix}label"];
					break;
				case Entry entry:
					entry.Style = (Style)Resources[$"{themePrefix}entry"];
					entry.TextColor = themePrefix == "white" ? Colors.Black : Colors.White;
					break;
			}
		}


		wordsell.Style = (Style)Resources[$"{themePrefix}picker"];
	}

	private async void backcompl_Clicked(object sender, EventArgs e)
	{
		UnfocusAll();
		await Navigation.PopModalAsync(animated: false);
	}

	private void entryspeed_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (sender is not Entry entry) return;

		var isValid = entry.Placeholder switch
		{
			"10-100000 чем выше тем медленнее" or
			"1-10000" or
			"1-10000 мин" => !string.IsNullOrEmpty(entry.Text) && entry.Text.Length <= 10000,
			_ => true
		};
		entry.TextColor = isValid ? (allpagefortheme.BackgroundColor == Colors.White ? Colors.Black : Colors.White) : Colors.Red;
	}

	private void wordsell_SelectedIndexChanged(object sender, EventArgs e)
	{
		UnfocusAll();
		Data.compl = wordsell.SelectedIndex;
	}

	private async void customplay_Clicked(object sender, EventArgs e)
	{
		if (!ValidateInputs()) return;

		Data.musplay = true;
		UnfocusAll();
		DisableAllControls();
		StringBuilder error = new StringBuilder();
		if (string.IsNullOrEmpty(entrypoint.Text) || !int.TryParse(entrypoint.Text, out int a) || a > 10000)
		{
			error.AppendLine("Ошибка,неверное число за очки");
			entrypoint.Text = string.Empty;
		}
		if (string.IsNullOrEmpty(entrytime.Text) || !int.TryParse(entrytime.Text, out int b) || b > 10000)
		{
			error.AppendLine("Ошибка,неверное число времени");
			entrytime.Text = string.Empty;
		}
		if (string.IsNullOrEmpty(entryspeed.Text) || !int.TryParse(entryspeed.Text, out int c) || c > 100000)
		{
			error.AppendLine("Ошибка,неверное число скорости");
			entryspeed.Text = string.Empty;
		}
		if (error.Length > 0)
		{
			await DisplayAlert("",error.ToString(),"ок");
			return;
		}
		Data.timecsm = int.Parse(entrytime.Text);
		Data.speedcsm = uint.Parse(entryspeed.Text);
		Data.pointcsm = int.Parse(entrypoint.Text);

		var page = new MainPage(_audioService);
		await Navigation.PushModalAsync(page, animated: false);
	}

	private void UnfocusAll()
	{
		entryspeed.Unfocus();
		entrypoint.Unfocus();
		entrytime.Unfocus();
		wordsell.Unfocus();
	}

	private void DisableAllControls()
	{
		backcompl.IsEnabled = false;
		entrypoint.IsEnabled = false;
		entryspeed.IsEnabled = false;
		entrytime.IsEnabled = false;
		wordsell.IsEnabled = false;
		customplay.IsEnabled = false;
	}

	private bool ValidateInputs()
	{
		return !string.IsNullOrEmpty(entrytime.Text) &&
			   !string.IsNullOrEmpty(entryspeed.Text) &&
			   !string.IsNullOrEmpty(entrypoint.Text);
	}
}