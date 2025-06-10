using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rainwords
{
	public static class CreatingLabels
	{

		private static readonly Queue<Label> _availableLabels = new Queue<Label>();//очередь свободных меток
		private static readonly List<Label> _activeLabels = new List<Label>(); //активные метки
		private static Color _currentTextColor;//цвет меток

		public static void Initialize(int initialPoolSize)//создает метки и добавляет в очередь
		{

			for (int i = 0; i < initialPoolSize; i++)
			{
				_availableLabels.Enqueue(CreateNewLabel());
			}
		}

		public static Label GetLabel(string text, double xPosition)//получение метки
		{
			Label label;

			if (_availableLabels.Count > 0)
			{
				label = _availableLabels.Dequeue();//берем из пула
			}
			else
			{
				label = CreateNewLabel();//или создаем новую если пул пуст
			}

			ConfigureLabel(label, text, xPosition);//настройка метки
			_activeLabels.Add(label);//добавляем в пул

			return label;
		}

		public static void ReleaseLabel(Label label)
		{
			if (label == null || !_activeLabels.Contains(label)) return;

			_activeLabels.Remove(label);//убираем из активных
			ResetLabel(label);//сбрасываем состояние
			_availableLabels.Enqueue(label);//возвращаем в пул для повторного использования
		}
		public static void Cleanup()
		{
			foreach (var label in _activeLabels)
			{
				if (label.Parent is Layout<View> parent)
				{
					parent.Children.Remove(label);// удаляем из родительского контейнера
				}
			}
			_activeLabels.Clear();//очищаем
			_availableLabels.Clear();
		}

		private static Label CreateNewLabel()//созание нового слова
		{
			return new Label
			{
				BackgroundColor = Colors.Transparent,
				WidthRequest = 100,
				TextColor = _currentTextColor
			};
		}

		private static void ConfigureLabel(Label label, string text, double xPosition)
		{
			label.Text = text;
			label.TranslationX = xPosition;
			label.TranslationY = -100;// начальная позиция
			label.Opacity = 1;
			label.Scale = 1;
			label.IsVisible = true;
			Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(label);// отмена предыдущие анимации
		}

		private static void ResetLabel(Label label)//сброс метки
		{
			label.Text = string.Empty;
			if (label.Parent is Layout<View> parent)
			{
				parent.Children.Remove(label);//удаляем из род контейнера
			}
			Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(label);//отмена анимации
		}

	}
}
