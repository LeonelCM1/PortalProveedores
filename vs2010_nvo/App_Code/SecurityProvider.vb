Imports CLPortalAbstract

Public Class DevSecurityProvider
    Inherits Web.Security.SqlMembershipProvider
    ReadOnly _objSecurity As CLPortalUsersBussiness.BLSecurity = New CLPortalUsersBussiness.BLSecurity

    Public Overloads Function ValidateUser(ByVal username As String, ByVal password As String, ByVal user As PerfilesUser) As PerfilesUser
        _objSecurity.UserName = username
        Try
            user = SiteLevelAuthentication(username, password, user)
            If user.Perfil.Any() Then
                SessionHandler.UserName = _objSecurity.UserName
            End If
            Return user
        Catch ex As Exception
            Return user
        End Try
    End Function

    Public Overloads Function ValidateUserInternal(ByVal user As String) As Integer
        Try
            SessionHandler.UserName = user
            Return 0
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Private Function SiteLevelAuthentication(ByVal userName As String, ByVal password As String, ByVal user As PerfilesUser) As PerfilesUser
        user = _objSecurity.ValidUser(userName, password, user)
        Return user
    End Function
End Class

