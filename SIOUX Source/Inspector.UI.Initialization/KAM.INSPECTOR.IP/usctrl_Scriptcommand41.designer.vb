<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_Scriptcommand41

    Inherits INSPECTOR.IP.usctrl_MainInspectionStep

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_Scriptcommand41))
        Me.rdPageSC41List = New Telerik.WinControls.UI.RadPageView()
        Me.rdCheckNextListImm = New Telerik.WinControls.UI.RadCheckBox()
        Me.radTxtBRemarks = New Telerik.WinControls.UI.RadTextBox()
        CType(Me.rdPageSC41List, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdCheckNextListImm, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.radTxtBRemarks, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdPageSC41List
        '
        resources.ApplyResources(Me.rdPageSC41List, "rdPageSC41List")
        Me.rdPageSC41List.Name = "rdPageSC41List"
        CType(Me.rdPageSC41List.GetChildAt(0), Telerik.WinControls.UI.RadPageViewStripElement).StripButtons = Telerik.WinControls.UI.StripViewButtons.None
        '
        'rdCheckNextListImm
        '
        resources.ApplyResources(Me.rdCheckNextListImm, "rdCheckNextListImm")
        Me.rdCheckNextListImm.Name = "rdCheckNextListImm"
        '
        'radTxtBRemarks
        '
        Me.radTxtBRemarks.AcceptsReturn = True
        resources.ApplyResources(Me.radTxtBRemarks, "radTxtBRemarks")
        Me.radTxtBRemarks.MaxLength = 255
        Me.radTxtBRemarks.Multiline = True
        Me.radTxtBRemarks.Name = "radTxtBRemarks"
        '
        '
        '
        Me.radTxtBRemarks.RootElement.AccessibleDescription = resources.GetString("radTxtBRemarks.RootElement.AccessibleDescription")
        Me.radTxtBRemarks.RootElement.AccessibleName = resources.GetString("radTxtBRemarks.RootElement.AccessibleName")
        Me.radTxtBRemarks.RootElement.Alignment = CType(resources.GetObject("radTxtBRemarks.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.radTxtBRemarks.RootElement.AngleTransform = CType(resources.GetObject("radTxtBRemarks.RootElement.AngleTransform"), Single)
        Me.radTxtBRemarks.RootElement.FlipText = CType(resources.GetObject("radTxtBRemarks.RootElement.FlipText"), Boolean)
        Me.radTxtBRemarks.RootElement.Margin = CType(resources.GetObject("radTxtBRemarks.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.radTxtBRemarks.RootElement.Text = resources.GetString("radTxtBRemarks.RootElement.Text")
        Me.radTxtBRemarks.RootElement.TextOrientation = CType(resources.GetObject("radTxtBRemarks.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.radTxtBRemarks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.radTxtBRemarks.TabStop = False
        '
        'usctrl_Scriptcommand41
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.radTxtBRemarks)
        Me.Controls.Add(Me.rdCheckNextListImm)
        Me.Controls.Add(Me.rdPageSC41List)
        Me.Name = "usctrl_Scriptcommand41"
        Me.Controls.SetChildIndex(Me.rdPageSC41List, 0)
        Me.Controls.SetChildIndex(Me.rdCheckNextListImm, 0)
        Me.Controls.SetChildIndex(Me.radTxtBRemarks, 0)
        CType(Me.rdPageSC41List, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdCheckNextListImm, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.radTxtBRemarks, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rdPageSC41List As Telerik.WinControls.UI.RadPageView
    Friend WithEvents rdCheckNextListImm As Telerik.WinControls.UI.RadCheckBox
    Friend WithEvents radTxtBRemarks As Telerik.WinControls.UI.RadTextBox

End Class
