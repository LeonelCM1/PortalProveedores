'Imports Microsoft.VisualBasic
Imports System.Runtime.Remoting.Messaging

Namespace Skytex.FacturaElectronica



#Region "Clases"



    Public Class Errores

        Private _interror As Integer
        Private _message As String
        Private _cveError As String

        Public Property Interror() As Integer
            Get
                Return _interror
            End Get
            Set(ByVal value As Integer)
                _interror = value
            End Set
        End Property

        Public Property Message() As String
            Get
                Return _message
            End Get
            Set(ByVal value As String)
                _message = value
            End Set
        End Property

        Public Property CveError() As String
            Get
                Return _cveError
            End Get
            Set(ByVal value As String)
                _cveError = value
            End Set
        End Property

    End Class



    Public Class RegimenFiscal
        Private _regimenFiscal As String

        Public Property RegimenFisc() As String
            Get
                Return _regimenFiscal
            End Get
            Set(value As String)
                _regimenFiscal = value
            End Set
        End Property


        Public Sub RegimenFiscal()

        End Sub

        Public Sub RegimenFiscal(ByVal RegimenFisc As String)
            Me.RegimenFisc = _regimenFiscal
        End Sub
    End Class



    Public Class domiciliofiscal

        Private _calle As String
        Private _no_interior As String
        Private _no_exterior As String
        Private _colonia As String
        Private _localidad As String
        Private _municipio As String
        Private _estado As String
        Private _pais As String
        Private _codigo_postal As String

        Public Property calle() As String
            Get
                Return _calle
            End Get
            Set(ByVal value As String)
                _calle = value
            End Set
        End Property
        Public Property no_interior() As String
            Get
                Return _no_interior
            End Get
            Set(value As String)
                _no_interior = value
            End Set
        End Property
        Public Property no_exterior() As String
            Get
                Return _no_exterior
            End Get
            Set(ByVal value As String)
                _no_exterior = value
            End Set
        End Property
        Public Property colonia() As String
            Get
                Return _colonia
            End Get
            Set(ByVal value As String)
                _colonia = value
            End Set
        End Property
        Public Property municipio() As String
            Get
                Return _municipio
            End Get
            Set(ByVal value As String)
                _municipio = value
            End Set
        End Property
        Public Property localidad() As String
            Get
                Return _localidad
            End Get
            Set(value As String)
                _localidad = value
            End Set
        End Property

        Public Property estado() As String
            Get
                Return _estado
            End Get
            Set(ByVal value As String)
                _estado = value
            End Set
        End Property
        Public Property pais() As String
            Get
                Return _pais
            End Get
            Set(ByVal value As String)
                _pais = value
            End Set
        End Property
        Public Property codigo_postal() As String
            Get
                Return _codigo_postal
            End Get
            Set(ByVal value As String)
                _codigo_postal = value
            End Set
        End Property

        Public Sub domiciliofiscal()
        End Sub

        Public Sub domiciliofiscal(ByVal calle As String, ByVal no_exterior As String, _
                                   ByVal no_interior As String, ByVal localidad As String, _
                                   ByVal colonia As String, ByVal municipio As String, _
                                   ByVal estado As String, ByVal pais As String, _
                                   ByVal codigo_postal As String)
            Me.calle = _calle
            Me.no_exterior = _no_exterior
            Me.no_interior = _no_interior
            Me.colonia = _colonia
            Me.localidad = _localidad
            Me.municipio = _municipio
            Me.estado = _estado
            Me.pais = _pais
            Me.codigo_postal = _codigo_postal

        End Sub

    End Class

    Public Class emisor
        Private _rfc As String
        Private _nombre As String
        Private _DomicilioFiscal As domiciliofiscal
        Private _regimen As RegimenFiscal

        Public Property rfc() As String
            Get
                Return _rfc
            End Get
            Set(ByVal value As String)
                _rfc = value
            End Set
        End Property
        Public Property nombre() As String
            Get
                Return _nombre
            End Get
            Set(ByVal value As String)
                _nombre = value
            End Set
        End Property
        Public Property DomicilioFiscal() As domiciliofiscal
            Get
                Return _DomicilioFiscal
            End Get
            Set(ByVal value As domiciliofiscal)
                _DomicilioFiscal = value
            End Set
        End Property
        Public Property Regimen() As RegimenFiscal
            Get
                Return _regimen
            End Get
            Set(value As RegimenFiscal)
                _regimen = value
            End Set
        End Property

        Public Sub emisor()
        End Sub
        Public Sub emisor(ByVal rfc As String, _
                          ByVal nombre As String, _
                          ByVal DomicilioFiscal As domiciliofiscal,
                          ByVal regimen As RegimenFiscal
                          )
            Me.rfc = _rfc
            Me.nombre = _nombre
            Me.DomicilioFiscal = _DomicilioFiscal
            Me.Regimen = _regimen

        End Sub
    End Class

    Public Class receptor
        Private _rfc As String
        Private _nombre As String
        Private _DomicilioFiscal As domiciliofiscal

        Public Property rfc() As String
            Get
                Return _rfc
            End Get
            Set(ByVal value As String)
                _rfc = value
            End Set
        End Property
        Public Property nombre() As String
            Get
                Return _nombre
            End Get
            Set(ByVal value As String)
                _nombre = value
            End Set
        End Property
        Public Property DomicilioFiscal() As domiciliofiscal
            Get
                Return _DomicilioFiscal
            End Get
            Set(ByVal value As domiciliofiscal)
                _DomicilioFiscal = value
            End Set
        End Property
        Public Sub receptor()
        End Sub
        Public Sub receptor(ByVal rfc As String, ByVal nombre As String, ByVal DomicilioFiscal As domiciliofiscal)
            Me.rfc = _rfc
            Me.nombre = _nombre
            Me.DomicilioFiscal = _DomicilioFiscal

        End Sub
    End Class

    Public Class concepto

        Private _cantidad As Decimal
        Private _unidad As String
        Private _no_identificacion As String
        Private _descripcion As String
        Private _valor_unitario As Decimal
        Private _importe As Decimal

        Public Property cantidad() As Decimal
            Get
                Return _cantidad
            End Get
            Set(ByVal value As Decimal)
                _cantidad = value
            End Set
        End Property
        Public Property unidad() As String
            Get
                Return _unidad
            End Get
            Set(ByVal value As String)
                _unidad = value
            End Set
        End Property
        Public Property no_identificacion() As String
            Get
                Return _no_identificacion
            End Get
            Set(ByVal value As String)
                _no_identificacion = value
            End Set
        End Property
        Public Property descripcion() As String
            Get
                Return _descripcion
            End Get
            Set(ByVal value As String)
                _descripcion = value
            End Set
        End Property
        Public Property valor_unitario() As Decimal
            Get
                Return _valor_unitario
            End Get
            Set(ByVal value As Decimal)
                _valor_unitario = value
            End Set
        End Property
        Public Property importe() As Decimal
            Get
                Return _importe
            End Get
            Set(ByVal value As Decimal)
                _importe = value
            End Set
        End Property
        Public Sub concepto()
        End Sub
        Public Sub concepto(ByVal cantidad As Decimal, ByVal unidad As String, _
                            ByVal no_identificaion As String, ByVal descripcion As String, _
                            ByVal valor_unitario As Decimal, ByVal importe As Decimal)
            Me.cantidad = _cantidad
            Me.unidad = _unidad
            Me.no_identificacion = _no_identificacion
            Me.descripcion = _descripcion
            Me.valor_unitario = _valor_unitario
            Me.importe = _importe
        End Sub

    End Class

    Public Class traslado
        Private _impuesto As String
        Private _tasa As Decimal
        Private _importe As Decimal

        Public Property impuesto() As String
            Get
                Return _impuesto
            End Get
            Set(ByVal value As String)
                _impuesto = value
            End Set
        End Property
        Public Property tasa() As Decimal
            Get
                Return _tasa
            End Get
            Set(ByVal value As Decimal)
                _tasa = value
            End Set
        End Property
        Public Property importe() As Decimal
            Get
                Return _importe
            End Get
            Set(ByVal value As Decimal)
                _importe = value
            End Set
        End Property

        Public Sub traslado()
        End Sub

        Public Sub traslado(ByVal impuesto As String, ByVal tasa As Decimal, _
                            ByVal importe As Decimal)
            Me.impuesto = _impuesto
            Me.tasa = _tasa
            Me.importe = _importe
        End Sub

    End Class

    Public Class retencion
        Private _impuesto As String
        Private _importe As Decimal

        Public Property impuesto() As String
            Get
                Return _impuesto
            End Get
            Set(ByVal value As String)
                _impuesto = value
            End Set
        End Property

        Public Property importe() As Decimal
            Get
                Return _importe
            End Get
            Set(ByVal value As Decimal)
                _importe = value
            End Set
        End Property

        Public Sub retencion()
        End Sub

        Public Sub retencion(ByVal impuesto As String, _
                            ByVal importe As Decimal)
            Me.impuesto = _impuesto
            Me.importe = _importe
        End Sub

    End Class

    Public Class impuestos

        Private _total_imp_trasl As Decimal
        Private _total_imp_reten As Decimal
        Private _Traslados As List(Of traslado)
        Private _Retenciones As List(Of retencion)
        Private _sw_retencion As Integer


        Public Property total_imp_trasl() As Decimal
            Get
                Return _total_imp_trasl
            End Get
            Set(ByVal value As Decimal)
                _total_imp_trasl = value
            End Set
        End Property

        Public Property total_imp_reten() As Decimal
            Get
                Return _total_imp_reten
            End Get
            Set(ByVal value As Decimal)
                _total_imp_reten = value
            End Set
        End Property

        Public Property Traslados() As List(Of traslado)
            Get
                Return _Traslados
            End Get
            Set(ByVal value As List(Of traslado))
                _Traslados = value
            End Set
        End Property

        Public Property Retenciones() As List(Of retencion)
            Get
                Return _Retenciones
            End Get
            Set(ByVal value As List(Of retencion))
                _Retenciones = value
            End Set
        End Property

        Public Property sw_retencion() As Integer
            Get
                Return _sw_retencion
            End Get
            Set(ByVal value As Integer)
                _sw_retencion = value
            End Set
        End Property



        Public Sub impuestos()
        End Sub

        Public Sub impuestos(ByVal total_imp_trasl As Decimal, ByVal total_imp_reten As Decimal, ByVal Traslados As List(Of traslado), _
                              ByVal Retenciones As List(Of retencion), ByVal sw_retencion As Integer)
            Me.total_imp_trasl = _total_imp_trasl
            Me.total_imp_reten = _total_imp_reten
            Me.Traslados = _Traslados
            Me.Retenciones = _Retenciones
            Me.sw_retencion = _sw_retencion
        End Sub
    End Class

    Public Class currency

        Private _currency_iso_code As String
        Public Property currency_iso_code() As String
            Get
                Return _currency_iso_code
            End Get
            Set(ByVal value As String)
                _currency_iso_code = value
            End Set
        End Property
        Public Sub currency()
        End Sub
        Public Sub currency(ByVal currency_iso_code As String)
            Me.currency_iso_code = _currency_iso_code
        End Sub
    End Class

    Public Class aditionaldata

        Private _text_data As String
        Private _metododepago As String
        Private _moneda As String

        Public Property text_data() As String
            Get
                Return _text_data
            End Get
            Set(ByVal value As String)
                'If value = Nothing Then
                '    value = ""
                'End If
                _text_data = value
            End Set
        End Property
        Public Property metododepago() As String
            Get
                Return _metododepago
            End Get
            Set(ByVal value As String)
                _metododepago = value
            End Set
        End Property
        Public Property moneda() As String
            Get
                Return _moneda
            End Get
            Set(ByVal value As String)
                _moneda = value
            End Set
        End Property

        Public Sub aditionaldata()
        End Sub
        Public Sub aditionaldata(ByVal text_data As String, ByVal metododepago As String, ByVal moneda As String)
            Me.text_data = _text_data
            Me.metododepago = _metododepago
            Me.moneda = _moneda
        End Sub
    End Class

    Public Class paymenttimeperiod

        Private _timeperiod As Integer
        Public Property timeperiod() As Integer
            Get
                Return _timeperiod
            End Get
            Set(ByVal value As Integer)
                _timeperiod = value
            End Set
        End Property
        Public Sub paymenttimeperiod()
        End Sub
        Public Sub paymenttimeperiod(ByVal timeperiod As Integer)
            Me.timeperiod = _timeperiod
        End Sub
    End Class

    Public Class provider

        Private _providerid As String
        Public Property providerid() As String
            Get
                Return _providerid
            End Get
            Set(ByVal value As String)
                _providerid = value
            End Set
        End Property
        Public Sub provider()
        End Sub
        Public Sub provider(ByVal providerid As String)
            Me.providerid = _providerid
        End Sub
    End Class

    Public Class lineitem


        Private _type As Integer
        Private _number As Integer
        Private _monto_decuento As Decimal
        Private _pct_decuento As Decimal
        Private _uns As Decimal
        Private _precio As Decimal
        Private _sku As String
        Private _partida As Integer
        Private _reference_identification As String
        Private _art_tip As String
        Private _uni_med As String
        Private _ordCompra As String

        Public Property type() As Integer
            Get
                Return _type
            End Get
            Set(ByVal value As Integer)
                If value = 1 Then
                    _type = 1
                ElseIf value = 2 Then
                    _type = 2
                Else
                    _type = 3
                End If

                '_type = value
            End Set
        End Property
        Public Property number() As Integer
            Get
                Return _number
            End Get
            Set(ByVal value As Integer)
                _number = value
            End Set
        End Property
        Public Property monto_decuento() As Decimal
            Get
                Return _monto_decuento
            End Get
            Set(ByVal value As Decimal)
                _monto_decuento = value
            End Set
        End Property
        Public Property pct_decuento() As Decimal
            Get
                Return _pct_decuento
            End Get
            Set(ByVal value As Decimal)
                _pct_decuento = value
            End Set
        End Property
        Public Property uns() As Decimal
            Get
                Return _uns
            End Get
            Set(ByVal value As Decimal)
                _uns = value
            End Set
        End Property
        Public Property precio() As Decimal
            Get
                Return _precio
            End Get
            Set(ByVal value As Decimal)
                _precio = value
            End Set
        End Property
        Public Property sku() As String
            Get
                Return _sku
            End Get
            Set(ByVal value As String)
                _sku = value
            End Set
        End Property
        Public Property partida() As Integer
            Get
                Return _partida
            End Get
            Set(ByVal value As Integer)
                _partida = value
            End Set
        End Property
        Public Property reference_identification() As String
            Get
                Return _reference_identification
            End Get
            Set(ByVal value As String)
                _reference_identification = value
            End Set
        End Property
        Public Property art_tip() As String
            Get
                Return _art_tip
            End Get
            Set(ByVal value As String)
                _art_tip = value
            End Set
        End Property
        Public Property uni_med() As String
            Get
                Return _uni_med
            End Get
            Set(ByVal value As String)
                _uni_med = value
            End Set
        End Property
        Public Property OrdCompra() As String
            Get
                Return _ordCompra
            End Get
            Set(ByVal value As String)
                _ordCompra = value
            End Set
        End Property

        Public Sub lineitem()
        End Sub
        Public Sub lineitem(ByVal type As String, ByVal number As Integer, _
                             ByVal monto_descuento As Decimal, ByVal pct_descuento As Decimal, _
                             ByVal uns As Decimal, ByVal precio As Decimal, _
                             ByVal sku As String, ByVal partida As Integer, _
                             ByVal reference_identification As String, ByVal art_tip As String, _
                             ByVal uni_med As String,
                             ByVal OrdCompra As String)
            Me.type = _type
            Me.number = _number
            Me.monto_decuento = _monto_decuento
            Me.pct_decuento = _pct_decuento
            Me.uns = _uns
            Me.precio = _precio
            Me.sku = _sku
            Me.partida = _partida
            Me.reference_identification = _reference_identification
            Me.art_tip = _art_tip
            Me.uni_med = _uni_med
            Me.OrdCompra = _ordCompra
        End Sub

    End Class



    Public Class Document
        Private _referenceIdentification As String

        Public Property referenceIdentification() As String
            Get
                Return _referenceIdentification
            End Get

            Set(ByVal value As String)
                _referenceIdentification = value
            End Set
        End Property
        Public Sub Document()
        End Sub
        Public Sub Document(ByVal referenceIdentification As String)
            Me.referenceIdentification = _referenceIdentification
        End Sub

    End Class

    Public Class requestforpayment

        Private _currency As currency
        Private _aditional_data As aditionaldata
        Private _paymenttimeperiod As paymenttimeperiod
        Private _provider As provider
        Private _line_items As List(Of lineitem)
        Private _document As Document
        Public Property currency() As currency
            Get
                Return _currency
            End Get
            Set(ByVal value As currency)
                _currency = value
            End Set
        End Property
        Public Property aditional_data() As aditionaldata
            Get
                Return _aditional_data
            End Get
            Set(ByVal value As aditionaldata)
                _aditional_data = value
            End Set
        End Property
        Public Property provider() As provider
            Get
                Return _provider
            End Get
            Set(ByVal value As provider)
                _provider = value
            End Set
        End Property
        Public Property line_items() As List(Of lineitem)
            Get
                Return _line_items
            End Get
            Set(ByVal value As List(Of lineitem))
                _line_items = value
            End Set
        End Property

        Public Property paymenttimeperiod() As paymenttimeperiod
            Get
                Return _paymenttimeperiod
            End Get
            Set(ByVal value As paymenttimeperiod)
                _paymenttimeperiod = value
            End Set
        End Property
        Public Property document() As Document
            Get
                Return _document
            End Get
            Set(ByVal value As Document)
                _document = value
            End Set
        End Property

        Public Sub requestforpayment()

        End Sub
        Public Sub requestforpayment(ByVal currency As currency, ByVal aditional_data As aditionaldata, ByVal paymenttimeperiod As paymenttimeperiod, ByVal provider As provider, ByVal line_items As List(Of lineitem))
            Me.currency = _currency
            Me.aditional_data = _aditional_data
            Me.paymenttimeperiod = _paymenttimeperiod
            Me.provider = _provider
            Me.line_items = _line_items
            Me.document = _document

        End Sub
    End Class

    Public Class llave_cfd


        Private _rfc_emisor As String
        Private _serie As String
        Private _folio_factura As Int64
        Private _version As String
        Private _version_nom As String
        Private _timbre_fiscal As timbre_fiscal_digital
        Private _iStatus As Integer
        Private _ef_cve As String
        Private _nom_arch As String
        Private _sw_sin_addenda As Integer
        Private _sw_tmp As Integer
        Private _proveedor As String
        Private _id_soludin As String ' es el usuario que sube las facturas de caja chica

        Public Property serie() As String
            Get
                Return _serie
            End Get
            Set(ByVal value As String)
                _serie = value
            End Set
        End Property
        Public Property folio_factura() As Int64
            Get
                Return _folio_factura
            End Get
            Set(ByVal value As Int64)
                _folio_factura = value
            End Set
        End Property
        Public Property rfc_emisor() As String
            Get
                Return _rfc_emisor
            End Get
            Set(ByVal value As String)
                _rfc_emisor = value
            End Set
        End Property
        Public Property version() As String
            Get
                Return _version
            End Get
            Set(ByVal value As String)
                _version = value
            End Set
        End Property
        Public Property version_nom() As String
            Get
                Return _version_nom
            End Get
            Set(ByVal value As String)
                _version_nom = value
            End Set
        End Property
        Public Property timbre_fiscal() As timbre_fiscal_digital
            Get
                Return _timbre_fiscal
            End Get
            Set(ByVal value As timbre_fiscal_digital)
                _timbre_fiscal = value
            End Set
        End Property
        Public Property iStatus() As Integer
            Get
                Return _iStatus
            End Get
            Set(ByVal value As Integer)
                _iStatus = value
            End Set
        End Property
        Public Property ef_cve() As String
            Get
                Return _ef_cve
            End Get
            Set(ByVal value As String)
                _ef_cve = value
            End Set
        End Property
        Public Property nom_arch() As String
            Get
                Return _nom_arch
            End Get
            Set(ByVal value As String)
                _nom_arch = value
            End Set
        End Property
        Public Property sw_sin_addenda() As Integer
            Get
                Return _sw_sin_addenda
            End Get
            Set(ByVal value As Integer)
                _sw_sin_addenda = value
            End Set
        End Property

        Public Property sw_tmp() As Integer
            Get
                Return _sw_tmp
            End Get
            Set(ByVal value As Integer)
                _sw_tmp = value
            End Set
        End Property

        Public Property Proveedor() As String
            Get
                Return _proveedor
            End Get
            Set(ByVal value As String)
                _proveedor = value
            End Set
        End Property

        Public Property IdSoludin() As String
            Get
                Return _id_soludin
            End Get
            Set(value As String)
                _id_soludin = value
            End Set
        End Property

        Public Sub llave_cfd()

        End Sub
        Public Sub llave_cfd(ByVal rfc_emisor As String, _
                             ByVal seria As String, _
                             ByVal folio_factura As Integer, _
                             ByVal version As String, _
                             ByVal version_nom As String, _
                             ByVal timbre_fiscal As String, _
                             ByVal iStatus As Integer, _
                             ByVal ef_cve As String, _
                             ByVal nom_arch As String, _
                             ByVal sw_sin_addenda As Integer, _
                             ByVal sw_tmp As Integer, _
                             ByVal Proveedor As String, _
                             ByVal IdSoludin As String)
            Me.rfc_emisor = _rfc_emisor
            Me.serie = _serie
            Me.folio_factura = _folio_factura
            Me.version = _version
            Me.version_nom = _version_nom
            Me.timbre_fiscal = _timbre_fiscal
            Me.iStatus = _iStatus
            Me.ef_cve = _ef_cve
            Me.nom_arch = _nom_arch
            Me.sw_sin_addenda = _sw_sin_addenda
            Me.sw_tmp = _sw_tmp
            Me.Proveedor = _proveedor
            Me.IdSoludin = _id_soludin
        End Sub
    End Class

    Public Class addenda

        Private _requestforpayment As requestforpayment

        Public Property requestforpayment() As requestforpayment
            Get
                Return _requestforpayment
            End Get
            Set(ByVal value As requestforpayment)
                _requestforpayment = value
            End Set
        End Property
        Public Sub addenda()

        End Sub
        Public Sub addenda(ByVal requestforpayment As requestforpayment)
            Me.requestforpayment = _requestforpayment
        End Sub
    End Class

    Public Class Comprobante
        Private _nom_arch As String
        Private _version As String
        Private _serie As String
        Private _folio_factura As Int64
        Private _fecha_factura As String
        Private _sello As String
        Private _no_aprobacion As Integer
        Private _ano_aprobacion As Integer
        Private _no_certificado As String
        Private _certificado As String
        Private _condiciones_pago As String
        Private _metodo_pago As String
        Private _sub_total As Decimal
        Private _descuento As Decimal
        Private _total As Decimal
        Private _rfc_emisor As String
        Private _rfc_receptor As String
        Private _cc_cve As String
        Private _cc_tipo As String
        Private _tipodoc_cve As String
        Private _tm As String
        Private _Emisor As emisor
        Private _Receptor As receptor
        Private _Conceptos As List(Of concepto)
        Private _Impuestos As impuestos
        Private _Addenda As addenda
        Private _moneda As String
        Private _nombreProveedor As String
        Private _TotaldeTraslados As Decimal
        Private _tc As String
        Private _TotaldeRetenciones As Decimal
        Private _totalImpuestosTrasladados As Decimal
        Private _totalPorcentajeIVA As Decimal
        Private _totalIEPS As Decimal
        Private _pctIva As Decimal
        Private _total_Valido As Decimal
        Private _swTImporte As Boolean
        Private _swImpTras As Boolean


        Public Property nom_arch() As String
            Get
                Return _nom_arch
            End Get
            Set(ByVal value As String)
                _nom_arch = value
            End Set
        End Property
        Public Property version() As String
            Get
                Return _version
            End Get
            Set(ByVal value As String)
                _version = value
            End Set
        End Property
        Public Property serie() As String
            Get
                Return _serie
            End Get
            Set(ByVal value As String)
                _serie = value
            End Set
        End Property
        Public Property folio_factura() As Int64
            Get
                Return _folio_factura
            End Get
            Set(ByVal value As Int64)
                _folio_factura = value
            End Set
        End Property
        Public Property fecha_factura() As String
            Get
                Return _fecha_factura
            End Get
            Set(ByVal value As String)
                _fecha_factura = value
            End Set
        End Property
        Public Property sello() As String
            Get
                Return _sello
            End Get
            Set(ByVal value As String)
                _sello = value
            End Set
        End Property
        Public Property no_aprobacion() As Integer
            Get
                Return _no_aprobacion
            End Get
            Set(ByVal value As Integer)
                _no_aprobacion = value
            End Set
        End Property
        Public Property ano_aprobacion() As Integer
            Get
                Return _ano_aprobacion
            End Get
            Set(ByVal value As Integer)
                _ano_aprobacion = value
            End Set
        End Property
        Public Property no_certificado() As String
            Get
                Return _no_certificado
            End Get
            Set(ByVal value As String)
                _no_certificado = value
            End Set
        End Property
        Public Property certificado() As String
            Get
                Return _certificado
            End Get
            Set(ByVal value As String)
                _certificado = value
            End Set
        End Property
        Public Property condiciones_pago() As String
            Get
                Return _condiciones_pago
            End Get
            Set(ByVal value As String)
                _condiciones_pago = value
            End Set
        End Property
        Public Property metodo_pago() As String
            Get
                Return _metodo_pago
            End Get
            Set(ByVal value As String)
                _metodo_pago = value
            End Set
        End Property
        Public Property sub_total() As Decimal
            Get
                Return _sub_total
            End Get
            Set(ByVal value As Decimal)
                _sub_total = value
            End Set
        End Property
        Public Property descuento() As Decimal
            Get
                Return _descuento
            End Get
            Set(ByVal value As Decimal)
                _descuento = value
            End Set
        End Property
        Public Property total() As Decimal
            Get
                Return _total
            End Get
            Set(ByVal value As Decimal)
                _total = value
            End Set
        End Property
        Public Property rfc_emisor() As String
            Get
                Return _rfc_emisor
            End Get
            Set(ByVal value As String)
                _rfc_emisor = value
            End Set
        End Property
        Public Property rfc_receptor() As String
            Get
                Return _rfc_receptor
            End Get
            Set(ByVal value As String)
                _rfc_receptor = value
            End Set
        End Property
        Public Property cc_cve() As String
            Get
                Return _cc_cve
            End Get
            Set(ByVal value As String)
                _cc_cve = value
            End Set
        End Property
        Public Property cc_tipo() As String
            Get
                Return _cc_tipo
            End Get
            Set(ByVal value As String)
                _cc_tipo = value
            End Set
        End Property
        Public Property tipodoc_cve() As String
            Get
                Return _tipodoc_cve
            End Get
            Set(ByVal value As String)
                _tipodoc_cve = value
            End Set
        End Property
        Public Property tm() As String
            Get
                Return _tm
            End Get
            Set(ByVal value As String)
                _tm = value
            End Set
        End Property
        Public Property Emisor() As emisor
            Get
                Return _Emisor
            End Get
            Set(ByVal value As emisor)
                _Emisor = value
            End Set
        End Property
        Public Property Receptor() As receptor
            Get
                Return _Receptor
            End Get
            Set(ByVal value As receptor)
                _Receptor = value
            End Set
        End Property
        Public Property Conceptos() As List(Of concepto)
            Get
                Return _Conceptos
            End Get
            Set(ByVal value As List(Of concepto))
                _Conceptos = value
            End Set
        End Property
        Public Property Impuestos() As impuestos
            Get
                Return _Impuestos
            End Get
            Set(ByVal value As impuestos)
                _Impuestos = value
            End Set
        End Property
        Public Property Addenda() As addenda
            Get
                Return _Addenda
            End Get
            Set(ByVal value As addenda)
                _Addenda = value
            End Set
        End Property
        Public Property moneda() As String
            Get
                Return _moneda
            End Get
            Set(ByVal value As String)
                _moneda = value
            End Set
        End Property
        Public Property NombreProveedor() As String
            Get
                Return _nombreProveedor
            End Get
            Set(value As String)
                _nombreProveedor = value
            End Set
        End Property
        Public Property TotaldeTraslados() As Decimal
            Get
                Return _TotaldeTraslados
            End Get
            Set(ByVal value As Decimal)
                _TotaldeTraslados = value
            End Set
        End Property
        Public Property tc() As String
            Get
                Return _tc
            End Get
            Set(ByVal value As String)
                _tc = value
            End Set
        End Property
        Public Property TotaldeRetenciones() As Decimal
            Get
                Return _TotaldeRetenciones
            End Get
            Set(ByVal value As Decimal)
                _TotaldeRetenciones = value
            End Set
        End Property
        Public Property totalImpuestosTrasladados() As Decimal
            Get
                Return _totalImpuestosTrasladados
            End Get
            Set(ByVal value As Decimal)
                _totalImpuestosTrasladados = value
            End Set
        End Property
        Public Property porcentajeIVA() As Decimal
            Get
                Return _totalPorcentajeIVA
            End Get
            Set(ByVal value As Decimal)
                _totalPorcentajeIVA = value
            End Set
        End Property
        Public Property totalIEPS() As Decimal
            Get
                Return _totalIEPS
            End Get
            Set(ByVal value As Decimal)
                _totalIEPS = value
            End Set
        End Property
        Public Property pctIva() As Decimal
            Get
                Return _pctIva
            End Get
            Set(ByVal value As Decimal)
                _pctIva = value
            End Set
        End Property
        Public Property total_Valido() As Decimal
            Get
                Return _total_Valido
            End Get
            Set(ByVal value As Decimal)
                _total_Valido = value
            End Set
        End Property
        Public Property swTImporte() As Boolean
            Get
                Return _swTImporte
            End Get
            Set(ByVal value As Boolean)
                _swTImporte = value
            End Set
        End Property
        Public Property swImpTras() As Boolean
            Get
                Return _swImpTras
            End Get
            Set(ByVal value As Boolean)
                _swImpTras = value
            End Set
        End Property
        Public Sub Comprobante()
        End Sub

        Public Sub Comprobante(ByVal no_arch As String, ByVal version As String, _
                               ByVal serie As String, ByVal folio_factura As String, _
                               ByVal fecha_factura As String, ByVal sello As String, _
                               ByVal no_aprobacion As String, ByVal ano_aprobacion As String, _
                               ByVal no_certificado As String, ByVal certificado As String, _
                               ByVal condiciones_pago As String, ByVal metodo_pago As String, _
                               ByVal sub_total As Decimal, ByVal descuento As Decimal, _
                               ByVal total As Decimal, ByVal rfc_emisor As Decimal, _
                               ByVal rfc_receptor As String, ByVal cc_cve As String, _
                               ByVal cc_tipo As String, ByVal tipdoc_cve As String, ByVal tm As String, _
                               ByVal Emisor As emisor, _
                               ByVal Receptor As receptor, ByVal Conceptos As List(Of concepto), _
                               ByVal Impuestos As impuestos, ByVal Addenda As addenda, _
                               ByVal moneda As String,
                               ByVal NombreProveedor As String, _
                               ByVal TotaldeTraslados As String, _
                               ByVal tc As String, _
                               ByVal TotaldeRetenciones As String, _
                               ByVal totalImpuestosTrasladados As String, _
                               ByVal pctIva As String, _
                               ByVal total_Valido As String, _
                               ByVal swTImporte As String, _
                               ByVal swImpTras As String
                               )

            Me.nom_arch = _nom_arch
            Me.version = _version
            Me.serie = _serie
            Me.folio_factura = _folio_factura
            Me.fecha_factura = _fecha_factura
            Me.sello = _sello
            Me.no_aprobacion = _no_aprobacion
            Me.ano_aprobacion = _ano_aprobacion
            Me.no_certificado = _no_certificado
            Me.certificado = _certificado
            Me.condiciones_pago = _condiciones_pago
            Me.metodo_pago = _metodo_pago
            Me.sub_total = _sub_total
            Me.descuento = _descuento
            Me.total = _total
            Me.rfc_emisor = _rfc_emisor
            Me.rfc_receptor = _rfc_receptor
            Me.cc_cve = _cc_cve
            Me.cc_tipo = _cc_tipo
            Me.tipodoc_cve = _tipodoc_cve
            Me.tm = _tm
            Me.Emisor = _Emisor
            Me.Receptor = _Receptor
            Me.Conceptos = _Conceptos
            Me.Impuestos = _Impuestos
            Me.Addenda = _Addenda
            Me.moneda = _moneda
            Me.NombreProveedor = _nombreProveedor
            Me.TotaldeTraslados = _TotaldeTraslados
            Me.tc = _tc
            Me.TotaldeRetenciones = _TotaldeRetenciones
            Me.totalImpuestosTrasladados = _totalImpuestosTrasladados
            Me.pctIva = _pctIva
            Me.total_Valido = _total_Valido
            Me.swTImporte = _swTImporte
            Me.swImpTras = _swImpTras

        End Sub

    End Class

    Public Class ClaveProveedor
        Private _cc_cve As String
        Private _cc_tipo As String
        Private _nombreProveedor As String

        Public Property CcCve() As String
            Get
                Return _cc_cve
            End Get
            Set(value As String)
                _cc_cve = value
            End Set
        End Property
        Public Property CcTipo() As String
            Get
                Return _cc_tipo
            End Get
            Set(value As String)
                _cc_tipo = value
            End Set
        End Property
        Public Property NombreProveedor() As String
            Get
                Return _nombreProveedor
            End Get
            Set(value As String)
                _nombreProveedor = value
            End Set
        End Property

        Public Sub ClaveProveedor()
        End Sub
        Public Sub ClaveProveedor(ByVal CcCve As String,
                                  ByVal CcTipo As String,
                                  ByVal NombreProveedor As String)
            Me.CcCve = _cc_cve
            Me.CcTipo = _cc_tipo
            Me.NombreProveedor = _nombreProveedor
        End Sub

    End Class


    Public Class nuevas_facturas


        Private _ef_cve As String
        Private _tipo_doc As String
        Private _num_fol As Integer
        Private _nom_arch As String


        Public Property ef_cve() As String
            Get
                Return _ef_cve
            End Get
            Set(ByVal value As String)
                _ef_cve = value
            End Set
        End Property
        Public Property num_fol() As Integer
            Get
                Return _num_fol
            End Get
            Set(ByVal value As Integer)
                _num_fol = value
            End Set
        End Property
        Public Property tipo_doc() As String
            Get
                Return _tipo_doc
            End Get
            Set(ByVal value As String)
                _tipo_doc = value
            End Set
        End Property
        Public Property nom_arch() As String
            Get
                Return _nom_arch
            End Get
            Set(ByVal value As String)
                _nom_arch = value
            End Set
        End Property


        Public Sub nuevas_facturas()

        End Sub
        Public Sub nuevas_facturas(ByVal ef_cve As String, ByVal tipo_doc As String, ByVal num_fol As Integer, ByVal nom_arch As String)
            Me.ef_cve = _ef_cve
            Me.tipo_doc = _tipo_doc
            Me.num_fol = _num_fol
            Me.nom_arch = _nom_arch
        End Sub
    End Class

    Public Class timbre_fiscal_digital
        Private _version As String
        Public Property version() As String
            Get
                Return _version
            End Get
            Set(ByVal value As String)
                _version = value
            End Set
        End Property
        Private _uuid As String
        Public Property uuid() As String
            Get
                Return _uuid
            End Get
            Set(ByVal value As String)
                _uuid = value
            End Set
        End Property
        Private _fecha_timbrado As String
        Public Property fecha_timbrado() As String
            Get
                Return _fecha_timbrado
            End Get
            Set(ByVal value As String)
                _fecha_timbrado = value
            End Set
        End Property
        Private _sello_cfd As String
        Public Property sello_cfd() As String
            Get
                Return _sello_cfd
            End Get
            Set(ByVal value As String)
                _sello_cfd = value
            End Set
        End Property
        Private _no_certificado_sat As String
        Public Property no_certificado_sat() As String
            Get
                Return _no_certificado_sat
            End Get
            Set(ByVal value As String)
                _no_certificado_sat = value
            End Set
        End Property
        Private _sello_sat As String
        Public Property sello_sat() As String
            Get
                Return _sello_sat
            End Get
            Set(ByVal value As String)
                _sello_sat = value
            End Set
        End Property
        Public Sub timbre_sical_digital()

        End Sub
        Public Sub timbre_fiscal_digital(ByVal version As String, ByVal uuid As String, ByVal fecha_timbrado As String, ByVal sello_cfd As String, ByVal no_sertificado_sat As String, ByVal sello_sat As String)

            Me.version = _version
            Me.uuid = _uuid
            Me.fecha_timbrado = _fecha_timbrado
            Me.sello_cfd = _sello_cfd
            Me.no_certificado_sat = _no_certificado_sat
            Me.sello_sat = _sello_sat

        End Sub
    End Class


    Public Class Archivos
        Private _archivoXml As String
        Private _archivoPdf As String

        Sub New()
            ' TODO: Complete member initialization 
        End Sub

        Public Property ArchivoXml() As String
            Get
                Return _archivoXml
            End Get
            Set(value As String)
                _archivoXml = value
            End Set
        End Property

        Public Property ArchivoPdf() As String
            Get
                Return _archivoPdf
            End Get
            Set(value As String)
                _archivoPdf = value
            End Set
        End Property
        'Public Sub NombreArchivos()
        '    _archivoXml = ArchivoXml
        '    _archivoPdf = ArchivoPdf
        'End Sub
        'Public Sub New(ByVal archivoXml As String, ByVal archivoPdf As String)
        '    Me.ArchivoXml = archivoXml
        '    Me.ArchivoPdf = archivoPdf
        'End Sub

    End Class


    Public Class GrupoComprobantes
        Private _iNumFactura As Integer
        Private _llave As llave_cfd
        Private _documentoCfd As Comprobante
        Private _erroresList As List(Of Errores)
        Private _facturaGenerada As List(Of nuevas_facturas)
        Private _archivosCmp As Archivos

        Sub New()
            ' TODO: Complete member initialization 
        End Sub


        Public Property InumFactura As Integer
            Get
                Return _iNumFactura
            End Get
            Set(value As Integer)
                _iNumFactura = value
            End Set
        End Property
        Public Property Llave() As llave_cfd
            Get
                Return _llave
            End Get
            Set(value As llave_cfd)
                _llave = value
            End Set
        End Property
        Public Property DocumentoCfd() As Comprobante
            Get
                Return _documentoCfd
            End Get
            Set(value As Comprobante)
                _documentoCfd = value
            End Set
        End Property
        Public Property ErroresList() As List(Of Errores)
            Get
                Return _erroresList
            End Get
            Set(value As List(Of Errores))
                _erroresList = value
            End Set
        End Property
        Public Property FacturaGenerada() As List(Of nuevas_facturas)
            Get
                Return _facturaGenerada
            End Get
            Set(value As List(Of nuevas_facturas))
                _facturaGenerada = value
            End Set
        End Property
        Public Property ArchivosCmp As Archivos
            Get
                Return _archivosCmp
            End Get
            Set(value As Archivos)
                _archivosCmp = value
            End Set
        End Property
        Public Sub AlmacenaComprobantes()
            _iNumFactura = InumFactura
            _llave = Llave
            _documentoCfd = DocumentoCfd
            _erroresList = ErroresList
            _facturaGenerada = FacturaGenerada
            _archivosCmp = ArchivosCmp
        End Sub
        Public Sub New(ByVal iNumFactura As Integer, _
                       ByVal llave As llave_cfd, _
                       ByVal documentoCfd As Comprobante, _
                       ByVal erroresList As List(Of Errores), _
                       ByVal facturaGenerada As List(Of nuevas_facturas), _
                       ByVal archivosCmp As Archivos)
            Me.InumFactura = iNumFactura
            Me.Llave = llave
            Me.DocumentoCfd = documentoCfd
            Me.ErroresList = erroresList
            Me.FacturaGenerada = facturaGenerada
            Me.ArchivosCmp = archivosCmp
        End Sub


    End Class




    Public Class combos


        Private _rfc As String
        Private _combo As String
        Private _parametro1 As String
        Private _parametro2 As String

        Public Property rfc() As String
            Get
                Return _rfc
            End Get
            Set(ByVal value As String)
                _rfc = value
            End Set
        End Property
        Public Property combo() As String
            Get
                Return _combo
            End Get
            Set(ByVal value As String)
                _combo = value
            End Set
        End Property
        Public Property parametro1() As String
            Get
                Return _parametro1
            End Get
            Set(ByVal value As String)
                _parametro1 = value
            End Set
        End Property
        Public Property parametro2() As String
            Get
                Return _parametro2
            End Get
            Set(ByVal value As String)
                _parametro2 = value
            End Set
        End Property

        Public Sub combos()

        End Sub
        Public Sub combos(ByVal rfc As String, ByVal combo As String, ByVal parametro1 As String, ByVal parametro2 As String)
            Me.rfc = _rfc
            Me.combo = _combo
            Me.parametro1 = _parametro1
            Me.parametro2 = _parametro2
        End Sub
    End Class



    Public Class comboItem

        Private _decripcion As String
        Private _valor As String

        Public Property Descripcion() As String
            Get
                Return _decripcion
            End Get
            Set(ByVal value As String)
                _decripcion = value
            End Set
        End Property
        Public Property Valor() As String
            Get
                Return _valor
            End Get
            Set(ByVal value As String)
                _valor = value
            End Set
        End Property

        Public Sub comboItem()

        End Sub
        Public Sub comboItem(ByVal Descripcion As String, ByVal Valor As String)
            Me.Descripcion = _decripcion
            Me.Valor = _valor
        End Sub
    End Class



#End Region
End Namespace

