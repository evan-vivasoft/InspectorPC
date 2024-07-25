<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AboutLicenseForm
    Inherits Telerik.WinControls.UI.RadForm

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AboutLicenseForm))
        Me.RadPageView1 = New Telerik.WinControls.UI.RadPageView()
        Me.PageAbout = New Telerik.WinControls.UI.RadPageViewPage()
        CType(Me.RadPageView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadPageView1.SuspendLayout()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RadPageView1
        '
        Me.RadPageView1.Controls.Add(Me.PageAbout)
        Me.RadPageView1.DefaultPage = Me.PageAbout
        resources.ApplyResources(Me.RadPageView1, "RadPageView1")
        Me.RadPageView1.Name = "RadPageView1"
        '
        '
        '
        Me.RadPageView1.RootElement.AccessibleDescription = resources.GetString("RadPageView1.RootElement.AccessibleDescription")
        Me.RadPageView1.RootElement.AccessibleName = resources.GetString("RadPageView1.RootElement.AccessibleName")
        Me.RadPageView1.RootElement.Alignment = CType(resources.GetObject("RadPageView1.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.RadPageView1.RootElement.AngleTransform = CType(resources.GetObject("RadPageView1.RootElement.AngleTransform"), Single)
        Me.RadPageView1.RootElement.FlipText = CType(resources.GetObject("RadPageView1.RootElement.FlipText"), Boolean)
        Me.RadPageView1.RootElement.Margin = CType(resources.GetObject("RadPageView1.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.RadPageView1.RootElement.Text = resources.GetString("RadPageView1.RootElement.Text")
        Me.RadPageView1.RootElement.TextOrientation = CType(resources.GetObject("RadPageView1.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.RadPageView1.SelectedPage = Me.PageAbout
        CType(Me.RadPageView1.GetChildAt(0), Telerik.WinControls.UI.RadPageViewStripElement).StripButtons = Telerik.WinControls.UI.StripViewButtons.None
        CType(Me.RadPageView1.GetChildAt(0), Telerik.WinControls.UI.RadPageViewStripElement).StripAlignment = Telerik.WinControls.UI.StripViewAlignment.Left
        CType(Me.RadPageView1.GetChildAt(0), Telerik.WinControls.UI.RadPageViewStripElement).ItemContentOrientation = Telerik.WinControls.UI.PageViewContentOrientation.Horizontal
        '
        'PageAbout
        '
        Me.PageAbout.ItemSize = New System.Drawing.SizeF(50.0!, 28.0!)
        resources.ApplyResources(Me.PageAbout, "PageAbout")
        Me.PageAbout.Name = "PageAbout"
        '
        'AboutLicenseForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.RadPageView1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "AboutLicenseForm"
        '
        '
        '
        Me.RootElement.AccessibleDescription = resources.GetString("AboutLicenseForm.RootElement.AccessibleDescription")
        Me.RootElement.AccessibleName = resources.GetString("AboutLicenseForm.RootElement.AccessibleName")
        Me.RootElement.Alignment = CType(resources.GetObject("AboutLicenseForm.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.RootElement.AngleTransform = CType(resources.GetObject("AboutLicenseForm.RootElement.AngleTransform"), Single)
        Me.RootElement.ApplyShapeToControl = True
        Me.RootElement.FlipText = CType(resources.GetObject("AboutLicenseForm.RootElement.FlipText"), Boolean)
        Me.RootElement.Margin = CType(resources.GetObject("AboutLicenseForm.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.RootElement.Text = resources.GetString("AboutLicenseForm.RootElement.Text")
        Me.RootElement.TextOrientation = CType(resources.GetObject("AboutLicenseForm.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.ShowIcon = False
        CType(Me.RadPageView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadPageView1.ResumeLayout(False)
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents RadPageView1 As Telerik.WinControls.UI.RadPageView
    Friend WithEvents PageAbout As Telerik.WinControls.UI.RadPageViewPage
End Class

