using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    
    public class Receipt : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
                _amount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Amount"));
            }
        }

        //
        // SPECIFIC PROPERTIES
        //

        private DateTime _transactionDate;
        public DateTime TransactionDate
        {
            get { return _transactionDate; }
            set
            {
                _transactionDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TransactionDate"));
            }
        }

        private DateTime _creationDate;
        public DateTime CreationDate
        {
            get { return _creationDate; }
            private set { _creationDate = value; }
        }

        private int _budgetMonth;
        public int BudgetMonth
        {
            get { return _budgetMonth; }
            set
            {
                _budgetMonth = value;
            }
        }

        private int _budgetYear;
        public int BudgetYear
        {
            get { return _budgetYear; }
            set
            {
                _budgetYear = value;
            }
        }

        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Notes"));
            }
        }

        private string _imageUrl;
        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageUrl"));
            }
        }

        private string _ocrString;
        public string OcrString
        {
            get { return _ocrString; }
            set
            {
                _ocrString = value;
            }
        }

        [SQLite.Ignore]
        public bool DisableNotifications { get; set; }

        //
        // FOREIGN KEYS
        //

        private int _budgetId;
        [SQLiteNetExtensions.Attributes.ForeignKey(typeof(Budget))]
        public int BudgetId
        {
            get { return _budgetId; }
            set
            {
                _budgetId = value;
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

        private int _typeId;
        [SQLiteNetExtensions.Attributes.ForeignKey(typeof(ReceiptTypeObject))]
        public int TypeId
        {
            get { return _typeId; }
            set
            {
                _typeId = value;
            }
        }

        //
        // SQLITE RELATIONSHIP VARIABLES
        //

        private Budget _budget;
        [SQLiteNetExtensions.Attributes.ManyToOne(CascadeOperations = SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead)]
        public Budget Budget
        {
            get { return _budget; }
            set
            {
                if (_budget != value)
                {
                    _budget = value;
                    if(!DisableNotifications)
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Budget"));
                    }
                }
            }
        }

        private Payee _payee;
        [SQLiteNetExtensions.Attributes.ManyToOne(CascadeOperations = SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead)]
        public Payee Payee
        {
            get { return _payee; }
            set
            {
                if (_payee != value)
                {
                    _payee = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Payee"));
                }
            }
        }

        private ReceiptTypeObject _type;
        [SQLiteNetExtensions.Attributes.ManyToOne(CascadeOperations = SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead)]
        public ReceiptTypeObject Type
        {
            get { return _type; }
            set
            {
                if(_type != value)
                {
                    _type = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Type"));
                }
            }
        }

        //
        // CONSTRUCTORS
        //

        public Receipt()
        {
            DisableNotifications = false;
        }
      
        //
        // MAIN CONSTRUCTOR
        //

        public Receipt(string Label, int Amount, DateTime TransactionDate, Budget Budget, ReceiptTypeObject Type, Payee Payee)
        {
            DisableNotifications = false;

            _label = Label;
            _amount = Amount;
            _budgetMonth = Budget.Month;
            _budgetYear = Budget.Year;
            
            _transactionDate = TransactionDate;
            _creationDate = DateTime.Now;

            this._budget = Budget;
            this._type = Type;
            this._payee = Payee;

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

            if(obj is Receipt receipt)
            {
                return this.Id == receipt.Id;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return Id ^ 7;
        }
    }

    public class ReceiptCalendarDataPoint 
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

        private int _receiptCount;
        public int ReceiptCount
        {
            get { return _receiptCount; }
            set
            {
                _receiptCount = value;
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

        public List<Receipt> Receipts { get; set; }


        public ReceiptCalendarDataPoint()
        {
            _amount = 0;
            _receiptCount = 0;
        }

        public ReceiptCalendarDataPoint(DateTime date)
        {
            _amount = 0;
            _receiptCount = 0;

            _date = date;
        }

        public ReceiptCalendarDataPoint(DateTime date, int amount, int receiptCount, List<Receipt> receipts)
        {
            _amount = amount;
            _receiptCount = receiptCount;

            _date = date;

            Receipts = receipts;
        }

    }
    
}
