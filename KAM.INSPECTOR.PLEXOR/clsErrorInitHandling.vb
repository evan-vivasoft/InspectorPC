'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports Inspector.Model

Public Class clsErrorInitHandling
#Region "Class members"
    Public Event EvntInspectionFinished(errorcode As Integer)
    Public Event EvntRestartInspection(errorcode As Integer)
    Public Event EvntDoNotRestartInspection(errorcode As Integer)

#End Region

#Region "Error codehandling"
    ''' <summary>
    ''' Handling of error code
    ''' Depending on error code restart initialization; show information
    ''' </summary>
    ''' <param name="errorCode"></param>
    Public Sub ErrorCodeHandling(ByVal errorCode As Integer)
        Select Case errorCode
            'Region Initialization
            ' <summary>
            ' The Initialization process finished succesfully
            ' </summary>
            Case ErrorCodes.INITIALIZATION_FINISHED_SUCCESSFULLY '= 1200
                'No action required

                ' <summary>
                ' The initialization step was executed successfully.
                ' </summary>
            Case ErrorCodes.INITIALIZATION_STEP_EXECUTED_SUCCESSFULLY '= 1215
                'No action required

                ' <summary>
                ' The Inspection process finished succesfully
                ' </summary>
            Case ErrorCodes.INSPECTION_FINISHED_SUCCESSFULLY '= 1300
                RaiseEvent EvntInspectionFinished(errorCode)

                ' <summary>
                ' The inspection process was aborted by the user
                ' </summary>
            Case ErrorCodes.INSPECTION_ABORTED_BY_USER '= 1307
                RaiseEvent EvntInspectionFinished(errorCode)

                'MOD 64
                ' <summary>
                ' The inspection process was aborted by the user
                ' </summary>
                'Case ErrorCodes.INSPECTION_COULD_NOT_STOP_CONTINUOUS_MEASUREMENT '= 1321
                ' RaiseEvent EvntInspectionFinished(errorCode)

            Case ErrorCodes.INSPECTION_PLEXOR_BLUETOOTH_ADDRESS_UNDEFINED  '= 1324
                RaiseEvent EvntDoNotRestartInspection(errorCode)

            Case Else
                RaiseEvent EvntRestartInspection(errorCode)
        End Select

    End Sub
#End Region
End Class
