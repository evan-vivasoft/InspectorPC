using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Kamstrup.LicenseKeyGenerator.LicenseServer.DAL
{
  internal static class LinqToSQL
  {
    #region Properties - kind of...

    private static QLMDataContext DataContext(string connectionString)
    {
      return new QLMDataContext(connectionString);
    }

    private enum Action
    {
      TrialKey,
      DeleteKey,
      ReleaseKey,
    }

    #endregion

    #region Public methods

    #region Log stuff

    public static void LogDeletion(string activationKey, string computerID, string orderNo, string comment,
                                   string connectionString)
    {
      using (QLMDataContext context = DataContext(connectionString))
      {
        Log4LKG log = new Log4LKG
                        {
                          Action = Action.DeleteKey.ToString(),
                          ComputerID = computerID,
                          Key = activationKey,
                          OrderNo = orderNo,
                          Comment = comment,
                        };
        SaveLog(context, log);
      }
    }

    public static void LogTrialKeyGeneration(string key, string orderNo, int numberOfDays, string productName,
                                             string connectionString)
    {
      using (QLMDataContext context = DataContext(connectionString))
      {
        Log4LKG log = new Log4LKG
                        {
                          Action = Action.TrialKey.ToString(),
                          ComputerID = null,
                          Key = key,
                          OrderNo = orderNo,
                          Comment = string.Format("Trial key generated for {0} days for {1}", numberOfDays, productName),
                        };
        SaveLog(context, log);
      }
    }

    public static void LogReleaseLicense(string activationKey, string computerID, string orderNo, string comment,
                                         string connectionString)
    {
      using (QLMDataContext context = DataContext(connectionString))
      {
        Log4LKG log = new Log4LKG
                        {
                          Action = Action.ReleaseKey.ToString(),
                          ComputerID = computerID,
                          Key = activationKey,
                          OrderNo = orderNo,
                          Comment = comment,
                        };
        SaveLog(context, log);
      }
    }

    public static IEnumerable<Log4LKG> GetLog(string connectionString)
    {
      using (QLMDataContext context = DataContext(connectionString))
      {
        var query = from log4Lkg in context.Log4LKGs
                    orderby log4Lkg.TimeStamp
                    select log4Lkg;
        return query.ToList();
      }
    }

    #endregion

    public static void DeleteLicense(string activationKey, string computerID, string connectionString)
    {
      string filter = string.Empty;
      if (!string.IsNullOrEmpty(computerID))
        filter = string.Format("[ComputerID] LIKE '%{0}%'", computerID);
      if (!string.IsNullOrEmpty(activationKey))
      {
        if (!filter.Equals(string.Empty))
          filter += " AND ";
        filter += string.Format("[ActivationKey] LIKE '%{0}%'", activationKey);
      }


      string deleteString =
        @"DELETE FROM [LicenseKeys] 
          WHERE 
          " + filter;

      SqlCommand cmd = new SqlCommand
                         {
                           CommandText = deleteString,
                           Connection = (SqlConnection) DataContext(connectionString).Connection
                         };
      int affectedRows;
      try
      {
        cmd.Connection.Open();
        affectedRows = cmd.ExecuteNonQuery();
      }
      finally
      {
        cmd.Connection.Close();
      }
      if (affectedRows != 1)
        throw new Exception(string.Format("Deletion affected {0} instead of one row.", affectedRows));
    }

    #endregion

    #region Helping methods

    private static void SaveLog(QLMDataContext context, Log4LKG log)
    {
      log.ID = Guid.NewGuid();
      log.TimeStamp = DateTime.Now;
      log.UserName = Environment.UserName;
      log.UserDomainName = Environment.UserDomainName;
      context.Log4LKGs.InsertOnSubmit(log);
      context.SubmitChanges();
    }

    #endregion
  }
}