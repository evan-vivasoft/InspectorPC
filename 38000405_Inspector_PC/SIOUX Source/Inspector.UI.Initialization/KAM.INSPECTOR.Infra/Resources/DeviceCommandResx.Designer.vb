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
    Friend Class DeviceCommandResx

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
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("KAM.INSPECTOR.Infra.DeviceCommandResx", GetType(DeviceCommandResx).Assembly)
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
        '''  Looks up a localized string similar to !CheckBatteryStatus.
        '''</summary>
        Friend Shared ReadOnly Property CheckBatteryStatus() As string
            Get
                Return ResourceManager.GetString("CheckBatteryStatus", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !CheckIdentification.
        '''</summary>
        Friend Shared ReadOnly Property CheckIdentification() As string
            Get
                Return ResourceManager.GetString("CheckIdentification", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !CheckManometerPresent.
        '''</summary>
        Friend Shared ReadOnly Property CheckManometerPresent() As string
            Get
                Return ResourceManager.GetString("CheckManometerPresent", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !CheckPressureUnit.
        '''</summary>
        Friend Shared ReadOnly Property CheckPressureUnit() As string
            Get
                Return ResourceManager.GetString("CheckPressureUnit", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !CheckRange.
        '''</summary>
        Friend Shared ReadOnly Property CheckRange() As string
            Get
                Return ResourceManager.GetString("CheckRange", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !CheckSCPIInterface.
        '''</summary>
        Friend Shared ReadOnly Property CheckSCPIInterface() As string
            Get
                Return ResourceManager.GetString("CheckSCPIInterface", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !Connect.
        '''</summary>
        Friend Shared ReadOnly Property Connect() As string
            Get
                Return ResourceManager.GetString("Connect", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !DevicePLEXOR.
        '''</summary>
        Friend Shared ReadOnly Property DeviceNumber0() As string
            Get
                Return ResourceManager.GetString("DeviceNumber0", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !DeviceManometerTH1.
        '''</summary>
        Friend Shared ReadOnly Property DeviceNumber1() As string
            Get
                Return ResourceManager.GetString("DeviceNumber1", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !DeviceManometerTH2.
        '''</summary>
        Friend Shared ReadOnly Property DeviceNumber2() As string
            Get
                Return ResourceManager.GetString("DeviceNumber2", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !Disconnect.
        '''</summary>
        Friend Shared ReadOnly Property Disconnect() As string
            Get
                Return ResourceManager.GetString("Disconnect", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !EnterRemoteLocalCommandMode.
        '''</summary>
        Friend Shared ReadOnly Property EnterRemoteLocalCommandMode() As string
            Get
                Return ResourceManager.GetString("EnterRemoteLocalCommandMode", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !ExitRemoteLocalCommandMode.
        '''</summary>
        Friend Shared ReadOnly Property ExitRemoteLocalCommandMode() As string
            Get
                Return ResourceManager.GetString("ExitRemoteLocalCommandMode", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !FlushManometerCache.
        '''</summary>
        Friend Shared ReadOnly Property FlushManometerCache() As string
            Get
                Return ResourceManager.GetString("FlushManometerCache", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !InitiateSelfTest.
        '''</summary>
        Friend Shared ReadOnly Property InitiateSelfTest() As string
            Get
                Return ResourceManager.GetString("InitiateSelfTest", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !MeasureContinously.
        '''</summary>
        Friend Shared ReadOnly Property MeasureContinously() As string
            Get
                Return ResourceManager.GetString("MeasureContinously", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !None.
        '''</summary>
        Friend Shared ReadOnly Property None() As string
            Get
                Return ResourceManager.GetString("None", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !SetPressureUnit.
        '''</summary>
        Friend Shared ReadOnly Property SetPressureUnit() As string
            Get
                Return ResourceManager.GetString("SetPressureUnit", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !SwitchInitializationLedOff.
        '''</summary>
        Friend Shared ReadOnly Property SwitchInitializationLedOff() As string
            Get
                Return ResourceManager.GetString("SwitchInitializationLedOff", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !SwitchInitializationLedOn.
        '''</summary>
        Friend Shared ReadOnly Property SwitchInitializationLedOn() As string
            Get
                Return ResourceManager.GetString("SwitchInitializationLedOn", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !SwitchToManometerTH1.
        '''</summary>
        Friend Shared ReadOnly Property SwitchToManometerTH1() As string
            Get
                Return ResourceManager.GetString("SwitchToManometerTH1", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !SwitchToManometerTH2.
        '''</summary>
        Friend Shared ReadOnly Property SwitchToManometerTH2() As string
            Get
                Return ResourceManager.GetString("SwitchToManometerTH2", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to !Wakeup.
        '''</summary>
        Friend Shared ReadOnly Property Wakeup() As string
            Get
                Return ResourceManager.GetString("Wakeup", resourceCulture)
            End Get
        End Property
    End Class
End Namespace
