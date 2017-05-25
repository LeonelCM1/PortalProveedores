'--------------------------------------------------------------------------------------------------
'Class Name     : DALSecurity
'Description    : This module handles Data Access Logic of Procedures using LINQ for Security
'--------------------------------------------------------------------------------------------------
Imports CLPortalUsersDataLink
Imports CLPortalAbstract

Public Class DalSecurity
    Private ReadOnly _obj As New Factura
    Public Function GetPortalUser(ByVal pUser As String, ByVal pPassword As String) As PerfilesUser 'As System.Collections.Generic.List(Of datos_user)
        Try
            Dim pf As PerfilesUser = _obj.GetPortalUser(pUser, pPassword)
            Return pf 'pf.perfil.ToList()
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Public Function GetConfig(ByVal noInf As Integer) As List(Of ConfigGral)
        Dim confg As List(Of ConfigGral)
        Try
            confg = _obj.GetConfig(10)
        Catch ex As Exception
            Throw ex
        End Try
        Return confg
    End Function
End Class
