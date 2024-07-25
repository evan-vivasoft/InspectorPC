Imports KAM.COMMUNICATOR.Synchronize.Infra.My.Resources
Public Module clsResourceConvert
#Region "Localization"
    ''' <summary>
    ''' Device commands are defined in KAM.COMMUNICATOR.Synchronize.Infra.SynchronisationStep
    ''' </summary>
    ''' <param name="StepID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSyncStepTranslation(stepID As Integer) As String
        Return (SyncStepStatusResx.ResourceManager.GetString("Step_" & stepID))
    End Function
    ''' <summary>
    ''' Device commands are defined in  KAM.COMMUNICATOR.Synchronize.Infra.SyncErrorMessages
    ''' </summary>
    ''' <param name="ErrorNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSyncErrorTranslation(errorNumber As Integer) As String
        Return (SyncStepErrorMessage_resx.ResourceManager.GetString("StepError_" & ErrorNumber))
    End Function

#End Region
End Module
