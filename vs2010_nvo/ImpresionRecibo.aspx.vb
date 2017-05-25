Imports System.Data.SqlClient
Imports System.Data
Imports CLPortalAbstract
Partial Class ImpresionRecibo
    Inherits Page
    Private _rfc As String
    Private ReadOnly _conexion As New Skytex.FacturaElectronica.Datos
    Private _myConnection As SqlConnection = _Conexion.MiConexion
    Private DS As New DataSet()

#Region "Metodos y Funciones"

    Private Sub FObtieneRfc()
        If Not Page.PreviousPage Is Nothing Then
            Dim cp As ContentPlaceHolder = CType(PreviousPage.Master.FindControl("_mainContent"), ContentPlaceHolder)
            If Not cp Is Nothing Then
                Dim loginControl As UserControl = CType(cp.FindControl("ctlLogin1"), UserControl)
                If Not loginControl Is Nothing Then
                    Dim userName1 As System.Web.UI.WebControls.Login
                    userName1 = loginControl.FindControl("Login1")
                    If Not userName1 Is Nothing Then
                        _rfc = userName1.UserName
                    End If
                End If
            End If
        End If
    End Sub

    Private Function FNombreProveedor(ByVal rfc As String) As String
        Dim user = CType(Page.Session("user"), PerfilesUser)
        Dim consulta = From con In user.Perfil _
                Select nomProv = con.Proveedor _
                Distinct
        Dim proveedor As String = ""
        For Each dato In consulta
            proveedor = dato
        Next
        Return proveedor
    End Function

#End Region


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        LlenaTablasSession()
        LlenaDtEmb()
        borraInfoCfd_session()

        If Not IsPostBack And Not Page.PreviousPage Is Nothing Then
            FObtieneRFC()
            
            Page.Session("RFC") = _rfc
            Page.Session("nomProveedor") = ""
        Else
            If Session("USRswUsrInterno") = 1 Then
                Me.LnkCambiaRFC.Visible = True
                lblUsuarioActivo.Text = "Usuario Actual:" & Session("USRnomUsuario")
                lblUsuarioActivo.Visible = True
                'GCM 07012015 Comentado
                'button2.Visible = True
                'Else
                '  button2.Visible = False
            End If

            'GCM 07012015 muestra acuse
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

    Protected Sub LinkAcuse_Click(sender As Object, e As EventArgs) Handles LinkAcuse.Click
        Response.Redirect("AcuseMercancia.aspx")
    End Sub


    Protected Sub LnkSeguimiento_Click(sender As Object, e As EventArgs) Handles LnkSeguimientoCap.Click
        Response.Redirect("SeguimientoCap.aspx")
    End Sub

    Protected Sub LinkFactura_Click(sender As Object, e As EventArgs) Handles LinkFactura.Click
        Response.Redirect("Facturas.aspx")
    End Sub


    Private Sub LlenaTablasSession()
        Dim dt = New DataTable()
        dt.Columns.Add(New DataColumn("Referencia", Type.GetType("System.String")))
        dt.Columns.Add(New DataColumn("Partida", Type.GetType("System.Int64")))
        dt.Columns.Add(New DataColumn("NoIdentificacion", Type.GetType("System.String")))
        dt.Columns.Add(New DataColumn("Uns", Type.GetType("System.Decimal")))
        dt.Columns.Add(New DataColumn("UM", Type.GetType("System.String")))
        dt.Columns.Add(New DataColumn("Precio", Type.GetType("System.Decimal")))
        dt.Columns.Add(New DataColumn("PctDesc", Type.GetType("System.Decimal")))
        dt.Columns.Add(New DataColumn("Importe", Type.GetType("System.Decimal")))
        dt.AcceptChanges()
        Page.Session("dtArt") = dt
        Dim dt2 = New DataTable()
        dt2.Columns.Add(New DataColumn("NoIdentificacion", Type.GetType("System.String")))
        dt2.Columns.Add(New DataColumn("Description", Type.GetType("System.String")))
        dt2.Columns.Add(New DataColumn("Importe", Type.GetType("System.Decimal")))
        dt2.AcceptChanges()
        Page.Session("dtCargo") = dt2
    End Sub
    Private Sub LlenaDtEmb()
        Dim dte = New DataTable()
        dte.Columns.Add(New DataColumn("Referencia", Type.GetType("System.String")))
        dte.Columns.Add(New DataColumn("NoIdentificacion", Type.GetType("System.String")))
        dte.Columns.Add(New DataColumn("Description", Type.GetType("System.String")))
        dte.Columns.Add(New DataColumn("Importe", Type.GetType("System.Decimal")))
        dte.AcceptChanges()
        Page.Session("dtTransportes") = dte
        Const swRetProv As Boolean = False
        Page.Session("sw_ret_prov") = swRetProv
    End Sub
    Private Sub borraInfoCfd_session()

        Page.Session("nombre_receptor") = System.String.Empty
        Page.Session("EmpresaTitle") = System.String.Empty
        Page.Session("subtotal") = System.Decimal.Zero
        Page.Session("descuento") = System.Decimal.Zero
        Page.Session("total") = System.Decimal.Zero
        Page.Session("tot_imp_trasl") = System.Decimal.Zero
        Page.Session("tot_imp_ret") = System.Decimal.Zero
        Page.Session("tasa") = System.Decimal.Zero
        Page.Session("importe") = System.Decimal.Zero
        Page.Session("total_conceptos") = System.Decimal.Zero
        Page.Session("importe_conceptos") = System.Decimal.Zero

    End Sub

    Protected Sub LnkCambiaRFC_Click(sender As Object, e As EventArgs) Handles LnkCambiaRFC.Click
        Response.Redirect("AdministraProveedor.aspx")
    End Sub
End Class
