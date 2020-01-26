// Copyright 2020 Andreas Müller
// This file is a part of Amusoft.CodeAnalysis.Analyzers and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.CodeAnalysis.Analyzers/blob/master/LICENSE for details

using System;

namespace Generator.Shared.DependencyInjection
{
	public class CommandManagerDelegate
	{
		public static CommandManagerDelegate Instance = new CommandManagerDelegate();

		protected virtual event EventHandler OnRequerySuggested;

		public static event EventHandler RequerySuggested
		{
			add { Instance.OnRequerySuggested += value; }
			remove { Instance.OnRequerySuggested += value; }
		}
	}
}