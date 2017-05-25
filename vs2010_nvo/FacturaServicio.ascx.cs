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


public partial class FacturaServicio : UserControl
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
        decimalesTmp = (string)_factura.GetConfigure(config, "retencion");
        _pctRetencion = decimal.Parse(decimalesTmp);
        decimalesTmp = (string)_factura.GetConfigure(config, "rangoMinMax");
        _rango = decimal.Parse(decimalesTmp);
    }

    private void LlenaLlave()
    {
        _llaveCfd = (llave_cfd)HttpContext.Current.Session["LlaveCFD"];
        _llaveCfd.sw_tmp = 1;
        _comprobante = (Comprobante)HttpContext.Current.Session["comprobante"];

        if (folio_fac.Text == "")
        {
            folio_fac.Text = "" + _llaveCfd.folio_factura;
            serie_fac.Text = "" + _llaveCfd.serie.Trim();
            uuid.Text = "" + _llaveCfd.timbre_fiscal.uuid.Trim();
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
            var sw = int.Parse(dtRet.Rows[0][0].ToString());
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

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            //if (HttpContext.Current.Session["RFC"] == null)
            //{ Response.Redirect("Login.aspx"); return; }

            if (HttpContext.Current.Session["RFC"] == null && HttpContext.Current.Session["USRswUsrInterno"].ToString() == "0")
            { Response.Redirect("Login.aspx"); return; }

            if (HttpContext.Current.Session["RFC"] == null && HttpContext.Current.Session["USRswUsrInterno"].ToString() == "1")
            { Response.Redirect("UserLogin.aspx"); return; }


            if (_llaveCfd.rfc_emisor == null || _comprobante == null) { LlenaLlave(); }
            HttpContext.Current.Session["LlaveCFD"] = _llaveCfd;
            HttpContext.Current.Session["msgs"] = "";
            LlenaInfo();

            hfRfc.Value = _llaveCfd.rfc_emisor;
            hfSerie.Value = _llaveCfd.serie;
            hfFolio.Value = _llaveCfd.folio_factura.ToString("es-ES");
            hfUUID.Value = _llaveCfd.timbre_fiscal.uuid == "" ? " " : _llaveCfd.timbre_fiscal.uuid;

            if (this.ddlReferencia.Items.Count == 0)
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

    private void LlenaCbos()
    {
        var error = new List<Errores>();
        var ds = new DataSet();
        var ccTipo = (string)HttpContext.Current.Session["cc_tipo"];
        var ccCve = (string)HttpContext.Current.Session["cc_cve"];
        //var documento = (string)HttpContext.Current.Session["tipdoc_cve"];
        var tipdocCve = (string)HttpContext.Current.Session["tipdoc_cve"];

        if (_llaveCfd.rfc_emisor == null) { LlenaLlave(); }

        //var cboCls = new combos();
        //cboCls.rfc = ccTipo + "@" + ccCve;
        //cboCls.combo = "cboDocSrv";
        //cboCls.parametro1 = "";
        //cboCls.parametro2 = "";
        //var dtTrans = _factura.LlenaCbo(cboCls);

        var dtTrans = _factura.cons_docs(error, _llaveCfd.ef_cve, tipdocCve, ccTipo, ccCve, "zzzzzz", "zzzzzz");

        ddlReferencia.DataSource = null;
        ddlReferencia.DataSource = dtTrans;

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
            //ddlReferencia.DataTextField = "descripcion";
            //ddlReferencia.DataValueField = "clave";
            ddlReferencia.DataTextField = "doc_papa";
            ddlReferencia.DataValueField = "doc_papa";
            ddlReferencia.DataBind();
            ddlReferencia.Items[0].Selected = true;
        }
    }

    protected void LnkGenerarFactura_Click(object sender, EventArgs e)
    {
        LoadConfig();
        var err = 0;
        var msg = "";
        //GCM 05012015 se tomara por default
        //if (ddlMetodo.SelectedIndex == 0)
        //{ err = 1; msg = "Seleccione Metodo de Pago"; }
        //if (ddlMoneda.SelectedIndex == 0)
        //{ err = 1; msg = "Seleccione Moneda"; }
        if (ddlReferencia.SelectedIndex < 0)
        { err = 1; msg = "Seleccione Acuse de Recibo";}
        
        if (err == 0)
        {
            var errores = new List<Errores>();
            //var error1 = CalculaTotalesE(1, 1);
            //{
            //    err = error1.Interror;
            //    msg = error1.Message;
            //}
            
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



    private int CompruebaCaptura()
    {
        var errores = 0;
        try
        {
            _comprobante.condiciones_pago = "0";
            _comprobante.Addenda.requestforpayment.paymenttimeperiod.timeperiod = 0;
            _comprobante.Addenda.requestforpayment.aditional_data.metododepago = "NO IDENTIFICADO"; //GCM 05012015 ddlMetodo.SelectedValue;
            _comprobante.Addenda.requestforpayment.aditional_data.moneda = _comprobante.moneda;//GCM 05012015 ddlMoneda.SelectedValue;
            var docTmp = new Document {referenceIdentification = ddlReferencia.SelectedValue};
            _comprobante.Addenda.requestforpayment.document = docTmp;
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



}