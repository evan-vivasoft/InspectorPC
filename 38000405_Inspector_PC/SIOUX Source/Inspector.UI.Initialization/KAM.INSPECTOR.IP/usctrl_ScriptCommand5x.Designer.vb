<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_scriptCommand5x

    Inherits INSPECTOR.IP.usctrl_MainInspectionStep

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_scriptCommand5x))
        Me.Usctrl_ChartResults = New KAM.INSPECTOR.IP.usctrl_ChartResults()
        Me.btnStop = New Telerik.WinControls.UI.RadButton()
        Me.btnRestart = New Telerik.WinControls.UI.RadButton()
        Me.timerProgressBar = New System.Windows.Forms.Timer(Me.components)
        Me.rdProgressBar = New Telerik.WinControls.UI.RadProgressBar()
        Me.rdpInstruction = New Telerik.WinControls.UI.RadPanel()
        Me.rdrtbInstruction = New Telerik.WinControls.UI.RadRichTextEditor()
        CType(Me.btnStop, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.btnRestart, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdProgressBar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdpInstruction, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.rdpInstruction.SuspendLayout()
        CType(Me.rdrtbInstruction, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Usctrl_ChartResults
        '
        resources.ApplyResources(Me.Usctrl_ChartResults, "Usctrl_ChartResults")
        Me.Usctrl_ChartResults.BackColor = System.Drawing.Color.Transparent
        Me.Usctrl_ChartResults.Name = "Usctrl_ChartResults"
        '
        'btnStop
        '
        resources.ApplyResources(Me.btnStop, "btnStop")
        Me.btnStop.Name = "btnStop"
        '
        'btnRestart
        '
        resources.ApplyResources(Me.btnRestart, "btnRestart")
        Me.btnRestart.Name = "btnRestart"
        '
        '
        '
        Me.btnRestart.RootElement.AccessibleDescription = resources.GetString("btnRestart.RootElement.AccessibleDescription")
        Me.btnRestart.RootElement.AccessibleName = resources.GetString("btnRestart.RootElement.AccessibleName")
        Me.btnRestart.RootElement.Alignment = CType(resources.GetObject("btnRestart.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.btnRestart.RootElement.AngleTransform = CType(resources.GetObject("btnRestart.RootElement.AngleTransform"), Single)
        Me.btnRestart.RootElement.FlipText = CType(resources.GetObject("btnRestart.RootElement.FlipText"), Boolean)
        Me.btnRestart.RootElement.Margin = CType(resources.GetObject("btnRestart.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.btnRestart.RootElement.Text = resources.GetString("btnRestart.RootElement.Text")
        Me.btnRestart.RootElement.TextOrientation = CType(resources.GetObject("btnRestart.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'timerProgressBar
        '
        '
        'rdProgressBar
        '
        resources.ApplyResources(Me.rdProgressBar, "rdProgressBar")
        Me.rdProgressBar.Name = "rdProgressBar"
        '
        '
        '
        Me.rdProgressBar.RootElement.AccessibleDescription = resources.GetString("rdProgressBar.RootElement.AccessibleDescription")
        Me.rdProgressBar.RootElement.AccessibleName = resources.GetString("rdProgressBar.RootElement.AccessibleName")
        Me.rdProgressBar.RootElement.Alignment = CType(resources.GetObject("rdProgressBar.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdProgressBar.RootElement.AngleTransform = CType(resources.GetObject("rdProgressBar.RootElement.AngleTransform"), Single)
        Me.rdProgressBar.RootElement.FlipText = CType(resources.GetObject("rdProgressBar.RootElement.FlipText"), Boolean)
        Me.rdProgressBar.RootElement.Margin = CType(resources.GetObject("rdProgressBar.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdProgressBar.RootElement.Text = resources.GetString("rdProgressBar.RootElement.Text")
        Me.rdProgressBar.RootElement.TextOrientation = CType(resources.GetObject("rdProgressBar.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdpInstruction
        '
        resources.ApplyResources(Me.rdpInstruction, "rdpInstruction")
        Me.rdpInstruction.Controls.Add(Me.rdrtbInstruction)
        Me.rdpInstruction.Name = "rdpInstruction"
        '
        '
        '
        Me.rdpInstruction.RootElement.AccessibleDescription = resources.GetString("rdpInstruction.RootElement.AccessibleDescription")
        Me.rdpInstruction.RootElement.AccessibleName = resources.GetString("rdpInstruction.RootElement.AccessibleName")
        Me.rdpInstruction.RootElement.Alignment = CType(resources.GetObject("rdpInstruction.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdpInstruction.RootElement.AngleTransform = CType(resources.GetObject("rdpInstruction.RootElement.AngleTransform"), Single)
        Me.rdpInstruction.RootElement.FlipText = CType(resources.GetObject("rdpInstruction.RootElement.FlipText"), Boolean)
        Me.rdpInstruction.RootElement.Margin = CType(resources.GetObject("rdpInstruction.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdpInstruction.RootElement.Text = resources.GetString("rdpInstruction.RootElement.Text")
        Me.rdpInstruction.RootElement.TextOrientation = CType(resources.GetObject("rdpInstruction.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdrtbInstruction
        '
        resources.ApplyResources(Me.rdrtbInstruction, "rdrtbInstruction")
        Me.rdrtbInstruction.BorderColor = System.Drawing.Color.FromArgb(CType(CType(156, Byte), Integer), CType(CType(189, Byte), Integer), CType(CType(232, Byte), Integer))
        Me.rdrtbInstruction.CaretWidth = Single.NaN
        Me.rdrtbInstruction.Name = "rdrtbInstruction"
        '
        '
        '
        Me.rdrtbInstruction.RootElement.AccessibleDescription = resources.GetString("rdrtbInstruction.RootElement.AccessibleDescription")
        Me.rdrtbInstruction.RootElement.AccessibleName = resources.GetString("rdrtbInstruction.RootElement.AccessibleName")
        Me.rdrtbInstruction.RootElement.Alignment = CType(resources.GetObject("rdrtbInstruction.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdrtbInstruction.RootElement.AngleTransform = CType(resources.GetObject("rdrtbInstruction.RootElement.AngleTransform"), Single)
        Me.rdrtbInstruction.RootElement.FlipText = CType(resources.GetObject("rdrtbInstruction.RootElement.FlipText"), Boolean)
        Me.rdrtbInstruction.RootElement.Margin = CType(resources.GetObject("rdrtbInstruction.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdrtbInstruction.RootElement.Text = resources.GetString("rdrtbInstruction.RootElement.Text")
        Me.rdrtbInstruction.RootElement.TextOrientation = CType(resources.GetObject("rdrtbInstruction.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.rdrtbInstruction.SelectionFill = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(78, Byte), Integer), CType(CType(158, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'usctrl_scriptCommand5x
        '
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.btnRestart)
        Me.Controls.Add(Me.rdpInstruction)
        Me.Controls.Add(Me.rdProgressBar)
        Me.Controls.Add(Me.btnStop)
        Me.Controls.Add(Me.Usctrl_ChartResults)
        Me.Name = "usctrl_scriptCommand5x"
        Me.Controls.SetChildIndex(Me.Usctrl_ChartResults, 0)
        Me.Controls.SetChildIndex(Me.btnStop, 0)
        Me.Controls.SetChildIndex(Me.rdProgressBar, 0)
        Me.Controls.SetChildIndex(Me.rdpInstruction, 0)
        Me.Controls.SetChildIndex(Me.btnRestart, 0)
        CType(Me.btnStop, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.btnRestart, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdProgressBar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdpInstruction, System.ComponentModel.ISupportInitialize).EndInit()
        Me.rdpInstruction.ResumeLayout(False)
        CType(Me.rdrtbInstruction, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Usctrl_ChartResults As KAM.INSPECTOR.IP.usctrl_ChartResults
    Friend WithEvents btnStop As Telerik.WinControls.UI.RadButton
    Friend WithEvents btnRestart As Telerik.WinControls.UI.RadButton
    Friend WithEvents timerProgressBar As System.Windows.Forms.Timer
    Friend WithEvents rdProgressBar As Telerik.WinControls.UI.RadProgressBar
    Friend WithEvents rdpInstruction As Telerik.WinControls.UI.RadPanel
    Friend WithEvents rdrtbInstruction As Telerik.WinControls.UI.RadRichTextEditor

End Class
