// Copyright 2020 Andreas Müller
// This file is a part of Amusoft.VisualStudio.TemplateGenerator and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.VisualStudio.TemplateGenerator/blob/master/LICENSE for details

using System;
using System.Threading.Tasks;
using Generator.Shared.DependencyInjection;

namespace Generator.Client.CommandLine.Dependencies
{
	public class UiService : IUIService
	{
		/// <inheritdoc />
		public Task<IProgressController> ShowProgressAsync(string title, string message, bool cancelable)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public bool GetYesNo(string question, string title)
		{
			again:
			Console.WriteLine(title);
			Console.WriteLine("(y)es or (n)o required.");

			switch (Console.ReadKey().KeyChar)
			{
				case 'y':
				case 'Y':
				case '1':
					return true;

				case 'n':
				case 'N':
				case '0':
					return false;

				default:
					goto again;
			}
		}

		/// <inheritdoc />
		public void DisplayMessage(string message, string title)
		{
			var before = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(title);
			Console.ForegroundColor = before;
			Console.WriteLine(message);
		}

		/// <inheritdoc />
		public void DisplayError(string message, string title)
		{
			var before = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(title);
			Console.ForegroundColor = before;
			Console.WriteLine(message);
		}
	}
}