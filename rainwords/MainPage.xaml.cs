using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Numerics;



namespace rainwords
{

	public partial class MainPage : ContentPage
	{
		private readonly IAudioService _audioService;
		public MainPage(IAudioService audioService)
		{
			var stopwatch = Stopwatch.StartNew();
			InitializeComponent();
			stopwatch.Stop();
			Console.WriteLine($"Settings loaded in {stopwatch.ElapsedMilliseconds} ms");
			_audioService = audioService;
			CreatingLabels.Initialize(10);
			InitializeAudio();
			LoadKeyboard();
			ApplyTheme();
			StartTimers();
		}


		void LoadKeyboard()
		{
			labels = new List<Label> { cell1, cell2, cell3, cell4, cell5, cell6, cell7, cell8, cell9 };
			int columns = 11;
			int rows = (int)Math.Ceiling(33.0 / columns);
			char[] letters = "йцукенгшщзхфывапролджэ ячсмитьбю".ToCharArray();
			var language = Preferences.Default.Get("languagepickcheck", "");
			var theme = Preferences.Default.Get("selthemedate", "");
			if (language == "English")
			{
				columns = 10;
				letters = "qwertyuiopasdfghjkl zxcvbnm".ToCharArray();
				Grid.SetRow(clearone, 2);
				Grid.SetColumnSpan(clearone, 2);
				Grid.SetRow(clear, 2);
				Grid.SetColumn(clear, 9);
				Grid.SetColumnSpan(clear, 2);
				clearone.Text = "clear";
				clear.Text = "all";
				paus.Text = "pause";
				continuebtn.Text = "continue";
				exit.Text = "exit";
				pause.Text = "p";
			}

			for (int i = 0; i < letters.Length; i++)
			{
				var button = new Button
				{
					Text = letters[i].ToString(),
					TextColor = Colors.Transparent,
					BorderWidth = 2,
					FontSize = 18,
					CornerRadius = 5,
					Style = GetTextColorByThemeButton()
				};

				var label = new Label
				{
					Text = letters[i].ToString(),
					Style = (Style)Resources["lbletter"],
					BackgroundColor = Colors.Transparent,
					TextColor = GetTextColorByTheme()
				};
				int row = i / columns;
				int column = i % columns;
				if (button.Text == " ")
				{
					label.IsVisible = false;
					label.IsVisible = false;
					button.IsEnabled = false;
					button.IsVisible = false;
				}

				if (language == "English")
				{
					button.WidthRequest = 30;
					button.HeightRequest = 50;
					if (row == 0)
					{
						button.Margin = new Thickness(50000, 0);
					}
					if (row == 1)
					{
						button.Margin = new Thickness(20, 0);
						column += 1;
					}
					if (row == 2)
					{

						if (column >= 0 && column <= 6)
						{
							column += 2;
						}
					}
				}
				button.Clicked += Button_Clicked;
				Grid.SetRow(button, row);
				Grid.SetColumn(button, column);
				Grid.SetRow(label, row);
				Grid.SetColumn(label, column);

				keyboard.Children.Add(button);
				keyboard.Children.Add(label);

			}
			timer_complex();
			for (int i = 0; i < labels.Count; i++)
			{
				var label = new Label
				{
					Text = "_",
					Style = (Style)Resources["lbforent"],
					TextColor = GetTextColorByTheme()
				};
				labels[i].TextColor = GetTextColorByTheme();
				lbent.Children.Add(label);
			}
		}


		void StartTimers()
		{
			_timer = Application.Current.Dispatcher.CreateTimer();
			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.Tick += Timer_Tick;
			_timer3 = Application.Current.Dispatcher.CreateTimer();
			_timer3.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
			_timer3.Tick += Timer3_Tick;
			_timer.Start();
			_timer3.Start();
		}


		private async void InitializeAudio()
		{
			if (!_audioService.IsInitialized)
			{
				await _audioService.InitializeAsync();
			}
			_audioService.PlayGameMusic();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			_audioService.PlayGameMusic();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			// Останавливаем музыку только если действительно уходим со страницы
			if (Navigation.NavigationStack.Count == 0 ||
				Navigation.NavigationStack.Last() is not MainPage)
			{
				_audioService.StopAllMusic();
				_timer.Stop();
				_timer3.Stop();

			}
		}


		private void OnPauseMusic()
		{
			_audioService.PauseGameMusic();
		}

		private void OnResumeMusic()
		{
			_audioService.ResumeGameMusic();
		}
		int cellindex = 0;
		Random random = new Random();
		int point = 0;

		List<Label> labels = new List<Label>();

		private void Button_Clicked(object sender, EventArgs e)
		{
			var button = sender as Button;
			if (button == null) return;
			if (flagenabled)
			{
				var letter = button.Text;
				if (cellindex < labels.Count)
				{
					labels[cellindex].Text = letter;
					cellindex++;

				}

			}

		}

		TimeSpan _time;
		IDispatcherTimer _timer;
		IDispatcherTimer _timer3;
		int complex = 0;
		private int _remainingSeconds = 300;

		private void Timer_Tick(object sender, EventArgs e)
		{

			_time = _time.Add(new TimeSpan(0, 0, -1));

			string timeString = string.Format("{0}:{1:D2}", (int)_time.TotalMinutes, _time.Seconds);
			tim.Text = timeString;
			_remainingSeconds--;
			if (_time.TotalSeconds == 0)
			{
				foreach (var a in field.Children.ToList())
				{
					if (a is Label label1)
					{
						Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(label1);
					}
				}

				_timer.Stop();
				_timer3.Stop();
				DisplayAlert("Время вышло!", "Ваши очки: " + point, "ok");
			}
			if (_remainingSeconds % complex == 0)
			{
				CreateWord();
			}
		}

		private void CreateWord()
		{
			var displayInfo = DeviceDisplay.MainDisplayInfo;
			var screenWidth = displayInfo.Width / displayInfo.Density;
			int randomword = random.Next(0, 70);
			randomX = random.Next(Convert.ToInt32(-screenWidth) + 250, Convert.ToInt32(screenWidth) - 250);
			var label = CreatingLabels.GetLabel(words[randomword], randomX);
			label.TextColor = GetTextColorByTheme();
			field.Children.Add(label);
			label.TranslateTo(randomX, 370, Data.speedcsm, Easing.Linear);

		}

		private Color GetTextColorByTheme()
		{
			return Preferences.Default.Get("selthemedate", "") switch
			{
				"swhitetheme.png" => Colors.Black,
				"spinktheme.png" => Colors.White,
				"sblacktheme.png" => Colors.White,
			};
		}

		private Style GetTextColorByThemeButton()
		{
			return Preferences.Default.Get("selthemedate", "") switch
			{
				"swhitetheme.png" => (Style)Resources["whitethemebutton"],
				"spinktheme.png" => (Style)Resources["pinkthemebutton"],
				"sblacktheme.png" => (Style)Resources["blackthemebutton"],
			};
		}


		private void Timer3_Tick(object sender, EventArgs e)
		{
			// Оптимизация: проверяем только видимые элементы
			var visibleLabels = field.Children.OfType<Label>().Where(l => l.IsVisible).ToList();

			// Оптимизация: кешируем wordwin
			string currentWordWin = "";
			switch (complextime)
			{
				case 0:
					currentWordWin = string.Concat(cell1.Text, cell2.Text, cell3.Text, cell4.Text, cell5.Text);
					break;
				case 1:
					currentWordWin = string.Concat(cell1.Text, cell2.Text, cell3.Text, cell4.Text, cell5.Text, cell6.Text, cell7.Text);
					break;
				case 2:
					currentWordWin = string.Concat(cell1.Text, cell2.Text, cell3.Text, cell4.Text, cell5.Text, cell6.Text, cell7.Text, cell8.Text, cell9.Text);
					break;
			}

			// Оптимизация: проверяем только слова, которые могут совпадать по длине
			foreach (var child in visibleLabels.Where(l => l.Text.Length == currentWordWin.Length).ToList())
			{
				if (child.Text == currentWordWin)
				{
					PlayCorrectWordEffect(child, true);
					point += Data.pointcsm;
					ClearInputCells();
					break;
				}
			}

			// Оптимизация: проверяем только элементы, которые достигли нижней границы
			foreach (var label in visibleLabels.Where(l => l.TranslationY >= 370).ToList())
			{
				PlayCorrectWordEffect(label, false);
				if (point >= Data.pointcsm) point -= Data.pointcsm;
			}

			poin.Text = point.ToString();
		}
		private void ClearInputCells()
		{
			for (int i = 0; i < labels.Count; i++)
			{
				labels[i].Text = "";
			}
			cellindex = 0;
		}
		private async void PlayCorrectWordEffect(Label label, bool checkplay)
		{
			var anim = Preferences.Default.Get("swanim", true);
			if (checkplay)
			{
				if (anim)
				{
					label.TextColor = Colors.Lime;
					Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(label);
					await label.ScaleTo(1.3, 200, Easing.Linear);
					await Task.WhenAll(label.FadeTo(0, 300), label.TranslateTo(label.TranslationX, label.TranslationY - 50, 300));
				}
				field.Children.Remove(label);
				CreatingLabels.ReleaseLabel(label);
			}
			else
			{
				if (anim)
				{
					label.TextColor = Colors.Red;
					Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(label);
					await label.ScaleTo(1.3, 200, Easing.Linear);
					await Task.WhenAll(label.FadeTo(0, 300), label.TranslateTo(label.TranslationX, label.TranslationY - 50, 300));
				}
				field.Children.Remove(label);
				CreatingLabels.ReleaseLabel(label);
			}
		}
		List<string> words = new List<string>();
		bool flagenabled = true;
		int complextime = Data.compl;
		double randomX;

		void timer_complex()
		{
			switch (complextime)
			{
				case 0:
					_time = new TimeSpan(00, Data.timecsm, 00);
					complex = 5;

					labels.Remove(cell9); labels.Remove(cell8); labels.Remove(cell7); labels.Remove(cell6);
					cell9.IsVisible = false; cell8.IsVisible = false; cell7.IsVisible = false; cell6.IsVisible = false;
					words = new List<string>
				  { "много","палец","нитка","башня","бимок","мышка","бодро","акулы","алмаз","ангел",
					"атака","ведро","вафля","глядь","герой","двери","драка","ежели","ехать","нотка",
					"закат","затея","изгиб","имена","копья","ковры","листы","бровь","может","питон",
					"точка","ровно","слеза","сборы","талия","тепло","суета","утеха","фобия","фирма",
					"химия","цапля","цифра","червь","ткань","шляпа","салют","ябеда","кукла","кость",
					"вилка","тачка","пачка","чашка","кошка","файлы","холод","тепло","осень","весна",
					"вечно","палец","пятка","шишка","ветка","мишка","тесто","кисло","огонь","пятно",};
					break;
				case 1:
					_time = new TimeSpan(00, Data.timecsm, 00);
					complex = 4;
					labels.Remove(cell9); labels.Remove(cell8);
					cell9.IsVisible = false; cell8.IsVisible = false;
					words = new List<string>
				  { "кипяток","телефон","наушник","стаканы","игрушка","заметки","самокат","охотник","покупка","капуста",
					"морковь","картина","напиток","волосок","голосок","блокнот","галстук","ботаник","аксиома","алгебра",
					"единица","дневник","диктант","деление","задание","корабль","яблочко","лесенка","занятия","мускулы",
					"равнина","потолок","дверной","полинка","родинка","голубое","линейка","букварь","загадка","история",
					"квадрат","отметка","площадь","предлог","предмет","процесс","рассказ","резинка","рисунок","рулетка",
					"студент","теорема","тетрадь","учебник","ушастый","учитель","цилиндр","циркуль","частное","экзамен",
					"планета","спутник","ледоход","носорог","паводок","сумерки","темнота","красный","конфеты","человек",};
					break;
				case 2:
					_time = new TimeSpan(00, Data.timecsm, 00);
					complex = 3;
					words = new List<string>
				  { "бриллиант","диафрагма","виновница","визитница","биография","бизнесмен","биосинтез","гинеколог","викторина","география",
					"химчистка","филология","философия","циферблат","симметрия","симуляция","рисование","киносеанс","геометрия","параллель",
					"лихорадка","лицемерие","мизантроп","милостыня","миллионер","космонавт","диспетчер","дистанция","живописец","килограмм",
					"миссионер","миниатюра","бутерброд","баскетбол","курортный","компьютер","публичное","кардиолог","претензии","именинник",
					"восемьсот","должность","знакомить","пиратский","подшипник","безглазый","непонятно","осуждение","последнее","мальчишка",
					"прочность","бесплатно","фотосхема","двадцатка","затухание","надбровый","вприсядку","выставить","пироженое","драматизм",
					"брачность","безгривый","проведать","подкрылье","выморозка","оцепление","сметанник","поисковик","крепление","спортсмен",};
					break;
				default:
					break;
			}
		}
		private void Button_Clicked_1(object sender, EventArgs e)
		{
			if (cellindex > 0)
			{
				cellindex--;
				labels[cellindex].Text = "";
			}
		}
		private void Button_Clicked_2(object sender, EventArgs e)
		{
			cellindex = 0;
			for (int i = 0; i < labels.Count; i++)
			{
				labels[i].Text = "";
			}
		}
		private void pause_Clicked(object sender, EventArgs e)
		{
			_timer.Stop();
			_timer3.Stop();
			OnPauseMusic();
			flagenabled = false;
			clearone.IsEnabled = false;
			clear.IsEnabled = false;
			absmenu.IsVisible = true;
			pause.IsEnabled = false;

			foreach (var a in field.Children.ToList())
			{
				if (a is Label label1)
				{
					Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(label1);
				}
			}
		}
		private void start_Clicked(object sender, EventArgs e)
		{
			flagenabled = true;
			OnResumeMusic();
			foreach (var a in field.Children.OfType<Label>())
			{
				if (a is Label label1)
				{
					double remainingDistance = 370 - label1.TranslationY;
					uint newDuration = (uint)(10000 * (remainingDistance / (370 - (-100))));

					label1.TranslateTo(label1.TranslationX, 370, newDuration, Easing.Linear);
				}
			}
			if (_time.TotalSeconds != 0)
			{
				_timer.Start();
				_timer3.Start();
				pause.IsEnabled = true;
				absmenu.IsVisible = false;
				clearone.IsEnabled = true;
				clear.IsEnabled = true;
			}

		}
		private async void exmenu(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync(animated: false);
			_audioService.PlayMenuMusic();
		}

		private void ApplyTheme()
		{
			var theme = Preferences.Default.Get("selthemedate", "");

			switch (theme)
			{
				case "swhitetheme.png":
					ApplyWhiteTheme();
					break;
				case "spinktheme.png":
					ApplyPinkTheme();
					break;
				case "sblacktheme.png":
					ApplyBlackTheme();
					break;
			}
		}

		private void ApplyWhiteTheme()
		{
			// Основные цвета
			main.BackgroundColor = Colors.White;
			coutscore.BackgroundColor = Colors.White;
			field.BackgroundColor = Colors.White;

			// Текст
			tim.TextColor = Colors.Black;
			poin.TextColor = Colors.Black;
			paus.TextColor = Colors.Black;

			// Кнопки
			ApplyButtonStyle("whitethemebutton");

			// Метки слова
			foreach (var label in labels)
			{
				label.TextColor = Colors.Black;
			}

			// Метки ввода
			foreach (var label in lbent.Children.OfType<Label>())
			{
				label.TextColor = Colors.Black;
			}

			// Кнопки клавиатуры
			UpdateKeyboardButtons(Colors.White, Colors.Black);
		}

		private void ApplyPinkTheme()
		{
			main.BackgroundColor = Colors.Pink;
			word.BackgroundColor = Colors.Pink;
			coutscore.BackgroundColor = Colors.Pink;
			field.BackgroundColor = Colors.Pink;

			tim.TextColor = Colors.White;
			poin.TextColor = Colors.White;
			paus.TextColor = Colors.Pink;

			ApplyButtonStyle("pinkthemebutton");

			foreach (var label in labels)
			{
				label.TextColor = Colors.White;
			}

			foreach (var label in lbent.Children.OfType<Label>())
			{
				label.TextColor = Colors.White;
			}

			UpdateKeyboardButtons(Colors.White, Colors.Pink);
		}

		private void ApplyBlackTheme()
		{
			main.BackgroundColor = Colors.Black;
			word.BackgroundColor = Colors.Black;
			coutscore.BackgroundColor = Colors.Black;
			field.BackgroundColor = Colors.Black;

			tim.TextColor = Colors.White;
			poin.TextColor = Colors.White;
			paus.TextColor = Colors.White;

			ApplyButtonStyle("blackthemebutton");

			foreach (var label in labels)
			{
				label.TextColor = Colors.White;
			}

			foreach (var label in lbent.Children.OfType<Label>())
			{
				label.TextColor = Colors.White;
			}

			UpdateKeyboardButtons(Colors.White, Colors.Black);
		}

		private void ApplyButtonStyle(string styleKey)
		{
			var style = (Style)Resources[styleKey];

			pause.Style = style;
			continuebtn.Style = style;
			exit.Style = style;
			clearone.Style = style;
			clear.Style = style;
		}

		private void UpdateKeyboardButtons(Color bgColor, Color textColor)
		{
			foreach (var view in keyboard.Children)
			{
				if (view is Button button)
				{
					button.BackgroundColor = bgColor;
				}
				else if (view is Label label && label.Style == (Style)Resources["lbletter"])
				{
					label.TextColor = textColor;
				}
			}
		}
	}
}
