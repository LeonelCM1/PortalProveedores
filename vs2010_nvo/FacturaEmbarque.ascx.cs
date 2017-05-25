using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Skytex.FacturaElectronica;
using CLPortalAbstract;

public partial class FacturaEmbarque : System.Web.UI.UserControl
{
    private readonly Factura _factura = new Factura();
    string _ruta = "";
    string _rutaTmp = "";
    private string _emails = "";
    int _numDec = 0;
    decimal _pctRetencion = 0;
    decimal _rango = 0;
    string _decimales;
    decimal _subtotal = 0;
    decimal _descuento = 0;
    decimal _total = 0;
    decimal _importe = 0;
    decimal _totalTrsl = 0;
    decimal _totalRet = 0;
    decimal _dTotal = 0;
    bool _swRetProv = false;
    Datos _dat = new Datos();
    llave_cfd _llaveCfd = new llave_cfd();
    timbre_fiscal_digital _timbreFiscal = new timbre_fiscal_digital();
    Comprobante _comprobante = new Comprobante();
   

    private void LoadConfig()
    {
        var config = (List<ConfigGral>)HttpContext.Current.Session["config"];
        _ruta = (string)_factura.GetConfigure(config, "Ruta");
        _rutaTmp = (string)_factura.GetConfigure(config, "RutaTMP");
        _emails = (string)_factura.GetConfigure(config, "Email_Admin");
        var decimalesTmp = (string)_factura.GetConfigure(config, "decimales");
        var decimalesTmp2 = decimal.Parse(decimalesTmp);
        _numDec = (int)decimalesTmp2;
        //_pctRetencion = (decimal)_factura.GetConfigure(config, "retencion");
        //_rango = (decimal)_factura.GetConfigure(config, "rangoMinMax");
        decimalesTmp = (string)_factura.GetConfigure(config, "retencion");
        _pctRetencion = decimal.Parse(decimalesTmp);
        decimalesTmp = (string)_factura.GetConfigure(config, "rangoMinMax");
        _rango = decimal.Parse(decimalesTmp);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["dtTransportes"] == null)
            LlenaGridTransportes();

        if (!IsPostBack)
        {
            if (HttpContext.Current.Session["RFC"] == null)
            { Response.Redirect("Login.aspx"); return; }

            if (_llaveCfd.rfc_emisor == null || _comprobante == null) { LlenaLlave(); }
            HttpContext.Current.Session["LlaveCFD"] = _llaveCfd;
            HttpContext.Current.Session["msgs"] = "";
            LlenaInfo();
            
            hfRfc.Value = _llaveCfd.rfc_emisor;
            hfSerie.Value = _llaveCfd.serie;
            hfFolio.Value = _llaveCfd.folio_factura.ToString("es-ES");
            hfUUID.Value = _llaveCfd.timbre_fiscal.uuid == "" ? " " : _llaveCfd.timbre_fiscal.uuid;

            if (ddlTransporte.Items.Count == 0) 
                LlenaCbos();
        }
        else
        {
            //if (!string.IsNullOrEmpty(HttpContext.Current.Session["RFC"].ToString())) return;
            //Response.Redirect("Login.aspx");

            if (HttpContext.Current.Session["RFC"] == null && HttpContext.Current.Session["USRswUsrInterno"].ToString() == "0")
            { Response.Redirect("Login.aspx"); return; }

            if (HttpContext.Current.Session["RFC"] == null && HttpContext.Current.Session["USRswUsrInterno"].ToString() == "1")
            { Response.Redirect("UserLogin.aspx"); return; }
        }
    }

    private void LlenaInfo()
    {
        LoadConfig();
        
        _decimales = _numDec == 2 ? "{0:0.00}" : "{0:0.0000}";
        LblEmpresa.Text = (string)HttpContext.Current.Session["nombre_receptor"];
        HttpContext.Current.Session["EmpresaTitle"] = (string)HttpContext.Current.Session["nombre_receptor"];
        _subtotal = (decimal)HttpContext.Current.Session["subtotal"];
        txtSubtotal.Text = string.Format(_decimales, _subtotal);
        _descuento = (decimal)HttpContext.Current.Session["descuento"];
        txtDcto.Text = string.Format(_decimales, _descuento);
        txtSubtotal.Text = string.Format(_decimales, _subtotal);
        _total = (decimal)HttpContext.Current.Session["total"];
        txtTotal.Text = string.Format(_decimales, _total);
        _totalTrsl = (decimal)HttpContext.Current.Session["tot_imp_trasl"];
        txtImporteT.Text = string.Format(_decimales, _totalTrsl);
        _totalRet = (decimal)HttpContext.Current.Session["tot_imp_ret"];
        txtImporteR.Text = string.Format(_decimales, _totalRet);
        _importe = (decimal)HttpContext.Current.Session["importe"];
        ConsultaProveedorRetencion();
    }
    private void ConsultaProveedorRetencion()
    { 
        var error = new List<Errores>();
        var ccCve = (string)HttpContext.Current.Session["cc_cve"];
        var ccTipo = (string)HttpContext.Current.Session["cc_tipo"];
        var tipdocCve = (string)HttpContext.Current.Session["tipdoc_cve"];
        var dtRet = _factura.cons_retP(error, ccTipo, ccCve, tipdocCve);

        if (dtRet.Rows.Count > 0)
        {
            var sw   = int.Parse(dtRet.Rows[0][0].ToString());
            if (sw != 1) return;
            _swRetProv = true;
            HttpContext.Current.Session["sw_ret_prov"] = _swRetProv;
        }
        else
        {
            var er = new Errores();
            {
                er.Interror = 3;
                er.Message = "No ha configuracion de retencion para este proveedor";
            }
            error.Add(er);
        }
    }
    private int LlenaCbos()
    {
        var error = new List<Errores>();
        var ds = new DataSet();
        var ccTipo = (string)HttpContext.Current.Session["cc_tipo"];
        var ccCve = (string)HttpContext.Current.Session["cc_cve"];
        var documento = (string)HttpContext.Current.Session["tipdoc_cve"];

        if (_llaveCfd.rfc_emisor == null) { LlenaLlave(); }

        /*inicia combo de transportes*/
        var dtTrans = _factura.cons_trans(error, ccTipo, ccCve);
        ddlTransporte.DataSource = null;
        ddlTransporte.DataSource = dtTrans; 

        if (dtTrans.Rows.Count == 0)
        {
            var er = new Errores();
            {
                er.Interror = 3;
                er.Message = "Sin información";
            }
            error.Add(er);
        }
        else
        {
            ddlTransporte.DataTextField = "nombre";
            ddlTransporte.DataValueField = "serv_cve";
            ddlTransporte.DataBind();
            ddlTransporte.Items[0].Selected = true;
        }
        /*termina combo de transportes*/
        CboPlaca();
        /*combo claves*/
        if (!error.Any())
        {
            var dtClave = _factura.ConsDatos(error, documento, 0, 0, _llaveCfd.ef_cve, 4);
            ddlNoIdentificacion.DataSource = null;
            ddlNoIdentificacion.DataSource = dtClave;
            if (dtClave.Rows.Count == 0)
            {
                var er = new Errores();
                {
                    er.Interror = 3;
                    er.Message = "Sin información";
                }
                error.Add(er);
            }
            else
            {
                ddlNoIdentificacion.DataTextField = "clave";
                ddlNoIdentificacion.DataValueField = "clave";
                ddlNoIdentificacion.DataBind();
                ddlNoIdentificacion.Items[0].Selected = true;
            }

        }
        return error.Count();
    }
    private int CboPlaca()
    {
        var error = new List<Errores>();
        var ccCve = (string)HttpContext.Current.Session["cc_cve"];
        var dtPlaca = _factura.cons_placa(error, ddlTransporte.SelectedValue, ccCve);
        this.ddlPlaca.DataSource = null;
        ddlPlaca.DataSource = dtPlaca;
        if (dtPlaca.Rows.Count == 0)
        {
            var er = new Errores();
            {
                er.Interror = 3;
                er.Message = "Sin información";
            }
            error.Add(er);
        }
        else
        {
            ddlPlaca.DataTextField = "op_nom";
            ddlPlaca.DataValueField = "op_cve_com";
            ddlPlaca.DataBind();
            ddlPlaca.Items[0].Selected = true;
        }
            CboDocumentos();
        /*Termina combo de placa*/

            return error.Count();
    }
    private int CboDocumentos()
    {
        var error = new List<Errores>();
        var ccTipo = (string)HttpContext.Current.Session["cc_tipo"];
        var ccCve = (string)HttpContext.Current.Session["cc_cve"];
        var tipdocCve = (string)HttpContext.Current.Session["tipdoc_cve"];
        /*Inicia combo de placa*/
        if (_llaveCfd.rfc_emisor == null) { LlenaLlave(); }
        var dtDocs = _factura.cons_docs(error, _llaveCfd.ef_cve, tipdocCve, ccTipo, ccCve, this.ddlTransporte.SelectedValue,this.ddlPlaca.SelectedValue);
        this.ddlReferencia.DataSource = null;
        ddlReferencia.DataSource = dtDocs;
        if (dtDocs.Rows.Count == 0)
        {
            var er = new Errores();
            {
                er.Interror = 3;
                er.Message = "Sin información";
            }
            error.Add(er);
        }
        else
        {
            ddlReferencia.DataTextField = "doc_papa";
            ddlReferencia.DataValueField = "doc_papa";
            ddlReferencia.DataBind();
        }
        /*Termina combo de placa*/
        return error.Count();
    }
    private void LlenaLlave()
    {
        _llaveCfd = (llave_cfd)HttpContext.Current.Session["LlaveCFD"];
        _comprobante = (Comprobante)HttpContext.Current.Session["comprobante"];

        if (folio_fac.Text == "")
        {
            folio_fac.Text = "" + _llaveCfd.folio_factura;
            serie_fac.Text = "" + _llaveCfd.serie.Trim();
            uuid.Text = "" + _llaveCfd.timbre_fiscal.uuid.Trim();
        }


    }

    private void LlenaGridTransportes()
    {
        var dt = new DataTable();
        dt.Columns.Add(new DataColumn("Referencia", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("NoIdentificacion", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Description", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Importe", Type.GetType("System.Decimal")));
        var row2 = dt.NewRow();
        dt.AcceptChanges();

        gvTransportes.DataSource = dt;
        gvTransportes.DataBind();
        HttpContext.Current.Session["dtTransportes"] = dt;
    }
    protected void gvTransportes_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        var dt = (DataTable)HttpContext.Current.Session["dtTransportes"];
        dt.Rows[e.RowIndex].Delete();
        dt.Rows[e.RowIndex].AcceptChanges();
        gvTransportes.DataSource = dt;
        gvTransportes.DataBind();
        Session["dtTransportes"] = dt;
        CalculaTotalesE(0, 0);
    }
    protected void gvTransportes_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            _dTotal += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Importe"));
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Text = @"Importe:";
            e.Row.Cells[2].Text = _dTotal.ToString();//dTotal.ToString("Importe");
            e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Font.Bold = true;
        }
    }
    protected void gvTransportes_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
        if (e.Exception != null)
        {
            e.ExceptionHandled = true;
        }
    }
    protected void imgbtnAgregar_Click(object sender, ImageClickEventArgs e)
    {
        var error = new Errores();
        decimal imp = 0;
        var referencia = ddlReferencia.SelectedValue;
        var clave = ddlNoIdentificacion.SelectedValue;

        if (referencia.Trim() == "")
            { error.Message = "Por favor seleccione el Folio Recepción CFD correcto"; error.Interror = 1; }

        if (clave.Trim() == "")
            { error.Message = "Por favor ingrese la informacion de Folio Recepción CFD correctos"; error.Interror = 1; }

        try
            { imp = decimal.Parse(this.txtImporte.Text); }
        catch (Exception)
            { error.Message = "Importe incorrecto debe ser decimal"; error.Interror = 1; }

        if (imp == 0)
            { error.Message = "El Importe no debe ser 0"; error.Interror = 1; }

        if (error.Interror== 0)
            error = ValidaDatos(referencia, clave, imp);

        if (error.Interror == 0)
            error = GrabaDatos(referencia, clave, imp);

        if (error.Interror != 0)
        {
            lblErr.Visible = true;
            lblErr.Text = "";
            lblErr.Text = error.Message;
        }
        else
        {
            ddlNoIdentificacion.SelectedIndex = 0;
            txtImporte.Text = "0";
            lblErr.Visible = false;
            lblErr.Text = "";
            ddlReferencia.Focus();
        }

        if (error.Interror == 0)
            CalculaTotalesE(0, 0);
    }
    private static Errores ValidaDatos(string referencia, string artCve,  decimal importe)
    {
        var error = new Errores();
        {
            error.Interror = 0;
        }

        var dt = (DataTable)HttpContext.Current.Session["dtTransportes"];
        var iExiste = (from DataRow dr in dt.AsEnumerable()
                       where (Convert.ToString(dr["Referencia"]) == referencia) && (Convert.ToString(dr["NoIdentificacion"]) == artCve)
                       select Convert.ToDecimal(dr["Importe"])).Count();
        if (iExiste != 0)
            error.Interror = 1;
        if (error.Interror != 0)
        {
            error.Message = "No repetir conceptos para el mismo Folio Recepcion CFD";
        }
        return error;
    }
    private Errores GrabaDatos(string referencia,string artCve, decimal importe)
    {
        var error = new Errores();
        try
        {
            var dt = (DataTable)HttpContext.Current.Session["dtTransportes"];
            var row = dt.NewRow();
            row[0] = referencia;
            row[1] = artCve;
            row[2] = artCve;
            row[3] = importe;
            dt.Rows.Add(row);
            dt.AcceptChanges();

            this.gvTransportes.DataSource = dt;
            this.gvTransportes.DataBind();
            HttpContext.Current.Session["dtTrans"] = dt;
        }
        catch (Exception ex)
        {
            error.Interror = 1;
            error.Message = ex.Message;
        }

        return error;
    }
    private int CompruebaCaptura()
    {


        int errores = 0;

        try
        {

            _comprobante.condiciones_pago = "0";
            _comprobante.Addenda.requestforpayment.paymenttimeperiod.timeperiod = 0;
            _comprobante.Addenda.requestforpayment.aditional_data.metododepago = "NO IDENTIFICADO"; //GCM 05012015 ddlMetodo.SelectedValue;
            _comprobante.Addenda.requestforpayment.aditional_data.moneda = _comprobante.moneda;//GCM 05012015 ddlMoneda.SelectedValue;

            var lineItems = new List<lineitem>();
            var number = 1;
            var dt = (DataTable)HttpContext.Current.Session["dtTransportes"];

            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    var identificacion = Convert.ToString(dr["NoIdentificacion"]);
                    var item = new lineitem();
                    var primer = identificacion.IndexOf("@", StringComparison.Ordinal);
                    item.sku = identificacion.Substring(0, primer);
                    item.art_tip = "SER";
                    item.type = 1;
                    item.number = number;
                    item.monto_decuento = 0;
                    item.pct_decuento = 0;
                    item.uns = 1;
                    item.precio = Convert.ToDecimal(dr["Importe"]);
                    item.partida = 0;
                    item.reference_identification = Convert.ToString(dr["Referencia"]);
                    item.uni_med = "zzzzzz";
                    lineItems.Add(item);
                    number = number + 1;
                }
            }
            _comprobante.Addenda.requestforpayment.line_items = lineItems;
        }
        catch (Exception ex)
        {
            errores = 1;
            string msg = ex.Message;
        }
        return errores;
    }
    private void CompruebaErrores(List<Errores> errores, llave_cfd llave, string emails)
    {
        if (errores.Count > 0)
        {
            lblErr.Text = "";
            var swMail = false;

            var msgUsr =
          from msg in errores
          where (msg.Interror == 1 && msg.Message != "") || (msg.Interror == 2 && msg.Message != "")
          select msg;

            var msgAdmin = from msg in errores
                           where msg.Interror == 3 || msg.Interror == 2
                           select msg;
            foreach (var err in msgUsr)
            {
                lblErr.Text = lblErr.Text + err.Message + Environment.NewLine;
                swMail = true;
            }

            if (swMail && msgAdmin.Count() != 0)
                _factura.GenMailErrHtml(llave, "Error al recibir Factura Electrónica", msgAdmin.ToList(), emails, HttpContext.Current.Session["nomProveedor"].ToString());
            else
            {
                if (msgUsr.ToList().Any())
                {
                    _factura.GenMailErrHtml(llave, "Error al recibir Factura Electrónica", msgUsr.ToList(), emails, HttpContext.Current.Session["nomProveedor"].ToString());
                }
            }
            var cadena = msgAdmin.Aggregate("", (current, err) => current + err.Message + Environment.NewLine);
            if (cadena == string.Empty) return;
            //_factura.EnviaMail(cadena, 1);
            //_factura.GenMailErrHtml(llave, "Error al recibir Factura Electrónica", msgAdmin.ToList(), emails, HttpContext.Current.Session["nomProveedor"].ToString());

            if (_factura.iErrorG > 0 & string.IsNullOrEmpty(lblErr.Text) & !string.IsNullOrEmpty(_factura.MensajeError))
            {
                lblErr.Text = _factura.MensajeError;

            }
        }

    }
    private Errores CalculaTotalesE(int impMsg, int swMsgs)
    {
        LoadConfig();
        var err1 = new Errores();
        var valor = 0;
        var error = 0;
        var msg = "";
        _decimales = _numDec == 2 ? "{0:0.00}" : "{0:0.0000}";

        try
        {

            var tasa = HttpContext.Current.Session["tasa"].ToString();
            var descuentoGlobal = HttpContext.Current.Session["descuento"].ToString();
            var subTotalComprobate = (decimal)HttpContext.Current.Session["subtotal"];
            var totImpTrasl = (decimal)HttpContext.Current.Session["tot_imp_trasl"];
            var totImpRet = (decimal)HttpContext.Current.Session["tot_imp_ret"];
            _total = (decimal)HttpContext.Current.Session["total"];
            _swRetProv = (bool)HttpContext.Current.Session["sw_ret_prov"];
            Decimal subtotalCap = 0;
            const decimal descuento = 0;

          
            var dt = (DataTable)HttpContext.Current.Session["dtTransportes"];

            //0107 aplicar retencion a esta clave lo que trae el combo o se graba es esto 0107@FLETE
            var impFleteRet = (from DataRow dr in dt.AsEnumerable()
                                       where (dr.RowState != DataRowState.Deleted && Convert.ToString(dr["NoIdentificacion"]) == "0107@FLETE")
                            select Convert.ToDecimal(dr["Importe"])).Sum();

            subtotalCap = (from DataRow dr in dt.AsEnumerable()
                            where dr.RowState != DataRowState.Deleted
                            select Convert.ToDecimal(dr["Importe"])).Sum();
            txtSubTcap.Text = string.Format(_decimales, subtotalCap);
            //var ivaCap = (subtotalCap - descuento) * (Decimal.Parse(tasa) / 100);
            var ivaCap = (subtotalCap - descuento) * Decimal.Parse(tasa);
            txtIva.Text = string.Format(_decimales, ivaCap);

            Decimal totRet = 0;
            Decimal totalCap =0;
            if (impFleteRet > 0 && _swRetProv == true)
            {
                totRet = (impFleteRet * _pctRetencion);
            }

            txtRetenciones.Text = string.Format(_decimales, totRet);
            totalCap = Math.Round((subtotalCap - descuento) + (ivaCap - totRet), _numDec, MidpointRounding.AwayFromZero);
            txtTotalSum.Text = string.Format(_decimales, totalCap);

            var subtotalConceptos = Math.Round((Decimal)HttpContext.Current.Session["total_conceptos"], _numDec, MidpointRounding.AwayFromZero);
            var importeConceptos = Math.Round((Decimal)HttpContext.Current.Session["importe_conceptos"], _numDec, MidpointRounding.AwayFromZero);

           


            if (error == 0)
            {
                var minimoTotal = subTotalComprobate - _rango;
                var maximoTotal = subTotalComprobate + _rango;

                if (subtotalCap < minimoTotal || subtotalCap > maximoTotal)
                {
                    if (swMsgs == 1)
                    {
                        msg = "El subtotal de lo capturado es incorrecto por favor verificar";
                    }
                    error = 1;
                }

            }

            if (error == 0)
            {
                var minimoTotal = totImpTrasl - decimal.Parse(".25");
                var maximoTotal = totImpTrasl + decimal.Parse(".25");

                if (ivaCap < minimoTotal || ivaCap > maximoTotal)
                {
                    if (swMsgs == 1)
                    {
                        msg = "El iva de lo capturado es incorrecto por favor verificar";
                    }
                    error = 1;
                }

            }

            if (error == 0 && _swRetProv == true)
            {
                var minimoTotal = totImpRet - decimal.Parse(".25");
                var maximoTotal = totImpRet + decimal.Parse(".25");

                if (totRet < minimoTotal || totRet > maximoTotal)
                {
                    if (swMsgs == 1)
                    {
                        msg = "La retención de lo capturado es incorrecto por favor verificar";
                    }
                    error = 1;
                }


            }


            if (error == 0 )
            {
                var minimoTotal = _total - _rango;
                var maximoTotal = _total + _rango;
                if (totalCap >= minimoTotal && totalCap <= maximoTotal && valor == 0)
                {
                    if (swMsgs == 1)
                    { msg = "El total de lo capturado es correcto por favor de click en Generar Factura"; }
                    error = 0;
                    HttpContext.Current.Session["sw_captura"] = 1;
                }
                else
                {
                    if (swMsgs == 1)
                    { msg = "El total de lo capturado es incorrecto por favor verificar"; }
                    error = 1;
                }
            }
           

            if (error == 0)
            {
                txtIva.ForeColor = System.Drawing.Color.Black;
                txtSubTcap.ForeColor = System.Drawing.Color.Black;
                txtRetenciones.ForeColor = System.Drawing.Color.Black;
                txtTotalSum.ForeColor = System.Drawing.Color.Black;

                txtTotalSum.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
                txtRetenciones.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
                txtIva.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
                txtSubTcap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
                lblErr.Text = "";
                lblErr.Visible = false;
            }
            else
            {
                txtIva.ForeColor = System.Drawing.Color.Black;
                txtSubTcap.ForeColor = System.Drawing.Color.Black;
                txtRetenciones.ForeColor = System.Drawing.Color.Black;
                txtTotalSum.ForeColor = System.Drawing.Color.Black;

                txtIva.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
                txtSubTcap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
                txtRetenciones.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
                txtTotalSum.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
                lblErr.Text = msg;
                lblErr.Visible = true;
            }



            if (totalCap == 0)
            {
                lblErr.Text = "";
                if (swMsgs == 1)
                { msg = "El importe no debe ser 0"; }
                error = 1;
            }

            if (error != 0)
            {
                if (swMsgs == 1)
                {
                    lblErr.Text = msg;
                }
                err1.Interror = error;
                err1.Message = msg;
            }
        }
        catch (Exception)
        {
        }
        return err1; 
    }
    protected void ddlTransporte_SelectedIndexChanged(object sender, EventArgs e)
    {
        CboPlaca();
        CboDocumentos();
        ddlTransporte.Focus();
    }
    protected void ddlPlaca_SelectedIndexChanged(object sender, EventArgs e)
    {
        CboDocumentos();
        //ddlPlaca.Focus();
    }
    protected void LnkGenerarFactura_Click(object sender, EventArgs e)
    {
        LoadConfig();
        var err = 0;
        var msg = "";
        //GCM 05012015 va ir por default
        //if (ddlMetodo.SelectedIndex == 0)
        //    { err = 1; msg = "Seleccione Metodo de Pago"; }
        //if (ddlMoneda.SelectedIndex == 0)
        //    { err = 1; msg = "Seleccione Moneda"; }
        if (err == 0)
        {
            var errores = new List<Errores>();
            var error1 = CalculaTotalesE(1, 1);
            {
                err = error1.Interror; 
                msg = error1.Message;
            }
            if (_llaveCfd.rfc_emisor == null) { LlenaLlave(); }


            if (err == 0)
            { err = CompruebaCaptura(); }

            if (err == 0)
            {
                //inicia el componente ajax
                System.Threading.Thread.Sleep(8000);

                dynamic items = _comprobante.Addenda.requestforpayment.line_items;
                dynamic contrarecibo = new nuevas_facturas();
                _factura.GrabaTmp(errores, _comprobante, _llaveCfd);


                if (errores.Count == 0)
                {
                    var config = (List<ConfigGral>)HttpContext.Current.Session["config"];

                    _factura.GenFactura(_rutaTmp + _llaveCfd.nom_arch + ".xml", _rutaTmp + _llaveCfd.nom_arch + ".pdf", errores, items, _comprobante, _llaveCfd, contrarecibo, config);
                }
                if (errores.Count == 0)
                {
                    HttpContext.Current.Session["FolioContra"] = contrarecibo.num_fol.ToString();
                    HttpContext.Current.Session["DoctoContra"] = contrarecibo.tipo_doc.ToString();
                    HttpContext.Current.Session["ef_cve"] = contrarecibo.ef_cve.ToString();
                }

                if (errores.Count != 0)
                {
                    err = 1;
                    HttpContext.Current.Session["errores"] = errores;
                }

                LnkGenerarFactura.Enabled = false;
                CompruebaErrores(errores, _llaveCfd, _emails);


                if (lblErr.Text != "" && err == 1)
                {
                    HttpContext.Current.Session["msgs"] = lblErr.Text;
                }

                if (errores.Count == 0 || err == 1)
                {
                    Response.Redirect("ImpresionRecibo.aspx");
                }
            }
        }

        if (err != 0)
        {
            lblErr.Visible = true;
            lblErr.Text = msg;
            lblErr.ForeColor = System.Drawing.Color.Red;
        }
        else
        {
            lblErr.Visible = false;
            lblErr.Text = "";
        }
    
    }

    protected void gvTransportes_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}