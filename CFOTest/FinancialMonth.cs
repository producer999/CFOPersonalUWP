using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CFOTest
{
    public class FinancialMonth : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _month;
        public int Month
        {
            get { return _month; }
            set
            {
                _month = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Month"));
            }        
        }

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

        private string _monthId;

        private int _incomeTotal;
        public int IncomeTotal
        {
            get
            {    
                return _incomeTotal;
            }
            private set
            {
                if (_incomeTotal != value)
                {
                    _incomeTotal = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncomeTotal"));
                }
            }
        }

        private int _expenseTotal;
        public int ExpenseTotal
        {
            get
            {             
                return _expenseTotal;
            }
            private set
            {
                if(_expenseTotal != value)
                {
                    _expenseTotal = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpenseTotal"));
                }
            }
        }

        private int _budgetTotal;
        public int BudgetTotal
        {
            get
            {
                return _budgetTotal;
            }
            private set
            {
                if (_budgetTotal != value)
                {
                    _budgetTotal = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BudgetTotal"));
                }
            }
        }

        private int _budgetRemainingTotal;
        public int BudgetRemainingTotal
        {
            get
            {
                return _budgetRemainingTotal;
            }
            private set
            {
                if (_budgetRemainingTotal != value)
                {
                    _budgetRemainingTotal = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BudgetRemainingTotal"));
                }
            }
        }

        private int _grandTotal;
        public int GrandTotal
        {
            get
            {
                return _grandTotal;
            }
            private set
            {
                if (_grandTotal != value)
                {
                    _grandTotal = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GrandTotal"));
                }
            }
        }

        private int _spentBudgetGrandTotal;
        public int SpentBudgetGrandTotal
        {
            get
            {
                return _spentBudgetGrandTotal;
            }
            private set
            {
                if (_spentBudgetGrandTotal != value)
                {
                    _spentBudgetGrandTotal = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpentBudgetGrandTotal"));
                }
            }
        }

        private int _amountSpent;
        public int AmountSpent
        {
            get
            {
                return _amountSpent;
            }
            private set
            {
                if (_amountSpent != value)
                {
                    _amountSpent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AmountSpent"));
                }
            }
        }

        public ObservableCollection<Income> IncomeList{ get; set; }
        public ObservableCollection<Expense> ExpenseList { get; set; }
        public ObservableCollection<Budget> BudgetList { get; set; }

        public ObservableCollection<Expense> TodaysExpenses { get; set; }

        private List<int> IncomeListOrder { get; set; }
        private List<int> ExpenseListOrder { get; set; }
        private List<int> BudgetListOrder { get; set; }

        public ObservableCollection<ExpenseTypeDataPoint> ExpenseTypeDataPoints { get; set; }
        public ObservableCollection<ReceiptTypeDataPoint> ReceiptTypeDataPoints { get; set; }
        public ObservableCollection<ReceiptPayeeDataPoint> ReceiptPayeeDataPoints { get; set; }
        public List<Brush> ReceiptTypeDataColors { get; set; }
        public ObservableCollection<IncomeTypeDataPoint> IncomeTypeDataPoints { get; set; }
        

        private DateTime _currentDate;
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                if (_currentDate != value)
                {
                    _currentDate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentDate"));
                }
            }
        }

        private Receipt _currentReceipt;
        public Receipt CurrentReceipt
        {
            get { return _currentReceipt; }
            set
            {
                //if(value != null)
                //{
                    if (_currentReceipt != value)
                    {
                        _currentReceipt = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentReceipt"));
                    }
                //}
            }
        }

        private Expense _currentExpense;
        public Expense CurrentExpense
        {
            get { return _currentExpense; }
            set
            {
                //if(value != null)
                //{
                if (_currentExpense != value)
                {
                    _currentExpense = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentExpense"));
                }
                //}
            }
        }

        private Budget _currentBudget;
        public Budget CurrentBudget
        {
            get { return _currentBudget; }
            set
            {
                if(_currentBudget != value)
                {
                    _currentBudget = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentBudget"));
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

        private bool _isImageLoading;
        public bool IsImageLoading
        {
            get { return _isImageLoading; }
            private set
            {
                _isImageLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsImageLoading"));
            }
        }

        public FinancialMonth(int month, int year)
        {

            _month = month;
            _year = year;

            _monthId = Year.ToString() + Month.ToString();

            CurrentDate = new DateTime(Year, Month, 1);

            _currentBudget = null;
            _currentReceipt = null;
            _currentExpense = null;

            IncomeList = new ObservableCollection<Income>();
            ExpenseList = new ObservableCollection<Expense>();
            BudgetList = new ObservableCollection<Budget>();

            TodaysExpenses = new ObservableCollection<Expense>();

            // Combined with LoadInitialListOrders
            //InitializeListData(month, year);

            // For Initialization and keeping track of the state of the list orders as they change
            LoadInitialListOrders();
            IncomeList.CollectionChanged += ListView_CollectionChanged;
            ExpenseList.CollectionChanged += ListView_CollectionChanged;
            BudgetList.CollectionChanged += ListView_CollectionChanged;

            // Get Expenses that are due today

            GetTodaysExpenses();

            // Initialize Structures For Data Visualization
            ExpenseTypeDataPoints = new ObservableCollection<ExpenseTypeDataPoint>();
            ReceiptTypeDataPoints = new ObservableCollection<ReceiptTypeDataPoint>();
            ReceiptPayeeDataPoints = new ObservableCollection<ReceiptPayeeDataPoint>();
            ReceiptTypeDataColors = new List<Brush>();
            IncomeTypeDataPoints = new ObservableCollection<IncomeTypeDataPoint>();
            
            
            // Do the Math
            // Note: Data Points are set in the following methods as well as totals
            RecalculateIncomeTotal();
            RecalculateExpenseTotal();
            RecalculateBudgetTotal();
            RecalculateBudgetRemainingTotal();
            RecalculateGrandTotal();
         
        }

        //
        // LIST ORDER METHODS
        //

        private void LoadInitialListOrders()
        {
            IncomeListOrder = SettingsHelper.ReadListOrderSetting(SettingsHelper.IncomeListOrderToken + _monthId);
            ExpenseListOrder = SettingsHelper.ReadListOrderSetting(SettingsHelper.ExpenseListOrderToken + _monthId);
            BudgetListOrder = SettingsHelper.ReadListOrderSetting( SettingsHelper.BudgetListOrderToken + _monthId);

            if (IncomeListOrder != null && IncomeListOrder.Count > 0)
            {
                IncomeList.Clear();

                for (int i = 0; i < IncomeListOrder.Count; i++)
                {
                    // BOTTLENECK
                    // Changed to non-recursive
                    IncomeList.Add(DBHelper.GetIncomeById(IncomeListOrder[i], false));
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncomeList"));
            }
            else
            {
                IncomeListOrder = new List<int>();

                ObservableCollection<Income> tempIncomeList = DBHelper.GetIncomesByMonth(Month, Year, false);

                IncomeList.Clear();
                foreach (Income i in tempIncomeList)
                {
                    IncomeList.Add(i);
                }
            }

            if (ExpenseListOrder != null && ExpenseListOrder.Count > 0)
            {
                ExpenseList.Clear();

                for (int i = 0; i < ExpenseListOrder.Count; i++)
                {
                    // BOTTLENECK
                    // Changed to non-recursive
                    ExpenseList.Add(DBHelper.GetExpenseById(ExpenseListOrder[i], false));
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpenseList"));
            }
            else
            {
                ExpenseListOrder = new List<int>();

                ObservableCollection<Expense> tempExpenseList = DBHelper.GetExpensesByMonth(Month, Year, false);

                ExpenseList.Clear();
                foreach (Expense e in tempExpenseList)
                {
                    ExpenseList.Add(e);
                }
            }

            if (BudgetListOrder != null && BudgetListOrder.Count > 0)
            {
                BudgetList.Clear();

                List<ReceiptTypeObject> usedTypes = new List<ReceiptTypeObject>();
                List<Payee> usedPayees = new List<Payee>();

                for (int i = 0; i < BudgetListOrder.Count; i++)
                {
                    // BOTTLENECK
                    // Changed to non-recursive
                    // TODO: Fix bc the Budgets need the list of Receipts (OK)
                    //       but also the Payee and Tpe and Budget of the receipts in the list (NOT GETTING LOADED bc non-recursive)
                    //BudgetList.Add(DBHelper.GetBudgetById(BudgetListOrder[i], false));

                    Budget b = DBHelper.GetBudgetById(BudgetListOrder[i], false);
                    if(b.Receipts != null && b.Receipts.Count > 0)
                    {
                        foreach (Receipt r in b.Receipts)
                        {
                            if(usedTypes.Any(rto => rto.Id == r.TypeId))
                            {
                                r.Type = usedTypes.Where(rto => rto.Id == r.TypeId).First();
                            }
                            else
                            {
                                ReceiptTypeObject newRto = DBHelper.GetReceiptTypeById(r.TypeId, false);
                                usedTypes.Add(newRto);
                                r.Type = newRto;
                            }
                            
                            if(usedPayees.Any(p => p.Id == r.PayeeId))
                            {
                                r.Payee = usedPayees.Where(p => p.Id == r.PayeeId).First();
                            }
                            else
                            {
                                Payee newPayee = DBHelper.GetPayeeById(r.PayeeId, false);
                                usedPayees.Add(newPayee);
                                r.Payee = newPayee;
                            }
                        }
                    }

                    BudgetList.Add(b);
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BudgetList"));
            }
            else
            {
                BudgetListOrder = new List<int>();

                ObservableCollection<Budget> tempBudgetList = DBHelper.GetBudgetsByMonth(Month, Year, false);

                List<ReceiptTypeObject> usedTypes = new List<ReceiptTypeObject>();
                List<Payee> usedPayees = new List<Payee>();

                BudgetList.Clear();
                foreach (Budget b in tempBudgetList)
                {
                    if (b.Receipts != null && b.Receipts.Count > 0)
                    {
                        foreach (Receipt r in b.Receipts)
                        {
                            if (usedTypes.Any(rto => rto.Id == r.TypeId))
                            {
                                r.Type = usedTypes.Where(rto => rto.Id == r.TypeId).First();
                            }
                            else
                            {
                                ReceiptTypeObject newRto = DBHelper.GetReceiptTypeById(r.TypeId, false);
                                usedTypes.Add(newRto);
                                r.Type = newRto;
                            }

                            if (usedPayees.Any(p => p.Id == r.PayeeId))
                            {
                                r.Payee = usedPayees.Where(p => p.Id == r.PayeeId).First();
                            }
                            else
                            {
                                Payee newPayee = DBHelper.GetPayeeById(r.PayeeId, false);
                                usedPayees.Add(newPayee);
                                r.Payee = newPayee;
                            }
                        }
                    }

                    BudgetList.Add(b);
                }
            }
        }

        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(sender is ObservableCollection<Income>)
            {
                RefreshIncomeListOrder();
            }
            else if(sender is ObservableCollection<Expense>)
            {
                RefreshExpenseListOrder();
            }
            else if(sender is ObservableCollection<Budget>)
            {
                RefreshBudgetListOrder();
            }
        }

        private void RefreshIncomeListOrder()
        {
            IncomeListOrder.Clear();

            if(IncomeList != null)
            {
                foreach(Income i in IncomeList)
                {
                    IncomeListOrder.Add(i.Id);
                }
            }

            SettingsHelper.SaveLocalObjectSetting(SettingsHelper.IncomeListOrderToken + _monthId, IncomeListOrder);
        }

        private static void AddToIncomeListOrder(Income income, string monthId)
        {
            List<int> newListOrder = SettingsHelper.ReadListOrderSetting(SettingsHelper.IncomeListOrderToken + monthId);

            if (newListOrder == null)
            {
                newListOrder = new List<int>();
                newListOrder.Add(income.Id);
            }
            else
            {
                newListOrder.Add(income.Id);
            }

            SettingsHelper.SaveLocalObjectSetting(SettingsHelper.IncomeListOrderToken + monthId, newListOrder);
        }

        private void RefreshExpenseListOrder()
        {
            ExpenseListOrder.Clear();

            if (ExpenseList != null)
            {
                foreach (Expense e in ExpenseList)
                {
                    ExpenseListOrder.Add(e.Id);
                }
            }

            SettingsHelper.SaveLocalObjectSetting(SettingsHelper.ExpenseListOrderToken + _monthId, ExpenseListOrder);
        }

        private static void AddToExpenseListOrder(Expense expense, string monthId)
        {
            List<int> newListOrder = SettingsHelper.ReadListOrderSetting(SettingsHelper.ExpenseListOrderToken + monthId);

            if (newListOrder == null)
            {
                newListOrder = new List<int>();
                newListOrder.Add(expense.Id);
            }
            else
            {
                newListOrder.Add(expense.Id);
            }

            SettingsHelper.SaveLocalObjectSetting(SettingsHelper.ExpenseListOrderToken + monthId, newListOrder);
        }

        private void RefreshBudgetListOrder()
        {
            BudgetListOrder.Clear();

            if (BudgetList != null)
            {
                foreach (Budget b in BudgetList)
                {
                    BudgetListOrder.Add(b.Id);
                }
            }

            SettingsHelper.SaveLocalObjectSetting(SettingsHelper.BudgetListOrderToken + _monthId, BudgetListOrder);
        }

        private static void AddToBudgetListOrder(Budget budget, string monthId)
        {
            List<int> newListOrder = SettingsHelper.ReadListOrderSetting(SettingsHelper.BudgetListOrderToken + monthId);

            if (newListOrder == null)
            {
                newListOrder = new List<int>();
                newListOrder.Add(budget.Id);
            }
            else
            {
                newListOrder.Add(budget.Id);
            }

            SettingsHelper.SaveLocalObjectSetting(SettingsHelper.BudgetListOrderToken + monthId, newListOrder);
        }

        public List<IncomeSortOptions> GetIncomeSortOptions()
        {
            return SortOptions.GetIncomeSortOptionsList();
        }

        public List<ExpenseSortOptions> GetExpenseSortOptions()
        {
            return SortOptions.GetExpenseSortOptionsList();
        }
        
        public List<BudgetSortOptions> GetBudgetSortOptions()
        {
            return SortOptions.GetBudgetSortOptionsList();
        }

        public void SortIncomeList(IncomeSortOptions sortOption)
        {
            ObservableCollection<Income> temp = new ObservableCollection<Income>();

            switch (sortOption)
            {
                case IncomeSortOptions.ByLabel:
                    temp = new ObservableCollection<Income>(IncomeList.OrderBy(key => key.Label));
                    goto default;
                case IncomeSortOptions.ByAmount:
                    temp = new ObservableCollection<Income>(IncomeList.OrderByDescending(key => key.Amount));
                    goto default;
                case IncomeSortOptions.ByCategory:
                    temp = new ObservableCollection<Income>(IncomeList.OrderBy(key => key.Type.Label));
                    goto default;
                case IncomeSortOptions.ByAddedDate:
                    temp = new ObservableCollection<Income>(IncomeList.OrderBy(key => key.Id));
                    goto default;
                default:
                    IncomeList.Clear();
                    foreach (Income i in temp)
                    {
                        IncomeList.Add(i);
                    }
                    break;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncomeList"));
        }

        public void SortExpenseList(ExpenseSortOptions sortOption)
        {
            ObservableCollection<Expense> temp = new ObservableCollection<Expense>();

            switch(sortOption)
            {
                case ExpenseSortOptions.ByLabel:
                    temp = new ObservableCollection<Expense>(ExpenseList.OrderBy(key => key.Label));
                    goto default;
                case ExpenseSortOptions.ByAmount:
                    temp = new ObservableCollection<Expense>(ExpenseList.OrderByDescending(key => key.Amount));
                    goto default;
                case ExpenseSortOptions.ByPayee:
                    temp = new ObservableCollection<Expense>(ExpenseList.OrderBy(key => key.Payee.Label));
                    goto default;
                case ExpenseSortOptions.ByCategory:
                    temp = new ObservableCollection<Expense>(ExpenseList.OrderBy(key => key.Type.Label));
                    goto default;
                case ExpenseSortOptions.ByDueDate:
                    temp = new ObservableCollection<Expense>(ExpenseList.OrderByDescending(key => key.DueDate));
                    goto default;
                case ExpenseSortOptions.ByPaidStatus:
                    temp = new ObservableCollection<Expense>(ExpenseList.OrderBy(key => key.IsPaid));
                    goto default;
                case ExpenseSortOptions.ByDateAdded:
                    temp = new ObservableCollection<Expense>(ExpenseList.OrderBy(key => key.Id));
                    goto default;
                default:
                    ExpenseList.Clear();
                    foreach (Expense e in temp)
                    {
                        ExpenseList.Add(e);
                    }
                    break;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpenseList"));
        }

        public void SortBudgetList(BudgetSortOptions sortOption)
        {
            ObservableCollection<Budget> temp = new ObservableCollection<Budget>();
            int currentBudgetId;

            if(CurrentBudget != null)
            {
                currentBudgetId = CurrentBudget.Id;
            }
            else
            {
                currentBudgetId = -1;
            }

            switch(sortOption)
            {
                case BudgetSortOptions.ByLabel:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderBy(key => key.Label));
                    goto default;
                case BudgetSortOptions.ByAmount:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderByDescending(key => key.Amount));
                    goto default;
                case BudgetSortOptions.ByRemainingAmount:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderByDescending(key => key.RemainingAmount));
                    goto default;
                case BudgetSortOptions.ByCategory:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderBy(key => key.Type.Label));
                    goto default;
                case BudgetSortOptions.ByClosedStatus:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderBy(key => key.IsClosed));
                    goto default;
                case BudgetSortOptions.ByDateAdded:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderBy(key => key.Id));
                    goto default;
                default:
                    BudgetList.Clear();
                    foreach (Budget b in temp)
                    {
                        BudgetList.Add(b);

                        if (currentBudgetId != -1 && b.Id == currentBudgetId)
                        {
                            CurrentBudget = b;
                        }
                    }
                    break;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BudgetList"));
        }

        // May introduce problems with receipts in the database...
        public void SortCurrentReceiptList(ReceiptSortOptions sortOption)
        {
            ObservableCollection<Receipt> temp = new ObservableCollection<Receipt>();

            switch (sortOption)
            {
                case ReceiptSortOptions.ByLabel:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderBy(key => key.Label));
                    goto default;
                case ReceiptSortOptions.ByAmount:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderByDescending(key => key.Amount));
                    goto default;
                case ReceiptSortOptions.ByPayee:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderBy(key => key.Payee.Label));
                    goto default;
                case ReceiptSortOptions.ByCategory:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderBy(key => key.Type.Label));
                    goto default;
                case ReceiptSortOptions.ByTransactionDate:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderBy(key => key.TransactionDate));
                    goto default;
                case ReceiptSortOptions.ByDateAdded:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderBy(key => key.CreationDate));
                    goto default;
                default:
                    CurrentBudget.Receipts.Clear();
                    foreach (Receipt r in temp)
                    {
                        CurrentBudget.Receipts.Add(r);
                    }
                    break;
            }
        }

        //
        // METHODS FOR ADDING AND UPDATING DATA
        //

        /// <summary>
            /// Add new Income to the Current Month and update the Amount Totals
            /// </summary>
            /// <param name="Label"></param>
            /// <param name="Amount"></param>
            /// <param name="Type"></param>

        public void AddIncome(string Label, int Amount, string Type)
        {
            IncomeTypeObject TypeObj;

            if (ItemTypes.IncomeTypes.Any(t => t.Label == Type)) // This will pretty much always be the case for new item since "Undefined" is set
            {
                TypeObj = ItemTypes.IncomeTypes.Where(t => t.Label == Type).ToList().First();
            }
            else
            {
                TypeObj = new IncomeTypeObject(Type);
                ItemTypes.AddIncomeType(TypeObj);
            }

            Income income = new Income(Label, Amount, TypeObj, Month, Year);

            // The order of te next two lines is important
            DBHelper.Insert(income);
            IncomeList.Insert(0, income);

            //TypeObj.Refresh();
            if (TypeObj != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add income to ItemTypes.IncomeTypes for the new category
                int newTypeIndex = ItemTypes.IncomeTypes.IndexOf(TypeObj);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.IncomeTypes[newTypeIndex].Incomes.Add(income);
                }
            }

            RecalculateIncomeTotal();
            RecalculateGrandTotal();
        }

        public void AddIncome(Income income)
        {
            IncomeList.Insert(0, income);
            DBHelper.Insert(income);

            RecalculateIncomeTotal();
            RecalculateGrandTotal();
        }

        /// <summary>
        /// Add new Expense to the Current Month and update the Amount Totals
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="Amount"></param>
        /// <param name="Type"></param>
        /// <param name="Payee"></param>

        public void AddExpense(string Label, int Amount, string Type, string Payee)
        {
            ExpenseTypeObject TypeObj;
            Payee PayeeObj;

            if (ItemTypes.ExpenseTypes.Any(t => t.Label == Type)) // This will pretty much always be the case for new item since "Undefined" is set
            {
                TypeObj = ItemTypes.ExpenseTypes.Where(t => t.Label == Type).ToList().First();
            }
            else
            {
                TypeObj = new ExpenseTypeObject(Type);
                ItemTypes.AddExpenseType(TypeObj);
            }
            if(ItemTypes.Payees.Any(p => p.Label == Payee))
            {
                PayeeObj = ItemTypes.Payees.Where(p => p.Label == Payee).ToList().First();
            }
            else
            {
                PayeeObj = new Payee(Payee);
                ItemTypes.AddPayee(PayeeObj);
            }

            Expense expense = new Expense(Label, Amount, TypeObj, PayeeObj, Month, Year);

            // The order of the next two lines is important
            DBHelper.Insert(expense);
            ExpenseList.Insert(0, expense);

            //TypeObj.AddExpense(expense);
            //PayeeObj.AddObject(expense);
            if (TypeObj != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.ExpenseTypes for the new category
                int newTypeIndex = ItemTypes.ExpenseTypes.IndexOf(TypeObj);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.ExpenseTypes[newTypeIndex].Expenses.Add(expense);
                }
            }
            if (PayeeObj != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.Payees for the new payee
                int newPayeeIndex = ItemTypes.Payees.IndexOf(PayeeObj);
                if (newPayeeIndex >= 0)
                {
                    ItemTypes.Payees[newPayeeIndex].Expenses.Add(expense);
                }
            }

            RecalculateExpenseTotal();
            RecalculateGrandTotal();
        }

        public void AddExpense(Expense expense)
        {
            ExpenseList.Insert(0, expense);
            DBHelper.Insert(expense);

            RecalculateExpenseTotal();
            RecalculateGrandTotal();
        }

        /// <summary>
        /// Add new Budget to the Current Month and update the Amount Totals
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="Amount"></param>
        /// <param name="Type"></param>

        public void AddBudget(string Label, int Amount, string Type)
        {
            BudgetTypeObject TypeObj;

            if (ItemTypes.BudgetTypes.Any(t => t.Label == Type)) // This will pretty much always be the case for new item since "Undefined" is se
            {
                TypeObj = ItemTypes.BudgetTypes.Where(t => t.Label == Type).ToList().First();
            }
            else
            {
                TypeObj = new BudgetTypeObject(Type);
                ItemTypes.AddBudgetType(TypeObj);
            }

            Budget budget = new Budget(Label, Amount, TypeObj, Month, Year);

            // The order of the next two lines is important
            DBHelper.Insert(budget);
            BudgetList.Insert(0, budget);

            // BOTTLENECK
            //TypeObj.Refresh();

            //TypeObj.AddBudget(budget);
            if (TypeObj != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.BudgetTypes for the new category
                int newTypeIndex = ItemTypes.BudgetTypes.IndexOf(TypeObj);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.BudgetTypes[newTypeIndex].Budgets.Add(budget);
                }
            }

            RecalculateBudgetTotal();
            RecalculateBudgetRemainingTotal();
            RecalculateGrandTotal();
        }

        public void AddBudget(Budget budget)
        {
            BudgetList.Insert(0, budget);
            DBHelper.Insert(budget);

            RecalculateBudgetTotal();
            RecalculateGrandTotal();
        }
        
        /// <summary>
        /// Add new Receipt to the Current Budget of the Current Month
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="Amount"></param>
        /// <param name="TransactionDate"></param>
        /// <param name="Budget"></param>
        /// <param name="Type"></param>
        /// <param name="Payee"></param>

        public void AddReceipt(string Label, int Amount, DateTime TransactionDate, Budget Budget, string Type, string Payee)
        {
            ReceiptTypeObject TypeObj;
            Payee PayeeObj;

            if (ItemTypes.ReceiptTypes.Any(t => t.Label == Type)) // This will pretty much always be the case for new item since "Undefined" is set
            {
                TypeObj = ItemTypes.ReceiptTypes.Where(t => t.Label == Type).ToList().First();
            }
            else
            {
                TypeObj = new ReceiptTypeObject(Type);
                ItemTypes.AddReceiptType(TypeObj);
            }
            if (ItemTypes.Payees.Any(p => p.Label == Payee)) // This will pretty much always be the case for new item since "Undefined" is set
            {
                PayeeObj = ItemTypes.Payees.Where(p => p.Label == Payee).ToList().First();
            }
            else
            {
                PayeeObj = new Payee(Payee);
                ItemTypes.AddPayee(PayeeObj);
            }

            if(CurrentBudget != null)
            {
                var newReceipt = CurrentBudget.AddReceipt(Label, Amount, TransactionDate, TypeObj, PayeeObj);
            }

            RecalculateBudgetTotal();
            RecalculateBudgetRemainingTotal();
            RecalculateGrandTotal();
        }

        public async void AddReceiptFromCamera()
        {
            string prevImagePath = CurrentReceipt.ImageUrl;

            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;

            StorageFile photo = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Guid.NewGuid().ToString() + ".jpg");

            photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                // User cancelled photo capture, revert the image path
                CurrentReceipt.ImageUrl = prevImagePath;
                return;
            }
            else
            {
                IsImageLoading = true;

                StorageFile oldImageFile = null;

                // If Receipt has an image, grab its file
                if (CurrentReceipt.ImageUrl != null)
                {
                    oldImageFile = await StorageFile.GetFileFromPathAsync(CurrentReceipt.ImageUrl);
                }

                // If Receipt currently has an image file a new file with a unique name will be created
                photo = await GetFileForReceiptImageAsync(photo);

                // Delete the previous receipt image file, if there was one
                if (oldImageFile != null)
                {
                    await oldImageFile.DeleteAsync();
                }

                CurrentReceipt.OcrString = await GetOcrFromImageFile(photo);
                CurrentReceipt.ImageUrl = photo.Path;

                UpdateItem(CurrentReceipt);

                IsImageLoading = false;
            }
        }

        public async void AddReceiptFromFile()
        {
            FileOpenPicker filePicker = new FileOpenPicker();

            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".gif");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".pdf");

            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.Desktop;

            StorageFile file = await filePicker.PickSingleFileAsync();
            StorageFile oldImageFile = null;

            if(file != null)
            {
                IsImageLoading = true;

                switch(file.FileType)
                {
                    case ".pdf":
                        await CreateImageUriFromPdfAsync(file);
                        break;
                    default:
                        // If Receipt has an image, grab its file
                        if(CurrentReceipt.ImageUrl != null)
                        {
                            oldImageFile = await StorageFile.GetFileFromPathAsync(CurrentReceipt.ImageUrl);
                        }

                        // If Receipt currently has an image file a new file with a unique name will be created
                        file = await GetFileForReceiptImageAsync(file);

                        // Delete the previous receipt image file, if there was one
                        if(oldImageFile != null)
                        {
                            await oldImageFile.DeleteAsync();
                        }

                        CurrentReceipt.OcrString = await GetOcrFromImageFile(file);
                        CurrentReceipt.ImageUrl = file.Path;

                        UpdateItem(CurrentReceipt);
                        break;
                }

                IsImageLoading = false;
            }
        }

        public static void CopyItemToNextMonth(object item)
        {
            if(item is Income i)
            {
                DateTime currentDate = new DateTime(i.Year, i.Month, 1);
                DateTime copyDate = currentDate.AddMonths(1);

                string nextMonthId = copyDate.Year.ToString() + copyDate.Month.ToString();

                IncomeTypeObject type = i.Type;

                Income newIncome = new Income(i.Label, i.Amount, type, copyDate.Month, copyDate.Year);

                DBHelper.Insert(newIncome);

                AddToIncomeListOrder(newIncome, nextMonthId);

                if (type != null)
                {
                    // Items are no longer recursively pulled from the database, therefore
                    // must manually add income to ItemTypes.IncomeTypes for the new category
                    int newTypeIndex = ItemTypes.IncomeTypes.IndexOf(type);
                    if (newTypeIndex >= 0)
                    {
                        ItemTypes.IncomeTypes[newTypeIndex].Incomes.Add(newIncome);
                    }
                }
            }
            else if(item is Expense e)
            {
                DateTime currentDate = new DateTime(e.Year, e.Month, 1);
                DateTime copyDate = currentDate.AddMonths(1);

                string nextMonthId = copyDate.Year.ToString() + copyDate.Month.ToString();

                ExpenseTypeObject type = e.Type;
                Payee payee = e.Payee;

                Expense newExpense = new Expense(e.Label, e.Amount, type, payee, copyDate.Month, copyDate.Year);

                newExpense.DueDate = e.DueDate.AddMonths(1);

                DBHelper.Insert(newExpense);

                AddToExpenseListOrder(newExpense, nextMonthId);

                if (type != null)
                {
                    // Items are no longer recursively pulled from the database, therefore
                    // must manually add expense to ItemTypes.ExpenseTypes for the new category
                    int newTypeIndex = ItemTypes.ExpenseTypes.IndexOf(type);
                    if (newTypeIndex >= 0)
                    {
                        ItemTypes.ExpenseTypes[newTypeIndex].Expenses.Add(newExpense);
                    }
                }
                if (payee != null)
                {
                    // Items are no longer recursively pulled from the database, therefore
                    // must manually add expense to ItemTypes.Payees for the new payee
                    int newPayeeIndex = ItemTypes.Payees.IndexOf(payee);
                    if (newPayeeIndex >= 0)
                    {
                        ItemTypes.Payees[newPayeeIndex].Expenses.Add(newExpense);
                    }
                }
            }
            else if(item is Budget b)
            {
                DateTime currentDate = new DateTime(b.Year, b.Month, 1);
                DateTime copyDate = currentDate.AddMonths(1);

                string nextMonthId = copyDate.Year.ToString() + copyDate.Month.ToString();

                BudgetTypeObject type = b.Type;

                Budget newBudget = new Budget(b.Label, b.Amount, type, copyDate.Month, copyDate.Year);

                DBHelper.Insert(newBudget);

                AddToBudgetListOrder(newBudget, nextMonthId);

                if (type != null)
                {
                    // Items are no longer recursively pulled from the database, therefore
                    // must manually add expense to ItemTypes.BudgetTypes for the new category
                    int newTypeIndex = ItemTypes.BudgetTypes.IndexOf(type);
                    if (newTypeIndex >= 0)
                    {
                        ItemTypes.BudgetTypes[newTypeIndex].Budgets.Add(newBudget);
                    }
                }
            }
        }

        public void CopyItemToThisMonth(object item)
        {
            if (item is Income i)
            {
                AddIncome(i.Label, i.Amount, i.Type.Label);
            }
            else if (item is Expense e)
            {
                AddExpense(e.Label, e.Amount, e.Type.Label, e.Payee.Label);
            }
            else if (item is Budget b)
            {
                AddBudget(b.Label, b.Amount, b.Type.Label);
            }
            else if (item is Receipt r)
            {
                AddReceipt(r.Label, r.Amount, DateTime.Now, r.Budget, r.Type.Label, r.Payee.Label);
            }
        }

        public void CopyTotalToNextMonth()
        {
            DateTime newDate = CurrentDate.AddMonths(1);

            string nextMonthId = newDate.Year.ToString() + newDate.Month.ToString();

            IncomeTypeObject TypeObj = ItemTypes.IncomeTypes.Where(t => t.Label == "Rollover").ToList().First();

            MonthYearFormatConverter converter = new MonthYearFormatConverter();
            string label = converter.Convert(CurrentDate, null, null, null) + " Rollover";

            Income inc = new Income(label, GrandTotal, TypeObj, newDate.Month, newDate.Year);

            DBHelper.Insert(inc);

            AddToIncomeListOrder(inc, nextMonthId);

            if (TypeObj != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add income to ItemTypes.IncomeTypes for the new category
                int newTypeIndex = ItemTypes.IncomeTypes.IndexOf(TypeObj);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.IncomeTypes[newTypeIndex].Incomes.Add(inc);
                }
            }
        }

        public void GetTodaysExpenses()
        {
            TodaysExpenses.Clear();
            IEnumerable<Expense> temp = ExpenseList.Where(r => r.DueDate.Day == DateTime.Now.Day &&
                                                                r.DueDate.Month == DateTime.Now.Month &&
                                                                r.DueDate.Year == DateTime.Now.Year &&
                                                                r.IsPaid == false);

            foreach (Expense expense in temp)
            {
                TodaysExpenses.Add(expense);
            }
        }

        public void UpdateIncomeType(Income income, IncomeTypeObject newType)
        {
            IncomeTypeObject prevType = income.Type;
            
            income.Type = newType;
            //newType.AddIncome(income);

            if(newType != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.IncomeTypes for the new category
                int newTypeIndex = ItemTypes.IncomeTypes.IndexOf(newType);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.IncomeTypes[newTypeIndex].Incomes.Add(income);
                }
            }

            if (prevType != null)
            {
                //prevType.RemoveIncome(income);

                // Must manually remove expense from ItemTypes.IncomeTypes for the old category
                int typeIndex = ItemTypes.IncomeTypes.IndexOf(prevType);
                if(typeIndex >= 0)
                {
                    ItemTypes.IncomeTypes[typeIndex].Incomes.Remove(income);
                }
            }

            UpdateItem(income);
        }

        public void UpdateExpenseType(Expense expense, ExpenseTypeObject newType)
        {
            ExpenseTypeObject prevType = expense.Type;
            
            expense.Type = newType;
            //newType.AddExpense(expense);

            if (newType != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.ExpenseTypes for the new category
                int newTypeIndex = ItemTypes.ExpenseTypes.IndexOf(newType);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.ExpenseTypes[newTypeIndex].Expenses.Add(expense);
                }
            }

            if (prevType != null)
            {
                //prevType.RemoveExpense(expense);

                // Must manually remove expense from ItemTypes.ExpenseTypes for the old category
                int typeIndex = ItemTypes.ExpenseTypes.IndexOf(prevType);
                if(typeIndex >= 0)
                {
                    ItemTypes.ExpenseTypes[typeIndex].Expenses.Remove(expense);
                }
            }

            UpdateItem(expense);
        }

        public void UpdateExpensePayee(Expense expense, Payee newPayee)
        {
            Payee prevPayee = expense.Payee;
            
            expense.Payee = newPayee;
            //newPayee.AddObject(expense);

            if (newPayee != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.Payees for the new payee
                int newPayeeIndex = ItemTypes.Payees.IndexOf(newPayee);
                if (newPayeeIndex >= 0)
                {
                    ItemTypes.Payees[newPayeeIndex].Expenses.Add(expense);
                }
            }

            if (prevPayee != null)
            {
                prevPayee.RemoveObject(expense);
                // Must manually remove expense from ItemTypes.Payees for the old payee
                int payeeIndex = ItemTypes.Payees.IndexOf(prevPayee);
                if(payeeIndex >= 0)
                {
                    ItemTypes.Payees[payeeIndex].Expenses.Remove(expense);
                }
            }

            UpdateItem(expense);
        }

        public void UpdateBudgetType(Budget budget, BudgetTypeObject newType)
        {
            BudgetTypeObject prevType = budget.Type;
            
            budget.Type = newType;

            if (newType != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.BudgetTypes for the new category
                int newTypeIndex = ItemTypes.BudgetTypes.IndexOf(newType);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.BudgetTypes[newTypeIndex].Budgets.Add(budget);
                }
            }

            if (prevType != null)
            {
                //prevType.RemoveBudget(budget);

                // Must manually remove receipt from ItemTypes.BudgetTypes for the old category
                int typeIndex = ItemTypes.BudgetTypes.IndexOf(prevType);
                if(typeIndex >= 0)
                {
                    ItemTypes.BudgetTypes[typeIndex].Budgets.Remove(budget);
                }
            }

            UpdateItem(budget);
        }

        public void UpdateReceiptType(Receipt receipt, ReceiptTypeObject newType)
        {
            ReceiptTypeObject prevType = receipt.Type;
            
            receipt.Type = newType;

            if (newType != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.ReceiptTypes for the new category
                int newTypeIndex = ItemTypes.ReceiptTypes.IndexOf(newType);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.ReceiptTypes[newTypeIndex].Receipts.Add(receipt);
                }
            }

            if (prevType != null)
            {
                //prevType.RemoveReceipt(receipt);

                // Must manually remove receipt from ItemTypes.ReceiptTypes for the old category
                int typeIndex = ItemTypes.ReceiptTypes.IndexOf(prevType);
                if(typeIndex >= 0)
                {
                    ItemTypes.ReceiptTypes[typeIndex].Receipts.Remove(receipt);
                }  
            }

            UpdateItem(receipt);
        }

        public void UpdateReceiptPayee(Receipt receipt, Payee newPayee)
        {
            Payee prevPayee = receipt.Payee;
            
            receipt.Payee = newPayee;

            if (newPayee != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.Payees for the new payee
                int newPayeeIndex = ItemTypes.Payees.IndexOf(newPayee);
                if (newPayeeIndex >= 0)
                {
                    ItemTypes.Payees[newPayeeIndex].Receipts.Add(receipt);
                }
            }

            if (prevPayee != null)
            {
                //prevPayee.RemoveObject(receipt);

                // Must manually remove receipt from ItemTypes.Payees for the old payee
                int payeeIndex = ItemTypes.Payees.IndexOf(prevPayee);
                if(payeeIndex >= 0)
                {
                    ItemTypes.Payees[payeeIndex].Receipts.Remove(receipt);
                }
            }

            UpdateItem(receipt);
        }

        // Is this even used? Future Use?
        //public void UpdateReceiptBudget(Receipt receipt, Budget newBudget)
        //{
        //    Budget prevBudget = receipt.Budget;

        //    receipt.Budget = newBudget;

        //    UpdateItem(receipt);

        //    newBudget.Refresh();
        //    if (prevBudget != null)
        //    {
        //        prevBudget.Refresh();
        //    }
        //}

        //
        // DATABASE MANIPULATION METHODS
        //

        public void UpdateItem(Object o)
        {
            if (o is Income)
            {
                Income income = o as Income;
                IncomeTypeObject typeObj = income.Type;

                DBHelper.Update(income);

                //ItemTypes entry must be manually updated as the objects loaded on app startup from the IncomeList are
                //different from those in ItemTypes - needed for chart updates
                int typeIndex = ItemTypes.IncomeTypes.IndexOf(typeObj);
                if (typeIndex >= 0)
                {
                    Income i = ItemTypes.IncomeTypes[typeIndex].Incomes.Where(x => x.Id == income.Id).First();

                    i.Amount = income.Amount;
                }

                RecalculateIncomeTotal();
            }
            if (o is Expense)
            {
                Expense expense = o as Expense;
                ExpenseTypeObject typeObj = expense.Type;

                DBHelper.Update(expense);

                //ItemTypes entry must be manually updated as the objects loaded on app startup from the ExpenseList are
                //different from those in ItemTypes - needed for chart updates
                int typeIndex = ItemTypes.ExpenseTypes.IndexOf(typeObj);
                if (typeIndex >= 0)
                {
                    Expense e = ItemTypes.ExpenseTypes[typeIndex].Expenses.Where(x => x.Id == expense.Id).First();

                    e.Amount = expense.Amount;
                }

                RecalculateExpenseTotal();
            }
            if (o is Budget)
            {
                Budget budget = o as Budget;

                DBHelper.Update(budget);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BudgetList"));

                RecalculateBudgetTotal();
                RecalculateBudgetRemainingTotal();
            }
            if (o is Receipt receipt)
            {
                CurrentBudget.UpdateReceipt(receipt);

                RecalculateBudgetTotal();
                RecalculateBudgetRemainingTotal();
            }

            RecalculateGrandTotal();
        }

        public void DeleteItem(Object o)
        {
            if (o is Income)
            {
                Income income = o as Income;
                IncomeTypeObject prevType = income.Type;

                IncomeList.Remove(income);
                DBHelper.Delete(income);

                if(prevType != null)
                {
                    int typeIndex = ItemTypes.IncomeTypes.IndexOf(prevType);
                    if (typeIndex >= 0)
                    {
                        ItemTypes.IncomeTypes[typeIndex].Incomes.Remove(income);
                    }
                }

                // BOTTLENECK
                //prevType.Refresh();

                //prevType.RemoveIncome(income);

                RecalculateIncomeTotal();
            }
            else if (o is Expense)
            {
                Expense expense = o as Expense;
                ExpenseTypeObject prevType = expense.Type;
                Payee prevPayee = expense.Payee;

                ExpenseList.Remove(expense);
                DBHelper.Delete(expense);

                if(prevType != null)
                {
                    int typeIndex = ItemTypes.ExpenseTypes.IndexOf(prevType);
                    if (typeIndex >= 0)
                    {
                        ItemTypes.ExpenseTypes[typeIndex].Expenses.Remove(expense);
                    }
                }

                if(prevPayee != null)
                {
                    int payeeIndex = ItemTypes.Payees.IndexOf(prevPayee);
                    if (payeeIndex >= 0)
                    {
                        ItemTypes.Payees[payeeIndex].Expenses.Remove(expense);
                    }
                }

                // BOTTLENECK
                //prevType.Refresh();
                //prevPayee.Refresh();

                //prevType.RemoveExpense(expense);
                //prevPayee.RemoveObject(expense);

                GetTodaysExpenses();
                RecalculateExpenseTotal();
            }
            else if (o is Budget)
            {
                Budget budget = o as Budget;
                BudgetTypeObject prevType = budget.Type;


                // BOTTLENECK
                foreach(Receipt rec in budget.Receipts)
                {
                    rec.DisableNotifications = true;

                    // Zero out the BudgetId for this budget's receipts but don't delete them, for archiving purposes
                    rec.BudgetId = 0;
                    rec.Budget = null;
                    rec.BudgetMonth = 0;
                    rec.BudgetYear = 0;

                    ReceiptTypeObject recType = rec.Type;
                    Payee recPayee = rec.Payee;

                    rec.DisableNotifications = false;

                    DBHelper.Update(rec);

                    if (recType != null)
                    {
                        //prevType.RemoveReceipt(receipt);

                        // Must manually remove receipt from ItemTypes.ReceiptTypes for the old category
                        int typeIndex = ItemTypes.ReceiptTypes.IndexOf(recType);
                        if (typeIndex >= 0)
                        {
                            ItemTypes.ReceiptTypes[typeIndex].Receipts.Remove(rec);
                        }
                    }

                    if (recPayee != null)
                    {
                        //prevPayee.RemoveObject(receipt);

                        // Must manually remove receipt from ItemTypes.Payees for the old payee
                        int payeeIndex = ItemTypes.Payees.IndexOf(recPayee);
                        if (payeeIndex >= 0)
                        {
                            ItemTypes.Payees[payeeIndex].Receipts.Remove(rec);
                        }
                    }
                }

                if(budget.Receipts != null)
                {
                    budget.Receipts.Clear();
                }

                BudgetList.Remove(budget);
                DBHelper.Delete(budget);

                // BOTTLENECK
                //prevType.Refresh();

                if(prevType != null)
                {
                    int typeIndex = ItemTypes.BudgetTypes.IndexOf(prevType);
                    if (typeIndex >= 0)
                    {
                        ItemTypes.BudgetTypes[typeIndex].Budgets.Remove(budget);
                    }
                }

                RecalculateBudgetTotal();
                RecalculateBudgetRemainingTotal();
            }
            else if (o is Receipt)
            {
                Receipt receipt = o as Receipt;
                
                CurrentBudget.DeleteReceipt(receipt);

                RecalculateBudgetTotal();
                RecalculateBudgetRemainingTotal();
            }

            RecalculateGrandTotal();
        }

        public async Task<StorageFolder> GetReceiptImageFolder()
        {
            return await SettingsHelper.GetReceiptImageFolder();
        }

        //
        // PRIVATE WORKER METHODS
        //

                
        /// <summary>
        /// Depricated. Slow and dysfunctional.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        private void InitializeListData(int month, int year)
        {
            // BOTTLENECK
            //ObservableCollection<Income> tempIncomeList = DBHelper.GetIncomesByMonth(month, year);
            ObservableCollection<Income> tempIncomeList = DBHelper.GetIncomesByMonth(month, year, false);

            IncomeList.Clear();
            foreach (Income i in tempIncomeList)
            {
                IncomeList.Add(i);
            }

            // BOTTLENECK
            //ObservableCollection<Expense> tempExpenseList = DBHelper.GetExpensesByMonth(month, year);
            ObservableCollection<Expense> tempExpenseList = DBHelper.GetExpensesByMonth(month, year, false);

            ExpenseList.Clear();
            foreach (Expense e in tempExpenseList)
            {
                ExpenseList.Add(e);
            }

            // BOTTLENECK
            //ObservableCollection<Budget> tempBudgetList = DBHelper.GetBudgetsByMonth(month, year);
            ObservableCollection<Budget> tempBudgetList = DBHelper.GetBudgetsByMonth(month, year, false);

            BudgetList.Clear();
            foreach (Budget b in tempBudgetList)
            {
                BudgetList.Add(b);
            }
        }

        private void RecalculateIncomeTotal()
        {
            int incomeTotal = 0;

            if (IncomeList != null)
            {
                try
                {
                    if (IncomeList.Count > 0)
                    {
                        checked
                        {
                            foreach (Income inc in IncomeList)
                            {
                                incomeTotal += inc.Amount;
                            }
                        }
                    }

                    IncomeTotal = incomeTotal;
                }
                catch
                {
                    IncomeTotal = Int32.MaxValue;
                }
            }
            else
            {
                IncomeTotal = incomeTotal;
            }
            
            RecalculateIncomeTypeDataPoints();
        }

        private void RecalculateExpenseTotal()
        {
            int expenseTotal = 0;

            if (ExpenseList != null)
            {
                try
                {
                    if (ExpenseList.Count > 0)
                    {
                        checked
                        {
                            foreach (Expense exp in ExpenseList)
                            {
                                expenseTotal += exp.Amount;
                            }
                        }
                    }

                    ExpenseTotal = expenseTotal;
                }
                catch
                {
                    ExpenseTotal = Int32.MaxValue;
                }
            }
            else
            {
                ExpenseTotal = expenseTotal;
            }
            
            RecalculateExpenseTypeDataPoints();
        }

        private void RecalculateBudgetTotal()
        {
            int budgetTotal = 0;

            if (BudgetList != null)
            {
                try
                {
                    if (BudgetList.Count > 0)
                    {
                        checked
                        {
                            foreach (Budget bud in BudgetList)
                            {
                                budgetTotal += bud.Amount;
                            }
                        }
                    }

                    BudgetTotal = budgetTotal;
                }
                catch
                {
                    BudgetTotal = Int32.MaxValue;
                }
            }
            else
            {
                BudgetTotal = budgetTotal;
            }
        }

        private void RecalculateBudgetRemainingTotal()
        {
            int budgetRemainingTotal = 0;

            if (BudgetList != null)
            {
                if (BudgetList.Count > 0)
                {
                    foreach (Budget bud in BudgetList)
                    {
                        budgetRemainingTotal += bud.RemainingAmount;
                    }
                }
            }
            BudgetRemainingTotal = budgetRemainingTotal;
            RecalculateReceiptTypeDataPoints();
            RecalculateReceiptPayeeDataPoints();
        }

        private void RecalculateGrandTotal()
        {
            GrandTotal = IncomeTotal - ExpenseTotal - (BudgetTotal - BudgetRemainingTotal);
            SpentBudgetGrandTotal = IncomeTotal - ExpenseTotal - BudgetTotal;
            AmountSpent = (BudgetTotal - BudgetRemainingTotal);
        }

        private void RecalculateExpenseTypeDataPoints()
        {
            ExpenseTypeDataPoints.Clear();

            // BOTTLENECK
            //ItemTypes.RefreshExpenseTypes();

            foreach (ExpenseTypeObject eto in ItemTypes.ExpenseTypes)
            {
                if (eto.Expenses.Count > 0)
                {
                    List<Expense> query = eto.Expenses.Where(e => e.Month == Month && e.Year == Year).ToList();

                    if (query.Count > 0)
                    {
                        int sum = 0;
                        int count = 0;

                        foreach (Expense expense in query)
                        {
                            sum += expense.Amount;
                            count++;
                        }

                        ExpenseTypeDataPoints.Add(new ExpenseTypeDataPoint(eto.Label, sum, count));
                    }
                }
            }
        }

        private void RecalculateIncomeTypeDataPoints()
        {
            IncomeTypeDataPoints.Clear();

            // BOTTLENECK
            //ItemTypes.RefreshIncomeTypes();

            // BOTTLENECK
            // First reference of static class ItemTypes runs the constructor, which is slow
            foreach (IncomeTypeObject ito in ItemTypes.IncomeTypes)
            {
                if (ito.Incomes.Count > 0)
                {
                    List<Income> query = ito.Incomes.Where(i => i.Month == Month && i.Year == Year).ToList();

                    if (query.Count > 0)
                    {
                        int sum = 0;
                        int count = 0;

                        foreach (Income income in query)
                        {
                            sum += income.Amount;
                            count++;
                        }

                        IncomeTypeDataPoints.Add(new IncomeTypeDataPoint(ito.Label, sum, count));
                    }
                }
            }
        }

        private void RecalculateReceiptTypeDataPoints()
        {
            ReceiptTypeDataPoints.Clear();
            ReceiptTypeDataColors.Clear();

            //BOTTLENECK
            //ItemTypes.RefreshReceiptTypes();

            HexStringToColorConverter converter = new HexStringToColorConverter();

            foreach (ReceiptTypeObject rto in ItemTypes.ReceiptTypes)
            {
                if (rto.Receipts.Count > 0)
                {
                    List<Receipt> query = rto.Receipts.Where(r => r.BudgetMonth == Month && r.BudgetYear == Year).ToList();

                    if (query.Count > 0)
                    {
                        int sum = 0;
                        int count = 0;

                        foreach (Receipt receipt in query)
                        {
                            sum += receipt.Amount;
                            count++;
                        }

                        ReceiptTypeDataPoints.Add(new ReceiptTypeDataPoint(rto.Label, sum, count, rto.ColorHexString));
                        ReceiptTypeDataColors.Add((SolidColorBrush)converter.Convert(rto.ColorHexString, null, null, null));
                    }
                }
            }
        }

        private void RecalculateReceiptPayeeDataPoints()
        {
            ReceiptPayeeDataPoints.Clear();

            // BOTTLENECK
            //ItemTypes.RefreshPayees();

            foreach (Payee p in ItemTypes.Payees)
            {
                if (p.Receipts.Count > 0)
                {
                    List<Receipt> query = p.Receipts.Where(r => r.BudgetMonth == Month && r.BudgetYear == Year).ToList();

                    if (query.Count > 0)
                    {
                        int sum = 0;
                        int count = 0;

                        foreach (Receipt receipt in query)
                        {
                            sum += receipt.Amount;
                            count++;
                        }

                        ReceiptPayeeDataPoints.Add(new ReceiptPayeeDataPoint(p.Label, sum, count));
                    }
                }
            }
        }

        private async Task<bool> CreateImageUriFromPdfAsync(StorageFile pdfFile)
        {
            try
            {
                PdfDocument pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile); ;
                if (pdfDocument != null && pdfDocument.PageCount > 0)
                {
                    for (int pageIndex = 0; pageIndex < 1; pageIndex++) //for now, only convert the first page to an image
                    {
                        var pdfPage = pdfDocument.GetPage((uint)pageIndex);
                        if (pdfPage != null)
                        {
                            StorageFile destinationFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Guid.NewGuid().ToString() + ".jpg");
                            StorageFile oldImageFile = null;
                            
                            if (destinationFile != null)
                            {
                                // If Receipt has an image, grab its file
                                if (CurrentReceipt.ImageUrl != null)
                                {
                                    oldImageFile = await StorageFile.GetFileFromPathAsync(CurrentReceipt.ImageUrl);
                                }

                                IRandomAccessStream randomStream = await destinationFile.OpenAsync(FileAccessMode.ReadWrite);
                                PdfPageRenderOptions pdfPageRenderOptions = new PdfPageRenderOptions();
                                pdfPageRenderOptions.DestinationWidth = 300;
                               
                                await pdfPage.RenderToStreamAsync(randomStream, pdfPageRenderOptions);

                                // If Receipt currently has an image file a new file with a unique name will be created
                                destinationFile = await GetFileForReceiptImageAsync(destinationFile);

                                // Delete the previous receipt image file, if there was on
                                if (oldImageFile != null)
                                {
                                    await oldImageFile.DeleteAsync();
                                }

                                CurrentReceipt.OcrString = await GetOcrFromImageFile(destinationFile);
                                CurrentReceipt.ImageUrl = destinationFile.Path;

                                UpdateItem(CurrentReceipt);

                                //clean up
                                await randomStream.FlushAsync();
                                randomStream.Dispose();
                                pdfPage.Dispose();

                                return true;
                            }
                        }
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                IsImageLoading = false;
                throw ex;
            }
        }

        private async Task<StorageFile> GetFileForReceiptImageAsync(StorageFile imageFile)
        {
            string foldernameMonth, foldernameLabel, filenameMonth, filenameDay, filenameLabel;

            foldernameMonth = Month < 10 ? "0" + Month : "" + Month;
            filenameMonth = CurrentReceipt.TransactionDate.Month < 10 ? "0" + CurrentReceipt.TransactionDate.Month : "" + CurrentReceipt.TransactionDate.Month;
            filenameDay = CurrentReceipt.TransactionDate.Day < 10 ? "0" + CurrentReceipt.TransactionDate.Day : "" + CurrentReceipt.TransactionDate.Day;

            // Strip spaces, punctuation and symbols from foldername and filename
            foldernameLabel = CurrentBudget.Label.Replace(" ", "");
            foldernameLabel = new string(foldernameLabel.Where(c => !char.IsPunctuation(c)).ToArray());
            foldernameLabel = new string(foldernameLabel.Where(c => !char.IsSymbol(c)).ToArray());

            if (String.IsNullOrWhiteSpace(foldernameLabel))
                foldernameLabel = "Budget";

            filenameLabel = CurrentReceipt.Label.Replace(" ", "");
            filenameLabel = new string(filenameLabel.Where(c => !char.IsPunctuation(c)).ToArray());
            filenameLabel = new string(filenameLabel.Where(c => !char.IsSymbol(c)).ToArray());

            if (String.IsNullOrWhiteSpace(filenameLabel))
                filenameLabel = "Receipt";

            string monthFolderName = Year + "-" + foldernameMonth;
            string budgetFolderName = foldernameLabel + "_" + CurrentBudget.Id;
            string filename = CurrentReceipt.TransactionDate.Year + "-" +
                              filenameMonth + "-" + filenameDay + "_" +
                              filenameLabel + "_" + CurrentReceipt.Id + ".jpg";

            StorageFolder destinationFolder = await GetReceiptImageFolder();
            destinationFolder = await destinationFolder.CreateFolderAsync(monthFolderName, CreationCollisionOption.OpenIfExists);
            destinationFolder = await destinationFolder.CreateFolderAsync(budgetFolderName, CreationCollisionOption.OpenIfExists);
            
            imageFile = await imageFile.CopyAsync(destinationFolder, filename, NameCollisionOption.GenerateUniqueName);

            return imageFile;
        }

        private async Task<string> GetOcrFromImageFile(StorageFile file)
        {
            if (file != null)
            {
                try
                {
                    var randomAccessStream = await file.OpenReadAsync();

                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(randomAccessStream);
                    SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync();

                    OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                    OcrResult ocrResult = await ocrEngine.RecognizeAsync(bitmap);

                    return ocrResult.Text;
                }
                catch
                {
                    return "";
                }
            } 
            else
            {
                return "";
            }


        }

        //
        // MONTH NAVIGATION METHODS
        //

        public async void GoToNextMonth()
        {
            IsLoading = true;
            await Task.Delay(300);

            CurrentDate = CurrentDate.AddMonths(1);

            Month = CurrentDate.Month;
            Year = CurrentDate.Year;

            _monthId = Year.ToString() + Month.ToString();

            CurrentBudget = null;
            CurrentReceipt = null;
            CurrentExpense = null;

            // Need to unsubscribe from CollectionChanged before Initialization or it loads blank lists
            IncomeList.CollectionChanged -= ListView_CollectionChanged;
            ExpenseList.CollectionChanged -= ListView_CollectionChanged;
            BudgetList.CollectionChanged -= ListView_CollectionChanged;

            //InitializeListData(Month, Year);

            // For keeping track of the state of the list orders as they change
            LoadInitialListOrders();
            IncomeList.CollectionChanged += ListView_CollectionChanged;
            ExpenseList.CollectionChanged += ListView_CollectionChanged;
            BudgetList.CollectionChanged += ListView_CollectionChanged;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncomeList"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpenseList"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BudgetList"));

            GetTodaysExpenses();

            RecalculateIncomeTotal();
            RecalculateExpenseTotal();
            RecalculateBudgetTotal();
            RecalculateBudgetRemainingTotal();
            RecalculateGrandTotal();

            IsLoading = false;
        }

        public async void GoToPreviousMonth()
        {
            IsLoading = true;
            await Task.Delay(300);

            CurrentDate = CurrentDate.AddMonths(-1);

            Month = CurrentDate.Month;
            Year = CurrentDate.Year;

            _monthId = Year.ToString() + Month.ToString();

            CurrentBudget = null;
            CurrentReceipt = null;
            CurrentExpense = null;

            // Need to unsubscribe from CollectionChanged before Initialization or it loads blank lists
            IncomeList.CollectionChanged -= ListView_CollectionChanged;
            ExpenseList.CollectionChanged -= ListView_CollectionChanged;
            BudgetList.CollectionChanged -= ListView_CollectionChanged;

            //InitializeListData(Month, Year);

            // For keeping track of the state of the list orders as they change
            LoadInitialListOrders();
            IncomeList.CollectionChanged += ListView_CollectionChanged;
            ExpenseList.CollectionChanged += ListView_CollectionChanged;
            BudgetList.CollectionChanged += ListView_CollectionChanged;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncomeList"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpenseList"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BudgetList"));

            GetTodaysExpenses();

            RecalculateIncomeTotal();
            RecalculateExpenseTotal();
            RecalculateBudgetTotal();
            RecalculateBudgetRemainingTotal();
            RecalculateGrandTotal();

            IsLoading = false;
        }

        public async void GoToMonth(DateTime date)
        {
            IsLoading = true;
            await Task.Delay(300);

            CurrentDate = date;

            Month = CurrentDate.Month;
            Year = CurrentDate.Year;

            _monthId = Year.ToString() + Month.ToString();

            CurrentBudget = null;
            CurrentReceipt = null;
            CurrentExpense = null;

            // Need to unsubscribe from CollectionChanged before Initialization or it loads blank lists
            IncomeList.CollectionChanged -= ListView_CollectionChanged;
            ExpenseList.CollectionChanged -= ListView_CollectionChanged;
            BudgetList.CollectionChanged -= ListView_CollectionChanged;

            //InitializeListData(Month, Year);

            // For keeping track of the state of the list orders as they change
            LoadInitialListOrders();
            IncomeList.CollectionChanged += ListView_CollectionChanged;
            ExpenseList.CollectionChanged += ListView_CollectionChanged;
            BudgetList.CollectionChanged += ListView_CollectionChanged;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncomeList"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpenseList"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BudgetList"));

            GetTodaysExpenses();

            RecalculateIncomeTotal();
            RecalculateExpenseTotal();
            RecalculateBudgetTotal();
            RecalculateBudgetRemainingTotal();
            RecalculateGrandTotal();

            IsLoading = false;
        }
    }
}
