namespace Kamstrup.LicenseKeyGenerator.Controller
{
  public class UITool
  {
    public static bool CorrectItemNumber(string vareNummer, out string catalogNo)
    {
        if(string.IsNullOrEmpty(vareNummer))
        {
           catalogNo = string.Empty;
           return false;
        }
        string catalogNumber = vareNummer;

        if (catalogNumber.StartsWith("USBPCSW") == false)
        {
            if (!catalogNumber.Contains("-"))
                catalogNumber = catalogNumber.Insert(4, "-");
        }
          
        catalogNo = catalogNumber;
        return true;
    }

    public static bool OrderNumberFormatApproved(string orderNumber)
    {
      double result;
      if (orderNumber.Trim().Length < 7 || !double.TryParse(orderNumber.Trim(), out result) || result < 0)
      {
        return false;
      }
      return true;
    }
  }
}
