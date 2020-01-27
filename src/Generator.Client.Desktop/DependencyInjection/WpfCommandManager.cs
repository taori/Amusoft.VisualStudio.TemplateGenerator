// Copyright 2020 Andreas Müller
// This file is a part of Amusoft.CodeAnalysis.Analyzers and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.CodeAnalysis.Analyzers/blob/master/LICENSE for details

using System;
using System.Windows.Input;
using Generator.Shared.DependencyInjection;

namespace Generator.Client.Desktop.DependencyInjection
{
	public class WpfCommandManager : CommandManagerDelegate
	{
		/// <inheritdoc />
		protected override event EventHandler OnRequerySuggested
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}
	}
}