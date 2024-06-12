using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public class IncomeTypeObject
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
        public ObservableCollection<Income> Incomes { get; set; }

        public IncomeTypeObject()
        {

        }

        public IncomeTypeObject(string Label)
        {
            _label = Label;
            Incomes = new ObservableCollection<Income>();
        }

        public IncomeTypeObject(IncomeTypes type)
        {
            _label = Enum.GetName(typeof(IncomeTypes), type);
            Incomes = new ObservableCollection<Income>();
        }
        
        public void Refresh()
        {
            Incomes = DBHelper.GetIncomeTypeByName(Label).Incomes;
        }

        public void AddIncome(Income i)
        {
            if (Incomes != null)
            {
                Incomes.Add(i);
            }
        }

        public void RemoveIncome(Income i)
        {
            if (Incomes != null && Incomes.Any(inc => inc.Id == i.Id))
            {
                Incomes.Remove(i);
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

            IncomeTypeObject type = (IncomeTypeObject)obj;
            return this.Id == type.Id;
        }

        public override int GetHashCode()
        {
            return Id ^ 7;
        }
    }

    public class IncomeTypeDataPoint
    {
        public string Label { get; set; }

        public int Amount { get; set; }

        public int IncomeCount { get; set; }

        public IncomeTypeDataPoint()
        {

        }
        public IncomeTypeDataPoint(string label, int amount, int incomeCount)
        {
            Label = label;
            Amount = amount;
            IncomeCount = incomeCount;
        }
    }
}
