'--------------------------------------------------------------------------------------------------
'Class Name     : BLSecurity
'Description    : This module handles Bussiness Logic
'--------------------------------------------------------------------------------------------------
Imports CLPortalUsersData
Imports CLPortalAbstract.AbstractLayer
Imports CLPortalAbstract

Public Class BlSecurity
    Inherits baseLogin
    Private objDataAccess As New DALSecurity

    Public Function ValidUser(ByVal UserName As String, ByVal pPassword As String, ByVal user As PerfilesUser) As PerfilesUser 'As Boolean
        Try
            user = objDataAccess.GetPortalUser(UserName, pPassword)
            Return user
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function ObtConfig() As List(Of ConfigGral)
        Dim user As List(Of ConfigGral)
        Try
            user = objDataAccess.GetConfig(10)
        Catch ex As Exception
            Throw ex
        End Try
        Return user
    End Function

End Class
