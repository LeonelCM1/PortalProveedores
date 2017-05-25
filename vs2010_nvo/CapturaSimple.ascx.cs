using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Skytex.FacturaElectronica;
using CLPortalAbstract;

public partial class CapturaSimple : UserControl
{
    string _ruta = "";
    string _rutaTmp = "";
    int _numDec = 0;
    //decimal _numDec = 0;
    private readonly Factura _factura = new Factura();
    string _decimales;
    decimal _rangoMinMax;
    string _emails;
    decimal _subtotal = 0;
    decimal _descuento = 0;
    decimal _total = 0;
    decimal _totalTrsl = 0;
    decimal _totalRet = 0;
    decimal _cargosTotales = 0;
    decimal _dTotal = 0, _dTotalC = 0;
    decimal _importe = 0;
    
    Datos _dat = new Datos();
    llave_cfd _llaveCfd = new llave_cfd();
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
        decimalesTmp = (string)_factura.GetConfigure(config, "rangoMinMax");
        _rangoMinMax = decimal.Parse(decimalesTmp);
        //_rangoMinMax = (decimal)_factura.GetConfigure(config, "rangoMinMax");
    }
    protected void gvArticulos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            _dTotal += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Importe"));
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[6].Text = @"Importe:";
            var culture = Thread.CurrentThread.CurrentCulture;
            e.Row.Cells[7].Text = _dTotal.ToString(culture);//dTotal.ToString("Importe");
            e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Font.Bold = true;
        }
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
    }
    protected void Page_Load(object sender, EventArgs e)
    {
       

            if (HttpContext.Current.Session["dtArt"] == null)
            LlenaGridArticulos();

        if (HttpContext.Current.Session["dtCargo"] == null)
            LlenaGridCargos();

        if (IsPostBack)
        {
            var ctrlName = Request.Params[Page.postEventSourceID];
            var args = Request.Params[Page.postEventArgumentID];
            HandleCustomPostbackEvent(ctrlName, args);
        }

        if (!IsPostBack)
        {
            if (HttpContext.Current.Session["RFC"] == null) { Response.Redirect("Login.aspx"); return; }
            if (_llaveCfd.rfc_emisor == null || _comprobante == null)  { LlenaLlave(); }
            HttpContext.Current.Session["LlaveCFD"] = _llaveCfd;
            HttpContext.Current.Session["msgs"] = "";
            LlenaInfo();
            hfRfc.Value = _llaveCfd.rfc_emisor;
            hfSerie.Value = _llaveCfd.serie;
            hfFolio.Value = _llaveCfd.folio_factura.ToString("es-ES");
            hfUUID.Value = _llaveCfd.timbre_fiscal.uuid == "" ? " " : _llaveCfd.timbre_fiscal.uuid;
        }
        else
        {
            //if (!string.IsNullOrEmpty(HttpContext.Current.Session["RFC"].ToString())) return;
            //Response.Redirect("Login.aspx");
            if (HttpContext.Current.Session["RFC"] == null && HttpContext.Current.Session["USRswUsrInterno"].ToString() == "0")
            { Response.Redirect("Login.aspx"); return; }

            if (HttpContext.Current.Session["RFC"] == null && HttpContext.Current.Session["USRswUsrInterno"].ToString() == "1")
            { Response.Redirect("UserLogin.aspx"); return; }

            return;
        }
    }
    private void LlenaLlave()
    {
        _llaveCfd = (llave_cfd)HttpContext.Current.Session["LlaveCFD"];
        _comprobante = (Comprobante)HttpContext.Current.Session["comprobante"];

        if (folio_fac.Text == "")
        {
            folio_fac.Text = "" + _llaveCfd.folio_factura;
            serie_fac.Text =   "" + _llaveCfd.serie.Trim();
            uuid.Text = "" + _llaveCfd.timbre_fiscal.uuid.Trim();
        }


    }
    private void LlenaInfo()
    {
        _decimales = _numDec == 2 ? "{0:0.00}" : "{0:0.0000}";
        HttpContext.Current.Session["EmpresaTitle"] = (string)HttpContext.Current.Session["nombre_receptor"];
        LblEmpresa.Text = (string)HttpContext.Current.Session["nombre_receptor"];
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
    }
    private void HandleCustomPostbackEvent(string ctrlName, string args)
    {
        if (ctrlName == txtPartida.UniqueID && args == "OnBlur")
        {
            busca_info();
            ddlNoIdentificacion.Focus();
        }
    }
    private int ValidaDatos(string referencia, int partida, string artCve, decimal uns, string um, decimal precio, decimal pctdesc, decimal importe)
    {
        var err = 0;

        var dt = new DataTable();
        dt = (DataTable)HttpContext.Current.Session["dtArt"];

        var iExiste = (from DataRow dr in dt.AsEnumerable()
                       where (Convert.ToString(dr["Referencia"]) == referencia) && (Convert.ToInt16(dr["Partida"]) == partida) 
                          select Convert.ToDecimal(dr["Importe"])).Count();
        if (iExiste != 0)
            err = 1;

        if (ddUM.SelectedIndex == 0)
            err = 1;

        return err;
    }
    private int ValidaDatos(string artCve, decimal importe)
    {
        var err = 0;
        var dt = (DataTable)HttpContext.Current.Session["dtCargo"];
        var iExiste = (from DataRow dr in dt.AsEnumerable()
                       where (Convert.ToString(dr["NoIdentificacion"]) == artCve)
                       select Convert.ToDecimal(dr["NoIdentificacion"])).Count();
        if (iExiste != 0)
            err = 1;
        if (ddlCargo.SelectedIndex == 0)
            err = 1;
        return err;
    }
    private int GrabaDatos(string referencia, int partida, string artCve, decimal uns, string um, decimal precio, decimal pctdesc, decimal importe)
    {
        var ierror = 0;
        try
        {
            var dt = (DataTable)HttpContext.Current.Session["dtArt"];
            var row = dt.NewRow();
            row[0] = referencia;
            row[1] = partida;
            row[2] = artCve;
            row[3] = uns;
            row[4] = um;
            row[5] = precio;
            row[6] = pctdesc;
            row[7] = importe;
            dt.Rows.Add(row);
            dt.AcceptChanges();
            gvArticulos.DataSource = dt;
            gvArticulos.DataBind();
            HttpContext.Current.Session["dtArt"] = dt;
        }
        catch (Exception ex)
        {
            ierror = 1;
            var msj = ex;
        }
        return ierror;
    }
    private int GrabaDatosCargo(string artCve,string descripcion,  decimal importe)
    {
        const int ierror = 0;
        try
        {
            var dt2 = (DataTable)HttpContext.Current.Session["dtCargo"];
            var row = dt2.NewRow();
            row[0] = artCve;
            row[1] = descripcion;
            row[2] = importe;
            dt2.Rows.Add(row);
            dt2.AcceptChanges();
            gvCargos.DataSource = dt2;
            gvCargos.DataBind();
            HttpContext.Current.Session["dtCargo"] = dt2;
        }
        catch (Exception)
        {
            throw;
        }

        return ierror;
    }
    private void LlenaGridArticulos()
    {
        var dt = new DataTable();
        dt.Columns.Add(new DataColumn("Referencia", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Partida", Type.GetType("System.Int64")));
        dt.Columns.Add(new DataColumn("NoIdentificacion", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Uns", Type.GetType("System.Decimal")));
        dt.Columns.Add(new DataColumn("UM", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Precio", Type.GetType("System.Decimal")));
        dt.Columns.Add(new DataColumn("PctDesc", Type.GetType("System.Decimal")));
        dt.Columns.Add(new DataColumn("Importe", Type.GetType("System.Decimal")));
        dt.AcceptChanges();
        gvArticulos.DataSource = dt;
        gvArticulos.DataBind();
        HttpContext.Current.Session["dtArt"] = dt;
        HttpContext.Current.Session["saldo"] = decimal.Parse("0.0");
    }
    private void LlenaGridCargos()
    {
        var dt2 = new DataTable();
        dt2.Columns.Add(new DataColumn("NoIdentificacion", Type.GetType("System.String")));
        dt2.Columns.Add(new DataColumn("Description", Type.GetType("System.String")));
        dt2.Columns.Add(new DataColumn("Importe", Type.GetType("System.Decimal")));
        dt2.AcceptChanges();

        gvCargos.DataSource = dt2;
        gvCargos.DataBind();
        HttpContext.Current.Session["dtCargo"] = dt2;
    }
    protected void gvArticulos_SelectedIndexChanged(object sender, EventArgs e)
    {
    
    }
    protected void gvCargos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            _dTotalC += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Importe"));
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Text = @"Importe:";
            e.Row.Cells[2].Text = _dTotalC.ToString();
            e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Font.Bold = true;
        }
    }
    protected void gvArticulos_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        var dt = (DataTable)HttpContext.Current.Session["dtArt"];
        dt.Rows[e.RowIndex].Delete();
        dt.Rows[e.RowIndex].AcceptChanges();
        gvArticulos.DataSource = dt;
        gvArticulos.DataBind();
        Session["dtArt"] = dt;
        CalculaTotalesE(0, 0);
    }
    protected void gvArticulos_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
        if (e.Exception != null)
        {
            e.ExceptionHandled = true;
        }
    }
    protected void txtPartida_TextChanged(object sender, EventArgs e)
    {
        

    }
    private static int ValidaInfCombo(string referencia, int partida)
    {
        var err = 0;
        var valor = referencia.IndexOf("@", StringComparison.Ordinal) + 1;

        if (valor == 0)
        {
            err = 1;
        }
        return err;
    }
    protected void ddlNoIdentificacion_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void gvCargos_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        var dt2 = (DataTable)HttpContext.Current.Session["dtCargo"];
        dt2.Rows[e.RowIndex].Delete();
        dt2.Rows[e.RowIndex].AcceptChanges();
        gvCargos.DataSource = dt2;
        gvCargos.DataBind();
        Session["dtCargo"] = dt2;
        CalculaTotalesE(0, 0);

    }
    protected void gvCargos_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
        if (e.Exception != null)
        {
            e.ExceptionHandled = true;
        }
    }
    protected void imgbtnAgregarC_Click(object sender, ImageClickEventArgs e)
    {
        var err = 0;
        var msg = "";

        var artCve = ddlCargo.SelectedValue;
        var descripcion = ddlCargo.SelectedItem.Text;
        if (artCve.Trim() == "")
        {
            msg = "No se puede agregar partida "; err = 1;
        }
        var importe = decimal.Parse(txtImpCargo.Text);
        if (importe ==0)
        {
            msg = "El importe no debe ser 0"; err = 1;
        }
        if (err == 0)
            err = ValidaDatos(artCve, importe);
        if (err == 0)
            err = GrabaDatosCargo(artCve, descripcion, importe);
        if (err == 0)
            CalculaTotalesE(0,0);    
        if (err != 0)
        {
            lblErrCargo.Visible = true;
            lblErrCargo.Text = msg;
            lblErrCargo.ForeColor = System.Drawing.Color.Red;
        }
        else
        {
            lblErrCargo.Visible = false;
            lblErrCargo.Text = "";
            ddlCargo.SelectedIndex = 0;
            txtImpCargo.Text = @"0";
            ddlCargo.Focus();
        }
    }
    private Errores busca_info()
    {
        var error = new List<Errores>();
        var errObj = new Errores();
        try
        {
            var referencia = txtReferencia.Text.Trim();
            var partida = int.Parse(txtPartida.Text);
            errObj.Interror = ValidaInfCombo(referencia, partida);
            var documento = "";
            var folio = 0;

            if (referencia.Length > 15)
            {
                errObj.Interror = 1;
                txtReferencia.Text = "";
            }
            if (errObj.Interror == 1)
                txtReferencia.Focus();
            if (errObj.Interror == 0)
            {
                int valor = referencia.IndexOf("@", StringComparison.Ordinal) + 1;
                documento = txtReferencia.Text.Substring(0, valor - 1);
                folio = int.Parse(txtReferencia.Text.Substring(valor, txtReferencia.Text.Length - valor));
            }
            if (errObj.Interror == 0)
            {
                var ds = new DataSet();
                if (_llaveCfd.rfc_emisor == null) { LlenaLlave(); }
                _factura.cons_doc(error, documento, folio, partida, _llaveCfd.ef_cve, 4, ds);
                ddlNoIdentificacion.DataSource = null;
                ddlNoIdentificacion.DataSource = ds.Tables[0];
                if (ds.Tables[0].Rows.Count == 0) 
                {
                    errObj.Interror = 1;
                }
                ddlNoIdentificacion.DataTextField = "sku_cve";
                ddlNoIdentificacion.DataValueField = "sku_cve";
                ddlNoIdentificacion.DataBind();
                if (errObj.Interror == 0)
                {
                    var saldo = decimal.Parse(ds.Tables[0].Rows[0]["saldo"].ToString());
                    if (saldo <= 0)
                    {
                        errObj.Interror = 1;
                        errObj.Message = "El folio de Recepcion CFD no tiene saldo";
                    }
                    else
                    {
                        HttpContext.Current.Session["saldo"] = saldo;
                    }
                }
                var oListItemA = ddlNoIdentificacion.Items.FindByValue(ds.Tables[0].Rows[0][2].ToString());
                if (oListItemA != null)
                    ddlNoIdentificacion.SelectedValue = oListItemA.Value;
               var um = new List<ListItem>();
               if (ds.Tables[0].Rows[0][4].ToString() == "")
                    { um.Add(new ListItem("Seleccione Uno", "")); }
               else {
                    um.Add(new ListItem("Seleccione Uno", ""));
                    um.Add(new ListItem(ds.Tables[0].Rows[0][4].ToString(), ds.Tables[0].Rows[0][4].ToString()));
                }
                ddUM.DataSource = um;
                ddUM.DataBind();
                var oListItem = ddUM.Items.FindByValue(ds.Tables[0].Rows[0][4].ToString());
                if (oListItem != null)
                    ddUM.SelectedValue = oListItem.Value;
            }
        }
        catch (Exception)
        {
            lblErr.Visible = true;
            lblErr.Text = @"Datos incorrectos en Folio Recepción CFD y Partida";
            lblErr.ForeColor = System.Drawing.Color.Red;
            errObj.Interror = 1;
            errObj.Message = "Datos incorrectos en Folio Recepción CFD y Partida";
        }
        return errObj;
    }
    protected void Page_Init(object sender, EventArgs e)
    {
        var onBlurScript = Page.ClientScript.GetPostBackEventReference(txtPartida, "OnBlur");
        txtPartida.Attributes.Add("onblur", onBlurScript);
    }
    protected void LnkGenerarFactura_Click(object sender, EventArgs e)
    {
        LoadConfig();
        var err = 0;
        var msg = "";
        //GCM 05012015 ya no pedira tm y tp
        //if (ddlMetodo.SelectedIndex == 0)
        //   { err = 1; msg = "Seleccione Metodo de Pago"; }
        //if (ddlMoneda.SelectedIndex == 0)
        //   { err = 1; msg = "Seleccione Moneda"; }
        if (err == 0)
           {
               var errores = new List<Errores>();
               var error1 = CalculaTotalesE(1, 1);
               { err = error1.Interror; msg = error1.Message; }
               if (_llaveCfd.rfc_emisor == null) { LlenaLlave(); }
               if (err == 0) { err = CompruebaCaptura(); }
               if (err == 0)
               {
                   var items = _comprobante.Addenda.requestforpayment.line_items;
                   dynamic contrarecibo = new nuevas_facturas();
                   _factura.GrabaTmp(errores, _comprobante, _llaveCfd);
                   if (errores.Count == 0)
                   { 
                       var config = (List<ConfigGral>)HttpContext.Current.Session["config"];
                       _factura.GenFactura(_rutaTmp + _llaveCfd.nom_arch + ".xml", _rutaTmp + _llaveCfd.nom_arch + ".pdf", errores, items, _comprobante, _llaveCfd, contrarecibo, config); }
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
            var lineItems = new List<lineitem>();
            var identificacion = "";
            var number = 1;
            var dt = (DataTable)HttpContext.Current.Session["dtArt"];
            foreach (var dr in dt.Rows.Cast<DataRow>().Where(dr => dr.RowState != DataRowState.Deleted))
            {
                identificacion = Convert.ToString(dr["NoIdentificacion"]);
                var item = new lineitem();
                var primer = identificacion.IndexOf("@",StringComparison.Ordinal);
                item.sku = identificacion.Substring(0, primer);
                item.art_tip = identificacion.Substring(primer + 1, identificacion.Length - (primer + 1));
                item.type = 1;
                item.number = number;
                item.monto_decuento = 0;
                item.pct_decuento = Convert.ToDecimal(dr["PctDesc"]);
                item.uns = Convert.ToDecimal(dr["Uns"]);
                item.precio = Convert.ToDecimal(dr["Precio"]);
                item.partida = Convert.ToInt16(dr["Partida"]);
                item.reference_identification = Convert.ToString(dr["Referencia"]);
                item.uni_med = Convert.ToString(dr["UM"]);
                lineItems.Add(item);
                number = number + 1;
            }
            var dt2 = (DataTable)HttpContext.Current.Session["dtCargo"];
            foreach (DataRow dr in dt2.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    identificacion = Convert.ToString(dr["NoIdentificacion"]);
                    var item = new lineitem();
                    {
                        item.sku = identificacion; //identificacion.Substring(0, primer);
                        item.art_tip = "SER";//identificacion.Substring(primer + 1, identificacion.Length - (primer + 1));
                        item.type = 2;
                        item.number = number;
                        item.monto_decuento = 0;
                        item.pct_decuento = 0; //Convert.ToDecimal(dr["PctDesc"]);
                        item.uns = 1;// Convert.ToDecimal(dr["Uns"]);
                        item.precio = Convert.ToDecimal(dr["Importe"]);
                        item.partida = 0; // Convert.ToInt16(dr["Partida"]);
                        item.reference_identification = "xxxxxx@x"; //Convert.ToString(dr["Referencia"]);
                        item.uni_med = "NO APLICA"; //Convert.ToString(dr["UM"]);
                        lineItems.Add(item);
                        number = number + 1;
                    }
                }
            }  
            _comprobante.Addenda.requestforpayment.line_items = lineItems;
        }
        catch (Exception ex)
        {
            errores = 1;
        }
        return errores;
    }
    private void CompruebaErrores(List<Errores> errores, llave_cfd llave, string emails)
    {
        if (errores.Count <= 0) return;
        lblErr.Text = "";
        var msgUsr =
            from msg in errores
            where (msg.Interror == 1 && msg.Message !="") || (msg.Interror == 2 && msg.Message !="")
            select msg;

        var msgAdmin = from msg in errores
                    where msg.Interror == 3 || msg.Interror == 2
                    select msg;
        var swMail = false;
        foreach (var err in msgUsr)
        {
            lblErr.Text = lblErr.Text + err.Message + Environment.NewLine;
            swMail = true;
        }

        if (swMail && msgAdmin.Count() != 0)
            _factura.GenMailErrHtml(llave, "Error al recibir Factura Electrónica", msgAdmin.ToList(), emails,  HttpContext.Current.Session["nomProveedor"].ToString());
        else
        {
            if (msgUsr.ToList().Any())
            {
                _factura.GenMailErrHtml(llave, "Error al recibir Factura Electrónica", msgUsr.ToList(), emails, HttpContext.Current.Session["nomProveedor"].ToString());
            }
        }

        var cadena = msgAdmin.Aggregate("", (current, err) => current + err.Message + Environment.NewLine);
        if (cadena == string.Empty) return;

        //_factura.GenMailErrHtml(llave, "Error al recibir Factura Electrónica", msgAdmin.ToList(), emails, HttpContext.Current.Session["nomProveedor"].ToString());

        if (_factura.iErrorG > 0 & string.IsNullOrEmpty(lblErr.Text) & !string.IsNullOrEmpty(_factura.MensajeError))
        {
            lblErr.Text = _factura.MensajeError;
        }
    }
    protected void imgbtnAgregar_Click(object sender, ImageClickEventArgs e)
    {
        var err = 0;
        var msg = "";
        var errObj = busca_info();
        if (errObj.Interror != 0)
            { err = errObj.Interror; msg = errObj.Message; }
        decimal pctdesc = 0;
        decimal precio = 0;
        var referencia = txtReferencia.Text.Trim().Replace(" ","");
        var partida = int.Parse(txtPartida.Text);
        var artCve = ddlNoIdentificacion.SelectedValue;
        if (err == 0)
        {
            if (artCve.Trim() == "")
                { msg = "Por favor ingrese la informacion de Folio Recepción CFD correctos"; err = 1; }
        }
        decimal uns = 0;
        if (err == 0)
        {
            try
            {   uns = decimal.Parse(txtUns.Text);}
            catch (Exception)
            {   msg = "Cantidad incorrecta debe ser decimal"; err = 1; }
         }
        if (err == 0)
        { 
            decimal saldo =  decimal.Parse(HttpContext.Current.Session["saldo"].ToString());
                                        if (saldo < uns)
            {
                msg = "La cantidad a facturar es mayor al saldo del acuse de recibo de mercancía"; err = 1;
            }
        }
        var um = ddUM.SelectedValue;
        if (err == 0)
        {
            try
            { precio = decimal.Parse(txtPrecio.Text); }
            catch (Exception)
            { msg = "Precio incorrecto debe ser decimal"; err = 1; }
        }
        if (err == 0)
        {
            try
            { pctdesc = decimal.Parse(txtPctDesc.Text); }
            catch (Exception) { msg = "Porcentaje de descuento incorrecto debe ser decimal"; err = 1; }
        }
        if (err == 0)
        {
            if (partida == 0)
                { msg = "Partida no debe ser 0"; err = 1; }
        }
        if (err == 0)
        {
            if (uns == 0)
                { msg = "Cantidad no debe ser 0"; err = 1; }
        }
        if (err == 0)
        {
            if (precio == 0)
            { msg = "Precio no debe ser 0"; err = 1; }
        }
        if (err == 0)
        {
            if (ddUM.SelectedIndex == 0)
            { msg = "Debe existir la unidad de medida"; err = 1; }
        }    
        var subT = uns * precio;
        var pctDesc = subT * (pctdesc / 100);
        var imp = subT - pctDesc;
        if (err == 0)
            err = ValidaDatos(referencia, partida, artCve, uns, um, precio, pctdesc, imp);
        if (err == 0)
            err = GrabaDatos(referencia, partida, artCve, uns, um, precio, pctdesc, imp);
        if (err != 0)
        {
            lblErr.Visible = true;
            lblErr.Text = "";
            lblErr.Text = msg;
        }
        else
        {
            txtPartida.Text = "";
            ddlNoIdentificacion.DataSource = null;
            ddlNoIdentificacion.Items.Clear();
            txtUns.Text = @"0";
            ddUM.SelectedIndex = 0;
            txtPrecio.Text = @"0";
            txtPctDesc.Text = @"0";
            txtImporte.Text = @"0";
            //lblErr.Visible = false;
            lblErr.Text = "";
            HttpContext.Current.Session["saldo"] = decimal.Parse("0.0");
            txtReferencia.Focus();
        }
        if (err == 0)
            CalculaTotalesE(0, 0);
    }
    protected void ddlCargo_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
    private Errores CalculaTotalesE(int impMsg, int swMsgs)
    {
        LoadConfig();
        var err1 = new Errores();
        var _rango = _rangoMinMax;
        //var rango = decimal.Parse("0.5");
        var msg = "";
        _decimales = _numDec == 2 ? "{0:0.00}" : "{0:0.0000}";

        try
        {
            var tasa = HttpContext.Current.Session["tasa"].ToString();
            var descuentoGlobal = HttpContext.Current.Session["descuento"].ToString();
            var totImpTrasl = HttpContext.Current.Session["tot_imp_trasl"].ToString();
            var totImpRet = HttpContext.Current.Session["tot_imp_ret"].ToString();
            _total = (decimal)HttpContext.Current.Session["total"];
            var dt = (DataTable)HttpContext.Current.Session["dtArt"];
            var subtotalCap = (from DataRow dr in dt.AsEnumerable()
                where dr.RowState != DataRowState.Deleted
                select Convert.ToDecimal(dr["Uns"]) * Convert.ToDecimal(dr["Precio"])).Sum();
            var descuento = (from DataRow dr in dt.AsEnumerable()
                where dr.RowState != DataRowState.Deleted
                select (Convert.ToDecimal(dr["Uns"]) * Convert.ToDecimal(dr["Precio"])) * Convert.ToDecimal(dr["PctDesc"]) / 100).Sum();
            var dtC = (DataTable)HttpContext.Current.Session["dtCargo"];
            _cargosTotales = (from DataRow dr in dtC.AsEnumerable()
                              where dr.RowState != DataRowState.Deleted
                              select Convert.ToDecimal(dr["Importe"])).Sum();

            var totSubToCarg = subtotalCap - descuento + _cargosTotales;
            txtTotImpA.Text = string.Format(_decimales, subtotalCap - descuento);
            txtTotImpC.Text = string.Format(_decimales, _cargosTotales);
            txtCargos.Text = string.Format(_decimales, _cargosTotales);
            //txtSubTcap.Text = string.Format(_decimales, subtotalCap);
            txtSubTcap.Text = string.Format(_decimales, totSubToCarg);
            txtDescap.Text = string.Format(_decimales, descuento);
            //var ivaCap = (_cargosTotales + subtotalCap - descuento) * (Decimal.Parse(tasa) / 100);
            var ivaCap = (_cargosTotales + subtotalCap - descuento) * (Decimal.Parse(tasa) );
            txtIvacap.Text = string.Format(_decimales, ivaCap);
            var totalCap = Math.Round((subtotalCap - descuento) + _cargosTotales + (Decimal.Parse(totImpTrasl) - Decimal.Parse(totImpRet)), _numDec, MidpointRounding.AwayFromZero);
            txtTotcap.Text = string.Format(_decimales, totalCap);
            //var subTotalComprobate = Math.Round((Decimal)HttpContext.Current.Session["total_conceptos"], _numDec, MidpointRounding.AwayFromZero);
            var subTotalComprobate = Math.Round((Decimal)HttpContext.Current.Session["total_conceptos"] - Decimal.Parse(descuentoGlobal), _numDec, MidpointRounding.AwayFromZero);
            //var ivaConceptos = Math.Round((subTotalComprobate - Decimal.Parse(descuentoGlobal)) * (Decimal.Parse(tasa) / 100), _numDec, MidpointRounding.AwayFromZero);
            var ivaConceptos = Math.Round((subTotalComprobate - Decimal.Parse(descuentoGlobal)) * Decimal.Parse(tasa) , _numDec, MidpointRounding.AwayFromZero);
            var totalConceptos = Math.Round((subTotalComprobate - Decimal.Parse(descuentoGlobal)) + (Decimal.Parse(totImpTrasl) - Decimal.Parse(totImpRet)), _numDec, MidpointRounding.AwayFromZero);

            //var minimoTotal = _total - rango;
            //var maximoTotal = _total + rango;

            var error = 0;


            if (error == 0)
            {
                var minimoTotal = subTotalComprobate - _rango;
                var maximoTotal = subTotalComprobate + _rango;

                //if (subtotalCap < minimoTotal || subtotalCap > maximoTotal)
                if (totSubToCarg < minimoTotal || totSubToCarg > maximoTotal)
                {
                    msg = "El subtotal de lo capturado es incorrecto por favor verificar";
                    error = 1;
                }

            }


            if (error == 0)
            {
                var minimoTotal = decimal.Parse(descuentoGlobal) - _rango;
                var maximoTotal = decimal.Parse(descuentoGlobal) + _rango;

                if (descuento < minimoTotal || descuento > maximoTotal)
                {
                    msg = "El decuento de lo capturado es incorrecto por favor verificar";
                    error = 1;
                }

            }

            if (error == 0)
            {
                var minimoTotal = _total - _rango;
                var maximoTotal = _total + _rango;
                if (totalCap >= minimoTotal && totalCap <= maximoTotal)
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


            if (error ==0 )
            {
                txtCargos.ForeColor = System.Drawing.Color.Black;
                txtSubTcap.ForeColor = System.Drawing.Color.Black;
                txtDescap.ForeColor = System.Drawing.Color.Black;
                txtTotcap.ForeColor = System.Drawing.Color.Black;
                txtIvacap.ForeColor = System.Drawing.Color.Black;

                txtCargos.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
                txtDescap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
                txtTotcap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
                txtIvacap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
                txtSubTcap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
                lblErr.Text = "";
                if (swMsgs == 1)
                { msg = "El total de lo capturado es correcto por favor de click en Generar Factura"; }
                error = 0;
                HttpContext.Current.Session["sw_captura"] = 1;
            }
            else
            {

                txtCargos.ForeColor = System.Drawing.Color.Black;
                txtSubTcap.ForeColor = System.Drawing.Color.Black;
                txtDescap.ForeColor = System.Drawing.Color.Black;
                txtTotcap.ForeColor = System.Drawing.Color.Black;
                txtIvacap.ForeColor = System.Drawing.Color.Black;

                txtCargos.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
                txtSubTcap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
                txtDescap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
                txtTotcap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
                txtIvacap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
                lblErr.Text = "";
                if (swMsgs == 1)
                { msg = "El total de lo capturado es incorrecto por favor verificar"; }
                error = 1;
            }

            if (totalCap == 0)
            {
                lblErr.Text = "";
                if (swMsgs == 1)
                { msg = "Por favor debe capturar las partidas"; }
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
        catch (Exception ex)
        {
            msg = ex.Message;
        }
        return err1; 
    }

   
}



//private Errores CalculaTotalesE(int impMsg, int swMsgs)
//    {
//        LoadConfig();
//        var err1 = new Errores();
//        var rango = _rangoMinMax;
//        //var rango = decimal.Parse("0.5");
//        var msg = "";
//        _decimales = _numDec == 2 ? "{0:0.00}" : "{0:0.0000}";

//        try
//        {
//            var tasa = HttpContext.Current.Session["tasa"].ToString();
//            var descuentoGlobal = HttpContext.Current.Session["descuento"].ToString();
//            var totImpTrasl = HttpContext.Current.Session["tot_imp_trasl"].ToString();
//            var totImpRet = HttpContext.Current.Session["tot_imp_ret"].ToString();
//            _total = (decimal)HttpContext.Current.Session["total"];
//            var dt = (DataTable)HttpContext.Current.Session["dtArt"];
//            var subtotalCap = (from DataRow dr in dt.AsEnumerable()
//                where dr.RowState != DataRowState.Deleted
//                select Convert.ToDecimal(dr["Uns"]) * Convert.ToDecimal(dr["Precio"])).Sum();
//            var descuento = (from DataRow dr in dt.AsEnumerable()
//                where dr.RowState != DataRowState.Deleted
//                select (Convert.ToDecimal(dr["Uns"]) * Convert.ToDecimal(dr["Precio"])) * Convert.ToDecimal(dr["PctDesc"]) / 100).Sum();
//            var dtC = (DataTable)HttpContext.Current.Session["dtCargo"];
//            _cargosTotales = (from DataRow dr in dtC.AsEnumerable()
//                              where dr.RowState != DataRowState.Deleted
//                              select Convert.ToDecimal(dr["Importe"])).Sum();
//            txtTotImpA.Text = string.Format(_decimales, subtotalCap - descuento);
//            txtTotImpC.Text = string.Format(_decimales, _cargosTotales);
//            txtCargos.Text = string.Format(_decimales, _cargosTotales);
//            txtSubTcap.Text = string.Format(_decimales, subtotalCap);
//            txtDescap.Text = string.Format(_decimales, descuento);
//            var ivaCap = (_cargosTotales + subtotalCap - descuento) * (Decimal.Parse(tasa) / 100);
//            txtIvacap.Text = string.Format(_decimales, ivaCap);
//            var totalCap = Math.Round((subtotalCap - descuento) + _cargosTotales + (Decimal.Parse(totImpTrasl) - Decimal.Parse(totImpRet)), _numDec, MidpointRounding.AwayFromZero);
//            txtTotcap.Text = string.Format(_decimales, totalCap);
//            var subtotalConceptos = Math.Round((Decimal)HttpContext.Current.Session["total_conceptos"], _numDec, MidpointRounding.AwayFromZero);
//            var ivaConceptos = Math.Round((subtotalConceptos - Decimal.Parse(descuentoGlobal)) * (Decimal.Parse(tasa) / 100), _numDec, MidpointRounding.AwayFromZero);
//            var totalConceptos = Math.Round((subtotalConceptos - Decimal.Parse(descuentoGlobal)) + (Decimal.Parse(totImpTrasl) - Decimal.Parse(totImpRet)), _numDec, MidpointRounding.AwayFromZero);

//            var minimoTotal = _total - rango;
//            var maximoTotal = _total + rango;

//            var error = 0;
//            if (totalCap >= minimoTotal && totalCap <= maximoTotal )
//            {
//                txtCargos.ForeColor = System.Drawing.Color.Black;
//                txtSubTcap.ForeColor = System.Drawing.Color.Black;
//                txtDescap.ForeColor = System.Drawing.Color.Black;
//                txtTotcap.ForeColor = System.Drawing.Color.Black;
//                txtIvacap.ForeColor = System.Drawing.Color.Black;

//                txtCargos.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
//                txtDescap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
//                txtTotcap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
//                txtIvacap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
//                txtSubTcap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ccffcc");
//                lblErr.Text = "";
//                if (swMsgs == 1)
//                { msg = "El total de lo capturado es correcto por favor de click en Generar Factura"; }
//                error = 0;
//                HttpContext.Current.Session["sw_captura"] = 1;
//            }
//            else
//            {

//                txtCargos.ForeColor = System.Drawing.Color.Black;
//                txtSubTcap.ForeColor = System.Drawing.Color.Black;
//                txtDescap.ForeColor = System.Drawing.Color.Black;
//                txtTotcap.ForeColor = System.Drawing.Color.Black;
//                txtIvacap.ForeColor = System.Drawing.Color.Black;

//                txtCargos.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
//                txtSubTcap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
//                txtDescap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
//                txtTotcap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
//                txtIvacap.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcccc");
//                lblErr.Text = "";
//                if (swMsgs == 1)
//                { msg = "El total de lo capturado es incorrecto por favor verificar"; }
//                error = 1;
//            }
//            if (totalCap == 0)
//            {
//                lblErr.Text = "";
//                if (swMsgs == 1)
//                { msg = "Por favor debe capturar las partidas"; }
//                error = 1;
//            }

//            if (error != 0)
//            {
//                lblErr.Text = msg;
//                err1.Interror = error;
//                err1.Message = msg;
//            }
//        }
//        catch (Exception ex)
//        {
//            msg = ex.Message;
//        }
//        return err1; 
//    }