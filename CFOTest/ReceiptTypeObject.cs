using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public class ReceiptTypeObject
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
        public ObservableCollection<Receipt> Receipts { get; set; }

        //
        // CONSTRUCTORS
        //

        public ReceiptTypeObject()
        {

        }

        public ReceiptTypeObject(string Label)
        {
            _label = Label;
            _colorHexString = null;
            Receipts = new ObservableCollection<Receipt>();
        }

        public ReceiptTypeObject(string Label, string ColorHexString)
        {
            _label = Label;
            _colorHexString = ColorHexString;
            Receipts = new ObservableCollection<Receipt>();
        }

        //
        // For Building initial data only
        //

        public ReceiptTypeObject(ReceiptTypes type)
        {
            _label = Enum.GetName(typeof(ReceiptTypes), type);
            _colorHexString = null;
        }

        //
        // For Building initial data only
        //

        public ReceiptTypeObject(ReceiptTypes type, string colorhexstring)
        {
            _label = Enum.GetName(typeof(ReceiptTypes), type);
            _colorHexString = colorhexstring;
        }

        /// <summary>
        /// Deprecated, do not use (very slow)
        /// </summary>
        public void Refresh()
        {
            Receipts = DBHelper.GetReceiptTypeByName(Label).Receipts;
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
            if(ReferenceEquals(obj, null))
            {
                return false;
            }
            if(ReferenceEquals(this, obj))
            {
                return true;
            }

            ReceiptTypeObject type = (ReceiptTypeObject)obj;
            return this.Id == type.Id;
        }

        public override int GetHashCode()
        {
            return Id ^ 7;
        }
    }

    public class ReceiptTypeDataPoint
    {
        public string Label { get; set; }

        public int Amount { get; set; }

        public int ReceiptCount { get; set; }

        public string ColorHexString { get; set; }

        /// <summary>
        /// Data Object representing the total Amount and Count of Receipts with a specific Receipt Type (label) in a given month
        /// </summary>

        public ReceiptTypeDataPoint()
        {

        }
        public ReceiptTypeDataPoint(string label, int amount, int receiptCount, string colorHexString)
        {
            Label = label;
            Amount = amount;
            ReceiptCount = receiptCount;
            ColorHexString = colorHexString;
        }
    }
}
