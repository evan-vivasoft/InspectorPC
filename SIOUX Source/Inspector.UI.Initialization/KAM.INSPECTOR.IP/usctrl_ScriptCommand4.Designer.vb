<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_scriptCommand4

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_scriptCommand4))
        Me.rdScrolPanelSelection = New Telerik.WinControls.UI.RadScrollablePanel()
        Me.rdRadioOption2 = New Telerik.WinControls.UI.RadRadioButton()
        Me.rdRadioOption3 = New Telerik.WinControls.UI.RadRadioButton()
        Me.rdRadioOption1 = New Telerik.WinControls.UI.RadRadioButton()
        Me.rdTextInput = New Telerik.WinControls.UI.RadTextBox()
        Me.rdpInstruction = New Telerik.WinControls.UI.RadPanel()
        Me.rdrtbInstruction = New Telerik.WinControls.UI.RadRichTextEditor()
        CType(Me.rdScrolPanelSelection, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.rdScrolPanelSelection.PanelContainer.SuspendLayout()
        Me.rdScrolPanelSelection.SuspendLayout()
        CType(Me.rdRadioOption2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdRadioOption3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdRadioOption1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdTextInput, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdpInstruction, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.rdpInstruction.SuspendLayout()
        CType(Me.rdrtbInstruction, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdScrolPanelSelection
        '
        resources.ApplyResources(Me.rdScrolPanelSelection, "rdScrolPanelSelection")
        Me.rdScrolPanelSelection.Name = "rdScrolPanelSelection"
        '
        'rdScrolPanelSelection.PanelContainer
        '
        Me.rdScrolPanelSelection.PanelContainer.Controls.Add(Me.rdRadioOption2)
        Me.rdScrolPanelSelection.PanelContainer.Controls.Add(Me.rdRadioOption3)
        Me.rdScrolPanelSelection.PanelContainer.Controls.Add(Me.rdRadioOption1)
        resources.ApplyResources(Me.rdScrolPanelSelection.PanelContainer, "rdScrolPanelSelection.PanelContainer")
        '
        'rdRadioOption2
        '
        resources.ApplyResources(Me.rdRadioOption2, "rdRadioOption2")
        Me.rdRadioOption2.Name = "rdRadioOption2"
        '
        'rdRadioOption3
        '
        resources.ApplyResources(Me.rdRadioOption3, "rdRadioOption3")
        Me.rdRadioOption3.Name = "rdRadioOption3"
        '
        'rdRadioOption1
        '
        resources.ApplyResources(Me.rdRadioOption1, "rdRadioOption1")
        Me.rdRadioOption1.Name = "rdRadioOption1"
        '
        'rdTextInput
        '
        Me.rdTextInput.AcceptsReturn = True
        resources.ApplyResources(Me.rdTextInput, "rdTextInput")
        Me.rdTextInput.MaxLength = 255
        Me.rdTextInput.Multiline = True
        Me.rdTextInput.Name = "rdTextInput"
        Me.rdTextInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.rdTextInput.TabStop = False
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
        'usctrl_scriptCommand4
        '
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.rdpInstruction)
        Me.Controls.Add(Me.rdTextInput)
        Me.Controls.Add(Me.rdScrolPanelSelection)
        Me.Name = "usctrl_scriptCommand4"
        Me.Controls.SetChildIndex(Me.rdScrolPanelSelection, 0)
        Me.Controls.SetChildIndex(Me.rdTextInput, 0)
        Me.Controls.SetChildIndex(Me.rdpInstruction, 0)
        Me.rdScrolPanelSelection.PanelContainer.ResumeLayout(False)
        Me.rdScrolPanelSelection.PanelContainer.PerformLayout()
        CType(Me.rdScrolPanelSelection, System.ComponentModel.ISupportInitialize).EndInit()
        Me.rdScrolPanelSelection.ResumeLayout(False)
        CType(Me.rdRadioOption2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdRadioOption3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdRadioOption1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdTextInput, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdpInstruction, System.ComponentModel.ISupportInitialize).EndInit()
        Me.rdpInstruction.ResumeLayout(False)
        CType(Me.rdrtbInstruction, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rdScrolPanelSelection As Telerik.WinControls.UI.RadScrollablePanel
    Friend WithEvents rdTextInput As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents rdRadioOption2 As Telerik.WinControls.UI.RadRadioButton
    Friend WithEvents rdRadioOption3 As Telerik.WinControls.UI.RadRadioButton
    Friend WithEvents rdRadioOption1 As Telerik.WinControls.UI.RadRadioButton
    Friend WithEvents rdpInstruction As Telerik.WinControls.UI.RadPanel
    Friend WithEvents rdrtbInstruction As Telerik.WinControls.UI.RadRichTextEditor

End Class
