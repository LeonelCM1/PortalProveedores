
Partial Class mpMenu
    Inherits System.Web.UI.MasterPage

    Protected Sub LnkCerrarSesion_Click(sender As Object, e As EventArgs) Handles LnkCerrarSesion.Click
        Session.Abandon()
        FormsAuthentication.SignOut()

        If Session("USRswUsrInterno") = 0 Then
            Response.Redirect("Login.aspx")
        Else
            Response.Redirect("UserLogin.aspx")
        End If

    End Sub

  
End Class

