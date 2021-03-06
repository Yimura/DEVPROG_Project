﻿using EindProject.Models;
using EindProject.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EindProject
{
    public partial class MainPage : MasterDetailPage
    {
        public readonly static NavigationMenu NavMenu = new NavigationMenu();
        public static User CurrentUser;
        public static Leader CurrentLeader;

        public MainPage()
        {
            InitializeComponent();

            NavMenu.SetMasterPage(masterPage);

            this.Init();
        }

        private async void Init()
        {
            NetworkAccess access = Connectivity.NetworkAccess;

            if (access == NetworkAccess.Unknown || access == NetworkAccess.None || access == NetworkAccess.Local)
            {
                await DisplayAlert("Alert", "This application requires an internet connection.\nPressing Ok will close the application.", "Ok");

                Process.GetCurrentProcess().Kill();
            }

            masterPage.lvwNavigation.ItemSelected += OnItemSelected;
            masterPage.lvwBottomNavigation.ItemSelected += OnItemSelected;

            if (Device.RuntimePlatform == Device.UWP)
            {
                MasterBehavior = MasterBehavior.Popover;
            }

            Detail = new NavigationPage(new LeaderboardPage());

            await this.SetUserContent();

            NavMenu.LoadNavigation();
        }

        private async Task SetUserContent()
        {
            MainPage.CurrentUser = await WakaTimeRepo.GetCurrentUser();

            if (MainPage.CurrentUser != null)
            {
                masterPage.gridUser.IsVisible = true;
                masterPage.gridUser.BindingContext = MainPage.CurrentUser;
            }

            Leaders leaders = await WakaTimeRepo.GetLeaders();
            MainPage.CurrentLeader = leaders.CurrentUser;
        }

        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            NavigationItem item = e.SelectedItem as NavigationItem;
            if (item == null) return;

            if (item.TargetType != null) Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
            else
            {
                switch (item.Title)
                {
                    case "Profile":
                        Detail = new NavigationPage(new UserPage(MainPage.CurrentLeader));

                        break;
                    case "Login":
                        if (await DisplayAlert("Login", "You are about to be redirected.", "Continue", "Cancel"))
                        {
                            try
                            {
                                await Browser.OpenAsync(WakaTimeRepo.RedirectUrl, BrowserLaunchMode.External);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Failed to start browser, the device might not have a browser installed.");

                                break;
                            }

                            string result = await DisplayPromptAsync("Token", "Go To https://wakatime.damon.sh if you didn't get redirected.\n\nPut the login token in the field below after authenticating.", "Confirm", "Cancel");

                            if (WakaTimeRepo.IsTokenValid(result))
                            {
                                App.cache.RemoveAll();

                                Preferences.Set("token", result);

                                (Application.Current).MainPage = new MainPage();
                            }
                            else DisplayAlert("Invalid Token", "An invalid WakaTime token has been inserted", "Ok");
                        }

                        break;
                    case "Logout":
                        if (await DisplayAlert("Logout?", "Are you sure you want to logout?", "Yes, please", "No, please take me back"))
                        {
                            App.cache.RemoveAll();

                            Preferences.Remove("token");

                            (Application.Current).MainPage = new MainPage();
                        }

                        break;
                    default:
                        break;
                }
            }
                
            IsPresented = false;
            masterPage.lvwNavigation.SelectedItem = null;
            masterPage.lvwBottomNavigation.SelectedItem = null;
        }
    }
}
