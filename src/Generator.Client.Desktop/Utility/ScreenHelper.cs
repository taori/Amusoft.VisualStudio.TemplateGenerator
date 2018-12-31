using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Rect = System.Drawing.Rectangle;

namespace Generator.Client.Desktop.Utility
{
	public class ScreenHelper
	{
		public static IEnumerable<Screen> GetXIntersects(Window window)
		{
			var screens = GetTaskbarRects();
			var rect = new Rect((int) window.Left, (int) window.Top, (int) window.Width, (int) window.Height);
			return screens.Where(d => d.Value.IntersectsWith(rect)).Select(s => s.Key);
		}

		public static Dictionary<Screen, Rectangle> GetTaskbarRects()
		{
			var results = new Dictionary<Screen, Rectangle>();
			foreach (var screen in Screen.AllScreens)
			{
				if (!screen.Bounds.Equals(screen.WorkingArea))
				{
					Rectangle rect = new Rectangle();

					var leftDockedWidth = Math.Abs((Math.Abs(screen.Bounds.Left) - Math.Abs(screen.WorkingArea.Left)));
					var topDockedHeight = Math.Abs((Math.Abs(screen.Bounds.Top) - Math.Abs(screen.WorkingArea.Top)));
					var rightDockedWidth = ((screen.Bounds.Width - leftDockedWidth) - screen.WorkingArea.Width);
					var bottomDockedHeight = ((screen.Bounds.Height - topDockedHeight) - screen.WorkingArea.Height);
					if ((leftDockedWidth > 0))
					{
						rect.X = screen.Bounds.Left;
						rect.Y = screen.Bounds.Top;
						rect.Width = leftDockedWidth;
						rect.Height = screen.Bounds.Height;
					}
					else if ((rightDockedWidth > 0))
					{
						rect.X = screen.WorkingArea.Right;
						rect.Y = screen.Bounds.Top;
						rect.Width = rightDockedWidth;
						rect.Height = screen.Bounds.Height;
					}
					else if ((topDockedHeight > 0))
					{
						rect.X = screen.WorkingArea.Left;
						rect.Y = screen.Bounds.Top;
						rect.Width = screen.WorkingArea.Width;
						rect.Height = topDockedHeight;
					}
					else if ((bottomDockedHeight > 0))
					{
						rect.X = screen.WorkingArea.Left;
						rect.Y = screen.WorkingArea.Bottom;
						rect.Width = screen.WorkingArea.Width;
						rect.Height = bottomDockedHeight;
					}
					else
					{
						// Nothing found!
					}

					results.Add(screen, rect);
				}
			}

			return results;
		}
	}
}