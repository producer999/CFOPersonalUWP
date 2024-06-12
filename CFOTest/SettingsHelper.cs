using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace CFOTest
{
    public class SettingsHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //Local Settings Object for this App
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        //Local Settings Object for Object Data
        private static LocalObjectStorageHelper localObjectSettings = new LocalObjectStorageHelper();

        //
        // BOUND APP SETTINGS
        //

        public const string IsLicenseActiveToken = "IsLicenseActiveStatus";

        public bool IsLicenseActive
        {
            get
            {
                if (localObjectSettings.KeyExists(IsLicenseActiveToken))
                {
                    return (bool)ReadLocalSetting(IsLicenseActiveToken);
                }
                else
                {
                    return false;
                }
            }
            private set
            {
                SaveLocalObjectSetting(IsLicenseActiveToken, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLicenseActive"));
            }
        }

        public const string IsLicenseTrialToken = "IsLicenseTrialStatus";

        public bool IsLicenseTrial
        {
            get
            {
                if (localObjectSettings.KeyExists(IsLicenseTrialToken))
                {
                    return (bool)ReadLocalSetting(IsLicenseTrialToken);
                }
                else
                {
                    return true;
                }
            }
            set
            {
                SaveLocalObjectSetting(IsLicenseTrialToken, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLicenseTrial"));
            }
        }

        public const string TrialExpirationDateToken = "TrialExpirationDate";

        public DateTimeOffset TrialExpirationDate
        {
            get
            {
                if (localObjectSettings.KeyExists(TrialExpirationDateToken))
                {
                    return (DateTime)ReadLocalSetting(TrialExpirationDateToken);
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            private set
            {
                SaveLocalObjectSetting(TrialExpirationDateToken, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TrialExpirationDate"));
            }
        }

        public const string IsTrialExpiredToken = "IsTrialExpiredStatus";

        public bool IsTrialExpired
        {
            get
            {
                if (localObjectSettings.KeyExists(IsTrialExpiredToken))
                {
                    return (bool)ReadLocalSetting(IsTrialExpiredToken);
                }
                else
                {
                    return true;
                }
            }
            private set
            {
                SaveLocalObjectSetting(IsTrialExpiredToken, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsTrialExpired"));
            }
        }

        public const string PurchaseDateToken = "PurchaseDate";

        public DateTime PurchaseDate
        {
            get
            {
                if (localObjectSettings.KeyExists(PurchaseDateToken))
                {
                    return localObjectSettings.Read<DateTime>(PurchaseDateToken);
                }
                else
                {
                    return DateTime.MaxValue;
                }
            }
            set
            {
                localObjectSettings.Save(PurchaseDateToken, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PurchaseDate"));
            }
        }

        public const string ReceiptBackupFolderToken = "ReceiptBackupFolder";

        /// <summary>
        /// Path to the backup folder for all Receipt images
        /// </summary>
        public string ReceiptBackupFolder
        {
            get
            {
                if (localObjectSettings.KeyExists(ReceiptBackupFolderToken))
                {
                    return localObjectSettings.Read<string>(ReceiptBackupFolderToken);
                }
                else
                {
                    return "Not Set";
                }
            }
            set
            {
                localObjectSettings.Save(ReceiptBackupFolderToken, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ReceiptBackupFolder"));
            }
        }

        private bool _isReceiptFolderSet;
        public bool IsReceiptFolderSet
        {
            get
            {
                if (localObjectSettings.KeyExists(ReceiptBackupFolderToken))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            private set
            {
                _isReceiptFolderSet = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsReceiptFolderSet"));
            }
        }
        

        public const string DisableConfirmationDialogToken = "DisableConfirmationDialogs";

        /// <summary>
        /// If set to true, there will be no confirmation popup when deleting a list item
        /// </summary>
        public bool DisableConfirmationDialogs
        {
            get
            {
                if (localObjectSettings.KeyExists(DisableConfirmationDialogToken))
                {
                    return localObjectSettings.Read<bool>(DisableConfirmationDialogToken);
                }
                else
                {
                    return false;
                }
            }
            set
            {
                localObjectSettings.Save(DisableConfirmationDialogToken, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisableConfirmationDialogs"));
            }
        }


        public const string AutomaticallyBackupReceiptsToken = "AutomaticallyBackupReceipts";
        /// <summary>
        /// Automatically Backup Receipt Images to the specified backup folder each time App is closed
        /// </summary>
        public bool AutomaticallyBackupReceipts
        {
            get
            {
                if (localObjectSettings.KeyExists(AutomaticallyBackupReceiptsToken))
                {
                    return localObjectSettings.Read<bool>(AutomaticallyBackupReceiptsToken);
                }
                else
                {
                    return false;
                }
            }
            set
            {
                localObjectSettings.Save(AutomaticallyBackupReceiptsToken, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AutomaticallyBackupReceipts"));
            }
        }

        private bool _isAutoBackupReceiptsSet;
        public bool IsAutoBackupReceiptsSet
        {
            get
            {
                if (localObjectSettings.KeyExists(AutomaticallyBackupReceiptsToken))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            private set
            {
                _isAutoBackupReceiptsSet = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsAutoBackupReceiptsSet"));
            }
        }


        public const string LastReceiptBackupDateToken = "LastReceiptBackupDate";
        /// <summary>
        /// Last Date and time the Receipts folder was backed up
        /// </summary>
        public DateTime LastReceiptBackupDate
        {
            get
            {
                if (localObjectSettings.KeyExists(LastReceiptBackupDateToken))
                {
                    return localObjectSettings.Read<DateTime>(LastReceiptBackupDateToken);
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                localObjectSettings.Save(LastReceiptBackupDateToken, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LastReceiptBackupDate"));
            }
        }

        private bool _isLastReceiptBackupDateSet;
        public bool IsLastReceiptBackupDateSet
        {
            get
            {
                if (localObjectSettings.KeyExists(LastReceiptBackupDateToken))
                {
                    if(LastReceiptBackupDate != DateTime.MinValue)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            private set
            {
                _isLastReceiptBackupDateSet = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLastReceiptBackupDateSet"));
            }
        }

        private bool _isReceiptClearInProgress;
        public bool IsReceiptClearInProgress
        {
            get
            {
                return _isReceiptClearInProgress;
            }
            set
            {
                _isReceiptClearInProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsReceiptClearInProgress"));
            }
        }

        //
        // UNBOUND SETTINGS
        //

        public const string IncomeListOrderToken = "IncomeListOrder";
        public const string ExpenseListOrderToken = "ExpenseListOrder";
        public const string BudgetListOrderToken = "BudgetListOrder";

        public const string ReceiptImagePlaceholderPath = "ms-appx:///Assets/receiptblur3.jpg";

        /// <summary>
        /// The start date for the application's views
        /// </summary>
        public static DateTime StartDate = DateTime.Now;

        // Used (temporarily) to hold the current year of the year view, for reinitialization when navigating to the page
        // TODO: Is there a better way?
        //public static int CurrentYear = StartDate.Year;

        //
        // CONSTRUCTOR
        //

        public SettingsHelper()
        {

        }

        //
        // For Simple Settings
        //

        public static void SaveLocalSetting(string key, object value)
        {
            localObjectSettings.Save(key, value);
        }

        public static object ReadLocalSetting(string key)
        {
            //return localSettings.Values[key];

            if (localObjectSettings.KeyExists(key))
            {
                return localObjectSettings.Read<object>(key);
            }
            else
            {
                return null;
            }
        }

        //
        // For Object Settings
        //

        public static void SaveLocalObjectSetting(string key, Object o)
        {
            localObjectSettings.Save(key, o);
        }

        public static List<int> ReadListOrderSetting(string key)
        {
            if(localObjectSettings.KeyExists(key))
            {
                return localObjectSettings.Read<List<int>>(key);
            }
            else
            {
                return null;
            }
        }


        public static async Task<StorageFolder> GetReceiptImageFolder()
        {
            try
            {
                return await ApplicationData.Current.LocalFolder.CreateFolderAsync("Receipts", CreationCollisionOption.OpenIfExists);
            }
            catch
            {
                return null;
            }
        }

        public async void ChooseReceiptBackupFolder()
        {
            StorageFolder receiptBackupFolder;

            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            receiptBackupFolder = await folderPicker.PickSingleFolderAsync();

            if (receiptBackupFolder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)

                StorageApplicationPermissions.FutureAccessList.AddOrReplace(ReceiptBackupFolderToken, receiptBackupFolder);
                ReceiptBackupFolder = receiptBackupFolder.Path;
                IsReceiptFolderSet = true;
            }
            else
            {
            }
        }

        public async Task BackupReceipts()
        {
            StorageFolder imagesFolder = await GetReceiptImageFolder();
            StorageFolder backupFolder = await StorageFolder.GetFolderFromPathAsync(ReceiptBackupFolder);

            try
            {
                await CopyFolderAsync(imagesFolder, backupFolder);

                LastReceiptBackupDate = DateTime.Now;
                IsLastReceiptBackupDateSet = true;
            }
            catch
            {

            }
        }

        public async Task CopyFolderAsync(StorageFolder source, StorageFolder destinationContainer, string desiredName = null)
        {
            StorageFolder destinationFolder = null;
            destinationFolder = await destinationContainer.CreateFolderAsync(
                desiredName ?? source.Name, CreationCollisionOption.ReplaceExisting);

            foreach (var file in await source.GetFilesAsync())
            {
                await file.CopyAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);
            }
            foreach (var folder in await source.GetFoldersAsync())
            {
                await CopyFolderAsync(folder, destinationFolder);
            }
        }

        public static void SetDefaultLocalSettings()
        {
            // Clear all user defined folder paths
            // 1) Clear the Receipt Image Backup Folder
            StorageApplicationPermissions.FutureAccessList.Clear();
            StorageApplicationPermissions.MostRecentlyUsedList.Clear();

            // 2) Clear any list orders that have been saved
            // 3) Clear the user setting for Diable Confirm Delete Dialog
            localSettings.Values.Clear();

            //
            // Set Default Values
            //

            // (default) Disable Confirmation Dialog = false
            SaveLocalSetting(DisableConfirmationDialogToken, false);
            // (default) Automatically Backup Receipts = false
            SaveLocalSetting(AutomaticallyBackupReceiptsToken, false);
            // (default) Last Receipt Folder Backup Date = DateTime.MinValue
            SaveLocalObjectSetting(LastReceiptBackupDateToken, DateTime.MinValue);

        }

        public static void ClearTemporaryFolder()
        {
            ApplicationData.Current.ClearAsync(ApplicationDataLocality.Temporary);
        }

        public void ClearTempFolder()
        {
            ApplicationData.Current.ClearAsync(ApplicationDataLocality.Temporary);
        }

        public static void GetCurrentSettings()
        {
            
        }

        public async Task RemoveDeletedReceiptsAsync()
        {
            DBHelper.RemoveAllDeletedReceipts();
        }

        public void RefreshLicenseInfo()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLicenseActive"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLicenseTrial"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TrialExpirationDate"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsTrialExpired"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PurchaseDate"));
        }
    }
}
