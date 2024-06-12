using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Telerik.UI.Xaml.Controls.Input.Calendar.Commands;

namespace CFOTest
{
    public class CustomCellTapCommand : CalendarCommand
    {
        public CustomCellTapCommand()
        {
            this.Id = CommandId.CellTap;
        }

        public override bool CanExecute(object parameter)
        {
            //Disable the CellTapCommand by setting it false
            return false;
        }

        public override void Execute(object parameter)
        {

        }
    }

    public class CustomMoveToNextViewCommand : CalendarCommand
    {
        public CustomMoveToNextViewCommand()
        {
            this.Id = CommandId.MoveToNextView;
        }

        public override bool CanExecute(object parameter)
        {
            //Disable the MoveToNextViewCommand by setting it false
            return false;
        }

        public override void Execute(object parameter)
        {

        }
    }

    public class CustomMoveToPreviousViewCommand : CalendarCommand
    {
        public CustomMoveToPreviousViewCommand()
        {
            this.Id = CommandId.MoveToPreviousView;
        }

        public override bool CanExecute(object parameter)
        {
            //Disable the MoveToPreviousViewCommand by setting it false
            return false;
        }

        public override void Execute(object parameter)
        {

        }
    }

    public class CustomMoveToUpperViewCommand : CalendarCommand
    {
        public CustomMoveToUpperViewCommand()
        {
            this.Id = CommandId.MoveToUpperView;
        }

        public override bool CanExecute(object parameter)
        {
            //Disable the MoveToUpperCommand by setting it false
            return false;
        }

        public override void Execute(object parameter)
        {

        }
    }

    public class CustomMoveToLowerViewCommand : CalendarCommand
    {
        public CustomMoveToLowerViewCommand()
        {
            this.Id = CommandId.MoveToLowerView;
        }

        public override bool CanExecute(object parameter)
        {
            //Disable the MoveToLowerCommand by setting it false
            return false;
        }

        public override void Execute(object parameter)
        {

        }
    }
}
