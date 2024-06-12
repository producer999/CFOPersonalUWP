using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace CFOTest
{
    public static class DBHelper
    {
        public static string DB_NAME { get; set; }
        public static string DB_PATH {get; set;}

        //create database with specified name
        //the path should usually be Windows.Storage.ApplicationData.Current.LocalFolder
        public static bool CreateDatabase(string filename)
        {
            DB_NAME = filename + ".sqlite";
      
            //If specified databse file does not exist, create a new blank database with correct columns
            if (!CheckFileExists(DB_NAME))
            {
                DB_PATH = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, DB_NAME);

                using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
                {              
                    conn.CreateTable<Income>();
                    conn.CreateTable<Expense>();
                    conn.CreateTable<Budget>();
                    conn.CreateTable<Receipt>();
                    conn.CreateTable<Payee>();
                    conn.CreateTable<IncomeTypeObject>();
                    conn.CreateTable<ExpenseTypeObject>();
                    conn.CreateTable<BudgetTypeObject>();
                    conn.CreateTable<ReceiptTypeObject>();

                    CreateInitializationData(conn);
                    SettingsHelper.SetDefaultLocalSettings();
                }

                return true;
            }
            else
            {
                DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_NAME);
                return false;
            }
        }

        public static async Task<bool> LoadDatabaseFromFileAsync()
        {
            try
            {
                FileOpenPicker picker = new FileOpenPicker();
                picker.SuggestedStartLocation = PickerLocationId.Desktop;
                picker.FileTypeFilter.Add(".sqlite");
                StorageFile file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    await file.CopyAsync(ApplicationData.Current.LocalFolder, DB_NAME, NameCollisionOption.ReplaceExisting);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> CopyDatabaseAsync()
        {
            try
            {
                FileSavePicker picker = new FileSavePicker();
                picker.SuggestedStartLocation = PickerLocationId.Desktop;
                picker.FileTypeChoices.Add("SQLite3 Database", new List<string>() { ".sqlite" });
                picker.DefaultFileExtension = ".sqlite";
                picker.SuggestedFileName = "Database";

                StorageFile newFile = await picker.PickSaveFileAsync();
                StorageFile existingFile = await StorageFile.GetFileFromPathAsync(DB_PATH);

                await existingFile.CopyAndReplaceAsync(newFile);

                return true;
            }
            catch
            {
                return false;
            }           
        }

        //check if the file filename exists in the Local data Folder
        private static bool CheckFileExists(string filename)
        {
            try
            {
                // THIS ASYNC CALL DOES NOT WORK WITH RELEASE BUILD
                //var store = await Windows.Storage.ApplicationData.Current.LocalFolder.TryGetItemAsync(filename);
                //return store != null;
                string folder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
                FileInfo fi = new FileInfo(folder + "\\" + filename);

                return fi.Exists;
            }
            catch
            {
                return false;
            }
        }

        //Insert new item into the Database - Updated 7/22/2017
        public static void Insert(Object o)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                conn.RunInTransaction(() =>
                {
                    conn.InsertWithChildren(o);
                });
            }
        }

        //TODO: Under Construction
        public static Object Get<T>(T item, int i)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                Type t = typeof(T);

                var methodInfo = typeof(ReadOperations).GetMethod("GetWithChildren", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                var method = methodInfo.MakeGenericMethod(t);

                Object[] parameters = { conn, item.GetType().GetPrimaryKey().GetValue(item), true };
                var result = method.Invoke(conn, parameters);

                return result;
            }
        }

        // Get all items of a certain type from the database as an ObservableCollection
        public static ObservableCollection<T> GetAll<T>() where T : class
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
                {
                    Type t = typeof(T);

                    var methodInfo = typeof(ReadOperations).GetMethod("GetAllWithChildren", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    var method = methodInfo.MakeGenericMethod(t);


                    Object[] parameters = { conn, null, true };
                    var result = method.Invoke(conn, parameters);

                    List<T> l = result as List<T>;

                    return new ObservableCollection<T>(l);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR - Problem getting items from the Database via GetAll<T>.");
                Debug.WriteLine(e);
                return null;
            }
        }

        // Get all items of a certain type from the database as an ObservableCollection
        public static ObservableCollection<T> GetAll<T>(bool recursive) where T : class
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
                {
                    Type t = typeof(T);

                    var methodInfo = typeof(ReadOperations).GetMethod("GetAllWithChildren", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    var method = methodInfo.MakeGenericMethod(t);


                    Object[] parameters = { conn, null, recursive };
                    var result = method.Invoke(conn, parameters);

                    List<T> l = result as List<T>;

                    return new ObservableCollection<T>(l);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR - Problem getting items from the Database via GetAll<T>.");
                Debug.WriteLine(e);
                return null;
            }
        }

        // Get an ObservableCollection containing all Expenses from a given month and year
        public static ObservableCollection<Expense> GetExpensesByMonth(int month, int year)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                ObservableCollection<Expense> result;

                List<Expense> query = conn.GetAllWithChildren<Expense>(e => e.Month == month && e.Year == year, true).ToList();
                result = new ObservableCollection<Expense>(query);

                return result;
            }
        }

        // Get an ObservableCollection containing all Expenses from a given month and year
        public static ObservableCollection<Expense> GetExpensesByMonth(int month, int year, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                ObservableCollection<Expense> result;

                List<Expense> query = conn.GetAllWithChildren<Expense>(e => e.Month == month && e.Year == year, recursive).ToList();
                result = new ObservableCollection<Expense>(query);

                return result;
            }
        }

        // Get a List containing all Expenses from a given year
        public static List<Expense> GetExpensesByYear(int year)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Expense> query = conn.GetAllWithChildren<Expense>(e => e.DueDateYear == year).ToList();
                return query;
            }
        }

        // Get a List containing all Expenses from a given year
        public static List<Expense> GetExpensesByYear(int year, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Expense> query = conn.GetAllWithChildren<Expense>(e => e.DueDateYear == year, recursive).ToList();
                return query;
            }
        }

        // Get an ObservableCollection containing all Incomes from a given month and year
        public static ObservableCollection<Income> GetIncomesByMonth(int month, int year)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                ObservableCollection<Income> result;

                List<Income> query = conn.GetAllWithChildren<Income>(e => e.Month == month && e.Year == year, true).ToList();
                result = new ObservableCollection<Income>(query);

                return result;
            }
        }

        // Get an ObservableCollection containing all Incomes from a given month and year
        public static ObservableCollection<Income> GetIncomesByMonth(int month, int year, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                ObservableCollection<Income> result;

                List<Income> query = conn.GetAllWithChildren<Income>(e => e.Month == month && e.Year == year, recursive).ToList();
                result = new ObservableCollection<Income>(query);

                return result;
            }
        }

        // Get an ObservableCollection containing all Budgets from a given month and year
        public static ObservableCollection<Budget> GetBudgetsByMonth(int month, int year)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                ObservableCollection<Budget> result;

                List<Budget> query = conn.GetAllWithChildren<Budget>(e => e.Month == month && e.Year == year, true).ToList();
                result = new ObservableCollection<Budget>(query);

                return result;
            }
        }

        // Get an ObservableCollection containing all Budgets from a given month and year
        public static ObservableCollection<Budget> GetBudgetsByMonth(int month, int year, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                ObservableCollection<Budget> result;

                List<Budget> query = conn.GetAllWithChildren<Budget>(e => e.Month == month && e.Year == year, recursive).ToList();
                result = new ObservableCollection<Budget>(query);

                return result;
            }
        }

        // Get an ObservableCollection containing all Receipts from a given month and year
        public static ObservableCollection<Receipt> GetReceiptsByMonth(int month, int year)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                ObservableCollection<Receipt> result;

                List<Receipt> query = conn.GetAllWithChildren<Receipt>(r => r.BudgetMonth == month && r.BudgetYear == year).ToList();
                result = new ObservableCollection<Receipt>(query);

                return result;
            }
        }

        // Get a List containing all Receipts from a given year
        public static List<Receipt> GetReceiptsByYear(int year)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                // Receipt with BudgetId = 0 have been deleted so don't get those

                // If a budget is entered in a month of parameter "year", but the TransactionDate set to another year, the receipt
                // will still be selected since it was created in a budget from "year". This seems like a bug but technically
                //it is the designed behavior.

                List<Receipt> query = conn.GetAllWithChildren<Receipt>(r => r.BudgetYear == year && r.BudgetId != 0).ToList();               
                return query;
            }
        }

        // Get a List containing all Receipts from a given year
        public static List<Receipt> GetReceiptsByYear(int year, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                // Receipt with BudgetId = 0 have been deleted so don't get those

                // If a budget is entered in a month of parameter "year", but the TransactionDate set to another year, the receipt
                // will still be selected since it was created in a budget from "year". This seems like a bug but technically
                //it is the designed behavior.

                List<Receipt> query = conn.GetAllWithChildren<Receipt>(r => r.BudgetYear == year && r.BudgetId != 0, recursive).ToList();
                return query;
            }
        }

        // Get an ObservableCollection containing all Receipts with a given Budget Id
        public static ObservableCollection<Receipt> GetReceiptsByBudgetId(int budgetId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                ObservableCollection<Receipt> result;

                List<Receipt> query = conn.GetAllWithChildren<Receipt>(r => r.BudgetId == budgetId, true).ToList();
                result = new ObservableCollection<Receipt>(query);

                return result;
            }
        }

        // Get an ObservableCollection containing all Receipts with a given Budget Id
        public static ObservableCollection<Receipt> GetReceiptsByBudgetId(int budgetId, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                ObservableCollection<Receipt> result;

                List<Receipt> query = conn.GetAllWithChildren<Receipt>(r => r.BudgetId == budgetId, recursive).ToList();
                result = new ObservableCollection<Receipt>(query);

                return result;
            }
        }

        // Get an particular Type or Payee item based on its Label
        public static IncomeTypeObject GetIncomeTypeByName(string label)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<IncomeTypeObject> query = conn.GetAllWithChildren<IncomeTypeObject>(it => it.Label == label, true).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static ExpenseTypeObject GetExpenseTypeByName(string label)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<ExpenseTypeObject> query = conn.GetAllWithChildren<ExpenseTypeObject>(it => it.Label == label, true).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static BudgetTypeObject GetBudgetTypeByName(string label)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<BudgetTypeObject> query = conn.GetAllWithChildren<BudgetTypeObject>(it => it.Label == label, true).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static ReceiptTypeObject GetReceiptTypeByName(string label)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<ReceiptTypeObject> query = conn.GetAllWithChildren<ReceiptTypeObject>(it => it.Label == label, true).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static ReceiptTypeObject GetReceiptTypeById(int id, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<ReceiptTypeObject> query = conn.GetAllWithChildren<ReceiptTypeObject>(rt => rt.Id == id, recursive).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static Payee GetPayeeByName(string label)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Payee> query = conn.GetAllWithChildren<Payee>(it => it.Label == label, true).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static Payee GetPayeeById(int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Payee> query = conn.GetAllWithChildren<Payee>(it => it.Id == id, true).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static Payee GetPayeeById(int id, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Payee> query = conn.GetAllWithChildren<Payee>(it => it.Id == id, recursive).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static Budget GetBudgetById(int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Budget> query = conn.GetAllWithChildren<Budget>(b => b.Id == id, true).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static Budget GetBudgetById(int id, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Budget> query = conn.GetAllWithChildren<Budget>(b => b.Id == id, recursive).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static Receipt GetReceiptById(int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Receipt> query = conn.GetAllWithChildren<Receipt>(r => r.Id == id, true).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static Receipt GetReceiptById(int id, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Receipt> query = conn.GetAllWithChildren<Receipt>(r => r.Id == id, recursive).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static Expense GetExpenseById(int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Expense> query = conn.GetAllWithChildren<Expense>(e => e.Id == id, true).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static Expense GetExpenseById(int id, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Expense> query = conn.GetAllWithChildren<Expense>(e => e.Id == id, recursive).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static Income GetIncomeById(int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Income> query = conn.GetAllWithChildren<Income>(i => i.Id == id, true).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        public static Income GetIncomeById(int id, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                List<Income> query = conn.GetAllWithChildren<Income>(i => i.Id == id, recursive).ToList();

                return query.Count == 0 ? null : query.First();
            }
        }

        //Update existing item in the database: Updated 8/3/2017 
        public static void Update(Object o)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                try
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.UpdateWithChildren(o);
                    });
                }
                catch(Exception e)
                {
                    Debug.WriteLine("ERROR: Problem running Update() (Maybe you are trying to update an entry that doesnt exist?");
                    Debug.WriteLine(e);
                    Debug.WriteLine("SQLite says: " + e.Message);
                }              
            }
        }

        //Delete all items of a certain type from the database, leaving their children
        public static void DeleteAll<T>()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                Type t = typeof(T);

                var methodInfo = typeof(SQLiteConnection).GetMethod("DropTable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                var method = methodInfo.MakeGenericMethod(t);

                Object[] parameters = { };
                method.Invoke(conn, parameters);

                conn.CreateTable(t);
            }
        }

        //Delete specific item from the database, leaving its children
        public static void Delete(Object o)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                try
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.Delete(o);
                    });
                }
                catch(Exception e)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("ERROR: Problem running Delete() (Maybe you are trying to delete an entry that doesn't exist?");
                    Debug.WriteLine(e);
                    Debug.WriteLine("Database says: " + e.Message);
                    Debug.WriteLine("");
                }
                
            }
        }

        //Delete specific item from the database, leaving its children
        public static void RemoveAllDeletedReceipts()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                try
                {
                    List<Receipt> deleteList = GetReceiptsByBudgetId(0, false).ToList();

                    foreach(Receipt r in deleteList)
                    {
                        conn.Delete(r);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("ERROR: Problem running Delete() (Maybe you are trying to delete an entry that doesn't exist?");
                    Debug.WriteLine(e);
                    Debug.WriteLine("Database says: " + e.Message);
                    Debug.WriteLine("");
                }
            }
        }

        //Delete specific item and all of its children from the database
        public static void Delete(Object o, bool recursive)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DB_PATH))
            {
                try
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.Delete(o, recursive);
                    });
                }
                catch (Exception e)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("ERROR: Problem running Delete() (Maybe you are trying to delete an entry that doesn't exist?");
                    Debug.WriteLine(e);
                    Debug.WriteLine("Database says: " + e.Message);
                    Debug.WriteLine("");
                }

            }
        }

        //
        //Build the Types from initial data and populate the lists with temp data
        //

        private static void CreateInitializationData(SQLiteConnection conn)
        {
            //
            // Set default settings
            //

            // Reset all of the user defined folder paths
            // Turn on the Delete Item Confirmation Dialog
            SettingsHelper.SetDefaultLocalSettings();

            //
            // Clear Temporary Folder
            //

            SettingsHelper.ClearTemporaryFolder();

            //
            // Build Type Tables from Initial Data Files
            //

            //
            // Initialize the default IncomeTypes
            //

            var incEnum = Enum.GetValues(typeof(IncomeTypes)).Cast<IncomeTypes>();
            List<IncomeTypes> incList = incEnum.ToList();

            foreach(IncomeTypes it in incList)
            {
                conn.InsertWithChildren(new IncomeTypeObject(it));
            }

            //
            // Initialize the default ExpenseTypes
            //

            var expEnum = Enum.GetValues(typeof(ExpenseTypes)).Cast<ExpenseTypes>();
            List<ExpenseTypes> expList = expEnum.ToList();

            foreach (ExpenseTypes et in expList)
            {
                conn.InsertWithChildren(new ExpenseTypeObject(et));
            }

            //
            // Initialize the default BudgetTypes
            //

            var budEnum = Enum.GetValues(typeof(BudgetTypes)).Cast<BudgetTypes>();
            List<BudgetTypes> budList = budEnum.ToList();

            foreach (BudgetTypes bt in budList)
            {
                conn.InsertWithChildren(new BudgetTypeObject(bt));
            }

            //
            // Initialize the default ReceiptTypes and Colors
            //

            string[] initialReceiptTypeColors = { "#000000", "#F5E1EA", "#61D670", "#ABD0F7", "#FBBEC3", "#64A796", "#DBD636",
                                                  "#CB1302", "#7FF8AA", "#F5F2B0", "#B77B84", "#E0F8B0", "#ECB103", "#237077",
                                                  "#F5F186", "#A9B1C2", "#1E230E", "#21901A", "#C55137", "#FB4D7B", "#4C002D",
                                                  "#FBE2E7", "#FD6806", "#F0F5F8", "#C6FDA2", "#24F8D6", "#B0AF0C", "#0068C4",
                                                  "#B612F7", "#C01685", "#67AFC1", "#DBD636"  };

            var recEnum = Enum.GetValues(typeof(ReceiptTypes)).Cast<ReceiptTypes>();
            List<ReceiptTypes> recList = recEnum.ToList();

            for(int i=0; i<recList.Count-1; i++)
            {
                conn.InsertWithChildren(new ReceiptTypeObject(recList[i], initialReceiptTypeColors[i]));
            }

            //
            // Initialize the default Payee
            //

            conn.InsertWithChildren(new Payee("Undefined"));

            //
            // ADD THE INITIAL INCOMES, EXPENSES AND BUDGETS
            //
            
            int initialMonth = DateTime.Today.Month;
            int initialYear = DateTime.Today.Year;

            //
            // Create Incomes with existing Types
            //

            //IncomeTypeObject paycheckType = conn.GetAllWithChildren<IncomeTypeObject>(t => t.Label == "Paycheck", false).ToList().First();
            //IncomeTypeObject royaltyType = conn.GetAllWithChildren<IncomeTypeObject>(t => t.Label == "Royalty", false).ToList().First();

            //Income viper = new Income("Viperbox", 364600, paycheckType, initialMonth, initialYear);
            //Income sen = new Income("sourcEleven", 500000, royaltyType, initialMonth, initialYear);
            //conn.InsertWithChildren(viper);
            //conn.InsertWithChildren(sen);

            //paycheckType.Refresh();
            //royaltyType.Refresh();

            //
            // Create Expesnes with new Payees and Existing Types
            //

            //ExpenseTypeObject rentType = conn.GetAllWithChildren<ExpenseTypeObject>(t => t.Label == "Rent", false).ToList().First();
            //ExpenseTypeObject loanType = conn.GetAllWithChildren<ExpenseTypeObject>(t => t.Label == "LoanPayment", false).ToList().First();

            //Payee jcrewPayee = new Payee("J-Crew");
            //Payee salliePayee = new Payee("Sallie Mae");
            //Payee discoverPayee = new Payee("Discover");
            //Payee citiPayee = new Payee("CitiBank");
            //conn.InsertWithChildren(jcrewPayee);
            //conn.InsertWithChildren(salliePayee);
            //conn.InsertWithChildren(discoverPayee);
            //conn.InsertWithChildren(citiPayee);

            //Expense rent = new Expense("Rent", 135000, rentType, jcrewPayee, initialMonth, initialYear);
            //Expense loan1 = new Expense("Loan 1", 18794, loanType, salliePayee, initialMonth, initialYear);
            //Expense loan2 = new Expense("Loan 2", 37145, loanType, discoverPayee, initialMonth, initialYear);
            //Expense loan3 = new Expense("Loan 3", 33992, loanType, citiPayee, initialMonth, initialYear);
            //conn.InsertWithChildren(rent);
            //conn.InsertWithChildren(loan1);
            //conn.InsertWithChildren(loan2);
            //conn.InsertWithChildren(loan3);

            //rentType.Refresh();
            //loanType.Refresh();
            //jcrewPayee.Refresh();
            //salliePayee.Refresh();
            //discoverPayee.Refresh();
            //citiPayee.Refresh();

            //
            // Create Expenses with existing Payee and existing Types
            //

            //ExpenseTypeObject utilType = conn.GetAllWithChildren<ExpenseTypeObject>(t => t.Label == "Utility", false).ToList().First();

            //Payee conedPayee = new Payee("ConEd");
            //conn.InsertWithChildren(conedPayee);

            //Expense gas = new Expense("Gas", 2000, utilType, conedPayee, initialMonth, initialYear);
            //Expense electric = new Expense("Electric", 5000, utilType, conedPayee, initialMonth, initialYear);
            //conn.InsertWithChildren(gas);
            //conn.InsertWithChildren(electric);

            //utilType.Refresh();
            //conedPayee.Refresh();

            //
            // Create Expesnes with new Payees and Existing Types
            //

            //ExpenseTypeObject carPaymentType = conn.GetAllWithChildren<ExpenseTypeObject>(t => t.Label == "CarPayment", false).ToList().First();
            //ExpenseTypeObject carInsuranceType = conn.GetAllWithChildren<ExpenseTypeObject>(t => t.Label == "CarInsurance", false).ToList().First();

            //Payee verizonPayee = new Payee("Verizon");
            //Payee chasePayee = new Payee("Chase");
            //Payee geicoPayee = new Payee("Geico");
            //conn.InsertWithChildren(verizonPayee);
            //conn.InsertWithChildren(chasePayee);
            //conn.InsertWithChildren(geicoPayee);

            //Expense cable = new Expense("Cable/Internet", 9000, utilType, verizonPayee, initialMonth, initialYear);
            //Expense carPay = new Expense("Car Payment", 27388, carPaymentType, chasePayee, initialMonth, initialYear);
            //Expense carIns = new Expense("Car Insurance", 16000, carInsuranceType, geicoPayee, initialMonth, initialYear);
            //conn.InsertWithChildren(cable);
            //conn.InsertWithChildren(carPay);
            //conn.InsertWithChildren(carIns);

            //utilType.Refresh();
            //verizonPayee.Refresh();
            //chasePayee.Refresh();
            //geicoPayee.Refresh();

            //
            // Create Budgets with Existing Type and add Receipts with no Payees 
            //

            //BudgetTypeObject creditType = conn.GetAllWithChildren<BudgetTypeObject>(t => t.Label == "Credit", false).ToList().First();

            //Budget transportation = new Budget("Transportation", 40000, creditType, initialMonth, initialYear);
            //conn.InsertWithChildren(transportation);

            //creditType.Refresh();

            // Add receipts to the budget with existing type (undefined) and payee (undefined)

            //Random rand = new Random();

            //ReceiptTypeObject undefRecType = conn.GetAllWithChildren<ReceiptTypeObject>(t => t.Label == "Undefined", false).ToList().First();
            //Payee undefPayee = conn.GetAllWithChildren<Payee>(t => t.Label == "Undefined", false).ToList().First();

            //transportation.AddReceipt("Gas", rand.Next(500, 2500), new DateTime(initialYear, initialMonth, rand.Next(1, 30)), undefRecType, undefPayee);
            //transportation.AddReceipt("Parking", rand.Next(100, 1000), new DateTime(initialYear, initialMonth, rand.Next(1, 30)), undefRecType, undefPayee);
            //transportation.AddReceipt("Ezpass", rand.Next(10000, 15000), new DateTime(initialYear, initialMonth, rand.Next(1, 30)), undefRecType, undefPayee);

            //
            // Create Budget with existing type and No Receipts
            //

            //BudgetTypeObject cashType = conn.GetAllWithChildren<BudgetTypeObject>(t => t.Label == "Cash", false).ToList().First();

            //Budget groceries = new Budget("Groceries", 30000, cashType, initialMonth, initialYear);
            //conn.InsertWithChildren(groceries);

            //cashType.Refresh();

            //
            // Create Budgets with existing type and add Receipts with new Payees
            //

            // 1) Create Budget

            //Budget cashEntertainment = new Budget("Entertainment", 10000, cashType, initialMonth, initialYear);
            //conn.InsertWithChildren(cashEntertainment);

            //cashType.Refresh();

            // 2) Add Receipts to budget with new Payee and existing types

            //ReceiptTypeObject foodType = conn.GetAllWithChildren<ReceiptTypeObject>(t => t.Label == "Food", false).ToList().First();
            //ReceiptTypeObject wineType = conn.GetAllWithChildren<ReceiptTypeObject>(t => t.Label == "Wine", false).ToList().First();

            //Payee chinesePayee = new Payee("21st Ave Chinese");
            //Payee winePayee = new Payee("Gary's Wine");
            //conn.InsertWithChildren(chinesePayee);
            //conn.InsertWithChildren(winePayee);

            //cashEntertainment.AddReceipt("Chinese Food", 1000, new DateTime(initialYear, initialMonth, rand.Next(1, 30)), foodType, chinesePayee);
            //cashEntertainment.AddReceipt("Wine", 1100, new DateTime(initialYear, initialMonth, rand.Next(1, 30)), wineType, winePayee);

            //
            // Create Budgets with existing type and add Receipts with existing Payee and existing type
            //

            // 1) Create Budget

            //Budget creditEntertainment = new Budget("Entertainment", 25000, creditType, initialMonth, initialYear);
            //conn.InsertWithChildren(creditEntertainment);

            //creditType.Refresh();

            // 2) Get or Create Payee and type and Add Receipts to budget with existing Payee and type

            //ReceiptTypeObject ccpayType = conn.GetAllWithChildren<ReceiptTypeObject>(t => t.Label == "CreditCardPayment", false).ToList().First();

            //Payee amexPayee = new Payee("American Express");
            //conn.InsertWithChildren(amexPayee);

            //creditEntertainment.AddReceipt("Amex Payment", 4000, new DateTime(initialYear, initialMonth, rand.Next(1, 30)), ccpayType, amexPayee);

            //
            // Add Receipt to budget with no Payee and existing type and add payee later
            //

            // 1) Add Receipt to Budget

            //ReceiptTypeObject elec = conn.GetAllWithChildren<ReceiptTypeObject>(t => t.Label == "Electronics", false).ToList().First();

            //Payee amazonPayee = new Payee("Amazon");
            //conn.InsertWithChildren(amazonPayee);

            //creditEntertainment.AddReceipt("Phone Holder", 1300, new DateTime(initialYear, initialMonth, rand.Next(1, 30)), elec, amazonPayee);         
      
        }

        
    }
    
}
