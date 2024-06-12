using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public class Income : INotifyPropertyChanged
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
            }
        }

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
            }
        }

        private int _typeId;
        [SQLiteNetExtensions.Attributes.ForeignKey(typeof(IncomeTypeObject))]
        public int TypeId
        {
            get { return _typeId; }
            set
            {
                if(_typeId != value)
                {
                    _typeId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TypeId"));
                }
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
        // SQLITE RELATIONSHIP VARIABLES
        //

        [SQLiteNetExtensions.Attributes.ManyToOne(CascadeOperations = SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead)]
        public IncomeTypeObject Type { get; set; }

        //
        // CONSTRUCTORS
        //

        public Income()
        {
            
        }

        //
        // MAIN CONSTRUCTOR
        //

        public Income(string Label, int Amount, IncomeTypeObject Type, int Month, int Year)
        {
            _label = Label;
            _amount = Amount;

            this.Type = Type;

            _month = Month;
            _year = Year;

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

            Income i = (Income)obj;
            return this.Id == i.Id;
        }

        public override int GetHashCode()
        {
            return Id ^ 7;
        }
    }
}
