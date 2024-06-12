using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public class Payee
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        //
        // SPECIFIC PROPERTIES
        //

        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
            }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
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

        //TODO: for future use
        public string GeoCoordinates { get; set; }

        //
        // SQLITE RELATIONSHIP VARIABLES
        //

        [SQLiteNetExtensions.Attributes.OneToMany(CascadeOperations = SQLiteNetExtensions.Attributes.CascadeOperation.CascadeInsert | SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead)]
        public ObservableCollection<Receipt> Receipts { get; set; }

        [SQLiteNetExtensions.Attributes.OneToMany(CascadeOperations =  SQLiteNetExtensions.Attributes.CascadeOperation.CascadeInsert | SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead)]
        public ObservableCollection<Expense> Expenses { get; set; }

        //
        // CONSTRUCTORS
        //

        public Payee()
        {

        }

        public Payee(string Label)
        {
            _label = Label;
            Receipts = new ObservableCollection<Receipt>();
            Expenses = new ObservableCollection<Expense>();
        }

        public void Refresh()
        {
            Expenses = DBHelper.GetPayeeByName(Label).Expenses;
            Receipts = DBHelper.GetPayeeByName(Label).Receipts;
        }

        public void AddObject(object o)
        {
            if(o is Receipt r)
            {
                if(Receipts != null)
                {
                    Receipts.Add(r);
                }
            }
            else if(o is Expense e)
            {
                if(Expenses != null)
                {
                    Expenses.Add(e);
                }
            }
        }

        public void RemoveObject(Object o)
        {
            if(o is Receipt r)
            {
                if (Receipts != null && Receipts.Any(rec => rec.Id == r.Id))
                {
                    Receipts.Remove(r);
                }
            }
            else if(o is Expense e)
            {
                if (Expenses != null && Expenses.Any(exp => exp.Id == e.Id))
                {
                    Expenses.Remove(e);
                }
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

            Payee payee = (Payee)obj;
            return this.Id == payee.Id;
        }

        public override int GetHashCode()
        {
            return Id ^ 7;
        }
    }

    public class ReceiptPayeeDataPoint
    {
        public string Label { get; set; }

        public int Amount { get; set; }

        public int ReceiptCount { get; set; }

        public ReceiptPayeeDataPoint()
        {

        }
        public ReceiptPayeeDataPoint(string label, int amount, int receiptCount)
        {
            Label = label;
            Amount = amount;
            ReceiptCount = receiptCount;
        }
    }
}
