<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_scriptCommand43
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_scriptCommand43))
        Me.rdListControl = New Telerik.WinControls.UI.RadListControl()
        Me.rdpInstruction = New Telerik.WinControls.UI.RadPanel()
        Me.rdrtbInstruction = New Telerik.WinControls.UI.RadRichTextEditor()
        CType(Me.rdListControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdpInstruction, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.rdpInstruction.SuspendLayout()
        CType(Me.rdrtbInstruction, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdListControl
        '
        resources.ApplyResources(Me.rdListControl, "rdListControl")
        Me.rdListControl.Name = "rdListControl"
        Me.rdListControl.ScrollMode = Telerik.WinControls.UI.ItemScrollerScrollModes.Deferred
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
        'usctrl_scriptCommand43
        '
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.rdpInstruction)
        Me.Controls.Add(Me.rdListControl)
        Me.Name = "usctrl_scriptCommand43"
        Me.Controls.SetChildIndex(Me.rdListControl, 0)
        Me.Controls.SetChildIndex(Me.rdpInstruction, 0)
        CType(Me.rdListControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdpInstruction, System.ComponentModel.ISupportInitialize).EndInit()
        Me.rdpInstruction.ResumeLayout(False)
        CType(Me.rdrtbInstruction, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rdListControl As Telerik.WinControls.UI.RadListControl
    Friend WithEvents rdpInstruction As Telerik.WinControls.UI.RadPanel
    Friend WithEvents rdrtbInstruction As Telerik.WinControls.UI.RadRichTextEditor

End Class
