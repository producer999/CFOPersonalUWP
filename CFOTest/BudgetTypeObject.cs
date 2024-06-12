using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public class BudgetTypeObject
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

        //
        // SQLITE RELATIONSHIP VARIABLES
        //

        [SQLiteNetExtensions.Attributes.OneToMany(CascadeOperations = SQLiteNetExtensions.Attributes.CascadeOperation.CascadeInsert | SQLiteNetExtensions.Attributes.CascadeOperation.CascadeRead)]
        public ObservableCollection<Budget> Budgets { get; set; }

        public BudgetTypeObject()
        {

        }

        public BudgetTypeObject(string Label)
        {
            _label = Label;
            Budgets = new ObservableCollection<Budget>();
        }

        public BudgetTypeObject(BudgetTypes type)
        {
            _label = Enum.GetName(typeof(BudgetTypes), type);
            Budgets = new ObservableCollection<Budget>();
        }

        public void Refresh()
        {
            Budgets = DBHelper.GetBudgetTypeByName(Label).Budgets;
        }

        public void AddBudget(Budget b)
        {
            if(Budgets != null)
            {
                Budgets.Add(b);
            }
        }
        public void RemoveBudget(Budget b)
        {
            if (Budgets != null && Budgets.Any(bud => bud.Id == b.Id))
            {
                Budgets.Remove(b);
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

            BudgetTypeObject type = (BudgetTypeObject)obj;
            return this.Id == type.Id;
        }

        public override int GetHashCode()
        {
            return Id ^ 7;
        }
    }
}
