using System;
using Kamstrup.LicenseKeyGenerator.Model;
using Kamstrup.LicenseKeyGenerator.Controller;
using System.Text;
using System.IO;

namespace Kamstrup.LicenseKeyGenerator.Console
{
    /// <summary>
    /// The console version of License Key Generator is only used for production in Denmark so the messages are in danish.
    /// </summary>
    public class ConsoleApp
    {
        private class InputData
        {
            public bool UseOnlyOrderNumber { get; set; }
            public bool UseSpecifikFileName { get; set; }
            public string VareNummer { get; set; }
            public int AntalLicenser { get; set; }
            public string OutPutFilNavn { get; set; }
        }

        /*
         * 3 methods to call this main:
         *  - ProductId
         *  - ProductId, number of licenses
         *  - ProductId, number of licenses, Path to your config file
         */
        public static void Main(string[] args)
        {
#if DEBUG
            //args = new[] { "9999-999.1" }; //Should produce an error
            //args = new[] { "6697-060" }; //Should produce an error
            //args = new[] { "6697-060.1" }; //PcBase III med 250 MP
            //args = new[] { "6697-060.2" }; //PcBase III med 250 MP - additional
            //args = new[] { "6697-060.3" }; //PcBase III med 250 MP - update 
            //args = new[] { "6697-067" }; //PcTermPro III
            //args = new[] { "6697-069" }; //PcImportExport III
            //args = new[] { "6697-073" }; //PcBase III Logger license
            //args = new[] { "6697-061" }; //PcNet III
            //args = new[] { "6697-074" }; //PcBase III Automatic Export
            //args = new[] { "USBPCSW0020", "1" };
            //args = new[] { "USBPCSW0060", "1", @"C:\Lictest\Licfile.txt" };
            //args = new[] { "6697-060.1", "1" };

#endif
            EqatecMonitor.EqatecMonitor.InitializeMonitor(EqatecMonitor.EqatecMonitorConstants.MonitorApplication.LicenseKeyGenerator, EqatecMonitor.EqatecMonitorConstants.LKG_Version, Environment.UserName, Environment.MachineName);
#if DEBUG || DEBUGNL || DEBUGMAINTENANCE
            EqatecMonitor.EqatecMonitor.DeactivateLogging = true;
#else
      EqatecMonitor.EqatecMonitor.DeactivateLogging = false;
#endif

#if DEBUGMAINTENANCE|| RELEASEMAINTENANCE
            return;
#endif
            EqatecMonitor.EqatecMonitor.Start();
            EqatecMonitor.EqatecMonitor.SendFeatureUse("Version", "Console");
            EqatecMonitor.EqatecMonitor.SendFeatureUse("Number of args", args.Length.ToString());
            EqatecMonitor.EqatecMonitor.SendFeatureUse("WhoCreatesActivationKeys", Environment.UserName + " - " + System.Globalization.RegionInfo.CurrentRegion.EnglishName);
            
            SettingsController.SaveBaseSettings();
            SettingsController.SetLicenseFile();
            CatalogItemController.Instance.GetCatalogItems();
            ProductController.Instance.GetProducts();

            InputData input = GetInPutValues(args);
            try
            {
                ValidateInputArgs(args);

                CatalogItemLookupObject item = GetCatalogItem(input);

                if (item != null)
                {
                    if (input.UseOnlyOrderNumber == true)
                    {
                        SaveKeyFileByOrderNumber(item);
                    }
                    else
                    {
                        SaveKeyFileByInput(input, item);
                    }

                    Environment.ExitCode = 0;
                }
            }
            catch (Exception ex)
            {
                ShowError(input, "Der skete en fejl ved udstedelse af licens." + Environment.NewLine + ex);
                Environment.ExitCode = -1;
                EqatecMonitor.EqatecMonitor.SendException(ex);
            }
            finally
            {
                EqatecMonitor.EqatecMonitor.Stop();
            }
        }

        private static void SaveKeyFileByInput(InputData input, CatalogItemLookupObject item)
        {
            //Get the licence keys and put then in a comma separated list
            StringBuilder licensKeys = new StringBuilder();
            for (int i = 0; i < input.AntalLicenser; i++)
            {
                string key = CreateKey(item);
                licensKeys.Append(key);

                if (i != input.AntalLicenser - 1)
                {
                    licensKeys.Append(" ");
                }
            }

            //Write the list to the file
            if (input.UseSpecifikFileName == false)
            {
                SaveKeyInFile(licensKeys.ToString());
            }
            else
            {
                SaveKeyInFile(licensKeys.ToString(), input);
            }
        }

        private static void SaveKeyFileByOrderNumber(CatalogItemLookupObject item)
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Indsæt arbejdsordre NR:");
            string orderNo = System.Console.ReadLine();
            while (!UITool.OrderNumberFormatApproved(orderNo))
            {
                System.Console.Beep();
                System.Console.WriteLine();
                System.Console.WriteLine(string.Format("Arbejdsordre NR '{0}' har et uventet format. Prøv igen:", orderNo));
                orderNo = System.Console.ReadLine();
            }
            System.Console.WriteLine();

            SaveKeyInFile(CreateKey(item));
        }

        private static CatalogItemLookupObject GetCatalogItem(InputData input)
        {
            string catalogNo;
            if (UITool.CorrectItemNumber(input.VareNummer, out catalogNo))
            {
                CatalogItemLookupObject item = FindData(catalogNo);
                if (item != null)
                {
                    return item;
                }
                else
                {
                    ShowError(input, string.Format("Varenummeret '{0}' kendes ikke af programmet License Key Generator.", input.VareNummer));
                    Environment.ExitCode = -1;
                    return null;
                }
            }
            else
            {
                ShowError(input, "Varenummeret er ikke korrekt format");
                Environment.ExitCode = -1;
                return null;
            }
        }

        private static InputData GetInPutValues(string[] args)
        {
            InputData input = new InputData() { UseOnlyOrderNumber = false, UseSpecifikFileName = false };

            input.UseOnlyOrderNumber = args.Length == 1;
            input.VareNummer = args[0];

            if (input.UseOnlyOrderNumber == false)
            {
                input.AntalLicenser = Convert.ToInt32(args[1]);

                input.UseSpecifikFileName = args.Length >= 2;
                if (input.UseSpecifikFileName == true)
                {
                    input.OutPutFilNavn = args[2];
                }
            }

            return input;
        }

        private static void ValidateInputArgs(string[] args)
        {
            //Constraint - we must have at least the product number          
            if (args.Length == 0)
            {
                InputData input = new InputData() { UseOnlyOrderNumber = true };
                ShowError(input, "Fejl: indtast som minimum varenummer!");
                Environment.ExitCode = -1;
            }

            //At current state we only have 3 inputs
            int maxParamCount = 3;
            if (args.Length > maxParamCount)
            {
                InputData input = new InputData() { UseOnlyOrderNumber = true };
                ShowError(input, "Fejl: forkert antal parametre! Max antal tiladte parametre: " + maxParamCount.ToString());
                Environment.ExitCode = -1;
            }
        }

        private static void ShowError(InputData input, string errorMessage)
        {  
            bool useConsole = input != null && string.IsNullOrEmpty(input.OutPutFilNavn);

            if (useConsole == true)
            {
                ConsoleColor color = System.Console.ForegroundColor;
                System.Console.Beep();
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(errorMessage);
                System.Console.ForegroundColor = color;
#if DEBUG
                try
                {
                    System.Console.ReadLine();
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch (Exception) {/*When debug error do nothing*/}
                // ReSharper restore EmptyGeneralCatchClause
#endif
            }
            else
            {
                string directory = Path.GetDirectoryName(input.OutPutFilNavn);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string errorFile = Path.Combine(directory, "Error.txt");
                TextWriter writer = File.CreateText(errorFile);
                writer.WriteLine(errorMessage);
                writer.Close();
            }

            Environment.ExitCode = -1;
        }

        private static CatalogItemLookupObject FindData(string catalogNo)
        {
            return CatalogItemController.Instance.LookupStoredItem(catalogNo);
        }

        /// <summary>
        /// When debuging license key is generated on test server.
        /// </summary>
        /// <param name="item">CatalogItemLookupObject item</param>
        /// <param name="orderNumber">Order number to store with the activation key</param>
        /// <returns>License key</returns>
        private static string CreateKey(CatalogItemLookupObject item, string orderNumber)
        {
            ProductItem product = ProductController.Instance.GetSpecificProduct(item.Catalog.ProductId);
            CatalogItemController.Instance.DefineProduct(product);
            string activationKey = CatalogItemController.Instance.GenerateActivationKey(item.Feature, orderNumber);
            //print the license info
            System.Console.WriteLine(string.Format("Activation key for {0} with {1} was created {2}{3}", item.Catalog.ProductDescription, item.Feature.ProductDescription, Environment.NewLine, activationKey));
            return activationKey;
        }

        private static string CreateKey(CatalogItemLookupObject item)
        {
            ProductItem product = ProductController.Instance.GetSpecificProduct(item.Catalog.ProductId);
            CatalogItemController.Instance.DefineProduct(product);
            string activationKey = CatalogItemController.Instance.GenerateActivationKey(item.Feature);

            return activationKey;
        }

        private static void SaveKeyInFile(string activationKey)
        {
            CatalogItemController.Instance.saveFile(activationKey);
        }

        private static void SaveKeyInFile(string activationKey, InputData input)
        {
            try
            {
                CatalogItemController.Instance.saveFile(activationKey, input.OutPutFilNavn);
            }
            catch (Exception e)
            {
                ShowError(input, "Error while creating key file: " + e.Message);
                Environment.ExitCode = -1;
            }
        }
    }
}
