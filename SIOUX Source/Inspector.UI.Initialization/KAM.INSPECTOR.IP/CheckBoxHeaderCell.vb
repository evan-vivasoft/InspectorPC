Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Telerik.WinControls.UI
Imports Telerik.WinControls
Imports System.Drawing
Imports Telerik.WinControls.Enumerations

Public Class CheckBoxHeaderCell

    Inherits GridHeaderCellElement

    Private checkbox As RadCheckBoxElement

    Protected Overrides ReadOnly Property ThemeEffectiveType() As Type
        Get
            Return GetType(GridHeaderCellElement)
        End Get
    End Property

    Public Sub New(ByVal column As GridViewColumn, ByVal row As GridRowElement)
        MyBase.New(column, row)

    End Sub

    Public Overrides Sub Initialize(ByVal column As GridViewColumn, ByVal row As GridRowElement)
        MyBase.Initialize(column, row)
        column.AllowSort = False
    End Sub


    Public Overloads Overrides Sub SetContent()
    End Sub

    Protected Overrides Sub DisposeManagedResources()
        RemoveHandler checkbox.ToggleStateChanged, AddressOf checkbox_ToggleStateChanged
        MyBase.DisposeManagedResources()
    End Sub

    Protected Overloads Overrides Sub CreateChildElements()
        MyBase.CreateChildElements()
        checkbox = New RadCheckBoxElement()
        AddHandler checkbox.ToggleStateChanged, AddressOf checkbox_ToggleStateChanged
        Me.Children.Add(checkbox)
    End Sub

    Protected Overloads Overrides Function ArrangeOverride(ByVal finalSize As SizeF) As SizeF
        Dim size As SizeF = MyBase.ArrangeOverride(finalSize)

        Dim rect As RectangleF = GetClientRectangle(finalSize)
        Me.checkbox.Arrange(New RectangleF((finalSize.Width - Me.checkbox.DesiredSize.Width) / 2, (rect.Height - 20) / 2, 20, 20))

        Return size
    End Function

    Public Overloads Overrides Function IsCompatible(ByVal data As Telerik.WinControls.UI.GridViewColumn, ByVal context As Object) As Boolean
        Return data.Name = "Select" AndAlso TypeOf context Is GridTableHeaderRowElement AndAlso MyBase.IsCompatible(data, context)
    End Function

    Private Sub checkbox_ToggleStateChanged(ByVal sender As Object, ByVal args As StateChangedEventArgs)

        If Not suspendProcessingToggleStateChanged Then
            Dim valueState As Boolean = False

            If args.ToggleState = Telerik.WinControls.Enumerations.ToggleState.[On] Then
                valueState = True
            End If
            Me.GridViewElement.EditorManager.EndEdit()
            Me.TableElement.BeginUpdate()
            For i As Integer = 0 To Me.ViewInfo.Rows.Count - 1
                Me.ViewInfo.Rows(i).Cells(Me.ColumnIndex).Value = valueState
            Next

            Me.TableElement.EndUpdate(False)
            Me.TableElement.Update(GridUINotifyAction.DataChanged)
        End If
    End Sub

    Private suspendProcessingToggleStateChanged As Boolean
    Public Sub SetCheckBoxState(ByVal state As Telerik.WinControls.Enumerations.ToggleState)
        suspendProcessingToggleStateChanged = True
        Me.checkbox.ToggleState = state
        suspendProcessingToggleStateChanged = False
    End Sub

    Public Overrides Sub Attach(ByVal data As Telerik.WinControls.UI.GridViewColumn, ByVal context As Object)
        MyBase.Attach(data, context)
        AddHandler Me.GridControl.ValueChanged, AddressOf GridControl_ValueChanged
    End Sub

    Public Overrides Sub Detach()
        MyBase.Detach()
        Try
            RemoveHandler Me.GridControl.ValueChanged, AddressOf GridControl_ValueChanged
        Catch ex As Exception

        End Try
    End Sub

    Private Sub GridControl_ValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim editor As RadCheckBoxEditor = TryCast(sender, RadCheckBoxEditor)
        If editor IsNot Nothing Then
            'to do Me.GridViewElement.EditorManager.EndEdit()
            If DirectCast(editor.Value, ToggleState) = ToggleState.Off Then
                SetCheckBoxState(ToggleState.Off)
            Else
                Dim found As Boolean = False
                For Each row As GridViewRowInfo In Me.ViewInfo.Rows
                    If row.Equals(Me.RowInfo) = False AndAlso row.Cells(Me.ColumnIndex).Value Is Nothing OrElse Not CBool(row.Cells(Me.ColumnIndex).Value) Then

                        found = True
                        Exit For
                    End If
                Next
                If Not found Then
                    SetCheckBoxState(ToggleState.[On])
                End If
            End If
        End If
    End Sub

End Class