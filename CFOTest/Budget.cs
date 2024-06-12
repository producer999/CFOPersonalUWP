using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public class Budget : INotifyPropertyChanged
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
                _label = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
            }
        }

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                RemainingAmount = _amount - GetReceiptsTotal();
                // Did not throw PropertyChanged becasue for now the only way to change the display of Amount is manually typing it
            }
        }

        private int _month;
        public int Month
        {
            get { return _month; }
            set
            {
                _month = value;
            }
        }

        private int _year;
        public int Year
        {
            get { return _year; }
            set
            {
                _year = value;
            }
        }

        //
        // SPECIFIC PROPERTIES
        //

        private int _remainingAmount;
        public int RemainingAmount
        {
            get
            {
                return _remainingAmount;
            }
            private set
            {
                _remainingAmount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RemainingAmount"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ReceiptsTotal"));
            }
        }

        [SQLite.Ignore]
        public double ReceiptsTotal
        {
            get
            {
                int total = GetReceiptsTotal();

                double result = (double)total / 100;
                result = Math.Round(result, 2);

                return result;
            }
        }

        private bool _isClosed;
        public bool IsClosed
        {
            get { return _isClosed; }
            set
            {
                _isClosed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsClosed"));
            }
        }

        //
        // FOREIGN KEYS
        //

        private int _typeId;
        [SQLiteNetExtensions.Attributes.ForeignKey(typeof(BudgetTypeObject))]
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

        private ObservableCollection<Receipt> _receipts;
        [SQLiteNetExtensions.Attributes.OneToMany(CascadeOperations = SQLiteNetExtensions.Attributes.CascadeOperation.CascadeInsert | SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead | SQLiteNetExtensions.Attributes.CascadeOperation.CascadeDelete)]
        public ObservableCollection<Receipt> Receipts
        {
            get { return _receipts; }
            set
            {
                if(_receipts != value)
                {
                    _receipts = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Receipts"));
                }            
            }
        }

        [SQLiteNetExtensions.Attributes.ManyToOne(CascadeOperations = SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead)]
        public BudgetTypeObject Type { get; set; }

        //
        // CONSTRUCTORS
        //

        public Budget()
        {
            
        }

        //
        // MAIN CONSTRUCTOR
        //

        public Budget(string Label, int Amount, BudgetTypeObject Type, int Month, int Year)
        {

            _label = Label;
            _amount = Amount;
            _remainingAmount = Amount;

            this.Type = Type;

            _month = Month;
            _year = Year;
            _isClosed = false;
            _isInEditMode = false;

            Receipts = new ObservableCollection<Receipt>();
        }

        // Make this the main method for adding recipts -yes
        public Receipt AddReceipt(string label, int amount, DateTime transactionDate, ReceiptTypeObject type, Payee payee)
        {
            Receipt r = new Receipt(label, amount, transactionDate, this, type, payee);
            DBHelper.Insert(r);

            // BOTTLENECK
            //type.Refresh();
            //payee.Refresh();
            //this.Refresh();

            //type.AddReceipt(r);
            //payee.AddObject(r);
            //this.AddReceipt(r);

            if (type != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.ReceiptTypes for the new category
                int newTypeIndex = ItemTypes.ReceiptTypes.IndexOf(type);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.ReceiptTypes[newTypeIndex].Receipts.Add(r);
                }
            }

            if (payee != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.Payees for the new payee
                int newPayeeIndex = ItemTypes.Payees.IndexOf(payee);
                if (newPayeeIndex >= 0)
                {
                    ItemTypes.Payees[newPayeeIndex].Receipts.Add(r);
                }
            }

            if (Receipts != null)
            {
                Receipts.Add(r);
            }

            RemainingAmount -= r.Amount;
            DBHelper.Update(this);

            return r;
        }

        public void UpdateReceipt(Receipt receipt)
        {
            ReceiptTypeObject typeObj = receipt.Type;
            Payee payeeObj = receipt.Payee;

            //Receipt hasnt been updated in the database yet, so getting the amount in DB will still be the previous amount
            // BOTTLENECK
            // Changed to non-recursive GetById to speed up
            int PrevAmount = (DBHelper.GetReceiptById(receipt.Id, false)).Amount;

            int NewAmount = receipt.Amount;

            RemainingAmount -= (NewAmount - PrevAmount);

            //Must manually update the receipt amount in the ItemTypes.ReceiptTypes list to ensure correct chart updating
            if(typeObj != null)
            {
                int typeIndex = ItemTypes.ReceiptTypes.IndexOf(typeObj);
                if (typeIndex >= 0)
                {
                    Receipt r = ItemTypes.ReceiptTypes[typeIndex].Receipts.Where(x => x.Id == receipt.Id).First();

                    r.Amount = receipt.Amount;
                }
            }

            //Must manually update the receipt amount in the ItemTypes.Payees list to ensure correct chart updating
            if (payeeObj != null)
            {
                int payeeIndex = ItemTypes.Payees.IndexOf(payeeObj);
                if (payeeIndex >= 0)
                {
                    Receipt r = ItemTypes.Payees[payeeIndex].Receipts.Where(x => x.Id == receipt.Id).First();

                    r.Amount = receipt.Amount;
                }
            }

            DBHelper.Update(receipt);
            DBHelper.Update(this);
        }

        public void DeleteReceipt(Receipt receipt)
        {
            if (Receipts != null)
            {
                if (Receipts.Count > 0)
                {
                    ReceiptTypeObject prevType = receipt.Type;
                    Payee prevPayee = receipt.Payee;

                    RemainingAmount += receipt.Amount;

                    receipt.BudgetId = 0;
                    receipt.Budget = null;
                    receipt.BudgetMonth = 0;
                    receipt.BudgetYear = 0;
                    DBHelper.Update(receipt);

                    if (prevType != null)
                    {
                        //prevType.RemoveReceipt(receipt);

                        // Must manually remove receipt from ItemTypes.ReceiptTypes for the old category
                        int typeIndex = ItemTypes.ReceiptTypes.IndexOf(prevType);
                        if (typeIndex >= 0)
                        {
                            ItemTypes.ReceiptTypes[typeIndex].Receipts.Remove(receipt);
                        }
                    }

                    if (prevPayee != null)
                    {
                        //prevPayee.RemoveObject(receipt);

                        // Must manually remove receipt from ItemTypes.Payees for the old payee
                        int payeeIndex = ItemTypes.Payees.IndexOf(prevPayee);
                        if (payeeIndex >= 0)
                        {
                            ItemTypes.Payees[payeeIndex].Receipts.Remove(receipt);
                        }
                    }

                    this.RemoveReceipt(receipt);

                    DBHelper.Update(this);
                }
            }
        }

        private int GetReceiptsTotal()
        {
            int total = 0;

            if (Receipts != null && Receipts.Count != 0)
            {
                foreach (Receipt rec in Receipts)
                {
                    total += rec.Amount;
                }

                return total;
            }

            return total;
        }

        public void Refresh()
        {
            Receipts = DBHelper.GetBudgetById(Id).Receipts;
        }

        public void AddReceipt(Receipt r)
        {
            if(Receipts != null)
            {
                Receipts.Add(r);
            }
        }

        public void RemoveReceipt(Receipt r)
        {
            if(Receipts != null && Receipts.Any(rec => rec.Id == r.Id))
            {
                Receipts.Remove(r);
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

            Budget b = (Budget)obj;
            return this.Id == b.Id;
        }

        public override int GetHashCode()
        {
            return Id ^ 7;
        }

    }
}
