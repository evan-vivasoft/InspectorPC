
Namespace Model.Station.Entities
    Public Module PrsConstants
        Public Const PRS_TABLE_NAME As String = "PRS"
        Public Const GCL_TABLE_NAME As String = "GasControlLine"

        Public Const PRS_PRSOBJECTS_RELATION_NAME As String = "PRS_PRSObjects"
        Public Const PRS_GCL_RELATION_NAME As String = "PRS_GCLs"
        Public Const GCL_GCLOBJECTS_RELATION_NAME As String = "GasControlLine_GCLObjects"
        Public Const GCLOBJECTS_BOUNDARIES_RELATION_NAME As String = "GCLObjects_Boundaries"

        ' PRS to GasControlLine primary key columns
        Public Const PRSGCL_PRIMARY_KEY_PART1_OF_2 As String = "PRSIdentification"
        Public Const PRSGCL_PRIMARY_KEY_PART2_OF_2 As String = "PRSName"

        Public Const PRSSTATUS_TABLE_NAME As String = "InspectionStatus"



    End Module


End Namespace