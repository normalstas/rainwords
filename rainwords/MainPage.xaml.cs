using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.Maui.Controls.Application; //объявляем глобально, чтобы Maui не путал какой нужен


namespace rainwords
{

	public partial class MainPage : ContentPage
	{
		private readonly IAudioService _audioService; //переменная класса с музыкой для управления ею
		public MainPage(IAudioService audioService) //создаем с параметром чтобы передавать музыку
		{

			InitializeComponent();
			_audioService = audioService; //инициализуем переменную
			_audioService.IsMusicEnabled = false; //выключаем музыку меню
			CreatingLabels.Initialize(10); //сразу создаем 10 меток для слов
			InitializeAudio(); //инициализируем потоки музыки
			var language = Preferences.Default.Get("languagepickcheck", ""); //определяем какой язык нужен
			DrawKeyboard(language); //функция для создания клавиатуры
			timer_complex(); //функция для определения сложности
			minusone.Text = "<-"; //не можем так писать в XAML пишем тут
			ApplyTheme(); //функция для определения темы приложения
			StartTimers(); //запускаем таймеры
			LanguageUI(language); //переводим все надписи на выбранный язык
			App.GamePaused += PauseTimers; //подписываемся на событие чтобы при сворачивании приложения останавливалась музыка и таймеры
		}

		private void PauseTimers()
		{
			pause_Clicked(pause, EventArgs.Empty); //открываем окно с паузой которая сама все сделает
		}



		void LanguageUI(string language)
		{
			if (language == "РУССКИЙ") //если в настройках русский выбран переводим на русский
			{
				paus.Text = "ПАУЗА";
				continuebtn.Text = "ПРОДОЛЖИТЬ";
				exit.Text = "ВЫЙТИ";
				againbtn.Text = "ЗАНОВО";
			}
			else //если нет то на английский
			{
				paus.Text = "PAUSE";
				continuebtn.Text = "CONTINUE";
				againbtn.Text = "AGAIN";
				exit.Text = "EXIT";
			}
		}

		void DrawKeyboard(string language)
		{
			if (language == "ENGLISH") //определяем какой язык выбран и открываем нужную функцию 
				DrawEnglishKeyboard();
			else if (language == "РУССКИЙ")
				DrawRussianKeyboard();
		}

		void DrawEnglishKeyboard() //создает английскую раскладку клавиатуры
		{
			string[] row0 = "qwertyuiop".Select(c => c.ToString()).ToArray(); //1 строка в клавиатуре
			string[] row1 = "asdfghjkl".Select(c => c.ToString()).ToArray();  //2 строка 
			string[] row2 = "zxcvbnm".Select(c => c.ToString()).ToArray();    //3 строка
																			  
			for (int i = 0; i < row0.Length; i++) //цикл для создания первой строки
				AddKey(row0[i], 0, i * 2 + 2); // созадем с расстоянием 1, 3, 6 ..

	
			for (int i = 0; i < row1.Length; i++) //цикл для создания второй строки
				AddKey(row1[i], 1, i * 2 + 3); //  созадем с расстоянием 5, 7, ...


			for (int i = 0; i < row2.Length; i++)//цикл для создания третьей строки
				AddKey(row2[i], 2, i * 2 + 5); // созадем с расстоянием 5, 9, ...
		}

		void DrawRussianKeyboard() //создает русскую раскладку
		{
			string[] row0 = "йцукенгшщзх".Select(c => c.ToString()).ToArray();   // 1 строка
			string[] row1 = "фывапролджэ".Select(c => c.ToString()).ToArray();   // 2
			string[] row2 = "ячсмитьбю".Select(c => c.ToString()).ToArray();     // 3

			for (int i = 0; i < row0.Length; i++) //цикл для создания 1 строки
				AddKey(row0[i], 0, i * 2 + 1); //  созадем с расстоянием 1, 5, ...

			for (int i = 0; i < row1.Length; i++) //цикл для создания 2 строки
				AddKey(row1[i], 1, i * 2 + 1); //  созадем с расстоянием 1, 5, ...

			for (int i = 0; i < row2.Length; i++)
				AddKey(row2[i], 2, i * 2 + 3); // созадем с расстоянием 3, 7, ...
		}

		void AddKey(string letter, int row, int column) //создает и кнопки и label и устанавливает в нужном расположении
		{
			var button = new Button //создание кнопки
			{
				Text = letter, //берем из функции выше
				TextColor = Colors.Transparent, //прозрачный текст
				CornerRadius = 5, //закругление кнопки
				Style = GetTextColorByThemeButton(), //стиль кнопки
			};

			var label = new Label //создание label чтобы было отчетливо видно букву у кнопки
			{
				Text = letter, //берем из функции выше
				Style = (Style)Resources["lbletter"], //стиль из XAML
				BackgroundColor = Colors.Transparent,//прозрачный фон
			};

			button.Clicked += KeyUp_Clicked; //создаем обработчик события для кнопки
			Grid.SetRow(button, row); //устанавливаем строку 1, 2 или 3
			Grid.SetColumn(button, column); //устанавливаем колонну из 24
			Grid.SetColumnSpan(button, 2); // кнопка занимает 2 колонки
			Grid.SetRow(label, row); //тоже самое делаем для лейблов
			Grid.SetColumn(label, column);
			Grid.SetColumnSpan(label, 2);

			keyboard.Children.Add(button); //добавляем и кнопку и лейбл
			keyboard.Children.Add(label); //
		}




		void StartTimers() //инициализация и запуск таймеров
		{
			_timer = Application.Current.Dispatcher.CreateTimer(); //создаем
			_timer.Interval = TimeSpan.FromSeconds(1); //устанавливаем интервал в 1 секунду
			_timer.Tick += Timer_Tick; //привязываем к событию
			_timer3 = Application.Current.Dispatcher.CreateTimer(); //создаем 2
			_timer3.Interval = TimeSpan.FromMilliseconds(1);//устанавливаем интервал в 1 миллисекунду
			_timer3.Tick += Timer3_Tick; //привязываем к другому событию
			_timer.Start(); //запуск 1
			_timer3.Start(); //запуск 2
		}


		private async void InitializeAudio() //инициализация музыки
		{
			var audio = Preferences.Default.Get("swsongs", true); //проверяем включена ли музыка в настройках
			if (!audio) return; //если нет не идем дальше
			if (!_audioService.IsInitialized) //такая же проверка но уже для класса
			{
				await _audioService.InitializeAsync(); //создаем поток
			}
			_audioService.PlayGameMusic(); //начинаем воспроизводить
		}

		protected override void OnAppearing()
		{
			//проверяем что звук не отключен в настройках
			var audio = Preferences.Default.Get("swsongs", true);
			if (!audio) return; //если отключен не выполняем далее
			base.OnAppearing();//вызов бвзовой реализации (страница включена)
			_audioService.PlayGameMusic(); //включаем игровую музыку
		}

		protected override void OnDisappearing()
		{
			var audio = Preferences.Default.Get("swsongs", true);//проверяем что звук не отключен в настройках
			if (!audio) return; //если отключен не выполняем далее
			base.OnDisappearing(); //вызывается, когда страница исчезает с экрана
			App.GamePaused -= PauseTimers; //отписывемся от события в App
										   // Останавливаем музыку только если действительно уходим со страницы
			if (Navigation.NavigationStack.Count == 0 ||
				Navigation.NavigationStack.Last() is not MainPage)
			{
				_audioService.StopAllMusic(); //отключаем всю музыку
				_timer.Stop(); //остановка главного таймера
				_timer3.Stop(); //остановка игрового таймера

			}
		}


		private void OnPauseMusic()//функция остановки музыки
		{
			var audio = Preferences.Default.Get("swsongs", true); //проверка свитч музыки в настройках
			if (!audio) return; //если выключен не идем двльше
			_audioService.PauseGameMusic(); //останавливаем музыку
		}

		private void OnResumeMusic() //функция продолжения музыки
		{
			var audio = Preferences.Default.Get("swsongs", true);//проверка свитч музыки в настройках
			if (!audio) return;//если выключен не идем двльше
			_audioService.ResumeGameMusic(); //продолжаем музыку
		}
		int cellindex = 0; //служит счетчиком уже написанных букв
		Random random = new Random(); //переменная рандома
		int point = 0; //счет очков

		List<Label> labels = new List<Label>(); //лист поля ввода

		private void KeyUp_Clicked(object sender, EventArgs e)
		{
			if (absmenu.IsVisible) return; //если окно паузы показывается не выполняем что дальше
			var button = sender as Button; //создаем переменную нажатой кнопки
			if (button == null) return; //если ошибочно было не идем двльше

			var letter = button.Text; //создание переменной текста кнопки
			if (cellindex < labels.Count) //если букв написанных больше чем счетчика(то есть у нас не переполненное поле ввода) 
			{//то
				labels[cellindex].Text = letter; //создаем в поле ввода нажатую букву
				cellindex++; //прибавляем счетчик

			}


		}

		TimeSpan _time; //будет служить отсчитыванием времени и его показу
		IDispatcherTimer _timer; //тот самый основной таймер игры
		IDispatcherTimer _timer3; //игровой таймер для проверки статуса слова
		int complex = 0; //уровень сложности
		private int _remainingSeconds = 300; //подсчет для создания слвоа

		private void Timer_Tick(object sender, EventArgs e) 
		{

			_time = _time.Add(new TimeSpan(0, 0, -1)); //отнимаем по секунде

			string timeString = string.Format("{0}:{1:D2}", (int)_time.TotalMinutes, _time.Seconds); //конвертируем в минуты и секунды только
			var language = Preferences.Default.Get("languagepickcheck", ""); //выбор языка
			tim.Text = timeString;//показываем время
			_remainingSeconds--; //также отнимаем по секунде
			if (_time.TotalSeconds == 0) //время вышло
			{
				foreach (var a in field.Children.ToList()) //перебираем все лейблы слов на экране
				{
					if (a is Label label1) //используем как раз только их
					{
						Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(label1); //останавливаем анимацию падения
					}
				}

				_timer.Stop(); //таймер останавливаем
				_timer3.Stop(); //второй тоже
				paus.Text = language == "РУССКИЙ" ? "ИГРА ОКОНЧЕНА!" : "THE GAME IS OVER!"; //информация о игре
				paus.FontSize = 16; //размер информации

				continuebtn.IsVisible = false;//скрываем кнопку продолжить
				againbtn.IsVisible = true;//показываем кнопку заново
				absmenu.IsVisible = true;//показываем окно окончания игры
			}
			if (!absmenu.IsVisible && _remainingSeconds % complex == 0) //если окно не показано и общее время делится на 5, 4 или 3
			{															//в зависимости от сложности
				CreateWord();//создаем слово
			}
		}

		private void CreateWord() //функция создания слов
		{
			var displayInfo = DeviceDisplay.MainDisplayInfo; //вычисляем дисплей(у нас стоит стого вертикальный вид)
			var screenWidth = displayInfo.Width / displayInfo.Density; //делим ширину дисплея на плотность
			int randomword = random.Next(0, 70); //случайное слово из списка
			randomX = random.Next(Convert.ToInt32(-screenWidth) + 250, Convert.ToInt32(screenWidth) - 250); //определяем ось х чтобы слово было полностью видно
			var label = CreatingLabels.GetLabel(words[randomword], randomX);//отправляем в класс создания								
			label.TextColor = GetTextColorByTheme(); //создаем тему для слова
			_audioService.PlayExplaSound();//вклюючаем звук появления
			field.Children.Add(label);//добавляем в игровое поле
			label.TranslateTo(randomX, 370, Data.speedcsm, Easing.Linear); //создаем анимацию падения где Data.speedcsm - скорость слова

		}

		private Color GetTextColorByTheme()
		{
			return Preferences.Default.Get("selthemedate", "") switch //в зависимости от темы создаем цвет для слова
			{
				"swhitetheme.png" => Colors.Black,
				"spinktheme.png" => Colors.Black,
				"sblacktheme.png" => Colors.White,
			};
		}

		private Style GetTextColorByThemeButton()
		{
			return Preferences.Default.Get("selthemedate", "") switch//в зависимости от темы создаем цвет для кнопок
			{
				"swhitetheme.png" => (Style)Resources["whitethemebutton"],
				"spinktheme.png" => (Style)Resources["pinkthemebutton"],
				"sblacktheme.png" => (Style)Resources["blackthemebutton"],
			};
		}


		private void Timer3_Tick(object sender, EventArgs e)
		{
			// оптимизация проверяяя только видимые элементы
			var visibleLabels = field.Children.OfType<Label>().Where(l => l.IsVisible).ToList();

			// оптимизация кешируем wordwin
			string currentWordWin = ""; //складывает все написанные буквы в одно слово
			switch (complextime)
			{
				case 0:
					currentWordWin = string.Concat(cell1.Text, cell2.Text, cell3.Text, cell4.Text, cell5.Text); //5 букв
					break;
				case 1: //7 букв
					currentWordWin = string.Concat(cell1.Text, cell2.Text, cell3.Text, cell4.Text, cell5.Text, cell6.Text, cell7.Text);
					break;
				case 2://9 букв
					currentWordWin = string.Concat(cell1.Text, cell2.Text, cell3.Text, cell4.Text, cell5.Text, cell6.Text, cell7.Text, cell8.Text, cell9.Text);
					break;
			}

			// оптимизация проверяя только слова, которые могут совпадать по длине
			foreach (var child in visibleLabels.Where(l => l.Text.Length == currentWordWin.Length).ToList())
			{
				if (child.Text == currentWordWin)//если слово сходится с тем что мы написали
				{
					PlayCorrectWordEffect(child, true); //эффект зеленый цвет
					_audioService.PlayWinSound();//звук победы над словом
					point += Data.pointcsm;//прибавляем очки
					ClearInputCells();//очищаем поле ввода
					break;//заканчиваем
				}
			}

			// оптимизация проверяем только элементы, которые достигли конца игрового поля
			foreach (var label in visibleLabels.Where(l => l.TranslationY >= 370).ToList())
			{
				PlayCorrectWordEffect(label, false); //эффект красный цвет
				_audioService.PlayLossSound();//звук поражения над словом
				if (point >= Data.pointcsm) point -= Data.pointcsm;//отнимаем очки если они не ровны 0
			}

			poin.Text = point.ToString(); //отображаем их
		}
		private void ClearInputCells()
		{
			for (int i = 0; i < labels.Count; i++) //очищаем все буквы в поле для вывода
			{
				labels[i].Text = "";
			}
			cellindex = 0;//обнуляем счетчик
		}
		private async void PlayCorrectWordEffect(Label label, bool checkplay)
		{
			var anim = Preferences.Default.Get("swanim", true);//проверяем что эффекты включены в настройках
			if (checkplay) //победа или проигрыш над словом
			{
				if (anim) //если в настройках включены
				{
					label.TextColor = Colors.Lime; //делаем заленый цвет текста
					Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(label); //отменяем анимацию
					await label.ScaleTo(1.3, 200, Easing.Linear); //расширяем слово
					await Task.WhenAll(label.FadeTo(0, 300), label.TranslateTo(label.TranslationX, label.TranslationY - 50, 300));//чутка поднимаем слово
				}
				field.Children.Remove(label);//удаляем из игрового поля
				CreatingLabels.ReleaseLabel(label);//освобждаем метку
			}
			else
			{
				if (anim)//делаем все тоже самое но с красным цветом
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
		List<string> words = new List<string>();//список падающий слов
		int complextime = Data.compl; //сколько общее время будет
		double randomX; //слово по оси х из случайной точки

		void timer_complex() //определение сложности
		{
			labels = new List<Label> { cell1, cell2, cell3, cell4, cell5, cell6, cell7, cell8, cell9 };//создаем поле ввода
			var language = Preferences.Default.Get("languagepickcheck", ""); //определния языка
			lbent.Children.Clear();//очищаем ранее созданное поле ввода
			switch (complextime) //какую сложность выбрали
			{

				case 0:
					_time = new TimeSpan(00, Data.timecsm, 00);//создаем время на 5 мин
					complex = 5;//итервал между новыми словами

					labels.Remove(cell9); labels.Remove(cell8); labels.Remove(cell7); labels.Remove(cell6);//удаляем поле ввода до 5 букв
					cell9.IsVisible = false; cell8.IsVisible = false; cell7.IsVisible = false; cell6.IsVisible = false;//скрываем их

					if (language == "РУССКИЙ")//если русский выбран заполняем список русскими словами
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
						words = new List<string> //если английский выбран заполняем список английскими словами
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
				case 1://все точно также только под среднюю сложность
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
						{  
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
				case 2://все точно также только под сложную сложность
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
			for (int i = 0; i < labels.Count; i++) //создание поля вывода 
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
		private void ClearOne_Clicked(object sender, EventArgs e) //очистка последней буквы
		{
			if (absmenu.IsVisible) return; //не сможем если стоим на паузе
			if (cellindex > 0)
			{
				cellindex--;
				labels[cellindex].Text = "";
			}
		}
		private void ClearAll_Clicked(object sender, EventArgs e) //очистка всего поля вывода
		{
			if (absmenu.IsVisible) return;//не сможем если стоим на паузе
			cellindex = 0;
			for (int i = 0; i < labels.Count; i++)
			{
				labels[i].Text = "";
			}
		}
		private void pause_Clicked(object sender, EventArgs e) //пауаз
		{
			if (absmenu.IsVisible) return; //не сможем если уже стоим на паузе
			_timer.Stop();//останавливаем все
			_timer3.Stop();
			OnPauseMusic();
			absmenu.IsVisible = true;//показываем окно паузы

			foreach (var a in field.Children.ToList())
			{
				if (a is Label label1)
				{
					Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(label1); //останавливаем аимацию всех слов
				}
			}
		}
		private void start_Clicked(object sender, EventArgs e)//продолжить
		{
			OnResumeMusic();//продолжаем музыку
			foreach (var a in field.Children.OfType<Label>())
			{
				if (a is Label label1)
				{
					double remainingDistance = 370 - label1.TranslationY;
					uint newDuration = (uint)(10000 * (remainingDistance / (370 - (-100))));

					label1.TranslateTo(label1.TranslationX, 370, newDuration, Easing.Linear);//вохвращаем анимацию и указываем сколько осталось лететь
				}
			}

			_timer.Start();//как с паузой но наоборот
			_timer3.Start();
			absmenu.IsVisible = false;
			clearone.IsEnabled = true;
			clear.IsEnabled = true;

		}
		private async void exmenu(object sender, EventArgs e)
		{//выход из игры
			await Navigation.PopModalAsync(animated: false);
			_audioService.IsMusicEnabled = true;
			_audioService.PlayMenuMusic();//включаем музыку меню
		}

		private void ApplyTheme()
		{
			var theme = Preferences.Default.Get("selthemedate", "");
			//определение и настраивание темы
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
			// основные цвета
			main.BackgroundColor = Colors.White;
			coutscore.BackgroundColor = Colors.White;
			field.BackgroundColor = Colors.White;
			framefield.BorderColor = Colors.Black;
			// текст
			tim.TextColor = Colors.Black;
			poin.TextColor = Colors.Black;
			paus.TextColor = Colors.Black;

			// кнопки
			ApplyButtonStyle("whitethemebutton");

			// метки слова
			foreach (var label in labels)
			{
				label.TextColor = Colors.Black;
			}

			// метки ввода
			foreach (var label in lbent.Children.OfType<Label>())
			{
				label.TextColor = Colors.Black;
			}

			// кнопки клавиатуры
			UpdateKeyboardButtons(Colors.White, Colors.Black);
		}

		private void ApplyPinkTheme()//тоже самое для розовой темы
		{
			main.BackgroundColor = Colors.HotPink;
			word.BackgroundColor = Colors.HotPink;
			coutscore.BackgroundColor = Colors.HotPink;
			field.BackgroundColor = Colors.HotPink;
			keyboard.BackgroundColor = Colors.HotPink;
			framefield.BorderColor = Colors.Black;
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

		private void ApplyBlackTheme()//и для черной темы
		{
			main.BackgroundColor = Colors.Black;
			word.BackgroundColor = Colors.Black;
			coutscore.BackgroundColor = Colors.Black;
			field.BackgroundColor = Colors.Black;
			keyboard.BackgroundColor = Colors.Black;
			framefield.BorderColor = Colors.White;
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

		private void ApplyButtonStyle(string styleKey)//стили для кнопок
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

		private void UpdateKeyboardButtons(Color bgColor, Color textColor)//стили для кнопок на клавиатуре
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

		private void UpdateKeyPinkButton(Color textColor)//для розовой
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

		private void againbtn_Clicked(object sender, EventArgs e)//начать заново
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
			//все возобновляем заново
		}
	}
}
