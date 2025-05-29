using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.Maui.Controls.Application;


namespace rainwords
{

	public partial class MainPage : ContentPage
	{
		private readonly IAudioService _audioService;
		private bool isManuallyPaused = true;
		public MainPage(IAudioService audioService)
		{
			var stopwatch = Stopwatch.StartNew();
			InitializeComponent();
			stopwatch.Stop();
			Console.WriteLine($"Settings loaded in {stopwatch.ElapsedMilliseconds} ms");
			_audioService = audioService;
			_audioService.IsMusicEnabled = false;
			CreatingLabels.Initialize(10);
			InitializeAudio();
			var language = Preferences.Default.Get("languagepickcheck", "");
			DrawKeyboard(language);
			timer_complex();
			ApplyTheme();
			StartTimers();
			LanguageUI(language);
			App.GamePaused += PauseTimers;
		}

		private void PauseTimers()
		{
			pause_Clicked(pause, EventArgs.Empty);
		}

		

		void LanguageUI(string language)
		{
			if (language == "РУССКИЙ")
			{
				paus.Text = "ПАУЗА";
				continuebtn.Text = "ПРОДОЛЖИТЬ";
				exit.Text = "ВЫЙТИ";
				againbtn.Text = "ЗАНОВО";
			}
			else
			{
				paus.Text = "PAUSE";
				continuebtn.Text = "CONTINUE";
				againbtn.Text = "AGAIN";
				exit.Text = "EXIT";
			}
		}

		void DrawKeyboard(string layout)
		{
			if (layout == "ENGLISH")
				DrawEnglishKeyboard();
			else if (layout == "РУССКИЙ")
				DrawRussianKeyboard();
		}

		void DrawEnglishKeyboard()
		{
			string[] row0 = "qwertyuiop".Select(c => c.ToString()).ToArray();  // 10 букв
			string[] row1 = "asdfghjkl".Select(c => c.ToString()).ToArray();   // 9 букв
			string[] row2 = "zxcvbnm".Select(c => c.ToString()).ToArray();     // 7 букв
			// Первая строка — QWERTYUIOP
			for (int i = 0; i < row0.Length; i++)
				AddKey(row0[i], 0, i * 2 + 2); // 1, 3, ..., 19

			// Вторая строка — ASDFGHJKL
			for (int i = 0; i < row1.Length; i++)
				AddKey(row1[i], 1, i * 2 + 3); // 2, 4, ..., 18

			// Третья строка — ZXCVBNM
			for (int i = 0; i < row2.Length; i++)
				AddKey(row2[i], 2, i * 2 + 5); // 4, 6, ..., 16
		}

		void DrawRussianKeyboard()
		{
			string[] row0 = "йцукенгшщзх".Select(c => c.ToString()).ToArray();   // 11
			string[] row1 = "фывапролджэ".Select(c => c.ToString()).ToArray();   // 11
			string[] row2 = "ячсмитьбю".Select(c => c.ToString()).ToArray();     // 10

			for (int i = 0; i < row0.Length; i++)
				AddKey(row0[i], 0, i * 2 + 1); // Й–Х → 1, 3, ..., 21

			for (int i = 0; i < row1.Length; i++)
				AddKey(row1[i], 1, i * 2 + 1); // Ф–Э → 1, 3, ..., 21

			for (int i = 0; i < row2.Length; i++)
				AddKey(row2[i], 2, i * 2 + 3); // Я–Ю → 2, 4, ..., 20
		}

		void AddKey(string letter, int row, int column)
		{
			var button = new Button
			{
				Text = letter,
				TextColor = Colors.Transparent,
				FontSize = 18,
				CornerRadius = 5,
				Style = GetTextColorByThemeButton(),
			};

			var label = new Label
			{
				Text = letter,
				Style = (Style)Resources["lbletter"],
				BackgroundColor = Colors.Transparent,
				//TextColor = GetTextColorByTheme(),
			};

			button.Clicked += Button_Clicked;
			Grid.SetRow(button, row);
			Grid.SetColumn(button, column);
			Grid.SetColumnSpan(button, 2); // кнопка занимает 2 колонки
			Grid.SetRow(label, row);
			Grid.SetColumn(label, column);
			Grid.SetColumnSpan(label, 2);

			keyboard.Children.Add(button);
			keyboard.Children.Add(label);
		}




		void StartTimers()
		{
			_timer = Application.Current.Dispatcher.CreateTimer();
			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.Tick += Timer_Tick;
			_timer3 = Application.Current.Dispatcher.CreateTimer();
			_timer3.Interval = TimeSpan.FromMilliseconds(1); // ~60 FPS
			_timer3.Tick += Timer3_Tick;
			_timer.Start();
			_timer3.Start();
		}


		private async void InitializeAudio()
		{
			var audio = Preferences.Default.Get("swsongs", true);
			if (!audio) return;
			if (!_audioService.IsInitialized)
			{
				await _audioService.InitializeAsync();
			}
			_audioService.PlayGameMusic();
		}

		protected override void OnAppearing()
		{
			var audio = Preferences.Default.Get("swsongs", true);
			if (!audio) return;
			base.OnAppearing();
			_audioService.PlayGameMusic();
		}

		protected override void OnDisappearing()
		{
			var audio = Preferences.Default.Get("swsongs", true);
			if (!audio) return;
			base.OnDisappearing();
			App.GamePaused -= PauseTimers;
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
			var audio = Preferences.Default.Get("swsongs", true);
			if (!audio) return;
			_audioService.PauseGameMusic();
		}

		private void OnResumeMusic()
		{
			var audio = Preferences.Default.Get("swsongs", true);
			if (!audio) return;
			_audioService.ResumeGameMusic();
		}
		int cellindex = 0;
		Random random = new Random();
		int point = 0;

		List<Label> labels = new List<Label>();

		private void Button_Clicked(object sender, EventArgs e)
		{
			if (absmenu.IsVisible) return;
			var button = sender as Button;
			if (button == null) return;

			var letter = button.Text;
			if (cellindex < labels.Count)
			{
				labels[cellindex].Text = letter;
				cellindex++;

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
			var language = Preferences.Default.Get("languagepickcheck", "");
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
				paus.Text = language == "РУССКИЙ" ? "ИГРА ОКОНЧЕНА!" : "THE GAME IS OVER!";
				paus.FontSize = 16;

				continuebtn.IsVisible = false;
				againbtn.IsVisible = true;
				absmenu.IsVisible = true;
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
			_audioService.PlayExplaSound();
			field.Children.Add(label);
			label.TranslateTo(randomX, 370, Data.speedcsm, Easing.Linear);

		}

		private Color GetTextColorByTheme()
		{
			return Preferences.Default.Get("selthemedate", "") switch
			{
				"swhitetheme.png" => Colors.Black,
				"spinktheme.png" => Colors.Black,
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
					_audioService.PlayWinSound();
					point += Data.pointcsm;
					ClearInputCells();
					break;
				}
			}

			// Оптимизация: проверяем только элементы, которые достигли нижней границы
			foreach (var label in visibleLabels.Where(l => l.TranslationY >= 370).ToList())
			{
				PlayCorrectWordEffect(label, false);
				_audioService.PlayLossSound();
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
		int complextime = Data.compl;
		double randomX;

		void timer_complex()
		{
			labels = new List<Label> { cell1, cell2, cell3, cell4, cell5, cell6, cell7, cell8, cell9 };
			var language = Preferences.Default.Get("languagepickcheck", "");
			lbent.Children.Clear();
			switch (complextime)
			{

				case 0:
					_time = new TimeSpan(00, Data.timecsm, 00);
					complex = 5;

					labels.Remove(cell9); labels.Remove(cell8); labels.Remove(cell7); labels.Remove(cell6);
					cell9.IsVisible = false; cell8.IsVisible = false; cell7.IsVisible = false; cell6.IsVisible = false;

					if (language == "РУССКИЙ")
					{
						words = new List<string>
						{ "много","палец","нитка","башня","бимок","мышка","бодро","акулы","алмаз","ангел",
						"атака","ведро","вафля","глядь","герой","двери","драка","ежели","ехать","нотка",
						"закат","затея","изгиб","имена","копья","ковры","листы","бровь","может","питон",
						"точка","ровно","слеза","сборы","талия","тепло","суета","утеха","фобия","фирма",
						"химия","цапля","цифра","червь","ткань","шляпа","салют","ябеда","кукла","кость",
						"вилка","тачка","пачка","чашка","кошка","файлы","холод","тепло","осень","весна",
						"вечно","палец","пятка","шишка","ветка","мишка","тесто","кисло","огонь","пятно",};
					}
					else
					{
						words = new List<string> //2 строка 2 слово 
						{ "apple","chair","dance","light","smile","ocean","cloud","peach","happy","grass",
						  "berry","breez","mango","lemon","quilt","music","honey","river","dream","books",
						  "cocoa","tulip","clean","plant","bread","sweet","grape","blush","beach","cream",
						  "bloom","chill","merry","maple","petal","fairy","puppy","jelly","sunny","hobby",
						  "sunny","grace","angel","flute","leafy","laugh","piano","donut","linen","soapy",
						  "green","creek","petal","smile","spark","chirp","climb","paint","dough","sheep",
						  "world","cozyy","words","panda","toast","swirl","skate","froth","coral","coral",

						};
					}

					break;
				case 1:
					_time = new TimeSpan(00, Data.timecsm, 00);
					complex = 4;
					labels.Remove(cell9); labels.Remove(cell8);
					cell9.IsVisible = false; cell8.IsVisible = false;
					if (language == "РУССКИЙ")
					{

						words = new List<string>
				  { "кипяток","телефон","наушник","стаканы","игрушка","заметки","самокат","охотник","покупка","капуста",
					"морковь","картина","напиток","волосок","голосок","блокнот","галстук","ботаник","аксиома","алгебра",
					"единица","дневник","диктант","деление","задание","корабль","яблочко","лесенка","занятия","мускулы",
					"равнина","потолок","дверной","полинка","родинка","голубое","линейка","букварь","загадка","история",
					"квадрат","отметка","площадь","предлог","предмет","процесс","рассказ","резинка","рисунок","рулетка",
					"студент","теорема","тетрадь","учебник","ушастый","учитель","цилиндр","циркуль","частное","экзамен",
					"планета","спутник","ледоход","носорог","паводок","сумерки","темнота","красный","конфеты","человек",};
					}
					else
					{
						words = new List<string>
						{  //6 строка 9 слово
							"blanket","popcorn","lantern","musical","bicycle","morning","rainbow","cupcake","jellybe","cottage",
							"teacher","cookies","apricot","picture","imagine","flowers","breezes","journey","zephyrs","snuggle",
							"almondy","dolphin","pajamas","balloon","drawing","cupcake","scribes","harvest","balance","humming",
							"freedom","twinkle","sandals","daisyed","galaxyy","blanket","unicorn","giggles","puddles","lantern",
							"snuggle","eyelash","delight","bubbles","natural","harmony","bubbley","journey","painter","peaches",
							"buttons","smiling","silence","melodyy","fashion","cuddled","peacock","cascade","whisper","journal",
							"popcorn","tickles","twinkle","shimmer","mellowy","chalked","freedom","sandals","evening","pastelz",
						};
					}
					break;
				case 2:
					_time = new TimeSpan(00, Data.timecsm, 00);
					complex = 3;
					if (language == "РУССКИЙ")
					{
						words = new List<string>
				  { "бриллиант","диафрагма","виновница","визитница","биография","бизнесмен","биосинтез","гинеколог","викторина","география",
					"химчистка","филология","философия","циферблат","симметрия","симуляция","рисование","киносеанс","геометрия","параллель",
					"лихорадка","лицемерие","мизантроп","милостыня","миллионер","космонавт","диспетчер","дистанция","живописец","килограмм",
					"миссионер","миниатюра","бутерброд","баскетбол","курортный","компьютер","публичное","кардиолог","претензии","именинник",
					"восемьсот","должность","знакомить","пиратский","подшипник","безглазый","непонятно","осуждение","последнее","мальчишка",
					"прочность","бесплатно","фотосхема","двадцатка","затухание","надбровый","вприсядку","выставить","пироженое","драматизм",
					"брачность","безгривый","проведать","подкрылье","выморозка","оцепление","сметанник","поисковик","крепление","спортсмен",};

					}
					else
					{
						words = new List<string>
						{
							"adventure","blueberry","butterfly","spaceship","chocolate","delighted","apartment","marshland","jellybean","evergreen",
							"harmonica","rainstorm","sunflower","backdrops","discovery","feathered","fireflies","kangaroos","lovelight","landscape",
							"classroom","evergreen","firelight","goldfinch","goldsmith","harmonica","honeycomb","jellybean","rainstorm","sandpaper",
							"moonlight","overjoyed","paintball","parklands","passenger","peacefuls","pinecones","playhouse","scrapbook","seashells",
							"snowflake","songbirds","sparkling","starfruit","stargazer","sunflower","sweetener","sweetshop","teacupful","whistling",
							"windchime","wristband","bluebirds","bumblebee","doughnuts","fairylike","fireflies","fireworks","flowerbed","goodnight",
							"happiness","classroom","daydreams","earthworm","figurines","goldcrest","jellyroll","lakeshore","longboard","outerwear",
						};
					}
					break;
				default:
					break;
			}
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
		private void Button_Clicked_1(object sender, EventArgs e)
		{
			if (absmenu.IsVisible) return;
			if (cellindex > 0)
			{
				cellindex--;
				labels[cellindex].Text = "";
			}
		}
		private void Button_Clicked_2(object sender, EventArgs e)
		{
			if (absmenu.IsVisible) return;
			cellindex = 0;
			for (int i = 0; i < labels.Count; i++)
			{
				labels[i].Text = "";
			}
		}
		private void pause_Clicked(object sender, EventArgs e)
		{
			if (absmenu.IsVisible) return;
			_timer.Stop();
			_timer3.Stop();
			OnPauseMusic();
			absmenu.IsVisible = true;

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
			OnResumeMusic();
			foreach (var a in field.Children.OfType<Label>())
			{
				if (a is Label label1)
				{
					double remainingDistance = 370 - label1.TranslationY;
					uint newDuration = (uint)(10000 * (remainingDistance / (370 - (-100))));

					label1.TranslateTo(label1.TranslationX, 370, Data.speedcsm, Easing.Linear);
				}
			}
			if (_time.TotalSeconds != 0)
			{
				_timer.Start();
				_timer3.Start();
				absmenu.IsVisible = false;
				clearone.IsEnabled = true;
				clear.IsEnabled = true;
			}

		}
		private async void exmenu(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync(animated: false);
			_audioService.IsMusicEnabled = true;
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
			main.BackgroundColor = Colors.HotPink;
			word.BackgroundColor = Colors.HotPink;
			coutscore.BackgroundColor = Colors.HotPink;
			field.BackgroundColor = Colors.HotPink;
			keyboard.BackgroundColor = Colors.HotPink;

			tim.TextColor = Colors.Black;
			poin.TextColor = Colors.Black;
			paus.TextColor = Colors.Black;

			ApplyButtonStyle("pinkthemebutton");

			foreach (var label in labels)
			{
				label.TextColor = Colors.White;
			}

			foreach (var label in lbent.Children.OfType<Label>())
			{
				label.TextColor = Colors.Black;
			}

			UpdateKeyPinkButton(Colors.Black);
		}

		private void ApplyBlackTheme()
		{
			main.BackgroundColor = Colors.Black;
			word.BackgroundColor = Colors.Black;
			coutscore.BackgroundColor = Colors.Black;
			field.BackgroundColor = Colors.Black;
			keyboard.BackgroundColor = Colors.Black;
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

			UpdateKeyboardButtons(Colors.Black, Colors.White);
		}

		private void ApplyButtonStyle(string styleKey)
		{
			var style = (Style)Resources[styleKey];
			switch (styleKey)
			{
				case "whitethemebutton":
					continuebtn.Style = (Style)Resources["whitemenu"];
					againbtn.Style = (Style)Resources["whitemenu"];
					exit.Style = (Style)Resources["whitemenu"];
					break;
				case "pinkthemebutton":
					continuebtn.Style = (Style)Resources["pinkmenu"];
					againbtn.Style = (Style)Resources["pinkmenu"];
					exit.Style = (Style)Resources["pinkmenu"];
					break;
				case "blackthemebutton":
					continuebtn.Style = (Style)Resources["blackmenu"];
					againbtn.Style = (Style)Resources["blackmenu"];
					exit.Style = (Style)Resources["blackmenu"];
					break;
				default:
					break;
			}
			
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

		private void UpdateKeyPinkButton(Color textColor)
		{
			foreach (var view in keyboard.Children)
			{
				if (view is Button button)
				{

					button.BackgroundColor = Color.FromArgb("#D5156B");
				}
				else if (view is Label label && label.Style == (Style)Resources["lbletter"])
				{
					label.TextColor = textColor;
				}
			}
		}

		private void againbtn_Clicked(object sender, EventArgs e)
		{
			absmenu.IsVisible = false;
			againbtn.IsVisible = false;
			continuebtn.IsVisible = true;
			paus.FontSize = 24;
			point = 0;
			_remainingSeconds = 300;
			var language = Preferences.Default.Get("languagepickcheck", "");
			LanguageUI(language);
			timer_complex();
			StartTimers();
		}
	}
}
