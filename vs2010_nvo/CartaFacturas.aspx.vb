
Partial Class CartaFacturas
    Inherits Page


    Protected Sub LnkRegresar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LnkRegresar.Click
        Response.Redirect("Login.aspx")
    End Sub

End Class
