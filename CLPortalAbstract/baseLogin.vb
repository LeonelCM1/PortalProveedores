Namespace AbstractLayer
    Public MustInherit Class baseLogin
        Private _UserName As String
        Private _Nombre As String
        'Nombre del Usuario
        Public Property UserName() As String
            Get
                Return _UserName
            End Get
            Set(ByVal value As String)
                If value <> _UserName Then
                    _UserName = value
                End If
            End Set
        End Property
        'Entidad Financiera
        Public Property Nombre() As String
            Get
                Return _Nombre
            End Get
            Set(ByVal value As String)
                If value <> _Nombre Then
                    _Nombre = value
                End If
            End Set
        End Property
    End Class
    Public MustInherit Class baseRecepcion
        Private _cc_cve As String
        Private _fec_mov As String
        Private _refer As String
        Private _div_cve As String
        Private _dato4 As String
        Private _fec_prom As String
        Private _suc_aten As String
        Private _pct_desc As String
        Private _alm_cve As String
        Private _foremb_cve As String
        Private _contenedor As String
        Private _plazo As String
        Private _sw_impexp As String
        Private _pct_canacp As String
        Private _sw_ent_par As String
        Private _tm As String
        Private _tc As String
        Private _cf_flete As String
        Private _seguro As String
        Private _sw_transp As String
        Private _forpag_cve As String
        Private _dest_cve As String
        Private _obs As String
        Private _ef_cve As String
        Private _tipo_doc As String
        Private _cc_tipo As String
        Private _id_ultact As String
        Private _ef_cvep As String
        Private _tipo_docp As String
        Private _num_folp As String
        Private _forem_tipo As String

        Public Property cc_cve() As String
            Get
                Return _cc_cve
            End Get
            Set(ByVal value As String)
                If value <> _cc_cve Then
                    _cc_cve = value
                End If
            End Set
        End Property
        Public Property fec_mov() As String
            Get
                Return _fec_mov
            End Get
            Set(ByVal value As String)
                If value <> _fec_mov Then
                    _fec_mov = value
                End If
            End Set
        End Property
        Public Property refer() As String
            Get
                Return _refer
            End Get
            Set(ByVal value As String)
                If value <> _refer Then
                    _refer = value
                End If
            End Set
        End Property
        Public Property div_cve() As String
            Get
                Return _div_cve
            End Get
            Set(ByVal value As String)
                If value <> _div_cve Then
                    _div_cve = value
                End If
            End Set
        End Property
        Public Property dato4() As String
            Get
                Return _dato4
            End Get
            Set(ByVal value As String)
                If value <> _dato4 Then
                    _dato4 = value
                End If
            End Set
        End Property
        Public Property fec_prom() As String
            Get
                Return _fec_prom
            End Get
            Set(ByVal value As String)
                If value <> _fec_prom Then
                    _fec_prom = value
                End If
            End Set
        End Property
        Public Property suc_aten() As String
            Get
                Return _suc_aten
            End Get
            Set(ByVal value As String)
                If value <> _suc_aten Then
                    _suc_aten = value
                End If
            End Set
        End Property
        Public Property pct_desc() As String
            Get
                Return _pct_desc
            End Get
            Set(ByVal value As String)
                If value <> _pct_desc Then
                    _pct_desc = value
                End If
            End Set
        End Property
        Public Property alm_cve() As String
            Get
                Return _alm_cve
            End Get
            Set(ByVal value As String)
                If value <> _alm_cve Then
                    _alm_cve = value
                End If
            End Set
        End Property
        Public Property foremb_cve() As String
            Get
                Return _foremb_cve
            End Get
            Set(ByVal value As String)
                If value <> _foremb_cve Then
                    _foremb_cve = value
                End If
            End Set
        End Property
        Public Property contenedor() As String
            Get
                Return _contenedor
            End Get
            Set(ByVal value As String)
                If value <> _contenedor Then
                    _contenedor = value
                End If
            End Set
        End Property
        Public Property plazo() As String
            Get
                Return _plazo
            End Get
            Set(ByVal value As String)
                If value <> _plazo Then
                    _plazo = value
                End If
            End Set
        End Property
        Public Property sw_impexp() As String
            Get
                Return _sw_impexp
            End Get
            Set(ByVal value As String)
                If value <> _sw_impexp Then
                    _sw_impexp = value
                End If
            End Set
        End Property
        Public Property pct_canacp() As String
            Get
                Return _pct_canacp
            End Get
            Set(ByVal value As String)
                If value <> _pct_canacp Then
                    _pct_canacp = value
                End If
            End Set
        End Property
        '----------------------
        Public Property sw_ent_par() As String
            Get
                Return _sw_ent_par
            End Get
            Set(ByVal value As String)
                If value <> _sw_ent_par Then
                    _sw_ent_par = value
                End If
            End Set
        End Property
        Public Property tm() As String
            Get
                Return _tm
            End Get
            Set(ByVal value As String)
                If value <> _tm Then
                    _tm = value
                End If
            End Set
        End Property
        Public Property tc() As String
            Get
                Return _tc
            End Get
            Set(ByVal value As String)
                If value <> _tc Then
                    _tc = value
                End If
            End Set
        End Property
        Public Property cf_flete() As String
            Get
                Return _cf_flete
            End Get
            Set(ByVal value As String)
                If value <> _cf_flete Then
                    _cf_flete = value
                End If
            End Set
        End Property
        Public Property seguro() As String
            Get
                Return _seguro
            End Get
            Set(ByVal value As String)
                If value <> _seguro Then
                    _seguro = value
                End If
            End Set
        End Property

        Public Property sw_transp() As String
            Get
                Return _sw_transp
            End Get
            Set(ByVal value As String)
                If value <> _sw_transp Then
                    _sw_transp = value
                End If
            End Set
        End Property
        Public Property forpag_cve() As String
            Get
                Return _forpag_cve
            End Get
            Set(ByVal value As String)
                If value <> _forpag_cve Then
                    _forpag_cve = value
                End If
            End Set
        End Property
        Public Property dest_cve() As String
            Get
                Return _dest_cve
            End Get
            Set(ByVal value As String)
                If value <> _dest_cve Then
                    _dest_cve = value
                End If
            End Set
        End Property
        Public Property obs() As String
            Get
                Return _obs
            End Get
            Set(ByVal value As String)
                If value <> _obs Then
                    _obs = value
                End If
            End Set
        End Property
        Public Property ef_cve() As String
            Get
                Return _ef_cve
            End Get
            Set(ByVal value As String)
                If value <> _ef_cve Then
                    _ef_cve = value
                End If
            End Set
        End Property
        '------------------------------------


        Public Property tipo_doc() As String
            Get
                Return _tipo_doc
            End Get
            Set(ByVal value As String)
                If value <> _tipo_doc Then
                    _tipo_doc = value
                End If
            End Set
        End Property
        Public Property cc_tipo() As String
            Get
                Return _cc_tipo
            End Get
            Set(ByVal value As String)
                If value <> _cc_tipo Then
                    _cc_tipo = value
                End If
            End Set
        End Property
        Public Property id_ultact() As String
            Get
                Return _id_ultact
            End Get
            Set(ByVal value As String)
                If value <> _id_ultact Then
                    _id_ultact = value
                End If
            End Set
        End Property
        Public Property ef_cvep() As String
            Get
                Return _ef_cvep
            End Get
            Set(ByVal value As String)
                If value <> _ef_cvep Then
                    _ef_cvep = value
                End If
            End Set
        End Property
        Public Property tipo_docp() As String
            Get
                Return _tipo_docp
            End Get
            Set(ByVal value As String)
                If value <> _tipo_docp Then
                    _tipo_docp = value
                End If
            End Set
        End Property
        Public Property num_folp() As String
            Get
                Return _num_folp
            End Get
            Set(ByVal value As String)
                If value <> _num_folp Then
                    _num_folp = value
                End If
            End Set
        End Property
        Public Property forem_tipo() As String
            Get
                Return _forem_tipo
            End Get
            Set(ByVal value As String)
                If value <> _forem_tipo Then
                    _forem_tipo = value
                End If
            End Set
        End Property
    End Class
    Public MustInherit Class baseTypeReception
        Private _ef_cve As String
        Private _nom2 As String
        'Nombre del Usuario
        Public Property Ef_cve() As String
            Get
                Return _ef_cve
            End Get
            Set(ByVal value As String)
                If value <> _ef_cve Then
                    _ef_cve = value
                End If
            End Set
        End Property
        'Entidad Financiera
        Public Property Nom2() As String
            Get
                Return _nom2
            End Get
            Set(ByVal value As String)
                If value <> _nom2 Then
                    _nom2 = value
                End If
            End Set
        End Property
    End Class
End Namespace


