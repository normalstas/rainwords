using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Numerics;

namespace rainwords;

public partial class Menu : ContentPage
{
	private readonly IAudioService _audioService;//переменная класса с музыкой для управления ею
	private MainPage page;//для сохранения игры
	private bool _isInitialized;//для определения включена или нет
	private bool _musicda = true; //чтобы не играла во время игры
	public Menu(IAudioService audioService)//создаем с параметром чтобы передавать музыку
	{
		InitializeComponent();
		_audioService = audioService; //инициализуем переменную
		Task.Run(() =>
		{
			InitializeDefaults();
			InitializeAudio();
			InitializeUI();
		});//ассинхронно включаем чтобы не лагало
		back.Text = "<"; //не можем так писать в XAML пишем тут
		App.GamePaused += PauseMenu;//подписываемся на событие чтобы при сворачивании приложения останавливалась музыка
		App.GameResumed += ResumeMenu;//включалась при открытии
	}

	private void PauseMenu() //есди свернута игра останавливаем музыку
	{
		_audioService.StopMenuMusic();
		_musicda = false;
	}

	private void ResumeMenu()//если заново открыли включаем музыку
	{
		if(!_musicda) _audioService.StartMenuMusic();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		// отписка от событий при уходе со страницы
		App.GamePaused -= PauseMenu;
		App.GameResumed -= ResumeMenu;
	}
	private void InitializeDefaults()
	{
		var currentTheme = Preferences.Default.Get("selthemedate", "");
		if (string.IsNullOrEmpty(currentTheme))//если только скачали приложение устанавливаем начальные настройки
		{
			Preferences.Default.Set("selthemedate", "swhitetheme.png");
			Preferences.Default.Set("languagepickcheck", "РУССКИЙ");
			Preferences.Default.Set("sweff", true);
			Preferences.Default.Set("swanim", true);
		}
	}

	private async Task InitializeAudio() //инициализация музыки
	{
		var audio = Preferences.Default.Get("swsongs", true);//проверяем включена ли музыка в настройках
		if (audio)//если да
		{
			await _audioService.InitializeAsync(); //создаем поток
			_audioService.PlayMenuMusic(); //начинаем воспроизводить
		}
		_isInitialized = true;//указываем что включили
	}

	private void InitializeUI()
	{
		Dispatcher.Dispatch(() =>//используется для главного потока UI
		{
			var language = Preferences.Default.Get("languagepickcheck", "");
			if (language == "ENGLISH")
			{
				UpdateTextToEnglish();//язык текста
			}

			ApplyTheme();//темы приложения
			btnlist.IsVisible = true;//сразу показываем начальные кнопки
		});
	}

	private void UpdateTextToEnglish()//обновляем текст если выбран англ
	{
		play.Text = "Play";
		setting.Text = "Setting";
		exit.Text = "Exit";
		confirmationexitone.Text = "Definitely get out?";
		confirmationexittwo.Text = "Your game will not be saved!";
		exitconf.Text = "Yes";
		non.Text = "No";
		selectcomplex.Text = "DIFFICULTY";
		contin.Text = "Continue";
		easy.Text = "Easy";
		average.Text = "Average";
		hard.Text = "Hard";
		custom.Text = "Custom";
	}
	private void ApplyTheme()//устанавливаем темы в зависимости от выбранного языка
	{
		var theme = Preferences.Default.Get("selthemedate", "");
		var buttons = btnlist.Children.OfType<Button>()
			.Concat(complex1.Children.OfType<Button>())
			.Concat(new[] { exitconf, non });

		string buttonStyleKey = "", labelStyleKey = "";
		string backgroundColor = "bgwhite.jpeg";
		Color BackInfor = Colors.White;
		Color StrokeInfo = Colors.Black;
		switch (theme)
		{
			case "swhitetheme.png":
				buttonStyleKey = "whitethemebutton";
				labelStyleKey = "whitethemelabel";
				backgroundColor = "bgwhite.jpeg";
				break;
			case "spinktheme.png":
				buttonStyleKey = "pinkthemebutton";
				labelStyleKey = "pinkthemelabel";
				backgroundColor = "bgpink.jpeg";
				BackInfor = Colors.HotPink;
				break;
			case "sblacktheme.png":
				buttonStyleKey = "blackthemebutton";
				labelStyleKey = "blackthemelabel";
				backgroundColor = "bgblack.jpeg";
				BackInfor = Colors.Black;
				StrokeInfo = Colors.White;
				break;
		}

		if (!string.IsNullOrEmpty(buttonStyleKey))
		{
			bg.Source = backgroundColor;
			var buttonStyle = (Style)Resources[buttonStyleKey];
			var labelStyle = (Style)Resources[labelStyleKey];
			confirmation.BackgroundColor = BackInfor;
			iamstroke.Stroke = StrokeInfo;
			foreach (var button in buttons)
			{
				button.Style = buttonStyle;
			}
			back.Style = labelStyle;
			confirmationexitone.Style = labelStyle;
			confirmationexittwo.Style = labelStyle;
			selectcomplex.Style = labelStyle;
		}
	}
	private async Task StartGame()//включаем игру
	{
		if (!_isInitialized) return;

		if (Preferences.Default.Get("swsongs", true))
		{
			_audioService.PlayGameMusic();//включаем музыку игры
		}

		page = new MainPage(_audioService);//запускаем игру
		await Navigation.PushModalAsync(page, animated: false);
	}
	private async void Play_Clicked(object sender, EventArgs e)//если нажали играть
	{
		back.IsVisible = true;//скрываем прошлые кнопки открываем сложности
		btnlist.IsVisible = false;
		btnlist.IsEnabled = false;
		complex1.IsVisible = true;
		complex1.IsEnabled = true;
		ApplyTheme();
	}

	private async void Setting_Clicked(object sender, EventArgs e)
	{
		btnlist.IsEnabled = false;
		await Navigation.PushModalAsync(new Settings(_audioService), animated: false);//открываем настройки
		
	}


	private void Exit_Clicked(object sender, EventArgs e)//выход 
	{
		if (contin.IsVisible)
		{//есди была запущена игра
			play.IsEnabled = false;
			setting.IsEnabled = false;
			exit.IsEnabled = false;
			confirmation.IsVisible = true;
		}
		else
		{
			Application.Current.Quit();//выход
		}
	}
	private void exitconf_Clicked(object sender, EventArgs e) => Application.Current.Quit();//тоже выход но уже точно уверены что игра будет потеряна

	private void cansel(object sender, EventArgs e)//если передумали выходить потому что игра осталась
	{
		play.IsEnabled = true;
		setting.IsEnabled = true;
		exit.IsEnabled = true;
		confirmation.IsVisible = false;
	}

		async private void buttoncomplex(object sender, EventArgs e)//определение сложности
	{
		int complexmenu;
		var button = sender as Button;
		btnlist.IsEnabled = complex1.IsEnabled = false;
		switch (button.CommandParameter)
		{
			case "playnext":
				back.IsVisible = false;
				Data.musplay = true;
				await Navigation.PushModalAsync(page, animated: false);
				break;
			case "playeasy":
				Data.musplay = true;
				complex1.IsEnabled = false;
				back.IsVisible = false;
				complexmenu = 0;
				Data.compl = complexmenu;
				Data.timecsm = 5;
				Data.speedcsm = 10000;
				Data.pointcsm = 20;
				await StartGame();
				//page = new MainPage(_audioService);
				//await Navigation.PushModalAsync(page);
				contin.IsVisible = true;
				break;
			case "playaverage":
				back.IsVisible = false;
				Data.musplay = true;
				complex1.IsEnabled = false;
				complexmenu = 1;
				Data.compl = complexmenu;
				Data.timecsm = 3;
				Data.speedcsm = 10000;
				Data.pointcsm = 30;
				await StartGame();
				//page = new MainPage(_audioService);
				//await Navigation.PushModalAsync(page);
				contin.IsVisible = true;
				break;
			case "playhard":
				Data.musplay = true;
				complex1.IsEnabled = false;
				back.IsVisible = false;
				complexmenu = 2;
				Data.compl = complexmenu;
				Data.timecsm = 2;
				Data.speedcsm = 10000;
				Data.pointcsm = 50;
				await StartGame();
				//page = new MainPage();
				//await Navigation.PushModalAsync(page);
				break;
			case "playcustom":
				back.IsVisible = false;
				CustomBuild f = new CustomBuild(_audioService);
				await Navigation.PushModalAsync(f);
				break;
			default:
				break;
		}

		complex1.IsVisible = false;
		complex1.IsEnabled = false;

		btnlist.IsVisible = true;
		btnlist.IsEnabled = true;

	}

	private void back_menu(object sender, EventArgs e)//выход из сложности
	{
		back.IsVisible = false;
		complex1.IsVisible = false;
		complex1.IsEnabled = false;
		btnlist.IsVisible = true;
		btnlist.IsEnabled = true;
		ApplyTheme();
	}
}