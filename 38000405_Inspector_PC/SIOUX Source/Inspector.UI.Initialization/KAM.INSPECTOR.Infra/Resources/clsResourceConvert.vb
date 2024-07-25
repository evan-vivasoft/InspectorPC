Imports KAM.INSPECTOR.Infra.My.Resources
Public Class clsResourceConvert
#Region "Localization"
    ''' <summary>
    ''' Error codes are defined in Inspector.Model.ErroCodes
    ''' </summary>
    ''' <param name="stepErrorCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getErrorTranslation(stepErrorCode As String) As string
        Dim stepErrorMessage As String = ""
        If stepErrorCode <> "-1" Then
            stepErrorMessage = My.Resources.InitializationErrorCodesResx.ResourceManager.GetString("Error_" & stepErrorCode)
            If stepErrorMessage = Nothing Then
                stepErrorMessage = KamInfraresx.str_Unkown_error_message & stepErrorCode
            Else

            End If
            Return stepErrorMessage
        Else
            Return KamInfraresx.str_Step_Started
        End If
    End Function
    ''' <summary>
    ''' Device commands are defined in Inspector.infra.Devicecommands
    ''' </summary>
    ''' <param name="deviceCommand"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getDeviceCommandsTranslation(deviceCommand As String) As string
        Dim deviceCommandMessage As String = ""
        deviceCommandMessage = DeviceCommandResx.ResourceManager.GetString(deviceCommand)
        If deviceCommandMessage = Nothing Then deviceCommandMessage = KamInfraresx.str_Unkown_device_command & deviceCommand
        Return deviceCommandMessage
    End Function
#End Region
End Class
