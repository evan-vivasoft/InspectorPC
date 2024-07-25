<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_FormDisplayResults

    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_FormDisplayResults))
        Dim TypeReportSource1 As Telerik.Reporting.TypeReportSource = New Telerik.Reporting.TypeReportSource()
        Dim TableViewDefinition1 As Telerik.WinControls.UI.TableViewDefinition = New Telerik.WinControls.UI.TableViewDefinition()
        Me.ReportViewer1 = New Telerik.ReportViewer.WinForms.ReportViewer()
        Me.rdStationName = New Telerik.WinControls.UI.RadLabel()
        Me.rdGridResultSelection = New Telerik.WinControls.UI.RadGridView()
        Me.rlblPRSName = New Telerik.WinControls.UI.RadLabel()
        CType(Me.rdStationName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGridResultSelection, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGridResultSelection.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.rdGridResultSelection.SuspendLayout()
        CType(Me.rlblPRSName, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ReportViewer1
        '
        Me.ReportViewer1.AccessibilityKeyMap = Nothing
        resources.ApplyResources(Me.ReportViewer1, "ReportViewer1")
        Me.ReportViewer1.Name = "ReportViewer1"
        TypeReportSource1.TypeName = "KAM.INSPECTOR.ResultsGridView.ReportInspectionResults, KAM.INSPECTOR.ResultsGridV" & _
    "iew, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
        Me.ReportViewer1.ReportSource = TypeReportSource1
        '
        'rdStationName
        '
        resources.ApplyResources(Me.rdStationName, "rdStationName")
        Me.rdStationName.Name = "rdStationName"
        '
        '
        '
        Me.rdStationName.RootElement.AccessibleDescription = resources.GetString("rdStationName.RootElement.AccessibleDescription")
        Me.rdStationName.RootElement.AccessibleName = resources.GetString("rdStationName.RootElement.AccessibleName")
        Me.rdStationName.RootElement.Alignment = CType(resources.GetObject("rdStationName.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdStationName.RootElement.AngleTransform = CType(resources.GetObject("rdStationName.RootElement.AngleTransform"), Single)
        Me.rdStationName.RootElement.FlipText = CType(resources.GetObject("rdStationName.RootElement.FlipText"), Boolean)
        Me.rdStationName.RootElement.Margin = CType(resources.GetObject("rdStationName.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdStationName.RootElement.Text = resources.GetString("rdStationName.RootElement.Text")
        Me.rdStationName.RootElement.TextOrientation = CType(resources.GetObject("rdStationName.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdGridResultSelection
        '
        resources.ApplyResources(Me.rdGridResultSelection, "rdGridResultSelection")
        Me.rdGridResultSelection.Controls.Add(Me.rlblPRSName)
        Me.rdGridResultSelection.Controls.Add(Me.rdStationName)
        '
        '
        '
        Me.rdGridResultSelection.MasterTemplate.ViewDefinition = TableViewDefinition1
        Me.rdGridResultSelection.Name = "rdGridResultSelection"
        '
        '
        '
        Me.rdGridResultSelection.RootElement.AccessibleDescription = resources.GetString("rdGridResultSelection.RootElement.AccessibleDescription")
        Me.rdGridResultSelection.RootElement.AccessibleName = resources.GetString("rdGridResultSelection.RootElement.AccessibleName")
        Me.rdGridResultSelection.RootElement.Alignment = CType(resources.GetObject("rdGridResultSelection.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdGridResultSelection.RootElement.AngleTransform = CType(resources.GetObject("rdGridResultSelection.RootElement.AngleTransform"), Single)
        Me.rdGridResultSelection.RootElement.FlipText = CType(resources.GetObject("rdGridResultSelection.RootElement.FlipText"), Boolean)
        Me.rdGridResultSelection.RootElement.Margin = CType(resources.GetObject("rdGridResultSelection.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdGridResultSelection.RootElement.Text = resources.GetString("rdGridResultSelection.RootElement.Text")
        Me.rdGridResultSelection.RootElement.TextOrientation = CType(resources.GetObject("rdGridResultSelection.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.rdGridResultSelection.TabStop = False
        '
        'rlblPRSName
        '
        resources.ApplyResources(Me.rlblPRSName, "rlblPRSName")
        Me.rlblPRSName.Name = "rlblPRSName"
        '
        '
        '
        Me.rlblPRSName.RootElement.AccessibleDescription = resources.GetString("rlblPRSName.RootElement.AccessibleDescription")
        Me.rlblPRSName.RootElement.AccessibleName = resources.GetString("rlblPRSName.RootElement.AccessibleName")
        Me.rlblPRSName.RootElement.Alignment = CType(resources.GetObject("rlblPRSName.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rlblPRSName.RootElement.AngleTransform = CType(resources.GetObject("rlblPRSName.RootElement.AngleTransform"), Single)
        Me.rlblPRSName.RootElement.FlipText = CType(resources.GetObject("rlblPRSName.RootElement.FlipText"), Boolean)
        Me.rlblPRSName.RootElement.Margin = CType(resources.GetObject("rlblPRSName.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rlblPRSName.RootElement.Text = resources.GetString("rlblPRSName.RootElement.Text")
        Me.rlblPRSName.RootElement.TextOrientation = CType(resources.GetObject("rlblPRSName.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'usctrl_FormDisplayResults
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ReportViewer1)
        Me.Controls.Add(Me.rdGridResultSelection)
        Me.Name = "usctrl_FormDisplayResults"
        CType(Me.rdStationName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdGridResultSelection.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.rdGridResultSelection, System.ComponentModel.ISupportInitialize).EndInit
        Me.rdGridResultSelection.ResumeLayout(false)
        Me.rdGridResultSelection.PerformLayout
        CType(Me.rlblPRSName, System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)

    End Sub
    Friend WithEvents ReportViewer1 As Telerik.ReportViewer.WinForms.ReportViewer
    Friend WithEvents rdStationName As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rdGridResultSelection As Telerik.WinControls.UI.RadGridView
    Friend WithEvents rlblPRSName As Telerik.WinControls.UI.RadLabel

End Class
