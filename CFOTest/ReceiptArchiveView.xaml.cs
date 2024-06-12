using Syncfusion.Pdf;
using Syncfusion.Pdf.Grid;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CFOTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReceiptArchiveView : Page
    {
        public ReceiptArchive Archive { get; set; }

        private string _currentSearchFilter;

        private GridRowSizingOptions _gridRowResizingOptions = new GridRowSizingOptions();
        private double _autoHeight = double.NaN;

        private static List<String> _excludeColumns = new List<string>() { "ImageUrl", "Label", "Amount", "TransactionDate",
                                                                            "Budget.Label", "Payee.Label", "Type",
                                                                            "CreationDate" };

        public ReceiptArchiveView()
        {
            this.InitializeComponent();

            Archive = new ReceiptArchive();

            _gridRowResizingOptions.ExcludeColumns = _excludeColumns;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Archive.IsInitialPageLoad == true)
            {
                Archive.IsInitialPageLoad = false;
            }
            else
            {
                Archive.RefreshData();
                if(DeletedCheckBox.IsChecked == true)
                {
                    Archive.ShowAllReceipts();
                }
                else
                {
                    Archive.ShowActiveReceipts();
                }
            }

            if(dataGrid.View != null)
            {
                Archive.CurrentViewCount = dataGrid.View.Count;
            }
        }

        //
        // GRID DISPLAY METHODS
        //

        private void ReceiptDataGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                var cell = e.AddedItems.First() as GridCellInfo;
                if (cell.Column.MappingName == "ImageUrl")
                {
                    Archive.CurrentReceipt = cell.RowData as Receipt;

                    if (!ReceiptEditorImageLargePopup.IsOpen)
                        ReceiptEditorImageLargePopup.IsOpen = true;
                }
            }
        }

        private void ReceiptDataGrid_QueryRowHeight(object sender, QueryRowHeightEventArgs e)
        {
            if (this.dataGrid.GridColumnSizer.GetAutoRowHeight(e.RowIndex, _gridRowResizingOptions, out _autoHeight))
            {
                if (_autoHeight > 45)
                {
                    e.Height = _autoHeight;
                    e.Handled = true;
                }
            }
        }

        //
        // DATA SEARCH METHODS
        //

        private void ReceiptSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            _currentSearchFilter = sender.Text;
            dataGrid.View.Filter = SearchFilter;
            dataGrid.View.RefreshFilter();

            Archive.CurrentViewCount =  dataGrid.View.Count;
        }

        public bool SearchFilter(Object o)
        {
            var receipt = o as Receipt;

            if (OcrCheckBox.IsChecked == true && receipt.OcrString != null)
            {
                if (receipt.Notes != null && receipt.Budget != null) // To search an active receipt with notes
                {
                    if (receipt.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Budget.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Payee.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Type.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Notes.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.OcrString.ToLower().Contains(_currentSearchFilter.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (receipt.Notes != null && receipt.Budget == null) // To search a deleted receipt with notes
                {
                    if (receipt.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Payee.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Type.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Notes.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.OcrString.ToLower().Contains(_currentSearchFilter.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (receipt.Notes == null && receipt.Budget != null) // To search an active receipt without notes
                {
                    if (receipt.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Budget.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Payee.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Type.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.OcrString.ToLower().Contains(_currentSearchFilter.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (receipt.Notes == null && receipt.Budget == null) // To search a deleted receipt without notes
                {
                    if (receipt.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Payee.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Type.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.OcrString.ToLower().Contains(_currentSearchFilter.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (receipt.Notes != null && receipt.Budget != null) // To search an active receipt with notes
                {
                    if (receipt.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Budget.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Payee.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Notes.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Type.Label.ToLower().Contains(_currentSearchFilter.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (receipt.Notes != null && receipt.Budget == null) // To search a deleted receipt with notes
                {
                    if (receipt.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Payee.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Notes.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Type.Label.ToLower().Contains(_currentSearchFilter.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (receipt.Notes == null && receipt.Budget != null) // To search an active receipt without notes
                {
                    if (receipt.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Budget.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Payee.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Type.Label.ToLower().Contains(_currentSearchFilter.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (receipt.Notes == null && receipt.Budget == null) // To search a deleted receipt without notes
                {
                    if (receipt.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Payee.Label.ToLower().Contains(_currentSearchFilter.ToLower()) ||
                        receipt.Type.Label.ToLower().Contains(_currentSearchFilter.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            throw new Exception("Something went wrong while searching, it shouldn't reach here"); // Something went wrong, it shouldn't reach here

        }

        //
        // HELPER METHODS
        //

        private void ShowDeletedReceipts_Checked(object sender, RoutedEventArgs e)
        {
            Archive.IsReceiptListLoading = true;
            Archive.ShowAllReceipts();
            Archive.IsReceiptListLoading = false;

            Archive.RefreshCurrentReceiptData();
            Archive.CurrentViewCount = dataGrid.View.Count;
        }

        private void ShowDeletedReceipts_Unchecked(object sender, RoutedEventArgs e)
        {
            Archive.IsReceiptListLoading = true;
            Archive.ShowActiveReceipts();
            Archive.IsReceiptListLoading = false;

            Archive.RefreshCurrentReceiptData();
            Archive.CurrentViewCount = dataGrid.View.Count;
        }

        private void ReceiptImageCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReceiptEditorImageLargePopup.IsOpen)
                ReceiptEditorImageLargePopup.IsOpen = false;

            dataGrid.SelectedItem = null;
        }

        private void ReceiptImagePopup_Closed(object sender, object e)
        {
            // Reset the zoom level and positioning of the large image popup
            // ScrollViewer.ChangeView() doesn't work
            LargeImagePopupScrollViewer.ZoomToFactor(1.0f);
        }

        //
        // DATA EXPORT METHODS
        //

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileSavePicker picker = new FileSavePicker();
                picker.SuggestedStartLocation = PickerLocationId.Desktop;
                picker.FileTypeChoices.Add("Excel Document", new List<string>() { ".xlsx" });
                picker.FileTypeChoices.Add("PDF Document", new List<string>() { ".pdf" });
                picker.DefaultFileExtension = ".xlsx";
                picker.SuggestedFileName = "CFO-ReceiptData";

                StorageFile newFile = await picker.PickSaveFileAsync();

                if(newFile != null)
                {
                    if(newFile.FileType == ".xlsx")
                    {
                        await ExportGridViewToXlsxAsync(newFile);
                    }
                    else if(newFile.FileType == ".pdf")
                    {
                        await ExportGridViewToPdfAsync(newFile);
                    }
                }
            }
            catch
            {
                
            }
        }

        private async Task ExportGridViewToXlsxAsync(StorageFile file)
        {
            var options = new ExcelExportingOptions();
            options.ExportMode = ExportMode.Text;
            options.ExcludeColumns.Add("ImageUrl");

            var excelEngine = dataGrid.ExportToExcel(dataGrid.View, options);

            IWorkbook workBook = excelEngine.Excel.Workbooks[0];

            await workBook.SaveAsAsync(file);

            workBook.Close();
            excelEngine.Dispose();
        }

        private async Task ExportGridViewToPdfAsync(StorageFile file)
        {
            var options = new PdfExportingOptions();

            var document = new PdfDocument();
            document.PageSettings.Orientation = PdfPageOrientation.Landscape;

            var page = document.Pages.Add();

            var pdfGrid = dataGrid.ExportToPdfGrid(dataGrid.View, options);

            var format = new PdfGridLayoutFormat()
            {
                Layout = Syncfusion.Pdf.Graphics.PdfLayoutType.Paginate,
                Break = Syncfusion.Pdf.Graphics.PdfLayoutBreakType.FitPage
            };

            pdfGrid.Draw(page, new PointF(), format);

            await document.SaveAsync(file);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Archive.IsReceiptListLoading = true;
            Archive.RefreshData();

            if(DeletedCheckBox.IsChecked == true)
            {
                DeletedCheckBox.IsChecked = false;
            }
            else
            {
                Archive.ShowActiveReceipts();
            }

            Archive.IsReceiptListLoading = false;

            if (dataGrid.View != null)
            {
                Archive.CurrentViewCount = dataGrid.View.Count;
            }
        }
    }
}
