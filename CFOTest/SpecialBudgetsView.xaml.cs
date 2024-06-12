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
    public sealed partial class SpecialBudgetsView : Page
    {
        public SpecialBudgets AllBudgets { get; private set; }

        public SpecialBudgetsView()
        {
            this.InitializeComponent();

            AllBudgets = new SpecialBudgets();

        }

        //
        // CLICK EVENT HANDLERS
        //

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            button.Content = (string)button.Content == "\xE96E" ? "\xE96D" : "\xE96E";

            switch (button.Name)
            {
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
                case "BudgetAddButton":
                    AllBudgets.AddBudget("New Budget", 0, "Undefined");
                    await Task.Delay(570);
                    Button eb3 = (Button)GetChildren(BudgetListView).First(x => x.Name == "EditButton");
                    eb3.Foreground = new SolidColorBrush(Colors.Yellow);
                    ListItemEditButton_Click(eb3, new RoutedEventArgs());
                    break;
                case "AddReceiptButton":
                    AllBudgets.AddReceipt("New Receipt", 0, DateTime.Now, AllBudgets.CurrentBudget, "Undefined", "Undefined");
                    await Task.Delay(570);
                    ReceiptListView.SelectedItem = AllBudgets.CurrentBudget.Receipts.Last();
                    await ReceiptEditor.ShowAsync();

                    AllBudgets.UpdateItem(AllBudgets.CurrentReceipt);
                    break;
            }
        }

        private void ListItemEditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                RelativePanel panel = button.Parent as RelativePanel;

                var currentItem = button.DataContext; //get the item that we are editing so we can update it in the database
                
                if (currentItem is Budget bud)
                {
                    if (bud.IsInEditMode == false) //if we are not in edit mode yet
                    {
                        bud.IsInEditMode = true;
                    }
                    else //if we are already in edit mode
                    {
                        var valueBox = (TextBox)GetChildren(panel).First(x => x.Name == "CurrencyTextbox");

                        if (TextBoxRegex.GetIsValid(valueBox) == true)
                        {
                            bud.IsInEditMode = false;
                            AllBudgets.UpdateItem(currentItem);
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

        private void ListItemCompleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;

                var currentItem = button.DataContext;

                if (currentItem is Budget bud)
                {
                    bud.IsClosed = !bud.IsClosed;
                    AllBudgets.UpdateItem(bud);
                }
            }
            catch
            {

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
                        if (budget == AllBudgets.CurrentBudget)
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

                    AllBudgets.DeleteItem(item);
                }
            }
            else // If Disable Confirmation Dialog is set to True, just delete
            {
                Button button = sender as Button;
                var item = button.DataContext;

                if (item is Budget budget)
                {
                    if (budget == AllBudgets.CurrentBudget)
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

                AllBudgets.DeleteItem(item);
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

            AllBudgets.UpdateItem(AllBudgets.CurrentReceipt);
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

        //
        // MOUSE EVENT HANDLERS
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

                if (data is Budget)
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
                    if (data is Budget bud)
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

                if (data is Budget)
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
                    if (data is Budget bud)
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

        private void ReceiptImageOverlay_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ReceiptImageOverlay.Opacity = 0;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        //
        // OTHER EVENT HANDLERS
        //

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                switch (sender.Name)
                {
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
                            if (!ItemTypes.BudgetTypes.Any(bt => bt.Label.ToLower() == sender.Text.ToLower()))
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

                        AllBudgets.UpdateBudgetType(budget, NewBudType);
                        break;

                    case "ReceiptTypeChooser":
                        if (AllBudgets.CurrentReceipt != null)
                        {
                            var receipt = AllBudgets.CurrentReceipt;

                            ReceiptTypeObject NewRecType;
                            ReceiptTypeObject PrevRecType = receipt.Type; //TODO: DELETE, for testing only

                            if (args.ChosenSuggestion != null) //If desired ReceiptType is an existing Type, remove what was there and add the new type
                            {
                                NewRecType = args.ChosenSuggestion as ReceiptTypeObject;
                            }
                            else //If the desired ReceiptType does not already exist, remove old type and add/create new type
                            {
                                if (!ItemTypes.ReceiptTypes.Any(rt => rt.Label.ToLower() == sender.Text.ToLower()))
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

                            AllBudgets.UpdateReceiptType(receipt, NewRecType);
                        }
                        else
                        {
                            Debug.WriteLine("ERROR: Attempting to change the ReceiptType of a Null Receipt.");
                        }
                        break;

                    case "ReceiptPayeeChooser":
                        if (AllBudgets.CurrentReceipt != null)
                        {
                            var receipt = AllBudgets.CurrentReceipt;

                            Payee NewRecPayee;
                            Payee PrevRecPayee = receipt.Payee; //TODO: DELETE, for testing only

                            if (args.ChosenSuggestion != null) //If desired Payee is an existing Type, remove what was there and add the new payee
                            {
                                NewRecPayee = args.ChosenSuggestion as Payee;
                            }
                            else //If the desired Payee does not already exist, remove old type and add/create new payee
                            {
                                if (!ItemTypes.Payees.Any(p => p.Label.ToLower() == sender.Text.ToLower()))
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

                            AllBudgets.UpdateReceiptPayee(receipt, NewRecPayee);
                        }
                        else
                        {
                            Debug.WriteLine("ERROR: Attempting to change the Payee of a Null Receipt.");
                        }
                        break;

                }
            }
        }

        private void SortMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox sortlist = sender as ListBox;

            if (sortlist.Name == "BudgetSortOptionsList")
            {
                AllBudgets.SortBudgetList((BudgetSortOptions)sortlist.SelectedItem);
            }
        }

        private void ReceiptSortButton_Checked(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            ReceiptSortOptions sortOption = (ReceiptSortOptions)(Convert.ToInt16(radio.Tag));

            AllBudgets.SortCurrentReceiptList(sortOption);
        }

        private void ReceiptEditorLargeImagePopup_Closed(object sender, object e)
        {
            // Reset the zoom level and positioning of the large image popup
            // ScrollViewer.ChangeView() doesn't work
            LargeImagePopupScrollViewer.ZoomToFactor(1.0f);
        }

        private void DisableConfirmDialog_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;

            SettingsHelper.SaveLocalSetting(SettingsHelper.DisableConfirmationDialogToken, checkbox.IsChecked);
        }

        //
        // HELPERS
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

        private void CopyToThisMonth_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            AllBudgets.CopyItemToThisMonth(element.DataContext);
        }

        private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
    }
}
