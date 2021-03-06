﻿using BlazorMobile.Common.Services;
using BlazorMobile.Components;
using BlazorMobile.ElectronNET.Components;
using BlazorMobile.Helper;
using BlazorMobile.Interop;
using ElectronNET.API;
using Microsoft.AspNetCore.Routing;
using System;
using System.Runtime.Serialization;
using Xamarin.Forms;

namespace Microsoft.AspNetCore.Builder
{
    public static class BlazorHostingApplicationBuilderExtensions
    {
        /// <summary>
        /// Call of this method will notify to BlazorMobile that the runtime is actually running under ElectronNET (server-side) and will also start your ElectronNET application.
        /// NOTE: Usage of blazor.server.js in your starting page is mandatory.
        /// If using BlazorMobile.Build on your Blazor base project, you should add a link reference to 'server_index.cshtml' in your server project from the base project, and register this file at app start.
        /// See BlazorMobile documentation for more info.
        /// </summary>
        public static IApplicationBuilder UseBlazorMobileWithElectronNET<TFormsApplication>(this IApplicationBuilder app) where TFormsApplication : BlazorApplication
        {
            if (!BlazorMobileService.IsInitCalled())
            {
                throw new InvalidOperationException($"BlazorMobileService.Init() method should be called before calling {nameof(UseBlazorMobileWithElectronNET)}");
            }

            ContextHelper.SetElectronNETUsage(true);

            PlatformHelper.SetIsElectronActive(() => HybridSupport.IsElectronActive);

            //Bridging Native receive method in memory on ElectronNET implementation
            ContextHelper.SetNativeReceive(ContextBridge.Receive);
            BlazorWebViewFactory.SetInternalElectronBlazorWebView(typeof(ElectronBlazorWebView));

            app.UseStaticFiles();

            Forms.Init();

            return app;
        }

        /// <summary>
        /// Map the BlazorMobile Request validator controller to your app.
        /// This is needed in order to fire IBlazorWebView.Navigating events with Xamarin.Forms
        /// </summary>
        /// <param name="endpoints">The Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.</param>
        /// <returns></returns>
        public static ControllerActionEndpointConventionBuilder MapBlazorMobileRequestValidator(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapControllerRoute("blazorMobileRequest", "{controller=BlazorMobileRequest}/{action=Index}/{id?}");
        }
    }
}