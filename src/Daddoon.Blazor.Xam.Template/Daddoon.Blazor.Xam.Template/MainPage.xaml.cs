﻿using Daddoon.Blazor.Xam.Template.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.Template
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            //Note: How to manage local files: https://docs.microsoft.com/fr-fr/xamarin/xamarin-forms/user-interface/webview?tabs=vswin

            var source = new HtmlWebViewSource();
            source.BaseUrl = DependencyService.Get<IBaseUrl>().Get();
            source.Html = @"<!DOCTYPE html>
                            <html>
                            <head>
                                <meta charset=""utf-8"" />
                                <title>Daddoon.Blazor.Xam.BlazorApp</title>
                                <base href=""/"" />
                                <link href=""css/bootstrap/bootstrap.min.css"" rel=""stylesheet"" />
                                <link href=""css/site.css"" rel=""stylesheet"" />
                            </head>
                            <body>
                                <app>Loading...</app>

                                <script src=""css/bootstrap/bootstrap-native.min.js""></script>
                                <script src=""_framework/blazor.js"" main=""Daddoon.Blazor.Xam.BlazorApp.dll"" entrypoint=""Daddoon.Blazor.Xam.BlazorApp.Program::Main"" references=""Microsoft.AspNetCore.Blazor.Browser.dll,Microsoft.AspNetCore.Blazor.dll,Microsoft.Extensions.DependencyInjection.Abstractions.dll,Microsoft.Extensions.DependencyInjection.dll,mscorlib.dll,netstandard.dll,System.Core.dll,System.dll,System.Net.Http.dll"" linker-enabled=""true""></script>
                            </body>
                            </html>
                            ";

            WebView webview = new WebView()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 1000,
                WidthRequest = 1000,
                Source = source
            };
            content.Children.Add(webview);
		}
	}
}
