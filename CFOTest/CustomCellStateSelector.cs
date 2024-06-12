using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;

namespace CFOTest
{
    public class CustomCellStateSelector : CalendarCellStateSelector
    {
        protected override void SelectStateCore(CalendarCellStateContext context, RadCalendar container)
        {
            if (context.Date.Month != container.DisplayDate.Month)
            {
                context.IsBlackout = true;
            }
        }
    }
}
