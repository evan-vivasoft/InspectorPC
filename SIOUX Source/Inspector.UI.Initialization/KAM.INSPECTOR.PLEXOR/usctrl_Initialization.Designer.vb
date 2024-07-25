<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_Initialization

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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_Initialization))
        Dim TableViewDefinition1 As Telerik.WinControls.UI.TableViewDefinition = New Telerik.WinControls.UI.TableViewDefinition()
        Me.rdGridInitStatus = New Telerik.WinControls.UI.RadGridView()
        Me.rdWaitBar = New Telerik.WinControls.UI.RadWaitingBar()
        Me.lstInitializationSteps = New Telerik.WinControls.UI.RadListControl()
        Me.RadDesktopAlert1 = New Telerik.WinControls.UI.RadDesktopAlert(Me.components)
        CType(Me.rdGridInitStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGridInitStatus.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdWaitBar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.lstInitializationSteps, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdGridInitStatus
        '
        resources.ApplyResources(Me.rdGridInitStatus, "rdGridInitStatus")
        '
        '
        '
        Me.rdGridInitStatus.MasterTemplate.ViewDefinition = TableViewDefinition1
        Me.rdGridInitStatus.Name = "rdGridInitStatus"
        '
        'rdWaitBar
        '
        resources.ApplyResources(Me.rdWaitBar, "rdWaitBar")
        Me.rdWaitBar.Name = "rdWaitBar"
        Me.rdWaitBar.WaitingIndicatorSize = New System.Drawing.Size(50, 30)
        Me.rdWaitBar.WaitingSpeed = 100
        Me.rdWaitBar.WaitingStep = 3
        Me.rdWaitBar.WaitingStyle = Telerik.WinControls.Enumerations.WaitingBarStyles.Throbber
        '
        'lstInitializationSteps
        '
        resources.ApplyResources(Me.lstInitializationSteps, "lstInitializationSteps")
        Me.lstInitializationSteps.Name = "lstInitializationSteps"
        '
        '
        '
        Me.lstInitializationSteps.RootElement.AccessibleDescription = resources.GetString("lstInitializationSteps.RootElement.AccessibleDescription")
        Me.lstInitializationSteps.RootElement.AccessibleName = resources.GetString("lstInitializationSteps.RootElement.AccessibleName")
        Me.lstInitializationSteps.RootElement.Alignment = CType(resources.GetObject("lstInitializationSteps.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.lstInitializationSteps.RootElement.AngleTransform = CType(resources.GetObject("lstInitializationSteps.RootElement.AngleTransform"), Single)
        Me.lstInitializationSteps.RootElement.FlipText = CType(resources.GetObject("lstInitializationSteps.RootElement.FlipText"), Boolean)
        Me.lstInitializationSteps.RootElement.Margin = CType(resources.GetObject("lstInitializationSteps.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.lstInitializationSteps.RootElement.Text = resources.GetString("lstInitializationSteps.RootElement.Text")
        Me.lstInitializationSteps.RootElement.TextOrientation = CType(resources.GetObject("lstInitializationSteps.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.lstInitializationSteps.SelectionMode = System.Windows.Forms.SelectionMode.None
        Me.lstInitializationSteps.ShowItemToolTips = False
        '
        'usctrl_Initialization
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lstInitializationSteps)
        Me.Controls.Add(Me.rdWaitBar)
        Me.Controls.Add(Me.rdGridInitStatus)
        Me.Name = "usctrl_Initialization"
        CType(Me.rdGridInitStatus.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdGridInitStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdWaitBar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.lstInitializationSteps, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents rdGridInitStatus As Telerik.WinControls.UI.RadGridView
    Friend WithEvents rdWaitBar As Telerik.WinControls.UI.RadWaitingBar
    Friend WithEvents lstInitializationSteps As Telerik.WinControls.UI.RadListControl
    Friend WithEvents RadDesktopAlert1 As Telerik.WinControls.UI.RadDesktopAlert

End Class
