﻿// Copyright (c) 2023 Files Community
// Licensed under the MIT License. See the LICENSE.

using Files.App.Commands;
using Files.App.Contexts;
using Files.App.Shell;
using Files.Backend.Helpers;

namespace Files.App.Actions
{
	internal class InstallInfDriverAction : ObservableObject, IAction
	{
		private readonly IContentPageContext context = Ioc.Default.GetRequiredService<IContentPageContext>();

		public string Label => "Install".GetLocalizedResource();
		
		public string Description => "InstallInfDriverDescription".GetLocalizedResource();

		public RichGlyph Glyph { get; } = new("\uE9F5");

		public bool IsExecutable => context.SelectedItems.Count == 1 &&
			FileExtensionHelpers.IsInfFile(context.SelectedItems[0].FileExtension) &&
			context.PageType is not ContentPageTypes.RecycleBin and not ContentPageTypes.ZipFolder;

		public InstallInfDriverAction()
		{
			context.PropertyChanged += Context_PropertyChanged;
		}

		public async Task ExecuteAsync()
		{
			foreach (ListedItem selectedItem in context.SelectedItems)
				await Win32API.InstallInf(selectedItem.ItemPath);
		}

		public void Context_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(IContentPageContext.SelectedItems):
				case nameof(IContentPageContext.PageType):
					OnPropertyChanged(nameof(IsExecutable));
					break;
			}
		}
	}
}
