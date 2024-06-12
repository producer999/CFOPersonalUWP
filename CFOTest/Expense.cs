using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public class Expense : INotifyPropertyChanged
    {
        //
        // DISPLAY PROPERTIES
        //

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isInEditMode;
        [SQLite.Ignore]
        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set
            {
                _isInEditMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsInEditMode"));
            }
        }

        //
        // DATABASE PROPERTIES
        //

        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        //
        // STANDARD PROPERTIES
        //

        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                if(_label != value)
                {
                    _label = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
                }               
            }
        }

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set
            {
                if(_amount != value)
                {
                    _amount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Amount"));
                }               
            }
        }

        private int _month;
        public int Month
        {
            get { return _month; }
            private set
            {
                _month = value;
            }
        }

        private int _year;
        public int Year
        {
            get { return _year; }
            private set
            {
                _year = value;
            }
        }

        //
        // SPECIFIC PROPERTIES
        //

        private DateTime _dueDate;
        public DateTime DueDate
        {
            get { return _dueDate; }
            set
            {
                if (_dueDate != value)
                {
                    _dueDate = value;
                    DueDateYear = _dueDate.Year;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DueDate"));
                }
            }
        }
        
        public DateTime Date { get; set; }

        public int DueDateYear { get; set; }

        private bool _isPaid;
        public bool IsPaid
        {
            get { return _isPaid; }
            set
            {
                if (_isPaid != value)
                {
                    _isPaid = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPaid"));
                }
            }
        }

        //
        // FOREIGN KEYS
        //

        private int _typeId;
        [SQLiteNetExtensions.Attributes.ForeignKey(typeof(ExpenseTypeObject))]
        public int TypeId
        {
            get { return _typeId; }
            set
            {
                if (_typeId != value)
                {
                    _typeId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TypeId"));
                }
            }
        }

        private int _payeeId;
        [SQLiteNetExtensions.Attributes.ForeignKey(typeof(Payee))]
        public int PayeeId
        {
            get { return _payeeId; }
            set
            {
                _payeeId = value;
            }
        }

        //
        // SQLITE RELATIONSHIP VARIABLES
        //

        [SQLiteNetExtensions.Attributes.ManyToOne(CascadeOperations = SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead)]
        public Payee Payee { get; set; }

        [SQLiteNetExtensions.Attributes.ManyToOne(CascadeOperations = SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead)]
        public ExpenseTypeObject Type { get; set; }

        //
        // CONSTRUCTORS
        //

        public Expense()
        {

        }

        //
        // MAIN CONSTRUCTOR
        //

        public Expense(string Label, int Amount, ExpenseTypeObject Type, Payee Payee, int Month, int Year)
        {
            _amount = Amount;
            _label = Label;

            this.Type = Type;
            this.Payee = Payee;

            _month = Month;
            _year = Year;
            Date = new DateTime(Year, Month, 1);
            DueDate = new DateTime(Year, Month, 1).AddYears(-1000);

            _isPaid = false;
            _isInEditMode = false;
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

            Expense expense = (Expense)obj;
            return this.Id == expense.Id;
        }

        public override int GetHashCode()
        {
            return Id ^ 7;
        }
    }

    public class ExpenseCalendarDataPoint
    {

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
            }
        }

        private int _expenseCount;
        public int ExpenseCount
        {
            get { return _expenseCount; }
            set
            {
                _expenseCount = value;
            }
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
            }
        }

        public List<Expense> Expenses { get; set; }


        public ExpenseCalendarDataPoint()
        {
            _amount = 0;
            _expenseCount = 0;
        }

        public ExpenseCalendarDataPoint(DateTime date)
        {
            _amount = 0;
            _expenseCount = 0;

            _date = date;
        }

        public ExpenseCalendarDataPoint(DateTime date, int amount, int expenseCount, List<Expense> expenses)
        {
            _amount = amount;
            _expenseCount = expenseCount;

            _date = date;

            Expenses = expenses;
        }

    }
}
