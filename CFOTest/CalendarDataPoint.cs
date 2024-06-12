using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public class CalendarDataPoint
    {
        public DateTime Date { get; set; }

        public ReceiptCalendarDataPoint ReceiptDataPoint { get; set; }
        public ExpenseCalendarDataPoint ExpenseDataPoint { get; set; }

        public CalendarDataPoint()
        {

        }

        public CalendarDataPoint(DateTime date, ReceiptCalendarDataPoint receiptData, ExpenseCalendarDataPoint expenseData)
        {
            Date = date;

            ReceiptDataPoint = receiptData;
            ExpenseDataPoint = expenseData;
        }
    }
}
