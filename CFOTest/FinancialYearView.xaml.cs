using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CFOTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinancialYearView : Page
    {

        public FinancialYear CurrentYear { get; private set; }

        public FinancialYearView()
        {
            this.InitializeComponent();

            CurrentYear = new FinancialYear();
            this.DataContext = CurrentYear;

        }

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

        private void ChangeYearButton_Click(object sender, RoutedEventArgs e)
        {  

            Button button = sender as Button;

            if (button.Name == "PreviousYearButton")
            {
                CurrentYear.GoToPreviousYear();
            }
            else if (button.Name == "NextYearButton")
            {
                CurrentYear.GoToNextYear();
            }
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Now;

            CurrentYear.GoToYear(today.Year);
        }


        private void YearOverlay_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            PreviousYearButton.Opacity = 1;
            NextYearButton.Opacity = 1;
        }

        private void YearOverlay_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            PreviousYearButton.Opacity = 0;
            NextYearButton.Opacity = 0;
        }

        private void CalendarPicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            InverseDateOffsetFormatConverter converter = new InverseDateOffsetFormatConverter();
            DateTime newDate = (DateTime)converter.ConvertBack(sender.Date, null, null, null);

            CurrentYear.GoToYear(newDate.Year);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
          
            if(CurrentYear.IsInitialPageLoad == true)
            {
                CurrentYear.IsInitialPageLoad = false;
            }
            else
            {
                // Refresh the view and the data on the CurrentYear
                // Why the view needs to go to the next year and come back to update is beyond me.
                CurrentYear.RefreshView();
            }
        }

        private void ReceiptCalendarCell_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
    }
}
