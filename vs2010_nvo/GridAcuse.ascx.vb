
Imports CrystalDecisions.Shared


Imports System.Data.SqlClient
Imports System.Data

Partial Class UserControlsGridAcuse
    Inherits System.Web.UI.UserControl
    Private ReadOnly _conexion As New Skytex.FacturaElectronica.Datos
    Private ReadOnly _myConnection As SqlConnection = _Conexion.MiConexion
    Dim _server As New _Server
    Dim DBdevelop As String = _Server.DBdevelop
    Dim DBproduc As String = _Server.DBproduc
    Dim DBCat As String = _Server.DBCat
    Dim DBPro As String = _Server.DBpro
    Dim _dtResultados As DataTable

#Region "Metodos y Funciones"
    Private Sub ConsultaFolios()
        'If Not IsPostBack Then
        Dim myDataAdapter = New SqlDataAdapter("sp_consAcuseXML", _myConnection)
        Dim ds As New DataSet()

        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()

        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", lblRFC.Text)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@tipo_docr", Me.ddlRecibo.SelectedValue)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", Me.ddlEmpresa.SelectedValue)

        Try
            myDataAdapter.Fill(ds, "sp_consAcuseXML")
            myDataAdapter.Dispose()
            _myConnection.Close()

            _dtResultados = ds.Tables.Item(0)


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
    End Sub



    Sub ObtieneDatos(ByVal rfc As String, ByVal serie As String, ByVal folio As String, _
                    ByVal contra As List(Of Skytex.FacturaElectronica.nuevas_facturas))
        Dim recibo As New Skytex.FacturaElectronica.nuevas_facturas
        Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", _myConnection)
        Dim DS As New DataSet()

        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfc)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", Int64.Parse(folio))
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", 1)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", "")

        Try
            myDataAdapter.Fill(DS, "sp_consInfXML")
            myDataAdapter.Dispose()
            _myConnection.Close()
            Dim tablaInfo As DataTable
            tablaInfo = DS.Tables.Item(0)
            Dim iError As Integer = CType(tablaInfo.Rows(0).Item("error").ToString.Trim, Integer)
            Dim msg = tablaInfo.Rows(0).Item("msg").ToString.Trim

            If iError = 0 Then
                recibo.num_fol = CType(tablaInfo.Rows(0).Item("num_fol_contra"), Integer)
                recibo.ef_cve = tablaInfo.Rows(0).Item("ef_cve").ToString.Trim
                recibo.tipo_doc = tablaInfo.Rows(0).Item("tipo_doc_contra").ToString.Trim
                contra.Add(recibo)
            End If

        Catch ex As Exception
            lblErr.Text = "Error al consultar sp_consInfXML"
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
        'End If
    End Sub


    Private Sub ImprimeReporte(ByVal efCve As String, ByVal folioContra As String, ByVal doctoContra As String)

        Dim rpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Const comodin As String = "?"
        Const fechacom As String = "1960-01-01 00:00:00.000"
        Dim exportPath As String = ""
        Dim reporteName As String = ""
        Dim claveRpt As Integer

        exportPath = folioContra + "_" + efCve
        If doctoContra = "ITABXP" Then
            reporteName = "Files/Irmov_itabxp.rpt"
            claveRpt = 2
        ElseIf doctoContra = "ITPASE" Or doctoContra = "ITRPAS" Then
            reporteName = "Files/irpasinst.rpt"
            claveRpt = 1
        Else
            reporteName = "Files/Irmov.rpt"
            claveRpt = 2
        End If
        rpt.Load(Server.MapPath(reporteName))
        'rpt.Refresh()
        Dim db As String = ""
        db = _conexion.ObtieneDb()
        'If db = "DEVELOP" Then
        If db = DBdevelop Then '//JPO: 27-06-16 valida el nombre de la instancia SQL a consultarl
            'rpt.DataSourceConnections(0).SetConnection("SKYHDEV", "develop", False)
            'rpt.DataSourceConnections(0).SetConnection("SKYHDEV3", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin", "pluma")
            'rpt.DataSourceConnections(0).SetConnection(DBdevelop, DBCat, False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            rpt.SetDatabaseLogon("soludin_develop", "dinamico20", "skyhdev3", "develop")
        End If
        If db = "SKYTEX" Then
            'If db = DBproduc Then
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
            'rpt.SetDatabaseLogon("soludin_develop", "dinamico20", "192.168.18.96", "skytex")
            rpt.DataSourceConnections(0).SetConnection("192.168.18.96", "skytex", False)
            rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
        End If
        If db = "SKYHJ" Then
            'rpt.DataSourceConnections(0).SetConnection("192.168.18.21", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin", "pluma")
            'rpt.DataSourceConnections(0).SetConnection("SQL", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            'rpt.DataSourceConnections(0).SetConnection(DBproduc, DBPro, False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            rpt.SetDatabaseLogon("soludin_develop", "dinamico20", "SKYHJ", "skytex")
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

        If doctoContra = "ITPASE" Or doctoContra = "ITRPAS" Then
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
        End If

        Try

            Response.Buffer = False
            Response.Clear()
            Response.ClearContent()
            Response.ClearHeaders()
            Response.ContentType = "application/pdf"
            rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, True, exportPath) ' aqui importando el espacio de nombres
            rpt.Close()

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

    Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
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

    Protected Sub btnCons_Click(sender As Object, e As System.EventArgs) Handles btnCons.Click
        Panel1.Visible = False
        ConsultaFolios()
    End Sub


    Private Sub LlenaCbos()

        Dim dsCboAcuse As New DataSet
        Dim dsCboEmpresa As New DataSet
        Dim cbo1 = New Skytex.FacturaElectronica.combos
        Dim cbo2 = New Skytex.FacturaElectronica.combos

        cbo1.rfc = lblRFC.Text
        cbo1.combo = "AcuseMercancia"
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

    Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)

        If e.CommandName = "Imprimir" Then

            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            Dim selectedRow As GridViewRow = GridView1.Rows(index)
            Dim lb As LinkButton = DirectCast(selectedRow.Cells(1).Controls(0), LinkButton)
            Dim folioAcuse As String = lb.Text.Trim
            ImprimeReporte(Me.ddlEmpresa.SelectedValue, folioAcuse, Me.ddlRecibo.SelectedValue)

        End If

    End Sub

    Protected Sub GridView1_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging

        GridView1.PageIndex = e.NewPageIndex
        ConsultaFolios()

    End Sub

End Class
