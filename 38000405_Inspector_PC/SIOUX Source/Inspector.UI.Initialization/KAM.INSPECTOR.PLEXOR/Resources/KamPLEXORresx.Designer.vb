'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.17020
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources

    'This class was auto-generated by the StronglyTypedResourceBuilder
    'class via a tool like ResGen or Visual Studio.
    'To add or remove a member, edit your .ResX file then rerun ResGen
    'with the /str option, or rebuild your VS project.
    '''<summary>
    '''  A strongly-typed resource class, for looking up localized strings, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()> _
    Friend Class KamPLEXORresx

        Private Shared resourceMan As Global.System.Resources.ResourceManager

        Private Shared resourceCulture As Global.System.Globalization.CultureInfo

        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
        Friend Sub New()
            MyBase.New
        End Sub

        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
        Friend Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("KAM.INSPECTOR.PLEXOR.KamPLEXORresx", GetType(KamPLEXORresx).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property

        '''<summary>
        '''  Overrides the current thread's CurrentUICulture property for all
        '''  resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
        Friend Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !Calibration date = .
        '''</summary>
        Friend Shared ReadOnly Property str_Calibration_date() As string
            Get
                Return ResourceManager.GetString("str_Calibration_date", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !PLEXOR device Calibration date.
        '''</summary>
        Friend Shared ReadOnly Property str_Calibration_Header() As string
            Get
                Return ResourceManager.GetString("str_Calibration_Header", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !The calibration date of the PLEXOR device is out of date..
        '''</summary>
        Friend Shared ReadOnly Property str_Calibration_out_of_date() As string
            Get
                Return ResourceManager.GetString("str_Calibration_out_of_date", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !!The calibration date of the PLEXOR device is about to expire.
        '''</summary>
        Friend Shared ReadOnly Property str_Calibration_to_expire() As string
            Get
                Return ResourceManager.GetString("str_Calibration_to_expire", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !Device.
        '''</summary>
        Friend Shared ReadOnly Property str_Device() As string
            Get
                Return ResourceManager.GetString("str_Device", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !No bluetooth device found at the PC.
        '''</summary>
        Friend Shared ReadOnly Property str_NoBTDevicePCFound() As string
            Get
                Return ResourceManager.GetString("str_NoBTDevicePCFound", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !Status.
        '''</summary>
        Friend Shared ReadOnly Property str_Status() As string
            Get
                Return ResourceManager.GetString("str_Status", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !StepID.
        '''</summary>
        Friend Shared ReadOnly Property str_StepID() As string
            Get
                Return ResourceManager.GetString("str_StepID", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !StepMessage.
        '''</summary>
        Friend Shared ReadOnly Property str_StepMessage() As string
            Get
                Return ResourceManager.GetString("str_StepMessage", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !StepNumber.
        '''</summary>
        Friend Shared ReadOnly Property str_StepNumber() As string
            Get
                Return ResourceManager.GetString("str_StepNumber", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !Step Number last device.
        '''</summary>
        Friend Shared ReadOnly Property str_StepNumberLastDevice() As string
            Get
                Return ResourceManager.GetString("str_StepNumberLastDevice", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !StepStatus.
        '''</summary>
        Friend Shared ReadOnly Property str_StepStatus() As string
            Get
                Return ResourceManager.GetString("str_StepStatus", resourceCulture)
            End Get
        End Property
    End Class
End Namespace
