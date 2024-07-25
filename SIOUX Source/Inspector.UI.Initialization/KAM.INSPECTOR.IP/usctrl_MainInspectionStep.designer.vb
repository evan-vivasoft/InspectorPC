<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_MainInspectionStep

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_MainInspectionStep))
        Me.lblIPStep = New Telerik.WinControls.UI.RadLabel()
        Me.btnNext = New Telerik.WinControls.UI.RadButton()
        Me.rdtbSection = New Telerik.WinControls.UI.RadTextBox()
        Me.TimerBtnNext = New System.Windows.Forms.Timer(Me.components)
        Me.rdtbSubSection = New Telerik.WinControls.UI.RadTextBox()
        CType(Me.lblIPStep, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.btnNext, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdtbSection, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdtbSubSection, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblIPStep
        '
        resources.ApplyResources(Me.lblIPStep, "lblIPStep")
        Me.lblIPStep.Name = "lblIPStep"
        '
        'btnNext
        '
        resources.ApplyResources(Me.btnNext, "btnNext")
        Me.btnNext.Name = "btnNext"
        Me.btnNext.TabStop = False
        '
        'rdtbSection
        '
        Me.rdtbSection.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        resources.ApplyResources(Me.rdtbSection, "rdtbSection")
        Me.rdtbSection.ForeColor = System.Drawing.Color.Black
        Me.rdtbSection.Name = "rdtbSection"
        Me.rdtbSection.ReadOnly = True
        '
        '
        '
        Me.rdtbSection.RootElement.AccessibleDescription = resources.GetString("rdtbSection.RootElement.AccessibleDescription")
        Me.rdtbSection.RootElement.AccessibleName = resources.GetString("rdtbSection.RootElement.AccessibleName")
        Me.rdtbSection.RootElement.Alignment = CType(resources.GetObject("rdtbSection.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdtbSection.RootElement.AngleTransform = CType(resources.GetObject("rdtbSection.RootElement.AngleTransform"), Single)
        Me.rdtbSection.RootElement.FlipText = CType(resources.GetObject("rdtbSection.RootElement.FlipText"), Boolean)
        Me.rdtbSection.RootElement.Margin = CType(resources.GetObject("rdtbSection.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdtbSection.RootElement.Text = resources.GetString("rdtbSection.RootElement.Text")
        Me.rdtbSection.RootElement.TextOrientation = CType(resources.GetObject("rdtbSection.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.rdtbSection.TabStop = False
        '
        'TimerBtnNext
        '
        '
        'rdtbSubSection
        '
        resources.ApplyResources(Me.rdtbSubSection, "rdtbSubSection")
        Me.rdtbSubSection.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.rdtbSubSection.ForeColor = System.Drawing.Color.Black
        Me.rdtbSubSection.Name = "rdtbSubSection"
        Me.rdtbSubSection.ReadOnly = True
        '
        '
        '
        Me.rdtbSubSection.RootElement.AccessibleDescription = resources.GetString("rdtbSubSection.RootElement.AccessibleDescription")
        Me.rdtbSubSection.RootElement.AccessibleName = resources.GetString("rdtbSubSection.RootElement.AccessibleName")
        Me.rdtbSubSection.RootElement.Alignment = CType(resources.GetObject("rdtbSubSection.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdtbSubSection.RootElement.AngleTransform = CType(resources.GetObject("rdtbSubSection.RootElement.AngleTransform"), Single)
        Me.rdtbSubSection.RootElement.FlipText = CType(resources.GetObject("rdtbSubSection.RootElement.FlipText"), Boolean)
        Me.rdtbSubSection.RootElement.Margin = CType(resources.GetObject("rdtbSubSection.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdtbSubSection.RootElement.Text = resources.GetString("rdtbSubSection.RootElement.Text")
        Me.rdtbSubSection.RootElement.TextOrientation = CType(resources.GetObject("rdtbSubSection.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.rdtbSubSection.TabStop = False
        Me.rdtbSubSection.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'usctrl_MainInspectionStep
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.rdtbSubSection)
        Me.Controls.Add(Me.rdtbSection)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.lblIPStep)
        Me.Name = "usctrl_MainInspectionStep"
        CType(Me.lblIPStep, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.btnNext, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdtbSection, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdtbSubSection, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblIPStep As Telerik.WinControls.UI.RadLabel
    Friend WithEvents btnNext As Telerik.WinControls.UI.RadButton
    Friend WithEvents rdtbSection As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents TimerBtnNext As System.Windows.Forms.Timer
    Friend WithEvents rdtbSubSection As Telerik.WinControls.UI.RadTextBox

End Class
