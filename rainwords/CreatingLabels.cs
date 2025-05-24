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

		private static readonly Queue<Label> _availableLabels = new Queue<Label>();
		private static readonly List<Label> _activeLabels = new List<Label>();
		private static Color _currentTextColor;

		public static void Initialize(int initialPoolSize)
		{

			for (int i = 0; i < initialPoolSize; i++)
			{
				_availableLabels.Enqueue(CreateNewLabel());
			}
		}

		public static Label GetLabel(string text, double xPosition)
		{
			Label label;

			if (_availableLabels.Count > 0)
			{
				label = _availableLabels.Dequeue();
			}
			else
			{
				label = CreateNewLabel();
			}

			ConfigureLabel(label, text, xPosition);
			_activeLabels.Add(label);

			return label;
		}

		public static void ReleaseLabel(Label label)
		{
			if (label == null || !_activeLabels.Contains(label)) return;

			_activeLabels.Remove(label);
			ResetLabel(label);
			_availableLabels.Enqueue(label);
		}
		public static void Cleanup()
		{
			foreach (var label in _activeLabels)
			{
				if (label.Parent is Layout<View> parent)
				{
					parent.Children.Remove(label);
				}
			}
			_activeLabels.Clear();
			_availableLabels.Clear();
		}

		private static Label CreateNewLabel()
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
			label.TranslationY = -100;
			label.Opacity = 1;
			label.Scale = 1;
			label.IsVisible = true;
			Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(label);
		}

		private static void ResetLabel(Label label)
		{
			label.Text = string.Empty;
			if (label.Parent is Layout<View> parent)
			{
				parent.Children.Remove(label);
			}
			Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(label);
		}

	}
}
