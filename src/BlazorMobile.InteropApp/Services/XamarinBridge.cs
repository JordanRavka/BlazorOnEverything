﻿using BlazorMobile.Common;
using BlazorMobile.InteropApp.Common.Interfaces;
using BlazorMobile.InteropApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlazorMobile.InteropApp.Services
{
    public class XamarinBridge : IXamarinBridge
    {
        public async Task<List<string>> DisplayAlert(string title, string msg, string cancel)
        {
            await App.Current.MainPage.DisplayAlert(title, msg, cancel);

            List<string> result = new List<string>()
            {
                "Lorem",
                "Ipsum",
                "Dolorem",
            };

            return result;
        }

        public Task CallFaultyTask()
        {
            throw new InvalidOperationException("This is an expected exception");
        }

        public Task TriggerPostMessageTest()
        {
            MainPage.webview.PostMessage("PostMessageTest", "My posted string");
            return Task.CompletedTask;
        }

        public Task TriggerPostMessageTestBool()
        {
            MainPage.webview.PostMessage("PostMessageTest", true);
            return Task.CompletedTask;
        }

        public Task TriggerJSInvokableTest()
        {
            MainPage.webview.CallJSInvokableMethod("BlazorMobile.InteropBlazorApp", "InvokableMethodTest", "My invoked string");
            return Task.CompletedTask;
        }
    }
}
