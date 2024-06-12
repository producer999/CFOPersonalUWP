using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFOTest
{
    public class ReceiptArchive : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Receipt> CurrentReceiptData { get; set; }
        public ObservableCollection<Receipt> AllReceipts { get; set; }
        public ObservableCollection<Receipt> ActiveReceipts { get; set; }

        //public List<Receipt> CurrentReceiptData { get; set; }
        //public List<Receipt> AllReceipts { get; set; }
        //public List<Receipt> ActiveReceipts { get; set; }

        private Receipt _currentReceipt;
        public Receipt CurrentReceipt
        {
            get { return _currentReceipt; }
            set
            {
                //if(value != null)
                //{
                if (_currentReceipt != value)
                {
                    _currentReceipt = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentReceipt"));
                }
                //}
            }
        }

        private int _currentViewCount;
        public int CurrentViewCount
        {
            get { return _currentViewCount; }
            set
            {
                if(_currentViewCount != value)
                {
                    _currentViewCount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentViewCount"));
                }
            }
        }

        private bool _isReceiptListLoading;
        public bool IsReceiptListLoading
        {
            get { return _isReceiptListLoading; }
            set
            {
                _isReceiptListLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsReceiptListLoading"));
            }
        }

        public bool IsInitialPageLoad { get; set; }

        public ReceiptArchive()
        {
            _isReceiptListLoading = false;
            _currentReceipt = null;

            CurrentReceiptData = new ObservableCollection<Receipt>();

            AllReceipts = DBHelper.GetAll<Receipt>(false);
            ActiveReceipts = new ObservableCollection<Receipt>(AllReceipts.Where(r => r.BudgetId != 0).ToList());

            foreach(Receipt r in ActiveReceipts)
            {
                CurrentReceiptData.Add(r);
            }

            RefreshCurrentReceiptData();
            CurrentViewCount = CurrentReceiptData.Count;
        }

        public void RefreshCurrentReceiptData()
        { 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentReceiptData"));
        }

        public void ShowAllReceipts()
        {
            if (CurrentReceiptData != null)
            {
                CurrentReceiptData.Clear();
                foreach (Receipt r in AllReceipts)
                {
                    CurrentReceiptData.Add(r);
                }
            }
            else
            {
                CurrentReceiptData = AllReceipts;
            }
        }

        public void ShowActiveReceipts()
        {
            if (CurrentReceiptData != null)
            {
                CurrentReceiptData.Clear();
                foreach (Receipt r in ActiveReceipts)
                {
                    CurrentReceiptData.Add(r);
                }
            }
            else
            {
                CurrentReceiptData = ActiveReceipts;
            }
        }

        public void RefreshData()
        {
            CurrentReceipt = null;

            ObservableCollection<Receipt> tempAllReceipts = DBHelper.GetAll<Receipt>(false);
            List<Receipt> tempActiveReceipts = tempAllReceipts.Where(r => r.BudgetId != 0).ToList();

            AllReceipts.Clear();

            foreach(Receipt r in tempAllReceipts)
            {
                AllReceipts.Add(r);
            }

            ActiveReceipts.Clear();

            foreach(Receipt r in tempActiveReceipts)
            {
                ActiveReceipts.Add(r);
            }

            RefreshCurrentReceiptData();
        }
        
    }
}
