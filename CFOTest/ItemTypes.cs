using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace CFOTest
{
    public static class ItemTypes
    {
        // Temporary collection of colors used until converted to a text file
        //string[] initialReceiptTypeColors = { "#8554A2", "#F5E1EA", "#61D670", "#ABD0F7", "#FBBEC3", "#64A796", "#DBD636",
        //                                          "#CB1302", "#7FF8AA", "#F5F2B0", "#B77B84", "#E0F8B0", "#ECB103", "#237077",
        //                                          "#F5F186", "#A9B1C2", "#1E230E", "#21901A", "#C55137", "#FB4D7B", "#4C002D",
        //                                          "#FBE2E7", "#FD6806", "#F0F5F8", "#C6FDA2", "#24F8D6", "#B0AF0C", "#0068C4",
        //                                          "#B612F7", "#C01685", "#67AFC1", "#DBD636"  };
        private static string[] availableTypeColors = new string[50]{ "#fd5c63", "#00a0af", "#b84592", "#00bce4", "#c1d82f", "#ff6a00", "#7f181b", "#c4dff6", "#3be8b0", "#d0d2d3",
                                                                    "#5ecc62", "#e01f3d", "#fff200", "#a0ac48", "#ffb900", "#788cb6", "#2baf2b", "#ce1126", "#d52685", "#fff9ea",
                                                                    "#00aaff", "#d03027", "#237f52", "#388ed1", "#fa9f1e", "#decba5", "#84754e", "#a25016", "#566127", "#ed1c16",
                                                                    "#ae63e4", "#76daff", "#004eaf", "#c0de9e", "#85c446", "#f8dfc2", "#cd595a", "#71c6c1", "#05cc47", "#febd17",
                                                                    "#005be2", "#56c1ab", "#f9a852", "#99aab5", "#ec1944", "#f7c8c9", "#a560e8", "#f0f0f0", "#2facb2", "#660099"};

        public static ObservableCollection<IncomeTypeObject> IncomeTypes { get; set; }
        public static ObservableCollection<ExpenseTypeObject> ExpenseTypes { get; set; }
        public static ObservableCollection<BudgetTypeObject> BudgetTypes { get; set; }
        public static ObservableCollection<ReceiptTypeObject> ReceiptTypes { get; set; }
        public static ObservableCollection<Payee> Payees { get; set; }

        static ItemTypes()
        {
            // BOTTLENECK, changed to non-recursive
            //IncomeTypes = DBHelper.GetAll<IncomeTypeObject>();
            //ExpenseTypes = DBHelper.GetAll<ExpenseTypeObject>();
            //BudgetTypes = DBHelper.GetAll<BudgetTypeObject>();
            //ReceiptTypes = DBHelper.GetAll<ReceiptTypeObject>();
            //Payees = DBHelper.GetAll<Payee>();

            IncomeTypes = DBHelper.GetAll<IncomeTypeObject>(false);
            ExpenseTypes = DBHelper.GetAll<ExpenseTypeObject>(false);
            BudgetTypes = DBHelper.GetAll<BudgetTypeObject>(false);
            ReceiptTypes = DBHelper.GetAll<ReceiptTypeObject>(false);
            Payees = DBHelper.GetAll<Payee>(false);
        }

        static void Initialize()
        {
            IncomeTypes = DBHelper.GetAll<IncomeTypeObject>();
            ExpenseTypes = DBHelper.GetAll<ExpenseTypeObject>();
            BudgetTypes = DBHelper.GetAll<BudgetTypeObject>();
            ReceiptTypes = DBHelper.GetAll<ReceiptTypeObject>();
            Payees = DBHelper.GetAll<Payee>();
        }

        public static void RefreshAll()
        {
            IncomeTypes = DBHelper.GetAll<IncomeTypeObject>();
            ExpenseTypes = DBHelper.GetAll<ExpenseTypeObject>();
            BudgetTypes = DBHelper.GetAll<BudgetTypeObject>();
            ReceiptTypes = DBHelper.GetAll<ReceiptTypeObject>();
            Payees = DBHelper.GetAll<Payee>();
        }

        public static void RefreshReceiptTypes()
        {
            ReceiptTypes = DBHelper.GetAll<ReceiptTypeObject>();
        }

        public static void RefreshIncomeTypes()
        {
            IncomeTypes = DBHelper.GetAll<IncomeTypeObject>();
        }

        public static void RefreshExpenseTypes()
        {
            ExpenseTypes = DBHelper.GetAll<ExpenseTypeObject>();
        }

        public static void RefreshBudgetTypes()
        {
            BudgetTypes = DBHelper.GetAll<BudgetTypeObject>();
        }

        public static void RefreshPayees()
        {
            Payees = DBHelper.GetAll<Payee>();
        }

        public static void AddExpenseType(ExpenseTypeObject eto)
        {
            if(eto != null)
            {
                if (!ExpenseTypes.Contains(eto))
                {
                    ExpenseTypes.Add(eto);
                    DBHelper.Insert(eto);
                }
            }
        }

        public static void AddIncomeType(IncomeTypeObject ito)
        {
            if (ito != null)
            {
                if (!IncomeTypes.Contains(ito))
                {
                    IncomeTypes.Add(ito);
                    DBHelper.Insert(ito);
                }
            }
        }

        public static void AddBudgetType(BudgetTypeObject bto)
        {
            if (bto != null)
            {
                if (!BudgetTypes.Contains(bto))
                {
                    BudgetTypes.Add(bto);
                    DBHelper.Insert(bto);
                }
            }
        }

        public static void AddReceiptType(ReceiptTypeObject rto)
        {
            if (rto != null)
            {
                if (!ReceiptTypes.Contains(rto))
                {
                    ReceiptTypes.Add(rto);
                    DBHelper.Insert(rto);
                }
            }
        }

        public static void AddPayee(Payee p)
        {
            if (p != null)
            {
                if (!Payees.Contains(p))
                {
                    Payees.Add(p);
                    DBHelper.Insert(p);
                }
            }
        }

        public static string GetRandomColorHex()
        {
            Random r = new Random();

            return availableTypeColors[r.Next(0,49)];
        }
    }
}
