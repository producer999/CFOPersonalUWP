using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public class FinancialYear : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<Receipt> Receipts;
        private List<Expense> Expenses;

        private DateTime CurrentDate;

        public ObservableCollection<DateTime> CalendarStartDates { get; set; }
        
        public ObservableCollection<CalendarDataPoint> CalendarDataPoints { get; set; }

        private int _year;
        public int Year
        {
            get { return _year; }
            set
            {
                _year = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Year"));
            }
        }

        private int _receiptsGrandTotal;
        public int ReceiptsGrandTotal
        {
            get
            {
                return _receiptsGrandTotal;
            }
            private set
            {
                if (_receiptsGrandTotal != value)
                {
                    _receiptsGrandTotal = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ReceiptsGrandTotal"));
                }
            }
        }

        private int _expensesGrandTotal;
        public int ExpensesGrandTotal
        {
            get
            {
                return _expensesGrandTotal;
            }
            private set
            {
                if (_expensesGrandTotal != value)
                {
                    _expensesGrandTotal = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpensesGrandTotal"));
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                _isLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLoading"));
            }
        }
        
        public bool IsInitialPageLoad { get; set; }
        

        public FinancialYear()
        {
            IsInitialPageLoad = true;

            CurrentDate = DateTime.Now;
            Year = CurrentDate.Year;

            Receipts = new List<Receipt>();
            Expenses = new List<Expense>();

            CalendarDataPoints = new ObservableCollection<CalendarDataPoint>();

            CalendarStartDates = new ObservableCollection<DateTime>();

            GetCalendarStartDates();

            RecalculateDataPoints();
        }

        public List<Receipt> GetReceiptsFromDate2(DateTime date)
        {
            CalendarDataPoint dataPoint = CalendarDataPoints.Where(data => data.Date.Day == date.Day
                                                                  && data.Date.Month == date.Month
                                                                  && data.Date.Year == date.Year).ToList().First();
            ReceiptCalendarDataPoint receiptData = dataPoint.ReceiptDataPoint;

            if (receiptData != null)
            {
                // TODO: Make a setting to choose the sort order
                return receiptData.Receipts.OrderBy(key => key.TypeId).ToList();
            }
            else
            {
                return null;
            }
        }

        public List<Expense> GetExpensesFromDate2(DateTime date)
        {
            CalendarDataPoint dataPoint = CalendarDataPoints.Where(data => data.Date.Day == date.Day
                                                                  && data.Date.Month == date.Month
                                                                  && data.Date.Year == date.Year).ToList().First();
            ExpenseCalendarDataPoint expenseData = dataPoint.ExpenseDataPoint;

            if (expenseData != null)
            {
                // TODO: Make a setting to choose the sort order
                return expenseData.Expenses.OrderBy(key => key.Amount).ToList();
            }
            else
            {
                return null;
            }
        }

        // TODO: This method breaks MVVM
        public Windows.UI.Xaml.Visibility IsReceiptDateVisibility2(DateTime date)
        {
            CalendarDataPoint dataPoint = CalendarDataPoints.Where(data => data.Date.Day == date.Day
                                                                  && data.Date.Month == date.Month
                                                                  && data.Date.Year == date.Year).ToList().First();
            ReceiptCalendarDataPoint receiptData = dataPoint.ReceiptDataPoint;

            if (receiptData != null)
            {
                return Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                return Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        // TODO: This method breaks MVVM
        public Windows.UI.Xaml.Visibility IsExpenseDateVisibility2(DateTime date)
        {
            CalendarDataPoint dataPoint = CalendarDataPoints.Where(data => data.Date.Day == date.Day
                                                                  && data.Date.Month == date.Month
                                                                  && data.Date.Year == date.Year).ToList().First();
            ExpenseCalendarDataPoint expenseData = dataPoint.ExpenseDataPoint;

            if (expenseData != null)
            {
                return Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                return Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        public string GetReceiptCountFromDate2(DateTime date)
        {
            CalendarDataPoint dataPoint = CalendarDataPoints.Where(data => data.Date.Day == date.Day
                                                                  && data.Date.Month == date.Month
                                                                  && data.Date.Year == date.Year).ToList().First();
            ReceiptCalendarDataPoint receiptData = dataPoint.ReceiptDataPoint;

            if (receiptData != null)
            {
                return receiptData.ReceiptCount.ToString();
            }
            else
            {
                return "0";
            }
        }

        public string GetExpenseCountFromDate2(DateTime date)
        {
            CalendarDataPoint dataPoint = CalendarDataPoints.Where(data => data.Date.Day == date.Day
                                                                 && data.Date.Month == date.Month
                                                                 && data.Date.Year == date.Year).ToList().First();
            ExpenseCalendarDataPoint expenseData = dataPoint.ExpenseDataPoint;

            if (expenseData != null)
            {
                return expenseData.ExpenseCount.ToString();
            }
            else
            {
                return "0";
            }
        }

        public string GetReceiptTotalFromDate2(DateTime date)
        {
            CalendarDataPoint dataPoint = CalendarDataPoints.Where(data => data.Date.Day == date.Day
                                                                  && data.Date.Month == date.Month
                                                                  && data.Date.Year == date.Year).ToList().First();
            ReceiptCalendarDataPoint receiptData = dataPoint.ReceiptDataPoint;

            if (receiptData != null)
            {
                CurrencyFormatConverter converter = new CurrencyFormatConverter();

                return (string)converter.Convert(receiptData.Amount, null, null, null);
            }
            else
            {
                return "$0.00";
            }
        }

        public string GetExpenseTotalFromDate2(DateTime date)
        {
            CalendarDataPoint dataPoint = CalendarDataPoints.Where(data => data.Date.Day == date.Day
                                                                  && data.Date.Month == date.Month
                                                                  && data.Date.Year == date.Year).ToList().First();
            ExpenseCalendarDataPoint expenseData = dataPoint.ExpenseDataPoint;

            if (expenseData != null)
            {
                CurrencyFormatConverter converter = new CurrencyFormatConverter();

                return (string)converter.Convert(expenseData.Amount, null, null, null);
            }
            else
            {
                return "$0.00";
            }
        }

        //
        // MONTH NAVIGATION METHODS
        //

        public async void GoToNextYear()
        {
            IsLoading = true;
            await Task.Delay(100);

            CurrentDate = CurrentDate.AddYears(1);
            Year = CurrentDate.Year;

            GetCalendarStartDates();

            RecalculateDataPoints();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalendarStartDates"));

            await Task.Delay(200);
            IsLoading = false;
        }

        public async void GoToPreviousYear()
        {
            IsLoading = true;
            await Task.Delay(100);

            CurrentDate = CurrentDate.AddYears(-1);
            Year = CurrentDate.Year;

            GetCalendarStartDates();

            RecalculateDataPoints();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalendarStartDates"));

            await Task.Delay(200);
            IsLoading = false;
        }

        public async void GoToYear(int year)
        {
            IsLoading = true;
            await Task.Delay(100);

            DateTime date = new DateTime(year, CurrentDate.Month, CurrentDate.Day);

            if (Year != date.Year)
            {
                CurrentDate = date;
                Year = date.Year;

                GetCalendarStartDates();

                RecalculateDataPoints();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalendarStartDates"));

            }

            await Task.Delay(200);
            IsLoading = false;
        }

        public void RefreshView()
        {
            IsLoading = true;

            CurrentDate = CurrentDate.AddYears(-1);
            Year = CurrentDate.Year;

            GetCalendarStartDates();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalendarStartDates"));

            CurrentDate = CurrentDate.AddYears(1);
            Year = CurrentDate.Year;

            GetCalendarStartDates();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalendarStartDates"));

            RecalculateDataPoints();

            IsLoading = false;
        }

        //
        // PRIVATE WORKER METHODS
        //

        private void GetCalendarStartDates()
        {
            ObservableCollection<DateTime> start = new ObservableCollection<DateTime>();

            for (int i = 1; i <= 12; i++)
            {
                var startDate = new DateTime(Year, i, 1);
                start.Add(startDate);
            }

            CalendarStartDates = start;
        }

        private void RecalculateDataPoints()
        {
            Receipts = DBHelper.GetReceiptsByYear(Year, false);
            Expenses = DBHelper.GetExpensesByYear(Year, false);

            ReceiptsGrandTotal = 0;
            ExpensesGrandTotal = 0;

            List<DateTime> allDates = new List<DateTime>();
            ObservableCollection<ReceiptCalendarDataPoint> newReceiptData = new ObservableCollection<ReceiptCalendarDataPoint>();
            ObservableCollection<ExpenseCalendarDataPoint> newExpenseData = new ObservableCollection<ExpenseCalendarDataPoint>();

            ObservableCollection<CalendarDataPoint> newCalendarData = new ObservableCollection<CalendarDataPoint>();


            if ((Receipts != null && Receipts.Count > 0) || (Expenses != null && Expenses.Count > 0)) // If there are some receipts for the year
            {
                foreach (Receipt r in Receipts) // Get a list of all the unique dates in the year for which 
                                                // there are receipts
                {
                    if (!(allDates.Any(d => d.Day == r.TransactionDate.Day
                                                 && d.Month == r.TransactionDate.Month
                                                 && d.Year == r.TransactionDate.Year)))
                    {
                        allDates.Add(r.TransactionDate);
                    }
                }

                foreach (Expense e in Expenses) // Get a list of all the unique dates in the year for which 
                                                // there are expenses due
                {
                    if (!(allDates.Any(d => d.Day == e.DueDate.Day
                                                 && d.Month == e.DueDate.Month
                                                 && d.Year == e.DueDate.Year)))
                    {
                        allDates.Add(e.DueDate);
                    }
                }

                if (allDates != null && allDates.Count > 0) // just check again that there are some valid dates
                {
                    int receiptgrandsum = 0; // for calculating the grand total of all the receipts of the year
                    int expensegrandsum = 0;

                    foreach (DateTime date in allDates) // For each date with a receipt, calculate the sum
                                                        // and count of the receipts for that date 
                                                        // and add that plus the receipts themselves
                                                        // to a new data point
                    {
                        ReceiptCalendarDataPoint newReceiptPoint = null;
                        ExpenseCalendarDataPoint newExpensePoint = null;

                        if (Receipts.Any(rec => rec.TransactionDate.Day == date.Day
                                                                    && rec.TransactionDate.Month == date.Month
                                                                    && rec.TransactionDate.Year == date.Year))
                        {
                            List<Receipt> query = Receipts.Where(rec => rec.TransactionDate.Day == date.Day
                                                                    && rec.TransactionDate.Month == date.Month
                                                                    && rec.TransactionDate.Year == date.Year).ToList();
                            if (query.Count > 0)
                            {
                                int sum = 0;
                                int count = 0;

                                List<Receipt> dataPointReceipts = new List<Receipt>();

                                foreach (Receipt receipt in query)
                                {
                                    sum += receipt.Amount;
                                    receiptgrandsum += receipt.Amount;
                                    count++;

                                    dataPointReceipts.Add(receipt);
                                }

                                newReceiptPoint = new ReceiptCalendarDataPoint(date, sum, count, dataPointReceipts);
                            }
                        }
                        else
                        {
                            newReceiptPoint = null;
                        }

                        if (Expenses.Any(rec => rec.DueDate.Day == date.Day
                                                                     && rec.DueDate.Month == date.Month
                                                                     && rec.DueDate.Year == date.Year))
                        {
                            List<Expense> query = Expenses.Where(rec => rec.DueDate.Day == date.Day
                                                                    && rec.DueDate.Month == date.Month
                                                                    && rec.DueDate.Year == date.Year).ToList();
                            if (query.Count > 0)
                            {
                                int sum = 0;
                                int count = 0;

                                List<Expense> dataPointExpenses = new List<Expense>();

                                foreach (Expense expense in query)
                                {
                                    sum += expense.Amount;
                                    expensegrandsum += expense.Amount;
                                    count++;

                                    dataPointExpenses.Add(expense);
                                }

                                newExpensePoint = new ExpenseCalendarDataPoint(date, sum, count, dataPointExpenses);
                            }
                        }
                        else
                        {
                            newExpensePoint = null;
                        }

                        newCalendarData.Add(new CalendarDataPoint(date, newReceiptPoint, newExpensePoint));
                    }

                    CalendarDataPoints = newCalendarData;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalendarDataPoints"));

                    ReceiptsGrandTotal = receiptgrandsum;
                    ExpensesGrandTotal = expensegrandsum;
                }
            }
            else // If there are no receipts for the year, simply update the view
            {
                CalendarDataPoints = newCalendarData;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalendarDataPoints"));
            }
        }

    }
}
