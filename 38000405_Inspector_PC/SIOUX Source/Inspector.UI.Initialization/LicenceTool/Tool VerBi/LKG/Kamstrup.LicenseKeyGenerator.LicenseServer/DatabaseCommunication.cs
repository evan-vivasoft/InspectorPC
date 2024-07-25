using System;
using System.Collections.Generic;
using System.Text;
using Kamstrup.LicenseKeyGenerator.LicenseServer.DAL;

namespace Kamstrup.LicenseKeyGenerator.LicenseServer
{
  public static class DatabaseCommunication
  {
    public static void LogDeletion(string activationKey, string computerID, string orderNo, string comment, string connectionString)
    {
      LinqToSQL.LogDeletion(activationKey, computerID, orderNo, comment, connectionString);
    }

    public static void LogTrialKeyGeneration(string key, string orderNo, int numberOfDays, string productName, string connectionString)
    {
      LinqToSQL.LogTrialKeyGeneration(key, orderNo, numberOfDays, productName, connectionString);
    }

    public static void LogReleaseLicense(string activationKey, string computerID, string orderNo, string comment, string connectionString)
    {
      LinqToSQL.LogReleaseLicense(activationKey, computerID, orderNo, comment, connectionString);
    }

    public static void DeleteLicense(string activationKey, string computerID, string connectionString)
    {
      LinqToSQL.DeleteLicense(activationKey, computerID, connectionString);
    }

    public static string GetLog(string connectionString)
    {
      IEnumerable<Log4LKG> logList = LinqToSQL.GetLog(connectionString);
      StringBuilder stringBuilder = new StringBuilder();
      const string header = "TimeStamp;\tAction;\tKey;\tComment;\tComputerID;\tOrderNo;\tUserName;\t";
      stringBuilder.Append(header);
      foreach (Log4LKG log in logList)
      {
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append(log.TimeStamp);
        stringBuilder.Append(';');
        stringBuilder.Append('\t');
        stringBuilder.Append(log.Action);
        stringBuilder.Append(';');
        stringBuilder.Append('\t');
        stringBuilder.Append(log.Key);
        stringBuilder.Append(';');
        stringBuilder.Append('\t');
        stringBuilder.Append(log.Comment);
        stringBuilder.Append(';');
        stringBuilder.Append('\t');
        stringBuilder.Append(log.ComputerID);
        stringBuilder.Append(';');
        stringBuilder.Append('\t');
        stringBuilder.Append(log.OrderNo);
        stringBuilder.Append(';');
        stringBuilder.Append('\t');
        stringBuilder.Append(log.UserName);
        stringBuilder.Append(';');
        stringBuilder.Append('\t');
      }
      return stringBuilder.ToString();
    }
  }
}
