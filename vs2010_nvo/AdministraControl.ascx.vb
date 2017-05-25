Imports System.Data.SqlClient
Imports System.Data
Imports System.Drawing
Imports Skytex.FacturaElectronica
Imports CLPortalAbstract
Partial Class AdministraControl
    Inherits System.Web.UI.UserControl
    ReadOnly _factura As New Skytex.FacturaElectronica.Factura
    Private ReadOnly _conexion As New Skytex.FacturaElectronica.Datos
    Private ReadOnly _myConnection As SqlConnection = _conexion.MiConexion
    Dim _dtResultados As DataTable

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblUsuarioActivo.Text = "Usuario Actual: " & Page.Session("USRnomUsuario").ToString()
            lblUsuarioActivo.Visible = True
            LlenaCombos()
            Dim confg As List(Of ConfigGral) = _factura.GetConfig(10)
            Page.Session("config") = confg
        Else
            If Page.Session("USRCve") = "" Then
                Response.Redirect("UserLogin.aspx")
                Exit Sub
            End If
        End If

        ddlTipoProv.Focus()
    End Sub

    Protected Sub ddlTipoProv_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlTipoProv.SelectedIndexChanged

        Dim itmTipo = ddlTipoProv.SelectedValue.ToString().Trim()

        If itmTipo = "CH" Then
            ddlNombreProv.DataSource = Nothing
            Dim itm = New List(Of comboItem)
            Dim it = New comboItem
            it.Descripcion = "De clic en ingresar"
            it.Valor = "NA"
            itm.Add(it)
            ddlNombreProv.DataSource = itm
            ddlNombreProv.DataTextField = "Descripcion"
            ddlNombreProv.DataValueField = "Valor"
            ddlNombreProv.DataBind()
        Else
            LlenaCboRfc()
        End If

    End Sub

    Private Sub LlenaCombos()
        LlenaCboTipo()
        LlenaCboRfc()
    End Sub

    Private Sub LlenaCboTipo()
        Dim datosCombo As New combos
        datosCombo.combo = "TipoProv"
        datosCombo.rfc = "?"
        datosCombo.parametro1 = "?"
        datosCombo.parametro2 = "?"
        Dim dt = _factura.LlenaCbo(datosCombo)
        ddlTipoProv.DataSource = Nothing
        ddlTipoProv.DataSource = dt
        ddlTipoProv.DataTextField = "descripcion"
        ddlTipoProv.DataValueField = "valor"
        ddlTipoProv.DataBind()
        ddlTipoProv.Items(0).Selected = True
    End Sub

    Private Sub LlenaCboRfc()
        Dim datosCombo As New combos
        datosCombo.combo = "RfcProvXTipo"
        datosCombo.rfc = "?"
        datosCombo.parametro1 = "?"
        datosCombo.parametro2 = ddlTipoProv.SelectedValue.ToString().Trim
        Dim dt = _factura.LlenaCbo(datosCombo)
        ddlNombreProv.DataSource = Nothing
        ddlNombreProv.DataSource = dt
        ddlNombreProv.DataTextField = "descripcion"
        ddlNombreProv.DataValueField = "valor"
        ddlNombreProv.DataBind()
        ddlNombreProv.Items(0).Selected = True
    End Sub
    Private Sub CajaChicaUsr()
        Response.Redirect("CajaChica.aspx")
    End Sub

    Private Sub ProveedoresUsr()

        Dim indice As Integer = ddlNombreProv.SelectedValue.ToString().Trim.IndexOf("@")
        Dim longitud As Integer = ddlNombreProv.SelectedValue.ToString().Trim.Length
        Dim pUser As String = ddlNombreProv.SelectedValue.ToString().Trim.Substring(0, indice)
        Dim pPassword As String = ddlNombreProv.SelectedValue.ToString().Trim.Substring(indice + 1, longitud - indice - 1)
        Dim user As PerfilesUser = _factura.GetPortalUser(pUser, pPassword)

        If user.Perfil.Count > 0 Then
            Page.Session("user") = user
            Page.Session("RFC") = pUser 'RFC
            Page.Session("nomProveedor") = "Bienvenido " + Trim(FNombreProveedor(Page.Session("RFC").ToString()))
            Page.Session("password") = pPassword 'GCM 14012015 para etiquetas
            lblError.Text = ""
            lblError.Visible = False
            Response.Redirect("Facturas.aspx")
            'Response.Redirect("Facturas.aspx", False)
            'Server.Transfer("Facturas.aspx", False)
        Else
            lblError.Visible = True
            lblError.Text = "No se puede obtener configuracion del proveedor"
        End If

    End Sub


    Protected Sub btnCambiar_Click(sender As Object, e As EventArgs) Handles btnCambiar.Click


        If ddlNombreProv.SelectedValue = "NA" And ddlTipoProv.SelectedValue <> "CH" Then
            lblError.Text = "Debe seleccionar caja chica en tipo"
            Exit Sub
        End If

        Dim itmTipo = ddlTipoProv.SelectedValue.ToString().Trim()

        If itmTipo = "CH" Then
            ' se quita validacion puede entrar cualquier usuario a caja chica
            'Dim iValido = ValidaUsuarioCuentaBancaria(Page.Session("USRCve"))

            'If iValido = True Then
            CajaChicaUsr()
            'End If
        Else
            ProveedoresUsr()
        End If





    End Sub


    Private Function ValidaUsuarioCuentaBancaria(ByVal usuario As String) As Boolean
        Dim iValido As Boolean = False
        Dim dt = New DataTable()
        dt = _factura.ObtDatosGenerico(usuario, "", "0", "", 11)

        If dt.Rows.Count > 0 Then
            If Integer.Parse(dt.Rows(0).Item("error").ToString()) = 0 Then
                iValido = True
            End If

        End If

        If iValido = False Then
            lblError.Text = "El usuario firmado no tiene una cuenta bancaria configurada" 'dt.Rows.Item("msg").ToString()
        End If


        Return iValido

    End Function

    Private Function FNombreProveedor(ByVal RFC As String) As String

        Dim user = CType(Page.Session("user"), PerfilesUser)
        Dim consulta = From con In user.Perfil _
                    Select nom_prov = con.Proveedor _
                    Distinct
        Dim proveedor As String = ""

        For Each dato In consulta
            proveedor = dato
        Next

        Return proveedor

    End Function

    
End Class
