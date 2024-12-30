


using Microsoft.Maui.Controls;
using System.Numerics;


namespace rainwords
{

	public partial class MainPage : ContentPage
	{

		public MainPage()
		{
			InitializeComponent();

			labels = new List<Label> { cell1, cell2, cell3, cell4, cell5, cell6, cell7, cell8, cell9 };
			timer_complex();
			int columns = 11;
			int rows = (int)Math.Ceiling(33.0 / columns);
			char[] letters = "йцукенгшщзхфывапролджэ ячсмитьбю".ToCharArray();
			if (Preferences.Default.Get("languagepickcheck", "") == "English")
			{
				letters = " qwertyuiop asdfghjkl   zxcvbnm".ToCharArray();
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
					//HeightRequest = 26,

				};

				var label = new Label
				{
					Text = letters[i].ToString(),
					Style = (Style)Resources["lbletter"],
					BackgroundColor = Colors.Transparent
				};
				switch (Preferences.Default.Get("selthemedate", ""))
				{
					case "swhitetheme.png":
						button.Style = (Style)Resources["whitethemebutton"];
						label.TextColor = Colors.White;
						break;
					case "spinktheme.png":
						button.Style = (Style)Resources["pinkthemebutton"];
						label.TextColor = Colors.Pink;
						break;
					case "sblacktheme.png":
						button.Style = (Style)Resources["blackthemebutton"];
						label.TextColor = Colors.Black;
						break;
					default:
						break;
				}
				int row = i / columns;
				int column = i % columns;
				if (button.Text == " ")
				{

					label.IsVisible = false;
					label.IsVisible = false;
					button.IsEnabled = false;
					button.IsVisible = false;

				}

				Grid.SetRow(button, row);
				Grid.SetColumn(button, column);
				Grid.SetRow(label, row);
				Grid.SetColumn(label, column);
				if (Preferences.Default.Get("languagepickcheck", "") == "English")
				{

					if (row == 2 && column == 0 || row == 2 && column == 10)
					{

						label.IsVisible = false;
						label.IsVisible = false;
						button.IsEnabled = false;
						button.IsVisible = false;
						Grid.SetColumnSpan(clearone, 2);
						Grid.SetColumn(clear, 9);
						//Grid.SetColumnSpan(clear, 2);
					}
				}
				button.Clicked += Button_Clicked;



				keyboard.Children.Add(button);
				keyboard.Children.Add(label);

			}
			for (int i = 0; i < labels.Count; i++)
			{
				var label = new Label
				{
					Text = "_",
					Style = (Style)Resources["lbforent"]
				};
				switch (Preferences.Default.Get("selthemedate", ""))
				{
					case "swhitetheme.png":
						label.TextColor = Colors.Black;
						labels[i].TextColor = Colors.Black;

						break;
					case "spinktheme.png":
						label.TextColor = Colors.White;
						labels[i].TextColor = Colors.White;
						break;
					case "sblacktheme.png":
						label.TextColor = Colors.White;
						labels[i].TextColor = Colors.White;
						break;
					default:
						break;
				}
				lbent.Children.Add(label);
			}
			switch (Preferences.Default.Get("selthemedate", ""))
			{
				case "swhitetheme.png":
					main.BackgroundColor = Colors.White;
					coutscore.BackgroundColor = Colors.White;
					field.BackgroundColor = Colors.White;
					clearone.Style = (Style)Resources["whitethemebutton"];
					clear.Style = (Style)Resources["whitethemebutton"];
					pause.Style = (Style)Resources["whitethemebutton"];

					continuebtn.Style = (Style)Resources["whitethemebutton"];
					exit.Style = (Style)Resources["whitethemebutton"];
					tim.TextColor = Colors.Black;
					poin.TextColor = Colors.Black;
					break;
				case "spinktheme.png":
					main.BackgroundColor = Colors.Pink;
					word.BackgroundColor = Colors.Pink;
					coutscore.BackgroundColor = Colors.Pink;
					field.BackgroundColor = Colors.Pink;
					clearone.Style = (Style)Resources["pinkthemebutton"];
					clear.Style = (Style)Resources["pinkthemebutton"];
					pause.Style = (Style)Resources["pinkthemebutton"];

					continuebtn.Style = (Style)Resources["pinkthemebutton"];
					exit.Style = (Style)Resources["pinkthemebutton"];
					tim.TextColor = Colors.White;
					poin.TextColor = Colors.White;
					break;
				case "sblacktheme.png":
					main.BackgroundColor = Colors.Black;
					word.BackgroundColor = Colors.Black;
					coutscore.BackgroundColor = Colors.Black;
					field.BackgroundColor = Colors.Black;
					clearone.Style = (Style)Resources["blackthemebutton"];
					clear.Style = (Style)Resources["blackthemebutton"];
					pause.Style = (Style)Resources["blackthemebutton"];

					continuebtn.Style = (Style)Resources["blackthemebutton"];
					exit.Style = (Style)Resources["blackthemebutton"];
					tim.TextColor = Colors.White;
					poin.TextColor = Colors.White;
					break;
				default:
					break;
			}


			_timer = Application.Current.Dispatcher.CreateTimer();
			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.Tick += Timer_Tick;
			_timer2 = Application.Current.Dispatcher.CreateTimer();
			_timer2.Interval = TimeSpan.FromSeconds(1);
			_timer2.Tick += Timer2_Tick;
			_timer3 = Application.Current.Dispatcher.CreateTimer();
			_timer3.Interval = TimeSpan.FromMicroseconds(1);

			_timer3.Tick += Timer3_Tick;
			_timer.Start();
			_timer2.Start();
			_timer3.Start();

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
		TimeSpan _time2;
		IDispatcherTimer _timer2;
		TimeSpan _time3;
		IDispatcherTimer _timer3;


		int complex = 0;

		string wordwin;


		private void Timer_Tick(object sender, EventArgs e)
		{

			_time = _time.Add(new TimeSpan(0, 0, -1));

			string timeString = string.Format("{0}:{1:D2}", (int)_time.TotalMinutes, _time.Seconds);
			tim.Text = timeString;


			if (_time.TotalSeconds == 0)
			{
				DisplayAlert("Время вышло!", "Ваши очки: " + point, "ok");

			}
		}


		private void Timer3_Tick(object sender, EventArgs e)
		{

			foreach (var child in wordsfield.ToList())
			{
				if (child.ToString() == wordwin)
				{
					foreach (var x in field.Children.ToList())
					{
						if (x is Label label1 && label1.Text == child.ToString())
						{
							field.Children.Remove(x);
							wordsfield.Remove(child);
							point += 20;
							for (int i = 0; i < labels.Count; i++)
							{
								labels[i].Text = "";

								cellindex = 0;

							}
						}
					}

					break;
				}
				switch (complextime)
				{
					case 0:
						wordwin = (cell1.Text + cell2.Text + cell3.Text + cell4.Text + cell5.Text).ToString();

						break;
					case 1:
						wordwin = (cell1.Text + cell2.Text + cell3.Text + cell4.Text + cell5.Text + cell6.Text + cell7.Text).ToString();

						break;
					case 2:
						wordwin = (cell1.Text + cell2.Text + cell3.Text + cell4.Text + cell5.Text + cell6.Text + cell7.Text
							+ cell8.Text + cell9.Text).ToString();
						break;
					default:
						break;
				}
			}

			poin.Text = point.ToString();


			foreach (var prov in field.Children.ToList())
			{
				if (prov is Label label1 && label1.TranslationY >= 370)
				{
					field.Children.Remove(prov);
					if (point >= 20)
					{
						point -= 20;
					}


				}
			}

		}
		List<Label> labels3 = new List<Label>();
		List<string> words = new List<string>();
		bool flagenabled = true;



		int complextime = Data.compl;
		double randomX;
		List<string> wordsfield = new List<string>();
		private void Timer2_Tick(object sender, EventArgs e)
		{
			var displayInfo = DeviceDisplay.MainDisplayInfo;
			var screenWidth = displayInfo.Width / displayInfo.Density;

			_time2 = _time2.Add(new TimeSpan(0, 0, -1));

			if (_time2.TotalSeconds == 0)
			{

				int randomword = random.Next(0, 70);
				randomX = random.Next(Convert.ToInt32(-screenWidth) + 250, Convert.ToInt32(screenWidth) - 250);
				var label = new Label
				{
					Text = words[randomword],
					TranslationY = -100,
					BackgroundColor = Colors.Transparent,
					WidthRequest = 100,
					TranslationX = randomX,
				};
				switch (Preferences.Default.Get("selthemedate", ""))
				{
					case "swhitetheme.png":
						label.TextColor = Colors.Black;
						break;
					case "spinktheme.png":
						label.TextColor = Colors.White;
						break;
					case "sblacktheme.png":
						label.TextColor = Colors.White;
						break;
					default:
						break;
				}



				field.Children.Add(label);

				wordsfield.Add(label.Text);
				_time2 = new TimeSpan(00, 00, complex);
				//Animation(label);


				label.TranslateTo(randomX, 370, 10000, Easing.Linear);

			}


		}





		void timer_complex()
		{
			switch (complextime)
			{
				case 0:
					_time = new TimeSpan(00, 5, 00);
					_time2 = new TimeSpan(00, 00, 05);
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
					_time = new TimeSpan(00, 3, 00);
					complex = 4;
					_time2 = new TimeSpan(00, 00, 04);
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
					_time = new TimeSpan(00, 2, 00);
					complex = 3;
					_time2 = new TimeSpan(00, 00, 3);
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
			double ohayo;
			_timer2.Stop();
			_timer.Stop();
			_timer3.Stop();

			flagenabled = false;
			//keyletter.IsVisible = false;
			clearone.IsEnabled = false;
			clear.IsEnabled = false;
			absmenu.IsVisible = true;
			pause.IsEnabled = false;
			foreach (var a in field.Children.ToList())
			{
				if (a is Label label1)
				{
					ohayo = label1.TranslationY;

					label1.TranslateTo(label1.TranslationX, ohayo, 10000, Easing.Linear);
				}


			}



		}

		private void start_Clicked(object sender, EventArgs e)
		{

			flagenabled = true;
			foreach (var a in field.Children.ToList())
			{

				if (a is Label label1)
				{
					label1.TranslateTo(label1.TranslationX, 370, 10000, Easing.Linear);
				}


			}

			_timer2.Start();
			_timer.Start();
			_timer3.Start();

			pause.IsEnabled = true;
			//keyletter.IsVisible = true;
			absmenu.IsVisible = false;
			clearone.IsEnabled = true;
			clear.IsEnabled = true;
		}
		private async void exmenu(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}

}
