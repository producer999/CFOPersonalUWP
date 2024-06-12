using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public class ExpenseTypeObject
    {
        //
        // DATABASE PROPERTIES
        //

        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                }
            }
        }

        // TODO: For future use
        private string _colorHexString;
        public string ColorHexString
        {
            get { return _colorHexString; }
            set
            {
                if (_colorHexString != value)
                {
                    _colorHexString = value;
                }
            }
        }

        //
        // SQLITE RELATIONSHIP VARIABLES
        //

        [SQLiteNetExtensions.Attributes.OneToMany(CascadeOperations = SQLiteNetExtensions.Attributes.CascadeOperation.CascadeInsert | SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead)]
        public ObservableCollection<Expense> Expenses { get; set; }

        public ExpenseTypeObject()
        {

        }

        public ExpenseTypeObject(string Label)
        {
            _label = Label;
            Expenses = new ObservableCollection<Expense>();
        }

        public ExpenseTypeObject(ExpenseTypes type)
        {
            _label = Enum.GetName(typeof(ExpenseTypes), type);
            Expenses = new ObservableCollection<Expense>();
        }

        public void Refresh()
        {
            Expenses = DBHelper.GetExpenseTypeByName(Label).Expenses;
        }

        public void AddExpense(Expense e)
        {
            if(Expenses != null)
            {
                Expenses.Add(e);
            }
        }
        public void RemoveExpense(Expense e)
        {
            if (Expenses != null && Expenses.Any(exp => exp.Id == e.Id))
            {
                Expenses.Remove(e);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            ExpenseTypeObject type = (ExpenseTypeObject)obj;
            return this.Id == type.Id;
        }

        public override int GetHashCode()
        {
            return Id ^ 7;
        }
    }

    public class ExpenseTypeDataPoint
    {
        public string Label { get; set; }

        public int Amount { get; set; }

        public int ExpenseCount { get; set; }

        public ExpenseTypeDataPoint()
        {

        }
        public ExpenseTypeDataPoint(string label, int amount, int expenseCount)
        {
            Label = label;
            Amount = amount;
            ExpenseCount = expenseCount;
        }
    }
}
