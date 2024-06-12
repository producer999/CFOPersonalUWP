using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml;

namespace CFOTest
{
    public class CustomCellStyleSelector : CalendarCellStyleSelector
    {
        public DataTemplate EventTemplate { get; set; }

        protected override void SelectStyleCore(CalendarCellStyleContext context, RadCalendar container)
        {
            var calendarDataPoints = (container.DataContext as FinancialYear).CalendarDataPoints;
            
            // If either an expense or a receipt exists on the date in question, use the special cell template
            if (calendarDataPoints.Where(data => data.Date.Day == context.Date.Day &&
                                                 data.Date.Month == context.Date.Month &&
                                                 data.Date.Year == context.Date.Year).Count() > 0)
            {
                context.CellTemplate = this.EventTemplate;
            }
        }
    }
}
