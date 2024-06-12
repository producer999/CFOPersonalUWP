using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml.Data;

namespace CFOTest
{
    public class CellModelToReceiptEventConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cellModel = value as CalendarCellModel;

            // Get a reference to the calendar container
            var calendar = cellModel.Presenter as RadCalendar;

            // Then you can get a reference to its DataContext (i.e. the page view model that holds the receipt information)
            //var receiptCalendarDataPoints = (calendar.DataContext as FinancialYear).ReceiptCalendarDataPoints;
            var calendarDataPoints = (calendar.DataContext as FinancialYear).CalendarDataPoints;

            // return custom label for receipt data cells
            var receiptData = calendarDataPoints.Where(data => data.Date.Day == cellModel.Date.Day &&
                                                               data.Date.Month == cellModel.Date.Month &&
                                                               data.Date.Year == cellModel.Date.Year).FirstOrDefault();
            if (receiptData.ReceiptDataPoint != null)
            {
                CurrencyFormatConverter converter = new CurrencyFormatConverter();
                
                return converter.Convert(receiptData.ReceiptDataPoint.Amount, null, null, null);
            }

            // return default label for regular cells
            return cellModel.Label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class CellModelToExpenseEventConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cellModel = value as CalendarCellModel;

            // Get a reference to the calendar container
            var calendar = cellModel.Presenter as RadCalendar;

            // Then you can get a reference to its DataContext (i.e. the page view model that holds the receipt information)
            //var expenseCalendarDataPoints = (calendar.DataContext as FinancialYear).ExpenseCalendarDataPoints;
            var calendarDataPoints = (calendar.DataContext as FinancialYear).CalendarDataPoints;

            // return custom label for receipt data cells
            var expenseData = calendarDataPoints.Where(data => data.Date.Day == cellModel.Date.Day &&
                                                               data.Date.Month == cellModel.Date.Month &&
                                                               data.Date.Year == cellModel.Date.Year).FirstOrDefault();
            if (expenseData.ExpenseDataPoint != null)
            {
                CurrencyFormatConverter converter = new CurrencyFormatConverter();

                return converter.Convert(expenseData.ExpenseDataPoint.Amount, null, null, null);
            }

            // return default label for regular cells
            return cellModel.Label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
