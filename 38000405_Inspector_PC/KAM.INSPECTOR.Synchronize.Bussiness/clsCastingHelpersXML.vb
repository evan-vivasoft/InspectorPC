'===============================================================================
'Copyright Wigersma 2013
'All rights reserved.
'===============================================================================

Imports Inspector.Model
Imports System.ComponentModel
Imports System.Globalization
Imports System.Text.RegularExpressions


Public Class clsCastingHelpersXML
#Region "Casting helpers"
    ''' <summary>
    ''' Casts to type range DM, defaults to unset.
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as TypeRangeDM if successfull otherwise TypeRangeDM.UNSET</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.Enum.TryParse<Inspector.Model.TypeRangeDM>(System.String,System.Boolean,Inspector.Model.TypeRangeDM@)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.Enum.TryParse<Inspector.BusinessLogic.InspectionManager.Model.TypeRangeDM>(System.String,System.Boolean,Inspector.BusinessLogic.InspectionManager.Model.TypeRangeDM@)")> _
    Public Shared Function CastToTypeRangeDMOrUnset(value As String) As TypeRangeDM
        Dim result As TypeRangeDM = TypeRangeDM.UNSET
        value = "Item" & value.Replace("-", [String].Empty).Replace(".", [String].Empty).Replace(" ", [String].Empty)
        [Enum].TryParse(Of TypeRangeDM)(value, True, result)
        Return result
    End Function


    ''' <summary>
    ''' Casts to int, defaults to -1.
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as int if succesfull otherwise -1</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.Int32.TryParse(System.String,System.Int32@)")> _
    Public Shared Function CastToIntOrNull(value) As System.Nullable(Of Integer)
        Dim result As System.Nullable(Of Integer) = Nothing
        If IsDBNull(value) Then
            Return result
        End If

        If Not [String].IsNullOrEmpty(value) Then
            Dim parsedResult As Integer = -1
            Integer.TryParse(value, parsedResult)
            result = parsedResult
        End If
        Return result
    End Function


    ''' <summary>
    ''' Casts to string, defaults to "".
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as string if succesfull otherwise ""</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.string.TryParse(System.String,System.String@)")> _
    Public Shared Function CastToStringOrEmpty(value) As String
        If IsDBNull(value) Then
            Return Nothing
        End If

        If Not [String].IsNullOrEmpty(value) Then
            Return value
        Else
            Return ""
        End If
    End Function

    ''' <summary>
    ''' Casts to string, or nothing.
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as string if succesfull otherwise ""</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.string.TryParse(System.String,System.String@)")> _
    Public Shared Function CastToStringOrNothing(value) As String
        If IsDBNull(value) Then
            Return Nothing
        End If

        If Not [String].IsNullOrEmpty(value) Then
            Return value
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Casts to double, defaults to double.NaN.
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as double if successfull otherwise double.NaN</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.Double.TryParse(System.String,System.Double@)")> _
    Public Shared Function CastToDoubleOrNan(value As String) As Double
        If IsNothing(value) Then Return Nothing

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

    ''' <summary>
    ''' Casts to double, defaults to "".
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as double if successfull otherwise double.NaN</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.Double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, result@)")> _
    Private Shared Function CastToTextOrNan(value As String) As String
        Dim result As String = ""
        Dim decimalSeparator As String = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
        If decimalSeparator = "." Then
            value = value.Replace(",", decimalSeparator)
        Else
            value = value.Replace(".", decimalSeparator)
        End If

        If Not [String].IsNullOrEmpty(value) Then
            Double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, result)
        End If
        Return result
    End Function

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
            'MOD 46
            value = Regex.Replace(value, "[^a-zA-Z0-9]", "")
            value = "Item" & value
            'value = "Item" & value.Replace("-", [String].Empty).Replace("/", [String].Empty)
            'value = value.Replace("(", [String].Empty)
            'value = value.Replace(")", [String].Empty)
            [Enum].TryParse(Of UnitOfMeasurement)(value, True, result)
        End If
        Return result
    End Function

    ''' <summary>
    ''' Casts to type dst value, defaults to No.
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as string if successfull otherwise NO</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.string.TryParse(System.String,System.String@)")> _
    Public Shared Function CastToTypeDstValueOrDefault(value As String) As String
        If Not [String].IsNullOrEmpty(value) Then
            Return value
        Else
            Return "No"
        End If
    End Function

    ''' <summary>
    ''' Casts to type dst value, defaults to No.
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as string if successfull otherwise NO</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.string.TryParse(System.String,System.String@)")> _
    Public Shared Function CastToTypeTimeZoneValueOrDefault(value As String) As String
        If Not [String].IsNullOrEmpty(value) Then
            Return value
        Else
            Return "GMT+01:00"
        End If
    End Function

    ''' <summary>
    ''' Casts to type Date value, defaults "1900,01,30"
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as string if successfull otherwise NO</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.string.TryParse(System.String,System.String@)")> _
    Public Shared Function CastToTypeDateValueOrDefault(value As String) As String
        'MOD 24
        Dim lDate As Date = "1900,01,30"
        If Not [String].IsNullOrEmpty(value) Then
            Try
                Dim checkDate As DateTime = DateTime.Parse(value)
                Return checkDate.ToString(Model.Result.formats.DateTimeStamp.DATE_FORMAT, CultureInfo.InvariantCulture)
            Catch ex As Exception
                Return lDate.ToString(Model.Result.formats.DateTimeStamp.DATE_FORMAT, CultureInfo.InvariantCulture)
            End Try
        Else
            Return lDate.ToString(Model.Result.formats.DateTimeStamp.DATE_FORMAT, CultureInfo.InvariantCulture)
        End If
    End Function

    ''' <summary>
    ''' Casts to type Time value, defaults "00:00:00"
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>value as string if successfull otherwise NO</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId:="System.string.TryParse(System.String,System.String@)")> _
    Public Shared Function CastToTypeTimeValueOrDefault(value As String) As String
        Dim lTime As Date = "00:00:00"
        If Not [String].IsNullOrEmpty(value) Then
            Try
                Dim checkTime As DateTime = DateTime.Parse(value)
                Return checkTime.ToString(Model.Result.formats.DateTimeStamp.TIME_FORMAT, CultureInfo.InvariantCulture)
            Catch ex As Exception
                Return lTime.ToString(Model.Result.formats.DateTimeStamp.TIME_FORMAT, CultureInfo.InvariantCulture)
            End Try
        Else
            Return lTime.ToString(Model.Result.formats.DateTimeStamp.TIME_FORMAT, CultureInfo.InvariantCulture)
        End If
    End Function

#End Region


    ''' <summary>
    ''' This procedure gets the "Description" attribute of an enum constant, if any.
    ''' Otherwise it gets the string name of the enum member.
    ''' </summary>
    ''' <param name="enumConstant"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function EnumDescription(ByVal enumConstant As [Enum]) As String
        Dim fi As Reflection.FieldInfo = enumConstant.GetType().GetField(enumConstant.ToString())
        Dim aattr() As DescriptionAttribute = DirectCast(fi.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())
        If aattr.Length > 0 Then
            Return aattr(0).Description
        Else
            Return (enumConstant.ToString())
        End If
    End Function

End Class

