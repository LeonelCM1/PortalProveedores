
Imports CrystalDecisions.Shared
Imports System.Data.SqlClient
Imports System.Data
Imports CLPortalAbstract
Imports System.Collections.Generic

Imports System.Drawing.Printing
Imports System.ComponentModel
Imports System.Drawing
Imports System.IO

Partial Class GeneraEtiqueta
    Inherits System.Web.UI.UserControl
    Private ReadOnly _conexion As New Skytex.FacturaElectronica.Datos
    Private ReadOnly _myConnection As SqlConnection = _Conexion.MiConexion

    Dim _dtResultados As DataTable
    Dim _ccCve, _ccTipo As String
    Dim _user As PerfilesUser = Nothing
    Dim _server As New _Server
    Dim DBdevelop As String = _Server.DBdevelop
    Dim DBproduc As String = _Server.DBproduc
    Dim DBCat As String = _Server.DBCat
    Dim DBPro As String = _Server.DBpro

#Region "Metodos y Funciones"
    Private Sub CrearFolios()
        'If Not IsPostBack Then
        Dim myDataAdapter = New SqlDataAdapter("sp_genPEntMercWeb", _myConnection)
        Dim ds As New DataSet()
        Dim rfc As String
        Dim passw As String

        lblFol.Text = ""
        lblDoc.Text = ""
        lblFol.Text = ""


        'cc_cve = compone

        passw = Page.Session("password")
        rfc = Page.Session("RFC")
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()

        myDataAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", Me.ddlEmpresa.SelectedValue)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@tipdoc_cve", Me.ddlRecibo.SelectedValue)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_fol", txtFolio.Text)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@RFC", rfc)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@Password", passw)


        Try
            myDataAdapter.Fill(ds, "sp_genPEntMercWeb")
            myDataAdapter.Dispose()
            _myConnection.Close()

            _dtResultados = ds.Tables.Item(0)

            lblFol.Text = _dtResultados.Rows(0).Item("folio")
            lblDoc.Text = _dtResultados.Rows(0).Item("tipo_doc")
            lblErr.Text = _dtResultados.Rows(0).Item("error")

            GridView1.DataSource = _dtResultados
            GridView1.DataBind()


        Catch ex As Exception
            lblErr.Text = "Error al consultar folio"
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
        'End If
        If lblFol.Text <> "0" Then
            GridView1.Visible = True
            btnEtiq.Visible = True
        End If




    End Sub
    Private Sub ConsultaFolios(ByVal accion As String)

        Dim myDataAdapter = New SqlDataAdapter("sp_consDetallesWeb", _myConnection)
        Dim ds As New DataSet()
        Dim uns As Integer
        Dim caja As Integer
        Dim lote As String
        Dim num_reng As Integer

        'lblErr.Visible = True
        'lblErr.Text = ""
        If accion = "Consultar" Then
            uns = 0
            caja = 0
            lote = "sl"
            num_reng = 0
        End If

        If accion = "Actualizar" Then
            uns = txtCant.Text
            caja = txtCaja.Text
            lote = txtLote.Text
            num_reng = txtReng.Text
        End If

        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()

        myDataAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", Me.ddlEmpresa.SelectedValue)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@tipdoc_cve", lblDoc.Text)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_fol", lblFol.Text)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_reng", num_reng)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uns", uns)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@caja", caja)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@lote", lote)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@accion", accion)


        Try
            myDataAdapter.Fill(ds, "sp_consDetallesWeb")
            myDataAdapter.Dispose()
            _myConnection.Close()

            _dtResultados = ds.Tables.Item(0)

            ' _dtResultados = ds.Tables.Item(0)

            If accion = "Actualizar" Then
                lblErr.Text = _dtResultados.Rows(0).Item("error_ms")
                lblErr.Visible = True
            End If

            If accion = "Consultar" Then
                GridView1.DataSource = _dtResultados
                GridView1.DataBind()
            End If

            If lblErr.Text = "Ya se genero la etiqueta no puedes actualizar" Then
                btnEtiq.Visible = False
                txtReng.Visible = False
                txtDesc.Visible = False
                txtCant.Visible = False
                txtCaja.Visible = False
                txtLote.Visible = False
                btnAct.Visible = False
                lblCaja.Visible = False
                lblCant.Visible = False
                lblDesc.Visible = False
                lblLote.Visible = False
                lblReng.Visible = False
                GridView1.Visible = False
                lblErr.Visible = True
            End If


        Catch ex As Exception
            lblErr.Text = "Error al consultar folio"
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try

    End Sub

    Private Sub ConfirmarMov()

        Dim myDataAdapter = New SqlDataAdapter("sp_confMovEtWeb", _myConnection)
        Dim ds As New DataSet()
        Dim errores = 0
        lblErr.Text = ""
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()

        myDataAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", Me.ddlEmpresa.SelectedValue)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@tipdoc_cve", lblDoc.Text)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_fol", lblFol.Text)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@id", "zzz")


        Try
            myDataAdapter.Fill(ds, "sp_confMovEtWeb")
            myDataAdapter.Dispose()
            _myConnection.Close()


        Catch ex As Exception
            lblErr.Text = "Error al confirmar folio"
            lblErr.Visible = True
            errores = 1

        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try

        If errores = 0 Then
            btnAct.Enabled = False
            ConsultaFolios("Consultar")
            ImprimeReporte(Me.ddlEmpresa.SelectedValue, lblFol.Text, lblDoc.Text)
        End If

    End Sub

    Private Sub ImprimeReporte(ByVal efCve As String, ByVal folioContra As String, ByVal doctoContra As String)

        Dim rpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument

        Const comodin As String = "?"
        Const fechacom As String = "1960-01-01 00:00:00.000"
        Dim exportPath As String = ""
        Dim reporteName As String = ""
        Dim claveRpt As Integer

        exportPath = folioContra + "_" + efCve
        reporteName = Server.MapPath("Files/iretprod47.rpt")
        claveRpt = 6

        rpt.Load(reporteName)
        rpt.Refresh()
        Dim db As String = ""
        db = _conexion.ObtieneDb()
        'If db = "DEVELOP" Then
        If db = DBdevelop Then '//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
            'rpt.DataSourceConnections(0).SetConnection("SKYHDEV", "develop", False)
            'rpt.DataSourceConnections(0).SetConnection("SKYHDEV3", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            'rpt.DataSourceConnections(0).SetConnection(DBdevelop, DBCat, False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            rpt.SetDatabaseLogon("soludin_develop", "dinamico20", "skyhdev3", "develop")
        End If
        'If db = "SKYTEX" Then
        If db = DBproduc Then
            'rpt.DataSourceConnections(0).SetConnection("192.168.18.21", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin", "pluma")
            'rpt.DataSourceConnections(0).SetConnection("SQL", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            'rpt.DataSourceConnections(0).SetConnection(DBproduc, DBPro, False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            rpt.SetDatabaseLogon("soludin_develop", "dinamico20", "SQL", "skytex")
        End If
        If db = "192.168.18.96" Then
            'rpt.DataSourceConnections(0).SetConnection("192.168.18.21", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin", "pluma")
            'rpt.DataSourceConnections(0).SetConnection("SQL", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            'rpt.DataSourceConnections(0).SetConnection(DBproduc, DBPro, False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            rpt.SetDatabaseLogon("soludin_develop", "dinamico20", "192.168.18.96", "skytex")
        End If

        rpt.SetParameterValue("@ef_cve", efCve)
        rpt.SetParameterValue("@tipdoc_cve", doctoContra)
        rpt.SetParameterValue("@fol_ini", folioContra)
        rpt.SetParameterValue("@fol_fin", folioContra)
        rpt.SetParameterValue("@cve_rpt", claveRpt)
        rpt.SetParameterValue("@r1_clstip", comodin)
        rpt.SetParameterValue("@r1_cls", 0)
        rpt.SetParameterValue("@r1_ini", comodin)
        rpt.SetParameterValue("@r1_fin", comodin)
        rpt.SetParameterValue("@r2_clstip", comodin)
        rpt.SetParameterValue("@r2_cls", 0)
        rpt.SetParameterValue("@r2_ini", comodin)
        rpt.SetParameterValue("@r2_fin", comodin)
        rpt.SetParameterValue("@r3_clstip", comodin)
        rpt.SetParameterValue("@r3_cls", 0)
        rpt.SetParameterValue("@r3_ini", comodin)
        rpt.SetParameterValue("@r3_fin", comodin)
        rpt.SetParameterValue("@ra_ini", comodin)
        rpt.SetParameterValue("@ra_fin", comodin)
        rpt.SetParameterValue("@fmov_ini", fechacom)
        rpt.SetParameterValue("@fmov_fin", fechacom)
        rpt.SetParameterValue("@st1_doc", comodin)
        rpt.SetParameterValue("@st1_cve", 0)
        rpt.SetParameterValue("@st1_fini", fechacom)
        rpt.SetParameterValue("@st1_ffin", fechacom)
        rpt.SetParameterValue("@st1_term", 0)
        rpt.SetParameterValue("@st2_doc", comodin)
        rpt.SetParameterValue("@st2_cve", 0)
        rpt.SetParameterValue("@st2_fini", fechacom)
        rpt.SetParameterValue("@st2_ffin", fechacom)
        rpt.SetParameterValue("@st2_term", 0)
        rpt.SetParameterValue("@st3_doc", comodin)
        rpt.SetParameterValue("@st3_cve", 0)
        rpt.SetParameterValue("@st3_fini", fechacom)
        rpt.SetParameterValue("@st3_ffin", fechacom)
        rpt.SetParameterValue("@st3_term", 0)
        rpt.SetParameterValue("@id_ultact", "WEB")
        rpt.SetParameterValue("@art_tip", comodin)
        rpt.SetParameterValue("@alm_ini", comodin)
        rpt.SetParameterValue("@alm_fin", comodin)
        rpt.SetParameterValue("@div_ini", comodin)
        rpt.SetParameterValue("@div_fin", comodin)
        rpt.SetParameterValue("@soptip_ini", comodin)
        rpt.SetParameterValue("@soptip_fin", comodin)
        rpt.SetParameterValue("@ele_cve", comodin)
        rpt.SetParameterValue("@pr1", comodin)
        rpt.SetParameterValue("@pr2", comodin)
        rpt.SetParameterValue("@pr3", comodin)
        rpt.SetParameterValue("@pr4", comodin)
        rpt.SetParameterValue("@pr5", comodin)


        Try

            Response.Buffer = False
            Response.Clear()
            Response.ClearContent()
            Response.ClearHeaders()
            Response.ContentType = "application/pdf"
            rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, True, exportPath) ' aqui importando el espacio de nombres
            'rpt.ExportToHttpResponse(ExportFormatType.WordForWindows, Response, True, exportPath) ' aqui importando el espacio de nombres
            rpt.Close()
            '  rpt.Export(ExportOptions)
            'Dim diskOpts As DiskFileDestinationOptions = _
            'ExportOptions.CreateDiskFileDestinationOptions()

            'Dim exportOpts As ExportOptions = New ExportOptions()
            'exportOpts.ExportFormatType = _
            '   ExportFormatType.RichText
            'exportOpts.ExportDestinationType = _
            '   ExportDestinationType.DiskFile

            'diskOpts.DiskFileName = exportPath
            'exportOpts.ExportDestinationOptions = diskOpts

            'rpt.Export(exportOpts)


        Catch ex As Exception
            ex = Nothing


            If Not IsDBNull(rpt.DataSourceConnections(0)) And rpt.IsLoaded Then
                rpt.Dispose()
                rpt.Close()
                rpt = Nothing
            End If

            GC.Collect()

        End Try


    End Sub

    Private Function LlenaCombo(ByVal combo As Skytex.FacturaElectronica.combos) As DataSet

        Dim recibo As New Skytex.FacturaElectronica.nuevas_facturas
        Dim myDataAdapter = New SqlDataAdapter("sp_cboWebXML", _myConnection)
        Dim DS As New DataSet()
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()

        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", combo.rfc)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@cbo", combo.combo)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@prm1", combo.parametro1.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@prm2", combo.parametro2.Trim())

        Try
            myDataAdapter.Fill(DS, "sp_cboWebXML")
            myDataAdapter.Dispose()
            _myConnection.Close()

            If DS.Tables(0).Rows.Count >= 1 Then
                Return DS
            End If

        Catch ex As Exception
            lblErr.Text = "Error al consultar sp_cboWebXML"
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
        Return DS

    End Function

#End Region

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            lblRFC.Text = Page.Session("RFC").ToString()
            LlenaCbos()

        Else
            If Page.Session("RFC") = "" And Session("USRswUsrInterno") = 0 Then
                Response.Redirect("Login.aspx")
                Exit Sub
            End If

            If Page.Session("RFC") = "" And Session("USRswUsrInterno") = 1 Then
                Response.Redirect("UserLogin.aspx")
                Exit Sub
            End If
        End If
    End Sub

    Protected Sub btnGen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGen.Click
        Panel1.Visible = False
        lblErr.Text = ""
        lblErr.Visible = False
        GridView1.Visible = False


        If txtFolio.Text = "" Or txtFolio.Text = "0" Then
            lblErr.Text = "Capturar folio"
            lblErr.Visible = True
        Else
            CrearFolios()

            If lblFol.Text = "" Or lblFol.Text = "0" Then
                btnEtiq.Visible = False
                lblReng.Visible = False
                lblCaja.Visible = False
                lblCant.Visible = False
                lblLote.Visible = False
                lblDesc.Visible = False
                txtReng.Visible = False
                txtCaja.Visible = False
                txtCant.Visible = False
                txtLote.Visible = False
                btnAct.Visible = False
                txtDesc.Visible = False
                lblErr.Visible = True
            Else
                lblReng.Visible = True
                lblCaja.Visible = True
                lblCant.Visible = True
                lblLote.Visible = True
                lblDesc.Visible = True
                txtReng.Visible = True
                txtCaja.Visible = True
                txtCant.Visible = True
                txtLote.Visible = True
                'btnAct.Visible = True
                txtDesc.Visible = True
                lblErr.Text = ""
                lblErr.Visible = False
                txtReng.Text = ""
                txtCant.Text = ""
                txtLote.Text = ""
                txtCaja.Text = ""
                txtDesc.Text = ""
                GridView1.Visible = True
                btnAct.Visible = True
            End If
        End If


    End Sub

    Protected Sub btnEtiq_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Panel1.Visible = False
        ConfirmarMov()
    End Sub



    Private Sub LlenaCbos()

        Dim dsCboAcuse As New DataSet
        Dim dsCboEmpresa As New DataSet
        Dim cbo1 = New Skytex.FacturaElectronica.combos
        Dim cbo2 = New Skytex.FacturaElectronica.combos

        cbo1.rfc = lblRFC.Text
        cbo1.combo = "PreMerc"
        cbo1.parametro1 = ""
        cbo1.parametro2 = ""
        dsCboAcuse = LlenaCombo(cbo1)

        cbo2.rfc = lblRFC.Text
        cbo2.combo = "empresas"
        cbo2.parametro1 = ""
        cbo2.parametro2 = ""
        dsCboEmpresa = LlenaCombo(cbo2)

        ddlRecibo.DataSource = dsCboAcuse.Tables(0)
        ddlRecibo.DataTextField = "nombre"
        ddlRecibo.DataValueField = "tipdoc_cve"
        ddlRecibo.DataBind()
        ddlEmpresa.DataSource = dsCboEmpresa.Tables(0)
        ddlEmpresa.DataTextField = "nom2"
        ddlEmpresa.DataValueField = "ef_cve"
        ddlEmpresa.DataBind()

    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex 'JAPH actualiza la Paginacion del grid
        CrearFolios() 'JAPH carga los datos en la nueva pagina del Grid1

        ' GridView1.Page.AutoPostBackControl = 
        'GridView1.PagerSettings.Mode = PagerButtons.NextPreviousFirstLast
        ' GridView1.PagerSettings.Mode = PagerButtons.NextPrevious

        'GridView1.DataBind() 'JAPH se agrego esta linea
        'ConsultaFolios("consulta")
        ' GridView1.Page.AutoPostBackControl = Page.AutoPostBackControl
    End Sub
   
    Protected Sub GridView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.SelectedIndexChanged
        '((BoundField)GridView1.Columns[2]).ReadOnly = false


    End Sub

    'Protected Sub GridView1_RowEditing(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.RowEditing
    '    '((BoundField)GridView1.Columns[2]).ReadOnly = false

    'End Sub

    'Protected Sub GridView1_RowCancelingEdit(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.RowCancelingEdit
    '    GridView1.EditIndex = -1
    '    ConsultaFolios()
    'End Sub

    'Protected Sub GridView1_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
    '    Dim row = GridView1.Rows(e.RowIndex)
    '    'Dim selectedRow As GridViewRow = GridView1.Rows(row)
    '    lblErr.Text = row.Cells(1).Text
    '    lblErr.Visible = True
    '    ConsultaFolios()
    '    GridView1.EditIndex = -1
    'End Sub

    Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)

        Dim info = e.CommandName

        If e.CommandName = "Select" Then
            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            Dim selectedRow As GridViewRow = GridView1.Rows(index)
            'Dim folio = selectedRow.Cells(1).
            ' Dim lb As LinkButton = DirectCast(selectedRow.Cells(1).Controls(0), LinkButton)
            'lblErr.Text = selectedRow.Cells(1).Text
            'Dim folioAcuse As String = lb.Text.Trim

            txtReng.Text = selectedRow.Cells(2).Text
            txtDesc.Text = selectedRow.Cells(4).Text
            txtCant.Text = selectedRow.Cells(5).Text
            txtCaja.Text = selectedRow.Cells(6).Text
            If selectedRow.Cells(7).Text = "&nbsp;" Then
                txtLote.Text = ""
            Else
                txtLote.Text = selectedRow.Cells(7).Text
            End If

            'Dim numero = GridView1.SelectedIndex.ToString()
            lblErr.Text = ""
            'lblEr.Visible = False

        End If



        '        TableCell celda = DataGrid1.SelectedItem.Cells[nºcelda];
        'string contenidoCelda=celda.Text;
    End Sub



    Protected Sub btnAct_Click(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles btnAct.Click 'Handles btnAct.Click

        Dim errores = 0
        Dim info = txtCant.Text
        Dim caja = Int64.Parse(txtCaja.Text)
        Dim resultado As Decimal
        Dim cantidad As Decimal
        Dim index As Integer

        index = 0

        For Each x As Char In info
            If x = "." Then
                index = index + 1
            End If
        Next

        If index > 1 Then
            lblErr.Text = "Formato de cantidad no valido"
            lblErr.Visible = True
            errores = 1
        End If

        If errores = 0 Then
            cantidad = Convert.ToDecimal(info)
        End If

        If txtReng.Text = "" Or txtReng.Text = "0" And errores = 0 Then
            lblErr.Text = "No has seleccionado registro a modificar"
            lblErr.Visible = True
            errores = 1
        End If

        If txtCaja.Text = "" And errores = 0 Then
            lblErr.Text = "Falta capturar caja"
            lblErr.Visible = True
            errores = 1
        End If

        If errores = 0 And txtLote.Text = "" Then
            lblErr.Text = "Debes capturar el lote"
            lblErr.Visible = True
            errores = 1
        End If

        If errores = 0 And cantidad = 0 Then
            lblErr.Text = "El valor de cantidad no puede ser cero"
            lblErr.Visible = True
            errores = 1
        End If

        If errores = 0 And caja = 0 Then
            lblErr.Text = "El valor de caja no puede ser cero"
            lblErr.Visible = True
            errores = 1
        End If

        If errores = 0 And caja <= cantidad Then
            resultado = cantidad Mod caja
            If resultado > 1 Then
                errores = 1
                lblErr.Text = "El articulo tiene mas de 1 unidad para repartir en cajas"
                lblErr.Visible = True
            End If
        End If


        If errores = 0 And cantidad < caja Then
            lblErr.Text = "El valor de cantidad no puede ser menor al de la caja"
            lblErr.Visible = True
            errores = 1
        End If

        If errores = 0 Then
            lblErr.Text = ""
            lblErr.Visible = False
            ConsultaFolios("Actualizar")
            ConsultaFolios("Consultar")
            btnEtiq.Enabled = True
            'lblErr.Text = ""
            'lblErr.Visible = False
            txtReng.Text = ""
            txtDesc.Text = ""
            txtCaja.Text = ""
            txtCant.Text = ""
            txtLote.Text = ""
        End If


    End Sub


End Class