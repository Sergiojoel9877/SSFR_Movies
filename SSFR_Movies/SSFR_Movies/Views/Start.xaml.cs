﻿using AsyncAwaitBestPractices.MVVM;
using Sharpnado.Tasks;
using SSFR_Movies.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSFR_Movies.Views
{
    public partial class Start : ContentPage
    {
        public Start()
        {
            InitializeComponent();

            TaskMonitor.Create(FireContainerCommand.ExecuteAsync());
        }

        readonly AsyncCommand fireContainerCommand;
        AsyncCommand FireContainerCommand
        {
            get => fireContainerCommand ?? (new AsyncCommand(async () =>
            {
                await Task.Yield();
      
                new Lazy<ContainerInitializer>(() => new ContainerInitializer()).Value.Initialize();

                await Device.InvokeOnMainThreadAsync(async ()=>
                {
                    ActIndicator.IsVisible = false;

                    ActIndicator.IsRunning = false;

                    Stack.IsVisible = true;

                    await ProBar.ProgressTo(.5, 200, Easing.Linear);
                });

                //var mainPage = new Lazy<AppShell>(() => new AppShell());

                await Task.WhenAll(Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Task.Yield();

                    Stack.IsVisible = false;

                    Application.Current.MainPage = new Lazy<AppShell>(() => new AppShell()).Value;

                }), ProBar.ProgressTo(100, 200, Easing.Linear));

                //await Device.InvokeOnMainThreadAsync(async () =>
                //{
                //    await ProBar.ProgressTo(100, 200, Easing.Linear);

                //    Stack.IsVisible = false;

                //    Application.Current.MainPage = mainPage.Value;
                //});
            }));
        }
    }
}