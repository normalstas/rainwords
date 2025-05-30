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
		backcompl.Text = "<";
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

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		// ������� �� ������� ��� ����� �� ��������
		App.GamePaused -= PauseMenu;
		App.GameResumed -= ResumeMenu;
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
		allpagefortheme.IsEnabled = true;

		var landuage = Preferences.Default.Get("languagepickcheck", "");
		if (landuage == "ENGLISH")
		{
			//backcompl.Text = "Back";
			speedcustom.Text = "SPEED";
			entryspeed.Placeholder = "10-100000";
			pointlb.Text = "POINTS";
			timelb.Text = "TIME";
			entrytime.Placeholder = "1-10000 min";
			wordslb.Text = "NUMBER OF LETTERS";
			customplay.Text = "GAME";
			titlelb.Text = "CUSTOM MODE";
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
			"whitetheme" => Colors.White,
			"pinktheme" => Colors.HotPink,
			"blacktheme" => Colors.Black,
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
					entry.TextColor = themePrefix == "blacktheme" ? Colors.Black : Colors.White;
					break;
				case Frame frame:
					frame.BackgroundColor = themePrefix == "blacktheme" ? Colors.Black : Colors.White;
					break;
				case Border border:
					border.Stroke = themePrefix == "blacktheme" ? Colors.White : Colors.Black;
					break;
			}
		}

		backcompl.TextColor = themePrefix == "blacktheme" ? Colors.White : Colors.Black;
		titlelb.TextColor = themePrefix == "blacktheme" ? Colors.White : Colors.Black; 
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
			"10-100000 ��� ���� ��� ���������" or
			"1-10000" or
			"1-10000 ���" => !string.IsNullOrEmpty(entry.Text) && entry.Text.Length <= 10000,
			_ => true
		};
		entry.TextColor = isValid ? (allpagefortheme.BackgroundColor == Colors.White ? Colors.Black : Colors.White) : Colors.Red;
	}

	private async void customplay_Clicked(object sender, EventArgs e)
	{
		if (!ValidateInputs()) return;

		allpagefortheme.IsEnabled = false;
		Data.musplay = true;
		UnfocusAll();
		StringBuilder error = new StringBuilder();
		if (!int.TryParse(entrypoint.Text, out int a) || a > 10000)
		{
			error.AppendLine("������,�������� ����� �� ����");
			entrypoint.Text = string.Empty;
		}
		if (!int.TryParse(entrytime.Text, out int b) || b > 10000)
		{
			error.AppendLine("������,�������� ����� �������");
			entrytime.Text = string.Empty;
		}
		if ( !int.TryParse(entryspeed.Text, out int c) || c > 100000)
		{
			error.AppendLine("������,�������� ����� ��������");
			entryspeed.Text = string.Empty;
		}
		if (error.Length > 0)
		{
			await DisplayAlert("",error.ToString(),"��");
			return;
		}
		Data.timecsm = int.Parse(entrytime.Text);
		Data.speedcsm = uint.Parse(entryspeed.Text);
		Data.pointcsm = int.Parse(entrypoint.Text);

		var page = new MainPage(_audioService);
		await Navigation.PushModalAsync(page, animated: false);
		allpagefortheme.IsEnabled = true;
	}

	private void UnfocusAll()
	{
		entryspeed.Unfocus();
		entrypoint.Unfocus();
		entrytime.Unfocus();
		entrycount.Unfocus();
	}

	private bool ValidateInputs()
	{
		return !string.IsNullOrEmpty(entrytime.Text) &&
			   !string.IsNullOrEmpty(entryspeed.Text) &&
			   !string.IsNullOrEmpty(entrypoint.Text);
	}
}