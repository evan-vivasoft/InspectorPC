Imports KAM.COMMUNICATOR.Synchronize.Bussiness
Imports Microsoft.Win32
Imports System.IO


Public Class FormTest
    Dim sdfFile As New KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Station.clsLinqToSql

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        Dim linqmsAccessToSql As New KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Station.clsLinqmsAccessToSdf
        ', Dim mdbDatabase As New KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Station.MsAccess.clsLinqToMDB
        Debug.Print("1 Started: " & Format(Now, "HH:mm:ss:fff"))
        linqmsAccessToSql.WriteStationInformation(Path.Combine(Application.StartupPath, "Test Files\Export", "StationInformationExportMDB.sdf"), Path.Combine(Application.StartupPath, "Test Files", "prs.mdb"))
0:
        Debug.Print("2 Finished: " & Format(Now, "HH:mm:ss:fff"))

    End Sub


    Private Sub Button1_Click_1(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Dim XMLfile As KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Station.clsLinqXMLToXML

        Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Test Files\Export"))

        Debug.Print("1 Started: " & Format(Now, "HH:mm:ss:da fff"))
        XMLfile = New KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Station.clsLinqXMLToXML
        ' XMLfile.LoadStationInformation(Path.Combine(Application.StartupPath, "test files", "StationInformationMsAExport.xml"), Path.Combine(Application.StartupPath, "test files", "StationInformation.xsd"), True, Path.Combine(Application.StartupPath, "test files", "InspectionStatus.xml"), Path.Combine(Application.StartupPath, "test files", "InspectionStatus.xsd"))
        Debug.Print("2: " & Format(Now, "HH:mm:ss:fff"))
        'XMLfile.WriteStationInformation(Path.Combine(Application.StartupPath, "Test Files\Export", "StationInformationExport.xml"), (Path.Combine(Application.StartupPath, "Test Files", "StationInformation.xsd")))
        Debug.Print("3: " & Format(Now, "HH:mm:ss:fff"))
        'KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Station.ConvertxmlFile(Path.Combine(Application.StartupPath, "Test Files\Export", "StationInformationExport.xml"), Path.Combine(Application.StartupPath, "Test Files\Export", "StationInformationExport_trans.xml"), Path.Combine(Application.StartupPath, "Test Files\", "MapToStationInformation.xslt"))
        'Debug.Print("4: " & Format(Now, "HH:mm:ss:fff"))

        ' ''Dim sdfDatabase As New KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Station.clsLinqToSql
        ' ''sdfDatabase.PRSEntities = XMLfile.PRSEntities
        ' ''sdfDatabase.WriteStationInformation(Path.Combine(Application.StartupPath, "Test Files\Export", "StationInformationExportXML.sdf"))
        ' ''Debug.Print("4 Finished: " & Format(Now, "HH:mm:ss:fff"))
    End Sub


    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        Debug.Print("1 Started: " & Format(Now, "HH:mm:ss:fff"))
        'sdfFile.LoadSqlImportData(Path.Combine(Application.StartupPath, "Test Files", "StationInformation.sdf"))
        Debug.Print("2: " & Format(Now, "HH:mm:ss:fff"))
        'sdfFile.WriteStationInformation(Path.Combine(Application.StartupPath, "Test Files\Export", "StationInformationExportsdf.sdf"))
        Debug.Print("3 Finished: " & Format(Now, "HH:mm:ss:fff"))

    End Sub


    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click
        'Dim XMLfile As KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Result.
        Dim accessToSdf As New KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Result.clsLinqMsAccessToSdf

        Debug.Print("1 Started: " & Format(Now, "HH:mm:ss:fff"))
        'accessToSdf.LoadWriteResults(Path.Combine(Application.StartupPath, "Test Files\Export", "ResultExportMDB.sdf"), Path.Combine(Application.StartupPath, "Test Files", "Result.mdb"))
        Debug.Print("2 Finished: " & Format(Now, "HH:mm:ss:fff"))



    End Sub

    'Station Access to XML
    Private Sub Button5_Click(sender As System.Object, e As System.EventArgs) Handles Button5.Click
        Dim mdbDatabase As New KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Station.clsLinqMsAccessToXml

        Debug.Print("1: " & Format(Now, "HH:mm:ss:fff"))
        mdbDatabase.LoadStationInformation(Path.Combine(Application.StartupPath, "Test Files", "prs.mdb")) ', "PDASNR=1;PDASNR=2;")
        Debug.Print("2: " & Format(Now, "HH:mm:ss:fff"))
        mdbDatabase.WriteStationInformation(Path.Combine(Application.StartupPath, "Test Files\Export", "StationInformationMsAExport.xml"), (Path.Combine(Application.StartupPath, "Test Files", "StationInformation.xsd")))

        Debug.Print("3 Finished: " & Format(Now, "HH:mm:ss:fff"))
    End Sub

    'Result Access to XML
    Private Sub Button6_Click(sender As System.Object, e As System.EventArgs) Handles Button6.Click
        Dim mdbDatabase As New KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Result.clsLinqMsAccessToXML

        Debug.Print("1: " & Format(Now, "HH:mm:ss:fff"))
        mdbDatabase.LoadResultsInformation(Path.Combine(Application.StartupPath, "Test Files", "result.mdb")) ', "PDASNR=1;PDASNR=2;")
        Debug.Print("2: " & Format(Now, "HH:mm:ss:fff"))
        mdbDatabase.WriteResultsInformation(Path.Combine(Application.StartupPath, "Test Files\Export", "ResultsMsAExport.xml"), (Path.Combine(Application.StartupPath, "Test Files", "InspectionResultsData.xsd")))

        Debug.Print("3 Finished: " & Format(Now, "HH:mm:ss:fff"))
    End Sub

    Private Sub Button7_Click(sender As System.Object, e As System.EventArgs) Handles Button7.Click
        Dim xmlHandling As New KAM.COMMUNICATOR.Synchronize.Bussiness.Model.Result.clsLinqXmlToXml

        Debug.Print("1: " & Format(Now, "HH:mm:ss:fff"))
        xmlHandling.LoadResults(Path.Combine(Application.StartupPath, "Test Files", "results.xml"), Path.Combine(Application.StartupPath, "Test Files", "InspectionResultsData.xsd")) ', "PDASNR=1;PDASNR=2;")
        Debug.Print("2: " & Format(Now, "HH:mm:ss:fff"))
        xmlHandling.WriteResults(Path.Combine(Application.StartupPath, "Test Files\Export", "resultsXmlExport.xml"), Path.Combine(Application.StartupPath, "Test Files", "InspectionResultsData.xsd"))

        Debug.Print("3 Finished: " & Format(Now, "HH:mm:ss:fff"))
    End Sub

    Private Sub Button8_Click(sender As System.Object, e As System.EventArgs) Handles Button8.Click
        Dim excelPath As String = Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\UnInstall\{23BDBC3D-117A-4246-AD3F-2C8E8E98E6C0}", "InstallLocation", "Key does not exist")
        MsgBox(excelPath)
    End Sub
End Class
