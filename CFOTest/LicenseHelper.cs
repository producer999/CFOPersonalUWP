using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace CFOTest
{
    public static class LicenseHelper
    {
        public static LicenseInformation AppLicense;

        public static void InitializeLicense()
        {
            if (AppLicense == null)
            {
                AppLicense = CurrentApp.LicenseInformation;
                //AppLicense = CurrentAppSimulator.LicenseInformation;
            }

            if (AppLicense != null)
            {
                if (AppLicense.IsActive)
                {
                    SettingsHelper.SaveLocalSetting(SettingsHelper.IsLicenseActiveToken, true);

                    if (AppLicense.IsTrial)
                    {
                        SettingsHelper.SaveLocalSetting(SettingsHelper.IsLicenseTrialToken, true);
                        SettingsHelper.SaveLocalSetting(SettingsHelper.IsTrialExpiredToken, false);
                        SettingsHelper.SaveLocalObjectSetting(SettingsHelper.TrialExpirationDateToken, AppLicense.ExpirationDate);
                    }
                    else
                    {
                        SettingsHelper.SaveLocalSetting(SettingsHelper.IsLicenseTrialToken, false);
                        SettingsHelper.SaveLocalSetting(SettingsHelper.IsTrialExpiredToken, false);
                    }
                }
                else
                {
                    SettingsHelper.SaveLocalSetting(SettingsHelper.IsLicenseActiveToken, false);
                    SettingsHelper.SaveLocalSetting(SettingsHelper.IsLicenseTrialToken, true);
                    SettingsHelper.SaveLocalSetting(SettingsHelper.IsTrialExpiredToken, true);
                    SettingsHelper.SaveLocalObjectSetting(SettingsHelper.TrialExpirationDateToken, AppLicense.ExpirationDate);
                }
            }

            AppLicense.LicenseChanged += LicenseInformationChanged;
        }

        public static void LicenseInformationChanged()
        {
            if (AppLicense != null)
            {
                if (AppLicense.IsActive)
                {
                    SettingsHelper.SaveLocalSetting(SettingsHelper.IsLicenseActiveToken, true);

                    if (AppLicense.IsTrial)
                    {
                        SettingsHelper.SaveLocalSetting(SettingsHelper.IsLicenseTrialToken, true);
                        SettingsHelper.SaveLocalSetting(SettingsHelper.IsTrialExpiredToken, false);
                        SettingsHelper.SaveLocalObjectSetting(SettingsHelper.TrialExpirationDateToken, AppLicense.ExpirationDate);
                    }
                    else
                    {
                        SettingsHelper.SaveLocalSetting(SettingsHelper.IsLicenseTrialToken, false);
                        SettingsHelper.SaveLocalSetting(SettingsHelper.IsTrialExpiredToken, false);
                    }
                }
                else
                {
                    SettingsHelper.SaveLocalSetting(SettingsHelper.IsLicenseActiveToken, false);
                    SettingsHelper.SaveLocalSetting(SettingsHelper.IsLicenseTrialToken, true);
                    SettingsHelper.SaveLocalSetting(SettingsHelper.IsTrialExpiredToken, true);
                    SettingsHelper.SaveLocalObjectSetting(SettingsHelper.TrialExpirationDateToken, AppLicense.ExpirationDate);
                }
            }
        }

        public async static Task<string> BuyAppLicenseAsync()
        {
            return await CurrentApp.RequestAppPurchaseAsync(true);
            //return await CurrentAppSimulator.RequestAppPurchaseAsync(true);
        }

    }
}
