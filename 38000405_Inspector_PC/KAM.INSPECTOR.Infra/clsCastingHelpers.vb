'===============================================================================
'Copyright Wigersma 2015
'All rights reserved.
'===============================================================================
Imports Inspector.Model
Imports System.Globalization

Public Class ClsCastingHelpers
    ''' <summary>
    ''' Casts to type units value, defaults to unset.
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as TypeUnitsValue if successfull otherwise TypeUnitsValue.UNSET</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.Enum.TryParse<Inspector.Model.UnitOfMeasurement>(System.String,System.Boolean,Inspector.Model.UnitOfMeasurement@)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.Enum.TryParse<Inspector.BusinessLogic.InspectionManager.Model.TypeUnitsValue>(System.String,System.Boolean,Inspector.BusinessLogic.InspectionManager.Model.TypeUnitsValue@)")> _
    Public Shared Function CastToTypeUnitsValueOrUnset(value As String) As UnitOfMeasurement
        Dim result As UnitOfMeasurement = UnitOfMeasurement.ItemMbar
        'MOD 16
        If Not [String].IsNullOrEmpty(value) Then
            value = "Item" & value.Replace("-", [String].Empty).Replace("/", [String].Empty)
            [Enum].TryParse(Of UnitOfMeasurement)(value, True, result)
        End If
        Return result
    End Function

    ''' <summary>
    ''' Casts to double, defaults to double.NaN.
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as double if successfull otherwise double.NaN</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.Double.TryParse(System.String,System.Double@)")> _
    Public Shared Function CastToDoubleOrNan(value As String) As Double
        Dim result As Double = Double.NaN
        Dim decimalSeparator As String = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
        If decimalSeparator = "." Then
            value = value.Replace(",", decimalSeparator)
        Else
            value = value.Replace(".", decimalSeparator)
        End If

        If Not [String].IsNullOrEmpty(value) Then
            Double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, result)
            ''  Double.TryParse(value, result)
        End If
        Return result
    End Function

End Class
