
Partial Class SeguimientoCaja
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack And Not Page.PreviousPage Is Nothing Then
        Else
            If Session("USRswUsrInterno") = 1 Then
                lblUsuarioActivo.Text = "Usuario Actual:" & Session("USRnomUsuario")
                lblUsuarioActivo.Visible = True
                LnkRetornaMenuOpc.Visible = True
                Page.Session("consulta") = "NA"
            End If
        End If
    End Sub

    Protected Sub LnkRetornaMenuOpc_Click(sender As Object, e As System.EventArgs) Handles LnkRetornaMenuOpc.Click
        Response.Redirect("AdministraProveedor.aspx")
    End Sub

   
    Protected Sub lblSubir_Click(sender As Object, e As System.EventArgs) Handles lblSubir.Click
        Response.Redirect("CajaChica.aspx")
    End Sub
End Class
