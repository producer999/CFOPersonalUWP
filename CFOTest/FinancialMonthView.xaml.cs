using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CFOTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinancialMonthView : Page
    {

        public FinancialMonth CurrentMonth { get; private set; }

        /// <summary>
        /// User Configurable Settings
        /// </summary>

        public int InitialMonth { get; set; }
        public int InitialYear { get; set; }
        public DateTime InitialDate { get; set; }

        public SolidColorBrush BackgroundColor { get; set; }
        public SolidColorBrush HeadingTextColor { get; set; }
        public SolidColorBrush PivotBackgroundColor { get; set; }
        public SolidColorBrush HighlightColor { get; set; }

        public FinancialMonthView()
        {
            this.InitializeComponent();

            //
            // Initialize Configurable Setting variables
            //

            // TODO: Load the initial date from the Configurable Settings
            InitialDate = SettingsHelper.StartDate;

            InitialMonth = InitialDate.Month;
            InitialYear = InitialDate.Year;

            //
            // Initialize the view model/initial month view
            //

            CurrentMonth = new FinancialMonth(InitialMonth, InitialYear);
        }

        //
        // Event Handler Methods
        //

        private void ListViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                Border item = sender as Border;
                Button deleteButton = (Button)GetChildren(item).First(x => x.Name == "ListItemDeleteButton");
                Button editButton = (Button)GetChildren(item).First(x => x.Name == "EditButton");

                var data = deleteButton.DataContext;

                Button completeButton = null;

                if (data is Budget || data is Expense)
                {
                    try
                    {
                        completeButton = (Button)GetChildren(item).First(x => x.Name == "CompletedButton");
                    }
                    catch
                    {
                        completeButton = null;
                    }
                }

                deleteButton.Foreground = new SolidColorBrush(Colors.LightSeaGreen);
                editButton.Foreground = new SolidColorBrush(Colors.LightSeaGreen);

                if (completeButton != null)
                {
                    if (data is Expense exp)
                    {
                        if (!exp.IsPaid)
                        {
                            completeButton.Foreground = new SolidColorBrush(Colors.LightSeaGreen);
                        }
                    }
                    else if (data is Budget bud)
                    {
                        if (!bud.IsClosed)
                        {
                            completeButton.Foreground = new SolidColorBrush(Colors.LightSeaGreen);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void ListViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                Border item = sender as Border;
                Button deleteButton = (Button)GetChildren(item).First(x => x.Name == "ListItemDeleteButton");
                Button editButton = (Button)GetChildren(item).First(x => x.Name == "EditButton");

                var data = deleteButton.DataContext;

                Button completeButton = null;

                if (data is Budget || data is Expense)
                {
                    try
                    {
                        completeButton = (Button)GetChildren(item).First(x => x.Name == "CompletedButton");
                    }
                    catch
                    {
                        completeButton = null;
                    }
                }

                deleteButton.Foreground = new SolidColorBrush(Colors.Transparent);

                if (completeButton != null)
                {
                    if (data is Expense exp)
                    {
                        if (!exp.IsPaid)
                        {
                            completeButton.Foreground = new SolidColorBrush(Colors.Transparent);
                        }
                    }
                    else if (data is Budget bud)
                    {
                        if (!bud.IsClosed)
                        {
                            completeButton.Foreground = new SolidColorBrush(Colors.Transparent);
                        }
                    }

                }

                if ((string)editButton.Content == "\xE70F")
                {
                    editButton.Foreground = new SolidColorBrush(Colors.Transparent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void ReceiptImageOverlay_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ReceiptImageOverlay.Opacity = 1;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        }

        private void ReceiptImageOverlay_PointerEited(object sender, PointerRoutedEventArgs e)
        {
            ReceiptImageOverlay.Opacity = 0;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        private void MonthOverlay_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            PreviousMonthButton.Opacity = 1;
            NextMonthButton.Opacity = 1;
        }

        private void MonthOverlay_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            PreviousMonthButton.Opacity = 0;
            NextMonthButton.Opacity = 0;
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

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            button.Content = (string)button.Content == "\xE96E" ? "\xE96D" : "\xE96E";

            switch (button.Name)
            {
                case "IncomeExpandButton":
                    IncomeListView.Visibility = IncomeListView.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    IncomeAddButton.IsEnabled = !IncomeAddButton.IsEnabled;
                    break;
                case "ExpenseExpandButton":
                    ExpenseListView.Visibility = ExpenseListView.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    ExpenseAddButton.IsEnabled = !ExpenseAddButton.IsEnabled;
                    break;
                case "BudgetExpandButton":
                    BudgetListView.Visibility = BudgetListView.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    BudgetAddButton.IsEnabled = !BudgetAddButton.IsEnabled;
                    break;

            }

        }

        private async void ListItemAddButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            switch (button.Name)
            {
                case "IncomeAddButton":
                    CurrentMonth.AddIncome("New Income", 0, "Undefined");
                    await Task.Delay(570);
                    Button eb1 = (Button)GetChildren(IncomeListView).First(x => x.Name == "EditButton");
                    eb1.Foreground = new SolidColorBrush(Colors.Yellow);
                    ListItemEditButton_Click(eb1, new RoutedEventArgs());
                    break;
                case "ExpenseAddButton":
                    CurrentMonth.AddExpense("New Expense", 0, "Undefined", "Undefined");
                    await Task.Delay(570);
                    Button eb2 = (Button)GetChildren(ExpenseListView).First(x => x.Name == "EditButton");
                    eb2.Foreground = new SolidColorBrush(Colors.Yellow);
                    ListItemEditButton_Click(eb2, new RoutedEventArgs());
                    break;
                case "BudgetAddButton":
                    CurrentMonth.AddBudget("New Budget", 0, "Undefined");
                    await Task.Delay(570);
                    Button eb3 = (Button)GetChildren(BudgetListView).First(x => x.Name == "EditButton");
                    eb3.Foreground = new SolidColorBrush(Colors.Yellow);
                    ListItemEditButton_Click(eb3, new RoutedEventArgs());
                    break;
                case "AddReceiptButton":
                    CurrentMonth.AddReceipt("New Receipt", 0, DateTime.Now, CurrentMonth.CurrentBudget, "Undefined", "Undefined");
                    await Task.Delay(570);
                    ReceiptListView.SelectedItem = CurrentMonth.CurrentBudget.Receipts.Last();
                    await ReceiptEditor.ShowAsync();

                    CurrentMonth.UpdateItem(CurrentMonth.CurrentReceipt);
                    break;
            }
        }

        private async void ListItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)SettingsHelper.ReadLocalSetting(SettingsHelper.DisableConfirmationDialogToken))
            {
                var result = await ItemDeleteConfirmation.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    Button button = sender as Button;
                    var item = button.DataContext;

                    if (item is Budget budget)
                    {
                        if (budget == CurrentMonth.CurrentBudget)
                        {
                            ReceiptTabBudgetPlaceholder.Visibility = Visibility.Visible;
                            ReceiptTabHeaderText.Visibility = Visibility.Collapsed;
                            ReceiptTabTextDivider.Visibility = Visibility.Collapsed;
                            ReceiptTabBudgetName.Visibility = Visibility.Collapsed;
                            AddReceiptButton.Visibility = Visibility.Collapsed;
                            ReceiptListInformation.Visibility = Visibility.Collapsed;
                            BudgetClosedWarningText.Width = 0;
                            BudgetClosedWarningBorder.Width = 0;

                            ReceiptListView.Visibility = Visibility.Collapsed;
                        }
                    }

                    CurrentMonth.DeleteItem(item);
                }
            }
            else // If Disable Confirmation Dialog is set to True, just delete
            {
                Button button = sender as Button;
                var item = button.DataContext;

                if (item is Budget budget)
                {
                    if (budget == CurrentMonth.CurrentBudget)
                    {
                        ReceiptTabBudgetPlaceholder.Visibility = Visibility.Visible;
                        ReceiptTabHeaderText.Visibility = Visibility.Collapsed;
                        ReceiptTabTextDivider.Visibility = Visibility.Collapsed;
                        ReceiptTabBudgetName.Visibility = Visibility.Collapsed;
                        AddReceiptButton.Visibility = Visibility.Collapsed;
                        ReceiptListInformation.Visibility = Visibility.Collapsed;
                        BudgetClosedWarningText.Width = 0;
                        BudgetClosedWarningBorder.Width = 0;

                        ReceiptListView.Visibility = Visibility.Collapsed;
                    }
                }

                CurrentMonth.DeleteItem(item);
            }
        }

        private void ListItemEditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                RelativePanel panel = button.Parent as RelativePanel;

                var currentItem = button.DataContext; //get the item that we are editing so we can update it in the database

                if (currentItem is Income inc)
                {
                    if (inc.IsInEditMode == false) //if we are not in edit mode yet
                    {
                        inc.IsInEditMode = true;
                    }
                    else //if we are already in edit mode
                    {
                        var valueBox = (TextBox)GetChildren(panel).First(x => x.Name == "CurrencyTextbox");

                        decimal input = Convert.ToDecimal(valueBox.Text.Replace("$", ""));

                        if (TextBoxRegex.GetIsValid(valueBox) == true && !(input > 21474835))
                        {
                            inc.IsInEditMode = false;
                            valueBox.Text = String.Format("{0:C}", input);
                            CurrentMonth.UpdateItem(currentItem);
                        }
                        else
                        {
                            valueBox.Foreground = new SolidColorBrush(Colors.Red);
                        }
                    }
                }
                else if (currentItem is Expense exp)
                {
                    if (exp.IsInEditMode == false) //if we are not in edit mode yet
                    {
                        exp.IsInEditMode = true;
                    }
                    else //if we are already in edit mode
                    {
                        var valueBox = (TextBox)GetChildren(panel).First(x => x.Name == "CurrencyTextbox");

                        decimal input = Convert.ToDecimal(valueBox.Text.Replace("$", ""));

                        if (TextBoxRegex.GetIsValid(valueBox) == true && !(input > 21474835))
                        {
                            exp.IsInEditMode = false;
                            valueBox.Text = String.Format("{0:C}", input);
                            CurrentMonth.UpdateItem(currentItem);
                        }
                        else
                        {
                            valueBox.Foreground = new SolidColorBrush(Colors.Red);
                        }
                    }
                }
                else if (currentItem is Budget bud)
                {
                    if (bud.IsInEditMode == false) //if we are not in edit mode yet
                    {
                        bud.IsInEditMode = true;
                    }
                    else //if we are already in edit mode
                    {
                        var valueBox = (TextBox)GetChildren(panel).First(x => x.Name == "CurrencyTextbox");

                        decimal input = Convert.ToDecimal(valueBox.Text.Replace("$", ""));

                        if (TextBoxRegex.GetIsValid(valueBox) == true && !(input > 21474835))
                        {
                            bud.IsInEditMode = false;
                            valueBox.Text = String.Format("{0:C}", input);
                            CurrentMonth.UpdateItem(currentItem);
                        }
                        else
                        {
                            valueBox.Foreground = new SolidColorBrush(Colors.Red);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        //Used to allow users to hit Enter to save the Item when they are in the Currency (Amount) field
        private void ListItemCurrencyBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if(sender is TextBox textbox)
                {
                    var context = textbox.DataContext;

                    if(context is Income i)
                    {
                        CurrencyFormatConverter converter = new CurrencyFormatConverter();
                        int enteredAmount = (int)converter.ConvertBack(textbox.Text, null, null, null);

                        if(enteredAmount != i.Amount)
                        {
                            i.Amount = enteredAmount;
                        }
                    }
                    else if(context is Expense ex)
                    {
                        CurrencyFormatConverter converter = new CurrencyFormatConverter();
                        int enteredAmount = (int)converter.ConvertBack(textbox.Text, null, null, null);

                        if (enteredAmount != ex.Amount)
                        {
                            ex.Amount = enteredAmount;
                        }
                    }
                    else if(context is Budget b)
                    {
                        CurrencyFormatConverter converter = new CurrencyFormatConverter();
                        int enteredAmount = (int)converter.ConvertBack(textbox.Text, null, null, null);

                        if (enteredAmount != b.Amount)
                        {
                            b.Amount = enteredAmount;
                        }
                    }

                    var panel = textbox.Parent;

                    var editButton = (Button)GetChildren(panel).First(x => x.Name == "EditButton");
                    ListItemEditButton_Click(editButton, e);
                }
            }
        }

        //Used to allow users to hit Enter to save the Item when they are in the Label field
        private void ListItemTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if(sender is TextBox textbox)
                {
                    var context = textbox.DataContext;

                    if(context is Income i)
                    {
                        if(textbox.Text != i.Label)
                        {
                            i.Label = textbox.Text;
                        }
                    }
                    else if(context is Expense ex)
                    {
                        if(textbox.Text != ex.Label)
                        {
                            ex.Label = textbox.Text;
                        }
                    }
                    else if(context is Budget b)
                    {
                        if(textbox.Text != b.Label)
                        {
                            b.Label = textbox.Text;
                        }
                    }

                    var panel = textbox.Parent;

                    var editButton = (Button)GetChildren(panel).First(x => x.Name == "EditButton");
                    ListItemEditButton_Click(editButton, e);
                }
            }
        }


        private void BudgetListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Set up the view for the Receipt/Summary Pivot
            InformationView.SelectedItem = ReceiptsTab;

            ReceiptTabBudgetPlaceholder.Visibility = Visibility.Collapsed;
            ReceiptTabHeaderText.Visibility = Visibility.Visible;
            ReceiptTabTextDivider.Visibility = Visibility.Visible;
            ReceiptTabBudgetName.Visibility = Visibility.Visible;
            AddReceiptButton.Visibility = Visibility.Visible;
            ReceiptListInformation.Visibility = Visibility.Visible;
            BudgetClosedWarningText.Width = 400;
            BudgetClosedWarningBorder.Width = 400;

            ReceiptListView.Visibility = Visibility.Visible;
        }

        private async void ReceiptListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await ReceiptEditor.ShowAsync();

            CurrentMonth.UpdateItem(CurrentMonth.CurrentReceipt);
        }

        private void ListItemCompleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;

                var currentItem = button.DataContext;

                if (currentItem is Expense exp)
                {
                    exp.IsPaid = !exp.IsPaid;
                    CurrentMonth.UpdateItem(exp);
                    CurrentMonth.GetTodaysExpenses();
                }
                else if (currentItem is Budget bud)
                {
                    bud.IsClosed = !bud.IsClosed;
                    CurrentMonth.UpdateItem(bud);
                }
            }
            catch
            {

            }
        }

        private void ReceiptImageExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ReceiptEditorImageLargePopup.IsOpen)
                ReceiptEditorImageLargePopup.IsOpen = true;
        }

        private void ReceiptImageCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReceiptEditorImageLargePopup.IsOpen)
                ReceiptEditorImageLargePopup.IsOpen = false;
        }

        private void CopyToNextMonth_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            FinancialMonth.CopyItemToNextMonth(element.DataContext);
        }

        private void CopyToThisMonth_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            CurrentMonth.CopyItemToThisMonth(element.DataContext);
        }

        private void CopyTotalToNextMonth_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                switch (sender.Name)
                {
                    case "ExpenseTypeChooser":
                        sender.ItemsSource = ItemTypes.ExpenseTypes.Where(et => et.Label.ToLower().Contains(sender.Text.ToLower()));
                        break;
                    case "IncomeTypeChooser":
                        sender.ItemsSource = ItemTypes.IncomeTypes.Where(it => it.Label.ToLower().Contains(sender.Text.ToLower()));
                        break;
                    case "BudgetTypeChooser":
                        sender.ItemsSource = ItemTypes.BudgetTypes.Where(bt => bt.Label.ToLower().Contains(sender.Text.ToLower()));
                        break;
                    case "ReceiptTypeChooser":
                        sender.ItemsSource = ItemTypes.ReceiptTypes.Where(rt => rt.Label.ToLower().Contains(sender.Text.ToLower()));
                        break;
                    case "PayeeTypeChooser":
                        sender.ItemsSource = ItemTypes.Payees.Where(p => p.Label.ToLower().Contains(sender.Text.ToLower()));
                        break;
                    case "ReceiptPayeeChooser":
                        sender.ItemsSource = ItemTypes.Payees.Where(p => p.Label.ToLower().Contains(sender.Text.ToLower()));
                        break;
                }
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (sender != null)
            {
                switch (sender.Name)
                {
                    case "ExpenseTypeChooser":
                        var expense = sender.DataContext as Expense;

                        ExpenseTypeObject NewExpType;
                        ExpenseTypeObject PrevExpType = expense.Type; //TODO: DELETE, for testing only
                        
                        if (args.ChosenSuggestion != null) //If desired ExpenseType is a Type in the list, remove what was there and add the new type
                        {
                            NewExpType = args.ChosenSuggestion as ExpenseTypeObject;
                        }
                        else //If the desired ExpenseType does not already exist in the list, remove old type and add/create new type if its unique
                        {
                            if(!ItemTypes.ExpenseTypes.Any(et => et.Label.ToLower() == sender.Text.ToLower()))
                            {
                                NewExpType = new ExpenseTypeObject(sender.Text);
                                ItemTypes.AddExpenseType(NewExpType);

                                ExpenseCategoryAddedStoryboard.Begin();
                            }
                            else
                            {
                                NewExpType = ItemTypes.ExpenseTypes.Where(et => et.Label.ToLower() == sender.Text.ToLower()).First();
                            }
                        }

                        CurrentMonth.UpdateExpenseType(expense, NewExpType);
                        break;

                    case "PayeeTypeChooser":
                        var exp = sender.DataContext as Expense;

                        Payee NewPayee;
                        Payee PrevPayee = exp.Payee; //TODO: DELETE, for testing only

                        if (args.ChosenSuggestion != null) //If desired Payee is an existing Type, remove what was there and add the new payee
                        {
                            NewPayee = args.ChosenSuggestion as Payee;
                        }
                        else //If the desired Payee does not already exist, remove old type and add/create new payee
                        {
                            if(!ItemTypes.Payees.Any(p => p.Label.ToLower() == sender.Text.ToLower()))
                            {
                                NewPayee = new Payee(sender.Text);
                                ItemTypes.AddPayee(NewPayee);

                                ExpenseCategoryAddedStoryboard.Begin();
                            }
                            else
                            {
                                NewPayee = ItemTypes.Payees.Where(p => p.Label.ToLower() == sender.Text.ToLower()).First();
                            }
                        }

                        CurrentMonth.UpdateExpensePayee(exp, NewPayee);
                        break;

                    case "IncomeTypeChooser":
                        var income = sender.DataContext as Income;

                        IncomeTypeObject NewIncType;
                        IncomeTypeObject PrevIncType = income.Type; //TODO: DELETE, for testing only

                        if (args.ChosenSuggestion != null) //If desired IncomeType is an existing Type, remove what was there and add the new type
                        {
                            NewIncType = args.ChosenSuggestion as IncomeTypeObject;
                        }
                        else //If the desired IncomeType does not already exist, remove old type and add/create new type
                        {
                            if(!ItemTypes.IncomeTypes.Any(it => it.Label.ToLower() == sender.Text.ToLower()))
                            {
                                NewIncType = new IncomeTypeObject(sender.Text);
                                ItemTypes.AddIncomeType(NewIncType);

                                IncomeCategoryAddedStoryboard.Begin();
                            }
                            else
                            {
                                NewIncType = ItemTypes.IncomeTypes.Where(it => it.Label.ToLower() == sender.Text.ToLower()).First();
                            }
                        }

                        CurrentMonth.UpdateIncomeType(income, NewIncType);
                        break;

                    case "BudgetTypeChooser":
                        var budget = sender.DataContext as Budget;

                        BudgetTypeObject NewBudType;
                        BudgetTypeObject PrevBudType = budget.Type; //TODO: DELETE, for testing only

                        if (args.ChosenSuggestion != null) //If desired BudgetType is an existing Type, remove what was there and add the new type
                        {
                            NewBudType = args.ChosenSuggestion as BudgetTypeObject;
                        }
                        else //If the desired BudgetType does not already exist, remove old type and add/create new type
                        {
                            if(!ItemTypes.BudgetTypes.Any(bt => bt.Label.ToLower() == sender.Text.ToLower()))
                            {
                                NewBudType = new BudgetTypeObject(sender.Text);
                                ItemTypes.AddBudgetType(NewBudType);

                                BudgetCategoryAddedStoryboard.Begin();
                            }
                            else
                            {
                                NewBudType = ItemTypes.BudgetTypes.Where(bt => bt.Label.ToLower() == sender.Text.ToLower()).First();
                            }
                        }

                        CurrentMonth.UpdateBudgetType(budget, NewBudType);
                        break;

                    case "ReceiptTypeChooser":
                        if (CurrentMonth.CurrentReceipt != null)
                        {
                            var receipt = CurrentMonth.CurrentReceipt;

                            ReceiptTypeObject NewRecType;
                            ReceiptTypeObject PrevRecType = receipt.Type; //TODO: DELETE, for testing only

                            if (args.ChosenSuggestion != null) //If desired ReceiptType is an existing Type, remove what was there and add the new type
                            {
                                NewRecType = args.ChosenSuggestion as ReceiptTypeObject;
                            }
                            else //If the desired ReceiptType does not already exist, remove old type and add/create new type
                            {
                                if(!ItemTypes.ReceiptTypes.Any(rt => rt.Label.ToLower() == sender.Text.ToLower()))
                                {
                                    NewRecType = new ReceiptTypeObject(sender.Text, ItemTypes.GetRandomColorHex());
                                    ItemTypes.AddReceiptType(NewRecType);

                                    ReceiptCategoryAddedStoryboard.Begin();
                                }
                                else
                                {
                                    NewRecType = ItemTypes.ReceiptTypes.Where(rt => rt.Label.ToLower() == sender.Text.ToLower()).First();
                                }
                            }

                            CurrentMonth.UpdateReceiptType(receipt, NewRecType);
                        }
                        else
                        {
                            Debug.WriteLine("ERROR: Attempting to change the ReceiptType of a Null Receipt.");
                        }
                        break;

                    case "ReceiptPayeeChooser":
                        if (CurrentMonth.CurrentReceipt != null)
                        {
                            var receipt = CurrentMonth.CurrentReceipt;

                            Payee NewRecPayee;
                            Payee PrevRecPayee = receipt.Payee; //TODO: DELETE, for testing only

                            if (args.ChosenSuggestion != null) //If desired Payee is an existing Type, remove what was there and add the new payee
                            {
                                NewRecPayee = args.ChosenSuggestion as Payee;
                            }
                            else //If the desired Payee does not already exist, remove old type and add/create new payee
                            {
                                if(!ItemTypes.Payees.Any(p => p.Label.ToLower() == sender.Text.ToLower()))
                                {
                                    NewRecPayee = new Payee(sender.Text);
                                    ItemTypes.AddPayee(NewRecPayee);

                                    ReceiptCategoryAddedStoryboard.Begin();
                                }
                                else
                                {
                                    NewRecPayee = ItemTypes.Payees.Where(p => p.Label.ToLower() == sender.Text.ToLower()).First();
                                }
                            }

                            CurrentMonth.UpdateReceiptPayee(receipt, NewRecPayee);
                        }
                        else
                        {
                            Debug.WriteLine("ERROR: Attempting to change the Payee of a Null Receipt.");
                        }
                        break;

                }
            }
        }

        private void AutosuggestBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if(sender is AutoSuggestBox box)
                {
                    var panel = box.Parent;

                    var editButton = (Button)GetChildren(panel).First(x => x.Name == "EditButton");
                    ListItemEditButton_Click(editButton, e);
                }
            }
        }


        private void AutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is AutoSuggestBox box)
            {
                var source = e.OriginalSource;
                AutoSuggestBox_QuerySubmitted(box, new AutoSuggestBoxQuerySubmittedEventArgs());
            }
        }



        private void ReceiptEditorLargeImagePopup_Closed(object sender, object e)
        {
            // Reset the zoom level and positioning of the large image popup
            // ScrollViewer.ChangeView() doesn't work
            LargeImagePopupScrollViewer.ZoomToFactor(1.0f);
        }

        private void CalendarPicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            //Reset the UI for the new Month view
            InformationView.SelectedItem = SummaryTab;

            ReceiptTabBudgetPlaceholder.Visibility = Visibility.Visible;
            ReceiptTabHeaderText.Visibility = Visibility.Collapsed;
            ReceiptTabTextDivider.Visibility = Visibility.Collapsed;
            ReceiptTabBudgetName.Visibility = Visibility.Collapsed;
            ReceiptListInformation.Visibility = Visibility.Collapsed;
            AddReceiptButton.Visibility = Visibility.Collapsed;
            BudgetClosedWarningText.Width = 0;
            BudgetClosedWarningBorder.Width = 0;

            ReceiptListView.Visibility = Visibility.Collapsed;

            InverseDateOffsetFormatConverter converter = new InverseDateOffsetFormatConverter();
            DateTime newDate = (DateTime)converter.ConvertBack(sender.Date, null, null, null);

            CurrentMonth.GoToMonth(newDate);
        }

        private void ChangeMonthButton_Click(object sender, RoutedEventArgs e)
        {
            //Reset the UI for the new Month view
            InformationView.SelectedItem = SummaryTab;

            ReceiptTabBudgetPlaceholder.Visibility = Visibility.Visible;
            ReceiptTabHeaderText.Visibility = Visibility.Collapsed;
            ReceiptTabTextDivider.Visibility = Visibility.Collapsed;
            ReceiptTabBudgetName.Visibility = Visibility.Collapsed;
            ReceiptListInformation.Visibility = Visibility.Collapsed;
            AddReceiptButton.Visibility = Visibility.Collapsed;
            BudgetClosedWarningText.Width = 0;
            BudgetClosedWarningBorder.Width = 0;

            ReceiptListView.Visibility = Visibility.Collapsed;

            Button button = sender as Button;

            if (button.Name == "PreviousMonthButton")
            {
                CurrentMonth.GoToPreviousMonth();
            }
            else if (button.Name == "NextMonthButton")
            {
                CurrentMonth.GoToNextMonth();
            }
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Now;
            DateTime current = CurrentMonth.CurrentDate;

            if (current.Month != today.Month || current.Year != today.Year)
            {
                //Reset the UI for the new Month view
                InformationView.SelectedItem = SummaryTab;

                ReceiptTabBudgetPlaceholder.Visibility = Visibility.Visible;
                ReceiptTabHeaderText.Visibility = Visibility.Collapsed;
                ReceiptTabTextDivider.Visibility = Visibility.Collapsed;
                ReceiptTabBudgetName.Visibility = Visibility.Collapsed;
                ReceiptListInformation.Visibility = Visibility.Collapsed;
                AddReceiptButton.Visibility = Visibility.Collapsed;
                BudgetClosedWarningText.Width = 0;
                BudgetClosedWarningBorder.Width = 0;

                ReceiptListView.Visibility = Visibility.Collapsed;

                CurrentMonth.GoToMonth(today);
            }

        }

        private void ExpenseDueDateSelector_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void DueDateCalendar_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Input.Calendar.CurrentSelectionChangedEventArgs e)
        {
            if(CurrentMonth.CurrentExpense != null)
            {
                CurrentMonth.CurrentExpense.DueDate = (DateTime)e.NewSelection;
                CurrentMonth.UpdateItem(CurrentMonth.CurrentExpense);

                // TODO: Consider trying to make this method/functionality private only
                CurrentMonth.GetTodaysExpenses();
            }
        }

        private void SortMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox sortlist = sender as ListBox;

            if(sortlist.Name == "IncomeSortOptionsList")
            {
                CurrentMonth.SortIncomeList((IncomeSortOptions)sortlist.SelectedItem);
            }
            else if(sortlist.Name == "ExpenseSortOptionsList")
            {
                CurrentMonth.SortExpenseList((ExpenseSortOptions)sortlist.SelectedItem);
            }
            else if(sortlist.Name == "BudgetSortOptionsList")
            {
                CurrentMonth.SortBudgetList((BudgetSortOptions)sortlist.SelectedItem);
            }
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listview = sender as ListView;

            if(listview.Name == "IncomeListView" || listview.Name == "ExpenseListView")
            {
                if(InformationView.SelectedItem != SummaryTab)
                {
                    //Reset the UI since a Budget is not selected anymore
                    InformationView.SelectedItem = SummaryTab;

                    await Task.Delay(250);

                    ReceiptTabBudgetPlaceholder.Visibility = Visibility.Visible;
                    ReceiptTabHeaderText.Visibility = Visibility.Collapsed;
                    ReceiptTabTextDivider.Visibility = Visibility.Collapsed;
                    ReceiptTabBudgetName.Visibility = Visibility.Collapsed;
                    ReceiptListInformation.Visibility = Visibility.Collapsed;
                    AddReceiptButton.Visibility = Visibility.Collapsed;
                    BudgetClosedWarningText.Width = 0;
                    BudgetClosedWarningBorder.Width = 0;

                    ReceiptListView.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ReceiptSortButton_Checked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            ReceiptSortOptions sortOption = (ReceiptSortOptions)(Convert.ToInt16(radio.Tag));

            CurrentMonth.SortCurrentReceiptList(sortOption);
        }

        private void DisableConfirmDialog_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;

            SettingsHelper.SaveLocalSetting(SettingsHelper.DisableConfirmationDialogToken, checkbox.IsChecked);
        }
        
    }
}
