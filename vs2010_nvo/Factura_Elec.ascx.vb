
Imports System.IO
Imports CrystalDecisions.Shared
Imports System.Diagnostics.Eventing.Reader
Imports Skytex.FacturaElectronica
Imports System.Web.UI.WebControls
Imports System.Data
Imports CLPortalAbstract

Partial Class UserControlsRecepcion
    Inherits UserControl
    ReadOnly _factura As New Skytex.FacturaElectronica.Factura

    Dim _ruta As String = ""
    Dim _rutaTmp As String = ""
    Dim _rutaExis As String = ""
    Dim _rutaErr As String = ""
    Dim _eMails As String = ""
    Dim _decimalesTrucados As Integer = 0
    Dim _decimales As Integer = 0
    Private ReadOnly _conexion As New Skytex.FacturaElectronica.Datos
    Dim _ccCve, _ccTipo As String
    Dim _user As PerfilesUser = Nothing
    Dim _rutaTmpPas As String = ""
    Dim _server As New _Server
    Dim DBdevelop As String = _Server.DBdevelop
    Dim DBproduc As String = _Server.DBproduc
    Dim DBCat As String = _Server.DBCat
    Dim DBPro As String = _server.DBpro
    Private Sub LoadConfig()
        Dim config = CType(Page.Session("config"), List(Of ConfigGral))
        _ruta = _factura.GetConfigure(config, "Ruta").ToString()
        _rutaTmp = _factura.GetConfigure(config, "RutaTMP").ToString()
        _rutaExis = _factura.GetConfigure(config, "RutaExis").ToString()
        _eMails = _factura.GetConfigure(config, "Email_Admin").ToString()
        _rutaErr = _factura.GetConfigure(config, "RutaErr").ToString()
        _decimalesTrucados = CType(_factura.GetConfigure(config, "decimalTrun"), Integer)
        '_rutaTmpPas = _factura.GetConfigure(config, "TmpPas").ToString()
        _decimales = CType(_factura.GetConfigure(config, "decimales"), Integer)
    End Sub

#Region "Metodos"
    Private Sub PInicializaVariables()
        Page.Session("ContPDF") = 1
        Page.Session("NomPDF") = ""
        Page.Session("ListaPDF") = ""
        Page.Session("ContXML") = 1
        Page.Session("NomXML") = ""
        Page.Session("ListaXML") = ""
        'Page.Session("sw_Prov_Add") = 1 ' con addenda 0 sin addenda
        Page.Session("sw_captura") = 0
        '  rdlFactura.TabIndex = Page.Session("radio_valor")
    End Sub
    ' GCM borrar
    '    Private Sub LeeCFD(ByVal nombreArchivoXml As String, ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd, ByVal ccCve As String, ByVal ccTipo As String, ByVal rfcSession As String, ByVal decTrun As Integer, ByVal decimales As Integer)
    '    Dim er As New Errores
    '    Dim swErrins As Integer = 0
    '    Dim receptor = New receptor
    '    Try
    '        If errores.Count = 0 And _factura.iErrorG = 0 Then
    '            _factura.LeeDatosLlaveLinq(errores, llaveCfd, nombreArchivoXml, receptor)
    '        End If
    '    Catch ex As Exception
    '        _factura.agrega_err(3, ex.Message, errores)
    '    End Try
    'End Sub

    Private Sub PLeeCfd(ByVal nombreArchivoXml As String, ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd, ByVal ccCve As String, ByVal ccTipo As String, ByVal rfcSession As String, ByVal decTrun As Integer, ByVal decimales As Integer)
        Dim er As New Errores
        Dim swErrins As Integer = 0
        Dim receptor = New receptor
        Try
            If errores.Count = 0 And _factura.iErrorG = 0 Then
                _factura.LeeDatosLlaveLinq(errores, llaveCfd, nombreArchivoXml, receptor)
            End If
        Catch ex As Exception
            _factura.agrega_err(3, ex.Message, errores)
        End Try




        Try
            If errores.Count = 0 And _factura.iErrorG = 0 Then
                _factura.ConsInsertaXml(errores, llaveCfd, ccTipo, ccCve, receptor.rfc, comprobante.tipodoc_cve)
                If _factura.iErrorG = 1 Then
                    swErrins = 1
                End If
            End If
        Catch ex As Exception
            _factura.agrega_err(3, ex.Message, errores)
        End Try

        If errores.Count = 0 Then
            If rfcSession.Trim() <> llaveCfd.rfc_emisor Then
                'Dim nuevo = New List(Of Errores)
                er.Interror = 3 '1
                er.Message = "Delimitador rfc_session[" + rfcSession.Trim() + "]   rfc_comprobante [" + llaveCfd.rfc_emisor + "]"
                er.CveError = "60061"
                'nuevo.Add(er)
                '_factura.GenMailErrHtml(llaveCfd, "Error en Facturación Electronica", nuevo, "ricardo.garcia@skytex.com", llaveCfd.Proveedor)
                '_factura.agrega_err(1, "Comprobante con RFC incorrecto", errores, "60061")
                _factura.agrega_err(3, "Comprobante con RFC incorrecto", errores, "60061")
                _factura.graba_error(errores, er, llaveCfd, "60061", "PLeeCFD")
                _factura.iErrorG = 60061
            End If
        End If

        Try
            If errores.Count = 0 And _factura.iErrorG = 0 And llaveCfd.sw_sin_addenda = 1 Then
                _factura.ValidaXSDLinq_SNAdd(errores, nombreArchivoXml, llaveCfd)
            End If
        Catch ex As Exception
            _factura.agrega_err(3, ex.Message, errores)
        End Try

        Try
            If errores.Count = 0 And _factura.iErrorG = 0 And llaveCfd.sw_sin_addenda = 0 And comprobante.tipodoc_cve = "QTFAPN" Then
                _factura.ValidaXsdLinq(errores, nombreArchivoXml, llaveCfd)
            End If
        Catch ex As Exception
            _factura.agrega_err(3, ex.Message, errores)
        End Try

        Try
            If errores.Count = 0 And _factura.iErrorG = 0 And llaveCfd.sw_sin_addenda = 0 And comprobante.tipodoc_cve = "VTFE" Then
                _factura.ValidaXsdLinqFacEmb(errores, nombreArchivoXml, llaveCfd)
            End If
        Catch ex As Exception
            _factura.agrega_err(3, ex.Message, errores)
        End Try

        ' crear metodo para validar XSD de facturas de servicio
        Try
            If errores.Count = 0 And _factura.iErrorG = 0 And llaveCfd.sw_sin_addenda = 0 And comprobante.tipodoc_cve = "BTFACS" Then
                _factura.ValidaXsdLinqFacSrv(errores, nombreArchivoXml, llaveCfd)
            End If
        Catch ex As Exception
            _factura.agrega_err(3, ex.Message, errores)
        End Try


        If errores.Count = 0 And _factura.iErrorG = 0 And llaveCfd.sw_sin_addenda = 0 Then
            _factura.ValidaExisteAddenda(errores, nombreArchivoXml, llaveCfd)
        End If

        Try
            If errores.Count = 0 And _factura.iErrorG = 0 And llaveCfd.sw_sin_addenda = 0 And comprobante.tipodoc_cve = "QTFAPN" Then
                If llaveCfd.version = "3.3" Then
                    _factura.LeeDatosFacturaLinq3_3(errores, comprobante, nombreArchivoXml, llaveCfd, ccCve, ccTipo)
                Else
                    _factura.LeeDatosFacturaLinq(errores, comprobante, nombreArchivoXml, llaveCfd, ccCve, ccTipo)
                End If
                '_factura.LeeDatosFacturaLinq(errores, comprobante, nombreArchivoXml, llaveCfd, ccCve, ccTipo)
            End If
        Catch ex As Exception
            _factura.agrega_err(3, ex.Message, errores)
        End Try

        Try
            If errores.Count = 0 And _factura.iErrorG = 0 And llaveCfd.sw_sin_addenda = 0 And comprobante.tipodoc_cve = "VTFE" Then
                If llaveCfd.version = "3.3" Then
                    _factura.LeeDatosFacturaLINQ_FacEmb3_3(errores, comprobante, nombreArchivoXml, llaveCfd, ccCve, ccTipo)
                Else
                    _factura.LeeDatosFacturaLINQ_FacEmb(errores, comprobante, nombreArchivoXml, llaveCfd, ccCve, ccTipo)
                End If
                '_factura.LeeDatosFacturaLINQ_FacEmb(errores, comprobante, nombreArchivoXml, llaveCfd, ccCve, ccTipo)
            End If
        Catch ex As Exception
            _factura.agrega_err(3, ex.Message, errores)
        End Try

        'lectura de addenda para factura de servicios
        Try
            If errores.Count = 0 And _factura.iErrorG = 0 And llaveCfd.sw_sin_addenda = 0 And comprobante.tipodoc_cve = "BTFACS" Then
                If llaveCfd.version = "3.3" Then
                    _factura.LeeDatosFacturaLINQ_FacSrv3_3(errores, comprobante, nombreArchivoXml, llaveCfd, ccCve, ccTipo)
                Else
                    _factura.LeeDatosFacturaLINQ_FacSrv(errores, comprobante, nombreArchivoXml, llaveCfd, ccCve, ccTipo)
                End If
                '_factura.LeeDatosFacturaLINQ_FacSrv(errores, comprobante, nombreArchivoXml, llaveCfd, ccCve, ccTipo)
            End If
        Catch ex As Exception
            _factura.agrega_err(3, ex.Message, errores)
        End Try


        If errores.Count = 0 And _factura.iErrorG = 0 And llaveCfd.sw_sin_addenda = 0 And IsNothing(comprobante.Addenda) Then
            _factura.agrega_err(1, "Comprobante sin addenda", errores)
        End If

        Try
            If errores.Count = 0 And _factura.iErrorG = 0 And llaveCfd.sw_sin_addenda = 1 Then
                If llaveCfd.version = "3.3" Then
                    _factura.LeeDatosFacturaLINQ_SNAdd3_3(errores, comprobante, nombreArchivoXml, llaveCfd)
                Else
                    _factura.LeeDatosFacturaLINQ_SNAdd(errores, comprobante, nombreArchivoXml, llaveCfd)
                End If
                '_factura.LeeDatosFacturaLINQ_SNAdd(errores, comprobante, nombreArchivoXml, llaveCfd)
            End If
        Catch ex As Exception

            _factura.agrega_err(1, "Ocurrio un erro al leer el XML por favor contacte con el administrador de sitio ", errores)
        End Try

        Try
            If llaveCfd.sw_sin_addenda = 1 And errores.Count = 0 And _factura.iErrorG = 0 Then
                'llaveCfd.ef_cve = _factura.lee_ef_cve(receptor.rfc)
                Dim dicEmp As Dictionary(Of String, String) = _factura.lee_ef_cve(receptor.rfc)
                Dim pairK As KeyValuePair(Of String, String)
                For Each pairK In dicEmp
                    llaveCfd.ef_cve = pairK.Key
                    comprobante.Receptor.nombre = pairK.Value
                Next
            End If

        Catch ex As Exception
            _factura.agrega_err(3, ex.Message, errores)
        End Try

        Try
            If errores.Count = 0 And _factura.iErrorG = 0 And llaveCfd.sw_sin_addenda = 0 Then
                If llaveCfd.version = "3.3" Then
                    _factura.ValidaTotales3_3(errores, comprobante, llaveCfd, decTrun, decimales)
                Else
                    _factura.ValidaTotales(errores, comprobante, llaveCfd, decTrun, decimales)
                End If
                '_factura.ValidaTotales(errores, comprobante, llaveCfd, decTrun, decimales)
            End If

        Catch ex As Exception
            _factura.agrega_err(3, ex.Message.Trim() + "ValidaTotales sw addenda:" + llaveCfd.sw_sin_addenda.ToString(), errores)
        End Try

        Try
            If errores.Count = 0 And llaveCfd.sw_sin_addenda = 1 And _factura.iErrorG = 0 Then
                If llaveCfd.version = "3.3" Then
                    _factura.ValidaTotales_SNAdd3_3(errores, comprobante, llaveCfd, decTrun, decimales)
                Else
                    _factura.ValidaTotales_SNAdd(errores, comprobante, llaveCfd, decTrun, decimales)
                End If
                '_factura.ValidaTotales_SNAdd(errores, comprobante, llaveCfd, decTrun, decimales)
            End If

        Catch ex As Exception
            _factura.agrega_err(3, ex.Message + "ValidaTotales_SNAdd sw addenda:" + llaveCfd.sw_sin_addenda.ToString(), errores)
        End Try

        Dim msg60000 = From msg In errores _
            Select interror = msg.Interror, message = msg.Message _
            Where (interror = 60000) Or (interror = 1 And message.Trim = "")

        'If msg60000.Count <> 0 Then
        '    If swErrins = 1 And _factura.iErrorG > 0 And llaveCfd.folio_factura <> 0 And llaveCfd.rfc_emisor <> "" And llaveCfd.serie <> "" Then
        '        _factura.LeerErroresSql(errores, llaveCfd)
        '    End If
        'End If

    End Sub

    Private Sub GenFactura(ByVal nombreArchivoXml As String, ByVal nombreArchivoPdf As String, ByVal errores As List(Of Errores), ByVal items As List(Of lineitem), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)

        If errores.Count = 0 And _factura.iErrorG = 0 Then
            _factura.ValidaDatosEncabezado(errores, comprobante, llaveCfd)
        End If



        If errores.Count = 0 And _factura.iErrorG = 0 And comprobante.tipodoc_cve = "BTFACS" Then
            _factura.ValidaDatosDetalleSrv(errores, llaveCfd, comprobante.Addenda.requestforpayment.document)
        Else
            If errores.Count = 0 And _factura.iErrorG = 0 Then
                _factura.ValidaDatosDetalle(errores, items, llaveCfd)
            End If
        End If


        If errores.Count = 0 And _factura.iErrorG = 0 Then
            _factura.ValidaDatosPapa(errores, llaveCfd)
        End If

        Dim factura As New List(Of nuevas_facturas)

        If errores.Count = 0 And _factura.iErrorG = 0 Then
            _factura.GeneraEncabezadoFactura(errores, llaveCfd, factura)
            For Each fact In factura
                Try
                    'FileSystem.FileCopy(nombreArchivoXml, _ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".xml")
                    'FileSystem.FileCopy(nombreArchivoPdf, _ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".pdf")
                    If FileIO.FileSystem.FileExists(_ruta + fact.ef_cve + fact.tipo_doc + fact.num_fol.ToString() + ".pdf") = True Then
                        FileSystem.FileCopy(nombreArchivoXml, _rutaExis + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".xml")
                        FileSystem.FileCopy(nombreArchivoPdf, _rutaExis + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".pdf")
                    Else
                        FileSystem.FileCopy(nombreArchivoXml, _ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".xml")
                        FileSystem.FileCopy(nombreArchivoPdf, _ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".pdf")
                    End If
                Catch ex As Exception
                    _factura.agrega_err(3, ex.Message, errores)
                End Try
            Next
        End If

        If errores.Count = 0 And _factura.iErrorG = 0 Then
            _factura.GeneraContraReciboFactura(errores, llaveCfd)
        End If

        If errores.Count = 0 And _factura.iErrorG = 0 Then
            TxtMensajes.Text = ""
            TxtMensajeExitoso.Text = "La factura se ha procesado exitosamente, consulte la sección Seguimiento para descargar el folio de contrarecibo : " + _factura.FolioCr.ToString.Trim + vbNewLine
            BtnImprimir.Visible = True
            Page.Session("FolioContra") = _factura.FolioCr
            Page.Session("DoctoContra") = _factura.DoctoCr
            Page.Session("ef_cve") = _factura.EfCveG
            Page.Session("doc_p") = _factura.Recepcion
            PInicializaVariables()
        End If

        If _factura.iErrorG > 0 And errores.Count = 0 Then
            _factura.agrega_err(1, "", errores)
        End If

        If errores.Count > 0 And _factura.iErrorG > 0 Then
            _factura.LeerErroresSql(errores, llaveCfd)
        End If

        If _factura.LlamaSqlImpresion = 1 Then
            _factura.llama_errores_tipo2(errores, llaveCfd)
            _factura.LlamaSqlImpresion = 0
        End If

    End Sub



    Private Sub GenFacturaBis(ByVal nombreArchivoXml As String, ByVal nombreArchivoPdf As String, ByVal errores As List(Of Errores), ByVal items As List(Of lineitem), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)

        'If errores.Count = 0 And _factura.iErrorG = 0 Then



        _factura.GrabaTmp(errores, comprobante, llaveCfd)
        'End If

        'If errores.Count = 0 And _factura.iErrorG = 0 Then
        '    _factura.ValidaDatosEncabezado(errores, comprobante, llaveCfd)
        'End If

        If errores.Count = 0 And _factura.iErrorG = 0 Then
            _factura.ValidaDatosEncabezadoCap(errores, comprobante, llaveCfd)
        End If

        If errores.Count = 0 And _factura.iErrorG = 0 Then
            _factura.ValidaDatosDetalleCap(errores, items, llaveCfd)
        End If

        If errores.Count = 0 And _factura.iErrorG = 0 Then
            _factura.ValidaDatosPapa(errores, llaveCfd)
        End If

        Dim factura As New List(Of nuevas_facturas)

        'If comprobante.tipodoc_cve = "BTFSER" Then
        '    Exit Sub
        'End If

        If errores.Count = 0 And _factura.iErrorG = 0 Then
            _factura.GeneraEncabezadoFactura(errores, llaveCfd, factura)
            For Each fact In factura
                Try
                    'FileSystem.FileCopy(nombreArchivoXml, _ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".xml")
                    'FileSystem.FileCopy(nombreArchivoPdf, _ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".pdf")
                    If FileIO.FileSystem.FileExists(_ruta + fact.ef_cve + fact.tipo_doc + fact.num_fol.ToString() + ".pdf") = True Then
                        FileSystem.FileCopy(nombreArchivoXml, _rutaExis + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".xml")
                        FileSystem.FileCopy(nombreArchivoPdf, _rutaExis + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".pdf")
                    Else
                        FileSystem.FileCopy(nombreArchivoXml, _ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".xml")
                        FileSystem.FileCopy(nombreArchivoPdf, _ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".pdf")
                    End If
                Catch ex As Exception
                    _factura.agrega_err(3, ex.Message, errores)
                End Try
            Next
        End If

        If errores.Count = 0 And _factura.iErrorG = 0 Then
            _factura.GeneraContraReciboFactura(errores, llaveCfd)
        End If

        If errores.Count = 0 And _factura.iErrorG = 0 Then
            TxtMensajes.Text = ""
            TxtMensajeExitoso.Text = "La factura se ha procesado exitosamente, consulte la sección Seguimiento para descargar el folio de contrarecibo : " + _factura.FolioCr.ToString.Trim + vbNewLine
            BtnImprimir.Visible = True
            Page.Session("FolioContra") = _factura.FolioCr
            Page.Session("DoctoContra") = _factura.DoctoCr
            Page.Session("ef_cve") = _factura.EfCveG
            Page.Session("doc_p") = _factura.Recepcion
            PInicializaVariables()
        End If

        If _factura.iErrorG > 0 And errores.Count = 0 Then
            _factura.agrega_err(1, "", errores)
        End If

        If errores.Count > 0 And _factura.iErrorG > 0 Then
            _factura.LeerErroresSql(errores, llaveCfd)
        End If

        If _factura.LlamaSqlImpresion = 1 Then
            _factura.llama_errores_tipo2(errores, llaveCfd)
            _factura.LlamaSqlImpresion = 0
        End If

    End Sub


    Public Function RevisaIntTasa(ByVal tasa As String) As Boolean
        Dim sw As Boolean = True

        If IsNumeric(tasa) Then
            If CInt(tasa) Then
                sw = True
            Else
                sw = False
            End If
        End If

        Return sw
    End Function

    Private Sub llena_datos_Captura(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)

        borraInfoCfd_session(llaveCfd, "All")

        Dim moneda As String = ""
        Dim metododepago As String = ""
        If comprobante.moneda <> "" Then
            moneda = comprobante.moneda
        End If

        If comprobante.metodo_pago <> "" Then
            metododepago = comprobante.metodo_pago
        End If

        Dim iva = From cons In comprobante.Impuestos.Traslados
                   Select cons.tasa, cons.importe, cons.impuesto
                   Where (impuesto = "IVA")

        Dim tasaVarIva As Decimal = 0 '= comprobante.Impuestos.Traslados.tasa / 100
        Dim importeIva As Decimal = 0

        ' For Each i In iva
        'tasaVarIva = i.tasa '/ 100 'FormatNumber(i.tasa / 100, decimales)
        'importeIva = i.importe 'FormatNumber(i.importe, decimales)
        'Next

        For Each i In iva
            'tasaVarIva = i.tasa / 100 'FormatNumber(i.tasa / 100, decimales)
            Dim swtasa = RevisaIntTasa(i.tasa.ToString())

            If swtasa = True Then
                tasaVarIva = i.tasa / 100 'FormatNumber(i.tasa / 100, decimales)
            Else
                tasaVarIva = i.tasa
            End If

            importeIva = importeIva + i.importe 'FormatNumber(i.importe, decimales)
        Next

        Dim ieps = From cons In comprobante.Impuestos.Traslados
                  Select cons.tasa, cons.importe, cons.impuesto
                  Where (impuesto = "IEPS")

        Dim tasa_ieps As Decimal = 0  '= comprobante.Impuestos.Traslados.tasa / 100
        Dim importe_ieps As Decimal = 0

        For Each i In ieps
            tasa_ieps = i.tasa '/ 100 ' FormatNumber(i.tasa / 100, decimales)
            importe_ieps = i.importe ' FormatNumber(i.importe, decimales)
        Next

        Dim retenciones = From cons In comprobante.Impuestos.Retenciones
                Select cons.importe, cons.impuesto
                Where (impuesto = "IVA")

        Dim reten_importe_iva As Decimal = 0

        For Each i In retenciones
            reten_importe_iva = i.importe 'FormatNumber(i.importe, decimales)
        Next

        Dim retisr = From cons In comprobante.Impuestos.Retenciones
              Select cons.importe, cons.impuesto
              Where (impuesto = "ISR")

        Dim reten_importe_isr As Decimal = 0

        For Each i In retisr
            reten_importe_isr = i.importe 'FormatNumber(i.importe, decimales)
        Next

        Dim totalConceptos As Decimal = 0
        Dim importeConceptos As Decimal = 0

        Dim qryResLine = From com2 In comprobante.Conceptos _
               Let sub_total = com2.cantidad * com2.valor_unitario _
               Select sub_total, com2.importe, com2.no_identificacion, com2.cantidad, com2.valor_unitario '_

        totalConceptos = Aggregate com2 In qryResLine _
                                Into Sum(com2.sub_total)

        importeConceptos = Aggregate com2 In qryResLine _
                           Into Sum(com2.importe)



        Dim totalImpTrasl As Decimal

        'Dim Traslados = New List(Of traslado)


        If comprobante.Impuestos.total_imp_trasl = 0 Then
            totalImpTrasl = comprobante.Impuestos.Traslados.Aggregate(Of Decimal)(0, Function(current, item) current + item.importe)
        Else
            totalImpTrasl = comprobante.Impuestos.total_imp_trasl
        End If

        Page.Session("nombre_receptor") = comprobante.Receptor.nombre
        Page.Session("EmpresaTitle") = System.String.Empty
        Page.Session("subtotal") = comprobante.sub_total
        Page.Session("descuento") = comprobante.descuento
        Page.Session("total") = comprobante.total
        Page.Session("tot_imp_trasl") = totalImpTrasl
        Page.Session("tot_imp_ret") = comprobante.Impuestos.total_imp_reten
        Page.Session("tasa") = tasaVarIva
        Page.Session("importe") = importeIva
        Page.Session("total_conceptos") = totalConceptos
        Page.Session("importe_conceptos") = importeConceptos

        If comprobante.tipodoc_cve = "VTFE" Then
            Const swRetProv As Boolean = False
            Page.Session("sw_ret_prov") = swRetProv
            Dim dte = New DataTable()
            dte.Columns.Add(New DataColumn("Referencia", System.Type.GetType("System.String")))
            dte.Columns.Add(New DataColumn("NoIdentificacion", System.Type.GetType("System.String")))
            dte.Columns.Add(New DataColumn("Description", System.Type.GetType("System.String")))
            dte.Columns.Add(New DataColumn("Importe", System.Type.GetType("System.Decimal")))
            dte.AcceptChanges()
            Page.Session("dtTransportes") = dte

        End If
    End Sub

    Private Sub LlenaGridFactura()
        'Llena el combo de contratos, codigo original
        'rdlFactura.Items.AddRange((From a In _user.Perfil Select New ListItem(a.NomDoc, a.TipdocCve)).ToArray())
        'rdlFactura.Items(0).Selected = True

        'Llena el radiobutton GCM 05012015 

        Dim swfCom = 0
        Dim swfemb = 0
        Dim swFSFac = 0
        Dim swCont = 0
        Dim swSFact = 0
        Dim sw_unico = 0

        ' Dim lb1 As New LinkButton()
        'lb1.Text = "Algo"
        'lb1.re()
        'pnlOpciones.Controls.Add(lb1)

        '11122014 Obtenemos si el query nos trae algun resultado con esos documentos
        Dim Fcomp = _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "QTFAPN")
        Dim Femb = _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "VTFE")
        Dim FSFac = _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "BTFACS")
        Dim FCont = _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "BTFSER")
        Dim SFact = _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "SFACTU")

        Dim facComp As IEnumerable(Of DatosUser) = If(TryCast(Fcomp, List(Of DatosUser)), Fcomp.ToList())
        Dim facEmb As IEnumerable(Of DatosUser) = If(TryCast(Femb, List(Of DatosUser)), Femb.ToList())
        Dim facSFac As IEnumerable(Of DatosUser) = If(TryCast(FSFac, List(Of DatosUser)), FSFac.ToList())
        Dim facCont As IEnumerable(Of DatosUser) = If(TryCast(FCont, List(Of DatosUser)), FCont.ToList())
        Dim sinFact As IEnumerable(Of DatosUser) = If(TryCast(SFact, List(Of DatosUser)), SFact.ToList())
        '11122014 Pintamos los labels que sean necesarios

        If sinFact.Any() Then
            swSFact = 1
            lblMensaje.Visible = True
        End If

        If swFSFac = 0 Then
            If facComp.Any() Then
                swfCom = 1
                LnkFComp.Visible = True
            End If


            If facEmb.Any() Then
                LnkFEmb.Visible = True
                swfemb = 1
            End If

            If facSFac.Any() Then
                LnkFGto.Visible = True
                swFSFac = 1
            End If

            If facCont.Any() Then
                LnkFGto.Visible = True
                swCont = 1
            End If


            '11122014 Revisamos si solo trae un documento, para no pintar label y llenar la informacion desde este punto
            If swfCom = 1 And swfemb = 0 And swFSFac = 0 And swCont = 0 Then
                sw_unico = 1
                rdlFactura.Items.AddRange((From a In _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "QTFAPN") Select New ListItem(a.NomDoc, a.TipdocCve)).ToArray())
            End If

            If swfCom = 0 And swfemb = 1 And swFSFac = 0 And swCont = 0 Then
                sw_unico = 1
                rdlFactura.Items.AddRange((From a In _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "VTFE") Select New ListItem(a.NomDoc, a.TipdocCve)).ToArray())
            End If

            If swfCom = 0 And swfemb = 0 And (swFSFac = 1 Or swCont = 1) Then
                sw_unico = 1
                If swFSFac = 1 Then
                    rdlFactura.Items.AddRange((From a In _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "BTFACS") Select New ListItem(a.NomDoc, a.TipdocCve)).ToArray())
                End If
                If swCont = 1 Then
                    rdlFactura.Items.AddRange((From a In _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "BTFSER") Select New ListItem(a.NomDoc, a.TipdocCve)).ToArray())
                End If
            End If


            If sw_unico = 1 Then
                rdlFactura.Items(0).Selected = True
                lblrdlFac.Visible = True
                LnkFComp.Visible = False
                LnkFEmb.Visible = True
                LnkFGto.Visible = False
                pnlOpciones.Visible = False
                lblXML.Visible = True
                AdjXML.Visible = True
                lblPDF.Visible = True
                AdjPDF.Visible = True
                upUfiles.Visible = True
                LnkFEmb.Enabled = False
                BtnAdjuntarXML.Visible = True
                If swCont = 1 Then
                    lblContrato.Visible = True
                    If ddlContrato.Items.Count > 0 Then
                        ddlContrato.Visible = True
                    End If
                End If
            Else
                lblXML.Visible = False
                AdjXML.Visible = False
                lblPDF.Visible = False
                AdjPDF.Visible = False
            End If
        End If

    End Sub

    Private Function borraInfoCfd_session(ByVal llaveCfd As llave_cfd, ByVal opc As String) As Integer

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

        Return 0

    End Function

    Private Sub ImprimeReporte(ByVal efCve As String, ByVal folioContra As String, ByVal doctoContra As String)
        Dim rpt As New CrystalDecisions.CrystalReports.Engine.ReportDocument
        Const comodin As String = "?"
        Const fechacom As String = "1960-01-01 00:00:00.000"
        Dim exportPath As String = ""
        exportPath = folioContra + "_" + efCve '+ ".pdf"
        Dim db As String = ""
        db = _conexion.ObtieneDb()

        'If db = "DEVELOP" Then
        If db = DBdevelop Then '//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
            rpt.Load(Server.MapPath("Files/QRCRWEBdev.rpt"))
            rpt.Refresh()
            'rpt.DataSourceConnections(0).SetConnection("SKYHDEV", "develop", False)
            'rpt.DataSourceConnections(0).SetConnection("SKYHDEV3", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin", "pluma")'JPO: 27/06/2016 
            'rpt.DataSourceConnections(0).SetConnection(DBdevelop, DBCat, False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            rpt.SetDatabaseLogon("soludin_develop", "dinamico20", "skyhdev3", "develop")
        End If
        If db = "192.168.18.96" Then '//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
            rpt.Load(Server.MapPath("Files/QRCRWEBskyhsql.rpt"))
            rpt.Refresh()
            'rpt.DataSourceConnections(0).SetConnection("SKYHDEV", "develop", False)
            'rpt.DataSourceConnections(0).SetConnection("SKYHDEV3", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin", "pluma")'JPO: 27/06/2016 
            'rpt.DataSourceConnections(0).SetConnection(DBdevelop, DBCat, False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            rpt.SetDatabaseLogon("soludin_develop", "dinamico20", "192.168.18.96", "skytex")
        End If

        'If db = "SKYTEX" Then
        If db = DBproduc Then
            rpt.Load(Server.MapPath("Files/QRCRWEB.rpt"))
            rpt.Refresh()
            'rpt.DataSourceConnections(0).SetConnection("192.168.18.21", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin", "pluma")
            'rpt.DataSourceConnections(0).SetConnection("SQL", "skytex", False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            'rpt.DataSourceConnections(0).SetConnection(DBproduc, DBPro, False)
            'rpt.DataSourceConnections(0).SetLogon("soludin_develop", "dinamico20")
            rpt.SetDatabaseLogon("soludin_develop", "dinamico20", "SQL", "skytex")
        End If

        'Paso de parametros
        rpt.SetParameterValue("@ef_cve", efCve)
        rpt.SetParameterValue("@tipdoc_cve", doctoContra)
        rpt.SetParameterValue("@fol_ini", folioContra)
        rpt.SetParameterValue("@fol_fin", folioContra)
        rpt.SetParameterValue("@cve_rpt", 4)
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
#End Region

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not (Page.IsPostBack) And Not Page.PreviousPage Is Nothing Then
            'Dim cp As ContentPlaceHolder = CType(Page.PreviousPage.Master.FindControl("MainContentPlaceHolder"), ContentPlaceHolder)
            'Dim loginControl As UserControl = CType(cp.FindControl("ctlLogin"), UserControl)
            'Dim userName1 As Login = DirectCast(loginControl.FindControl("Login1"), Login)
            Page.Session("ContPDF") = 1
            Page.Session("ContXML") = 1
            Page.Session("sw_captura") = 0
            'ScriptManager1.RegisterPostBackControl(BtnAdjuntarXML)


        End If

        If Not (Page.IsPostBack) Then
            If IsNothing(_user) Then
                _user = CType(Page.Session("user"), PerfilesUser)
            End If

            Contratos()
            LlenaGridFactura()
            LoadConfig()
        End If



    End Sub

    Sub Contratos()
        Dim datosCombo As New combos
        datosCombo.combo = "contratos"
        datosCombo.rfc = Page.Session("RFC").ToString()
        Dim swcombo As Boolean = False

        Dim dicEmp As Dictionary(Of String, String) = ObtCves()
        Dim pairK As KeyValuePair(Of String, String)
        For Each pairK In dicEmp
            datosCombo.parametro1 = pairK.Key
            datosCombo.parametro2 = pairK.Value
            swcombo = True
        Next


        If swcombo = True Then
            'Page.Session("datosCombo") = datosCombo
            'LlenaCbo(datosCombo)
            Dim dt = _factura.LlenaCbo(datosCombo)
            Dim lista As New List(Of comboItem)

            If dt.Rows.Count > 0 Then

                'ddlContrato.DataSource = dt.DefaultView
                'ddlContrato.DataTextField = "descripcion"
                'ddlContrato.DataValueField = "valor"
                'ddlContrato.DataBind()

                For Each o As DataRow In dt.Rows
                    Dim item As New comboItem
                    item.Descripcion = o("descripcion").ToString()
                    item.Valor = o("valor").ToString()
                    lista.Add(item)
                    'ddlContrato.Items.Add(New ListItem(o("descripcion").ToString(), o("valor").ToString()))
                Next
            End If
            Page.Session("listaCbo") = lista
            'Page.Session("datosCombo") = datosCombo

            LlenaCbo()
        End If
    End Sub

    Private Function ObtCves() As Dictionary(Of String, String)
        Dim ccCve = New Dictionary(Of String, String)
        Dim tipo As String = ""
        Dim cve As String = ""

        If IsNothing(_user) Then
            _user = CType(Page.Session("user"), PerfilesUser)
        End If

        Dim query = _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "BTFSER")


        Dim datosUsers As IEnumerable(Of DatosUser) = If(TryCast(query, List(Of DatosUser)), query.ToList())
        If datosUsers.Any() Then
            datosUsers.Single().Seleccionado = True
        End If



        Dim consulta = _user.Perfil.Where(Function(q As DatosUser) q.Seleccionado = True)

        For Each l In consulta
            tipo = l.CcCve.Trim()
            cve = l.CcTipo.Trim()
        Next


        'Page.Session("user") = _user

        If datosUsers.Any() Then
            datosUsers.Single().Seleccionado = False
        End If

        ccCve.Add(tipo, cve)



        Page.Session("user") = _user

        Return ccCve

    End Function
    Private Sub LlenaCbo()


        If (Session("listaCbo") IsNot Nothing) Then
            Dim lista As New List(Of comboItem)
            lista = CType(Page.Session("listaCbo"), List(Of comboItem))


            If lista.Count > 0 Then

                ddlContrato.DataSource = lista
                ddlContrato.DataTextField = "descripcion"
                ddlContrato.DataValueField = "valor"
                ddlContrato.DataBind()
                ddlContrato.Items(0).Selected = True
            Else
                lblContrato.Text = "No existen contratos con este proveedor"
            End If

        End If



    End Sub

    Private Sub BtnImprimir_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnImprimir.Click
        ImprimeReporte(Page.Session("ef_cve").ToString(), Page.Session("FolioContra").ToString(), Page.Session("DoctoContra").ToString())
        Page.Session("FolioContra") = ""
        Page.Session("DoctoContra") = ""
        Page.Session("ef_cve") = ""
    End Sub

    Private Sub BtnAdjuntarXML_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnAdjuntarXML.Click

        Dim tipoXml As String = ""
        Dim tipoPdf As String = ""
        Dim tipdocCve As String = ""
        Dim errores As New List(Of Errores)
        Dim items = New List(Of lineitem)
        Dim comprobante = New Comprobante
        Dim llaveCfd = New llave_cfd
        Dim contra As New List(Of Skytex.FacturaElectronica.nuevas_facturas)
        Dim iErrorVal As Integer = 0
        Dim rfcSession As String
        Dim selInd As Integer = Page.Session("radio_valor")


        LoadConfig()

        TxtMensajes.Text = ""
        TxtMensajeExitoso.Text = ""
        BtnImprimir.Visible = False


        If Page.Session("RFC").ToString().Trim() = "" And Session("USRswUsrInterno") = 0 Then
            Response.Redirect("Login.aspx")
            Exit Sub
        End If

        If Page.Session("RFC").ToString().Trim() = "" And Session("USRswUsrInterno") = 1 Then
            Response.Redirect("UserLogin.aspx")
            Exit Sub
        End If

        rfcSession = Page.Session("RFC").ToString.Trim()

        'GCM 27112014 llena radio button
        Dim docFactura As String = rdlFactura.SelectedValue 'DirectCast(Page.FindControl("rdlFactura"), RadioButtonList).SelectedValue

        If IsNothing(_user) Then
            _user = CType(Page.Session("user"), PerfilesUser)
        End If

        Dim query = _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = docFactura)

        Dim datosUsers As IEnumerable(Of DatosUser) = If(TryCast(query, List(Of DatosUser)), query.ToList())
        If datosUsers.Any() Then
            datosUsers.Single().Seleccionado = True
        End If


        Page.Session("user") = _user
        Dim consulta = _user.Perfil.Where(Function(q As DatosUser) q.Seleccionado = True And q.TipdocCve = docFactura)
        'Dim consulta = _user.Perfil.Where(Function(q As DatosUser) q.Seleccionado = True)GCM 29042015 Codigo original agregue el tipdoc_cve

        For Each l In consulta
            _ccCve = l.CcCve.Trim()
            _ccTipo = l.CcTipo.Trim()
            tipdocCve = l.TipdocCve.Trim()
        Next


        Page.Session("cc_tipo") = _ccTipo
        Page.Session("cc_cve") = _ccCve
        Page.Session("tipdoc_cve") = tipdocCve


        If IsDBNull(_ccTipo) Or IsDBNull(_ccCve) Then
            If Session("USRswUsrInterno") = 0 Then
                Response.Redirect("Login.aspx")
                Exit Sub
            End If

            If Session("USRswUsrInterno") = 1 Then
                Response.Redirect("UserLogin.aspx")
                Exit Sub
            End If

        End If


        'ahora en lugar de cc_Cve, solo ocupar rfc y cc_tipo para regresar a pantalla de login 


        'If tipdocCve = "BTFSER" And ddlContrato.SelectedValue = "" Then
        If tipdocCve = "BTFSER" And ddlContrato.SelectedIndex < 0 Then
            TxtMensajes.Text = "Debe seleccionar un numero de contrato" + vbNewLine
            iErrorVal = 1
        End If

        'GCM 05012015
        'If tipdocCve = "BTFSER" And ddlMetodo.SelectedIndex < 1 Then
        '    TxtMensajes.Text = "Debe seleccionar un metodo de pago" + vbNewLine
        '    iErrorVal = 1
        'End If

        If AdjXML.HasFile = False Then
            TxtMensajes.Text = "Seleccione archivo XML." + vbNewLine
            iErrorVal = 1
        End If


        If AdjXML.HasFile = True Then
            tipoXml = System.IO.Path.GetExtension(AdjXML.FileName).ToUpper
        End If

        If tipoXml <> ".XML" Then
            TxtMensajes.Text = "Extencion de archivo incorrecta debe ser XML." + vbNewLine
            iErrorVal = 1
        End If

        If AdjXML.FileName.IndexOf(" ", StringComparison.Ordinal) > 0 Then
            TxtMensajes.Text = "El nombre del archivo XML esta incorrecto no debe contener espacios" + vbNewLine
            iErrorVal = 1
        End If

        If AdjPDF.FileName.IndexOf(" ", StringComparison.Ordinal) > 0 Then
            TxtMensajes.Text = "El nombre del archivo PDF esta incorrecto no debe contener espacios" + vbNewLine
            iErrorVal = 1
        End If

        'If iErrorVal = 0 And tipoXml = ".XML" Then
        '    Page.Session("NomXML") = Replace(AdjXML.FileName.ToUpper, ".XML", "") ' Trim(Replace(AdjXML.FileName.ToUpper, ".XML", ""))
        '    'AdjXML.SaveAs(_rutaTmp + AdjXML.FileName) 'GCM 25112014 comentado para pruebas original
        '    AdjXML.SaveAs("\\fsb\CFDWeb\TmpPas\" + Page.Session("RFC").ToString().Trim() + AdjXML.FileName) 'GCM 25112014 Prueba
        '    'LeeCFD("\\fsb\CFDWeb\TmpPas\" + Page.Session("RFC").ToString().Trim() + Page.Session("NomXML").ToString() + ".xml", errores, comprobante, llaveCfd, _ccCve, _ccTipo, rfcSession, _decimalesTrucados, _decimales) 'GCM 25112014 Pruebas

        'End If

        If iErrorVal = 0 And tipoXml = ".XML" Then
            'tipo = System.IO.Path.GetExtension(AdjXML.FileName).ToUpper
            Page.Session("NomXML") = Replace(AdjXML.FileName.ToUpper, ".XML", "") ' Trim(Replace(AdjXML.FileName.ToUpper, ".XML", ""))
            'AdjXML.SaveAs(_rutaTmp + AdjXML.FileName) 'GCM 25112014 comentado para pruebas original
            AdjXML.SaveAs(_rutaTmp + Page.Session("RFC").ToString().Trim() + AdjXML.FileName) 'GCM 25112014 Prueba
            Page.Session("ListaXML") = AdjXML.FileName
            Page.Session("ContXML") = Page.Session("ContXML") + 1
            TxtMensajes.Text = vbNewLine + Page.Session("ListaXML").ToString()
        End If

        If Page.Session("NomXML") = "" And iErrorVal = 0 Then
            TxtMensajes.Text = "Debe seleccionar archivo XML" + vbNewLine
            iErrorVal = 1
        End If


        If AdjPDF.HasFile = False Then
            TxtMensajes.Text = "Seleccione archivo PDF." + vbNewLine
            iErrorVal = 1
        End If


        If AdjPDF.HasFile = True Then
            tipoPdf = System.IO.Path.GetExtension(AdjPDF.FileName).ToUpper
        End If

        If tipoPdf <> ".PDF" Then
            TxtMensajes.Text = "Extencion de archivo incorrecta debe ser PDF." + vbNewLine
            iErrorVal = 1
        End If



        'GCM 24112014 aqui se hace la inseccion del xml y pdf

        If iErrorVal = 0 And tipoPdf = ".PDF" Then
            Page.Session("NomPDF") = Replace(AdjPDF.FileName.ToUpper, ".PDF", "") 'Trim(Replace(AdjPDF.FileName.ToUpper, ".PDF", ""))
            'AdjPDF.SaveAs(_rutaTmp + AdjPDF.FileName) 'GCM 25112014 comentado para pruebas original
            AdjPDF.SaveAs(_rutaTmp + Page.Session("RFC").ToString().Trim() + AdjPDF.FileName) 'GCM 25112014 pruebas
            Page.Session("ListaPDF") = AdjPDF.FileName
            Page.Session("ContPDF") = Page.Session("ContPDF") + 1
            TxtMensajes.Text = vbNewLine + Page.Session("ListaPDF").ToString()
        End If

        If Page.Session("NomPDF") = "" And iErrorVal = 0 Then
            TxtMensajes.Text = "Debe seleccionar archivo PDF" + vbNewLine
            iErrorVal = 1
        End If



        If iErrorVal = 0 Then
            If Page.Session("NomXML") <> Page.Session("NomPDF") Then
                TxtMensajes.Text = "Los archivos XML y PDF deben tener el mismo nombre. " + vbNewLine + "Los archivos no fueron recibidos, intente nuevamente." + vbNewLine + "XML: " + Page.Session("NomXML").ToString() + ".xml" + vbNewLine + "PDF: " + Page.Session("NomPDF").ToString() + ".pdf" + vbNewLine
                'File.Delete(_rutaTmp + Page.Session("NomXML").ToString() + ".XML") 'GCM 25112014 comentado para pruebas original
                'File.Delete(_rutaTmp + Page.Session("NomPDF").ToString() + ".PDF") 'GCM 25112014 comentado para pruebas original
                File.Delete(_rutaTmp + Page.Session("RFC").ToString().Trim() + Page.Session("NomXML").ToString() + ".XML") 'GCM 25112014 comentado para pruebas
                File.Delete(_rutaTmp + Page.Session("RFC").ToString().Trim() + Page.Session("NomXML").ToString() + ".PDF") 'GCM 25112014 comentado para pruebas

            End If
        End If

        'GCM 21102014 No permite nombres con caracteres especiales

        If iErrorVal = 0 Then
            Dim CadenaLimpia = Len(Regex.Replace(AdjXML.FileName, "[^A-Za-z0-9_]", ""))
            Dim CadenaNormal = Len(AdjXML.FileName) - 1
            If CadenaLimpia <> CadenaNormal Then
                TxtMensajes.Text = "Nombre del archivo incorrecto, verificar:" + Page.Session("NomXML")
                'File.Delete(_rutaTmp + Page.Session("NomXML").ToString() + ".XML") 'GCM 25112014 comentado para pruebas original
                'File.Delete(_rutaTmp + Page.Session("NomPDF").ToString() + ".PDF") 'GCM 25112014 comentado para pruebas original
                File.Delete(_rutaTmp + Page.Session("RFC").ToString().Trim() + Page.Session("NomXML").ToString() + ".XML") 'GCM 25112014 comentado para pruebas
                File.Delete(_rutaTmp + Page.Session("RFC").ToString().Trim() + Page.Session("NomXML").ToString() + ".PDF") 'GCM 25112014 comentado para pruebas
                iErrorVal = 1
            End If

        End If

        If Not Directory.Exists(_rutaTmp) And iErrorVal = 0 Then
            TxtMensajes.Text = "Por favor comuniquese con el departamento de Sistemas." + vbNewLine
            iErrorVal = 1
        End If

        If tipdocCve = "TTFLT" And iErrorVal = 0 Then
            TxtMensajes.Text = "Seleccione otro tipo de factura" + vbNewLine
            iErrorVal = 1
        End If


        If iErrorVal = 0 Then
            'If File.Exists(_rutaTmp + AdjXML.FileName) Then 'GCM 25112014 original pruebas
            If File.Exists(_rutaTmp + Page.Session("RFC").ToString().Trim() + AdjXML.FileName) Then 'GCM 25112014 pruebas

                'inicia animacion se ajax
                System.Threading.Thread.Sleep(8000)

                Dim archivo As String
                archivo = Trim(Replace(AdjXML.FileName.ToUpper, ".XML", ""))
                'archivo = Trim(Replace(archivo, ".PDF", "")) 'GCM 25112014 para pruebas original
                archivo = Page.Session("RFC").ToString().Trim() + Trim(Replace(archivo, ".PDF", "")) 'GCM 25112014 prueba

                comprobante.nom_arch = archivo
                llaveCfd.nom_arch = archivo
                llaveCfd.Proveedor = Page.Session("nomProveedor").ToString()
                llaveCfd.sw_sin_addenda = 0
                comprobante.cc_tipo = _ccTipo
                comprobante.cc_cve = _ccCve
                comprobante.tipodoc_cve = tipdocCve
                'PLeeCfd(_rutaTmp + Page.Session("NomXML").ToString() + ".xml", errores, comprobante, llaveCfd, _ccCve, _ccTipo, rfcSession, _decimalesTrucados, _decimales) 'GCM 25112014 Pruebas original
                PLeeCfd(_rutaTmp + Page.Session("RFC").ToString().Trim() + Page.Session("NomXML").ToString() + ".xml", errores, comprobante, llaveCfd, _ccCve, _ccTipo, rfcSession, _decimalesTrucados, _decimales) 'GCM 25112014 Pruebas

                If llaveCfd.sw_sin_addenda = 1 And errores.Count = 0 Then
                    _factura.ObtieneDatos(llaveCfd.rfc_emisor, llaveCfd.serie, llaveCfd.folio_factura.ToString(), llaveCfd.timbre_fiscal.uuid, contra, 2, errores)
                    If errores.Count = 0 Then
                        llaveCfd.sw_tmp = 1
                    Else
                        llaveCfd.sw_tmp = 0
                        errores.Clear()
                        _factura.iErrorG = 0
                    End If

                    If tipdocCve = "BTFSER" Then
                        llaveCfd.sw_tmp = 1
                    End If

                End If




                If llaveCfd.sw_sin_addenda = 1 And errores.Count = 0 And llaveCfd.sw_tmp = 0 Then
                    comprobante.cc_tipo = _ccTipo
                    comprobante.cc_cve = _ccCve
                    comprobante.Addenda.requestforpayment.provider.providerid = comprobante.cc_tipo + "@" + comprobante.cc_cve + "@" + llaveCfd.ef_cve
                    llena_datos_Captura(errores, comprobante, llaveCfd)
                    Page.Session("comprobante") = comprobante
                    Page.Session("LlaveCFD") = llaveCfd
                    PInicializaVariables()

                    If tipdocCve = "BTFACS" Then
                        Response.Redirect("CapturaSrv.aspx")
                        Exit Sub
                    End If
                    If tipdocCve = "VTFE" Then
                        Response.Redirect("CapturaEmb.aspx")
                        Exit Sub
                    End If
                    If tipdocCve = "QTFAPN" Then
                        Response.Redirect("Captura.aspx")
                        Exit Sub
                    End If

                End If

                If errores.Count = 0 And llaveCfd.sw_sin_addenda = 0 Then
                    items = comprobante.Addenda.requestforpayment.line_items
                    'GenFactura(_rutaTmp + Page.Session("NomXML").ToString() + ".xml", _rutaTmp + Page.Session("NomPDF").ToString() + ".pdf", errores, items, comprobante, llaveCfd) 'GCM 25112014 pruebas original
                    GenFactura(_rutaTmp + Page.Session("RFC").ToString().Trim() + Page.Session("NomXML").ToString() + ".xml", _rutaTmp + Page.Session("RFC").ToString().Trim() + Page.Session("NomPDF").ToString() + ".pdf", errores, items, comprobante, llaveCfd) 'GCM 25112014 prueba nuevo
                End If

                If tipdocCve = "BTFSER" And llaveCfd.sw_sin_addenda = 1 Then
                    Dim sdoc = New Document
                    Dim addenda = New addenda
                    Dim requesForPayment = New requestforpayment
                    Dim provider = New provider
                    Dim aditionalData = New aditionaldata
                    Dim paymentTimePeriod = New paymenttimeperiod
                    'Dim valor = ddlContrato.SelectedValue.ToString()
                    'TTPGPR
                    sdoc.referenceIdentification = "TTPGPR@" & Format(CType(ddlContrato.SelectedValue.ToString(), Double), "000000") 'valor.ToString(CType("000000", IFormatProvider))
                    'sdoc.referenceIdentification = tipdocCve & "@" & Format(CType(ddlContrato.SelectedValue.ToString(), Double), "000000") 'valor.ToString(CType("000000", IFormatProvider))
                    requesForPayment.document = sdoc
                    addenda.requestforpayment = requesForPayment
                    comprobante.Addenda = addenda
                    comprobante.cc_tipo = _ccTipo
                    comprobante.cc_cve = _ccCve
                    provider.providerid = comprobante.cc_tipo + "@" + comprobante.cc_cve + "@" + llaveCfd.ef_cve
                    comprobante.Addenda.requestforpayment.provider = provider
                    aditionalData.text_data = "z"
                    aditionalData.metododepago = "NO IDENTIFICADO" 'GCM 05012015 ddlMetodo.SelectedValue
                    'GCM 14112014 se contemplo moneda nacional
                    If comprobante.moneda <> "" Then
                        If comprobante.moneda = "MONEDA NACIONAL" Then
                            aditionalData.moneda = "MXN"
                        Else
                            aditionalData.moneda = comprobante.moneda
                        End If
                    Else
                        aditionalData.moneda = "MXN"
                    End If
                    comprobante.Addenda.requestforpayment.aditional_data = aditionalData
                    paymentTimePeriod.timeperiod = 0
                    comprobante.Addenda.requestforpayment.paymenttimeperiod = paymentTimePeriod
                    'GenFacturaBis(_rutaTmp + Page.Session("NomXML").ToString() + ".xml", _rutaTmp + Page.Session("NomPDF").ToString() + ".pdf", errores, items, comprobante, llaveCfd) 'GCM 25112014 original pruebas
                    GenFacturaBis(_rutaTmp + Page.Session("RFC").ToString().Trim() + Page.Session("NomXML").ToString() + ".xml", _rutaTmp + Page.Session("RFC").ToString().Trim() + Page.Session("NomPDF").ToString() + ".pdf", errores, items, comprobante, llaveCfd) ' GCM 25112014 comentado pruebas

                End If

                If errores.Count > 0 Then
                    TxtMensajes.Text = ""
                    Dim msgUsr = From msg In errores _
                                 Where (msg.Interror = 1 And msg.Message.Trim() <> "") _
                                Select msg
                    Dim msgAdmin = From msg In errores _
                            Where (msg.Interror = 3 And msg.Message.Trim() <> "") Or (msg.Interror = 2 And msg.Message.Trim() <> "")
                            Select msg
                    For Each msgs In msgUsr
                        TxtMensajes.Text = TxtMensajes.Text & msgs.Message + vbNewLine
                    Next
                    Dim msgMails = From msg In errores _
                           Where (msg.Interror = 1 And msg.Message.Trim() <> "") _
                           Select msg

                    Dim swMail As Boolean = msgUsr.Any()
                    Dim swMailAd As Boolean = msgAdmin.Any()

                    If swMail = True And msgMails.Count() <> 0 Then
                        _factura.GenMailErrHtml(llaveCfd, "Error al recibir Factura Electrónica", msgMails.ToList(), _eMails, Page.Session("nomProveedor").ToString())
                    End If

                    If swMailAd = True And msgAdmin.Count() <> 0 Then
                        _factura.GenMailErrHtml(llaveCfd, "Error al recibir Factura Electrónica", msgAdmin.ToList(), _eMails, Page.Session("nomProveedor").ToString())
                    End If

                    Dim cadena As String = msgAdmin.Aggregate("", Function(current, msgs) current & msgs.Message + vbNewLine)

                    If cadena <> String.Empty Then
                        'If _factura.iErrorG > 0 And TxtMensajes.Text = "" And _factura.MensajeError <> "" Then
                        '    TxtMensajes.Text = _factura.MensajeError
                        'End If
                        If _factura.iErrorG > 0 And TxtMensajes.Text = "" Then
                            TxtMensajes.Text = cadena
                        End If
                    End If

                    PInicializaVariables()
                End If
                'GCM comentar para que no me de error
                If errores.Count > 0 Then
                    TxtMensajeExitoso.Text = ""
                    FileSystem.FileCopy(_rutaTmp + archivo + ".xml", _rutaErr + archivo + ".xml")
                    FileSystem.FileCopy(_rutaTmp + archivo + ".pdf", _rutaErr + archivo + ".pdf")
                End If
            End If
        End If


        If iErrorVal <> 0 Then
            PInicializaVariables()
        End If

        TxtMensajes.Enabled = True



        'Catch ex As Exception

        '    _factura.MailGenericBis("subArch", ex.Message, "ricardo.garcia@skytex.com.mx")
        'End Try

    End Sub

    Sub rdlFactura_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rdlFactura.SelectedIndexChanged

        'If TxtMensajes.Text = "No existen contratos con este proveedor" Then
        '    TxtMensajes.Text = ""
        'End If
        'GCM 02122014 agregar variable sesion 
        Page.Session("radio_valor") = rdlFactura.SelectedIndex


        If rdlFactura.SelectedValue = "BTFSER" Then
            lblContrato.Visible = True
            If ddlContrato.Items.Count > 0 Then
                ddlContrato.Visible = True
            End If
            'lblMetodo.Visible = True 'GCM 05012015
            'ddlMetodo.Visible = True 'GCM 05012015
        Else
            lblContrato.Visible = False
            ddlContrato.Visible = False
            'lblMetodo.Visible = False 'GCM 05012015
            'ddlMetodo.Visible = False 'GCM 05012015
        End If
    End Sub

    Protected Sub LnkFComp_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LnkFComp.Click
        llenaCboFact("QTFAPN")

    End Sub
    Protected Sub LnkFEmb_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LnkFEmb.Click
        llenaCboFact("VTFE")
    End Sub
    Protected Sub LnkFGto_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LnkFGto.Click
        rdlFactura.Items.Clear()
        _user = CType(Page.Session("user"), PerfilesUser)

        Dim FSFac = _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "BTFACS")
        Dim FCont = _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "BTFSER")

        Dim facSFac As IEnumerable(Of DatosUser) = If(TryCast(FSFac, List(Of DatosUser)), FSFac.ToList())
        Dim facCont As IEnumerable(Of DatosUser) = If(TryCast(FCont, List(Of DatosUser)), FCont.ToList())
        Dim colSelec = "#D3D3D3"
        Dim colMenu = "#f2f2f2"
        pnlRFac.CssClass = "fondo"
        pnlupfile.CssClass = "fondo"
        rdlFactura.Items.Clear()
        LnkFComp.BackColor = Drawing.ColorTranslator.FromHtml(colMenu)
        LnkFEmb.BackColor = Drawing.ColorTranslator.FromHtml(colMenu)
        LnkFGto.BackColor = Drawing.ColorTranslator.FromHtml(colSelec)
        lblPDF.BackColor = Drawing.ColorTranslator.FromHtml(colSelec)

        If facSFac.Any() Then
            rdlFactura.Items.AddRange((From a In _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "BTFACS") Select New ListItem(a.NomDoc, a.TipdocCve)).ToArray())
        End If

        If facCont.Any() Then
            rdlFactura.Items.AddRange((From a In _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "BTFSER") Select New ListItem(a.NomDoc, a.TipdocCve)).ToArray())
            lblContrato.Visible = True
            ddlContrato.Visible = True
        End If


        lblrdlFac.Visible = True
        lblXML.Visible = True
        lblXML.Visible = True
        AdjXML.Visible = True
        lblPDF.Visible = True
        AdjPDF.Visible = True
        rdlFactura.Items(0).Selected = True
        BtnAdjuntarXML.Visible = True

    End Sub


    Private Sub llenaCboFact(ByVal documento As String)
        'GCM 11122014 llena el radio boton con la información correspondiente
        Dim colSelec = "#D3D3D3"
        Dim colMenu = "#f2f2f2"
        pnlRFac.CssClass = "fondo"
        pnlupfile.CssClass = "fondo"
        rdlFactura.Items.Clear()
        If documento = "QTFAPN" Then
            LnkFComp.BackColor = Drawing.ColorTranslator.FromHtml(colSelec)
            LnkFEmb.BackColor = Drawing.ColorTranslator.FromHtml(colMenu)
            LnkFGto.BackColor = Drawing.ColorTranslator.FromHtml(colMenu)
        End If
        If documento = "VTFE" Then
            LnkFComp.BackColor = Drawing.ColorTranslator.FromHtml(colMenu)
            LnkFEmb.BackColor = Drawing.ColorTranslator.FromHtml(colSelec)
            LnkFGto.BackColor = Drawing.ColorTranslator.FromHtml(colMenu)
        End If
        lblContrato.Visible = False
        ddlContrato.Visible = False

        lblrdlFac.Visible = True
        lblXML.Visible = True
        lblXML.Visible = True
        AdjXML.Visible = True
        lblPDF.Visible = True
        AdjPDF.Visible = True
        _user = CType(Page.Session("user"), PerfilesUser)
        Dim Fcomp = _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = documento)
        rdlFactura.Items.AddRange((From a In _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = documento) Select New ListItem(a.NomDoc, a.TipdocCve)).ToArray())
        rdlFactura.Items(0).Selected = True
        upUfiles.Visible = True
        BtnAdjuntarXML.Visible = True
    End Sub

    Protected Sub xmldsMetodo_Transforming(sender As Object, e As System.EventArgs) Handles xmldsMetodo.Transforming

    End Sub
End Class

