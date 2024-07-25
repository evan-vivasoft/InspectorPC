<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_InspectionInformation

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_InspectionInformation))
        Me.rdGridInspectionSteps = New Telerik.WinControls.UI.RadGridView()
        Me.rdDropIPNames = New Telerik.WinControls.UI.RadDropDownList()
        Me.rlblIPName = New Telerik.WinControls.UI.RadLabel()
        CType(Me.rdGridInspectionSteps, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGridInspectionSteps.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdDropIPNames, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rlblIPName, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdGridInspectionSteps
        '
        resources.ApplyResources(Me.rdGridInspectionSteps, "rdGridInspectionSteps")
        Me.rdGridInspectionSteps.Name = "rdGridInspectionSteps"
        '
        'rdDropIPNames
        '
        Me.rdDropIPNames.AllowShowFocusCues = False
        resources.ApplyResources(Me.rdDropIPNames, "rdDropIPNames")
        Me.rdDropIPNames.AutoCompleteDisplayMember = Nothing
        Me.rdDropIPNames.AutoCompleteValueMember = Nothing
        Me.rdDropIPNames.DropDownStyle  = Telerik.WinControls.RadDropDownStyle.DropDownList
        Me.rdDropIPNames.Name = "rdDropIPNames"
        '
        'rlblIPName
        '
        resources.ApplyResources(Me.rlblIPName, "rlblIPName")
        Me.rlblIPName.Name = "rlblIPName"
        '
        '
        '
        Me.rlblIPName.RootElement.AccessibleDescription = resources.GetString("rlblIPName.RootElement.AccessibleDescription")
        Me.rlblIPName.RootElement.AccessibleName = resources.GetString("rlblIPName.RootElement.AccessibleName")
        Me.rlblIPName.RootElement.Alignment = CType(resources.GetObject("rlblIPName.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rlblIPName.RootElement.AngleTransform = CType(resources.GetObject("rlblIPName.RootElement.AngleTransform"), Single)
        Me.rlblIPName.RootElement.FlipText = CType(resources.GetObject("rlblIPName.RootElement.FlipText"), Boolean)
        Me.rlblIPName.RootElement.Margin = CType(resources.GetObject("rlblIPName.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rlblIPName.RootElement.Text = resources.GetString("rlblIPName.RootElement.Text")
        Me.rlblIPName.RootElement.TextOrientation = CType(resources.GetObject("rlblIPName.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'usctrl_InspectionInformation
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.rlblIPName)
        Me.Controls.Add(Me.rdDropIPNames)
        Me.Controls.Add(Me.rdGridInspectionSteps)
        Me.Name = "usctrl_InspectionInformation"
        CType(Me.rdGridInspectionSteps.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdGridInspectionSteps, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdDropIPNames, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rlblIPName, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rdGridInspectionSteps As Telerik.WinControls.UI.RadGridView
    Friend WithEvents rdDropIPNames As Telerik.WinControls.UI.RadDropDownList
    Friend WithEvents rlblIPName As Telerik.WinControls.UI.RadLabel

End Class
