using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CFOTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public Frame AppFrame { get { return ContentFrame; } }

        public SettingsHelper AppSettings = new SettingsHelper();

        public MainPage()
        {

            this.InitializeComponent();

            DBHelper.CreateDatabase("CFOTEST");

            AppFrame.Navigate(typeof(FinancialMonthView));

            Window.Current.SizeChanged += (sender, args) => 
            {
                SettingsPopupBorder.Height = Window.Current.Bounds.Height;
                SettingsPopupBorder.Width = Window.Current.Bounds.Width-50;
            };
        }

        //
        // Event Handler Methods
        //

        private List<FrameworkElement> GetChildren(DependencyObject parent)
        {
            List<FrameworkElement> controls = new List<FrameworkElement>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement)
                {
                    controls.Add(child as FrameworkElement);
                }
                controls.AddRange(GetChildren(child));
            }

            return controls;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if(SettingsPopup.IsOpen)
            {
                SettingsPopup.IsOpen = false;
            }

            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private async void NavigationMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SettingsPopup.IsOpen)
            {
                SettingsPopup.IsOpen = false;
            }

            if (sender is ListBox list)
            {
                ListBoxItem item = (ListBoxItem)list.SelectedItem;

                if(item != null)
                {
                    var currentPage = AppFrame.Content as Page;

                    switch(item.Name)
                    {
                        case "MonthView":
                            if (currentPage == null || !(currentPage.GetType() == typeof(FinancialMonthView)))
                            {
                                LoadingPage.IsLoading = true;
                                await Task.Delay(100);
                                AppFrame.Navigate(typeof(FinancialMonthView));
                                LoadingPage.IsLoading = false;
                            }
                            break;

                        case "YearView":
                            if (currentPage == null || !(currentPage.GetType() == typeof(FinancialYearView)))
                            {
                                if ((AppSettings.IsLicenseActive && !AppSettings.IsLicenseTrial) ||
                                    AppSettings.IsLicenseActive && (AppSettings.IsLicenseTrial && !AppSettings.IsTrialExpired))
                                {
                                    LoadingPage.IsLoading = true;
                                    await Task.Delay(100);
                                    AppFrame.Navigate(typeof(FinancialYearView));
                                    LoadingPage.IsLoading = false;
                                }
                                else
                                {
                                    SettingsButton_Tapped(null, null);
                                }
                            }
                            break;

                        case "SpecialBudgets":
                            if (currentPage == null || !(currentPage.GetType() == typeof(SpecialBudgetsView)))
                            {
                                    LoadingPage.IsLoading = true;
                                    await Task.Delay(100);
                                    AppFrame.Navigate(typeof(SpecialBudgetsView));
                                    LoadingPage.IsLoading = false;
                            }
                            break;

                        case "ReceiptArchive":
                            if (currentPage == null || !(currentPage.GetType() == typeof(ReceiptArchiveView)))
                            {
                                if ((AppSettings.IsLicenseActive && !AppSettings.IsLicenseTrial) ||
                                    AppSettings.IsLicenseActive && (AppSettings.IsLicenseTrial && !AppSettings.IsTrialExpired))
                                {
                                    LoadingPage.IsLoading = true;
                                    await Task.Delay(100);
                                    AppFrame.Navigate(typeof(ReceiptArchiveView));
                                    LoadingPage.IsLoading = false;
                                }
                                else
                                {
                                    SettingsButton_Tapped(null, null);
                                }
                            }
                            break;
                            
                        default:
                            break;

                    }
                }
            }
        }

        private void CalculatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsPopup.IsOpen)
            {
                SettingsPopup.IsOpen = false;
            }

            CalculatorPopup.Visibility = CalculatorPopup.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsPopup.IsOpen)
            {
                SettingsPopup.IsOpen = false;
            }

            AppSettings.RefreshLicenseInfo();

            await HelpDialogue.ShowAsync();
        }

        private void SettingsButton_Tapped(object sender, RoutedEventArgs e)
        {
            if(MySplitView.IsPaneOpen)
            {
                MySplitView.IsPaneOpen = false;
            }

            SettingsPopup.IsOpen = !SettingsPopup.IsOpen;
            
            if (SettingsPopup.IsOpen)
            {
                AppSettings.RefreshLicenseInfo();

                SettingsPopupBorder.Height = Window.Current.Bounds.Height;
                SettingsPopupBorder.Width = Window.Current.Bounds.Width - 50;

                SettingsSlideInStoryboard.Begin();
            }
        }

        private async void ClearDeletedReceipts_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.IsReceiptClearInProgress = true;
            await Task.Run(() => AppSettings.RemoveDeletedReceiptsAsync());
            AppSettings.IsReceiptClearInProgress = false;
        }

        private async void BuyNow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string receipt = await LicenseHelper.BuyAppLicenseAsync();

                if (!String.IsNullOrWhiteSpace(receipt))
                {
                    HelpDialogue.Hide();

                    await PurchaseDialogue.ShowAsync();

                    SettingsHelper.SaveLocalSetting(SettingsHelper.PurchaseDateToken, DateTime.Now);
                    AppSettings.RefreshLicenseInfo();
                }
                else
                {

                }
            }
            catch
            {
                HelpDialogue.Hide();

                ContentDialog error = new ContentDialog();
                error.Content = "There was a problem with your purchase, please try again or check on the Windows Store for more details.";
                error.PrimaryButtonText = "OK";
                await error.ShowAsync();
            }
        }
    }
}
