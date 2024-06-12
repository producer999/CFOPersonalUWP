using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace CFOTest
{
    public class SpecialBudgets : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Budget> BudgetList { get; set; }

        private List<int> BudgetListOrder { get; set; }

        private const string _monthId = "10001";

        private Budget _currentBudget;
        public Budget CurrentBudget
        {
            get { return _currentBudget; }
            set
            {
                if (_currentBudget != value)
                {
                    _currentBudget = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentBudget"));
                }
            }
        }

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

        private bool _isImageLoading;
        public bool IsImageLoading
        {
            get { return _isImageLoading; }
            private set
            {
                _isImageLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsImageLoading"));
            }
        }

        public SpecialBudgets()
        {
            _currentBudget = null;
            _currentReceipt = null;

            //BudgetList = DBHelper.GetBudgetsByMonth(1, 1000);
            BudgetList = new ObservableCollection<Budget>();

            // Initialization here too
            // For keeping track of the state of the list orders as they change
            LoadInitialListOrders();
            BudgetList.CollectionChanged += ListView_CollectionChanged;
        }

        //
        // LIST ORDER METHODS
        //

        private void LoadInitialListOrders()
        {
            BudgetListOrder = SettingsHelper.ReadListOrderSetting(SettingsHelper.BudgetListOrderToken + _monthId);

            if (BudgetListOrder != null && BudgetListOrder.Count > 0)
            {
                BudgetList.Clear();

                List<ReceiptTypeObject> usedTypes = new List<ReceiptTypeObject>();
                List<Payee> usedPayees = new List<Payee>();

                for (int i = 0; i < BudgetListOrder.Count; i++)
                {
                    // BOTTLENECK
                    // Changed to non-recursive
                    // TODO: Fix bc the Budgets need the list of Receipts (OK)
                    //       but also the Payee and Tpe and Budget of the receipts in the list (NOT GETTING LOADED bc non-recursive)
                    //BudgetList.Add(DBHelper.GetBudgetById(BudgetListOrder[i], false));

                    Budget b = DBHelper.GetBudgetById(BudgetListOrder[i], false);
                    if (b.Receipts != null && b.Receipts.Count > 0)
                    {
                        foreach (Receipt r in b.Receipts)
                        {
                            if (usedTypes.Any(rto => rto.Id == r.TypeId))
                            {
                                r.Type = usedTypes.Where(rto => rto.Id == r.TypeId).First();
                            }
                            else
                            {
                                ReceiptTypeObject newRto = DBHelper.GetReceiptTypeById(r.TypeId, false);
                                usedTypes.Add(newRto);
                                r.Type = newRto;
                            }

                            if (usedPayees.Any(p => p.Id == r.PayeeId))
                            {
                                r.Payee = usedPayees.Where(p => p.Id == r.PayeeId).First();
                            }
                            else
                            {
                                Payee newPayee = DBHelper.GetPayeeById(r.PayeeId, false);
                                usedPayees.Add(newPayee);
                                r.Payee = newPayee;
                            }
                        }
                    }

                    BudgetList.Add(b);
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BudgetList"));
            }
            else
            {
                BudgetListOrder = new List<int>();

                ObservableCollection<Budget> tempBudgetList = DBHelper.GetBudgetsByMonth(1, 1000, false);

                if(tempBudgetList != null && tempBudgetList.Count > 0)
                {
                    BudgetList.Clear();
                    foreach (Budget b in tempBudgetList)
                    {
                        BudgetList.Add(b);
                    }
                }
            }
        }

        private void RefreshBudgetListOrder()
        {
            BudgetListOrder.Clear();

            if (BudgetList != null)
            {
                foreach (Budget b in BudgetList)
                {
                    BudgetListOrder.Add(b.Id);
                }
            }

            SettingsHelper.SaveLocalObjectSetting(SettingsHelper.BudgetListOrderToken + _monthId, BudgetListOrder);
        }

        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<Budget>)
            {
                RefreshBudgetListOrder();
            }
        }

        public void SortBudgetList(BudgetSortOptions sortOption)
        {
            ObservableCollection<Budget> temp = new ObservableCollection<Budget>();
            int currentBudgetId;

            if (CurrentBudget != null)
            {
                currentBudgetId = CurrentBudget.Id;
            }
            else
            {
                currentBudgetId = -1;
            }

            switch (sortOption)
            {
                case BudgetSortOptions.ByLabel:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderBy(key => key.Label));
                    goto default;
                case BudgetSortOptions.ByAmount:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderByDescending(key => key.Amount));
                    goto default;
                case BudgetSortOptions.ByRemainingAmount:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderByDescending(key => key.RemainingAmount));
                    goto default;
                case BudgetSortOptions.ByCategory:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderBy(key => key.Type.Label));
                    goto default;
                case BudgetSortOptions.ByClosedStatus:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderBy(key => key.IsClosed));
                    goto default;
                case BudgetSortOptions.ByDateAdded:
                    temp = new ObservableCollection<Budget>(BudgetList.OrderBy(key => key.Id));
                    goto default;
                default:
                    BudgetList.Clear();
                    foreach (Budget b in temp)
                    {
                        BudgetList.Add(b);

                        if (currentBudgetId != -1 && b.Id == currentBudgetId)
                        {
                            CurrentBudget = b;
                        }
                    }
                    break;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BudgetList"));
        }

        // May introduce problems with receipts in the database...
        public void SortCurrentReceiptList(ReceiptSortOptions sortOption)
        {
            ObservableCollection<Receipt> temp = new ObservableCollection<Receipt>();

            switch (sortOption)
            {
                case ReceiptSortOptions.ByLabel:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderBy(key => key.Label));
                    goto default;
                case ReceiptSortOptions.ByAmount:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderByDescending(key => key.Amount));
                    goto default;
                case ReceiptSortOptions.ByPayee:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderBy(key => key.Payee.Label));
                    goto default;
                case ReceiptSortOptions.ByCategory:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderBy(key => key.Type.Label));
                    goto default;
                case ReceiptSortOptions.ByTransactionDate:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderBy(key => key.TransactionDate));
                    goto default;
                case ReceiptSortOptions.ByDateAdded:
                    temp = new ObservableCollection<Receipt>(CurrentBudget.Receipts.OrderBy(key => key.CreationDate));
                    goto default;
                default:
                    CurrentBudget.Receipts.Clear();
                    foreach (Receipt r in temp)
                    {
                        CurrentBudget.Receipts.Add(r);
                    }
                    break;
            }
        }

        public List<BudgetSortOptions> GetBudgetSortOptions()
        {
            return SortOptions.GetBudgetSortOptionsList();
        }

        //
        // METHODS FOR ADDING AND UPDATING DATA
        //

        public void AddBudget(string Label, int Amount, string Type)
        {
            BudgetTypeObject TypeObj;

            if (ItemTypes.BudgetTypes.Any(t => t.Label == Type)) // This will pretty much always be the case for new item since "Undefined" is se
            {
                TypeObj = ItemTypes.BudgetTypes.Where(t => t.Label == Type).ToList().First();
            }
            else
            {
                TypeObj = new BudgetTypeObject(Type);
                ItemTypes.AddBudgetType(TypeObj);
            }

            //Special Budgets all have the year 01/1000 so they don't conflict with Month View Budgets
            Budget budget = new Budget(Label, Amount, TypeObj, 1, 1000);

            // The order of the next two lines is important
            DBHelper.Insert(budget);
            BudgetList.Insert(0, budget);

            // BOTTLENECK
            //TypeObj.Refresh();

            if (TypeObj != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.BudgetTypes for the new category
                int newTypeIndex = ItemTypes.BudgetTypes.IndexOf(TypeObj);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.BudgetTypes[newTypeIndex].Budgets.Add(budget);
                }
            }
            
        }

        public void AddReceipt(string Label, int Amount, DateTime TransactionDate, Budget Budget, string Type, string Payee)
        {
            ReceiptTypeObject TypeObj;
            Payee PayeeObj;

            if (ItemTypes.ReceiptTypes.Any(t => t.Label == Type)) // This will pretty much always be the case for new item since "Undefined" is set
            {
                TypeObj = ItemTypes.ReceiptTypes.Where(t => t.Label == Type).ToList().First();
            }
            else
            {
                TypeObj = new ReceiptTypeObject(Type);
                ItemTypes.AddReceiptType(TypeObj);
            }
            if (ItemTypes.Payees.Any(p => p.Label == Payee)) // This will pretty much always be the case for new item since "Undefined" is set
            {
                PayeeObj = ItemTypes.Payees.Where(p => p.Label == Payee).ToList().First();
            }
            else
            {
                PayeeObj = new Payee(Payee);
                ItemTypes.AddPayee(PayeeObj);
            }

            if (CurrentBudget != null)
            {
                var newReceipt = CurrentBudget.AddReceipt(Label, Amount, TransactionDate, TypeObj, PayeeObj);
            }
        }

        public async void AddReceiptFromCamera()
        {
            string prevImagePath = CurrentReceipt.ImageUrl;

            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;

            StorageFile photo = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Guid.NewGuid().ToString() + ".jpg");

            photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                // User cancelled photo capture, revert the image path
                CurrentReceipt.ImageUrl = prevImagePath;
                return;
            }
            else
            {
                IsImageLoading = true;

                StorageFile oldImageFile = null;

                // If Receipt has an image, grab its file
                if (CurrentReceipt.ImageUrl != null)
                {
                    oldImageFile = await StorageFile.GetFileFromPathAsync(CurrentReceipt.ImageUrl);
                }

                // If Receipt currently has an image file a new file with a unique name will be created
                photo = await GetFileForReceiptImageAsync(photo);

                // Delete the previous receipt image file, if there was one
                if (oldImageFile != null)
                {
                    await oldImageFile.DeleteAsync();
                }

                CurrentReceipt.OcrString = await GetOcrFromImageFile(photo);
                CurrentReceipt.ImageUrl = photo.Path;

                UpdateItem(CurrentReceipt);

                IsImageLoading = false;
            }
        }

        public async void AddReceiptFromFile()
        {
            FileOpenPicker filePicker = new FileOpenPicker();

            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".gif");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".pdf");

            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.Desktop;

            StorageFile file = await filePicker.PickSingleFileAsync();
            StorageFile oldImageFile = null;

            if (file != null)
            {
                IsImageLoading = true;

                switch (file.FileType)
                {
                    case ".pdf":
                        await CreateImageUriFromPdfAsync(file);
                        break;
                    default:
                        // If Receipt has an image, grab its file
                        if (CurrentReceipt.ImageUrl != null)
                        {
                            oldImageFile = await StorageFile.GetFileFromPathAsync(CurrentReceipt.ImageUrl);
                        }

                        // If Receipt currently has an image file a new file with a unique name will be created
                        file = await GetFileForReceiptImageAsync(file);

                        // Delete the previous receipt image file, if there was one
                        if (oldImageFile != null)
                        {
                            await oldImageFile.DeleteAsync();
                        }

                        CurrentReceipt.OcrString = await GetOcrFromImageFile(file);
                        CurrentReceipt.ImageUrl = file.Path;

                        UpdateItem(CurrentReceipt);
                        break;
                }

                IsImageLoading = false;
            }
        }

        public void CopyItemToThisMonth(object item)
        {
            if (item is Budget b)
            {
                AddBudget(b.Label, b.Amount, b.Type.Label);
            }
            else if (item is Receipt r)
            {
                AddReceipt(r.Label, r.Amount, DateTime.Now, r.Budget, r.Type.Label, r.Payee.Label);
            }
        }

        public void UpdateBudgetType(Budget budget, BudgetTypeObject newType)
        {
            BudgetTypeObject prevType = budget.Type;

            budget.Type = newType;

            if (newType != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.BudgetTypes for the new category
                int newTypeIndex = ItemTypes.BudgetTypes.IndexOf(newType);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.BudgetTypes[newTypeIndex].Budgets.Add(budget);
                }
            }

            if (prevType != null)
            {
                // Must manually remove receipt from ItemTypes.BudgetTypes for the old category
                int typeIndex = ItemTypes.BudgetTypes.IndexOf(prevType);
                if (typeIndex >= 0)
                {
                    ItemTypes.BudgetTypes[typeIndex].Budgets.Remove(budget);
                }
            }

            UpdateItem(budget);
        }

        public void UpdateReceiptType(Receipt receipt, ReceiptTypeObject newType)
        {
            ReceiptTypeObject prevType = receipt.Type;

            receipt.Type = newType;

            if (newType != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.ReceiptTypes for the new category
                int newTypeIndex = ItemTypes.ReceiptTypes.IndexOf(newType);
                if (newTypeIndex >= 0)
                {
                    ItemTypes.ReceiptTypes[newTypeIndex].Receipts.Add(receipt);
                }
            }

            if (prevType != null)
            {
                // Must manually remove receipt from ItemTypes.ReceiptTypes for the old category
                int typeIndex = ItemTypes.ReceiptTypes.IndexOf(prevType);
                if (typeIndex >= 0)
                {
                    ItemTypes.ReceiptTypes[typeIndex].Receipts.Remove(receipt);
                }
            }

            UpdateItem(receipt);
        }

        public void UpdateReceiptPayee(Receipt receipt, Payee newPayee)
        {
            Payee prevPayee = receipt.Payee;

            receipt.Payee = newPayee;

            if (newPayee != null)
            {
                // Items are no longer recursively pulled from the database, therefore
                // must manually add expense to ItemTypes.Payees for the new payee
                int newPayeeIndex = ItemTypes.Payees.IndexOf(newPayee);
                if (newPayeeIndex >= 0)
                {
                    ItemTypes.Payees[newPayeeIndex].Receipts.Add(receipt);
                }
            }

            if (prevPayee != null)
            {
                // Must manually remove receipt from ItemTypes.Payees for the old payee
                int payeeIndex = ItemTypes.Payees.IndexOf(prevPayee);
                if (payeeIndex >= 0)
                {
                    ItemTypes.Payees[payeeIndex].Receipts.Remove(receipt);
                }
            }

            UpdateItem(receipt);
        }

        // Is this even used? Future Use?
        //public void UpdateReceiptBudget(Receipt receipt, Budget newBudget)
        //{
        //    Budget prevBudget = receipt.Budget;

        //    receipt.Budget = newBudget;

        //    UpdateItem(receipt);

        //    newBudget.Refresh();
        //    if (prevBudget != null)
        //    {
        //        prevBudget.Refresh();
        //    }
        //}

        //
        // DATABASE MANIPULATION METHODS
        //

        public void UpdateItem(Object o)
        {  
            if (o is Budget)
            {
                Budget budget = o as Budget;

                DBHelper.Update(budget);
            }
            if (o is Receipt receipt)
            {
                CurrentBudget.UpdateReceipt(receipt);
            }
        }

        public void DeleteItem(Object o)
        {
            if (o is Budget)
            {
                Budget budget = o as Budget;
                BudgetTypeObject prevType = budget.Type;

                // BOTTLENECK
                foreach (Receipt rec in budget.Receipts)
                {
                    rec.DisableNotifications = true;

                    // Zero out the BudgetId for this budget's receipts but don't delete them, for archiving purposes
                    rec.BudgetId = 0;
                    rec.Budget = null;
                    rec.BudgetMonth = 0;
                    rec.BudgetYear = 0;

                    ReceiptTypeObject recType = rec.Type;
                    Payee recPayee = rec.Payee;

                    rec.DisableNotifications = false;

                    DBHelper.Update(rec);

                    if (recType != null)
                    {
                        // Must manually remove receipt from ItemTypes.ReceiptTypes for the old category
                        int typeIndex = ItemTypes.ReceiptTypes.IndexOf(recType);
                        if (typeIndex >= 0)
                        {
                            ItemTypes.ReceiptTypes[typeIndex].Receipts.Remove(rec);
                        }
                    }

                    if (recPayee != null)
                    {
                        //prevPayee.RemoveObject(receipt);

                        // Must manually remove receipt from ItemTypes.Payees for the old payee
                        int payeeIndex = ItemTypes.Payees.IndexOf(recPayee);
                        if (payeeIndex >= 0)
                        {
                            ItemTypes.Payees[payeeIndex].Receipts.Remove(rec);
                        }
                    }
                }

                if (budget.Receipts != null)
                {
                    budget.Receipts.Clear();
                }

                BudgetList.Remove(budget);
                DBHelper.Delete(budget);

                // BOTTLENECK
                //prevType.Refresh();

                if (prevType != null)
                {
                    int typeIndex = ItemTypes.BudgetTypes.IndexOf(prevType);
                    if (typeIndex >= 0)
                    {
                        ItemTypes.BudgetTypes[typeIndex].Budgets.Remove(budget);
                    }
                }
            }
            else if (o is Receipt)
            {
                Receipt receipt = o as Receipt;
                
                CurrentBudget.DeleteReceipt(receipt);
            }
        }

        public async Task<StorageFolder> GetReceiptImageFolder()
        {
            return await SettingsHelper.GetReceiptImageFolder();
        }

        //
        // PRIVATE WORKER METHODS
        //

        private async Task<bool> CreateImageUriFromPdfAsync(StorageFile pdfFile)
        {
            try
            {
                PdfDocument pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile); ;
                if (pdfDocument != null && pdfDocument.PageCount > 0)
                {
                    for (int pageIndex = 0; pageIndex < 1; pageIndex++) //for now, only convert the first page to an image
                    {
                        var pdfPage = pdfDocument.GetPage((uint)pageIndex);
                        if (pdfPage != null)
                        {
                            StorageFile destinationFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Guid.NewGuid().ToString() + ".jpg");
                            StorageFile oldImageFile = null;

                            if (destinationFile != null)
                            {
                                // If Receipt has an image, grab its file
                                if (CurrentReceipt.ImageUrl != null)
                                {
                                    oldImageFile = await StorageFile.GetFileFromPathAsync(CurrentReceipt.ImageUrl);
                                }

                                IRandomAccessStream randomStream = await destinationFile.OpenAsync(FileAccessMode.ReadWrite);
                                PdfPageRenderOptions pdfPageRenderOptions = new PdfPageRenderOptions();
                                pdfPageRenderOptions.DestinationWidth = 300;

                                await pdfPage.RenderToStreamAsync(randomStream, pdfPageRenderOptions);

                                // If Receipt currently has an image file a new file with a unique name will be created
                                destinationFile = await GetFileForReceiptImageAsync(destinationFile);

                                // Delete the previous receipt image file, if there was on
                                if (oldImageFile != null)
                                {
                                    await oldImageFile.DeleteAsync();
                                }

                                CurrentReceipt.OcrString = await GetOcrFromImageFile(destinationFile);
                                CurrentReceipt.ImageUrl = destinationFile.Path;

                                UpdateItem(CurrentReceipt);

                                //clean up
                                await randomStream.FlushAsync();
                                randomStream.Dispose();
                                pdfPage.Dispose();

                                return true;
                            }
                        }
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<StorageFile> GetFileForReceiptImageAsync(StorageFile imageFile)
        {
            string foldernameLabel, filenameMonth, filenameDay, filenameLabel;
            
            filenameMonth = CurrentReceipt.TransactionDate.Month < 10 ? "0" + CurrentReceipt.TransactionDate.Month : "" + CurrentReceipt.TransactionDate.Month;
            filenameDay = CurrentReceipt.TransactionDate.Day < 10 ? "0" + CurrentReceipt.TransactionDate.Day : "" + CurrentReceipt.TransactionDate.Day;

            // Strip spaces, punctuation and symbols from foldername and filename
            foldernameLabel = CurrentBudget.Label.Replace(" ", "");
            foldernameLabel = new string(foldernameLabel.Where(c => !char.IsPunctuation(c)).ToArray());
            foldernameLabel = new string(foldernameLabel.Where(c => !char.IsSymbol(c)).ToArray());

            if (String.IsNullOrWhiteSpace(foldernameLabel))
                foldernameLabel = "Budget";

            filenameLabel = CurrentReceipt.Label.Replace(" ", "");
            filenameLabel = new string(filenameLabel.Where(c => !char.IsPunctuation(c)).ToArray());
            filenameLabel = new string(filenameLabel.Where(c => !char.IsSymbol(c)).ToArray());

            if (String.IsNullOrWhiteSpace(filenameLabel))
                filenameLabel = "Receipt";

            string monthFolderName = "_SpecialBudgets";
            string budgetFolderName = foldernameLabel + "_" + CurrentBudget.Id;
            string filename = CurrentReceipt.TransactionDate.Year + "-" +
                              filenameMonth + "-" + filenameDay + "_" +
                              filenameLabel + "_" + CurrentReceipt.Id + ".jpg";

            StorageFolder destinationFolder = await GetReceiptImageFolder();
            destinationFolder = await destinationFolder.CreateFolderAsync(monthFolderName, CreationCollisionOption.OpenIfExists);
            destinationFolder = await destinationFolder.CreateFolderAsync(budgetFolderName, CreationCollisionOption.OpenIfExists);

            imageFile = await imageFile.CopyAsync(destinationFolder, filename, NameCollisionOption.GenerateUniqueName);

            return imageFile;
        }

        private async Task<string> GetOcrFromImageFile(StorageFile file)
        {
            if (file != null)
            {
                try
                {
                    var randomAccessStream = await file.OpenReadAsync();

                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(randomAccessStream);
                    SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync();

                    OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                    OcrResult ocrResult = await ocrEngine.RecognizeAsync(bitmap);

                    return ocrResult.Text;
                }
                catch
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

    }
}
