Imports System.Data.SqlClient
Imports System.Data

Partial Public Class SeguimientoCap
    Inherits Page
    Private _dtNomProv As DataTable = Nothing
    Private _rfc As String
    Private ReadOnly _conexion As New Skytex.FacturaElectronica.Datos
    Private ReadOnly _myConnection As SqlConnection = _Conexion.MiConexion
    Private ReadOnly _ds As New DataSet()

#Region "Metodos y Funciones"
    Private Sub FObtieneRFC()
        If Not Page.PreviousPage Is Nothing Then
            Dim CP As ContentPlaceHolder = CType(PreviousPage.Master.FindControl("_mainContent"), ContentPlaceHolder)
            If Not CP Is Nothing Then
                Dim LoginControl As UserControl = CType(CP.FindControl("ctlLogin1"), UserControl)
                If Not LoginControl Is Nothing Then
                    Dim UserName1 As System.Web.UI.WebControls.Login
                    UserName1 = LoginControl.FindControl("Login1")
                    If Not UserName1 Is Nothing Then
                        _rfc = UserName1.UserName
                    End If
                End If
            End If
        End If
    End Sub

    Private Function FNombreProveedor(ByVal rfc As String) As String
        Dim myDataAdapter = New SqlDataAdapter("sp_NombreProv_web", _myConnection)
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc", RFC)
        _myConnection.Open()
        Try
            myDataAdapter.Fill(_ds, "sp_NombreProv_web")
            myDataAdapter.Dispose()
            _myConnection.Close()
            _dtNomProv = _ds.Tables.Item(0)
        Catch ex As Exception
            LblProveedor.Text = "Error: " + ex.Message
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
        Return _dtNomProv.Rows(0).Item(0).ToString
    End Function
#End Region

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack And Not Page.PreviousPage Is Nothing Then
            FObtieneRfc()
            Page.Session("RFC") = _rfc
            Page.Session("nomProveedor") = ""
        Else
            If Session("USRswUsrInterno") = 1 Then
                Me.LnkCambiaRFC.Visible = True
                lblUsuarioActivo.Text = "Usuario Actual:" & Session("USRnomUsuario")
                lblUsuarioActivo.Visible = True
                'GCM 19122014 comentado
                'button2.Visible = True
                'Else
                '   button2.Visible = False
            End If

            'GCM 18122014 enciente acuse
            If Session("Acuse") = 1 Then
                LinkAcuse.Visible = True
            End If

            'GCM 19012015 muestra acuse
            If Session("Prov") = 1 Then
                LnkPreEntrega.Visible = True
            End If

            If Page.Session("RFC") = "" And Session("USRswUsrInterno") = 0 Then
                Response.Redirect("Login.aspx")
                Exit Sub
            End If

            If Page.Session("RFC") = "" And Session("USRswUsrInterno") = 1 Then
                Response.Redirect("UserLogin.aspx")
                Exit Sub
            End If
            If Not IsPostBack Then
                If Page.Session("nomProveedor") = "" Then
                    LblProveedor.Text = Trim(FNombreProveedor(Page.Session("RFC").ToString()))
                    If Page.Session("nomProveedor") = "" Then
                        Page.Session("nomProveedor") = LblProveedor.Text
                    End If
                Else
                    LblProveedor.Text = Page.Session("nomProveedor").ToString()
                End If
                LblProveedor.Attributes("style") = "TEXT-ALIGN: right"
            End If
        End If
    End Sub

    Private Sub LnkFacturas_Click(sender As Object, e As EventArgs) Handles LnkFacturas.Click
        Response.Redirect("Facturas.aspx")
    End Sub

    Private Sub LinkAcuse_Click(sender As Object, e As EventArgs) Handles LinkAcuse.Click
        Response.Redirect("AcuseMercancia.aspx")
    End Sub

    Protected Sub LnkCambiaRFC_Click(sender As Object, e As EventArgs) Handles LnkCambiaRFC.Click
        Response.Redirect("AdministraProveedor.aspx")
    End Sub
    Protected Sub LnkPreEntrega_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LnkPreEntrega.Click
        Response.Redirect("PreEntregaMercancia.aspx")
    End Sub
End Class