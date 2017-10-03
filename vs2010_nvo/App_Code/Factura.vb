'Option Explicit On
Imports System.Xml.Schema
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net.Mail
Imports System.Xml.Linq
Imports System
Imports System.Configuration
Imports System.Math
'Imports Microsoft.VisualBasic.Activities
Imports CLPortalAbstract
Imports Microsoft.VisualBasic.CompilerServices
Imports System.Text.RegularExpressions
Imports _Server




Namespace Skytex.FacturaElectronica

    Public Class Factura
        'Private ReadOnly _rutaTmpExis As String = ConfigurationManager.AppSettings("RutaExis")
        Private _rutaTmpExis As String = ""
#Region "Variables"

        Private _Conexion As New Datos
        Private Conexion As SqlConnection = _Conexion.MiConexion
        Private ReadOnly _timeout As Integer = _Conexion.time_out
        Private _transaccion As SqlTransaction
        Private ds As New DataSet
        Private dsNE As New DataSet
        Private dsCPEmisor As New DataSet
        Private dsCPReceptor As New DataSet
        Private dsNEEmisor As New DataSet
        Private dsNEReceptor As New DataSet

        'CONCEPTOS
        Private _recepcion As String = "Sin Datos"

        Private _dtValidacion As DataTable
        Private _dtValidacionDireccion As DataTable
        Private _dtValidacionCPReceptor As DataTable
        Private _dtValidacionCPEmisor As DataTable
        Private _dtValidacionNEReceptor As DataTable
        Private _dtValidacionNEEmisor As DataTable
        Dim nomEstadoEmisor As String
        Dim nomEstadoReceptor As String
        Dim cpReceptor As String
        Dim cpEmisor As String
        Private _efCveG As String
        Private _iErrorG As Integer = 0
        Private _mensaje As String

        Private _folioCr As Integer 'Folio Contrarecibo
        Private _doctoCr As String 'tipo_doc del contra recibo

        Private _msjError As String = ""
        Private _llamaSqlImpresion As Integer = 0
        Dim _server As New _Server
        Private DBdevelop As String = _Server.DBdevelop
        Private DBproduc As String = _Server.DBproduc
#End Region

#Region "Propiedades"

        Public Property LlamaSqlImpresion() As Integer
            Get
                Return _llamaSqlImpresion
            End Get
            Set(ByVal value As Integer)
                _llamaSqlImpresion = value
            End Set
        End Property
        Public Property Recepcion() As String
            Get
                Return _recepcion
            End Get
            Set(ByVal value As String)
                _recepcion = value
            End Set
        End Property
        Public Property ValidacionEncabezado() As DataTable
            Get
                Return _dtValidacion
            End Get
            Set(ByVal value As DataTable)
                _dtValidacion = value
            End Set
        End Property
        Public Property ValidacionCPReceptor() As DataTable
            Get
                Return _dtValidacionCPReceptor
            End Get
            Set(ByVal value As DataTable)
                _dtValidacionCPReceptor = value
            End Set
        End Property
        Public Property ValidacionCPEmisor() As DataTable
            Get
                Return _dtValidacionCPEmisor
            End Get
            Set(ByVal value As DataTable)
                _dtValidacionCPEmisor = value
            End Set
        End Property
        Public Property ValidacionNEReceptor() As DataTable
            Get
                Return _dtValidacionNEReceptor
            End Get
            Set(ByVal value As DataTable)
                _dtValidacionNEReceptor = value
            End Set
        End Property
        Public Property ValidacionNEEmisor() As DataTable
            Get
                Return _dtValidacionNEEmisor
            End Get
            Set(ByVal value As DataTable)
                _dtValidacionNEEmisor = value
            End Set
        End Property
        Public Property ValidacionDireccionFiscal() As DataTable
            Get
                Return _dtValidacionDireccion
            End Get
            Set(ByVal value As DataTable)
                _dtValidacionDireccion = value
            End Set
        End Property
        Public Property EfCveG() As String
            Get
                Return _efCveG
            End Get
            Set(ByVal value As String)
                _efCveG = value
            End Set
        End Property

        Public Property iErrorG() As Integer
            Get
                Return _iErrorG
            End Get
            Set(ByVal value As Integer)
                _iErrorG = value
            End Set
        End Property

        Public Property Mensaje() As String
            Get
                Return _mensaje
            End Get
            Set(ByVal value As String)
                _mensaje = value
            End Set
        End Property

        Public Property MensajeError() As String
            Get
                Return _msjError
            End Get
            Set(ByVal value As String)
                _msjError = value
            End Set
        End Property


        Public Property FolioCr() As Integer
            Get
                Return _folioCr
            End Get
            Set(ByVal value As Integer)
                _folioCr = value
            End Set
        End Property

        Public Property DoctoCr() As String
            Get
                Return _doctoCr
            End Get
            Set(ByVal value As String)
                _doctoCr = value
            End Set
        End Property


#End Region

#Region "Métodos"

        Public Function GetConfigure(ByVal listaConfig As List(Of ConfigGral), ByVal sConf As String) As Object
            Dim value As Object = Nothing
            Dim rutaEnum = From con In listaConfig
                                Where con.config = sConf
                                Select con.valor
            For Each itm As String In rutaEnum
                value = itm
            Next
            Return value
        End Function

        Public Sub GenMailErrHtml(ByVal llaveCfd As llave_cfd, ByVal titulo As String,
                                  ByVal errores As List(Of Errores),
                                  ByVal eMail As String, ByVal proveedor As String)

            Dim nomProveedor As String = proveedor.Substring(10, proveedor.Length - 10)

            Dim sBody As String
            Dim sAttach As New List(Of String)
            Const sEnc As String = "<!DOCTYPE HTML><meta http-equiv='Content-Type' content='text/html; charset=ISO-8859-1'> <style type='text/css'> body { font-family:Calibri, sans-serif;font-size:14px;} </style> <head></head><body> <table  border='0' cellpadding='0' cellspacing='0' ><tr><td style:'width:800px;' >"
            sBody = sEnc + "Datos del comprobante:" + "</br>"
            sBody = sBody + "</br>"
            sBody = sBody + " <table border='0' cellpadding='0' cellspacing='0' >"
            'sBody = sBody + "<tr>"
            'sBody = sBody + "<th style='width:30px;text-align:left;'></th>"
            'sBody = sBody + "<th style='width:90px;text-align:left;'></th>"
            'sBody = sBody + "<th style='width:700px;text-align:left;'></th>"
            'sBody = sBody + "</tr>"
            sBody = sBody + "<tr>"
            sBody = sBody + "<td style='height:5px;width:30px;text-align:left;'></td>"
            sBody = sBody + "<td style='height:5px;width:90px;text-align:left;'>Proveedor:</td>"
            sBody = sBody + "<td style='height:5px;width:700px;text-align:left;'>" + nomProveedor.Trim() + "</td>"
            sBody = sBody + "</tr>"
            sBody = sBody + "<tr>"
            sBody = sBody + "<td style='height:5px'></td>"
            sBody = sBody + "<td style='height:5px'>Folio:</td>"
            sBody = sBody + "<td style='height:5px'>" + llaveCfd.folio_factura.ToString() + "</td>"
            sBody = sBody + "</tr>"
            sBody = sBody + "<tr>"
            sBody = sBody + "<td style='height:5px'></td>"
            sBody = sBody + "<td style='height:5px'>Serie:</td>"
            sBody = sBody + "<td style='height:5px'>" + llaveCfd.serie + "</td>"
            sBody = sBody + "</tr>"
            sBody = sBody + "<tr>"
            sBody = sBody + "<td style='height:5px'></td>"
            sBody = sBody + "<td style='height:5px'>UUID:</td>"
            sBody = sBody + "<td style='height:5px'>" + llaveCfd.timbre_fiscal.uuid + "</td>"
            sBody = sBody + "</tr>"
            sBody = sBody + "<tr>"
            'sBody = sBody + "<td></br></td>"
            sBody = sBody + "<td></td>"
            sBody = sBody + "<td></td>"
            sBody = sBody + "<td></td>"
            sBody = sBody + "</tr>"
            For Each o As Errores In errores
                Dim tipoError As String
                Dim claveMsg As String
                Dim descripcionError As String
                tipoError = o.Interror.ToString()
                claveMsg = o.CveError
                descripcionError = o.Message.Trim()
                If claveMsg <> "" Then
                    sBody = sBody + "<tr>"
                    sBody = sBody + "<td style='height:5px'></br></td>"
                    sBody = sBody + "<td style='height:5px'>Tipo de error:</td>"
                    sBody = sBody + "<td style='height:5px'>" + tipoError + "</td>"
                    sBody = sBody + "</tr>"
                    sBody = sBody + "<tr>"
                    sBody = sBody + "<td style='height:5px'></td>"
                    sBody = sBody + "<td style='height:5px'>Clave del error:</td>"
                    sBody = sBody + "<td style='height:5px'>" + claveMsg + "</td>"
                    sBody = sBody + "</tr>"
                    sBody = sBody + "<tr>"
                    sBody = sBody + "<td style='height:5px'></td>"
                    sBody = sBody + "<td style='height:5px'>Mensaje:</td>"
                    sBody = sBody + "<td style='height:5px'>" + descripcionError + "</td>"
                    sBody = sBody + "</tr>"
                    'sBody = sBody + "</tr></br>"
                Else
                    sBody = sBody + "<tr>"
                    sBody = sBody + "<td style='height:5px'></td>"
                    If tipoError = 1 Then
                        sBody = sBody + "<td style='height:5px'>Mensaje:</td>"
                    Else
                        sBody = sBody + "<td style='height:5px'>Rutina:</td>"
                    End If

                    sBody = sBody + "<td style='height:5px'>" + descripcionError + "</td>"
                    sBody = sBody + "</tr>"
                    'sBody = sBody + "</tr></br>"
                End If
            Next
            sBody = sBody + "</table> "
            sBody = sBody + "</td></tr></table>"
            sBody = sBody + "</br>"
            sBody = sBody + "</body></HTML> "
            MailGeneric(titulo, sBody, eMail, sAttach)
        End Sub

        Public Sub MailGeneric(ByVal sSubject As String, _
                               ByVal sBody As String, _
                               ByVal sEmails As String,
                               ByVal lAttach As List(Of String))

            Dim objMail As New MailMessage
            Dim smtp As New SmtpClient
            Dim dat As New Datos
            Dim emailFrom As String = ConfigurationManager.AppSettings("Email_From")
            Dim emailSmtp As String = ConfigurationManager.AppSettings("Email_SMTP")
            Dim emailUser As String = ConfigurationManager.AppSettings("Email_User")
            Dim emailPsswd As String = ConfigurationManager.AppSettings("Email_Psswd")
            Dim oAttch As Attachment
            Dim base As String = ""
            base = dat.ObtieneDb()
            Try
                objMail.To.Add(sEmails)
                'objMail.To.Add("fernando.garcia@skytex.com.mx")

                For Each item As String In lAttach
                    oAttch = New Attachment(item.Trim())
                    objMail.Attachments.Add(oAttch)
                Next
                objMail.From = New MailAddress(emailFrom)

                'If base = "develop" Then
                If base = DBdevelop Then '//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
                    objMail.Subject = sSubject + " Desarrollo DEVELOP"
                Else
                    objMail.Subject = sSubject
                End If
                objMail.Body = sBody.Trim()
                objMail.IsBodyHtml = True
                objMail.BodyEncoding = Encoding.UTF8
                objMail.Priority = MailPriority.Normal
                'smtp.Host = emailSmtp
                'smtp.Credentials = New Net.NetworkCredential(emailUser, emailPsswd)
                ''smtp.Send(objMail)
            Catch ex As SmtpException
                MensajeError = "Error al enviar mail en la aplicación. " + sBody + " ::: " + ex.Message
                iErrorG = 1
            End Try

        End Sub

        Public Sub MailGenericBis(ByVal sSubject As String, _
                              ByVal sBody As String, _
                              ByVal sEmails As String
                              )

            Dim objMail As New MailMessage
            Dim smtp As New SmtpClient
            Dim dat As New Datos
            Dim emailFrom As String = ConfigurationManager.AppSettings("Email_From")
            Dim emailSmtp As String = ConfigurationManager.AppSettings("Email_SMTP")
            Dim emailUser As String = ConfigurationManager.AppSettings("Email_User")
            Dim emailPsswd As String = ConfigurationManager.AppSettings("Email_Psswd")
            Dim oAttch As Attachment
            Dim base As String = ""
            base = dat.ObtieneDb()
            Try
                objMail.To.Add(sEmails)

                objMail.From = New MailAddress(emailFrom)
                If base = DBdevelop Then '//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
                    objMail.Subject = sSubject + " Desarrollo DEVELOP"
                Else
                    objMail.Subject = sSubject
                End If
                objMail.Body = sBody.Trim()
                objMail.IsBodyHtml = True
                objMail.BodyEncoding = Encoding.UTF8
                objMail.Priority = MailPriority.Normal
                smtp.Host = emailSmtp
                smtp.Credentials = New Net.NetworkCredential(emailUser, emailPsswd)
                smtp.Send(objMail)
            Catch ex As SmtpException
                MensajeError = "Error al enviar mail en la aplicación. " + sBody + " ::: " + ex.Message
                iErrorG = 1
            End Try

        End Sub

        Sub ObtieneDatos(ByVal rfc As String, ByVal serie As String, ByVal folio As String, ByVal uuid As String, _
                    ByVal contra As List(Of nuevas_facturas), ByVal numInfo As Integer, ByVal errores As List(Of Errores))
            Dim recibo As New nuevas_facturas
            Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", Conexion)
            Dim ds As New DataSet()
            myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            Conexion.Open()
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfc)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", Int64.Parse(folio))
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", numInfo)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid)

            Try
                myDataAdapter.Fill(ds, "sp_consInfXML")
                myDataAdapter.Dispose()
                Conexion.Close()
                Dim tablaInfo As DataTable
                tablaInfo = ds.Tables.Item(ds.Tables.Count - 1)
                Dim iError As Integer = Int32.Parse(tablaInfo.Rows(0).Item("error").ToString.Trim)
                Dim msg = tablaInfo.Rows(0).Item("msg").ToString.Trim
                If iError = 0 Then
                    recibo.num_fol = Int32.Parse(CType(tablaInfo.Rows(0).Item("num_fol_contra"), String))
                    recibo.ef_cve = tablaInfo.Rows(0).Item("ef_cve").ToString.Trim
                    recibo.tipo_doc = tablaInfo.Rows(0).Item("tipo_doc_contra").ToString.Trim
                    recibo.nom_arch = tablaInfo.Rows(0).Item("nom_arch").ToString.Trim
                    contra.Add(recibo)
                End If
                If iError <> 0 Then
                    agrega_err(iError, msg, errores)
                End If
            Catch ex As Exception
                Dim msg As String
                msg = "sp_consInfXML:"
                iErrorG = 3
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub

        Public Sub GrabaTmp(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)
            iErrorG = 0
            If errores.Count = 0 And iErrorG = 0 Then
                If comprobante.tipodoc_cve = "BTFACS" Or comprobante.tipodoc_cve = "BTFSER" Then
                    ValidaDatosDetalleTmpSrv(errores, llaveCfd, comprobante.Addenda.requestforpayment.document)
                Else
                    ValidaDatosDetalleTmp(errores, comprobante.Addenda.requestforpayment.line_items, llaveCfd)
                End If

            End If
            If errores.Count = 0 And iErrorG = 0 Then
                If llaveCfd.version = "3.3" Then
                    ValidaDatosEncabezadoTmp3_3(errores, comprobante, llaveCfd)
                Else
                    ValidaDatosEncabezadoTmp(errores, comprobante, llaveCfd)
                End If
                'ValidaDatosEncabezadoTmp(errores, comprobante, llaveCfd)
            End If
            If iErrorG = 60089 Then
                agrega_err(1, "Error, la captura no fue procesada, el folio del comprobante ya había sido aceptado", errores, "60089")
            End If
            If iErrorG = 60090 Then
                agrega_err(1, "Error en la aplicacion, consulte con su administrador del sitio ", errores, "60090")
            End If
            If errores.Count > 0 Or iErrorG > 0 Then
                LeerErroresSql(errores, llaveCfd)
            End If
        End Sub

        Public Sub GrabaTmp3_3(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)
            iErrorG = 0
            If errores.Count = 0 And iErrorG = 0 Then
                If comprobante.tipodoc_cve = "BTFACS" Or comprobante.tipodoc_cve = "BTFSER" Then
                    ValidaDatosDetalleTmpSrv(errores, llaveCfd, comprobante.Addenda.requestforpayment.document)
                Else
                    ValidaDatosDetalleTmp(errores, comprobante.Addenda.requestforpayment.line_items, llaveCfd)
                End If

            End If
            If errores.Count = 0 And iErrorG = 0 Then
                ValidaDatosEncabezadoTmp3_3(errores, comprobante, llaveCfd)
            End If
            If iErrorG = 60089 Then
                agrega_err(1, "Error, la captura no fue procesada, el folio del comprobante ya había sido aceptado", errores, "60089")
            End If
            If iErrorG = 60090 Then
                agrega_err(1, "Error en la aplicacion, consulte con su administrador del sitio ", errores, "60090")
            End If
            If errores.Count > 0 Or iErrorG > 0 Then
                LeerErroresSql(errores, llaveCfd)
            End If
        End Sub

        Public Sub ValidaDatosEncabezadoTmp(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)
            Dim er As New Errores
            Dim sqlAdapter = New SqlDataAdapter("sp_datos_xml_temp", Conexion)
            Dim importeIva As Decimal = 0
            'Dim sqlAdapter = New SqlDataAdapter("sp_datos_xml_temp_zfj", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            If Not (llaveCfd.ef_cve Is Nothing) And EfCveG Is Nothing Then
                EfCveG = llaveCfd.ef_cve
            End If
            If EfCveG Is Nothing Then
                EfCveG = "zzz"
            End If
            Dim iva = From cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.impuesto, cons.importe
            Where (impuesto = "IVA")
            Dim varIva As Decimal '= comprobante.Impuestos.Traslados.tasa / 100
            Dim varIvaImporte As Decimal
            Dim noiva As Integer
            'GCM 20012015 Se contempla en btfser en subtotal y total para calcular el pct_iva
            Dim retisr = From cons In comprobante.Impuestos.Retenciones
                  Select cons.importe, cons.impuesto
                  Where (impuesto = "ISR")
            Dim retenImporteIsr As Decimal = 0
            For Each i In retisr
                retenImporteIsr = retenImporteIsr + i.importe 'FormatNumber(i.importe, decimales)
            Next

            Dim subtotal = comprobante.sub_total - comprobante.descuento + comprobante.TotaldeTraslados - comprobante.TotaldeRetenciones - retenImporteIsr

            'GCM 17122014 Agrego para contemplar caso totalimpuestos
            If comprobante.tipodoc_cve = "BTFSER" Then
                noiva = Aggregate cons In comprobante.Impuestos.Traslados
                           Select cons.tasa, cons.importe, cons.impuesto
                           Where (impuesto = "IVA")
                              Into Count()
            Else
                noiva = 1
            End If



            '  For Each i In iva
            'Dim swtasa = RevisaIntTasa(i.tasa.ToString())
            'If swtasa = True Then
            'varIva = i.tasa
            'Else
            'varIva = i.tasa * 100
            'End If
            'importeIva = importeIva + i.importe
            'Next

            For Each i In iva
                Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                If swtasa = True Then
                    If varIva = 0 Then
                        varIva = i.tasa
                    End If
                Else
                    If varIva = 0 Then
                        varIva = i.tasa * 100
                    End If
                End If
                importeIva = importeIva + i.importe
            Next



            'GCM 17122014 Agrego para contemplar caso totalimpuestos
            If comprobante.tipodoc_cve = "BTFSER" Then
                varIvaImporte = comprobante.totalImpuestosTrasladados
                If noiva = 0 Then
                    If subtotal = comprobante.total Then
                        varIva = 0
                    Else
                        varIva = comprobante.pctIva / 100
                    End If
                End If
            Else
                varIvaImporte = importeIva
            End If





            sqlAdapter.SelectCommand.Parameters.AddWithValue("@nom_arch", comprobante.nom_arch)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@version", comprobante.version)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", comprobante.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", comprobante.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@fecha_factura", comprobante.fecha_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sello", comprobante.sello)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_aprobacion", comprobante.no_aprobacion)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ano_aprobacion", comprobante.ano_aprobacion)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado", comprobante.no_certificado)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@certificado", comprobante.certificado)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@condiciones_pago", comprobante.Addenda.requestforpayment.paymenttimeperiod.timeperiod.ToString())
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@metodo_pago", comprobante.Addenda.requestforpayment.aditional_data.metododepago) 'comprobante.metodo_pago)
            If comprobante.tipodoc_cve = "BTFSER" Then
                sqlAdapter.SelectCommand.Parameters.AddWithValue("@sub_total", comprobante.sub_total - comprobante.descuento + comprobante.TotaldeTraslados - comprobante.TotaldeRetenciones)
            Else
                sqlAdapter.SelectCommand.Parameters.AddWithValue("@sub_total", comprobante.sub_total)
            End If
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@descuento", comprobante.descuento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@total", comprobante.total)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", comprobante.Emisor.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_receptor", comprobante.Receptor.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tm", comprobante.Addenda.requestforpayment.aditional_data.moneda) 'comprobante.moneda)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@d_adicionales", comprobante.Addenda.requestforpayment.aditional_data.text_data)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@pct_iva", varIva) 'Decimal.Parse(comprobante.Impuestos.Traslados.tasa))
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", comprobante.cc_cve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_tipo", comprobante.cc_tipo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_Cve", llaveCfd.ef_cve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado_sat", llaveCfd.timbre_fiscal.no_certificado_sat)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@id", "WEB")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_cf", llaveCfd.version_nom)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sw_reten_iva", comprobante.Impuestos.sw_retencion)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@iva", varIvaImporte)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@id_soludin", llaveCfd.IdSoludin)

            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_datos_xml_temp")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)

                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                If iErrorG > 0 Then
                    agrega_err(1, "", errores)
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
            Catch ex As Exception
                Dim msg As String
                msg = "sp_datos_xml_temp"
                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_datos_xml_temp")
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub

        Public Sub ValidaDatosEncabezadoTmp3_3(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)
            Dim er As New Errores
            Dim sqlAdapter = New SqlDataAdapter("sp_datos_xml_temp", Conexion)
            Dim importeIva As Decimal = 0
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            If Not (llaveCfd.ef_cve Is Nothing) And EfCveG Is Nothing Then
                EfCveG = llaveCfd.ef_cve
            End If
            If EfCveG Is Nothing Then
                EfCveG = "zzz"
            End If
            Dim iva = From cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.impuesto, cons.importe
            Where (impuesto = "002")
            Dim varIva As Decimal
            Dim varIvaImporte As Decimal
            Dim noiva As Integer
            Dim retisr = From cons In comprobante.Impuestos.Retenciones
                  Select cons.importe, cons.impuesto
                  Where (impuesto = "001")
            Dim retenImporteIsr As Decimal = 0
            For Each i In retisr
                retenImporteIsr = retenImporteIsr + i.importe
            Next

            Dim subtotal = comprobante.sub_total - comprobante.descuento + comprobante.TotaldeTraslados - comprobante.TotaldeRetenciones - retenImporteIsr
            If comprobante.tipodoc_cve = "BTFSER" Then
                noiva = Aggregate cons In comprobante.Impuestos.Traslados
                           Select cons.tasa, cons.importe, cons.impuesto
                           Where (impuesto = "002")
                              Into Count()
            Else
                noiva = 1
            End If

            For Each i In iva
                Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                If swtasa = True Then
                    If varIva = 0 Then
                        varIva = i.tasa
                    End If
                Else
                    If varIva = 0 Then
                        varIva = i.tasa * 100
                    End If
                End If
                importeIva = importeIva + i.importe
            Next

            If comprobante.tipodoc_cve = "BTFSER" Then
                varIvaImporte = comprobante.totalImpuestosTrasladados
                If noiva = 0 Then
                    If subtotal = comprobante.total Then
                        varIva = 0
                    Else
                        varIva = comprobante.pctIva / 100
                    End If
                End If
            Else
                varIvaImporte = importeIva
            End If

            sqlAdapter.SelectCommand.Parameters.AddWithValue("@nom_arch", comprobante.nom_arch)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@version", comprobante.version)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", comprobante.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", comprobante.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@fecha_factura", comprobante.fecha_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sello", comprobante.sello)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_aprobacion", comprobante.no_aprobacion)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ano_aprobacion", comprobante.ano_aprobacion)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado", comprobante.no_certificado)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@certificado", comprobante.certificado)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@condiciones_pago", comprobante.Addenda.requestforpayment.paymenttimeperiod.timeperiod.ToString())
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@metodo_pago", comprobante.Addenda.requestforpayment.aditional_data.metododepago) 'comprobante.metodo_pago)
            If comprobante.tipodoc_cve = "BTFSER" Then
                sqlAdapter.SelectCommand.Parameters.AddWithValue("@sub_total", comprobante.sub_total - comprobante.descuento + comprobante.TotaldeTraslados - comprobante.TotaldeRetenciones)
            Else
                sqlAdapter.SelectCommand.Parameters.AddWithValue("@sub_total", comprobante.sub_total)
            End If
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@descuento", comprobante.descuento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@total", comprobante.total)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", comprobante.Emisor.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_receptor", comprobante.Receptor.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tm", comprobante.Addenda.requestforpayment.aditional_data.moneda) 'comprobante.moneda)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@d_adicionales", comprobante.Addenda.requestforpayment.aditional_data.text_data)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@pct_iva", varIva) 'Decimal.Parse(comprobante.Impuestos.Traslados.tasa))
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", comprobante.cc_cve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_tipo", comprobante.cc_tipo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_Cve", llaveCfd.ef_cve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado_sat", llaveCfd.timbre_fiscal.no_certificado_sat)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@id", "WEB")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_cf", llaveCfd.version_nom)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sw_reten_iva", comprobante.Impuestos.sw_retencion)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@iva", varIvaImporte)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@id_soludin", llaveCfd.IdSoludin)

            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_datos_xml_temp")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                If iErrorG > 0 Then
                    agrega_err(1, "", errores)
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
            Catch ex As Exception
                Dim msg As String
                msg = "sp_datos_xml_temp"
                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_datos_xml_temp")
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub

        Public Sub ValidaDatosDetalleTmp(ByVal errores As List(Of Errores), ByVal items As List(Of lineitem), ByVal llaveCfd As llave_cfd)
            Dim item = New lineitem
            For Each item In items
                ValidaDatosItemTmp(errores, item, llaveCfd)
                If errores.Count > 0 Or iErrorG > 0 Then
                    Exit For
                End If
            Next
        End Sub

        Public Sub ValidaDatosDetalleTmpSrv(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd, ByVal documento As Document)
            Dim item = New lineitem
            item.reference_identification = documento.referenceIdentification
            item.type = 1
            item.number = 0
            item.monto_decuento = Decimal.Parse("0.0000")
            item.pct_decuento = Decimal.Parse("0.0000")
            item.uns = 0
            item.precio = Decimal.Parse("0.0000")
            item.sku = "zzzzzz"
            item.partida = 0
            item.art_tip = "zzz"
            item.uni_med = "zzzzzz"
            ValidaDatosItemTmp(errores, item, llaveCfd)
        End Sub

        Public Sub ValidaDatosItemTmp(ByVal errores As List(Of Errores), ByVal item As lineitem, ByVal llaveCfd As llave_cfd)

            Dim er As New Errores
            Dim sqlAdapter = New SqlDataAdapter("sp_detalle_xml_temp", Conexion)
            'Dim sqlAdapter = New SqlDataAdapter("sp_detalle_xml_temp_zfj", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sku_cve", item.sku)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@art_tip", item.art_tip)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@num_reng", item.number)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uns", item.uns)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uni_med", item.uni_med)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@pct_descto", item.pct_decuento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@mto_descto", item.monto_decuento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@precio", item.precio)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", llaveCfd.ef_cve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@recepcion", item.reference_identification.Replace(" ", ""))
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@num_rengp", item.partida)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_reng", item.type)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_detalle_xml_temp")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                If iErrorG > 0 Then
                    agrega_err(1, "", errores)
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
            Catch ex As Exception

                Dim msg As String
                msg = "sp_detalle_xml_temp"
                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_detalle_xml_temp")
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try

        End Sub

        Public Sub PLeeCfd(ByVal nombreArchivoXml As String, ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd, ByVal ccCve As String, ByVal ccTipo As String, ByVal decTrun As Integer, ByVal decimales As Integer)

            Const swErrins As Integer = 0
            Dim receptor = New receptor
            iErrorG = 0
            Try
                If errores.Count = 0 And iErrorG = 0 Then
                    'se ocupa para xml 3.2 y 3.3
                    LeeDatosLlaveLinq(errores, llaveCfd, nombreArchivoXml, receptor)
                End If
            Catch ex As Exception
                agrega_err(3, ex.Message, errores)
            End Try
            Try
                If errores.Count = 0 And llaveCfd.sw_sin_addenda = 0 And iErrorG = 0 Then
                    'se ocupa para xml 3.2 y 3.3
                    LeeDatosFacturaLinq(errores, comprobante, nombreArchivoXml, llaveCfd, ccCve, ccTipo)
                End If
            Catch ex As Exception
                agrega_err(3, ex.Message, errores)
            End Try
            Try
                If errores.Count = 0 And llaveCfd.sw_sin_addenda = 1 And iErrorG = 0 Then
                    'se ocupa para xml 3.2 y 3.3
                    'LeeDatosFacturaLINQ_SNAdd(errores, comprobante, nombreArchivoXml, llaveCfd)
                    If llaveCfd.version = "3.3" Then
                        LeeDatosFacturaLINQ_SNAdd3_3(errores, comprobante, nombreArchivoXml, llaveCfd)
                    Else
                        LeeDatosFacturaLINQ_SNAdd(errores, comprobante, nombreArchivoXml, llaveCfd)
                    End If
                End If
            Catch ex As Exception
                agrega_err(3, ex.Message, errores)
            End Try
            Try
                If llaveCfd.sw_sin_addenda = 1 And iErrorG = 0 And errores.Count = 0 Then

                    Dim dicEmp As Dictionary(Of String, String) = lee_ef_cve(receptor.rfc)
                    Dim pairK As KeyValuePair(Of String, String)
                    For Each pairK In dicEmp
                        llaveCfd.ef_cve = pairK.Key
                        comprobante.Receptor.nombre = pairK.Value
                    Next

                End If
            Catch ex As Exception
                agrega_err(3, ex.Message, errores)
            End Try
            Try
                If errores.Count = 0 And llaveCfd.sw_sin_addenda = 0 And iErrorG = 0 Then
                    If llaveCfd.version = "3.3" Then
                        'se ocupa para xml 3.3
                        ValidaTotales3_3(errores, comprobante, llaveCfd, decTrun, decimales)
                    Else
                        'se ocupa para xml 3.2
                        ValidaTotales(errores, comprobante, llaveCfd, decTrun, decimales)
                    End If
                    
                End If
            Catch ex As Exception
                agrega_err(3, ex.Message, errores)
            End Try
            Try
                If errores.Count = 0 And llaveCfd.sw_sin_addenda = 1 And iErrorG = 0 Then
                    If llaveCfd.version = "3.3" Then
                        'se ocupa para xml 3.3
                        ValidaTotales_SNAdd3_3(errores, comprobante, llaveCfd, decTrun, decimales)
                    Else
                        'se ocupa para xml 3.2
                        ValidaTotales_SNAdd(errores, comprobante, llaveCfd, decTrun, decimales)
                    End If
                    
                End If
            Catch ex As Exception
                agrega_err(3, ex.Message, errores)
            End Try
            Dim msg60000 = From msg In errores _
                Select interror = msg.Interror, message = msg.Message _
                Where (interror = 60000)
            If msg60000.Count <> 0 Then
                If swErrins = 1 And llaveCfd.folio_factura <> 0 And llaveCfd.rfc_emisor <> "" And llaveCfd.serie <> "" Then
                    LeerErroresSql(errores, llaveCfd)
                End If
            End If

        End Sub

        Public Sub GenFactura(ByVal nombreArchivoXml As String, ByVal nombreArchivoPdf As String, ByVal errores As List(Of Errores), ByVal items As List(Of lineitem), ByVal comprobante As Comprobante, ByVal LlaveCfd As llave_cfd, ByVal contrarecibo As nuevas_facturas, ByVal config As List(Of ConfigGral))

            iErrorG = 0
            If errores.Count = 0 And LlaveCfd.sw_sin_addenda = 0 And iErrorG = 0 Then
                'se ocupa para xml 3.2 y 3.3
                ValidaDatosEncabezado(errores, comprobante, LlaveCfd)
            End If
            If errores.Count = 0 And LlaveCfd.sw_sin_addenda = 1 And iErrorG = 0 Then
                'se ocupa para xml 3.2 y 3.3
                ValidaDatosEncabezadoCap(errores, comprobante, LlaveCfd)
            End If
            If errores.Count = 0 And LlaveCfd.sw_sin_addenda = 0 And iErrorG = 0 Then
                'se ocupa para xml 3.2 y 3.3
                ValidaDatosDetalle(errores, items, LlaveCfd)
            End If
            If errores.Count = 0 And LlaveCfd.sw_sin_addenda = 1 And iErrorG = 0 Then
                'se ocupa para xml 3.2 y 3.3
                ValidaDatosDetalleCap(errores, items, LlaveCfd)
            End If
            If errores.Count = 0 And iErrorG = 0 Then
                'se ocupa para xml 3.2 y 3.3
                ValidaDatosPapa(errores, LlaveCfd)
            End If
            Dim factura As New List(Of nuevas_facturas)
            If errores.Count = 0 And iErrorG = 0 Then
                'se ocupa para xml 3.2 y 3.3
                GeneraEncabezadoFactura(errores, LlaveCfd, factura)
            End If
            If errores.Count = 0 And iErrorG = 0 Then
                'se ocupa para xml 3.2 y 3.3
                GeneraContraReciboFactura(errores, LlaveCfd)
            End If
            If errores.Count = 0 And iErrorG = 0 Then
                Dim ruta As String = CType(GetConfigure(config, "Ruta"), String)
                Dim rTmpExis As String = CType(GetConfigure(config, "RutaExis"), String)
                If errores.Count = 0 And iErrorG = 0 Then
                    For Each fact In factura
                        Try
                            If FileIO.FileSystem.FileExists(ruta + fact.ef_cve + fact.tipo_doc + fact.num_fol.ToString() + ".pdf") = True Then
                                FileSystem.FileCopy(nombreArchivoXml, rTmpExis + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".xml")
                                FileSystem.FileCopy(nombreArchivoPdf, rTmpExis + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".pdf")
                            Else
                                FileSystem.FileCopy(nombreArchivoXml, ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".xml")
                                FileSystem.FileCopy(nombreArchivoPdf, ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".pdf")
                            End If
                        Catch ex As Exception
                            agrega_err(3, ex.Message, errores)
                        End Try
                    Next
                End If
            End If

            If iErrorG = 60089 Then
                agrega_err(1, "Error este comprobante fiscal digital ya fue aceptado", errores)
            End If
            If iErrorG = 60090 Then
                agrega_err(1, "Error en la aplicacion, consulte con su administrador del sitio ", errores)
            End If

            If errores.Count = 0 And iErrorG = 0 Then
                Mensaje = "La factura se ha procesado exitosamente, consulte la sección Seguimiento para descargar el contrarecibo: " + FolioCr.ToString.Trim + vbNewLine
                contrarecibo.num_fol = FolioCr
                contrarecibo.tipo_doc = DoctoCr
                contrarecibo.ef_cve = EfCveG
                contrarecibo.nom_arch = Recepcion
            End If
            If iErrorG > 0 And errores.Count = 0 Then
                agrega_err(1, "", errores)
            End If
            If errores.Count > 0 And iErrorG > 0 Then
                LeerErroresSql(errores, LlaveCfd)
            End If
            If LlamaSqlImpresion = 1 Then
                llama_errores_tipo2(errores, LlaveCfd)
                LlamaSqlImpresion = 0
            End If

        End Sub

        Public Sub ValidaDatosEncabezadoCap(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)
            Dim sqlAdapter = New SqlDataAdapter("sp_Valida_XML", Conexion)
            'Dim sqlAdapter = New SqlDataAdapter("sp_Valida_XML_zfj", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            If Not (llaveCfd.ef_cve Is Nothing) And EfCveG Is Nothing Then
                EfCveG = llaveCfd.ef_cve
            End If
            If EfCveG Is Nothing Then
                EfCveG = "zzz"
            End If
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@nom_arch", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@version", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", comprobante.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", comprobante.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@fecha_factura", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sello", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_aprobacion", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ano_aprobacion", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@certificado", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@condiciones_pago", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@metodo_pago", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sub_total", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@descuento", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@total", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", comprobante.Emisor.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_receptor", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tm", comprobante.moneda)
            'comprobante.Addenda.requestforpayment.aditional_data.moneda)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@d_adicionales", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@pct_iva", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_tipo", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_Cve", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado_sat", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sw_reten_iva", comprobante.Impuestos.sw_retencion)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_Valida_XML")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
            Catch ex As Exception
                Dim er As Errores = Nothing
                Dim msg As String
                msg = "sp_Valida_XML"

                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_Valida_XML")
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub

        Public Sub ValidaDatosDetalleCap(ByVal errores As List(Of Errores), ByVal items As List(Of lineitem), ByVal llaveCfd As llave_cfd)
            Dim item = New lineitem
            If llaveCfd.sw_tmp = 1 Then
                'se ocupa para xml 3.2 y 3.3
                ValidaDatosItemCap(errores, item, llaveCfd)
            Else
                For Each item In items
                    'se ocupa para xml 3.2 y 3.3
                    ValidaDatosItemCap(errores, item, llaveCfd)
                    Exit For
                Next
            End If
        End Sub

        Public Sub ValidaDatosDetalleSrv(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd, ByVal documento As Document)
            Dim item = New lineitem

            item.reference_identification = documento.referenceIdentification
            item.type = 1
            item.number = 0
            item.monto_decuento = Decimal.Parse("0.0000")
            item.pct_decuento = Decimal.Parse("0.0000")
            item.uns = 0
            item.precio = Decimal.Parse("0.0000")
            item.sku = "zzzzzz"
            item.partida = 0
            item.art_tip = "zzz"
            item.uni_med = "zzzzzz"

            If llaveCfd.sw_tmp = 1 Then
                ValidaDatosItemCap(errores, item, llaveCfd)
            Else
                ValidaDatosItem(errores, item, llaveCfd)
            End If
        End Sub

        Public Sub ValidaDatosItemCap(ByVal errores As List(Of Errores), ByVal item As lineitem, ByVal llaveCfd As llave_cfd)

            Dim er As New Errores
            Dim sqlAdapter = New SqlDataAdapter("sp_Inserta_Detalle_XML", Conexion)
            'Dim sqlAdapter = New SqlDataAdapter("sp_Inserta_Detalle_XML_zfj", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_Inserta_Detalle_XML")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
                If iErrorG > 0 Then
                    agrega_err(1, "", errores)
                End If
            Catch ex As Exception
                Dim msg As String
                msg = "sp_Inserta_Detalle_XML"
                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_Inserta_Detalle_XML")
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
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

        Public Sub ValidaTotales(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd, ByVal decimalesTruncados As Integer, ByVal decimales As Integer)

            ' 02102014 se solicita que en la taza de iva no se calcule cuando la taza sea decimal ya que algunos tienen 16 y otros comprobantes tienen .16


            Dim mtoDescGlobal As Decimal = 0
            Dim minimoTotal As Decimal
            Dim maximoTotal As Decimal
            Dim iva = From cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.importe, cons.impuesto
                       Where (impuesto = "IVA")
            Dim tasaVarIva As Decimal = 0 '= comprobante.Impuestos.Traslados.tasa / 100
            Dim importeIva As Decimal = 0

            '24022015 GCM se contempla el rango max y min del query del servidor
            Dim cmax As New SqlCommand
            Dim Excepmax As Object
            cmax.CommandText = "select convert(decimal(19,4),prm15) from xcdconapl_cl where tipdoc_cve = 'xmlcfd' and sp_cve = 'webconfig' and spd_cve = 'rangoMinMax'"
            cmax.CommandType = CommandType.Text
            cmax.Connection = Conexion
            Conexion.Open()
            Excepmax = cmax.ExecuteScalar()
            Conexion.Close()

            For Each i In iva
                Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                If swtasa = True Then
                    tasaVarIva = i.tasa / 100 'FormatNumber(i.tasa / 100, decimales)
                Else
                    tasaVarIva = i.tasa
                End If
                importeIva = importeIva + i.importe  'FormatNumber(i.importe, decimales)
            Next
            Dim ieps = From cons In comprobante.Impuestos.Traslados
                      Select cons.tasa, cons.importe, cons.impuesto
                      Where (impuesto = "IEPS")
            Dim tasaIeps As Decimal = 0  '= comprobante.Impuestos.Traslados.tasa / 100
            Dim importeIeps As Decimal = 0
            For Each i In ieps
                Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                If swtasa = True Then
                    tasaIeps = i.tasa / 100 ' FormatNumber(i.tasa / 100, decimales)
                Else
                    tasaIeps = i.tasa
                End If
                importeIeps = importeIeps + i.importe ' FormatNumber(i.importe, decimales)
            Next
            Dim retenciones = From cons In comprobante.Impuestos.Retenciones
                    Select cons.importe, cons.impuesto
                    Where (impuesto = "IVA")
            Dim retenImporteIva As Decimal = 0
            For Each i In retenciones
                retenImporteIva = retenImporteIva + i.importe 'FormatNumber(i.importe, decimales)
            Next
            Dim retisr = From cons In comprobante.Impuestos.Retenciones
                  Select cons.importe, cons.impuesto
                  Where (impuesto = "ISR")
            Dim retenImporteIsr As Decimal = 0
            For Each i In retisr
                retenImporteIsr = retenImporteIsr + i.importe 'FormatNumber(i.importe, decimales)
            Next
            Dim errorCfd As Integer = 0
            Dim qryResultConceptos = From com In comprobante.Conceptos _
                       Select comprobante.Conceptos
            Dim qryResultItems = From com In comprobante.Addenda.requestforpayment.line_items _
                        Select comprobante.Addenda.requestforpayment.line_items
            Dim concep As List(Of concepto) = Nothing
            Dim con = New concepto
            Dim line As List(Of lineitem) = Nothing
            Dim sumaTotalCon As Decimal
            For Each concep In qryResultConceptos
            Next
            For Each line In qryResultItems
            Next
            Dim qryResLine = From com2 In concep _
                Let sub_total = com2.cantidad * com2.valor_unitario _
                Select sub_total, com2.importe, com2.no_identificacion, com2.cantidad, com2.valor_unitario '_
            Dim qry_tot_con = From com2 In concep _
               Let tot_con = com2.importe _
               Select com2.importe, com2.no_identificacion '_
            sumaTotalCon = Aggregate com2 In qryResLine _
                                    Into Sum(com2.sub_total)
            sumaTotalCon = FormatNumber(Round(sumaTotalCon, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim totalCon = Aggregate com2 In qry_tot_con _
                            Into Sum(com2.importe)
            Dim cantConcep = Aggregate com2 In qryResLine _
                              Into Sum(com2.cantidad)
            Dim valUnit = From com2 In qryResLine _
                            Select com2.valor_unitario _
                            Distinct
            totalCon = FormatNumber(Round(totalCon, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            sumaTotalCon = FormatNumber(Round(sumaTotalCon, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            minimoTotal = sumaTotalCon - Excepmax
            maximoTotal = sumaTotalCon + Excepmax
            If totalCon < minimoTotal Or totalCon > maximoTotal Then
                errorCfd = 1
            End If

            Dim descuentoCalculado As Decimal
            mtoDescGlobal = comprobante.descuento ' Monto global de descuento
            If comprobante.tipodoc_cve <> "BTFACS" Then
                Dim qryResLineItem = From com2 In line _
                 Let sub_total = com2.uns * com2.precio _
                 Select sub_total, com2.pct_decuento, com2.sku, com2.monto_decuento, com2.uns, com2.precio
                Dim sumaTotalLine = Aggregate com2 In qryResLineItem _
                                        Into Sum(com2.sub_total)
                sumaTotalLine = FormatNumber(Round(sumaTotalLine, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)

                Dim unsLine = Aggregate com2 In qryResLineItem _
                          Into Sum(com2.uns)
                Dim precioLine = From com2 In qryResLineItem _
                                Select com2.precio _
                                Distinct
                minimoTotal = CType((sumaTotalLine - Excepmax), Decimal)
                maximoTotal = CType((sumaTotalLine + Excepmax), Decimal)
                'validacion ocupada en los xml 3.3
                If sumaTotalCon < minimoTotal Or sumaTotalCon > maximoTotal Then
                    errorCfd = 1
                End If
                Dim desctoLineitem = From com3 In line _
                                          Let descto = (com3.uns * com3.precio) * (com3.pct_decuento / 100)
                descuentoCalculado = Aggregate com3 In desctoLineitem _
                                          Into Sum(com3.descto)
                Dim mtoDesc = Aggregate com3 In line _
                               Into Sum(com3.monto_decuento)

                If errorCfd = 0 Then
                    '------------------------------------------ Validar los descuentos

                    If descuentoCalculado <> 0 Or mtoDesc <> 0 Then
                        If errorCfd = 0 Then
                            minimoTotal = CType((mtoDesc - Excepmax), Decimal)
                            maximoTotal = CType((mtoDesc + Excepmax), Decimal)
                            If descuentoCalculado < minimoTotal Or descuentoCalculado > maximoTotal Then
                                errorCfd = 2
                            End If
                        End If
                    End If
                End If

            End If



            Dim subtotalComprobante = FormatNumber(Round(comprobante.sub_total, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim subtotalConceptos = From com4 In concep _
                                     Let sub_total = com4.cantidad * com4.valor_unitario _
                                     Select sub_total, com4.importe, com4.no_identificacion
            Dim subtC = Aggregate com4 In subtotalConceptos _
                          Into Sum(com4.sub_total)
            subtC = FormatNumber(Round(subtC, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)

            Dim subtL As Decimal
            If comprobante.tipodoc_cve <> "BTFACS" Then
                Dim subtotalItems = From com4 In line _
                                     Let sub_total = com4.uns * com4.precio _
                                     Select sub_total, com4.pct_decuento, com4.sku, com4.monto_decuento
                subtL = Aggregate com4 In subtotalItems _
                             Into Sum(com4.sub_total)
                subtL = FormatNumber(Round(subtL, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            End If


            If errorCfd = 0 Then
                '-------------------------------- Subtotales
                'If subtotalComprobante <> subtC Then
                '    errorCfd = 1
                'End If

                minimoTotal = CType((subtC - Excepmax), Decimal)
                maximoTotal = CType((subtC + Excepmax), Decimal)

                If subtotalComprobante < minimoTotal Or subtotalComprobante > maximoTotal Then
                    errorCfd = 1
                End If


                If comprobante.tipodoc_cve <> "BTFACS" Then
                    'If subtotalComprobante <> subtL Then
                    '    errorCfd = 1
                    'End If
                    minimoTotal = CType((subtL - Excepmax), Decimal)
                    maximoTotal = CType((subtL + Excepmax), Decimal)

                    If subtotalComprobante < minimoTotal Or subtotalComprobante > maximoTotal Then
                        errorCfd = 1
                    End If

                    'checar si esta validacion va a sustituir a la qvalidacion siguiente
                    'If subtC < minimoTotal Or subtC > maximoTotal Then
                    '    agrega_err(1, "No coincide Subtotal de Conceptos con LineItems: [" + subtC.ToString() + ":" + subtL.ToString() + "]", errores)
                    '    errorCfd = 1
                    'End If

                    If subtC <> subtL Then
                        agrega_err(1, "No coincide Subtotal de Conceptos con LineItems: [" + subtC.ToString() + ":" + subtL.ToString() + "]", errores)
                        errorCfd = 1
                    End If
                End If

            End If
            Dim totalComprobante = FormatNumber(Round(comprobante.total, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim totImpTraslComprobante = FormatNumber(Round(comprobante.Impuestos.total_imp_trasl, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim traslComprobanteTotImp = FormatNumber(Round(((importeIva + importeIeps)), decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim traslConceptos = FormatNumber(Round(((subtC - mtoDescGlobal) * tasaVarIva), decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            'Dim traslLineitems = FormatNumber(Round(((subtL - descuentoCalculado) * tasaVarIva), decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim traslLineitems As String 'Decimal
            If comprobante.tipodoc_cve = "VTFE" Or comprobante.tipodoc_cve = "BTFACS" Then
                traslLineitems = FormatNumber(Round(((subtL - mtoDescGlobal) * tasaVarIva), decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Else
                traslLineitems = FormatNumber(Round(((subtL - descuentoCalculado) * tasaVarIva), decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            End If

            Dim totalReten = Round((retenImporteIva + retenImporteIsr), decimalesTruncados, MidpointRounding.AwayFromZero)

            If errorCfd = 0 Then

                'If errorCfd = 0 Then
                '    minimoTotal = CType((traslComprobanteTotImp - 0.5), Decimal)
                '    maximoTotal = CType((traslComprobanteTotImp + 0.5), Decimal)
                '    If traslComprobanteTotImp < minimoTotal Or traslComprobanteTotImp > maximoTotal Then
                '        errorCfd = 4
                '    End If
                'End If
                'If errorCfd = 0 Then
                '    minimoTotal = CType((traslConceptos - 0.5), Decimal)
                '    maximoTotal = CType((traslConceptos + 0.5), Decimal)
                '    If traslConceptos < minimoTotal Or traslConceptos > maximoTotal Then
                '        errorCfd = 4
                '    End If
                'End If
                If errorCfd = 0 Then
                    minimoTotal = CType((traslConceptos - Excepmax), Decimal)
                    maximoTotal = CType((traslConceptos + Excepmax), Decimal)

                    If traslComprobanteTotImp < minimoTotal Or traslComprobanteTotImp > maximoTotal Then
                        errorCfd = 4
                    End If

                End If

                If comprobante.tipodoc_cve <> "BTFACS" Then
                    If errorCfd = 0 Then
                        minimoTotal = CType((traslLineitems - Excepmax), Decimal)
                        maximoTotal = CType((traslLineitems + Excepmax), Decimal)
                        If traslLineitems < minimoTotal Or traslLineitems > maximoTotal Then
                            errorCfd = 4
                        End If
                    End If
                End If

            End If
            Dim totalConceptos = FormatNumber(Round((subtC - mtoDescGlobal + traslConceptos) - totalReten, decimalesTruncados, MidpointRounding.AwayFromZero), decimales) 'Subt_c - mto_desc_global + iva_conceptos
            'Dim totalListitems = FormatNumber(Round((subtL - descuentoCalculado + traslLineitems) - totalReten, decimalesTruncados, MidpointRounding.AwayFromZero), decimales) 'Subt_l - descuento_calculado + iva_lineitems

            Dim totalListitems As String
            If comprobante.tipodoc_cve = "VTFE" Or comprobante.tipodoc_cve = "BTFACS" Then
                totalListitems = FormatNumber(Round((subtL - mtoDescGlobal + traslLineitems) - totalReten, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Else
                totalListitems = FormatNumber(Round((subtL - descuentoCalculado + traslLineitems) - totalReten, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            End If

            If errorCfd = 0 Then
                minimoTotal = CType((totalConceptos - Excepmax), Decimal)
                maximoTotal = CType((totalConceptos + Excepmax), Decimal)
                If totalComprobante < minimoTotal Or totalComprobante > maximoTotal Then
                    errorCfd = 5
                End If
            End If
            If comprobante.tipodoc_cve <> "BTFACS" Then
                If errorCfd = 0 Then
                    minimoTotal = 0
                    maximoTotal = 0
                    minimoTotal = CType((totalListitems - Excepmax), Decimal)
                    maximoTotal = CType((totalListitems + Excepmax), Decimal)
                    If totalComprobante < minimoTotal Or totalComprobante > maximoTotal Then
                        errorCfd = 5
                    End If
                End If
            End If

            Dim msg As String = ""
            Dim er As New Errores
            If errorCfd > 0 Then
                er.Interror = 1
            End If
            Select Case errorCfd
                Case 1
                    msg = "El subtotal de la factura es incorrecto"
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60063", "ValidaTotales")
                Case 2
                    msg = "El porcentaje de descuento en LineItems y monto de descuento del CFD no coinciden"
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60064", "ValidaTotales")
                Case 3, 5
                    msg = "El total de la factura es incorrecto"
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60065", "ValidaTotales")
                Case 4
                    msg = "No coincide el iva"
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60066", "ValidaTotales")
                Case 6
                    msg = "No coincide cantidad, valor unitario de Concepto con uns y precio de lineItem  "
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60067", "ValidaTotales")
                Case Else
                    msg = ""
            End Select
            If errorCfd > 0 Then
                agrega_err(1, msg, errores)
            End If
        End Sub

        Public Sub ValidaTotales3_3(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd, ByVal decimalesTruncados As Integer, ByVal decimales As Integer)

            Dim mtoDescGlobal As Decimal = 0
            Dim minimoTotal As Decimal
            Dim maximoTotal As Decimal
            Dim iva = From cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.importe, cons.impuesto
                       Where (impuesto = "002")
            Dim tasaVarIva As Decimal = 0
            Dim importeIva As Decimal = 0

            Dim cmax As New SqlCommand
            Dim Excepmax As Object
            cmax.CommandText = "select convert(decimal(19,4),prm15) from xcdconapl_cl where tipdoc_cve = 'xmlcfd' and sp_cve = 'webconfig' and spd_cve = 'rangoMinMax'"
            cmax.CommandType = CommandType.Text
            cmax.Connection = Conexion
            Conexion.Open()
            Excepmax = cmax.ExecuteScalar()
            Conexion.Close()

            For Each i In iva
                Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                If swtasa = True Then
                    tasaVarIva = i.tasa / 100
                Else
                    tasaVarIva = i.tasa
                End If
                importeIva = importeIva + i.importe
                'importeIva = i.importe
            Next
            Dim ieps = From cons In comprobante.Impuestos.Traslados
                      Select cons.tasa, cons.importe, cons.impuesto
                      Where (impuesto = "003")
            Dim tasaIeps As Decimal = 0
            Dim importeIeps As Decimal = 0
            For Each i In ieps
                Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                If swtasa = True Then
                    tasaIeps = i.tasa / 100
                Else
                    tasaIeps = i.tasa
                End If
                importeIeps = importeIeps + i.importe
            Next
            Dim retenciones = From cons In comprobante.Impuestos.Retenciones
                    Select cons.importe, cons.impuesto
                    Where (impuesto = "002")
            Dim retenImporteIva As Decimal = 0
            For Each i In retenciones
                retenImporteIva = retenImporteIva + i.importe
            Next
            Dim retisr = From cons In comprobante.Impuestos.Retenciones
                  Select cons.importe, cons.impuesto
                  Where (impuesto = "001")
            Dim retenImporteIsr As Decimal = 0
            For Each i In retisr
                retenImporteIsr = retenImporteIsr + i.importe
            Next
            Dim errorCfd As Integer = 0
            Dim qryResultConceptos = From com In comprobante.Conceptos _
                       Select comprobante.Conceptos
            Dim qryResultItems = From com In comprobante.Addenda.requestforpayment.line_items _
                        Select comprobante.Addenda.requestforpayment.line_items
            Dim concep As List(Of concepto) = Nothing
            Dim con = New concepto
            Dim line As List(Of lineitem) = Nothing
            Dim sumaTotalCon As Decimal
            For Each concep In qryResultConceptos
            Next
            For Each line In qryResultItems
            Next
            Dim qryResLine = From com2 In concep _
                Let sub_total = com2.cantidad * com2.valor_unitario _
                Select sub_total, com2.importe, com2.no_identificacion, com2.cantidad, com2.valor_unitario '_
            Dim qry_tot_con = From com2 In concep _
               Let tot_con = com2.importe _
               Select com2.importe, com2.no_identificacion '_
            sumaTotalCon = Aggregate com2 In qryResLine _
                                    Into Sum(com2.sub_total)
            sumaTotalCon = FormatNumber(Round(sumaTotalCon, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim totalCon = Aggregate com2 In qry_tot_con _
                            Into Sum(com2.importe)
            Dim cantConcep = Aggregate com2 In qryResLine _
                              Into Sum(com2.cantidad)
            Dim valUnit = From com2 In qryResLine _
                            Select com2.valor_unitario _
                            Distinct
            totalCon = FormatNumber(Round(totalCon, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            sumaTotalCon = FormatNumber(Round(sumaTotalCon, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            minimoTotal = sumaTotalCon - Excepmax
            maximoTotal = sumaTotalCon + Excepmax
            If totalCon < minimoTotal Or totalCon > maximoTotal Then
                errorCfd = 1
            End If

            Dim descuentoCalculado As Decimal
            mtoDescGlobal = comprobante.descuento ' Monto global de descuento
            If comprobante.tipodoc_cve <> "BTFACS" Then
                Dim qryResLineItem = From com2 In line _
                 Let sub_total = com2.uns * com2.precio _
                 Select sub_total, com2.pct_decuento, com2.sku, com2.monto_decuento, com2.uns, com2.precio
                Dim sumaTotalLine = Aggregate com2 In qryResLineItem _
                                        Into Sum(com2.sub_total)
                sumaTotalLine = FormatNumber(Round(sumaTotalLine, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)

                Dim unsLine = Aggregate com2 In qryResLineItem _
                          Into Sum(com2.uns)
                Dim precioLine = From com2 In qryResLineItem _
                                Select com2.precio _
                                Distinct
                minimoTotal = CType((sumaTotalLine - Excepmax), Decimal)
                maximoTotal = CType((sumaTotalLine + Excepmax), Decimal)

                'se quito la validacion ocupada en los xml 3.2
                'If sumaTotalCon < minimoTotal Or sumaTotalCon > maximoTotal Then
                '    errorCfd = 1
                'End If
                Dim desctoLineitem = From com3 In line _
                                          Let descto = (com3.uns * com3.precio) * (com3.pct_decuento / 100)
                descuentoCalculado = Aggregate com3 In desctoLineitem _
                                          Into Sum(com3.descto)
                Dim mtoDesc = Aggregate com3 In line _
                               Into Sum(com3.monto_decuento)

                If errorCfd = 0 Then
                    '------------------------------------------ Validar los descuentos
                    'Valida Comprobante: descuento VS LineItems: suma(uns * precio) * pct_desc
                    If descuentoCalculado <> 0 Or mtoDesc <> 0 Then
                        If errorCfd = 0 Then
                            minimoTotal = CType((mtoDesc - Excepmax), Decimal)
                            maximoTotal = CType((mtoDesc + Excepmax), Decimal)
                            If descuentoCalculado < minimoTotal Or descuentoCalculado > maximoTotal Then
                                errorCfd = 2
                            End If
                        End If
                    End If
                End If

            End If



            Dim subtotalComprobante = FormatNumber(Round(comprobante.sub_total, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim subtotalConceptos = From com4 In concep _
                                     Let sub_total = com4.cantidad * com4.valor_unitario _
                                     Select sub_total, com4.importe, com4.no_identificacion
            Dim subtC = Aggregate com4 In subtotalConceptos _
                          Into Sum(com4.sub_total)
            subtC = FormatNumber(Round(subtC, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)

            Dim subtL As Decimal
            If comprobante.tipodoc_cve <> "BTFACS" Then
                Dim subtotalItems = From com4 In line _
                                     Let sub_total = com4.uns * com4.precio _
                                     Select sub_total, com4.pct_decuento, com4.sku, com4.monto_decuento
                subtL = Aggregate com4 In subtotalItems _
                             Into Sum(com4.sub_total - (com4.sub_total * (com4.pct_decuento / 100)))
                subtL = FormatNumber(Round(subtL, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            End If

            'se quito la validacion ocupada en los xml 3.2
            'If errorCfd = 0 Then
            '    '-------------------------------- Subtotales
            '    'If subtotalComprobante <> subtC Then
            '    '    errorCfd = 1
            '    'End If

            '    minimoTotal = CType((subtC - Excepmax), Decimal)
            '    maximoTotal = CType((subtC + Excepmax), Decimal)

            '    If subtotalComprobante < minimoTotal Or subtotalComprobante > maximoTotal Then
            '        errorCfd = 1
            '    End If


            '    If comprobante.tipodoc_cve <> "BTFACS" Then
            '        'If subtotalComprobante <> subtL Then
            '        '    errorCfd = 1
            '        'End If
            '        minimoTotal = CType((subtL - Excepmax), Decimal)
            '        maximoTotal = CType((subtL + Excepmax), Decimal)

            '        If subtotalComprobante < minimoTotal Or subtotalComprobante > maximoTotal Then
            '            errorCfd = 1
            '        End If


            '        If subtC <> subtL Then
            '            agrega_err(1, "No coincide Subtotal de Conceptos con LineItems: [" + subtC.ToString() + ":" + subtL.ToString() + "]", errores)
            '            errorCfd = 1
            '        End If
            '    End If

            'End If
            Dim totalComprobante = FormatNumber(Round(comprobante.total, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim totImpTraslComprobante = FormatNumber(Round(comprobante.Impuestos.total_imp_trasl, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim traslComprobanteTotImp = FormatNumber(Round(((importeIva + importeIeps)), decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim traslConceptos = FormatNumber(Round(((subtC - mtoDescGlobal) * tasaVarIva), decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            'Dim traslLineitems = FormatNumber(Round(((subtL - descuentoCalculado) * tasaVarIva), decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Dim traslLineitems As String 'Decimal
            If comprobante.tipodoc_cve = "VTFE" Or comprobante.tipodoc_cve = "BTFACS" Then
                traslLineitems = FormatNumber(Round(((subtL - mtoDescGlobal) * tasaVarIva), decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Else
                traslLineitems = FormatNumber(Round(((subtL - descuentoCalculado) * tasaVarIva), decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            End If

            Dim totalReten = Round((retenImporteIva + retenImporteIsr), decimalesTruncados, MidpointRounding.AwayFromZero)

            If errorCfd = 0 Then
                'se quito la validacion ocupada en los xml 3.2
                'If errorCfd = 0 Then
                '    minimoTotal = CType((traslComprobanteTotImp - 0.5), Decimal)
                '    maximoTotal = CType((traslComprobanteTotImp + 0.5), Decimal)
                '    If traslComprobanteTotImp < minimoTotal Or traslComprobanteTotImp > maximoTotal Then
                '        errorCfd = 4
                '    End If
                'End If
                'If errorCfd = 0 Then
                '    minimoTotal = CType((traslConceptos - 0.5), Decimal)
                '    maximoTotal = CType((traslConceptos + 0.5), Decimal)
                '    If traslConceptos < minimoTotal Or traslConceptos > maximoTotal Then
                '        errorCfd = 4
                '    End If
                'End If
                'If errorCfd = 0 Then
                '    minimoTotal = CType((traslConceptos - Excepmax), Decimal)
                '    maximoTotal = CType((traslConceptos + Excepmax), Decimal)

                '    If traslComprobanteTotImp < minimoTotal Or traslComprobanteTotImp > maximoTotal Then
                '        errorCfd = 4
                '    End If

                'End If

                If comprobante.tipodoc_cve <> "BTFACS" Then
                    If errorCfd = 0 Then
                        minimoTotal = CType((traslLineitems - Excepmax), Decimal)
                        maximoTotal = CType((traslLineitems + Excepmax), Decimal)
                        If traslLineitems < minimoTotal Or traslLineitems > maximoTotal Then
                            errorCfd = 4
                        End If
                    End If
                End If

            End If
            Dim totalConceptos = FormatNumber(Round((subtC - mtoDescGlobal + traslConceptos) - totalReten, decimalesTruncados, MidpointRounding.AwayFromZero), decimales) 'Subt_c - mto_desc_global + iva_conceptos
            'Dim totalListitems = FormatNumber(Round((subtL - descuentoCalculado + traslLineitems) - totalReten, decimalesTruncados, MidpointRounding.AwayFromZero), decimales) 'Subt_l - descuento_calculado + iva_lineitems

            Dim totalListitems As String
            If comprobante.tipodoc_cve = "VTFE" Or comprobante.tipodoc_cve = "BTFACS" Then
                totalListitems = FormatNumber(Round((subtL - mtoDescGlobal + traslLineitems) - totalReten, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            Else
                totalListitems = FormatNumber(Round((subtL - descuentoCalculado + traslLineitems) - totalReten, decimalesTruncados, MidpointRounding.AwayFromZero), decimales)
            End If
            'se quito la validacion ocupada en los xml 3.2
            'If errorCfd = 0 Then
            '    minimoTotal = CType((totalConceptos - Excepmax), Decimal)
            '    maximoTotal = CType((totalConceptos + Excepmax), Decimal)
            '    If totalComprobante < minimoTotal Or totalComprobante > maximoTotal Then
            '        errorCfd = 5
            '    End If
            'End If
            If comprobante.tipodoc_cve <> "BTFACS" Then
                If errorCfd = 0 Then
                    minimoTotal = 0
                    maximoTotal = 0
                    minimoTotal = CType((totalListitems - Excepmax), Decimal)
                    maximoTotal = CType((totalListitems + Excepmax), Decimal)
                    If totalComprobante < minimoTotal Or totalComprobante > maximoTotal Then
                        errorCfd = 5
                    End If
                End If
            End If

            'Valida Conceptos: suma(cantidad*valorUnitario) VS LineItems: suma(uns * precio)
            If totalListitems <> subtC Then
                errorCfd = 7
            End If

            'Valida Comprobante: subtotal VS LineItems: suma(uns * precio).
            If totalListitems <> subtotalComprobante Then
                errorCfd = 8
            End If

            Dim msg As String = ""
            Dim er As New Errores
            If errorCfd > 0 Then
                er.Interror = 1
            End If
            Select Case errorCfd
                Case 1
                    msg = "El subtotal de la factura es incorrecto"
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60063", "ValidaTotales")
                Case 2
                    msg = "El porcentaje de descuento en LineItems y monto de descuento del CFD no coinciden"
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60064", "ValidaTotales")
                Case 3, 5
                    msg = "El total de la factura es incorrecto"
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60065", "ValidaTotales")
                Case 4
                    msg = "No coincide el iva"
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60066", "ValidaTotales")
                Case 6
                    msg = "No coincide cantidad, valor unitario de Concepto con uns y precio de lineItem  "
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60067", "ValidaTotales")
                Case 7
                    msg = "No coincide la sumatoria de Conceptos y la sumatoria de LineItems"
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60067", "ValidaTotales")
                Case 8
                    msg = "El subtotal no coincide con la sumatoria de LineItems"
                    er.Message = msg
                    graba_error(errores, er, llaveCfd, "60067", "ValidaTotales")
                Case Else
                    msg = ""
            End Select
            If errorCfd > 0 Then
                agrega_err(1, msg, errores)
            End If
        End Sub

        Public Sub ValidaTotales_SNAdd(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal LlaveCfd As llave_cfd, ByVal decimales_truncados As Integer, ByVal decimales As Integer)

            'GCM 04022015 para contemplar la configuracion de noiva
            Dim cmd As New SqlCommand
            Dim Excep As Object
            Dim cmax As New SqlCommand
            Dim Excepmax As Object
            cmd.CommandText = "select count(*) from xcdconapl_cl where tipdoc_cve = 'xmlcfd' and sp_cve = 'novaliva' and spd_cve = '" + comprobante.cc_tipo + "'  and prm7 =  '" + LlaveCfd.rfc_emisor + "'"
            cmax.CommandText = "select convert(decimal(19,4),prm15) from xcdconapl_cl where tipdoc_cve = 'xmlcfd' and sp_cve = 'webconfig' and spd_cve = 'rangoMinMax'"
            cmd.CommandType = CommandType.Text
            cmax.CommandType = CommandType.Text
            cmd.Connection = Conexion
            cmax.Connection = Conexion
            Conexion.Open()
            Excep = cmd.ExecuteScalar()
            Excepmax = cmax.ExecuteScalar()
            Conexion.Close()


            Dim mtoDescGlobal As Decimal = 0
            Dim minimoTotal As Decimal
            Dim maximoTotal As Decimal
            Dim iva = From cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.importe, cons.impuesto
                       Where (impuesto = "IVA" And importe > 0)
            Dim tasaVarIva As Decimal = 0 '= comprobante.Impuestos.Traslados.tasa / 100
            Dim importeIva As Decimal = 0
            Dim totalConceptos As Decimal = 0
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
            'If LlaveCfd.timbre_fiscal.uuid = "C677B813-4D8D-4390-9924-6614787A34C4" Then
            '    importeIva = importeIva / 2
            'End If

            Dim ieps = From cons In comprobante.Impuestos.Traslados
                      Select cons.tasa, cons.importe, cons.impuesto
                      Where (impuesto = "IEPS")
            Dim tasaIeps As Decimal = 0 '= comprobante.Impuestos.Traslados.tasa / 100
            Dim importeIeps As Decimal = 0
            For Each i In ieps
                Dim swtasa = RevisaIntTasa(i.tasa.ToString())

                If swtasa = True Then
                    tasaIeps = i.tasa / 100 ' FormatNumber(i.tasa / 100, decimales)
                Else
                    tasaIeps = i.tasa
                End If


                importeIeps = importeIeps + i.importe ' FormatNumber(i.importe, decimales)
            Next
            Dim retenciones = From cons In comprobante.Impuestos.Retenciones
                    Select cons.importe, cons.impuesto
                    Where (impuesto = "IVA")
            Dim retenImporteIva As Decimal = 0
            For Each i In retenciones
                retenImporteIva = retenImporteIva + i.importe 'FormatNumber(i.importe, decimales)
            Next
            Dim retisr = From cons In comprobante.Impuestos.Retenciones
                  Select cons.importe, cons.impuesto
                  Where (impuesto = "ISR")
            Dim retenImporteIsr As Decimal = 0
            For Each i In retisr
                retenImporteIsr = retenImporteIsr + i.importe 'FormatNumber(i.importe, decimales)
            Next
            Dim errorCfd As Integer = 0
            Dim qryResultConceptos = From com In comprobante.Conceptos _
                       Select comprobante.Conceptos
            Dim concep As List(Of concepto) = Nothing
            For Each concep In qryResultConceptos
            Next
            Dim qryResLine = From com2 In concep _
                Let sub_total = com2.cantidad * com2.valor_unitario _
                Select sub_total, com2.importe, com2.no_identificacion, com2.cantidad, com2.valor_unitario
            Dim qry_tot_con = From com2 In concep _
               Let tot_con = com2.importe _
               Select com2.importe, com2.no_identificacion
            Dim cant_concep = Aggregate com2 In qryResLine _
                              Into Sum(com2.cantidad)
            Dim val_unit = From com2 In qryResLine _
                            Select com2.valor_unitario _
                            Distinct
            If errorCfd = 0 Then
                '------------------------------------------ Validar los descuentos
                mtoDescGlobal = FormatNumber(comprobante.descuento, decimales) ' Monto global de descuento
            End If
            Dim subtotalComprobante = FormatNumber(comprobante.sub_total, decimales_truncados)
            Dim subtotalConceptos = From com4 In concep _
                                     Let sub_total = com4.cantidad * com4.valor_unitario _
                                     Select sub_total, com4.importe, com4.no_identificacion
            Dim Subt_c = Aggregate com4 In subtotalConceptos _
                          Into Sum(com4.sub_total)
            Dim traslConceptos As Decimal
            Subt_c = FormatNumber(Round(Subt_c, decimales_truncados, MidpointRounding.AwayFromZero), decimales)

            If comprobante.tipodoc_cve <> "BTFSER" Then
                If errorCfd = 0 Then
                    minimoTotal = subtotalComprobante - Excepmax
                    maximoTotal = subtotalComprobante + Excepmax
                    If Subt_c < minimoTotal Or Subt_c > maximoTotal Then
                        errorCfd = 1
                    End If
                End If
            End If
            'GCM 23032015 mtoDescGlobal se agrego para el BTFSER
            Dim totaltrasladados = comprobante.TotaldeTraslados
            Dim totalComprobante = FormatNumber(comprobante.total, decimales_truncados)
            Dim traslComprobanteTotImp = FormatNumber(Round(((importeIva + importeIeps)), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            If comprobante.tipodoc_cve <> "BTFSER" Then
                traslConceptos = FormatNumber(Round(((Subt_c - mtoDescGlobal) * tasaVarIva), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            Else
                traslConceptos = FormatNumber(Round(((subtotalComprobante - mtoDescGlobal) * tasaVarIva), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            End If

            Dim totImpRet = (retenImporteIva + retenImporteIsr)
            If errorCfd = 0 Then

                'If errorCfd = 0 Then
                '    minimoTotal = traslComprobanteTotImp - 0.5
                '    maximoTotal = traslComprobanteTotImp + 0.5

                '    If traslComprobanteTotImp < minimoTotal Or traslComprobanteTotImp > maximoTotal Then
                '        errorCfd = 4
                '    End If

                'End If


                'If errorCfd = 0 Then 'tot_imp_trasl_comprobante <> trasl_conceptos Then
                '    minimoTotal = traslConceptos - 0.5
                '    maximoTotal = traslConceptos + 0.5

                '    If traslConceptos < minimoTotal Or traslConceptos > maximoTotal Then
                '        errorCfd = 4
                '    End If

                'End If

                If errorCfd = 0 Then
                    minimoTotal = CType((traslConceptos - Excepmax), Decimal)
                    maximoTotal = CType((traslConceptos + Excepmax), Decimal)

                    If traslComprobanteTotImp < minimoTotal Or traslComprobanteTotImp > maximoTotal Then
                        errorCfd = 4
                    End If

                End If

            End If

            'GCM 30012015 para no validar IVA en las facturas BTFSER para estos rfc
            If errorCfd = 4 And comprobante.tipodoc_cve = "BTFSER" And Excep > 0 Then '(LlaveCfd.rfc_emisor = "RTS591030RL3" Or LlaveCfd.rfc_emisor = "RDI841003QJ4" Or LlaveCfd.rfc_emisor = "CAHC740704H99") Then
                errorCfd = 0
                traslConceptos = importeIva
            End If

            'GCM 17122014 hasta que lo autorice 
            If comprobante.tipodoc_cve = "BTFSER" Then
                totalConceptos = FormatNumber(Round((subtotalComprobante - mtoDescGlobal + traslConceptos - totImpRet + comprobante.TotaldeTraslados - comprobante.TotaldeRetenciones), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            Else
                totalConceptos = FormatNumber(Round((Subt_c - mtoDescGlobal + traslConceptos - totImpRet), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            End If

            If errorCfd = 0 Then
                minimoTotal = totalConceptos - Excepmax
                maximoTotal = totalConceptos + Excepmax

                If totalComprobante < minimoTotal Or totalComprobante > maximoTotal Then
                    errorCfd = 5
                End If
            End If


            If LlaveCfd.rfc_emisor = "CAHC740704H99" Or LlaveCfd.rfc_emisor = "TCH850701RM1" Then ' Or LlaveCfd.rfc_emisor = "HMI950125KG8" Then
                errorCfd = 0
            End If

            If LlaveCfd.rfc_emisor = "EIZJ480901PJ7" And LlaveCfd.timbre_fiscal.uuid = "92BD3F8B-D023-400F-A2F0-171809C9D588" Then
                errorCfd = 0
            End If

            If LlaveCfd.rfc_emisor = "IABC820706GM6" Then 'GCM se mete exepcion 15102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
                errorCfd = 0
            End If
            'GCM 23102014 Para dejar pasar unas facturas
            If LlaveCfd.rfc_emisor = "COM890206254" And (
                                                        comprobante.folio_factura = 12900 Or
                                                        comprobante.folio_factura = 12901 Or
                                                        comprobante.folio_factura = 12902 Or
                                                        comprobante.folio_factura = 12903 Or
                                                        comprobante.folio_factura = 12905 Or
                                                        comprobante.folio_factura = 12906 Or
                                                        comprobante.folio_factura = 12907 Or
                                                        comprobante.folio_factura = 12909 Or
                                                        comprobante.folio_factura = 12911) Then
                errorCfd = 0
            End If

            'GCM 06012015
            If LlaveCfd.rfc_emisor = "CFE370814QI0" And (
                                                            LlaveCfd.timbre_fiscal.uuid = "5CE11D93-82E3-48F5-84A5-AC3DAE9E5C65" Or
                                                            LlaveCfd.timbre_fiscal.uuid = "64301B4C-A63A-4899-BE4F-C1DF4C96A8E9" Or
                                                            LlaveCfd.timbre_fiscal.uuid = "BCF8ECD3-58C1-4F70-90BD-EA6E5DA01223" Or
                                                            LlaveCfd.timbre_fiscal.uuid = "B100ACE9-A0A9-4D43-8A78-45F2C6F80035") Then
                errorCfd = 0
            End If

            'GCM 23032015
            If LlaveCfd.rfc_emisor = "CPF6307036N8" And LlaveCfd.timbre_fiscal.uuid = "71b1321e-729a-4341-9904-aaf225b8fe7b" Then
                errorCfd = 0
            End If

            'GCM 04012016
            If LlaveCfd.rfc_emisor = "AOGC740622GL3" And LlaveCfd.timbre_fiscal.uuid = "162EEF97-815B-4688-81EE-563865E67F4F" Then
                errorCfd = 0
            End If

            Dim msg As String = ""
            Dim er As New Errores
            If errorCfd > 0 Then
                er.Interror = 1

            End If
            Select Case errorCfd
                Case 1
                    msg = "El subtotal de la factura es incorrecto"
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60063", "ValidaTotales_SNAdd")
                Case 2
                    msg = "El porcentaje de descuento en LineItems y monto de descuento del CFD no coinciden"
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60064", "ValidaTotales_SNAdd")
                Case 3, 5
                    msg = "El total de la factura es incorrecto"
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60065", "ValidaTotales_SNAdd")
                Case 4
                    msg = "No coincide el iva"
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60066", "ValidaTotales_SNAdd")
                Case 6
                    msg = "No coincide cantidad, valor unitario de Concepto con uns y precio de lineItem  "
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60067", "ValidaTotales_SNAdd")
                Case Else
                    msg = ""
            End Select
            If errorCfd > 0 Then
                agrega_err(1, msg, errores)
            End If
        End Sub

        Public Sub ValidaTotales_SNAdd3_3(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal LlaveCfd As llave_cfd, ByVal decimales_truncados As Integer, ByVal decimales As Integer)

            'GCM 04022015 para contemplar la configuracion de noiva
            Dim cmd As New SqlCommand
            Dim Excep As Object
            Dim cmax As New SqlCommand
            Dim Excepmax As Object
            cmd.CommandText = "select count(*) from xcdconapl_cl where tipdoc_cve = 'xmlcfd' and sp_cve = 'novaliva' and spd_cve = '" + comprobante.cc_tipo + "'  and prm7 =  '" + LlaveCfd.rfc_emisor + "'"
            cmax.CommandText = "select convert(decimal(19,4),prm15) from xcdconapl_cl where tipdoc_cve = 'xmlcfd' and sp_cve = 'webconfig' and spd_cve = 'rangoMinMax'"
            cmd.CommandType = CommandType.Text
            cmax.CommandType = CommandType.Text
            cmd.Connection = Conexion
            cmax.Connection = Conexion
            Conexion.Open()
            Excep = cmd.ExecuteScalar()
            Excepmax = cmax.ExecuteScalar()
            Conexion.Close()


            Dim mtoDescGlobal As Decimal = 0
            Dim minimoTotal As Decimal
            Dim maximoTotal As Decimal
            Dim iva = From cons In comprobante.Impuestos.Traslados
                        Select cons.tasa, cons.importe, cons.impuesto
                        Where (impuesto = "002" And importe > 0)
            Dim tasaVarIva As Decimal = 0 '= comprobante.Impuestos.Traslados.tasa / 100
            Dim importeIva As Decimal = 0
            Dim totalConceptos As Decimal = 0
            iva = iva.Distinct()
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
            'If LlaveCfd.timbre_fiscal.uuid = "C677B813-4D8D-4390-9924-6614787A34C4" Then
            '    importeIva = importeIva / 2
            'End If

            Dim ieps = From cons In comprobante.Impuestos.Traslados
                      Select cons.tasa, cons.importe, cons.impuesto
                      Where (impuesto = "003")
            Dim tasaIeps As Decimal = 0 '= comprobante.Impuestos.Traslados.tasa / 100
            Dim importeIeps As Decimal = 0
            For Each i In ieps
                Dim swtasa = RevisaIntTasa(i.tasa.ToString())

                If swtasa = True Then
                    tasaIeps = i.tasa / 100 ' FormatNumber(i.tasa / 100, decimales)
                Else
                    tasaIeps = i.tasa
                End If


                importeIeps = importeIeps + i.importe ' FormatNumber(i.importe, decimales)
            Next
            Dim retenciones = From cons In comprobante.Impuestos.Retenciones
                    Select cons.importe, cons.impuesto
                    Where (impuesto = "002")
            Dim retenImporteIva As Decimal = 0
            retenciones = retenciones.Distinct
            For Each i In retenciones
                retenImporteIva = retenImporteIva + i.importe 'FormatNumber(i.importe, decimales)
            Next
            Dim retisr = From cons In comprobante.Impuestos.Retenciones
                  Select cons.importe, cons.impuesto
                  Where (impuesto = "001")
            Dim retenImporteIsr As Decimal = 0
            For Each i In retisr
                retenImporteIsr = retenImporteIsr + i.importe 'FormatNumber(i.importe, decimales)
            Next
            Dim errorCfd As Integer = 0
            Dim qryResultConceptos = From com In comprobante.Conceptos _
                       Select comprobante.Conceptos
            Dim concep As List(Of concepto) = Nothing
            For Each concep In qryResultConceptos
            Next
            Dim qryResLine = From com2 In concep _
                Let sub_total = com2.cantidad * com2.valor_unitario _
                Select sub_total, com2.importe, com2.no_identificacion, com2.cantidad, com2.valor_unitario
            Dim qry_tot_con = From com2 In concep _
               Let tot_con = com2.importe _
               Select com2.importe, com2.no_identificacion
            Dim cant_concep = Aggregate com2 In qryResLine _
                              Into Sum(com2.cantidad)
            Dim val_unit = From com2 In qryResLine _
                            Select com2.valor_unitario _
                            Distinct

            Dim totalListitems = Aggregate com2 In qryResLine _
                              Into Sum(com2.sub_total)
            If errorCfd = 0 Then
                '------------------------------------------ Validar los descuentos
                mtoDescGlobal = FormatNumber(comprobante.descuento, decimales) ' Monto global de descuento
            End If
            Dim subtotalComprobante = FormatNumber(comprobante.sub_total, decimales_truncados)
            Dim subtotalConceptos = From com4 In concep _
                                     Let sub_total = com4.cantidad * com4.valor_unitario _
                                     Select sub_total, com4.importe, com4.no_identificacion
            Dim Subt_c = Aggregate com4 In subtotalConceptos _
                          Into Sum(com4.sub_total)
            Dim traslConceptos As Decimal
            Subt_c = FormatNumber(Round(Subt_c, decimales_truncados, MidpointRounding.AwayFromZero), decimales)

            If comprobante.tipodoc_cve <> "BTFSER" Then
                If errorCfd = 0 Then
                    minimoTotal = subtotalComprobante - Excepmax
                    maximoTotal = subtotalComprobante + Excepmax
                    If Subt_c < minimoTotal Or Subt_c > maximoTotal Then
                        errorCfd = 1
                    End If
                End If
            End If
            'GCM 23032015 mtoDescGlobal se agrego para el BTFSER
            Dim totaltrasladados = comprobante.TotaldeTraslados
            Dim totalComprobante = FormatNumber(comprobante.total, decimales_truncados)
            Dim traslComprobanteTotImp = FormatNumber(Round(((importeIva + importeIeps)), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            If comprobante.tipodoc_cve <> "BTFSER" Then
                traslConceptos = FormatNumber(Round(((Subt_c - mtoDescGlobal) * tasaVarIva), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            Else
                traslConceptos = FormatNumber(Round(((subtotalComprobante - mtoDescGlobal) * tasaVarIva), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            End If

            Dim totImpRet = (retenImporteIva + retenImporteIsr)
            If errorCfd = 0 Then
                'se quito la validacion ocupada en los xml 3.2
                'If errorCfd = 0 Then
                '    minimoTotal = traslComprobanteTotImp - 0.5
                '    maximoTotal = traslComprobanteTotImp + 0.5

                '    If traslComprobanteTotImp < minimoTotal Or traslComprobanteTotImp > maximoTotal Then
                '        errorCfd = 4
                '    End If

                'End If


                'If errorCfd = 0 Then 'tot_imp_trasl_comprobante <> trasl_conceptos Then
                '    minimoTotal = traslConceptos - 0.5
                '    maximoTotal = traslConceptos + 0.5

                '    If traslConceptos < minimoTotal Or traslConceptos > maximoTotal Then
                '        errorCfd = 4
                '    End If

                'End If

                If errorCfd = 0 Then
                    minimoTotal = CType((traslConceptos - Excepmax), Decimal)
                    maximoTotal = CType((traslConceptos + Excepmax), Decimal)

                    If traslComprobanteTotImp < minimoTotal Or traslComprobanteTotImp > maximoTotal Then
                        errorCfd = 4
                    End If

                End If

            End If

            'GCM 30012015 para no validar IVA en las facturas BTFSER para estos rfc
            If errorCfd = 4 And comprobante.tipodoc_cve = "BTFSER" And Excep > 0 Then '(LlaveCfd.rfc_emisor = "RTS591030RL3" Or LlaveCfd.rfc_emisor = "RDI841003QJ4" Or LlaveCfd.rfc_emisor = "CAHC740704H99") Then
                errorCfd = 0
                traslConceptos = importeIva
            End If

            'GCM 17122014 hasta que lo autorice 
            If comprobante.tipodoc_cve = "BTFSER" Then
                totalConceptos = FormatNumber(Round((subtotalComprobante - mtoDescGlobal + traslConceptos - totImpRet + comprobante.TotaldeTraslados - comprobante.TotaldeRetenciones), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            Else
                totalConceptos = FormatNumber(Round((Subt_c - mtoDescGlobal + traslConceptos - totImpRet), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            End If

            If errorCfd = 0 Then
                minimoTotal = totalConceptos - Excepmax
                maximoTotal = totalConceptos + Excepmax

                If totalComprobante < minimoTotal Or totalComprobante > maximoTotal Then
                    errorCfd = 5
                End If
            End If


            If LlaveCfd.rfc_emisor = "CAHC740704H99" Or LlaveCfd.rfc_emisor = "TCH850701RM1" Then ' Or LlaveCfd.rfc_emisor = "HMI950125KG8" Then
                errorCfd = 0
            End If

            If LlaveCfd.rfc_emisor = "EIZJ480901PJ7" And LlaveCfd.timbre_fiscal.uuid = "92BD3F8B-D023-400F-A2F0-171809C9D588" Then
                errorCfd = 0
            End If

            If LlaveCfd.rfc_emisor = "IABC820706GM6" Then 'GCM se mete exepcion 15102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
                errorCfd = 0
            End If
            'GCM 23102014 Para dejar pasar unas facturas
            If LlaveCfd.rfc_emisor = "COM890206254" And (
                                                        comprobante.folio_factura = 12900 Or
                                                        comprobante.folio_factura = 12901 Or
                                                        comprobante.folio_factura = 12902 Or
                                                        comprobante.folio_factura = 12903 Or
                                                        comprobante.folio_factura = 12905 Or
                                                        comprobante.folio_factura = 12906 Or
                                                        comprobante.folio_factura = 12907 Or
                                                        comprobante.folio_factura = 12909 Or
                                                        comprobante.folio_factura = 12911) Then
                errorCfd = 0
            End If

            'GCM 06012015
            If LlaveCfd.rfc_emisor = "CFE370814QI0" And (
                                                            LlaveCfd.timbre_fiscal.uuid = "5CE11D93-82E3-48F5-84A5-AC3DAE9E5C65" Or
                                                            LlaveCfd.timbre_fiscal.uuid = "64301B4C-A63A-4899-BE4F-C1DF4C96A8E9" Or
                                                            LlaveCfd.timbre_fiscal.uuid = "BCF8ECD3-58C1-4F70-90BD-EA6E5DA01223" Or
                                                            LlaveCfd.timbre_fiscal.uuid = "B100ACE9-A0A9-4D43-8A78-45F2C6F80035") Then
                errorCfd = 0
            End If

            'GCM 23032015
            If LlaveCfd.rfc_emisor = "CPF6307036N8" And LlaveCfd.timbre_fiscal.uuid = "71b1321e-729a-4341-9904-aaf225b8fe7b" Then
                errorCfd = 0
            End If

            'GCM 04012016
            If LlaveCfd.rfc_emisor = "AOGC740622GL3" And LlaveCfd.timbre_fiscal.uuid = "162EEF97-815B-4688-81EE-563865E67F4F" Then
                errorCfd = 0
            End If

            'Valida Conceptos: suma(cantidad*valorUnitario) VS LineItems: suma(uns * precio)
            If Not totalListitems = Subt_c Then
                errorCfd = 7
            End If

            'Valida Comprobante: subtotal VS LineItems: suma(uns * precio).
            If Not totalListitems = subtotalComprobante Then
                errorCfd = 8
            End If

            Dim msg As String = ""
            Dim er As New Errores
            If errorCfd > 0 Then
                er.Interror = 1

            End If
            Select Case errorCfd
                Case 1
                    msg = "El subtotal de la factura es incorrecto"
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60063", "ValidaTotales_SNAdd")
                Case 2
                    msg = "El porcentaje de descuento en LineItems y monto de descuento del CFD no coinciden"
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60064", "ValidaTotales_SNAdd")
                Case 3, 5
                    msg = "El total de la factura es incorrecto"
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60065", "ValidaTotales_SNAdd")
                Case 4
                    msg = "No coincide el iva"
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60066", "ValidaTotales_SNAdd")
                Case 6
                    msg = "No coincide cantidad, valor unitario de Concepto con uns y precio de lineItem  "
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60067", "ValidaTotales_SNAdd")
                Case 7
                    msg = "No coincide la sumatoria de Conceptos y la sumatoria de LineItems"
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60067", "ValidaTotales")
                Case 8
                    msg = "El subtotal no coincide con la sumatoria de LineItems"
                    er.Message = msg
                    graba_error(errores, er, LlaveCfd, "60067", "ValidaTotales")
                Case Else
                    msg = ""
            End Select
            If errorCfd > 0 Then
                agrega_err(1, msg, errores)
            End If
        End Sub

        Public Sub ExecTmpFact(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd, ByVal numInfo As Integer)
            Dim sqlAdapter = New SqlDataAdapter("sp_consInfXML", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@num_info", numInfo)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_consInfXML")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                Mensaje = CType(ValidacionEncabezado.Rows(0).Item("msg"), String)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                Else
                    agrega_err(iErrorG, Mensaje, errores)
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
            Catch ex As Exception
                Dim msg As String
                msg = "sp_consInfXML"
                iErrorG = 3
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub

        Public Function lee_ef_cve(ByVal rfcReceptor As String) As Dictionary(Of String, String)
            Dim er As New Errores
            Dim dicEmpresa = New Dictionary(Of String, String)
            Dim sqlAdapter = New SqlDataAdapter("sp_qefcve", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_receptor", rfcReceptor)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_qefcve")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                Dim iCount = ValidacionEncabezado.Rows.Count()
                If iCount > 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString()
                    Dim empresa As String = ValidacionEncabezado.Rows(0).Item("empresa").ToString().Trim()
                    dicEmpresa.Add(EfCveG, empresa)
                End If
                Conexion.Close()
                ds.Reset()
            Catch ex As Exception
                Dim msg = "sp_qefcve:" + rfcReceptor
                iErrorG = 3
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return dicEmpresa
        End Function

        Public Sub ValidaXsdLinqFacSrv(ByVal errores As List(Of Errores), ByVal xmlDocFilePath As String, ByVal llaveCfd As llave_cfd)

            Dim xmlElm As XDocument
            xmlElm = XDocument.Load(xmlDocFilePath)
            Dim schemas As XmlSchemaSet = New XmlSchemaSet()

            Dim leyendasFiscales As Integer = 0
            Dim detallista As Integer = 0
            Dim registrofiscal As Integer = 0
            Dim impuestosLocales As Integer = 0
            Dim donatarias As Integer = 0
            Dim valesdedespensa As Integer = 0
            Dim aerolineas As Integer = 0
            'GCM 27102014 agregue venta vehiculos
            Dim VentaVehiculos As Integer = 0
            'GCM 21012015 agregue terceros
            Dim Terceros As Integer = 0
            'JPO 29-06-2016 AGREGAMOS IEP
            Dim iep As Integer = 0



            Dim qList1 = From xe In xmlElm.Descendants _
               Select xe

            Dim xElements As IEnumerable(Of XElement) = If(TryCast(qList1, XElement()), qList1.ToArray())
            If xElements.Any(Function(xe) xe.Name.LocalName = "LeyendasFiscales") Then
                leyendasFiscales = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "detallista") Then
                detallista = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "CFDIRegistroFiscal") Then
                registrofiscal = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "ImpuestosLocales") Then
                impuestosLocales = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "Donatarias") Then
                donatarias = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "ValesDeDespensa") Then
                valesdedespensa = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "Aerolineas") Then
                aerolineas = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "VentaVehiculos") Then
                VentaVehiculos = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "PorCuentadeTerceros") Then
                Terceros = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "acreditamientoIEPS") Then
                iep = 1
            End If


            Dim xsd22 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/CFD_Skytex-2.2.xsd"))
            Dim xsd32 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdv32.xsd"))
            Dim xsd33 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdv33.xsd"))
            Dim xsdCat33 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/catCFDI33.xsd"))
            Dim xsdAdd As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/AddendaS_v1.0.xsd"))
            Dim xsdTdf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/TimbreFiscalDigital.xsd"))
            Dim xsdLf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/leyendasFisc.xsd"))
            Dim xsdDet As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/detallista.xsd"))
            Dim xsdRegf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdiregistrofiscal.xsd"))
            Dim xsdImp As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/implocal.xsd"))
            Dim xsdDon As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/donat11.xsd"))
            Dim xsdVal As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/valesdedespensa.xsd"))
            Dim xsdAer As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/aerolineas.xsd"))
            Dim xsdVtaV As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/ventavehiculos11.xsd"))
            Dim xsdTer As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/terceros11.xsd"))
            Dim xsdIep As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/AcreditamientoIEPS10.xsd"))

            If llaveCfd.version = "3.3" Then
                'schemas.Add("http://www.sat.gob.mx/cfd/3", xsd33.CreateReader)
                schemas.Add("http://www.sat.gob.mx/sitio_internet/cfd/catalogos", xsdCat33.CreateReader)
                schemas.Add("http://www.skytex.com.mx/sky", xsdAdd.CreateReader)
            Else
                schemas.Add("http://www.sat.gob.mx/cfd/2", xsd22.CreateReader)
                schemas.Add("http://www.sat.gob.mx/cfd/3", xsd32.CreateReader)
                schemas.Add("http://www.skytex.com.mx/sky", xsdAdd.CreateReader)
                schemas.Add("http://www.sat.gob.mx/TimbreFiscalDigital", xsdTdf.CreateReader)
            End If
            If leyendasFiscales = 1 Then
                schemas.Add("http://www.sat.gob.mx/leyendasFiscales", xsdLf.CreateReader)
            End If
            If detallista = 1 Then
                schemas.Add("http://www.sat.gob.mx/detallista", xsdDet.CreateReader)
            End If
            If registrofiscal = 1 Then
                schemas.Add("http://www.sat.gob.mx/registrofiscal", xsdRegf.CreateReader)
            End If
            If impuestosLocales = 1 Then
                schemas.Add("http://www.sat.gob.mx/implocal", xsdImp.CreateReader)
            End If
            If donatarias = 1 Then
                schemas.Add("http://www.sat.gob.mx/donat", xsdDon.CreateReader)
            End If

            If valesdedespensa = 1 Then
                schemas.Add("http://www.sat.gob.mx/valesdedespensa", xsdVal.CreateReader)
            End If

            If aerolineas = 1 Then
                schemas.Add("http://www.sat.gob.mx/aerolineas", xsdAer.CreateReader)
            End If

            If VentaVehiculos = 1 Then
                schemas.Add("http://www.sat.gob.mx/ventavehiculos", xsdVtaV.CreateReader)
            End If

            If Terceros = 1 Then
                schemas.Add("http://www.sat.gob.mx/terceros", xsdTer.CreateReader)
            End If

            If iep = 1 Then
                schemas.Add("http://www.sat.gob.mx/acreditamiento", xsdIep.CreateReader)
            End If

            xmlElm.Validate(schemas, Function(s As Object, e As ValidationEventArgs) XsdErr(s, e, errores, llaveCfd), True)

            If errores.Count > 0 Then
                Dim er As New Errores
                er.Interror = 1
                er.Message = ""
                graba_error(errores, er, llaveCfd, "60068", "XSDErr")
            End If

        End Sub

        Public Sub ValidaXsdLinqFacEmb(ByVal errores As List(Of Errores), ByVal xmlDocFilePath As String, ByVal llaveCfd As llave_cfd)

            Dim xmlElm As XDocument
            xmlElm = XDocument.Load(xmlDocFilePath)
            Dim schemas As XmlSchemaSet = New XmlSchemaSet()

            Dim leyendasFiscales As Integer = 0
            Dim detallista As Integer = 0
            Dim registrofiscal As Integer = 0
            Dim impuestosLocales As Integer = 0
            Dim donatarias As Integer = 0
            Dim valesdedespensa As Integer = 0
            Dim aerolineas As Integer = 0
            'GCM 27102014 agregue venta vehiculos
            Dim VentaVehiculos As Integer = 0
            'GCM 21012015 agregue terceros
            Dim Terceros As Integer = 0
            'JPO 29-06-2016 AGREGAMOS IEP
            Dim iep As Integer = 0

            Dim qList1 = From xe In xmlElm.Descendants _
               Select xe

            Dim xElements As IEnumerable(Of XElement) = If(TryCast(qList1, XElement()), qList1.ToArray())
            If xElements.Any(Function(xe) xe.Name.LocalName = "LeyendasFiscales") Then
                leyendasFiscales = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "detallista") Then
                detallista = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "CFDIRegistroFiscal") Then
                registrofiscal = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "ImpuestosLocales") Then
                impuestosLocales = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "Donatarias") Then
                donatarias = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "ValesDeDespensa") Then
                valesdedespensa = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "Aerolineas") Then
                aerolineas = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "VentaVehiculos") Then
                VentaVehiculos = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "PorCuentadeTerceros") Then
                Terceros = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "acreditamientoIEPS") Then
                iep = 1
            End If

            Dim xsd22 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/CFD_Skytex-2.2.xsd"))
            Dim xsd32 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdv32.xsd"))
            Dim xsd33 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdv33.xsd"))
            Dim xsdCat33 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/catCFDI33.xsd"))
            Dim xsdAdd As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/AddendaS_v1.0.xsd"))
            Dim xsdTdf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/TimbreFiscalDigital.xsd"))
            Dim xsdLf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/leyendasFisc.xsd"))
            Dim xsdDet As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/detallista.xsd"))
            Dim xsdRegf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdiregistrofiscal.xsd"))
            Dim xsdImp As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/implocal.xsd"))
            Dim xsdDon As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/donat11.xsd"))
            Dim xsdVal As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/valesdedespensa.xsd"))
            Dim xsdAer As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/aerolineas.xsd"))
            Dim xsdVtaV As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/ventavehiculos11.xsd"))
            Dim xsdTer As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/terceros11.xsd"))
            Dim xsdIep As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/AcreditamientoIEPS10.xsd"))

            If llaveCfd.version = "3.3" Then
                'schemas.Add("http://www.sat.gob.mx/cfd/3", xsd33.CreateReader)
                schemas.Add("http://www.sat.gob.mx/sitio_internet/cfd/catalogos", xsdCat33.CreateReader)
                schemas.Add("http://www.skytex.com.mx/sky", xsdAdd.CreateReader)
            Else
                schemas.Add("http://www.sat.gob.mx/cfd/2", xsd22.CreateReader)
                schemas.Add("http://www.sat.gob.mx/cfd/3", xsd32.CreateReader)
                schemas.Add("http://www.skytex.com.mx/sky", xsdAdd.CreateReader)
                schemas.Add("http://www.sat.gob.mx/TimbreFiscalDigital", xsdTdf.CreateReader)
            End If
            If leyendasFiscales = 1 Then
                schemas.Add("http://www.sat.gob.mx/leyendasFiscales", xsdLf.CreateReader)
            End If
            If detallista = 1 Then
                schemas.Add("http://www.sat.gob.mx/detallista", xsdDet.CreateReader)
            End If
            If registrofiscal = 1 Then
                schemas.Add("http://www.sat.gob.mx/registrofiscal", xsdRegf.CreateReader)
            End If
            If impuestosLocales = 1 Then
                schemas.Add("http://www.sat.gob.mx/implocal", xsdImp.CreateReader)
            End If
            If donatarias = 1 Then
                schemas.Add("http://www.sat.gob.mx/donat", xsdDon.CreateReader)
            End If
            If valesdedespensa = 1 Then
                schemas.Add("http://www.sat.gob.mx/valesdedespensa", xsdVal.CreateReader)
            End If
            If aerolineas = 1 Then
                schemas.Add("http://www.sat.gob.mx/aerolineas", xsdAer.CreateReader)
            End If
            If VentaVehiculos = 1 Then
                schemas.Add("http://www.sat.gob.mx/ventavehiculos", xsdVtaV.CreateReader)
            End If
            If Terceros = 1 Then
                schemas.Add("http://www.sat.gob.mx/terceros", xsdTer.CreateReader)
            End If
            If iep = 1 Then
                schemas.Add("http://www.sat.gob.mx/acreditamiento", xsdIep.CreateReader)
            End If
            xmlElm.Validate(schemas, Function(s As Object, e As ValidationEventArgs) XsdErr(s, e, errores, llaveCfd), True)

            If errores.Count > 0 Then
                Dim er As New Errores
                er.Interror = 1
                er.Message = ""
                graba_error(errores, er, llaveCfd, "60068", "XSDErr")
            End If

        End Sub

        Public Sub ValidaXsdLinq(ByVal errores As List(Of Errores), ByVal xmlDocFilePath As String, ByVal llaveCfd As llave_cfd)

            Dim xmlElm As XDocument
            xmlElm = XDocument.Load(xmlDocFilePath)
            Dim schemas As XmlSchemaSet = New XmlSchemaSet()

            Dim leyendasFiscales As Integer = 0
            Dim detallista As Integer = 0
            Dim registrofiscal As Integer = 0
            Dim impuestosLocales As Integer = 0
            Dim donatarias As Integer = 0
            Dim valesdedespensa As Integer = 0
            Dim aerolineas As Integer = 0
            'GCM 27102014 agregue venta vehiculos
            Dim VentaVehiculos As Integer = 0
            'GCM 21012015 agregue terceros
            Dim Terceros As Integer = 0
            'JPO 29-06-2016 AGREGAMOS IEP
            Dim iep As Integer = 0

            Dim qList1 = From xe In xmlElm.Descendants _
               Select xe

            Dim xElements As IEnumerable(Of XElement) = If(TryCast(qList1, List(Of XElement)), qList1.ToList())
            If xElements.Any(Function(xe) xe.Name.LocalName = "LeyendasFiscales") Then
                leyendasFiscales = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "detallista") Then
                detallista = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "CFDIRegistroFiscal") Then
                registrofiscal = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "ImpuestosLocales") Then
                impuestosLocales = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "Donatarias") Then
                donatarias = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "ValesDeDespensa") Then
                valesdedespensa = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "Aerolineas") Then
                aerolineas = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "VentaVehiculos") Then
                VentaVehiculos = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "PorCuentadeTerceros") Then
                Terceros = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "acreditamientoIEPS") Then
                iep = 1
            End If

            Dim xsd22 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/CFD_Skytex-2.2.xsd"))
            Dim xsd32 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdv32.xsd"))
            Dim xsd33 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdv33.xsd"))
            Dim xsdCat33 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/catCFDI33.xsd"))
            Dim xsdAdd As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/Addenda_v2.2.xsd"))
            Dim xsdTdf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/TimbreFiscalDigital.xsd"))
            Dim xsdLf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/leyendasFisc.xsd"))
            Dim xsdDet As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/detallista.xsd"))
            Dim xsdRegf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdiregistrofiscal.xsd"))
            Dim xsdImp As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/implocal.xsd"))
            Dim xsdDon As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/donat11.xsd"))
            Dim xsdVal As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/valesdedespensa.xsd"))
            Dim xsdAer As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/aerolineas.xsd"))
            Dim xsdVtaV As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/ventavehiculos11.xsd"))
            Dim xsdTer As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/terceros11.xsd"))
            Dim xsdIep As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/AcreditamientoIEPS10.xsd"))

            If llaveCfd.version = "3.3" Then
                'schemas.Add("http://www.sat.gob.mx/cfd/3", xsd33.CreateReader)
                schemas.Add("http://www.sat.gob.mx/sitio_internet/cfd/catalogos", xsdCat33.CreateReader)
                schemas.Add("http://www.skytex.com.mx/sky", xsdAdd.CreateReader)
            Else
                schemas.Add("http://www.sat.gob.mx/cfd/2", xsd22.CreateReader)
                schemas.Add("http://www.sat.gob.mx/cfd/3", xsd32.CreateReader)
                schemas.Add("http://www.skytex.com.mx/sky", xsdAdd.CreateReader)
                schemas.Add("http://www.sat.gob.mx/TimbreFiscalDigital", xsdTdf.CreateReader)
            End If
            If leyendasFiscales = 1 Then
                schemas.Add("http://www.sat.gob.mx/leyendasFiscales", xsdLf.CreateReader)
            End If
            If detallista = 1 Then
                schemas.Add("http://www.sat.gob.mx/detallista", xsdDet.CreateReader)
            End If
            If registrofiscal = 1 Then
                schemas.Add("http://www.sat.gob.mx/registrofiscal", xsdRegf.CreateReader)
            End If
            If impuestosLocales = 1 Then
                schemas.Add("http://www.sat.gob.mx/implocal", xsdImp.CreateReader)
            End If
            If donatarias = 1 Then
                schemas.Add("http://www.sat.gob.mx/donat", xsdDon.CreateReader)
            End If
            If valesdedespensa = 1 Then
                schemas.Add("http://www.sat.gob.mx/valesdedespensa", xsdVal.CreateReader)
            End If
            If aerolineas = 1 Then
                schemas.Add("http://www.sat.gob.mx/aerolineas", xsdAer.CreateReader)
            End If
            If VentaVehiculos = 1 Then
                schemas.Add("http://www.sat.gob.mx/ventavehiculos", xsdVtaV.CreateReader)
            End If
            If Terceros = 1 Then
                schemas.Add("http://www.sat.gob.mx/terceros", xsdTer.CreateReader)
            End If
            If iep = 1 Then
                schemas.Add("http://www.sat.gob.mx/acreditamiento", xsdIep.CreateReader)
            End If

            xmlElm.Validate(schemas, Function(s As Object, e As ValidationEventArgs) XsdErr(s, e, errores, llaveCfd), True)

            If errores.Count > 0 Then
                Dim er As New Errores
                er.Interror = 1
                er.Message = ""
                graba_error(errores, er, llaveCfd, "60068", "XSDErr")
            End If

        End Sub

        Public Sub ValidaXSDLinq_SNAdd(ByVal errores As List(Of Errores), ByVal xmlDocFilePath As String, ByVal llaveCfd As llave_cfd)

            Dim xmlElm As XDocument
            xmlElm = XDocument.Load(xmlDocFilePath)
            Dim schemas As XmlSchemaSet = New XmlSchemaSet()

            Dim leyendasFiscales As Integer = 0
            Dim detallista As Integer = 0
            Dim registrofiscal As Integer = 0
            Dim impuestosLocales As Integer = 0
            Dim donatarias As Integer = 0
            Dim valesdedespensa As Integer = 0
            Dim aerolineas As Integer = 0
            'GCM 27102014 agregue venta vehiculos
            Dim VentaVehiculos As Integer = 0
            'GCM 21012015 agregue terceros
            Dim Terceros As Integer = 0
            'JPO 29-06-2016 AGREGAMOS IEP
            Dim iep As Integer = 0

            Dim qList1 = From xe In xmlElm.Descendants _
               Select xe

            Dim xElements As IEnumerable(Of XElement) = If(TryCast(qList1, List(Of XElement)), qList1.ToList())
            If xElements.Any(Function(xe) xe.Name.LocalName = "LeyendasFiscales") Then
                leyendasFiscales = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "detallista") Then
                detallista = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "CFDIRegistroFiscal") Then
                registrofiscal = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "ImpuestosLocales") Then
                impuestosLocales = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "Donatarias") Then
                donatarias = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "ValesDeDespensa") Then
                valesdedespensa = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "Aerolineas") Then
                aerolineas = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "VentaVehiculos") Then
                VentaVehiculos = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "PorCuentadeTerceros") Then
                Terceros = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "acreditamientoIEPS") Then
                iep = 1
            End If

            'esto es para remover la addenda
            For Each a In From a1 In xmlElm.Root.Elements Where a1.Name.LocalName = "Addenda"
                a.Remove()
            Next

            Dim xsd22 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/CFD_Skytex-2.2.xsd"))
            Dim xsd32 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdv32.xsd"))
            Dim xsd33 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdv33.xsd"))
            Dim xsdCat33 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/catCFDI33.xsd"))
            Dim xsdTdf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/TimbreFiscalDigital.xsd"))
            Dim xsdLf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/leyendasFisc.xsd"))
            Dim xsdDet As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/detallista.xsd"))
            Dim xsdRegf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdiregistrofiscal.xsd"))
            Dim xsdImp As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/implocal.xsd"))
            Dim xsdDon As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/donat11.xsd"))
            Dim xsdVal As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/valesdedespensa.xsd"))
            Dim xsdAer As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/aerolineas.xsd"))
            Dim xsdVtaV As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/ventavehiculos11.xsd"))
            Dim xsdTer As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/terceros11.xsd"))
            Dim xsdIep As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/AcreditamientoIEPS10.xsd"))

            If llaveCfd.version = "3.3" Then
                'schemas.Add("http://www.sat.gob.mx/cfd/3", xsd33.CreateReader)
                schemas.Add("http://www.sat.gob.mx/sitio_internet/cfd/catalogos", xsdCat33.CreateReader)
            Else
                schemas.Add("http://www.sat.gob.mx/cfd/2", xsd22.CreateReader)
                schemas.Add("http://www.sat.gob.mx/cfd/3", xsd32.CreateReader)
                schemas.Add("http://www.sat.gob.mx/TimbreFiscalDigital", xsdTdf.CreateReader)
            End If
           
            

            If leyendasFiscales = 1 Then
                schemas.Add("http://www.sat.gob.mx/leyendasFiscales", xsdLf.CreateReader)
            End If
            If detallista = 1 Then
                schemas.Add("http://www.sat.gob.mx/detallista", xsdDet.CreateReader)
            End If
            If registrofiscal = 1 Then
                schemas.Add("http://www.sat.gob.mx/registrofiscal", xsdRegf.CreateReader)
            End If
            If impuestosLocales = 1 Then
                schemas.Add("http://www.sat.gob.mx/implocal", xsdImp.CreateReader)
            End If
            If donatarias = 1 Then
                schemas.Add("http://www.sat.gob.mx/donat", xsdDon.CreateReader)
            End If

            If valesdedespensa = 1 Then
                schemas.Add("http://www.sat.gob.mx/valesdedespensa", xsdVal.CreateReader)
            End If
            If aerolineas = 1 Then
                schemas.Add("http://www.sat.gob.mx/aerolineas", xsdAer.CreateReader)
            End If
            If VentaVehiculos = 1 Then
                schemas.Add("http://www.sat.gob.mx/ventavehiculos", xsdVtaV.CreateReader)
            End If
            If Terceros = 1 Then
                schemas.Add("http://www.sat.gob.mx/terceros", xsdTer.CreateReader)
            End If
            If iep = 1 Then
                schemas.Add("http://www.sat.gob.mx/acreditamiento", xsdIep.CreateReader)
            End If

            xmlElm.Validate(schemas, Function(s As Object, e As ValidationEventArgs) XsdErr(s, e, errores, llaveCfd), True)

            If errores.Count > 0 Then
                Dim er As New Errores
                er.Interror = 1
                er.Message = ""
                graba_error(errores, er, llaveCfd, "60068", "XSDErr")
            End If
        End Sub

        Private Function XsdErr(ByVal sender As Object, ByVal e As ValidationEventArgs, ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd) As Errores
            Dim er As New Errores

            iErrorG = 1
            er.Interror = 1
            er.Message = e.Message
            errores.Add(er)
            'graba_error(errores, er, LlaveCFD, "60068", "XSDErr")

            Return er
        End Function

        Public Sub ValidaExisteAddenda(ByVal errores As List(Of Errores), ByVal xmlDocFilePath As String, ByVal llaveCfd As llave_cfd)

            Dim xmlElm As XDocument
            xmlElm = XDocument.Load(xmlDocFilePath)
            Dim schemas As XmlSchemaSet = New XmlSchemaSet()
            Dim iExiste As Int32 = 0
            'esto es para remover la addenda
            For Each a In From a1 In xmlElm.Root.Elements Where a1.Name.LocalName = "Addenda"
                iExiste = 1
            Next

            If iExiste = 0 Then
                Dim er As New Errores
                iErrorG = 1
                er.Interror = 1
                er.Message = "Comprobante sin addenda por favor revisar"
                errores.Add(er)
                graba_error(errores, er, llaveCfd, "60069", "CFD sin addenda")
            End If


        End Sub
        'Public Sub LeeDatos(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd, ByVal xmlDocFilePath As String, ByVal receptor As receptor)

        'End Sub

        'Public Sub LeeDatosLlaveLinq(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd, ByVal xmlDocFilePath As String, ByVal receptor As receptor)
        '    Dim xmlElm As XElement
        '    Try
        '        xmlElm = XElement.Load(xmlDocFilePath)
        '        Dim root As XElement = XElement.Load(xmlDocFilePath)
        '        Dim qAtrib As IEnumerable(Of XAttribute) = _
        '            From atr In root.Attributes() _
        '            Select atr
        '        Dim timbre = New timbre_fiscal_digital
        '        Dim contador = 0

        '        If IsNothing(root.Attribute("version")) Then
        '            agrega_err(1, "No puede leerse el dato version", errores)
        '        Else
        '            llaveCfd.version = root.Attribute("version").Value.ToString()
        '        End If
        '        Select Case llaveCfd.version
        '            Case "2.2"
        '                llaveCfd.version_nom = "CFD"
        '            Case "3.2"
        '                llaveCfd.version_nom = "CFDI"
        '            Case Else
        '                agrega_err(1, "Version de comprobante incorrecta", errores)
        '                llaveCfd.version_nom = ""
        '        End Select
        '        Dim sello As String = ""
        '        If IsNothing(root.Attribute("sello")) Then
        '            agrega_err(1, "No puede leerse el dato sello", errores)
        '        Else
        '            sello = root.Attribute("sello").Value.ToString()
        '        End If
        '        If llaveCfd.version_nom = "CFDI" Then
        '            Dim qList = From xe In xmlElm.Descendants _
        '                        Select xe
        '            For Each xe In qList
        '                If xe.Name.LocalName = "TimbreFiscalDigital" Then
        '                    timbre.version = xe.Attribute("version").Value
        '                    timbre.uuid = xe.Attribute("UUID").Value
        '                    timbre.fecha_timbrado = xe.Attribute("FechaTimbrado").Value
        '                    timbre.sello_cfd = xe.Attribute("selloCFD").Value
        '                    timbre.no_certificado_sat = xe.Attribute("noCertificadoSAT").Value
        '                    timbre.sello_sat = xe.Attribute("selloSAT").Value
        '                    contador = 1
        '                    Exit For
        '                End If
        '            Next
        '            llaveCfd.timbre_fiscal = timbre
        '            'GCM 22102014 se agrego el error sin timbre fiscal
        '            If contador = 0 Then
        '                agrega_err(1, "sin timbre fiscal", errores)
        '            End If

        '            If sello <> llaveCfd.timbre_fiscal.sello_cfd And contador = 1 Then
        '                agrega_err(1, "el sello es diferente al del timbre fiscal", errores)
        '            End If
        '        Else
        '            timbre.uuid = ""
        '            llaveCfd.timbre_fiscal = timbre
        '        End If
        '        If IsNothing(root.Attribute("serie")) Then
        '            llaveCfd.serie = ""
        '        Else
        '            llaveCfd.serie = root.Attribute("serie").Value.ToString()
        '        End If
        '        If IsNothing(root.Attribute("folio")) Then
        '            llaveCfd.folio_factura = 0
        '        Else
        '            'GCM 17102014 Limpia el folio

        '            Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("folio").Value, "[^0-9]", "")) 'Int32.Parse(root.Attribute("folio").Value)

        '            If folPaso >= 2147483640 Then
        '                llaveCfd.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
        '            Else
        '                llaveCfd.folio_factura = folPaso
        '            End If

        '        End If
        '        Dim qList1 = From xe In xmlElm.Descendants _
        '        Select xe
        '        For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "Emisor"
        '            llaveCfd.rfc_emisor = CType(xe.Attribute("rfc"), String)
        '            Exit For
        '        Next
        '        For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "Receptor"
        '            receptor.rfc = CType(xe.Attribute("rfc"), String)
        '            Exit For
        '        Next
        '        If llaveCfd.rfc_emisor = "" Then
        '            agrega_err(1, "No puede leerse el dato rfc del Emisor", errores)
        '        End If
        '        If receptor.rfc = "" Then
        '            agrega_err(1, "No puede leerse el dato rfc del Receptor", errores)
        '        End If
        '    Catch ex As Exception
        '        agrega_err(1, ex.Message, errores)
        '    End Try

        'End Sub

        Public Sub LeeDatosLlaveLinq(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd, ByVal xmlDocFilePath As String, ByVal receptor As receptor)
            Dim xmlElm As XElement
            Try
                xmlElm = XElement.Load(xmlDocFilePath)
                Dim root As XElement = XElement.Load(xmlDocFilePath)
                Dim qAtrib As IEnumerable(Of XAttribute) = _
                    From atr In root.Attributes() _
                    Select atr
                Dim timbre = New timbre_fiscal_digital
                Dim contador = 0



                If IsNothing(root.Attribute("version")) Then
                    If IsNothing(root.Attribute("Version")) Then
                        agrega_err(1, "No puede leerse el dato version", errores)
                    Else
                        llaveCfd.version = root.Attribute("Version").Value.ToString()
                    End If
                    'agrega_err(1, "No puede leerse el dato version", errores)
                Else
                    llaveCfd.version = root.Attribute("version").Value.ToString()
                End If
                Select Case llaveCfd.version
                    Case "2.2"
                        llaveCfd.version_nom = "CFD"
                    Case "3.2"
                        llaveCfd.version_nom = "CFDI"
                    Case "3.3"
                        llaveCfd.version_nom = "CFDI"
                    Case Else
                        agrega_err(1, "Version de comprobante incorrecta", errores)
                        llaveCfd.version_nom = ""
                End Select
                Dim sello As String = ""

                If llaveCfd.version = "3.3" Then
                    If IsNothing(root.Attribute("Sello")) Then
                        agrega_err(1, "No puede leerse el dato sello", errores)
                    Else
                        sello = root.Attribute("Sello").Value.ToString()
                    End If
                    If llaveCfd.version_nom = "CFDI" Then
                        Dim qList = From xe In xmlElm.Descendants _
                                    Select xe
                        For Each xe In qList
                            If xe.Name.LocalName = "TimbreFiscalDigital" Then
                                timbre.version = xe.Attribute("Version").Value
                                timbre.uuid = xe.Attribute("UUID").Value
                                timbre.fecha_timbrado = xe.Attribute("FechaTimbrado").Value
                                timbre.sello_cfd = xe.Attribute("SelloCFD").Value
                                timbre.no_certificado_sat = xe.Attribute("NoCertificadoSAT").Value
                                timbre.sello_sat = xe.Attribute("SelloSAT").Value
                                contador = 1
                                Exit For
                            End If
                        Next
                        llaveCfd.timbre_fiscal = timbre
                        'GCM 22102014 se agrego el error sin timbre fiscal
                        If contador = 0 Then
                            agrega_err(1, "sin timbre fiscal", errores)
                        End If

                        If sello <> llaveCfd.timbre_fiscal.sello_cfd And contador = 1 Then
                            agrega_err(1, "el sello es diferente al del timbre fiscal", errores)
                        End If
                    Else
                        timbre.uuid = ""
                        llaveCfd.timbre_fiscal = timbre
                    End If
                    If IsNothing(root.Attribute("Serie")) Then
                        llaveCfd.serie = ""
                    Else
                        llaveCfd.serie = root.Attribute("Serie").Value.ToString()
                    End If
                    If IsNothing(root.Attribute("Folio")) Then
                        llaveCfd.folio_factura = 0
                    Else
                        'GCM 17102014 Limpia el folio

                        Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("Folio").Value, "[^0-9]", "")) 'Int32.Parse(root.Attribute("folio").Value)

                        If folPaso >= 2147483647 Then
                            'llaveCfd.folio_factura = Int64.Parse(folPaso.ToString())
                            llaveCfd.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                        Else
                            llaveCfd.folio_factura = folPaso
                        End If

                    End If

                    Dim qList1 = From xe In xmlElm.Descendants _
                    Select xe
                    For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "Emisor"
                        llaveCfd.rfc_emisor = CType(xe.Attribute("Rfc"), String)
                        Exit For
                    Next
                    For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "Receptor"
                        receptor.rfc = CType(xe.Attribute("Rfc"), String)
                        Exit For
                    Next
                    'For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "DomicilioFiscal"
                    '    nomEstadoEmisor = CType(xe.Attribute("estado"), String)
                    '    cpEmisor = CType(xe.Attribute("codigoPostal"), String)
                    '    Exit For
                    'Next
                    'For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "Domicilio"
                    '    nomEstadoReceptor = CType(xe.Attribute("estado"), String)
                    '    cpReceptor = CType(xe.Attribute("codigoPostal"), String)
                    '    Exit For
                    'Next

                    'FGV 27012017
                    'se agrego la validacion para que el nombre del estado no sea ninguno relacionado con el distrito federal

                    If cpEmisor = "" Then
                        cpEmisor = "01"
                        nomEstadoEmisor = "CDMX"
                    Else
                        cpEmisor = cpEmisor.Substring(0, 2)
                    End If
                    If cpReceptor = "" Then
                        cpReceptor = "01"
                        nomEstadoReceptor = "CDMX"
                    Else
                        cpReceptor = cpReceptor.Substring(0, 2)
                    End If

                    'validacion para el emisor
                    Dim sqlAdapterCPE = New SqlDataAdapter("sp_excepcionCpEstado", Conexion)
                    'Dim nomEstado As String
                    'nomEstado = nomEstado
                    sqlAdapterCPE.SelectCommand.CommandType = CommandType.StoredProcedure
                    sqlAdapterCPE.SelectCommand.CommandTimeout = _timeout
                    sqlAdapterCPE.SelectCommand.Parameters.AddWithValue("@cp", cpEmisor)
                    sqlAdapterCPE.SelectCommand.Parameters.AddWithValue("@opc", "1")
                    Try
                        Conexion.Open()
                        sqlAdapterCPE.Fill(dsCPEmisor, "sp_excepcionCpEstado")
                        sqlAdapterCPE.Dispose()
                        ValidacionCPEmisor = dsCPEmisor.Tables.Item(0)
                        Mensaje = CType(ValidacionCPEmisor.Rows(0).Item("sw_valida"), String)
                        If Mensaje = 1 Then

                            Dim sqlAdapterEE = New SqlDataAdapter("sp_excepcionNomEstado", Conexion)
                            sqlAdapterEE.SelectCommand.CommandType = CommandType.StoredProcedure
                            sqlAdapterEE.SelectCommand.CommandTimeout = _timeout
                            sqlAdapterEE.SelectCommand.Parameters.AddWithValue("@nomEstado", nomEstadoEmisor.ToUpper())
                            sqlAdapterEE.SelectCommand.Parameters.AddWithValue("@opc", "1")
                            Try
                                'Conexion.Open()
                                sqlAdapterEE.Fill(dsNEEmisor, "sp_excepcionNomEstado")
                                sqlAdapterEE.Dispose()
                                ValidacionNEEmisor = dsNEEmisor.Tables.Item(0)
                                Mensaje = CType(ValidacionNEEmisor.Rows(0).Item("sw_valida"), String)
                                If Mensaje = 0 Then
                                    agrega_err(1, "Para este codigo postal, el nombre del estado del emisor no aceptado por Skytex <br />", errores)
                                End If
                                Conexion.Close()
                                dsCPReceptor.Reset()
                                ValidacionNEEmisor.Rows.Clear()
                            Catch ex As Exception
                                Dim msg As String
                                msg = "sp_excepcionNomEstado"
                                iErrorG = 3
                                agrega_err(iErrorG, msg, errores)
                            Finally
                                If Conexion.State = ConnectionState.Open Then
                                    Conexion.Close()
                                End If
                            End Try
                            'agrega_err(1, "El nombre del estado no aceptado por Skytex <br />", errores)
                        End If
                        Conexion.Close()
                        dsNE.Reset()
                        ValidacionCPEmisor.Rows.Clear()
                    Catch ex As Exception
                        Dim msg As String
                        msg = "sp_excepcionNomEstado"
                        iErrorG = 3
                        agrega_err(iErrorG, msg, errores)
                    Finally
                        If Conexion.State = ConnectionState.Open Then
                            Conexion.Close()
                        End If
                    End Try

                    'validacion para el receptor
                    Dim sqlAdapterCPR = New SqlDataAdapter("sp_excepcionCpEstado", Conexion)
                    'Dim nomEstado As String
                    'nomEstado = nomEstado
                    sqlAdapterCPR.SelectCommand.CommandType = CommandType.StoredProcedure
                    sqlAdapterCPR.SelectCommand.CommandTimeout = _timeout
                    sqlAdapterCPR.SelectCommand.Parameters.AddWithValue("@cp", cpReceptor)
                    sqlAdapterCPR.SelectCommand.Parameters.AddWithValue("@opc", "1")
                    Try
                        Conexion.Open()
                        sqlAdapterCPR.Fill(dsCPReceptor, "sp_excepcionCpEstado")
                        sqlAdapterCPR.Dispose()
                        ValidacionCPReceptor = dsCPReceptor.Tables.Item(0)
                        Mensaje = CType(ValidacionCPReceptor.Rows(0).Item("sw_valida"), String)
                        If Mensaje = 1 Then
                            If Not IsNothing(nomEstadoReceptor) Then 'si el nomEstadoReceptor tiene valor 
                                Dim sqlAdapterER = New SqlDataAdapter("sp_excepcionNomEstado", Conexion)
                                sqlAdapterER.SelectCommand.CommandType = CommandType.StoredProcedure
                                sqlAdapterER.SelectCommand.CommandTimeout = _timeout
                                sqlAdapterER.SelectCommand.Parameters.AddWithValue("@nomEstado", nomEstadoReceptor.ToUpper())
                                sqlAdapterER.SelectCommand.Parameters.AddWithValue("@opc", "1")
                                Try
                                    'Conexion.Open()
                                    sqlAdapterER.Fill(dsNEReceptor, "sp_excepcionNomEstado")
                                    sqlAdapterER.Dispose()
                                    ValidacionNEReceptor = dsNEReceptor.Tables.Item(0)
                                    Mensaje = CType(ValidacionNEReceptor.Rows(0).Item("sw_valida"), String)
                                    If Mensaje = 0 Then
                                        agrega_err(1, "Para este codigo postal, el nombre del estado del receptor no aceptado por Skytex <br />", errores)
                                    End If
                                    Conexion.Close()
                                    dsCPReceptor.Reset()
                                    ValidacionNEReceptor.Rows.Clear()
                                Catch ex As Exception
                                    Dim msg As String
                                    msg = "sp_excepcionNomEstado"
                                    iErrorG = 3
                                    agrega_err(iErrorG, msg, errores)
                                Finally
                                    If Conexion.State = ConnectionState.Open Then
                                        Conexion.Close()
                                    End If
                                End Try
                            End If
                            'agrega_err(1, "El nombre del estado no aceptado por Skytex <br />", errores)
                        End If
                        Conexion.Close()
                        dsNE.Reset()
                        ValidacionCPReceptor.Rows.Clear()
                    Catch ex As Exception
                        Dim msg As String
                        msg = "sp_excepcionNomEstado"
                        iErrorG = 3
                        agrega_err(iErrorG, msg, errores)
                    Finally
                        If Conexion.State = ConnectionState.Open Then
                            Conexion.Close()
                        End If
                    End Try

                    ''FGV 31/01/2017
                    '' si el codigo postal es igual a 11510 se valida el nombre del estado del receptor (Skytex)
                    'If cpReceptor = "11510" Then
                    '    Dim sqlAdapterCPR = New SqlDataAdapter("sp_excepcionNomEstado", Conexion)
                    '    sqlAdapterCPR.SelectCommand.CommandType = CommandType.StoredProcedure
                    '    sqlAdapterCPR.SelectCommand.CommandTimeout = _timeout
                    '    sqlAdapterCPR.SelectCommand.Parameters.AddWithValue("@nomEstado", nomEstadoReceptor.ToUpper())
                    '    sqlAdapterCPR.SelectCommand.Parameters.AddWithValue("@opc", "1")
                    '    Try
                    '        Conexion.Open()
                    '        sqlAdapterCPR.Fill(dsCPReceptor, "sp_excepcionNomEstado")
                    '        sqlAdapterCPR.Dispose()
                    '        ValidacionCPReceptor = dsCPReceptor.Tables.Item(0)
                    '        Mensaje = CType(ValidacionCPReceptor.Rows(0).Item("sw_valida"), String)
                    '        If Mensaje = 1 Then
                    '            agrega_err(1, "Para este codigo postal, el nombre del estado no aceptado por Skytex <br />", errores)
                    '        End If
                    '        Conexion.Close()
                    '        dsCPReceptor.Reset()
                    '        ValidacionCPReceptor.Rows.Clear()
                    '    Catch ex As Exception
                    '        Dim msg As String
                    '        msg = "sp_excepcionNomEstado"
                    '        iErrorG = 3
                    '        agrega_err(iErrorG, msg, errores)
                    '    Finally
                    '        If Conexion.State = ConnectionState.Open Then
                    '            Conexion.Close()
                    '        End If
                    '    End Try
                    'End If



                    'FGV 14012016
                    'se agrego la excepcion para que con algunos rfc's no aplicara la validacion del numCtaPago
                    Dim sqlAdapter = New SqlDataAdapter("sp_excepcionNumCtaPago", Conexion)
                    sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
                    sqlAdapter.SelectCommand.CommandTimeout = _timeout
                    sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
                    Try
                        Conexion.Open()
                        sqlAdapter.Fill(ds, "sp_excepcionNumCtaPago")
                        sqlAdapter.Dispose()
                        ValidacionEncabezado = ds.Tables.Item(0)
                        Mensaje = CType(ValidacionEncabezado.Rows(0).Item("sw_valida"), String)
                        If Mensaje = 0 Then
                            'GCM 18082016 no permitir nodos NumCtaPago
                            'GCM 02092016 Se comenta por instrucciones del Ing Hindi Correo NumCtaPago
                            If Not IsNothing(root.Attribute("NumCtaPago")) Then 'si NumCtaPago tiene un valor manda el error
                                If (root.Attribute("NumCtaPago").Value.ToString().Equals("0000") = False) Then 'si numCatPago es diferente a ""(espacio en blanco) manda un error
                                    agrega_err(1, "Atributo NumCtaPago no aceptado por Skytex <br />", errores)
                                End If
                            End If
                        End If
                        Conexion.Close()
                        ds.Reset()
                        ValidacionEncabezado.Rows.Clear()
                    Catch ex As Exception
                        Dim msg As String
                        msg = "sp_excepcionNumCtaPago"
                        iErrorG = 3
                        agrega_err(iErrorG, msg, errores)
                    Finally
                        If Conexion.State = ConnectionState.Open Then
                            Conexion.Close()
                        End If
                    End Try

                    If llaveCfd.rfc_emisor = "" Then
                        agrega_err(1, "No puede leerse el dato rfc del Emisor", errores)
                    End If
                    If receptor.rfc = "" Then
                        agrega_err(1, "No puede leerse el dato rfc del Receptor", errores)
                    End If

                Else
                    'version 3.2
                    If IsNothing(root.Attribute("sello")) Then
                        agrega_err(1, "No puede leerse el dato sello", errores)
                    Else
                        sello = root.Attribute("sello").Value.ToString()
                    End If
                    If llaveCfd.version_nom = "CFDI" Then
                        Dim qList = From xe In xmlElm.Descendants _
                                    Select xe
                        For Each xe In qList
                            If xe.Name.LocalName = "TimbreFiscalDigital" Then
                                timbre.version = xe.Attribute("version").Value
                                timbre.uuid = xe.Attribute("UUID").Value
                                timbre.fecha_timbrado = xe.Attribute("FechaTimbrado").Value
                                timbre.sello_cfd = xe.Attribute("selloCFD").Value
                                timbre.no_certificado_sat = xe.Attribute("noCertificadoSAT").Value
                                timbre.sello_sat = xe.Attribute("selloSAT").Value
                                contador = 1
                                Exit For
                            End If
                        Next
                        llaveCfd.timbre_fiscal = timbre
                        'GCM 22102014 se agrego el error sin timbre fiscal
                        If contador = 0 Then
                            agrega_err(1, "sin timbre fiscal", errores)
                        End If

                        If sello <> llaveCfd.timbre_fiscal.sello_cfd And contador = 1 Then
                            agrega_err(1, "el sello es diferente al del timbre fiscal", errores)
                        End If
                    Else
                        timbre.uuid = ""
                        llaveCfd.timbre_fiscal = timbre
                    End If
                    If IsNothing(root.Attribute("serie")) Then
                        llaveCfd.serie = ""
                    Else
                        llaveCfd.serie = root.Attribute("serie").Value.ToString()
                    End If
                    If IsNothing(root.Attribute("folio")) Then
                        llaveCfd.folio_factura = 0
                    Else
                        'GCM 17102014 Limpia el folio

                        Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("folio").Value, "[^0-9]", "")) 'Int32.Parse(root.Attribute("folio").Value)

                        If folPaso >= 2147483647 Then
                            'llaveCfd.folio_factura = Int64.Parse(folPaso.ToString())
                            llaveCfd.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                        Else
                            llaveCfd.folio_factura = folPaso
                        End If

                    End If

                    Dim qList1 = From xe In xmlElm.Descendants _
                    Select xe
                    For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "Emisor"
                        llaveCfd.rfc_emisor = CType(xe.Attribute("rfc"), String)
                        Exit For
                    Next
                    For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "Receptor"
                        receptor.rfc = CType(xe.Attribute("rfc"), String)
                        Exit For
                    Next
                    For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "DomicilioFiscal"
                        nomEstadoEmisor = CType(xe.Attribute("estado"), String)
                        cpEmisor = CType(xe.Attribute("codigoPostal"), String)
                        Exit For
                    Next
                    For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "Domicilio"
                        nomEstadoReceptor = CType(xe.Attribute("estado"), String)
                        cpReceptor = CType(xe.Attribute("codigoPostal"), String)
                        Exit For
                    Next

                    'FGV 27012017
                    'se agrego la validacion para que el nombre del estado no sea ninguno relacionado con el distrito federal

                    If cpEmisor = "" Then

                    Else
                        cpEmisor = cpEmisor.Substring(0, 2)
                    End If
                    If cpReceptor = "" Then

                    Else
                        cpReceptor = cpReceptor.Substring(0, 2)
                    End If

                    If cpEmisor = "" Then
                        cpEmisor = "01"
                        nomEstadoEmisor = "CDMX"
                    End If
                    'validacion para el emisor
                    Dim sqlAdapterCPE = New SqlDataAdapter("sp_excepcionCpEstado", Conexion)
                    'Dim nomEstado As String
                    'nomEstado = nomEstado
                    sqlAdapterCPE.SelectCommand.CommandType = CommandType.StoredProcedure
                    sqlAdapterCPE.SelectCommand.CommandTimeout = _timeout
                    sqlAdapterCPE.SelectCommand.Parameters.AddWithValue("@cp", cpEmisor)
                    sqlAdapterCPE.SelectCommand.Parameters.AddWithValue("@opc", "1")
                    Try
                        Conexion.Open()
                        sqlAdapterCPE.Fill(dsCPEmisor, "sp_excepcionCpEstado")
                        sqlAdapterCPE.Dispose()
                        ValidacionCPEmisor = dsCPEmisor.Tables.Item(0)
                        Mensaje = CType(ValidacionCPEmisor.Rows(0).Item("sw_valida"), String)
                        If Mensaje = 1 Then

                            Dim sqlAdapterEE = New SqlDataAdapter("sp_excepcionNomEstado", Conexion)
                            sqlAdapterEE.SelectCommand.CommandType = CommandType.StoredProcedure
                            sqlAdapterEE.SelectCommand.CommandTimeout = _timeout
                            sqlAdapterEE.SelectCommand.Parameters.AddWithValue("@nomEstado", nomEstadoEmisor.ToUpper())
                            sqlAdapterEE.SelectCommand.Parameters.AddWithValue("@opc", "1")
                            Try
                                'Conexion.Open()
                                sqlAdapterEE.Fill(dsNEEmisor, "sp_excepcionNomEstado")
                                sqlAdapterEE.Dispose()
                                ValidacionNEEmisor = dsNEEmisor.Tables.Item(0)
                                Mensaje = CType(ValidacionNEEmisor.Rows(0).Item("sw_valida"), String)
                                If Mensaje = 0 Then
                                    agrega_err(1, "Para este codigo postal, el nombre del estado del emisor no aceptado por Skytex <br />", errores)
                                End If
                                Conexion.Close()
                                dsCPReceptor.Reset()
                                ValidacionNEEmisor.Rows.Clear()
                            Catch ex As Exception
                                Dim msg As String
                                msg = "sp_excepcionNomEstado"
                                iErrorG = 3
                                agrega_err(iErrorG, msg, errores)
                            Finally
                                If Conexion.State = ConnectionState.Open Then
                                    Conexion.Close()
                                End If
                            End Try
                            'agrega_err(1, "El nombre del estado no aceptado por Skytex <br />", errores)
                        End If
                        Conexion.Close()
                        dsNE.Reset()
                        ValidacionCPEmisor.Rows.Clear()
                    Catch ex As Exception
                        Dim msg As String
                        msg = "sp_excepcionNomEstado"
                        iErrorG = 3
                        agrega_err(iErrorG, msg, errores)
                    Finally
                        If Conexion.State = ConnectionState.Open Then
                            Conexion.Close()
                        End If
                    End Try

                    If cpReceptor = "" Then
                        cpReceptor = "01"
                        nomEstadoReceptor = "CDMX"
                    End If

                    'validacion para el receptor
                    Dim sqlAdapterCPR = New SqlDataAdapter("sp_excepcionCpEstado", Conexion)
                    'Dim nomEstado As String
                    'nomEstado = nomEstado
                    sqlAdapterCPR.SelectCommand.CommandType = CommandType.StoredProcedure
                    sqlAdapterCPR.SelectCommand.CommandTimeout = _timeout
                    sqlAdapterCPR.SelectCommand.Parameters.AddWithValue("@cp", cpReceptor)
                    sqlAdapterCPR.SelectCommand.Parameters.AddWithValue("@opc", "1")
                    Try
                        Conexion.Open()
                        sqlAdapterCPR.Fill(dsCPReceptor, "sp_excepcionCpEstado")
                        sqlAdapterCPR.Dispose()
                        ValidacionCPReceptor = dsCPReceptor.Tables.Item(0)
                        Mensaje = CType(ValidacionCPReceptor.Rows(0).Item("sw_valida"), String)
                        If Mensaje = 1 Then
                            If Not IsNothing(nomEstadoReceptor) Then 'si el nomEstadoReceptor tiene valor 
                                Dim sqlAdapterER = New SqlDataAdapter("sp_excepcionNomEstado", Conexion)
                                sqlAdapterER.SelectCommand.CommandType = CommandType.StoredProcedure
                                sqlAdapterER.SelectCommand.CommandTimeout = _timeout
                                sqlAdapterER.SelectCommand.Parameters.AddWithValue("@nomEstado", nomEstadoReceptor.ToUpper())
                                sqlAdapterER.SelectCommand.Parameters.AddWithValue("@opc", "1")
                                Try
                                    'Conexion.Open()
                                    sqlAdapterER.Fill(dsNEReceptor, "sp_excepcionNomEstado")
                                    sqlAdapterER.Dispose()
                                    ValidacionNEReceptor = dsNEReceptor.Tables.Item(0)
                                    Mensaje = CType(ValidacionNEReceptor.Rows(0).Item("sw_valida"), String)
                                    If Mensaje = 0 Then
                                        agrega_err(1, "Para este codigo postal, el nombre del estado del receptor no aceptado por Skytex <br />", errores)
                                    End If
                                    Conexion.Close()
                                    dsCPReceptor.Reset()
                                    ValidacionNEReceptor.Rows.Clear()
                                Catch ex As Exception
                                    Dim msg As String
                                    msg = "sp_excepcionNomEstado"
                                    iErrorG = 3
                                    agrega_err(iErrorG, msg, errores)
                                Finally
                                    If Conexion.State = ConnectionState.Open Then
                                        Conexion.Close()
                                    End If
                                End Try
                            End If
                            'agrega_err(1, "El nombre del estado no aceptado por Skytex <br />", errores)
                        End If
                        Conexion.Close()
                        dsNE.Reset()
                        ValidacionCPReceptor.Rows.Clear()
                    Catch ex As Exception
                        Dim msg As String
                        msg = "sp_excepcionNomEstado"
                        iErrorG = 3
                        agrega_err(iErrorG, msg, errores)
                    Finally
                        If Conexion.State = ConnectionState.Open Then
                            Conexion.Close()
                        End If
                    End Try

                    ''FGV 31/01/2017
                    '' si el codigo postal es igual a 11510 se valida el nombre del estado del receptor (Skytex)
                    'If cpReceptor = "11510" Then
                    '    Dim sqlAdapterCPR = New SqlDataAdapter("sp_excepcionNomEstado", Conexion)
                    '    sqlAdapterCPR.SelectCommand.CommandType = CommandType.StoredProcedure
                    '    sqlAdapterCPR.SelectCommand.CommandTimeout = _timeout
                    '    sqlAdapterCPR.SelectCommand.Parameters.AddWithValue("@nomEstado", nomEstadoReceptor.ToUpper())
                    '    sqlAdapterCPR.SelectCommand.Parameters.AddWithValue("@opc", "1")
                    '    Try
                    '        Conexion.Open()
                    '        sqlAdapterCPR.Fill(dsCPReceptor, "sp_excepcionNomEstado")
                    '        sqlAdapterCPR.Dispose()
                    '        ValidacionCPReceptor = dsCPReceptor.Tables.Item(0)
                    '        Mensaje = CType(ValidacionCPReceptor.Rows(0).Item("sw_valida"), String)
                    '        If Mensaje = 1 Then
                    '            agrega_err(1, "Para este codigo postal, el nombre del estado no aceptado por Skytex <br />", errores)
                    '        End If
                    '        Conexion.Close()
                    '        dsCPReceptor.Reset()
                    '        ValidacionCPReceptor.Rows.Clear()
                    '    Catch ex As Exception
                    '        Dim msg As String
                    '        msg = "sp_excepcionNomEstado"
                    '        iErrorG = 3
                    '        agrega_err(iErrorG, msg, errores)
                    '    Finally
                    '        If Conexion.State = ConnectionState.Open Then
                    '            Conexion.Close()
                    '        End If
                    '    End Try
                    'End If



                    'FGV 14012016
                    'se agrego la excepcion para que con algunos rfc's no aplicara la validacion del numCtaPago
                    Dim sqlAdapter = New SqlDataAdapter("sp_excepcionNumCtaPago", Conexion)
                    sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
                    sqlAdapter.SelectCommand.CommandTimeout = _timeout
                    sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
                    Try
                        Conexion.Open()
                        sqlAdapter.Fill(ds, "sp_excepcionNumCtaPago")
                        sqlAdapter.Dispose()
                        ValidacionEncabezado = ds.Tables.Item(0)
                        Mensaje = CType(ValidacionEncabezado.Rows(0).Item("sw_valida"), String)
                        If Mensaje = 0 Then
                            'GCM 18082016 no permitir nodos NumCtaPago
                            'GCM 02092016 Se comenta por instrucciones del Ing Hindi Correo NumCtaPago
                            If Not IsNothing(root.Attribute("NumCtaPago")) Then 'si NumCtaPago tiene un valor manda el error
                                If (root.Attribute("NumCtaPago").Value.ToString().Equals("0000") = False) Then 'si numCatPago es diferente a ""(espacio en blanco) manda un error
                                    agrega_err(1, "Atributo NumCtaPago no aceptado por Skytex <br />", errores)
                                End If
                            End If
                        End If
                        Conexion.Close()
                        ds.Reset()
                        ValidacionEncabezado.Rows.Clear()
                    Catch ex As Exception
                        Dim msg As String
                        msg = "sp_excepcionNumCtaPago"
                        iErrorG = 3
                        agrega_err(iErrorG, msg, errores)
                    Finally
                        If Conexion.State = ConnectionState.Open Then
                            Conexion.Close()
                        End If
                    End Try

                    If llaveCfd.rfc_emisor = "" Then
                        agrega_err(1, "No puede leerse el dato rfc del Emisor", errores)
                    End If
                    If receptor.rfc = "" Then
                        agrega_err(1, "No puede leerse el dato rfc del Receptor", errores)
                    End If
                End If
            Catch ex As Exception
                agrega_err(1, ex.Message, errores)
            End Try

        End Sub

        Public Function ExecuteScalar() As Object

        End Function

        Public Sub LeeDatosFacturaLINQ_SNAdd(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal xmlDocFilePath As String, ByVal llaveCfd As llave_cfd)

            'GCM 17122014 paraobtener el IVA
            Dim cmd As New SqlCommand
            Dim pctIva As Object
            cmd.CommandText = "select pct_iva from iciva where iva_cve = 1"
            cmd.CommandType = CommandType.Text
            cmd.Connection = Conexion
            Conexion.Open()
            pctIva = cmd.ExecuteScalar()
            Conexion.Close()
            comprobante.pctIva = pctIva

            Dim xmlElm As XElement
            Try
                xmlElm = XElement.Load(xmlDocFilePath)
                Dim root As XElement = XElement.Load(xmlDocFilePath)
                Dim qAtrib As IEnumerable(Of XAttribute) = _
                    From atr In root.Attributes() _
                    Select atr

                'GCM 17122014 
                Dim qList1 = From xe In xmlElm.Descendants _
                Select xe
                ''GCM 17122014 

                'esto es para remover la addenda
                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Addenda" Then
                        a.Remove()
                    End If
                Next

                'GCM 17122014 obtenemos el valor de total de traslados
                For Each xe In qList1
                    If xe.Name.LocalName = "ImpuestosLocales" Then
                        comprobante.TotaldeTraslados = Decimal.Parse(xe.Attribute("TotaldeTraslados").Value)
                        comprobante.TotaldeRetenciones = Decimal.Parse(xe.Attribute("TotaldeRetenciones").Value) 'GCM 11112014 obtenemos el valor de total de retenciones
                    End If
                Next
                'GCM 17122014 

                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Complemento" Then
                        a.Remove()
                    End If
                Next


                If llaveCfd.version_nom <> "" Then
                    comprobante.version = llaveCfd.version
                End If
                If IsNothing(root.Attribute("serie")) Then
                    comprobante.serie = ""
                Else
                    comprobante.serie = root.Attribute("serie").Value.ToString()
                End If
                'folio 
                If IsNothing(root.Attribute("folio")) Then
                    comprobante.folio_factura = 0
                Else
                    'comprobante.folio_factura = Int64.Parse(root.Attribute("folio").Value)
                    Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("folio").Value, "[^0-9]", ""))  'Int32.Parse(root.Attribute("folio").Value)
                    If folPaso >= 2147483647 Then
                        'comprobante.folio_factura = Int64.Parse(folPaso.ToString())
                        comprobante.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                    Else
                        comprobante.folio_factura = folPaso
                    End If
                End If
                'fecha
                If IsNothing(root.Attribute("fecha")) Then
                    agrega_err(1, "No puede leerse el dato fecha", errores)
                Else
                    comprobante.fecha_factura = root.Attribute("fecha").Value.ToString()
                End If
                'fecha
                If IsNothing(root.Attribute("sello")) Then
                    agrega_err(1, "No puede leerse el dato sello", errores)
                Else
                    comprobante.sello = root.Attribute("sello").Value.ToString()
                End If
                If llaveCfd.version_nom = "CFD" Then
                    ' numero de aprobacion
                    If IsNothing(root.Attribute("noAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato noAprobacion", errores)
                    Else
                        comprobante.no_aprobacion = Integer.Parse(root.Attribute("noAprobacion").Value)
                    End If
                    ' año de aprobacion
                    If IsNothing(root.Attribute("anoAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato anoAprobacion", errores)
                    Else
                        comprobante.ano_aprobacion = Integer.Parse(root.Attribute("anoAprobacion").Value)
                    End If
                End If
                If IsNothing(root.Attribute("noCertificado")) Then
                    agrega_err(1, "No puede leerse el dato noCertificado", errores)
                Else
                    comprobante.no_certificado = root.Attribute("noCertificado").Value.ToString()
                End If
                If Not IsNothing(root.Attribute("certificado")) Then
                    comprobante.certificado = root.Attribute("certificado").Value.ToString()
                Else
                    comprobante.certificado = ""
                End If
                ' subtotal
                If IsNothing(root.Attribute("subTotal")) Then
                    agrega_err(1, "No puede leerse el dato subTotal", errores)
                Else
                    comprobante.sub_total = Decimal.Parse(root.Attribute("subTotal").Value)
                End If
                ' descuento
                If IsNothing(root.Attribute("descuento")) Then
                    'agrega_err(1, "No puede leerse el dato descuento", errores)
                    comprobante.descuento = 0 'el descuento se manda en 0 en caso de no haber atributo 14/08/2013
                Else
                    comprobante.descuento = Decimal.Parse(root.Attribute("descuento").Value)
                End If
                ' total
                If IsNothing(root.Attribute("total")) Then
                    agrega_err(1, "No puede leerse el dato total", errores)
                Else
                    comprobante.total = Decimal.Parse(root.Attribute("total").Value)
                End If
                ' tipo de comprobante
                If IsNothing(root.Attribute("tipoDeComprobante")) Then
                    agrega_err(1, "No puede leerse el dato tipoDeComprobante", errores)
                Else
                    Dim tipoDeComprobante = root.Attribute("tipoDeComprobante").Value.ToString()
                End If

                'GCM 05012015 Comentado el tipo de documento ya que no se capturara el tm
                'If comprobante.tipodoc_cve = "BTFSER" Then

                'GCM 07012015 Se agrego el TC
                If IsNothing(root.Attribute("TipoCambio")) Then
                    comprobante.tc = "0"
                Else
                    comprobante.tc = root.Attribute("TipoCambio").Value.ToString
                End If

                'GCM 06012015 se contempla el tc
                If llaveCfd.version = "3.3" Then
                    'Cambio para CDFI 3.3 FGV (12/08/2017)
                    'se va a enviar el id del catalogo de las Monedas
                    comprobante.moneda = root.Attribute("Moneda").Value.ToString()
                Else
                    If IsNothing(root.Attribute("Moneda")) Then
                        If IsNothing(root.Attribute("TipoCambio")) Then
                            comprobante.moneda = "MXN"
                        Else
                            If (comprobante.tc Like "1.00*" Or comprobante.tc = "1") Then
                                comprobante.moneda = "MXN"
                            Else
                                comprobante.moneda = ""
                            End If
                        End If
                    Else
                        'GCM 07012015 contemplamos $ 13012015 contemplamos Ninguno
                        'GCM 11092015 Agregamos MONEDA NACIONAL sin contemplar el TC
                        If (root.Attribute("Moneda").Value.ToString() = "$" Or
                            root.Attribute("Moneda").Value.ToString() = "Ninguno") And
                        (comprobante.tc = "1.00" Or comprobante.tc = "1") Then
                            comprobante.moneda = "MXN"
                        Else
                            'FGV 09/11/2016 se agrego la cadena "Modena Nacional" y "moneda nacional" ya que al no encontrarse la cadena "MONEDA NACIONAL"(en mayusculas) se le asignaba el valor del campo moneda, al momento de insertarlo se enviaba la cadena completa y no la cadena "MXN" 
                            If (root.Attribute("Moneda").Value.ToString().ToUpper() = "MONEDA NACIONAL") Then
                                comprobante.moneda = "MXN"
                            Else
                                If (root.Attribute("Moneda").Value.ToString() = "Pesos") Then
                                    comprobante.moneda = "PES"
                                Else
                                    comprobante.moneda = root.Attribute("Moneda").Value.ToString()
                                End If
                            End If
                        End If
                    End If
                End If



                'If IsNothing(root.Attribute("Moneda")) Then
                '    comprobante.moneda = ""
                'Else
                '    comprobante.moneda = root.Attribute("Moneda").Value.ToString()
                'End If


                Dim qList = From xe In xmlElm.Descendants _
                Select xe
                Dim emisor = New emisor
                Dim receptor = New receptor
                Dim conce As New List(Of concepto)
                Dim impuestos = New impuestos
                Dim traslados = New List(Of traslado)
                Dim retenciones = New List(Of retencion)
                Dim addenda = New addenda
                Dim requesForPayment = New requestforpayment
                Dim currency = New currency
                Dim aditionalData = New aditionaldata
                Dim paymentTimePeriod = New paymenttimeperiod
                Dim provider = New provider
                Dim lineItems As New List(Of lineitem)
                Const swCargosConcep As Boolean = False
                Dim errorConceptos As Boolean = False
                impuestos.total_imp_trasl = 0
                impuestos.sw_retencion = 0
                impuestos.total_imp_reten = 0
                Dim swTotalCapT As Boolean = False
                Dim swTotalCapR As Boolean = False
                Dim Tot_ImpuestosTras As Decimal = 0
                comprobante.swImpTras = True
                comprobante.swTImporte = False

                For Each xe In qList


                    If xe.Name.LocalName = "Emisor" Then
                        emisor.rfc = xe.Attribute("rfc").Value.ToString()
                    End If
                    'GCM 12022015
                    If emisor.rfc = "TME970922LV3" And comprobante.moneda = "P-US$" Then
                        comprobante.moneda = "DOL"
                    End If

                    If emisor.rfc = "CEM900327M82" And (comprobante.moneda = "D&amp;oacute;lar estadounidense" Or
                                                        comprobante.moneda = "D&oacute;lar estadounidense") Then
                        comprobante.moneda = "DOL"
                    End If

                    If xe.Name.LocalName = "Receptor" Then
                        receptor.rfc = xe.Attribute("rfc").Value.ToString()
                    End If
                    'GCM 17122014
                    If xe.Name.LocalName = "Impuestos" Then
                        If IsNothing(xe.Attribute("totalImpuestosTrasladados")) Then
                            comprobante.totalImpuestosTrasladados = 0
                            comprobante.swImpTras = False
                        Else
                            comprobante.totalImpuestosTrasladados = xe.Attribute("totalImpuestosTrasladados").Value.ToString()
                        End If
                    End If
                    'GCM 17122014
                    If xe.Name.LocalName = "Concepto" And errorConceptos = False Then
                        Dim itemComceptos = New concepto
                        itemComceptos.cantidad = CType(xe.Attribute("cantidad"), Decimal)
                        itemComceptos.valor_unitario = CType(xe.Attribute("valorUnitario"), Decimal)
                        itemComceptos.importe = CType(xe.Attribute("importe"), Decimal)
                        conce.Add(itemComceptos)
                        If itemComceptos.cantidad = 0 And itemComceptos.valor_unitario > 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False 'no aplica ya que hay facturas como esta que no maneja cantidad.
                            Else
                                agrega_err(1, "Si la cantidad es 0, no debe agregar valor unitario", errores)
                                errorConceptos = True
                            End If
                        End If
                        If swCargosConcep = True And errorConceptos = False Then
                            If itemComceptos.cantidad <> 1 Then
                                agrega_err(1, "La cantidad siempre debe ser 1 en el cargo extra", errores)
                                errorConceptos = True
                            End If
                        End If
                    End If

                    If xe.Name.LocalName = "Retencion" Then ' And swTotalCapR = False Then
                        If Not IsNothing(xe.Attribute("importe")) Then
                            impuestos.total_imp_reten = impuestos.total_imp_reten + CType(xe.Attribute("importe"), Decimal)
                            Dim itemRet = New retencion
                            itemRet.impuesto = CType(xe.Attribute("impuesto"), String)
                            itemRet.importe = CType(xe.Attribute("importe"), Decimal)
                            retenciones.Add(itemRet)
                            If impuestos.total_imp_reten > 0 Then
                                impuestos.sw_retencion = 1
                            End If

                        Else
                            impuestos.total_imp_trasl = 0
                        End If
                    End If



                    If xe.Name.LocalName = "Traslado" Then 'And  swTotalCapT = False Then

                        'GCM 22012015 Obtenemos prefijo, solo entrara si es cfdi
                        Dim prefijo = xe.GetPrefixOfNamespace(xe.Name.NamespaceName)

                        If prefijo = "cfdi" Then
                            If Not IsNothing(xe.Attribute("importe")) Then
                                impuestos.total_imp_trasl = impuestos.total_imp_trasl + CType(xe.Attribute("importe"), Decimal)
                                Dim itemTras = New traslado
                                itemTras.impuesto = CType(xe.Attribute("impuesto"), String)
                                'itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                                'itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                                If CType(xe.Attribute("tasa"), Decimal) > 0 And CType(xe.Attribute("importe"), Decimal) > 0 Then
                                    itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                                    itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                                Else
                                    itemTras.tasa = 0
                                    itemTras.importe = 0
                                End If
                                traslados.Add(itemTras)
                            End If
                        End If
                    End If


                Next
                If conce.Count = 0 Then
                    agrega_err(1, "El comprobante no tiene conceptos", errores)
                    errorConceptos = True
                End If
                aditionalData.text_data = "Z"
                provider.providerid = ""
                comprobante.Conceptos = conce
                requesForPayment.provider = provider
                requesForPayment.line_items = lineItems
                requesForPayment.currency = currency
                requesForPayment.aditional_data = aditionalData
                requesForPayment.paymenttimeperiod = paymentTimePeriod
                addenda.requestforpayment = requesForPayment
                comprobante.Emisor = emisor
                comprobante.Receptor = receptor
                impuestos.Traslados = traslados
                impuestos.Retenciones = retenciones
                comprobante.Impuestos = impuestos
                comprobante.Addenda = addenda

                Dim msgUsr = From msg In errores _
                    Select interror = msg.Interror, message = msg.Message _
                    Where (interror = 1)
                Dim er As New Errores
                Dim cadena As String = msgUsr.Aggregate("", Function(current, msgs) current & msgs.message.Trim + ", ")
                If cadena.Trim <> "" Then
                    er.Interror = 1
                    er.Message = cadena.Trim
                    graba_error(errores, er, llaveCfd, "60069", "LeeDatosFacturaLINQ_SinAddenda")
                End If
            Catch ex As Exception
                'agrega_err(1, ex.Message, errores)
                agrega_err(1, "Ocurrio un erro al leer el XML por favor contacte con el administrador de sitio ", errores, "60069")
            End Try

        End Sub

        Public Sub LeeDatosFacturaLinq(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal xmlDocFilePath As String, ByVal llaveCfd As llave_cfd, ByVal ccCve As String, ByVal ccTipo As String)
            Dim xmlElm As XElement
            Try
                xmlElm = XElement.Load(xmlDocFilePath)
                Dim root As XElement = XElement.Load(xmlDocFilePath)


                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Complemento" Then
                        a.Remove()
                    End If
                Next

                If llaveCfd.version_nom <> "" Then
                    comprobante.version = llaveCfd.version
                End If
                If IsNothing(root.Attribute("serie")) Then
                    comprobante.serie = ""
                Else
                    comprobante.serie = root.Attribute("serie").Value.ToString()
                End If
                'folio 
                If IsNothing(root.Attribute("folio")) Then
                    'agrega_err(1, "No puede leerse el dato folio", errores)
                    comprobante.folio_factura = 0
                Else
                    'comprobante.folio_factura = Int64.Parse(root.Attribute("folio").Value)
                    Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("folio").Value, "[^0-9]", ""))  'Int32.Parse(root.Attribute("folio").Value)
                    If folPaso >= 2147483647 Then
                        comprobante.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                    Else
                        comprobante.folio_factura = folPaso
                    End If
                End If
                'fecha
                If IsNothing(root.Attribute("fecha")) Then
                    agrega_err(1, "No puede leerse el dato fecha", errores)
                Else
                    comprobante.fecha_factura = root.Attribute("fecha").Value.ToString()
                End If
                'fecha
                If IsNothing(root.Attribute("sello")) Then
                    agrega_err(1, "No puede leerse el dato sello", errores)
                Else
                    comprobante.sello = root.Attribute("sello").Value.ToString()
                End If
                If llaveCfd.version_nom = "CFD" Then
                    ' numero de aprobacion
                    If IsNothing(root.Attribute("noAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato noAprobacion", errores)
                    Else
                        comprobante.no_aprobacion = Integer.Parse(root.Attribute("noAprobacion").Value)
                    End If
                    ' año de aprobacion
                    If IsNothing(root.Attribute("anoAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato anoAprobacion", errores)
                    Else
                        comprobante.ano_aprobacion = Integer.Parse(root.Attribute("anoAprobacion").Value)
                    End If
                End If
                ' numero de certificado
                If IsNothing(root.Attribute("noCertificado")) Then
                    agrega_err(1, "No puede leerse el dato noCertificado", errores)
                Else
                    comprobante.no_certificado = root.Attribute("noCertificado").Value.ToString()
                End If
                ' certificado
                If IsNothing(root.Attribute("certificado")) Then
                    agrega_err(1, "No puede leerse el dato certificado", errores)
                Else
                    comprobante.certificado = root.Attribute("certificado").Value.ToString()
                End If
                ' subtotal
                If IsNothing(root.Attribute("subTotal")) Then
                    agrega_err(1, "No puede leerse el dato subTotal", errores)
                Else
                    comprobante.sub_total = Decimal.Parse(root.Attribute("subTotal").Value)
                End If
                ' descuento
                If IsNothing(root.Attribute("descuento")) Then
                    comprobante.descuento = 0 'el descuento se manda en 0 en caso de no haber atributo 14/08/2013
                Else
                    comprobante.descuento = Decimal.Parse(root.Attribute("descuento").Value)
                End If
                ' total
                If IsNothing(root.Attribute("total")) Then
                    agrega_err(1, "No puede leerse el dato total", errores)
                Else
                    comprobante.total = Decimal.Parse(root.Attribute("total").Value)
                End If
                ' tipo de comprobante
                If IsNothing(root.Attribute("tipoDeComprobante")) Then
                    agrega_err(1, "No puede leerse el dato tipoDeComprobante", errores)
                Else
                    Dim tipoDeComprobante = root.Attribute("tipoDeComprobante").Value.ToString()
                End If


                comprobante.condiciones_pago = "0"
                Dim qList = From xe In xmlElm.Descendants _
                Select xe
                Dim emisor = New emisor
                Dim receptor = New receptor
                Dim conce As New List(Of concepto)
                Dim impuestos = New impuestos
                Dim traslados = New List(Of traslado)
                Dim retenciones = New List(Of retencion)
                Dim addenda = New addenda
                Dim requesForPayment = New requestforpayment
                Dim currency = New currency
                Dim aditionalData = New aditionaldata
                Dim paymentTimePeriod = New paymenttimeperiod
                Dim provider = New provider
                Dim lineItems As New List(Of lineitem)
                Dim renglon As Integer = 1
                Const swCargosConcep As Boolean = False
                Dim swCargos As Boolean = False
                Dim errorConceptos As Boolean = False
                Dim errorItms As Boolean = False
                impuestos.total_imp_trasl = 0
                impuestos.sw_retencion = 0
                impuestos.total_imp_reten = 0
                Dim swTotalCapT As Boolean = False
                Dim swTotalCapR As Boolean = False

                For Each xe In qList
                    If xe.Name.LocalName = "Emisor" Then
                        emisor.rfc = CType(xe.Attribute("rfc"), String)
                    End If
                    If xe.Name.LocalName = "Receptor" Then
                        receptor.rfc = CType(xe.Attribute("rfc"), String)
                    End If
                    If xe.Name.LocalName = "Concepto" And errorConceptos = False Then
                        Dim itemComceptos = New concepto
                        itemComceptos.cantidad = CType(xe.Attribute("cantidad"), Decimal)
                        itemComceptos.valor_unitario = CType(xe.Attribute("valorUnitario"), Decimal)
                        itemComceptos.importe = CType(xe.Attribute("importe"), Decimal)
                        conce.Add(itemComceptos)
                        'If itemComceptos.cantidad = 0 And itemComceptos.valor_unitario > 0 Then 'GCM 05082015 codigo original
                        'agrega_err(1, "Si la cantidad es 0, no debe agregar valor unitario", errores)
                        'errorConceptos = True
                        'End If
                        If itemComceptos.cantidad = 0 And itemComceptos.valor_unitario > 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False 'no aplica ya que hay facturas como esta que no maneja cantidad.
                            Else
                                agrega_err(1, "Si la cantidad es 0, no debe agregar valor unitario", errores)
                                errorConceptos = True
                            End If
                        End If

                        If swCargosConcep = True And errorConceptos = False Then
                            If itemComceptos.cantidad <> 1 Then
                                agrega_err(1, "La cantidad siempre debe ser 1 en el cargo extra", errores)
                                errorConceptos = True
                            End If
                        End If
                    End If

                    If xe.Name.LocalName = "Retencion" Then ' And swTotalCapR = False Then
                        If Not IsNothing(xe.Attribute("importe")) Then
                            impuestos.total_imp_reten = impuestos.total_imp_reten + CType(xe.Attribute("importe"), Decimal)
                            Dim itemRet = New retencion
                            itemRet.impuesto = CType(xe.Attribute("impuesto"), String)
                            itemRet.importe = CType(xe.Attribute("importe"), Decimal)
                            retenciones.Add(itemRet)
                            If impuestos.total_imp_reten > 0 Then
                                impuestos.sw_retencion = 1
                            End If

                        Else
                            impuestos.total_imp_trasl = 0
                        End If
                    End If
                    If xe.Name.LocalName = "Traslado" Then 'And  swTotalCapT = False Then
                        If Not IsNothing(xe.Attribute("importe")) Then
                            impuestos.total_imp_trasl = impuestos.total_imp_trasl + CType(xe.Attribute("importe"), Decimal)
                            Dim itemTras = New traslado
                            itemTras.impuesto = CType(xe.Attribute("impuesto"), String)
                            'itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                            'itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                            If CType(xe.Attribute("tasa"), Decimal) > 0 And CType(xe.Attribute("importe"), Decimal) > 0 Then
                                itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                                itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                            Else
                                itemTras.tasa = 0
                                itemTras.importe = 0
                            End If
                            traslados.Add(itemTras)
                        End If
                    End If


                    If xe.Name.LocalName = "AditionalData" Then
                        aditionalData.text_data = CType(xe.Attribute("textData"), String)
                        aditionalData.metododepago = CType(xe.Attribute("metodoDePago"), String)
                        aditionalData.moneda = CType(xe.Attribute("Moneda"), String)
                    End If
                    If xe.Name.LocalName = "Provider" Then
                        provider.providerid = CType(xe.Attribute("ProviderID"), String)
                        Const caracterSeparador As String = "@"
                        Dim testPos As Integer = InStr(provider.providerid, caracterSeparador)
                        If testPos <> 2 Then
                            agrega_err(1, "El proveedor no tiene el formato correcto", errores)
                        End If
                        If provider.providerid.Length <> 12 Then
                            agrega_err(1, "Longitud incorrecta en ProviderID", errores)
                        Else
                            Dim primer As Integer = provider.providerid.IndexOf("@", StringComparison.Ordinal)
                            comprobante.cc_tipo = provider.providerid.Substring(0, primer)
                            Dim segundo As Integer = InStr(3, provider.providerid, "@", CompareMethod.Text)
                            comprobante.cc_cve = provider.providerid.Substring(2, 6)
                            EfCveG = provider.providerid.Substring(segundo, 3)
                        End If
                        If comprobante.cc_cve <> ccCve Or comprobante.cc_tipo <> ccTipo Then
                            agrega_err(1, "El dato Provider es incorrecto no corresponde con la session", errores)
                        End If
                    End If
                    If xe.Name.LocalName = "lineItem" And errorItms = False Then
                        Dim item = New lineitem
                        item.type = CType(xe.Attribute("type").Value, Integer)
                        item.number = CType(xe.Attribute("number"), Integer)
                        item.monto_decuento = CType(xe.Attribute("montoDescuento"), Decimal)
                        item.pct_decuento = CType(xe.Attribute("pctDescuento"), Decimal)
                        item.uns = CType(xe.Attribute("uns"), Decimal)
                        item.precio = CType(xe.Attribute("precio"), Decimal)
                        item.sku = CType(xe.Attribute("sku"), String)
                        item.partida = CType(xe.Attribute("partida"), Integer)
                        item.reference_identification = CType(xe.Attribute("referenceIdentification"), String)
                        item.art_tip = CType(xe.Attribute("art_tip"), String)
                        item.uni_med = CType(xe.Attribute("uni_med"), String)
                        Dim primer As Integer = item.reference_identification.IndexOf("@", StringComparison.Ordinal)
                        If primer < 1 Then
                            agrega_err(1, "El formato del atributo referenceIdentification es incorrecto : " + item.reference_identification, errores)
                            errorItms = True
                        End If

                        lineItems.Add(item)

                        If item.number <> renglon Then
                            agrega_err(1, "Los renglones deben ser agregados en orden lineItem: sku: " + item.sku + "number: " + item.number.ToString(), errores)
                            errorItms = True
                        End If

                        If swCargos = True And item.type = 1 Then
                            agrega_err(1, "Los cargos deben agregarse después de todos los artículos lineItem: sku: " + item.sku, errores)
                            errorItms = True
                        End If
                        If item.type = 2 And swCargos = False Then
                            swCargos = True
                        End If

                        If item.uns = 0 And item.precio > 0 Then
                            agrega_err(1, "Si las uns son 0, no debe agregar precio", errores)
                            errorItms = True
                        End If

                        If item.uns > 0 And item.precio = 0 Then
                            agrega_err(1, "Si el precio es 0, no debe agregar uns", errores)
                            errorItms = True
                        End If

                        If swCargos = True Then
                            If item.monto_decuento > 0 Then
                                agrega_err(1, "Incorrecto aplicar descuento a cargo extra ", errores) ' + item.sku, errores)
                                errorItms = True
                            End If
                            If item.pct_decuento > 0 Then
                                agrega_err(1, "Los cargos no debe grabarse porcentaje de descuento ", errores) '+ item.sku, errores)
                                errorItms = True
                            End If
                            If item.partida > 0 Then
                                agrega_err(1, "El dato partida para los cargos extra debe ser 0", errores)
                                errorItms = True
                            End If
                            If item.uns <> 1 Then
                                agrega_err(1, "Las uns siempre debe ser 1 en el cargo extra", errores)
                                errorItms = True
                            End If

                            If item.uni_med.ToString().ToUpper <> "NO APLICA" Then
                                agrega_err(1, "La unidad de medida debe ser 'No aplica' en cargo extra", errores)
                                errorItms = True
                            End If

                        End If
                        renglon = renglon + 1

                    End If
                Next

                If conce.Count = 0 Then
                    agrega_err(1, "El comprobante no tiene conceptos", errores, "60069")
                    errorConceptos = True
                End If

                comprobante.Conceptos = conce
                requesForPayment.line_items = lineItems
                requesForPayment.currency = currency
                requesForPayment.aditional_data = aditionalData
                requesForPayment.paymenttimeperiod = paymentTimePeriod
                addenda.requestforpayment = requesForPayment
                comprobante.Emisor = emisor
                impuestos.Traslados = traslados
                impuestos.Retenciones = retenciones
                comprobante.Emisor = emisor
                comprobante.Receptor = receptor
                comprobante.Impuestos = impuestos
                comprobante.Addenda = addenda

                Dim msgUsr = From msg In errores _
                    Select interror = msg.Interror, message = msg.Message _
                    Where (interror = 1)

                Dim er As New Errores
                Dim cadena As String = msgUsr.Aggregate("", Function(current, msgs) current & msgs.message.Trim + ", ")

                If cadena.Trim <> "" Then
                    er.Interror = 1
                    er.Message = cadena.Trim
                    graba_error(errores, er, llaveCfd, "60069", "LeeDatosFacturaLINQ")
                End If

            Catch ex As Exception
                agrega_err(1, ex.Message, errores, "60069")
            End Try

        End Sub

        Public Sub LeeDatosFacturaLINQ_FacEmb(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal XMLDOCFILEPATH As String, ByVal LlaveCFD As llave_cfd, ByVal cc_cve As String, ByVal cc_tipo As String)

            Dim xmlElm As XElement

            Try

                xmlElm = XElement.Load(XMLDOCFILEPATH)
                Dim root As XElement = XElement.Load(XMLDOCFILEPATH)
                Dim qAtrib As IEnumerable(Of XAttribute) = _
                    From atr In root.Attributes() _
                    Select atr

                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Complemento" Then
                        a.Remove()
                    End If
                Next

                If LlaveCFD.version_nom <> "" Then
                    comprobante.version = LlaveCFD.version
                End If

                If IsNothing(root.Attribute("serie")) Then
                    comprobante.serie = ""
                Else
                    comprobante.serie = root.Attribute("serie").Value.ToString()
                End If
                'folio 
                If IsNothing(root.Attribute("folio")) Then
                    'agrega_err(1, "No puede leerse el dato folio", errores)
                    comprobante.folio_factura = 0
                Else
                    'comprobante.folio_factura = Int64.Parse(root.Attribute("folio").Value)
                    Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("folio").Value, "[^0-9]", ""))  'Int32.Parse(root.Attribute("folio").Value)
                    If folPaso >= 2147483647 Then
                        comprobante.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                    Else
                        comprobante.folio_factura = folPaso
                    End If
                End If
                'fecha
                If IsNothing(root.Attribute("fecha")) Then
                    agrega_err(1, "No puede leerse el dato fecha", errores)
                Else
                    comprobante.fecha_factura = root.Attribute("fecha").Value.ToString()
                End If
                'fecha
                If IsNothing(root.Attribute("sello")) Then
                    agrega_err(1, "No puede leerse el dato sello", errores)
                Else
                    comprobante.sello = root.Attribute("sello").Value.ToString()
                End If

                If LlaveCFD.version_nom = "CFD" Then
                    ' numero de aprobacion
                    If IsNothing(root.Attribute("noAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato noAprobacion", errores)
                    Else
                        comprobante.no_aprobacion = Integer.Parse(root.Attribute("noAprobacion").Value)
                    End If
                    ' año de aprobacion
                    If IsNothing(root.Attribute("anoAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato anoAprobacion", errores)
                    Else
                        comprobante.ano_aprobacion = Integer.Parse(root.Attribute("anoAprobacion").Value)
                    End If
                End If


                ' numero de certificado
                If IsNothing(root.Attribute("noCertificado")) Then
                    agrega_err(1, "No puede leerse el dato noCertificado", errores)
                Else
                    comprobante.no_certificado = root.Attribute("noCertificado").Value.ToString()
                End If
                ' certificado
                If IsNothing(root.Attribute("certificado")) Then
                    agrega_err(1, "No puede leerse el dato certificado", errores)
                Else
                    comprobante.certificado = root.Attribute("certificado").Value.ToString()
                End If
                ' subtotal
                If IsNothing(root.Attribute("subTotal")) Then
                    agrega_err(1, "No puede leerse el dato subTotal", errores)
                Else
                    comprobante.sub_total = Decimal.Parse(root.Attribute("subTotal").Value)
                End If
                ' descuento
                If IsNothing(root.Attribute("descuento")) Then
                    'agrega_err(1, "No puede leerse el dato descuento", errores)
                    comprobante.descuento = 0 'el descuento se manda en 0 en caso de no haber atributo 14/08/2013
                Else
                    comprobante.descuento = Decimal.Parse(root.Attribute("descuento").Value)
                End If
                ' total
                If IsNothing(root.Attribute("total")) Then
                    agrega_err(1, "No puede leerse el dato total", errores)
                Else
                    comprobante.total = Decimal.Parse(root.Attribute("total").Value)
                End If
                ' tipo de comprobante
                If IsNothing(root.Attribute("tipoDeComprobante")) Then
                    agrega_err(1, "No puede leerse el dato tipoDeComprobante", errores)
                Else
                    Dim tipoDeComprobante = root.Attribute("tipoDeComprobante").Value.ToString()
                End If


                comprobante.condiciones_pago = "0"



                Dim qList = From xe In xmlElm.Descendants _
                Select xe
                Dim emisor = New emisor
                Dim receptor = New receptor
                Dim conce As New List(Of concepto)
                Dim impuestos = New impuestos
                Dim traslados = New List(Of traslado)
                Dim retenciones = New List(Of retencion)
                Dim addenda = New addenda
                Dim requesForPayment = New requestforpayment
                Dim currency = New currency
                Dim aditionalData = New aditionaldata
                Dim paymentTimePeriod = New paymenttimeperiod
                Dim provider = New provider
                Dim lineItems As New List(Of lineitem)
                Dim renglon As Integer = 1
                Const swCargosConcep As Boolean = False
                Dim errorConceptos As Boolean = False
                Dim errorItms As Boolean = False
                impuestos.total_imp_trasl = 0
                impuestos.sw_retencion = 0
                impuestos.total_imp_reten = 0
                Dim swTotalCapT As Boolean = False
                Dim swTotalCapR As Boolean = False

                For Each xe In qList

                    If xe.Name.LocalName = "Emisor" Then
                        emisor.rfc = CType(xe.Attribute("rfc"), String)
                    End If
                    If xe.Name.LocalName = "Receptor" Then
                        receptor.rfc = CType(xe.Attribute("rfc"), String)
                    End If
                    If xe.Name.LocalName = "Concepto" And errorConceptos = False Then
                        Dim itemComceptos = New concepto
                        itemComceptos.cantidad = CType(xe.Attribute("cantidad"), Decimal)
                        itemComceptos.valor_unitario = CType(xe.Attribute("valorUnitario"), Decimal)
                        itemComceptos.importe = CType(xe.Attribute("importe"), Decimal)
                        conce.Add(itemComceptos)


                        If itemComceptos.cantidad = 0 And itemComceptos.valor_unitario > 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False 'no aplica ya que hay facturas como esta que no maneja cantidad.
                            Else
                                agrega_err(1, "Si la cantidad es 0, no debe agregar valor unitario", errores)
                                errorConceptos = True
                            End If
                        End If

                        If swCargosConcep = True And errorConceptos = False Then
                            If itemComceptos.cantidad <> 1 Then
                                agrega_err(1, "La cantidad siempre debe ser 1 en el cargo extra", errores)
                                errorConceptos = True
                            End If
                        End If
                    End If
                    If xe.Name.LocalName = "Retencion" Then ' And swTotalCapR = False Then
                        If Not IsNothing(xe.Attribute("importe")) Then
                            impuestos.total_imp_reten = impuestos.total_imp_reten + CType(xe.Attribute("importe"), Decimal)
                            Dim itemRet = New retencion
                            itemRet.impuesto = CType(xe.Attribute("impuesto"), String)
                            itemRet.importe = CType(xe.Attribute("importe"), Decimal)
                            retenciones.Add(itemRet)
                            If impuestos.total_imp_reten > 0 Then
                                impuestos.sw_retencion = 1
                            End If

                        Else
                            impuestos.total_imp_trasl = 0
                        End If
                    End If
                    If xe.Name.LocalName = "Traslado" Then 'And  swTotalCapT = False Then
                        If Not IsNothing(xe.Attribute("importe")) Then
                            impuestos.total_imp_trasl = impuestos.total_imp_trasl + CType(xe.Attribute("importe"), Decimal)
                            Dim itemTras = New traslado
                            itemTras.impuesto = CType(xe.Attribute("impuesto"), String)
                            'itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                            'itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                            If CType(xe.Attribute("tasa"), Decimal) > 0 And CType(xe.Attribute("importe"), Decimal) > 0 Then
                                itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                                itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                            Else
                                itemTras.tasa = 0
                                itemTras.importe = 0
                            End If
                            traslados.Add(itemTras)
                        End If
                    End If


                    If xe.Name.LocalName = "aditionalData" Then
                        aditionalData.text_data = CType(xe.Attribute("textData"), String)
                        aditionalData.metododepago = CType(xe.Attribute("methodOfPayment"), String)
                        aditionalData.moneda = CType(xe.Attribute("currency"), String)
                    End If

                    If xe.Name.LocalName = "provider" Then
                        provider.providerid = CType(xe.Attribute("providerID"), String)
                        Const caracterSeparador As String = "@"
                        Dim testPos As Integer = InStr(provider.providerid, caracterSeparador)
                        If testPos <> 2 Then
                            agrega_err(1, "El proveedor no tiene el formato correcto", errores)
                        End If
                        If provider.providerid.Length <> 12 Then
                            agrega_err(1, "Longitud incorrecta en providerID", errores)
                        Else
                            Dim primer As Integer = provider.providerid.IndexOf("@", StringComparison.Ordinal)
                            comprobante.cc_tipo = provider.providerid.Substring(0, primer)
                            Dim segundo As Integer = InStr(3, provider.providerid, "@", CompareMethod.Text)
                            comprobante.cc_cve = provider.providerid.Substring(2, 6)
                            EfCveG = provider.providerid.Substring(segundo, 3)
                        End If
                        If comprobante.cc_cve <> cc_cve Or comprobante.cc_tipo <> cc_tipo Then
                            agrega_err(1, "El dato Provider es incorrecto no corresponde con la session", errores)
                        End If
                    End If
                    If xe.Name.LocalName = "lineItem" And errorItms = False Then

                        Dim item = New lineitem
                        item.type = 1 '@tipo_reng 'xe.Attribute("type")
                        item.number = CType(xe.Attribute("number"), Integer)
                        item.monto_decuento = Decimal.Parse("0.0000") '@mto_descto 'xe.Attribute("montoDescuento")
                        item.pct_decuento = Decimal.Parse("0.0000") '@pct_descto  'xe.Attribute("pctDescuento")
                        item.uns = 1 '@uns 'xe.Attribute("uns")
                        item.precio = CType(xe.Attribute("price"), Decimal)
                        item.sku = CType(xe.Attribute("sku"), String)
                        item.partida = 0 '@num_rengp 'xe.Attribute("partida")
                        item.reference_identification = CType(xe.Attribute("referenceIdentification"), String)
                        item.art_tip = CType(xe.Attribute("articleType"), String)
                        item.uni_med = "zzzzzz" '@uni_med 'xe.Attribute("uni_med")
                        Dim primer As Integer = item.reference_identification.IndexOf("@", StringComparison.Ordinal)
                        If primer < 1 Then
                            agrega_err(1, "El formato del atributo referenceIdentification es incorrecto : " + item.reference_identification, errores)
                            errorItms = True
                        End If
                        lineItems.Add(item)
                        If item.number <> renglon Then
                            agrega_err(1, "Los renglones deben ser agregados en orden lineItem: sku: " + item.sku + "number: " + item.number.ToString(), errores)
                            errorItms = True
                        End If
                        renglon = renglon + 1
                    End If
                Next

                If conce.Count = 0 Then
                    agrega_err(1, "El comprobante no tiene conceptos", errores)
                    errorConceptos = True
                End If

                comprobante.Conceptos = conce
                requesForPayment.line_items = lineItems
                requesForPayment.currency = currency
                requesForPayment.aditional_data = aditionalData
                requesForPayment.paymenttimeperiod = paymentTimePeriod
                addenda.requestforpayment = requesForPayment
                comprobante.Emisor = emisor
                impuestos.Traslados = traslados
                impuestos.Retenciones = retenciones
                comprobante.Emisor = emisor
                comprobante.Receptor = receptor
                comprobante.Impuestos = impuestos
                comprobante.Addenda = addenda

                Dim msgUsr = From msg In errores _
                    Select interror = msg.Interror, message = msg.Message _
                    Where (interror = 1)

                Dim er As New Errores
                Dim cadena As String = msgUsr.Aggregate("", Function(current, msgs) current & msgs.message.Trim + ", ")

                If cadena.Trim <> "" Then
                    er.Interror = 1
                    er.Message = cadena.Trim
                    graba_error(errores, er, LlaveCFD, "60069", "LeeDatosFacturaLINQ")
                End If

            Catch ex As Exception
                agrega_err(1, ex.Message, errores)
            End Try

        End Sub

        Public Sub LeeDatosFacturaLINQ_FacSrv(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal XMLDOCFILEPATH As String, ByVal LlaveCFD As llave_cfd, ByVal cc_cve As String, ByVal cc_tipo As String)

            Dim xmlElm As XElement

            Try

                xmlElm = XElement.Load(XMLDOCFILEPATH)
                Dim root As XElement = XElement.Load(XMLDOCFILEPATH)
                Dim qAtrib As IEnumerable(Of XAttribute) = _
                    From atr In root.Attributes() _
                    Select atr

                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Complemento" Then
                        a.Remove()
                    End If
                Next

                If LlaveCFD.version_nom <> "" Then
                    comprobante.version = LlaveCFD.version
                End If

                If IsNothing(root.Attribute("serie")) Then
                    comprobante.serie = ""
                Else
                    comprobante.serie = root.Attribute("serie").Value.ToString()
                End If
                'folio 
                If IsNothing(root.Attribute("folio")) Then
                    'agrega_err(1, "No puede leerse el dato folio", errores)
                    comprobante.folio_factura = 0
                Else
                    'comprobante.folio_factura = Int64.Parse(root.Attribute("folio").Value)

                    Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("folio").Value, "[^0-9]", ""))  'Int32.Parse(root.Attribute("folio").Value)
                    If folPaso >= 2147483647 Then
                        comprobante.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                    Else
                        comprobante.folio_factura = folPaso
                    End If

                End If
                'fecha
                If IsNothing(root.Attribute("fecha")) Then
                    agrega_err(1, "No puede leerse el dato fecha", errores)
                Else
                    comprobante.fecha_factura = root.Attribute("fecha").Value.ToString()
                End If
                'fecha
                If IsNothing(root.Attribute("sello")) Then
                    agrega_err(1, "No puede leerse el dato sello", errores)
                Else
                    comprobante.sello = root.Attribute("sello").Value.ToString()
                End If

                If LlaveCFD.version_nom = "CFD" Then
                    ' numero de aprobacion
                    If IsNothing(root.Attribute("noAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato noAprobacion", errores)
                    Else
                        comprobante.no_aprobacion = Integer.Parse(root.Attribute("noAprobacion").Value)
                    End If
                    ' año de aprobacion
                    If IsNothing(root.Attribute("anoAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato anoAprobacion", errores)
                    Else
                        comprobante.ano_aprobacion = Integer.Parse(root.Attribute("anoAprobacion").Value)
                    End If
                End If


                ' numero de certificado
                If IsNothing(root.Attribute("noCertificado")) Then
                    agrega_err(1, "No puede leerse el dato noCertificado", errores)
                Else
                    comprobante.no_certificado = root.Attribute("noCertificado").Value.ToString()
                End If
                ' certificado
                If IsNothing(root.Attribute("certificado")) Then
                    agrega_err(1, "No puede leerse el dato certificado", errores)
                Else
                    comprobante.certificado = root.Attribute("certificado").Value.ToString()
                End If
                ' subtotal
                If IsNothing(root.Attribute("subTotal")) Then
                    agrega_err(1, "No puede leerse el dato subTotal", errores)
                Else
                    comprobante.sub_total = Decimal.Parse(root.Attribute("subTotal").Value)
                End If
                ' descuento
                If IsNothing(root.Attribute("descuento")) Then
                    'agrega_err(1, "No puede leerse el dato descuento", errores)
                    comprobante.descuento = 0 'el descuento se manda en 0 en caso de no haber atributo 14/08/2013
                Else
                    comprobante.descuento = Decimal.Parse(root.Attribute("descuento").Value)
                End If
                ' total
                If IsNothing(root.Attribute("total")) Then
                    agrega_err(1, "No puede leerse el dato total", errores)
                Else
                    comprobante.total = Decimal.Parse(root.Attribute("total").Value)
                End If
                ' tipo de comprobante
                If IsNothing(root.Attribute("tipoDeComprobante")) Then
                    agrega_err(1, "No puede leerse el dato tipoDeComprobante", errores)
                Else
                    Dim tipoDeComprobante = root.Attribute("tipoDeComprobante").Value.ToString()
                End If


                comprobante.condiciones_pago = "0"


                Dim qList = From xe In xmlElm.Descendants _
                Select xe
                Dim emisor = New emisor
                Dim receptor = New receptor
                Dim conce As New List(Of concepto)
                Dim impuestos = New impuestos
                Dim traslados = New List(Of traslado)
                Dim retenciones = New List(Of retencion)
                Dim addenda = New addenda
                Dim requesForPayment = New requestforpayment
                Dim currency = New currency
                Dim aditionalData = New aditionaldata
                Dim paymentTimePeriod = New paymenttimeperiod
                Dim provider = New provider
                Dim lineItems As New List(Of lineitem)
                Dim renglon As Integer = 1
                Const swCargosConcep As Boolean = False
                Dim errorConceptos As Boolean = False
                Dim errorItms As Boolean = False
                impuestos.total_imp_trasl = 0
                impuestos.sw_retencion = 0
                impuestos.total_imp_reten = 0
                Dim swTotalCapT As Boolean = False
                Dim swTotalCapR As Boolean = False
                Dim sdoc = New Document

                For Each xe In qList

                    If xe.Name.LocalName = "Emisor" Then
                        emisor.rfc = CType(xe.Attribute("rfc"), String)
                    End If
                    If xe.Name.LocalName = "Receptor" Then
                        receptor.rfc = CType(xe.Attribute("rfc"), String)
                    End If
                    If xe.Name.LocalName = "Concepto" And errorConceptos = False Then
                        Dim itemComceptos = New concepto
                        itemComceptos.cantidad = CType(xe.Attribute("cantidad"), Decimal)
                        itemComceptos.valor_unitario = CType(xe.Attribute("valorUnitario"), Decimal)
                        itemComceptos.importe = CType(xe.Attribute("importe"), Decimal)
                        conce.Add(itemComceptos)

                        If itemComceptos.cantidad = 0 And itemComceptos.valor_unitario > 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False 'no aplica ya que hay facturas como esta que no maneja cantidad.
                            Else
                                agrega_err(1, "Si la cantidad es 0, no debe agregar valor unitario", errores)
                                errorConceptos = True
                            End If
                        End If

                        If swCargosConcep = True And errorConceptos = False Then
                            If itemComceptos.cantidad <> 1 Then
                                agrega_err(1, "La cantidad siempre debe ser 1 en el cargo extra", errores)
                                errorConceptos = True
                            End If
                        End If
                    End If
                    If xe.Name.LocalName = "Retencion" Then ' And swTotalCapR = False Then
                        If Not IsNothing(xe.Attribute("importe")) Then
                            impuestos.total_imp_reten = impuestos.total_imp_reten + CType(xe.Attribute("importe"), Decimal)
                            Dim itemRet = New retencion
                            itemRet.impuesto = CType(xe.Attribute("impuesto"), String)
                            itemRet.importe = CType(xe.Attribute("importe"), Decimal)
                            retenciones.Add(itemRet)
                            If impuestos.total_imp_reten > 0 Then
                                impuestos.sw_retencion = 1
                            End If

                        Else
                            impuestos.total_imp_trasl = 0
                        End If
                    End If
                    If xe.Name.LocalName = "Traslado" Then 'And  swTotalCapT = False Then
                        If Not IsNothing(xe.Attribute("importe")) Then
                            impuestos.total_imp_trasl = impuestos.total_imp_trasl + CType(xe.Attribute("importe"), Decimal)
                            Dim itemTras = New traslado
                            itemTras.impuesto = CType(xe.Attribute("impuesto"), String)
                            'itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                            'itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                            If CType(xe.Attribute("tasa"), Decimal) > 0 And CType(xe.Attribute("importe"), Decimal) > 0 Then
                                itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                                itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                            Else
                                itemTras.tasa = 0
                                itemTras.importe = 0
                            End If
                            traslados.Add(itemTras)
                        End If
                    End If


                    If xe.Name.LocalName = "aditionalData" Then
                        aditionalData.text_data = CType(xe.Attribute("textData"), String)
                        aditionalData.metododepago = CType(xe.Attribute("methodOfPayment"), String)
                        aditionalData.moneda = CType(xe.Attribute("currency"), String)
                    End If

                    If xe.Name.LocalName = "provider" Then
                        provider.providerid = CType(xe.Attribute("providerID"), String)
                        Const caracterSeparador As String = "@"
                        Dim testPos As Integer = InStr(provider.providerid, caracterSeparador)
                        If testPos <> 2 Then
                            agrega_err(1, "El proveedor no tiene el formato correcto", errores)
                        End If
                        If provider.providerid.Length <> 12 Then
                            agrega_err(1, "Longitud incorrecta en providerID", errores)
                        Else
                            Dim primer As Integer = provider.providerid.IndexOf("@", StringComparison.Ordinal)
                            comprobante.cc_tipo = provider.providerid.Substring(0, primer)
                            Dim segundo As Integer = InStr(3, provider.providerid, "@", CompareMethod.Text)
                            comprobante.cc_cve = provider.providerid.Substring(2, 6)
                            EfCveG = provider.providerid.Substring(segundo, 3)
                        End If
                        If comprobante.cc_cve <> cc_cve Or comprobante.cc_tipo <> cc_tipo Then
                            agrega_err(1, "El dato Provider es incorrecto no corresponde con la session", errores)
                        End If
                    End If
                    If xe.Name.LocalName = "document" Then


                        sdoc.referenceIdentification = CType(xe.Attribute("referenceIdentification"), String)
                        Dim primer As Integer = sdoc.referenceIdentification.IndexOf("@", StringComparison.Ordinal)
                        If primer < 1 Then
                            agrega_err(1, "El formato del atributo referenceIdentification es incorrecto : " + sdoc.referenceIdentification, errores)
                        End If

                    End If
                Next

                If conce.Count = 0 Then
                    agrega_err(1, "El comprobante no tiene conceptos", errores)
                    errorConceptos = True
                End If

                comprobante.Conceptos = conce
                requesForPayment.line_items = lineItems
                requesForPayment.currency = currency
                requesForPayment.aditional_data = aditionalData
                requesForPayment.paymenttimeperiod = paymentTimePeriod
                requesForPayment.document = sdoc
                addenda.requestforpayment = requesForPayment
                comprobante.Emisor = emisor
                impuestos.Traslados = traslados
                impuestos.Retenciones = retenciones
                comprobante.Emisor = emisor
                comprobante.Receptor = receptor
                comprobante.Impuestos = impuestos
                comprobante.Addenda = addenda

                Dim msgUsr = From msg In errores _
                    Select interror = msg.Interror, message = msg.Message _
                    Where (interror = 1)

                Dim er As New Errores
                Dim cadena As String = msgUsr.Aggregate("", Function(current, msgs) current & msgs.message.Trim + ", ")

                If cadena.Trim <> "" Then
                    er.Interror = 1
                    er.Message = cadena.Trim
                    graba_error(errores, er, LlaveCFD, "60069", "LeeDatosFacturaLINQ_FacSrv")
                End If

            Catch ex As Exception
                agrega_err(1, ex.Message, errores)
            End Try

        End Sub

        'versiones 3.3
        Public Sub LeeDatosFacturaLINQ_SNAdd3_3(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal xmlDocFilePath As String, ByVal llaveCfd As llave_cfd)

            'GCM 17122014 paraobtener el IVA
            Dim cmd As New SqlCommand
            Dim pctIva As Object
            cmd.CommandText = "select pct_iva from iciva where iva_cve = 1"
            cmd.CommandType = CommandType.Text
            cmd.Connection = Conexion
            Conexion.Open()
            pctIva = cmd.ExecuteScalar()
            Conexion.Close()
            comprobante.pctIva = pctIva

            Dim xmlElm As XElement
            Try
                xmlElm = XElement.Load(xmlDocFilePath)
                Dim root As XElement = XElement.Load(xmlDocFilePath)
                Dim qAtrib As IEnumerable(Of XAttribute) = _
                    From atr In root.Attributes() _
                    Select atr

                'GCM 17122014 
                Dim qList1 = From xe In xmlElm.Descendants _
                Select xe
                ''GCM 17122014 

                'esto es para remover la addenda
                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Addenda" Then
                        a.Remove()
                    End If
                Next

                'GCM 17122014 obtenemos el valor de total de traslados
                For Each xe In qList1
                    If xe.Name.LocalName = "ImpuestosLocales" Then
                        comprobante.TotaldeTraslados = Decimal.Parse(xe.Attribute("TotaldeTraslados").Value)
                        comprobante.TotaldeRetenciones = Decimal.Parse(xe.Attribute("TotaldeRetenciones").Value) 'GCM 11112014 obtenemos el valor de total de retenciones
                    End If
                Next
                'GCM 17122014 

                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Complemento" Then
                        a.Remove()
                    End If
                Next


                If llaveCfd.version_nom <> "" Then
                    comprobante.version = llaveCfd.version
                End If
                If IsNothing(root.Attribute("Serie")) Then
                    comprobante.serie = ""
                Else
                    comprobante.serie = root.Attribute("Serie").Value.ToString()
                End If
                'folio 
                If IsNothing(root.Attribute("Folio")) Then
                    comprobante.folio_factura = 0
                Else
                    'comprobante.folio_factura = Int64.Parse(root.Attribute("folio").Value)
                    Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("Folio").Value, "[^0-9]", ""))  'Int32.Parse(root.Attribute("folio").Value)
                    If folPaso >= 2147483647 Then
                        'comprobante.folio_factura = Int64.Parse(folPaso.ToString())
                        comprobante.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                    Else
                        comprobante.folio_factura = folPaso
                    End If
                End If
                'fecha
                If IsNothing(root.Attribute("Fecha")) Then
                    agrega_err(1, "No puede leerse el dato fecha", errores)
                Else
                    comprobante.fecha_factura = root.Attribute("Fecha").Value.ToString()
                End If
                'fecha
                If IsNothing(root.Attribute("Sello")) Then
                    agrega_err(1, "No puede leerse el dato sello", errores)
                Else
                    comprobante.sello = root.Attribute("Sello").Value.ToString()
                End If
                If llaveCfd.version_nom = "CFD" Then
                    ' numero de aprobacion
                    If IsNothing(root.Attribute("noAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato noAprobacion", errores)
                    Else
                        comprobante.no_aprobacion = Integer.Parse(root.Attribute("noAprobacion").Value)
                    End If
                    ' año de aprobacion
                    If IsNothing(root.Attribute("anoAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato anoAprobacion", errores)
                    Else
                        comprobante.ano_aprobacion = Integer.Parse(root.Attribute("anoAprobacion").Value)
                    End If
                End If
                If IsNothing(root.Attribute("NoCertificado")) Then
                    agrega_err(1, "No puede leerse el dato noCertificado", errores)
                Else
                    comprobante.no_certificado = root.Attribute("NoCertificado").Value.ToString()
                End If
                If Not IsNothing(root.Attribute("Certificado")) Then
                    comprobante.certificado = root.Attribute("Certificado").Value.ToString()
                Else
                    comprobante.certificado = ""
                End If
                ' subtotal
                If IsNothing(root.Attribute("SubTotal")) Then
                    agrega_err(1, "No puede leerse el dato subTotal", errores)
                Else
                    comprobante.sub_total = Decimal.Parse(root.Attribute("SubTotal").Value)
                End If
                ' descuento
                If IsNothing(root.Attribute("Descuento")) Then
                    'agrega_err(1, "No puede leerse el dato descuento", errores)
                    comprobante.descuento = 0 'el descuento se manda en 0 en caso de no haber atributo 14/08/2013
                Else
                    comprobante.descuento = Decimal.Parse(root.Attribute("Descuento").Value)
                End If
                ' total
                If IsNothing(root.Attribute("Total")) Then
                    agrega_err(1, "No puede leerse el dato total", errores)
                Else
                    comprobante.total = Decimal.Parse(root.Attribute("Total").Value)
                End If
                ' tipo de comprobante
                If IsNothing(root.Attribute("TipoDeComprobante")) Then
                    agrega_err(1, "No puede leerse el dato tipoDeComprobante", errores)
                Else
                    Dim tipoDeComprobante = root.Attribute("TipoDeComprobante").Value.ToString()
                End If

                'GCM 05012015 Comentado el tipo de documento ya que no se capturara el tm
                'If comprobante.tipodoc_cve = "BTFSER" Then

                'GCM 07012015 Se agrego el TC
                If IsNothing(root.Attribute("TipoCambio")) Then
                    comprobante.tc = "0"
                Else
                    comprobante.tc = root.Attribute("TipoCambio").Value.ToString
                End If

                'GCM 06012015 se contempla el tc
                If llaveCfd.version = "3.3" Then
                    'Cambio para CDFI 3.3 FGV (12/08/2017)
                    'se va a enviar el id del catalogo de las Monedas
                    comprobante.moneda = root.Attribute("Moneda").Value.ToString()
                Else
                    If IsNothing(root.Attribute("Moneda")) Then
                        If IsNothing(root.Attribute("TipoCambio")) Then
                            comprobante.moneda = "MXN"
                        Else
                            If (comprobante.tc Like "1.00*" Or comprobante.tc = "1") Then
                                comprobante.moneda = "MXN"
                            Else
                                comprobante.moneda = ""
                            End If
                        End If
                    Else
                        'GCM 07012015 contemplamos $ 13012015 contemplamos Ninguno
                        'GCM 11092015 Agregamos MONEDA NACIONAL sin contemplar el TC
                        If (root.Attribute("Moneda").Value.ToString() = "$" Or
                            root.Attribute("Moneda").Value.ToString() = "Ninguno") And
                        (comprobante.tc = "1.00" Or comprobante.tc = "1") Then
                            comprobante.moneda = "MXN"
                        Else
                            'FGV 09/11/2016 se agrego la cadena "Modena Nacional" y "moneda nacional" ya que al no encontrarse la cadena "MONEDA NACIONAL"(en mayusculas) se le asignaba el valor del campo moneda, al momento de insertarlo se enviaba la cadena completa y no la cadena "MXN" 
                            If (root.Attribute("Moneda").Value.ToString().ToUpper() = "MONEDA NACIONAL") Then
                                comprobante.moneda = "MXN"
                            Else
                                If (root.Attribute("Moneda").Value.ToString() = "Pesos") Then
                                    comprobante.moneda = "PES"
                                Else
                                    comprobante.moneda = root.Attribute("Moneda").Value.ToString()
                                End If
                            End If
                        End If
                    End If
                End If



                'If IsNothing(root.Attribute("Moneda")) Then
                '    comprobante.moneda = ""
                'Else
                '    comprobante.moneda = root.Attribute("Moneda").Value.ToString()
                'End If


                Dim qList = From xe In xmlElm.Descendants _
                Select xe
                Dim emisor = New emisor
                Dim receptor = New receptor
                Dim conce As New List(Of concepto)
                Dim impuestos = New impuestos
                Dim traslados = New List(Of traslado)
                Dim retenciones = New List(Of retencion)
                Dim addenda = New addenda
                Dim requesForPayment = New requestforpayment
                Dim currency = New currency
                Dim aditionalData = New aditionaldata
                Dim paymentTimePeriod = New paymenttimeperiod
                Dim provider = New provider
                Dim lineItems As New List(Of lineitem)
                Const swCargosConcep As Boolean = False
                Dim errorConceptos As Boolean = False
                impuestos.total_imp_trasl = 0
                impuestos.sw_retencion = 0
                impuestos.total_imp_reten = 0
                Dim swTotalCapT As Boolean = False
                Dim swTotalCapR As Boolean = False
                Dim Tot_ImpuestosTras As Decimal = 0
                comprobante.swImpTras = True
                comprobante.swTImporte = False

                For Each xe In qList


                    If xe.Name.LocalName = "Emisor" Then
                        emisor.rfc = xe.Attribute("Rfc").Value.ToString()
                    End If
                    'GCM 12022015
                    If emisor.rfc = "TME970922LV3" And comprobante.moneda = "P-US$" Then
                        comprobante.moneda = "DOL"
                    End If

                    If emisor.rfc = "CEM900327M82" And (comprobante.moneda = "D&amp;oacute;lar estadounidense" Or
                                                        comprobante.moneda = "D&oacute;lar estadounidense") Then
                        comprobante.moneda = "DOL"
                    End If

                    If xe.Name.LocalName = "Receptor" Then
                        receptor.rfc = xe.Attribute("Rfc").Value.ToString()
                    End If
                    'GCM 17122014
                    If xe.Name.LocalName = "Impuestos" Then
                        If IsNothing(xe.Attribute("TotalImpuestosTrasladados")) Then
                            comprobante.totalImpuestosTrasladados = 0
                            comprobante.swImpTras = False
                        Else
                            comprobante.totalImpuestosTrasladados = xe.Attribute("TotalImpuestosTrasladados").Value.ToString()
                        End If
                    End If
                    'GCM 17122014
                    If xe.Name.LocalName = "Concepto" And errorConceptos = False Then
                        Dim itemComceptos = New concepto
                        itemComceptos.cantidad = CType(xe.Attribute("Cantidad"), Decimal)
                        itemComceptos.valor_unitario = CType(xe.Attribute("ValorUnitario"), Decimal)
                        itemComceptos.importe = CType(xe.Attribute("Importe"), Decimal)
                        conce.Add(itemComceptos)
                        If itemComceptos.cantidad = 0 And itemComceptos.valor_unitario > 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False 'no aplica ya que hay facturas como esta que no maneja cantidad.
                            Else
                                agrega_err(1, "Si la cantidad es 0, no debe agregar valor unitario", errores)
                                errorConceptos = True
                            End If
                        End If
                        If itemComceptos.cantidad > 0 And itemComceptos.valor_unitario = 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False '
                            Else
                                agrega_err(1, "El valor unitario no puede ser 0", errores)
                                errorConceptos = True
                            End If
                        End If
                        If swCargosConcep = True And errorConceptos = False Then
                            If itemComceptos.cantidad <> 1 Then
                                agrega_err(1, "La cantidad siempre debe ser 1 en el cargo extra", errores)
                                errorConceptos = True
                            End If
                        End If
                    End If

                    If xe.Name.LocalName = "Retencion" Then ' And swTotalCapR = False Then
                        If IsNothing(xe.Attribute("Base")) Then
                            If Not IsNothing(xe.Attribute("Importe")) Then
                                impuestos.total_imp_reten = impuestos.total_imp_reten + CType(xe.Attribute("Importe"), Decimal)
                                Dim itemRet = New retencion
                                itemRet.impuesto = CType(xe.Attribute("Impuesto"), String)
                                itemRet.importe = CType(xe.Attribute("Importe"), Decimal)
                                retenciones.Add(itemRet)
                                If impuestos.total_imp_reten > 0 Then
                                    impuestos.sw_retencion = 1
                                End If

                            Else
                                impuestos.total_imp_trasl = 0
                            End If
                        End If
                    End If


                    If xe.Name.LocalName = "Traslado" Then
                        If IsNothing(xe.Attribute("Base")) Then 'And  swTotalCapT = False Then

                            'GCM 22012015 Obtenemos prefijo, solo entrara si es cfdi
                            Dim prefijo = xe.GetPrefixOfNamespace(xe.Name.NamespaceName)

                            If prefijo = "cfdi" Then
                                If Not IsNothing(xe.Attribute("Importe")) Then
                                    impuestos.total_imp_trasl = impuestos.total_imp_trasl + CType(xe.Attribute("Importe"), Decimal)
                                    Dim itemTras = New traslado
                                    itemTras.impuesto = CType(xe.Attribute("Impuesto"), String)
                                    'itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                                    'itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                                    If CType(xe.Attribute("TasaOCuota"), Decimal) > 0 And CType(xe.Attribute("Importe"), Decimal) > 0 Then
                                        itemTras.tasa = CType(xe.Attribute("TasaOCuota"), Decimal)
                                        itemTras.importe = CType(xe.Attribute("Importe"), Decimal)
                                    Else
                                        itemTras.tasa = 0
                                        itemTras.importe = 0
                                    End If
                                    traslados.Add(itemTras)
                                End If
                            End If
                        End If
                    End If

                Next
                If conce.Count = 0 Then
                    agrega_err(1, "El comprobante no tiene conceptos", errores)
                    errorConceptos = True
                End If
                aditionalData.text_data = "Z"
                provider.providerid = ""
                comprobante.Conceptos = conce
                requesForPayment.provider = provider
                requesForPayment.line_items = lineItems
                requesForPayment.currency = currency
                requesForPayment.aditional_data = aditionalData
                requesForPayment.paymenttimeperiod = paymentTimePeriod
                addenda.requestforpayment = requesForPayment
                comprobante.Emisor = emisor
                comprobante.Receptor = receptor
                impuestos.Traslados = traslados
                impuestos.Retenciones = retenciones
                comprobante.Impuestos = impuestos
                comprobante.Addenda = addenda

                Dim msgUsr = From msg In errores _
                    Select interror = msg.Interror, message = msg.Message _
                    Where (interror = 1)
                Dim er As New Errores
                Dim cadena As String = msgUsr.Aggregate("", Function(current, msgs) current & msgs.message.Trim + ", ")
                If cadena.Trim <> "" Then
                    er.Interror = 1
                    er.Message = cadena.Trim
                    graba_error(errores, er, llaveCfd, "60069", "LeeDatosFacturaLINQ_SinAddenda")
                End If
            Catch ex As Exception
                'agrega_err(1, ex.Message, errores)
                agrega_err(1, "Ocurrio un erro al leer el XML por favor contacte con el administrador de sitio ", errores, "60069")
            End Try

        End Sub

        Public Sub LeeDatosFacturaLinq3_3(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal xmlDocFilePath As String, ByVal llaveCfd As llave_cfd, ByVal ccCve As String, ByVal ccTipo As String)
            Dim xmlElm As XElement
            Try
                xmlElm = XElement.Load(xmlDocFilePath)
                Dim root As XElement = XElement.Load(xmlDocFilePath)


                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Complemento" Then
                        a.Remove()
                    End If
                Next

                If llaveCfd.version_nom <> "" Then
                    comprobante.version = llaveCfd.version
                End If
                If IsNothing(root.Attribute("Serie")) Then
                    comprobante.serie = ""
                Else
                    comprobante.serie = root.Attribute("Serie").Value.ToString()
                End If
                'folio 
                If IsNothing(root.Attribute("Folio")) Then
                    'agrega_err(1, "No puede leerse el dato folio", errores)
                    comprobante.folio_factura = 0
                Else
                    'comprobante.folio_factura = Int64.Parse(root.Attribute("folio").Value)
                    Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("Folio").Value, "[^0-9]", ""))  'Int32.Parse(root.Attribute("folio").Value)
                    If folPaso >= 2147483647 Then
                        comprobante.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                    Else
                        comprobante.folio_factura = folPaso
                    End If
                End If
                'fecha
                If IsNothing(root.Attribute("Fecha")) Then
                    agrega_err(1, "No puede leerse el dato fecha", errores)
                Else
                    comprobante.fecha_factura = root.Attribute("Fecha").Value.ToString()
                End If
                'fecha
                If IsNothing(root.Attribute("Sello")) Then
                    agrega_err(1, "No puede leerse el dato sello", errores)
                Else
                    comprobante.sello = root.Attribute("Sello").Value.ToString()
                End If
                ' numero de certificado
                If IsNothing(root.Attribute("NoCertificado")) Then
                    agrega_err(1, "No puede leerse el dato noCertificado", errores)
                Else
                    comprobante.no_certificado = root.Attribute("NoCertificado").Value.ToString()
                End If
                ' certificado
                If IsNothing(root.Attribute("Certificado")) Then
                    agrega_err(1, "No puede leerse el dato certificado", errores)
                Else
                    comprobante.certificado = root.Attribute("Certificado").Value.ToString()
                End If
                ' subtotal
                If IsNothing(root.Attribute("SubTotal")) Then
                    agrega_err(1, "No puede leerse el dato subTotal", errores)
                Else
                    comprobante.sub_total = Decimal.Parse(root.Attribute("SubTotal").Value)
                End If
                ' descuento
                If IsNothing(root.Attribute("Descuento")) Then
                    comprobante.descuento = 0 'el descuento se manda en 0 en caso de no haber atributo 14/08/2013
                Else
                    comprobante.descuento = Decimal.Parse(root.Attribute("Descuento").Value)
                End If
                ' total
                If IsNothing(root.Attribute("Total")) Then
                    agrega_err(1, "No puede leerse el dato total", errores)
                Else
                    comprobante.total = Decimal.Parse(root.Attribute("Total").Value)
                End If
                ' tipo de comprobante
                If IsNothing(root.Attribute("TipoDeComprobante")) Then
                    agrega_err(1, "No puede leerse el dato tipoDeComprobante", errores)
                Else
                    Dim tipoDeComprobante = root.Attribute("TipoDeComprobante").Value.ToString()
                End If


                comprobante.condiciones_pago = "0"
                Dim qList = From xe In xmlElm.Descendants _
                Select xe
                Dim emisor = New emisor
                Dim receptor = New receptor
                Dim conce As New List(Of concepto)
                Dim impuestos = New impuestos
                Dim traslados = New List(Of traslado)
                Dim retenciones = New List(Of retencion)
                Dim addenda = New addenda
                Dim requesForPayment = New requestforpayment
                Dim currency = New currency
                Dim aditionalData = New aditionaldata
                Dim paymentTimePeriod = New paymenttimeperiod
                Dim provider = New provider
                Dim lineItems As New List(Of lineitem)
                Dim renglon As Integer = 1
                Const swCargosConcep As Boolean = False
                Dim swCargos As Boolean = False
                Dim errorConceptos As Boolean = False
                Dim errorItms As Boolean = False
                impuestos.total_imp_trasl = 0
                impuestos.sw_retencion = 0
                impuestos.total_imp_reten = 0
                Dim swTotalCapT As Boolean = False
                Dim swTotalCapR As Boolean = False

                For Each xe In qList
                    If xe.Name.LocalName = "Emisor" Then
                        emisor.rfc = CType(xe.Attribute("Rfc"), String)
                    End If
                    If xe.Name.LocalName = "Receptor" Then
                        receptor.rfc = CType(xe.Attribute("Rfc"), String)
                    End If
                    If xe.Name.LocalName = "Concepto" And errorConceptos = False Then
                        Dim itemComceptos = New concepto
                        itemComceptos.cantidad = CType(xe.Attribute("Cantidad"), Decimal)
                        itemComceptos.valor_unitario = CType(xe.Attribute("ValorUnitario"), Decimal)
                        itemComceptos.importe = CType(xe.Attribute("Importe"), Decimal)
                        conce.Add(itemComceptos)
                        'If itemComceptos.cantidad = 0 And itemComceptos.valor_unitario > 0 Then 'GCM 05082015 codigo original
                        'agrega_err(1, "Si la cantidad es 0, no debe agregar valor unitario", errores)
                        'errorConceptos = True
                        'End If
                        If itemComceptos.cantidad = 0 And itemComceptos.valor_unitario > 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False 'no aplica ya que hay facturas como esta que no maneja cantidad.
                            Else
                                agrega_err(1, "Si la cantidad es 0, no debe agregar valor unitario", errores)
                                errorConceptos = True
                            End If
                        End If

                        If itemComceptos.cantidad > 0 And itemComceptos.valor_unitario = 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False '
                            Else
                                agrega_err(1, "El valor unitario no puede ser 0", errores)
                                errorConceptos = True
                            End If
                        End If

                        If swCargosConcep = True And errorConceptos = False Then
                            If itemComceptos.cantidad <> 1 Then
                                agrega_err(1, "La cantidad siempre debe ser 1 en el cargo extra", errores)
                                errorConceptos = True
                            End If
                        End If
                    End If

                    If xe.Name.LocalName = "Retencion" Then ' And swTotalCapR = False Then
                        If IsNothing(xe.Attribute("Base")) Then
                            If Not IsNothing(xe.Attribute("Importe")) Then
                                impuestos.total_imp_reten = impuestos.total_imp_reten + CType(xe.Attribute("Importe"), Decimal)
                                Dim itemRet = New retencion
                                itemRet.impuesto = CType(xe.Attribute("Impuesto"), String)
                                itemRet.importe = CType(xe.Attribute("Importe"), Decimal)
                                retenciones.Add(itemRet)
                                If impuestos.total_imp_reten > 0 Then
                                    impuestos.sw_retencion = 1
                                End If

                            Else
                                impuestos.total_imp_trasl = 0
                            End If
                        End If
                    End If
                    If xe.Name.LocalName = "Traslado" Then 'And  swTotalCapT = False Then
                        If IsNothing(xe.Attribute("Base")) Then
                            If Not IsNothing(xe.Attribute("Importe")) Then
                                impuestos.total_imp_trasl = impuestos.total_imp_trasl + CType(xe.Attribute("Importe"), Decimal)
                                Dim itemTras = New traslado
                                itemTras.impuesto = CType(xe.Attribute("Impuesto"), String)
                                'itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                                'itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                                If CType(xe.Attribute("TasaOCuota"), Decimal) > 0 And CType(xe.Attribute("Importe"), Decimal) > 0 Then
                                    itemTras.tasa = CType(xe.Attribute("TasaOCuota"), Decimal)
                                    itemTras.importe = CType(xe.Attribute("Importe"), Decimal)
                                Else
                                    itemTras.tasa = 0
                                    itemTras.importe = 0
                                End If
                                traslados.Add(itemTras)
                            End If
                        End If
                    End If


                    If xe.Name.LocalName = "AditionalData" Then
                        aditionalData.text_data = CType(xe.Attribute("textData"), String)
                        aditionalData.metododepago = CType(xe.Attribute("metodoDePago"), String)
                        aditionalData.moneda = CType(xe.Attribute("Moneda"), String)
                    End If
                    If xe.Name.LocalName = "Provider" Then
                        provider.providerid = CType(xe.Attribute("ProviderID"), String)
                        Const caracterSeparador As String = "@"
                        Dim testPos As Integer = InStr(provider.providerid, caracterSeparador)
                        If testPos <> 2 Then
                            agrega_err(1, "El proveedor no tiene el formato correcto", errores)
                        End If
                        If provider.providerid.Length <> 12 Then
                            agrega_err(1, "Longitud incorrecta en ProviderID", errores)
                        Else
                            Dim primer As Integer = provider.providerid.IndexOf("@", StringComparison.Ordinal)
                            comprobante.cc_tipo = provider.providerid.Substring(0, primer)
                            Dim segundo As Integer = InStr(3, provider.providerid, "@", CompareMethod.Text)
                            comprobante.cc_cve = provider.providerid.Substring(2, 6)
                            EfCveG = provider.providerid.Substring(segundo, 3)
                        End If
                        If comprobante.cc_cve <> ccCve Or comprobante.cc_tipo <> ccTipo Then
                            agrega_err(1, "El dato Provider es incorrecto no corresponde con la session", errores)
                        End If
                    End If
                    If xe.Name.LocalName = "lineItem" And errorItms = False Then
                        Dim item = New lineitem
                        item.type = CType(xe.Attribute("type").Value, Integer)
                        item.number = CType(xe.Attribute("number"), Integer)
                        item.monto_decuento = CType(xe.Attribute("montoDescuento"), Decimal)
                        item.pct_decuento = CType(xe.Attribute("pctDescuento"), Decimal)
                        item.uns = CType(xe.Attribute("uns"), Decimal)
                        item.precio = CType(xe.Attribute("precio"), Decimal)
                        item.sku = CType(xe.Attribute("sku"), String)
                        item.partida = CType(xe.Attribute("partida"), Integer)
                        item.reference_identification = CType(xe.Attribute("referenceIdentification"), String)
                        item.art_tip = CType(xe.Attribute("art_tip"), String)
                        item.uni_med = CType(xe.Attribute("uni_med"), String)
                        Dim primer As Integer = item.reference_identification.IndexOf("@", StringComparison.Ordinal)
                        If primer < 1 Then
                            agrega_err(1, "El formato del atributo referenceIdentification es incorrecto : " + item.reference_identification, errores)
                            errorItms = True
                        End If

                        lineItems.Add(item)

                        If item.number <> renglon Then
                            agrega_err(1, "Los renglones deben ser agregados en orden lineItem3_3: sku: " + item.sku + "number: " + item.number.ToString(), errores)
                            errorItms = True
                        End If

                        If swCargos = True And item.type = 1 Then
                            agrega_err(1, "Los cargos deben agregarse después de todos los artículos lineItem3_3: sku: " + item.sku, errores)
                            errorItms = True
                        End If
                        If item.type = 2 And swCargos = False Then
                            swCargos = True
                        End If

                        If item.uns = 0 And item.precio > 0 Then
                            agrega_err(1, "Si las uns son 0, no debe agregar precio", errores)
                            errorItms = True
                        End If

                        If item.uns > 0 And item.precio = 0 Then
                            agrega_err(1, "Si el precio es 0, no debe agregar uns", errores)
                            errorItms = True
                        End If

                        If swCargos = True Then
                            If item.monto_decuento > 0 Then
                                agrega_err(1, "Incorrecto aplicar descuento a cargo extra ", errores) ' + item.sku, errores)
                                errorItms = True
                            End If
                            If item.pct_decuento > 0 Then
                                agrega_err(1, "Los cargos no debe grabarse porcentaje de descuento ", errores) '+ item.sku, errores)
                                errorItms = True
                            End If
                            If item.partida > 0 Then
                                agrega_err(1, "El dato partida para los cargos extra debe ser 0", errores)
                                errorItms = True
                            End If
                            If item.uns <> 1 Then
                                agrega_err(1, "Las uns siempre debe ser 1 en el cargo extra", errores)
                                errorItms = True
                            End If

                            If item.uni_med.ToString().ToUpper <> "NO APLICA" Then
                                agrega_err(1, "La unidad de medida debe ser 'No aplica' en cargo extra", errores)
                                errorItms = True
                            End If

                        End If
                        renglon = renglon + 1

                    End If
                Next

                If conce.Count = 0 Then
                    agrega_err(1, "El comprobante no tiene conceptos", errores, "60069")
                    errorConceptos = True
                End If

                comprobante.Conceptos = conce
                requesForPayment.line_items = lineItems
                requesForPayment.currency = currency
                requesForPayment.aditional_data = aditionalData
                requesForPayment.paymenttimeperiod = paymentTimePeriod
                addenda.requestforpayment = requesForPayment
                comprobante.Emisor = emisor
                impuestos.Traslados = traslados
                impuestos.Retenciones = retenciones
                comprobante.Emisor = emisor
                comprobante.Receptor = receptor
                comprobante.Impuestos = impuestos
                comprobante.Addenda = addenda

                Dim msgUsr = From msg In errores _
                    Select interror = msg.Interror, message = msg.Message _
                    Where (interror = 1)

                Dim er As New Errores
                Dim cadena As String = msgUsr.Aggregate("", Function(current, msgs) current & msgs.message.Trim + ", ")

                If cadena.Trim <> "" Then
                    er.Interror = 1
                    er.Message = cadena.Trim
                    graba_error(errores, er, llaveCfd, "60069", "LeeDatosFacturaLINQ")
                End If

            Catch ex As Exception
                agrega_err(1, ex.Message, errores, "60069")
            End Try

        End Sub

        Public Sub LeeDatosFacturaLINQ_FacEmb3_3(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal XMLDOCFILEPATH As String, ByVal LlaveCFD As llave_cfd, ByVal cc_cve As String, ByVal cc_tipo As String)

            Dim xmlElm As XElement

            Try

                xmlElm = XElement.Load(XMLDOCFILEPATH)
                Dim root As XElement = XElement.Load(XMLDOCFILEPATH)
                Dim qAtrib As IEnumerable(Of XAttribute) = _
                    From atr In root.Attributes() _
                    Select atr

                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Complemento" Then
                        a.Remove()
                    End If
                Next

                If LlaveCFD.version_nom <> "" Then
                    comprobante.version = LlaveCFD.version
                End If

                If IsNothing(root.Attribute("Serie")) Then
                    comprobante.serie = ""
                Else
                    comprobante.serie = root.Attribute("Serie").Value.ToString()
                End If
                'folio 
                If IsNothing(root.Attribute("Folio")) Then
                    'agrega_err(1, "No puede leerse el dato folio", errores)
                    comprobante.folio_factura = 0
                Else
                    'comprobante.folio_factura = Int64.Parse(root.Attribute("folio").Value)
                    Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("Folio").Value, "[^0-9]", ""))  'Int32.Parse(root.Attribute("folio").Value)
                    If folPaso >= 2147483647 Then
                        comprobante.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                    Else
                        comprobante.folio_factura = folPaso
                    End If
                End If
                'fecha
                If IsNothing(root.Attribute("Fecha")) Then
                    agrega_err(1, "No puede leerse el dato fecha", errores)
                Else
                    comprobante.fecha_factura = root.Attribute("Fecha").Value.ToString()
                End If
                'fecha
                If IsNothing(root.Attribute("Sello")) Then
                    agrega_err(1, "No puede leerse el dato sello", errores)
                Else
                    comprobante.sello = root.Attribute("Sello").Value.ToString()
                End If

                ' numero de certificado
                If IsNothing(root.Attribute("NoCertificado")) Then
                    agrega_err(1, "No puede leerse el dato noCertificado", errores)
                Else
                    comprobante.no_certificado = root.Attribute("NoCertificado").Value.ToString()
                End If
                ' certificado
                If IsNothing(root.Attribute("Certificado")) Then
                    agrega_err(1, "No puede leerse el dato certificado", errores)
                Else
                    comprobante.certificado = root.Attribute("Certificado").Value.ToString()
                End If
                ' subtotal
                If IsNothing(root.Attribute("SubTotal")) Then
                    agrega_err(1, "No puede leerse el dato subTotal", errores)
                Else
                    comprobante.sub_total = Decimal.Parse(root.Attribute("SubTotal").Value)
                End If
                ' descuento
                If IsNothing(root.Attribute("Descuento")) Then
                    'agrega_err(1, "No puede leerse el dato descuento", errores)
                    comprobante.descuento = 0 'el descuento se manda en 0 en caso de no haber atributo 14/08/2013
                Else
                    comprobante.descuento = Decimal.Parse(root.Attribute("Descuento").Value)
                End If
                ' total
                If IsNothing(root.Attribute("Total")) Then
                    agrega_err(1, "No puede leerse el dato total", errores)
                Else
                    comprobante.total = Decimal.Parse(root.Attribute("Total").Value)
                End If
                ' tipo de comprobante
                If IsNothing(root.Attribute("TipoDeComprobante")) Then
                    agrega_err(1, "No puede leerse el dato tipoDeComprobante", errores)
                Else
                    Dim tipoDeComprobante = root.Attribute("TipoDeComprobante").Value.ToString()
                End If


                comprobante.condiciones_pago = "0"



                Dim qList = From xe In xmlElm.Descendants _
                Select xe
                Dim emisor = New emisor
                Dim receptor = New receptor
                Dim conce As New List(Of concepto)
                Dim impuestos = New impuestos
                Dim traslados = New List(Of traslado)
                Dim retenciones = New List(Of retencion)
                Dim addenda = New addenda
                Dim requesForPayment = New requestforpayment
                Dim currency = New currency
                Dim aditionalData = New aditionaldata
                Dim paymentTimePeriod = New paymenttimeperiod
                Dim provider = New provider
                Dim lineItems As New List(Of lineitem)
                Dim renglon As Integer = 1
                Const swCargosConcep As Boolean = False
                Dim errorConceptos As Boolean = False
                Dim errorItms As Boolean = False
                impuestos.total_imp_trasl = 0
                impuestos.sw_retencion = 0
                impuestos.total_imp_reten = 0
                Dim swTotalCapT As Boolean = False
                Dim swTotalCapR As Boolean = False

                For Each xe In qList

                    If xe.Name.LocalName = "Emisor" Then
                        emisor.rfc = CType(xe.Attribute("Rfc"), String)
                    End If
                    If xe.Name.LocalName = "Receptor" Then
                        receptor.rfc = CType(xe.Attribute("Rfc"), String)
                    End If
                    If xe.Name.LocalName = "Concepto" And errorConceptos = False Then
                        Dim itemComceptos = New concepto
                        itemComceptos.cantidad = CType(xe.Attribute("Cantidad"), Decimal)
                        itemComceptos.valor_unitario = CType(xe.Attribute("ValorUnitario"), Decimal)
                        itemComceptos.importe = CType(xe.Attribute("Importe"), Decimal)
                        conce.Add(itemComceptos)


                        If itemComceptos.cantidad = 0 And itemComceptos.valor_unitario > 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False 'no aplica ya que hay facturas como esta que no maneja cantidad.
                            Else
                                agrega_err(1, "Si la cantidad es 0, no debe agregar valor unitario", errores)
                                errorConceptos = True
                            End If
                        End If

                        If itemComceptos.cantidad > 0 And itemComceptos.valor_unitario = 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False '
                            Else
                                agrega_err(1, "El valor unitario no puede ser 0", errores)
                                errorConceptos = True
                            End If
                        End If

                        If swCargosConcep = True And errorConceptos = False Then
                            If itemComceptos.cantidad <> 1 Then
                                agrega_err(1, "La cantidad siempre debe ser 1 en el cargo extra", errores)
                                errorConceptos = True
                            End If
                        End If
                    End If
                    If xe.Name.LocalName = "Retencion" Then ' And swTotalCapR = False Then
                        If Not IsNothing(xe.Attribute("Importe")) Then
                            impuestos.total_imp_reten = impuestos.total_imp_reten + CType(xe.Attribute("Importe"), Decimal)
                            Dim itemRet = New retencion
                            itemRet.impuesto = CType(xe.Attribute("Impuesto"), String)
                            itemRet.importe = CType(xe.Attribute("Importe"), Decimal)
                            retenciones.Add(itemRet)
                            If impuestos.total_imp_reten > 0 Then
                                impuestos.sw_retencion = 1
                            End If

                        Else
                            impuestos.total_imp_trasl = 0
                        End If
                    End If
                    If xe.Name.LocalName = "Traslado" Then 'And  swTotalCapT = False Then
                        If Not IsNothing(xe.Attribute("Importe")) Then
                            impuestos.total_imp_trasl = impuestos.total_imp_trasl + CType(xe.Attribute("Importe"), Decimal)
                            Dim itemTras = New traslado
                            itemTras.impuesto = CType(xe.Attribute("Impuesto"), String)
                            'itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                            'itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                            If CType(xe.Attribute("TasaOCuota"), Decimal) > 0 And CType(xe.Attribute("Importe"), Decimal) > 0 Then
                                itemTras.tasa = CType(xe.Attribute("TasaOCuota"), Decimal)
                                itemTras.importe = CType(xe.Attribute("Importe"), Decimal)
                            Else
                                itemTras.tasa = 0
                                itemTras.importe = 0
                            End If
                            traslados.Add(itemTras)
                        End If
                    End If


                    If xe.Name.LocalName = "aditionalData" Then
                        aditionalData.text_data = CType(xe.Attribute("textData"), String)
                        aditionalData.metododepago = CType(xe.Attribute("methodOfPayment"), String)
                        aditionalData.moneda = CType(xe.Attribute("currency"), String)
                    End If

                    If xe.Name.LocalName = "provider" Then
                        provider.providerid = CType(xe.Attribute("providerID"), String)
                        Const caracterSeparador As String = "@"
                        Dim testPos As Integer = InStr(provider.providerid, caracterSeparador)
                        If testPos <> 2 Then
                            agrega_err(1, "El proveedor no tiene el formato correcto", errores)
                        End If
                        If provider.providerid.Length <> 12 Then
                            agrega_err(1, "Longitud incorrecta en providerID", errores)
                        Else
                            Dim primer As Integer = provider.providerid.IndexOf("@", StringComparison.Ordinal)
                            comprobante.cc_tipo = provider.providerid.Substring(0, primer)
                            Dim segundo As Integer = InStr(3, provider.providerid, "@", CompareMethod.Text)
                            comprobante.cc_cve = provider.providerid.Substring(2, 6)
                            EfCveG = provider.providerid.Substring(segundo, 3)
                        End If
                        If comprobante.cc_cve <> cc_cve Or comprobante.cc_tipo <> cc_tipo Then
                            agrega_err(1, "El dato Provider es incorrecto no corresponde con la session", errores)
                        End If
                    End If
                    If xe.Name.LocalName = "lineItem" And errorItms = False Then

                        Dim item = New lineitem
                        item.type = 1 '@tipo_reng 'xe.Attribute("type")
                        item.number = CType(xe.Attribute("number"), Integer)
                        item.monto_decuento = Decimal.Parse("0.0000") '@mto_descto 'xe.Attribute("montoDescuento")
                        item.pct_decuento = Decimal.Parse("0.0000") '@pct_descto  'xe.Attribute("pctDescuento")
                        item.uns = 1 '@uns 'xe.Attribute("uns")
                        item.precio = CType(xe.Attribute("price"), Decimal)
                        item.sku = CType(xe.Attribute("sku"), String)
                        item.partida = 0 '@num_rengp 'xe.Attribute("partida")
                        item.reference_identification = CType(xe.Attribute("referenceIdentification"), String)
                        item.art_tip = CType(xe.Attribute("articleType"), String)
                        item.uni_med = "zzzzzz" '@uni_med 'xe.Attribute("uni_med")
                        Dim primer As Integer = item.reference_identification.IndexOf("@", StringComparison.Ordinal)
                        If primer < 1 Then
                            agrega_err(1, "El formato del atributo referenceIdentification es incorrecto : " + item.reference_identification, errores)
                            errorItms = True
                        End If
                        lineItems.Add(item)
                        If item.number <> renglon Then
                            agrega_err(1, "Los renglones deben ser agregados en orden lineItem3_3: sku: " + item.sku + "number: " + item.number.ToString(), errores)
                            errorItms = True
                        End If
                        renglon = renglon + 1
                    End If
                Next

                If conce.Count = 0 Then
                    agrega_err(1, "El comprobante no tiene conceptos", errores)
                    errorConceptos = True
                End If

                comprobante.Conceptos = conce
                requesForPayment.line_items = lineItems
                requesForPayment.currency = currency
                requesForPayment.aditional_data = aditionalData
                requesForPayment.paymenttimeperiod = paymentTimePeriod
                addenda.requestforpayment = requesForPayment
                comprobante.Emisor = emisor
                impuestos.Traslados = traslados
                impuestos.Retenciones = retenciones
                comprobante.Emisor = emisor
                comprobante.Receptor = receptor
                comprobante.Impuestos = impuestos
                comprobante.Addenda = addenda

                Dim msgUsr = From msg In errores _
                    Select interror = msg.Interror, message = msg.Message _
                    Where (interror = 1)

                Dim er As New Errores
                Dim cadena As String = msgUsr.Aggregate("", Function(current, msgs) current & msgs.message.Trim + ", ")

                If cadena.Trim <> "" Then
                    er.Interror = 1
                    er.Message = cadena.Trim
                    graba_error(errores, er, LlaveCFD, "60069", "LeeDatosFacturaLINQ")
                End If

            Catch ex As Exception
                agrega_err(1, ex.Message, errores)
            End Try

        End Sub

        Public Sub LeeDatosFacturaLINQ_FacSrv3_3(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal XMLDOCFILEPATH As String, ByVal LlaveCFD As llave_cfd, ByVal cc_cve As String, ByVal cc_tipo As String)

            Dim xmlElm As XElement

            Try

                xmlElm = XElement.Load(XMLDOCFILEPATH)
                Dim root As XElement = XElement.Load(XMLDOCFILEPATH)
                Dim qAtrib As IEnumerable(Of XAttribute) = _
                    From atr In root.Attributes() _
                    Select atr

                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Complemento" Then
                        a.Remove()
                    End If
                Next

                If LlaveCFD.version_nom <> "" Then
                    comprobante.version = LlaveCFD.version
                End If

                If IsNothing(root.Attribute("Serie")) Then
                    comprobante.serie = ""
                Else
                    comprobante.serie = root.Attribute("Serie").Value.ToString()
                End If
                'folio 
                If IsNothing(root.Attribute("Folio")) Then
                    'agrega_err(1, "No puede leerse el dato folio", errores)
                    comprobante.folio_factura = 0
                Else
                    'comprobante.folio_factura = Int64.Parse(root.Attribute("folio").Value)

                    Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("Folio").Value, "[^0-9]", ""))  'Int32.Parse(root.Attribute("folio").Value)
                    If folPaso >= 2147483647 Then
                        comprobante.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                    Else
                        comprobante.folio_factura = folPaso
                    End If

                End If
                'fecha
                If IsNothing(root.Attribute("Fecha")) Then
                    agrega_err(1, "No puede leerse el dato fecha", errores)
                Else
                    comprobante.fecha_factura = root.Attribute("Fecha").Value.ToString()
                End If
                'fecha
                If IsNothing(root.Attribute("Sello")) Then
                    agrega_err(1, "No puede leerse el dato sello", errores)
                Else
                    comprobante.sello = root.Attribute("Sello").Value.ToString()
                End If

                ' numero de certificado
                If IsNothing(root.Attribute("NoCertificado")) Then
                    agrega_err(1, "No puede leerse el dato noCertificado", errores)
                Else
                    comprobante.no_certificado = root.Attribute("NoCertificado").Value.ToString()
                End If
                ' certificado
                If IsNothing(root.Attribute("Certificado")) Then
                    agrega_err(1, "No puede leerse el dato certificado", errores)
                Else
                    comprobante.certificado = root.Attribute("Certificado").Value.ToString()
                End If
                ' subtotal
                If IsNothing(root.Attribute("SubTotal")) Then
                    agrega_err(1, "No puede leerse el dato subTotal", errores)
                Else
                    comprobante.sub_total = Decimal.Parse(root.Attribute("SubTotal").Value)
                End If
                ' descuento
                If IsNothing(root.Attribute("Descuento")) Then
                    'agrega_err(1, "No puede leerse el dato descuento", errores)
                    comprobante.descuento = 0 'el descuento se manda en 0 en caso de no haber atributo 14/08/2013
                Else
                    comprobante.descuento = Decimal.Parse(root.Attribute("Descuento").Value)
                End If
                ' total
                If IsNothing(root.Attribute("Total")) Then
                    agrega_err(1, "No puede leerse el dato total", errores)
                Else
                    comprobante.total = Decimal.Parse(root.Attribute("Total").Value)
                End If
                ' tipo de comprobante
                If IsNothing(root.Attribute("TipoDeComprobante")) Then
                    agrega_err(1, "No puede leerse el dato tipoDeComprobante", errores)
                Else
                    Dim tipoDeComprobante = root.Attribute("TipoDeComprobante").Value.ToString()
                End If


                comprobante.condiciones_pago = "0"


                Dim qList = From xe In xmlElm.Descendants _
                Select xe
                Dim emisor = New emisor
                Dim receptor = New receptor
                Dim conce As New List(Of concepto)
                Dim impuestos = New impuestos
                Dim traslados = New List(Of traslado)
                Dim retenciones = New List(Of retencion)
                Dim addenda = New addenda
                Dim requesForPayment = New requestforpayment
                Dim currency = New currency
                Dim aditionalData = New aditionaldata
                Dim paymentTimePeriod = New paymenttimeperiod
                Dim provider = New provider
                Dim lineItems As New List(Of lineitem)
                Dim renglon As Integer = 1
                Const swCargosConcep As Boolean = False
                Dim errorConceptos As Boolean = False
                Dim errorItms As Boolean = False
                impuestos.total_imp_trasl = 0
                impuestos.sw_retencion = 0
                impuestos.total_imp_reten = 0
                Dim swTotalCapT As Boolean = False
                Dim swTotalCapR As Boolean = False
                Dim sdoc = New Document

                For Each xe In qList

                    If xe.Name.LocalName = "Emisor" Then
                        emisor.rfc = CType(xe.Attribute("Rfc"), String)
                    End If
                    If xe.Name.LocalName = "Receptor" Then
                        receptor.rfc = CType(xe.Attribute("Rfc"), String)
                    End If
                    If xe.Name.LocalName = "Concepto" And errorConceptos = False Then
                        Dim itemComceptos = New concepto
                        itemComceptos.cantidad = CType(xe.Attribute("Cantidad"), Decimal)
                        itemComceptos.valor_unitario = CType(xe.Attribute("ValorUnitario"), Decimal)
                        itemComceptos.importe = CType(xe.Attribute("Importe"), Decimal)
                        conce.Add(itemComceptos)

                        If itemComceptos.cantidad = 0 And itemComceptos.valor_unitario > 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False 'no aplica ya que hay facturas como esta que no maneja cantidad.
                            Else
                                agrega_err(1, "Si la cantidad es 0, no debe agregar valor unitario", errores)
                                errorConceptos = True
                            End If
                        End If

                        If itemComceptos.cantidad > 0 And itemComceptos.valor_unitario = 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False '
                            Else
                                agrega_err(1, "El valor unitario no puede ser 0", errores)
                                errorConceptos = True
                            End If
                        End If

                        If swCargosConcep = True And errorConceptos = False Then
                            If itemComceptos.cantidad <> 1 Then
                                agrega_err(1, "La cantidad siempre debe ser 1 en el cargo extra", errores)
                                errorConceptos = True
                            End If
                        End If
                    End If
                    If xe.Name.LocalName = "Retencion" Then ' And swTotalCapR = False Then
                        If Not IsNothing(xe.Attribute("Importe")) Then
                            impuestos.total_imp_reten = impuestos.total_imp_reten + CType(xe.Attribute("Importe"), Decimal)
                            Dim itemRet = New retencion
                            itemRet.impuesto = CType(xe.Attribute("Impuesto"), String)
                            itemRet.importe = CType(xe.Attribute("Importe"), Decimal)
                            retenciones.Add(itemRet)
                            If impuestos.total_imp_reten > 0 Then
                                impuestos.sw_retencion = 1
                            End If

                        Else
                            impuestos.total_imp_trasl = 0
                        End If
                    End If
                    If xe.Name.LocalName = "Traslado" Then 'And  swTotalCapT = False Then
                        If Not IsNothing(xe.Attribute("Importe")) Then
                            impuestos.total_imp_trasl = impuestos.total_imp_trasl + CType(xe.Attribute("Importe"), Decimal)
                            Dim itemTras = New traslado
                            itemTras.impuesto = CType(xe.Attribute("Impuesto"), String)
                            'itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                            'itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                            If CType(xe.Attribute("TasaOCuota"), Decimal) > 0 And CType(xe.Attribute("Importe"), Decimal) > 0 Then
                                itemTras.tasa = CType(xe.Attribute("TasaOCuota"), Decimal)
                                itemTras.importe = CType(xe.Attribute("Importe"), Decimal)
                            Else
                                itemTras.tasa = 0
                                itemTras.importe = 0
                            End If
                            traslados.Add(itemTras)
                        End If
                    End If


                    If xe.Name.LocalName = "aditionalData" Then
                        aditionalData.text_data = CType(xe.Attribute("textData"), String)
                        aditionalData.metododepago = CType(xe.Attribute("methodOfPayment"), String)
                        aditionalData.moneda = CType(xe.Attribute("currency"), String)
                    End If

                    If xe.Name.LocalName = "provider" Then
                        provider.providerid = CType(xe.Attribute("providerID"), String)
                        Const caracterSeparador As String = "@"
                        Dim testPos As Integer = InStr(provider.providerid, caracterSeparador)
                        If testPos <> 2 Then
                            agrega_err(1, "El proveedor no tiene el formato correcto", errores)
                        End If
                        If provider.providerid.Length <> 12 Then
                            agrega_err(1, "Longitud incorrecta en providerID", errores)
                        Else
                            Dim primer As Integer = provider.providerid.IndexOf("@", StringComparison.Ordinal)
                            comprobante.cc_tipo = provider.providerid.Substring(0, primer)
                            Dim segundo As Integer = InStr(3, provider.providerid, "@", CompareMethod.Text)
                            comprobante.cc_cve = provider.providerid.Substring(2, 6)
                            EfCveG = provider.providerid.Substring(segundo, 3)
                        End If
                        If comprobante.cc_cve <> cc_cve Or comprobante.cc_tipo <> cc_tipo Then
                            agrega_err(1, "El dato Provider es incorrecto no corresponde con la session", errores)
                        End If
                    End If
                    If xe.Name.LocalName = "document" Then


                        sdoc.referenceIdentification = CType(xe.Attribute("referenceIdentification"), String)
                        Dim primer As Integer = sdoc.referenceIdentification.IndexOf("@", StringComparison.Ordinal)
                        If primer < 1 Then
                            agrega_err(1, "El formato del atributo referenceIdentification es incorrecto : " + sdoc.referenceIdentification, errores)
                        End If

                    End If
                Next

                If conce.Count = 0 Then
                    agrega_err(1, "El comprobante no tiene conceptos", errores)
                    errorConceptos = True
                End If

                comprobante.Conceptos = conce
                requesForPayment.line_items = lineItems
                requesForPayment.currency = currency
                requesForPayment.aditional_data = aditionalData
                requesForPayment.paymenttimeperiod = paymentTimePeriod
                requesForPayment.document = sdoc
                addenda.requestforpayment = requesForPayment
                comprobante.Emisor = emisor
                impuestos.Traslados = traslados
                impuestos.Retenciones = retenciones
                comprobante.Emisor = emisor
                comprobante.Receptor = receptor
                comprobante.Impuestos = impuestos
                comprobante.Addenda = addenda

                Dim msgUsr = From msg In errores _
                    Select interror = msg.Interror, message = msg.Message _
                    Where (interror = 1)

                Dim er As New Errores
                Dim cadena As String = msgUsr.Aggregate("", Function(current, msgs) current & msgs.message.Trim + ", ")

                If cadena.Trim <> "" Then
                    er.Interror = 1
                    er.Message = cadena.Trim
                    graba_error(errores, er, LlaveCFD, "60069", "LeeDatosFacturaLINQ_FacSrv")
                End If

            Catch ex As Exception
                agrega_err(1, ex.Message, errores)
            End Try

        End Sub


        Public Sub agrega_err(ByVal errorXml As Integer, ByVal smensaje As String, ByVal errores As List(Of Errores))
            Dim er As New Errores
            iErrorG = 1
            er.Interror = errorXml
            er.Message = smensaje
            er.CveError = ""
            errores.Add(er)
        End Sub

        Public Sub agrega_err(ByVal errorXml As Integer, ByVal smensaje As String, ByVal errores As List(Of Errores), ByVal cveError As String)
            Dim er As New Errores
            iErrorG = 1
            er.Interror = errorXml
            er.Message = smensaje
            er.CveError = cveError
            errores.Add(er)
        End Sub

        Public Sub agrega_folio(ByVal factura As List(Of nuevas_facturas), ByVal numFol As Integer, ByVal tipoDoc As String, ByVal ef_cve As String)
            Dim fact As New nuevas_facturas
            fact.num_fol = numFol
            fact.tipo_doc = tipoDoc
            fact.ef_cve = ef_cve
            factura.Add(fact)
        End Sub

        Public Sub LeerErroresSql(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd)
            Dim sqlAdapter = New SqlDataAdapter("sp_cons_errores_xml", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_cons_errores_xml")
                sqlAdapter.Dispose()
                Dim errorTable = ds.Tables.Item(0)

                For Each row As DataRow In errorTable.Rows
                    Dim rfcEmisor As String = CStr(row("rfc_emisor"))
                    Dim serie = CStr(row("serie"))
                    Dim folioFactura = CStr(row("folio_factura"))
                    Dim tipoError As Integer = CType(row("tipo_error"), Integer)
                    Dim msgCve As String = CStr(row("msg_cve"))
                    Dim mensajeErr As String = CStr(row("mensaje"))
                    Dim msgAdicional As String = CStr(row("msg_adicional"))
                    If tipoError = 1 Then
                        agrega_err(1, mensajeErr + " " + msgAdicional, errores)
                    End If

                    If tipoError = 2 Or tipoError = 3 Then
                        'Dim sCadena As String = mensajeErr + " " + msgAdicional + " ," + msgCve + " Tipo:" + tipoError.ToString _
                        '                       + vbNewLine + "RFC Emisor:" + rfcEmisor _
                        '                       + vbNewLine + "Serie:" + serie _
                        '                       + vbNewLine + "Folio:" + folioFactura

                        Dim sCadena As String = mensajeErr + " " + msgAdicional + vbNewLine
                        agrega_err(tipoError, sCadena, errores, msgCve)
                    End If
                    If msgCve = 60000 Then
                        agrega_err(1, "Su comprobante CFD no fue recibido, por favor consulte con Soporte CFD", errores)
                    End If
                    If msgCve = 60057 Then
                        agrega_err(1, "Factura incorrecta, ya existen folios posteriores", errores)
                    End If
                    If msgCve = 60051 Then
                        agrega_err(1, "", errores)
                    End If
                Next
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
            Catch ex As Exception
                Dim msg = "sp_cons_errores_xml"
                iErrorG = 3
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try

        End Sub

        Public Sub ValidaDatosEncabezado(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)

            'Dim sqlAdapter = New SqlDataAdapter("sp_Valida_XML_zfj", Conexion)
            Dim sqlAdapter = New SqlDataAdapter("sp_Valida_XML", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout

            If Not (llaveCfd.ef_cve Is Nothing) And EfCveG Is Nothing Then
                EfCveG = llaveCfd.ef_cve
            End If

            If EfCveG Is Nothing Then
                EfCveG = "zzz"
            End If

            Dim varIva As Decimal '= comprobante.Impuestos.Traslados.tasa / 100

            If llaveCfd.version = "3.3" Then
                Dim iva = From cons In comprobante.Impuestos.Traslados
                     Select cons.tasa, cons.impuesto
                     Where (impuesto = "002")

                For Each i In iva
                    Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                    If swtasa = True Then
                        varIva = i.tasa
                    Else
                        varIva = i.tasa * 100
                    End If
                Next
            Else
                Dim iva = From cons In comprobante.Impuestos.Traslados
                     Select cons.tasa, cons.impuesto
                     Where (impuesto = "IVA")

                For Each i In iva
                    Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                    If swtasa = True Then
                        varIva = i.tasa
                    Else
                        varIva = i.tasa * 100
                    End If
                Next
            End If

            



            sqlAdapter.SelectCommand.Parameters.AddWithValue("@nom_arch", comprobante.nom_arch)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@version", comprobante.version)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", comprobante.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", comprobante.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@fecha_factura", comprobante.fecha_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sello", comprobante.sello)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_aprobacion", comprobante.no_aprobacion)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ano_aprobacion", comprobante.ano_aprobacion)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado", comprobante.no_certificado)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@certificado", comprobante.certificado)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@condiciones_pago", comprobante.Addenda.requestforpayment.paymenttimeperiod.timeperiod.ToString())
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@metodo_pago", comprobante.Addenda.requestforpayment.aditional_data.metododepago) 'comprobante.metodo_pago)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sub_total", comprobante.sub_total)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@descuento", comprobante.descuento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@total", comprobante.total)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", comprobante.Emisor.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_receptor", comprobante.Receptor.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tm", comprobante.Addenda.requestforpayment.aditional_data.moneda) 'comprobante.moneda)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@d_adicionales", comprobante.Addenda.requestforpayment.aditional_data.text_data)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@pct_iva", varIva) ' Decimal.Parse(comprobante.Impuestos.Traslados.tasa))
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", comprobante.cc_cve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_tipo", comprobante.cc_tipo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_Cve", EfCveG)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado_sat", llaveCfd.timbre_fiscal.no_certificado_sat)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sw_reten_iva", comprobante.Impuestos.sw_retencion)

            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_Valida_XML")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)

                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If

                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()

            Catch ex As Exception
                Dim msg As String
                Dim er As Errores = Nothing

                msg = "sp_Valida_XML"

                er.Interror = 3
                er.Message = ex.Message

                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_Valida_XML")

            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub

        ' Metodos para cuando se sube una factura
        Public Sub ConsInsertaXml(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd, ByVal ccTipo As String, ByVal ccCve As String, ByVal rfcReceptor As String, ByVal tipdocCve As String)
            Dim sqlAdapter = New SqlDataAdapter("sp_inserta_xml", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            Dim er As New Errores
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@id", "WEB")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_cf", llaveCfd.version_nom)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@nom_arch", llaveCfd.nom_arch)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", ccCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_tipo", ccTipo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_receptor", rfcReceptor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_doc", tipdocCve)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_inserta_xml")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                Dim sw As Integer
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                    sw = CType(ValidacionEncabezado.Rows(0).Item("sw_cfd"), Integer)
                End If
                If sw = 3 Then
                    llaveCfd.sw_sin_addenda = 1
                End If

                If tipdocCve = "BTFSER" Then 'siempre sera como captura para hacer efecto en tablas temporales
                    llaveCfd.sw_sin_addenda = 1
                End If

                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
                If iErrorG = 60000 Then
                    agrega_err(1, "Usted subió un archivo con este folio y serie, consulte seguimiento", errores)
                End If
                If iErrorG = 60057 Then
                    agrega_err(1, "Factura incorrecta, ya existen folios posteriores", errores)
                End If
                If iErrorG <> 0 Then
                    agrega_err(iErrorG, "", errores, iErrorG.ToString())
                End If
            Catch ex As Exception
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
                Dim msg As String
                msg = "sp_inserta_xml"
                iErrorG = 3
                er.Interror = iErrorG
                er.Message = ex.Message
                graba_error(errores, er, llaveCfd, "0", "sp_inserta_xml")
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub

        Public Sub ValidaDatosDetalle(ByVal errores As List(Of Errores), ByVal items As List(Of lineitem), ByVal llaveCfd As llave_cfd)
            Dim item = New lineitem

            For Each item In items
                'se ocupa para xml 3.2 y 3.3
                ValidaDatosItem(errores, item, llaveCfd)
                If errores.Count > 0 Or iErrorG > 0 Then
                    Exit For
                End If
            Next
        End Sub

        Public Sub ValidaDatosItem(ByVal errores As List(Of Errores), ByVal item As lineitem, ByVal llaveCfd As llave_cfd)

            Dim er As New Errores
            Dim sqlAdapter = New SqlDataAdapter("sp_Valida_Detalle_XML", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sku_cve", item.sku)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@art_tip", item.art_tip)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@num_reng", item.number)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uns", item.uns)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uni_med", item.uni_med)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@pct_descto", item.pct_decuento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@mto_descto", item.monto_decuento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@precio", item.precio)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", EfCveG)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@recepcion", item.reference_identification.Replace(" ", ""))
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@num_rengp", item.partida)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_reng", item.type)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_Valida_XML")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()

            Catch ex As Exception
                Dim msg As String
                msg = "sp_Valida_Detalle_XML"
                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_Valida_Detalle_XML")

            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try

        End Sub

        Public Sub ValidaDatosPapa(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd)
            Dim er As New Errores
            Dim sqlAdapter = New SqlDataAdapter("sp_valida_XML_papa", Conexion)
            'Dim sqlAdapter = New SqlDataAdapter("sp_valida_XML_papa_zfj", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", EfCveG)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_valida_XML_papa")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
                If iErrorG > 0 Then
                    agrega_err(iErrorG, MensajeError, errores)
                End If
            Catch ex As Exception
                Dim msg As String
                msg = "sp_valida_XML_papa"
                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_valida_XML_papa")
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub

        Public Sub GeneraEncabezadoFactura(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd, ByVal factura As List(Of nuevas_facturas))

            Dim er As New Errores
            Dim sqlAdapter = New SqlDataAdapter("sp_genera_factura", Conexion)
            'Dim sqlAdapter = New SqlDataAdapter("sp_genera_factura_zfj", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout

            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", EfCveG)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)

            Try
                Conexion.Open()
                _transaccion = Conexion.BeginTransaction()
                sqlAdapter.SelectCommand.Transaction = _transaccion
                sqlAdapter.Fill(ds, "sp_genera_factura")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(ds.Tables.Count - 1)
                For Each row As DataRow In ValidacionEncabezado.Rows
                    iErrorG = CType(row("error"), Integer)
                    If iErrorG = 0 Then
                        EfCveG = CStr(row("ef_cve"))
                    End If
                    agrega_folio(factura, CType(row("num_fol"), Integer), CStr(row("tipo_doc")), CStr(row("ef_cve")))
                Next

                If iErrorG > 0 Then
                    agrega_err(iErrorG, "", errores)
                    _transaccion.Rollback()
                Else
                    _transaccion.Commit()
                End If
                _transaccion.Dispose()
                Conexion.Close()
                SqlConnection.ClearPool(Conexion)
                SqlConnection.ClearAllPools()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()

            Catch ex As Exception
                If _transaccion IsNot Nothing Then
                    _transaccion.Rollback()
                End If
                Dim msg As String
                msg = "sp_genera_factura"
                iErrorG = 3
                er.Interror = iErrorG
                er.Message = ex.Message + " GeneraEncabezadoFactura"
                Dim msgCve As String = DirectCast(ex, SqlException).Number.ToString()

                Const procedimiento As String = "sp_genera_factura"
                graba_error(errores, er, llaveCfd, msgCve.ToString(), procedimiento)
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                    SqlConnection.ClearPool(Conexion)
                    SqlConnection.ClearAllPools()
                End If
            End Try

        End Sub

        Public Sub graba_error(ByVal errores As List(Of Errores), ByVal err As Errores, ByVal llaveCfd As llave_cfd, ByVal msgCve As String, ByVal procedimiento As String)
            Dim sqlAdapter = New SqlDataAdapter("sp_gerror_xml", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            Dim errMsg As String = ""
            If err.Message.Length > 50 Then
                errMsg = err.Message.Substring(0, 50)
            End If
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_error", err.Interror)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@msg_cve", msgCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@msg_adicional", errMsg)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@procedimiento", procedimiento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                If Conexion.State = ConnectionState.Closed Then
                    Conexion.Open()
                End If
                sqlAdapter.Fill(ds, "sp_gerror_xml")
                sqlAdapter.Dispose()
                Conexion.Close()
                ds.Reset()
            Catch ex As Exception


                Dim msg As String
                msg = "sp_cons_errores_xml"
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub

        Public Sub GeneraContraReciboFactura(ByVal errores As List(Of Errores), ByVal llaveCdf As llave_cfd)

            Dim er As New Errores

            If EfCveG = "" Or EfCveG = Nothing Then
                EfCveG = llaveCdf.ef_cve
            End If

            Dim sqlAdapter = New SqlDataAdapter("sp_gencontrarec_fe", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", EfCveG)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCdf.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCdf.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCdf.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCdf.timbre_fiscal.uuid)
            Try
                Conexion.Open()
                _transaccion = Conexion.BeginTransaction
                sqlAdapter.SelectCommand.Transaction = _transaccion
                sqlAdapter.Fill(ds, "sp_gencontrarec_fe")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                FolioCr = CType(ValidacionEncabezado.Rows(0).Item("num_fol_contra"), Integer)
                DoctoCr = ValidacionEncabezado.Rows(0).Item("tipo_doc_contra").ToString.Trim
                _transaccion.Commit()
                _transaccion.Dispose()
                Conexion.Close()
                ValidacionEncabezado.Rows.Clear()
                ds.Reset()
            Catch ex As Exception
                If _transaccion IsNot Nothing Then
                    _transaccion.Rollback()
                End If

                MensajeError = "sp_gencontrarec"
                iErrorG = 3
                er.Interror = iErrorG
                er.Message = ex.Message + " GeneraContraReciboFactura"
                Dim msgCve = DirectCast(ex, System.Data.SqlClient.SqlException).Number

                graba_error(errores, er, llaveCdf, msgCve.ToString(), "sp_gencontrarec_fe")
                agrega_err(iErrorG, MensajeError, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try

        End Sub

        Public Sub llama_errores_tipo2(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd)
            Dim sqlAdapter = New SqlDataAdapter("sp_gmail_xml", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", EfCveG)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo", 2)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_gmail_xml")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()

                If iErrorG > 0 Then
                    agrega_err(iErrorG, MensajeError, errores)
                End If
            Catch ex As Exception
                Dim msg As String
                msg = "sp_gmail_xml"
                iErrorG = 3
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub

        Public Sub cons_doc(ByVal errores As List(Of Errores), ByVal documento As String, ByVal folio As Integer, ByVal partida As Integer, ByVal efCve As String, ByVal tipo As Integer, ByVal dsRes As DataSet)
            Dim sqlAdapter = New SqlDataAdapter("sp_consDatosAcuseR", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", efCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_doc", documento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@num_fol", folio)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@num_reng", partida)
            Try
                Conexion.Open()
                sqlAdapter.Fill(dsRes, "sp_consDatosAcuseR")
                sqlAdapter.Dispose()
                Conexion.Close()
                If iErrorG > 0 Then
                    agrega_err(iErrorG, MensajeError, errores)
                End If
            Catch ex As Exception
                Dim msg As String
                msg = "sp_consDatosAcuseR"
                iErrorG = 3
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub

        Public Function cons_trans(ByVal errores As List(Of Errores), ByVal ccTipo As String, ByVal ccCve As String) As DataTable
            Dim dtRes As New DataTable
            Dim sqlAdapter = New SqlDataAdapter("qcomsercon1_web", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_tipo", ccTipo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", ccCve)
            Try
                Conexion.Open()
                sqlAdapter.Fill(dtRes)
                sqlAdapter.Dispose()
                Conexion.Close()
            Catch ex As Exception
                Dim msg As String
                msg = "qcomsercon1_web:" + ccTipo + ", " + ccCve
                iErrorG = 3
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return dtRes
        End Function

        Public Function cons_placa(ByVal errores As List(Of Errores), ByVal tipo As String, ByVal ccCve As String) As DataTable
            Dim dtPlaca As New DataTable
            Dim sqlAdapter = New SqlDataAdapter("qcomplacas", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo", tipo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", ccCve)
            Try
                Conexion.Open()
                sqlAdapter.Fill(dtPlaca)
                sqlAdapter.Dispose()
                Conexion.Close()
            Catch ex As Exception
                Dim msg As String
                msg = "qcomcve11:" + tipo + ", " + ccCve
                iErrorG = 3
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return dtPlaca
        End Function

        Public Function ConsDatos(ByVal errores As List(Of Errores), ByVal documento As String, ByVal folio As Int64, ByVal partida As Integer, ByVal efCve As String, ByVal tipo As Integer) As DataTable
            Dim dtDatos As New DataTable
            Dim sqlAdapter = New SqlDataAdapter("sp_consDatosAcuseR", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", efCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_doc", documento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@num_fol", folio)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@num_reng", partida)
            Try
                Conexion.Open()
                sqlAdapter.Fill(dtDatos)
                sqlAdapter.Dispose()
                Conexion.Close()
            Catch ex As Exception
                Dim msg As String
                msg = "sp_consDatosAcuseR:" + documento + ", " + efCve + ", " + folio.ToString
                iErrorG = 3
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return dtDatos
        End Function

        Public Function cons_docs(ByVal errores As List(Of Errores), ByVal efCve As String, ByVal tipdocCve As String, ByVal ccTipo As String, ByVal ccCve As String, ByVal tipTransp As String, ByVal cc3Cve As String) As DataTable
            Dim dtDatos As New DataTable
            Dim sqlAdapter = New SqlDataAdapter("sp_qReferenceId", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cver", efCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_doc", tipdocCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_tipo", ccTipo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", ccCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tip_transp", tipTransp)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc3_cve", cc3Cve)
            Try
                Conexion.Open()
                sqlAdapter.Fill(dtDatos)
                sqlAdapter.Dispose()
                Conexion.Close()
            Catch ex As Exception
                Dim msg As String
                msg = "sp_qReferenceId:" + efCve + ", " + tipdocCve + ", " + ccTipo + ", " + ccCve + ", " + tipTransp + ", " + cc3Cve
                iErrorG = 3
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return dtDatos
        End Function

        Public Function cons_retP(ByVal errores As List(Of Errores), ByVal ccTipo As String, ByVal ccCve As String, ByVal tipdocCve As String) As DataTable
            Dim dtDatos As New DataTable

            Dim sqlAdapter = New SqlDataAdapter("sp_cons_sw_reten", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_tipo", ccTipo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", ccCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipdoc_cve", tipdocCve)
            Try
                Conexion.Open()
                sqlAdapter.Fill(dtDatos)
                sqlAdapter.Dispose()
                Conexion.Close()
            Catch ex As Exception
                Dim msg As String
                msg = "sp_cons_sw_reten:" + ccTipo + ", " + ccCve + ", " + tipdocCve + ""
                iErrorG = 3
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return dtDatos
        End Function

        Public Function LlenaCbo(ByVal combo As combos) As DataTable

            Dim recibo As New nuevas_facturas
            Dim myDataAdapter = New SqlDataAdapter("sp_cboWebXML", Conexion)
            Dim ds As New DataSet()
            myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            Conexion.Open()
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", combo.rfc)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@cbo", combo.combo)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@prm1", combo.parametro1.Trim())
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@prm2", combo.parametro2.Trim())

            Try
                myDataAdapter.Fill(ds, "sp_cboWebXML")
                myDataAdapter.Dispose()
                Conexion.Close()
                If ds.Tables(0).Rows.Count >= 1 Then
                    Return ds.Tables(ds.Tables.Count - 1)
                End If
            Catch ex As Exception
                'Dim msg As String
                'msg = "sp_cboWebXML:" + combo.rfc + ", " + combo.combo + ", " + combo.parametro1.Trim() + "," + combo.parametro1.Trim()
                'iErrorG = 3
                'agrega_err(iErrorG, msg, Errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return ds.Tables(ds.Tables.Count - 1)

        End Function

        Public Function ConsultaEntidad() As DataTable
            Dim dtRes As New DataTable
            Dim sqlAdapter = New SqlDataAdapter("consWeb", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@opc", "entFin")
            Try
                Conexion.Open()
                sqlAdapter.Fill(dtRes)
                sqlAdapter.Dispose()
                Conexion.Close()
            Catch ex As Exception
                Dim msg As String
                msg = "consWeb:entFin"
                'iErrorG = 3
                'agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return dtRes
        End Function

        Public Function ConsultaUsuarios(ByRef efCve As String) As DataTable
            Dim dtRes As New DataTable
            Dim sqlAdapter = New SqlDataAdapter("consWeb", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@opc", "usrEnt")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@prm1", efCve)
            Try
                Conexion.Open()
                sqlAdapter.Fill(dtRes)
                sqlAdapter.Dispose()
                Conexion.Close()
            Catch ex As Exception
                Dim msg As String
                msg = "consWeb:entFin"
                'iErrorG = 3
                'agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return dtRes
        End Function

        Public Function ValidaUsuario(ByRef efCve As String, ByRef usrCve As String, ByRef pass As String) As Int16
            Dim dtRes As New DataTable
            Dim valida As Int16 = 0
            Dim sqlAdapter = New SqlDataAdapter("consWeb", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@opc", "valUsr")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@prm1", efCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@prm2", usrCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@prm3", pass)
            Try
                Conexion.Open()
                sqlAdapter.Fill(dtRes)
                If dtRes.Rows.Count > 0 Then
                    valida = Int16.Parse(dtRes.Rows(0).Item("valida").ToString())
                End If
                sqlAdapter.Dispose()
                Conexion.Close()
            Catch ex As Exception
                Dim msg As String
                msg = "consWeb:valUsr"
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return valida
        End Function

        Public Function GetPortalUser(ByVal pUser As String, ByVal pPassword As String) As PerfilesUser
            Dim sqlAdapter = New SqlDataAdapter("sp_accesoweb", Conexion)
            'Dim sqlAdapter = New SqlDataAdapter("sp_accesoweb_zfj", Conexion)
            Dim ds As New DataSet
            Dim pf = New PerfilesUser
            Dim usr As New List(Of DatosUser)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = 50
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@RFC", pUser)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@password", pPassword)
            Try
                Conexion.Open()
                _transaccion = Conexion.BeginTransaction
                sqlAdapter.SelectCommand.Transaction = _transaccion
                sqlAdapter.Fill(ds, "sp_accesoweb")
                Dim acceso As DataTable = ds.Tables("sp_accesoweb")
                Dim consulta = From datos In acceso.AsEnumerable _
                    Select New With { _
                        .rfc = datos.Field(Of String)("rfc"), _
                        .proveedor = datos.Field(Of String)("proveedor"), _
                        .cc_tipo = datos.Field(Of String)("cc_tipo"), _
                        .cc_cve = datos.Field(Of String)("cc_cve"), _
                        .tipdoc_cve = datos.Field(Of String)("tipdoc_cve"), _
                        .nom_doc = datos.Field(Of String)("nom_doc")
                        }
                For Each con In consulta
                    Dim dta As New DatosUser
                    dta.Rfc = Trim(con.rfc)
                    dta.Proveedor = con.proveedor
                    dta.CcTipo = con.cc_tipo
                    dta.CcCve = con.cc_cve
                    dta.TipdocCve = con.tipdoc_cve
                    dta.NomDoc = con.nom_doc
                    dta.Seleccionado = False
                    usr.Add(dta)
                Next
                pf.Perfil = usr
                sqlAdapter.Dispose()
                _transaccion.Commit()
                Conexion.Close()
                _transaccion.Dispose()
            Catch ex As Exception
                Dim msg As String
                msg = "Error al ejecutar sp. sp_accesoweb:" + pUser + ", " + pPassword
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return pf
        End Function

        Public Function GetConfig(ByVal noInf As Integer) As List(Of ConfigGral)
            Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", Conexion)
            Dim ds As New DataSet()
            Dim configuraciones = New List(Of ConfigGral)
            myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            Conexion.Open()
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", "?")
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", 0)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", "?")
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", 10)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", "?")
            Try
                myDataAdapter.Fill(ds, "sp_consInfXML")
                myDataAdapter.Dispose()
                Conexion.Close()
                For Each itm As DataRow In ds.Tables(0).Rows
                    Dim config As String = itm(0)
                    Dim tipo As String = itm(1).ToString.Trim
                    Dim valors As String = itm(2).ToString.Trim
                    Dim valorn As Decimal = itm(3)
                    If tipo = "s" Then
                        configuraciones.Add(New ConfigGral() With {.config = config, .tipo = tipo, .valor = valors})
                    End If
                    If tipo = "n" Then
                        configuraciones.Add(New ConfigGral() With {.config = config, .tipo = tipo, .valor = valorn})
                    End If
                Next
            Catch ex As Exception
                Dim msg As String = "Error al consultar sp_consInfXML"
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return configuraciones
        End Function

        Public Function ObtDatosGenerico(ByVal rfc As String, _
                             ByVal serie As String, _
                             ByVal folio As String, _
                             ByVal uuid As String, _
                             ByVal numInfo As Integer
                             ) As DataTable
            Dim recibo As New Skytex.FacturaElectronica.nuevas_facturas
            Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", Conexion)
            Dim dt As New DataTable()
            myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfc)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", Int64.Parse(folio))
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", numInfo)

            Try
                Conexion.Open()
                myDataAdapter.Fill(dt)
                myDataAdapter.Dispose()
                Conexion.Close()
            Catch ex As Exception
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try

            Return dt
        End Function


#End Region

        Private Function Program() As Object
            Throw New NotImplementedException
        End Function

    End Class


    Public Class Datos
        Public Function MiConexion() As SqlConnection
            Dim conexion As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("developConnectionString")
            Dim myConnection As New SqlConnection(conexion.ConnectionString)
            Return myConnection
        End Function

        Public Function time_out() As Integer
            Dim time As Integer = Int16.Parse(ConfigurationManager.AppSettings("time"))
            Return time
        End Function

        Public Function ObtieneDb() As String
            'Dim cadena As String = ConfigurationManager.ConnectionStrings("developConnectionString").ToString
            'Dim partes() As String = cadena.Split(New Char() {";"c})
            'Dim base As String = ""

            'For index = 0 To partes.Count - 1
            '    Dim i As Integer = partes(index).IndexOf("Initial Catalog", StringComparison.Ordinal)
            '    If i > -1 Then
            '        base = partes(index).Substring(partes(index).IndexOf("=", StringComparison.Ordinal) + 1, partes(index).Length - partes(index).IndexOf("=", StringComparison.Ordinal) - 1)
            '    End If
            'Next
            'Return base.ToUpper
            Dim cadena As String = ConfigurationManager.ConnectionStrings("developConnectionString").ToString
            Dim partes() As String = cadena.Split(New Char() {";"c})
            Dim base As String = ""

            For index = 0 To partes.Count - 1
                ' Dim i As Integer = partes(index).IndexOf("Initial Catalog", StringComparison.Ordinal)
                Dim i As Integer = partes(index).IndexOf("Data Source", StringComparison.Ordinal)
                If i > -1 Then
                    base = partes(index).Substring(partes(index).IndexOf("=", StringComparison.Ordinal) + 1, partes(index).Length - partes(index).IndexOf("=", StringComparison.Ordinal) - 1)
                End If
            Next
            Return base.ToUpper
        End Function
    End Class



End Namespace






