<%@ control language="C#" autoeventwireup="true" inherits="CapturaSimple, App_Web_p3kwygq0" %>



  
    <%--<script>
        void function calc() {
            var a1, a2;

            a1 = document.getElementById(<%="txtUns"%>).value;
            a2 = document.getElementById(<%="txtPrecio"%>).value;


            total = parseFloat(a1) + parseFloat(a2)

            document.getElementById(<%="txtImporte"%>).value = total;

        }
    </script>--%>
    <script src="Layouts/scripts/jquery-1.5.2.min.js" type="text/javascript"></script>
    <script type="text/javascript" >
            function sumar() {
            var cantidad, precio, pctDesc, subTotal, total; // Se declara la variable
            cantidad = document.getElementById("<%=txtUns.ClientID%>").value; //captura del contenido del TextBox
            precio = document.getElementById("<%=txtPrecio.ClientID%>").value;
            pctDesc = document.getElementById("<%=txtPctDesc.ClientID%>").value;
            subTotal = (parseFloat(cantidad) * parseFloat(precio));
            pctDesc = subTotal * (parseFloat(pctDesc) / 100);
            total = subTotal - pctDesc;
            document.getElementById("<%=txtImporte.ClientID%>").value = total; // El resultado en TextBox resultado
            var saldo = parseFloat('<%=Session["saldo"]%>');
                if (saldo < cantidad) {
                    //document.getElementById("<%=lblErr.ClientID%>").value = "La cantidad a facturar es mayor al saldo del acuse de recibo de mercancía";
                    //document.getElementById("<%=lblErr%>").value = "La cantidad a facturar es mayor al saldo del acuse de recibo de mercancía";
                    //document.getElementById("lblErr").value = "La cantidad a facturar es mayor al saldo del acuse de recibo de mercancía";
                    document.all("<%=lblErr.ClientID%>").innerText = "La cantidad a facturar es mayor al saldo del acuse de recibo de mercancía";
                    //$("#Error").focus().after("<span class='error'>La cantidad a facturar es mayor al saldo del acuse de recibo de mercancía</span>");
                    //document.getElementById("<%=lblErr.ClientID%>").value = 'prueba';
                    //alert("Saldo:" + saldo);
                } else {
                    document.all("<%=lblErr.ClientID%>").innerText = "";
                }

                /*if (cantidad == "0.000001") {
                    document.all("<=lblErr.ClientID%>").innerText = "La cantidad 0.000001 no es valida, en su defecto inserte la cantidad 0";
                } else {
                    document.all("<=lblErr.ClientID%>").innerText = "";
                } */
                    //document.getElementById("<=lblErr.ClientID%>").value = '';
                    //alert('mmmmmmmm');
                    //}
            }

      
    </script>

    <%--Este es para decimales --%>

    <%--<script type="text/javascript" src="funciones1.js"></script>--%>
     <%--<script type="text/javascript">
         //function AllowNumericOnly2(e) {
         function AllowNumericOnly2(event) {

             //var key = window.event.keyCode;
             var key = event.keyCode;
             var icount = 0;
             if ((key < 46 || key > 57) || key == 47) {
                 window.event.keyCode = 0;
             }
         }
    </script>
    <script type="text/javascript">
        function AllowNumericOnly(e) {
            var key = window.event.keyCode;
            if (key < 48 || key > 57) {
                window.event.keyCode = 0;
            }
        }
    </script>--%>

    <script type="text/javascript">
        /*solo enteros*/
      
        function impideletras(e) {

            

            if (window.event) {
                if (window.event.keyCode < 48 || window.event.keyCode > 57) {
                    window.event.keyCode = 0;
                }
            }
            else {
                if (e) {
                    var nav4 = window.Event ? true : false;
                    var key = nav4 ? evt.which : evt.keyCode;
                    

                    if (key.keyCode < 48 || key.keyCode > 57) {
                        //key.preventDefault();
                        key.returnValue = false;
                    }

                    //if (e.keyCode < 48 || e.keyCode > 57) {
                    //    e.preventDefault();
                    //    e.returnValue = false;
                    //}
                }
            }
        }
        /*enteros y decimales*/
        function impideletras2(e) {

          
            //if (window.event) {
            //    if ((window.event.keyCode < 48 || window.event.keyCode > 57) || window.event.keyCode == 110) {
            //        window.event.keyCode = 0;
            //    }
            //}
            var key;
            
            key = window.event.keyCode;
            key = event.keyCode;
            var icount = 0;
            if ((key < 46 || key > 57) || key == 47) {
                window.event.keyCode = 0;
            }
            else {
                if (e) {
                    if ((key < 46 || key > 57) || key == 47) {
                        e.preventDefault();
                    }
                }
            }
        }
        
        
        function soloEnteros(objeto, e) {

            var keynum;
            var keychar;
            var numcheck;

            if (window.event) { /*/ IE*/
                //keynum = e.keyCode
                //keynum = window.event.keyCode;
                if (window.event.keyCode < 48 || window.event.keyCode > 57) {
                    window.event.keyCode = 0;
                }
            }

            else if (e.which) { /*/ Netscape/Firefox/Opera/*/
                keynum = e; // || window.event;
            }

            if ((keynum >= 35 && keynum <= 37) || keynum == 8 || keynum == 9 || keynum == 46 || keynum == 39) {
                //window.event.keyCode = 0;
                return true;
            }

            if ((keynum >= 95 && keynum <= 105) || (keynum >= 48 && keynum <= 57)) {
                return true;
            } else {
                return false;
            }
        }
        


        function onlyNumbersF(e) {
            var val = (document.all);
            var key = val ? e.keyCode : e.which;
            if (key > 31 && (key < 48 || key > 57)) {
                if (val)
                    window.event.keyCode = 0;
                else {
                    e.stopPropagation();
                    e.preventDefault();
                }
            }
        }


        function onlyNumbersF2(e) {
            var val = (document.all);
            var key = val ? e.keyCode : e.which;
            if (key > 31 && (key < 48 || key > 57)) {
                if (val)
                    window.event.keyCode = 0;
                else {
                    e.stopPropagation();
                    e.preventDefault();
                }
            }
        }

    </script>
<%--<p id="demo"></p>--%>
<script type="text/javascript">

    function fieldNumber(objeto, e) {
        var valorCampo;
        //var evento_key;
        //var key
        var val = (document.all);
        evento_key = val ? e.keyCode : e.which;

        //document.getElementById("demo").innerHTML = evento_key;

        //if (window.event) {
        //    evento_key = window.event.keyCode;
        //}
        //else {
        //    evento_key = objeto || window.event;
        //}

        var numPosPunto = 0;
        var strParteEntera = "";
        var strParteDecimal = "";
        var NUM_DECIMALES = 4;
        switch (evento_key) {
        case 48:
        case 49:
        case 50:
        case 51:
        case 52:
        case 53:
        case 54:
        case 55:
        case 56:
        case 57:
        case 46:
        case 8:
            break;
        default:
            if (val) {
                window.event.keyCode = 0;
                return false;
            } else {
                //e.stopPropagation();
                //e.preventDefault();
                //event.keyCode = 0;
                //event.charCode = 0;
                //document.getElementById('keydown').innerHTML = '';
                return false;
            }
                
        }
        valorCampo = objeto.value;
        //document.getElementById("demo").innerHTML = valorCampo;
        
        if (evento_key == 46)
            if (valorCampo.indexOf(".") != -1) {

                if (val) {
                    window.event.keyCode = 0;
                    return false;
                } else {
                    //e.stopPropagation();
                    //e.preventDefault();
                    //event.keyCode = 0;
                    //event.charCode = 0;
                    //document.getElementById('keydown').innerHTML = '';
                    return false;
                }
            }
        /* Sólo puede teclear el número de decimales indicado en NUM_DECIMALES */
        if ((numPosPunto = valorCampo.indexOf(".")) != -1) {
            strParteEntera = valorCampo.substr(0, (numPosPunto - 1));
            strParteDecimal = valorCampo.substr((numPosPunto + 1), valorCampo.length)
            if (strParteDecimal.length > (NUM_DECIMALES - 1)) {

                if (val) {
                    window.event.keyCode = 0;
                    return false;
                }
                else {
                    //e.stopPropagation();
                    //e.preventDefault();
                    //event.keyCode = 0;
                    //event.charCode = 0;
                    //document.getElementById('keydown').innerHTML = '';
                    return false;
                }
            }
        }
        return true;
    }

    function validaNum(n, mini, maxi) {
        n = parseInt(n);
        if (n < mini || n > maxi) alert("El valor debe ser entre 0 - 100");
    }
</script>



  
    <script type="text/javascript">
        function mpeSeleccionOnOk() {
            //var lblpCargo = document.getElementById("lblpCargo");
            //var txtImpCargo = document.getElementById("txtImpCargo");

            //txtSituacion.value = ddlCiudades.options[ddlCiudades.selectedIndex].value + ", " +
            //    ddlMeses.options[ddlMeses.selectedIndex].value + " de " +
            //    ddlAnualidades.options[ddlAnualidades.selectedIndex].value;
        }
        function mpeSeleccionOnCancel() {
            //var txtSituacion = document.getElementById("txtSituacion");
            //txtSituacion.value = "";
            //txtSituacion.style.backgroundColor = "#FFFF99";
        }

        function ShowProgress() {
            document.getElementById('<%=UpdateProgress1.ClientID %>').style.display = "inline";
        }

    </script>

    <%--<script>
        function TriggerPostBack(control, arg) {
            __doPostBack(control, arg);
        }
    </script>--%>

   <style type="text/css">
    .overlay
    {
        position: fixed;
        z-index: 98;
        top: 0px;
        left: 0px;
        right: 0px;
        bottom: 0px;
        background-color: #aaa;
        filter: alpha(opacity=80);
        opacity: 0.8;
    }
    .overlayContent
    {
        text-align:center;
        z-index: 99;
        margin: 250px auto;
        width: 400px;
        height: 80px;
    }
    .overlayContent h2
    {
        font-size: 18px;
        font-weight: bold;
        color: #000;
    }
    .overlayContent img
    {
        width: 80px;
        height: 80px;
    }
</style>

<style type="text/css">
    /* .... */
    .FondoAplicacion
    {
        background-color: Gray;
        filter: alpha(opacity=70);
        opacity: 0.7;
    }
</style>


<style type="text/css">
    .CajaDialogo
    {
        background-color: #F2F2F2;
        border-width: 4px;
        border-style: outset;
        /*border-color: Yellow;*/
        padding: 0px;
        width: 450px;
        /*font-weight: bold;
        font-style: italic;*/
    }
    .CajaDialogo div
    {
        margin: 5px;
        text-align: center;
    }
    .auto-style2
    {
        height: 26px;
    }
</style>


<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering = "false"></asp:ScriptManager>
<%--<asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>--%>

<table style="width: 861px; " border="0">
   <%-- <tr>
        
        <td style="width:650px; text-align:right;" >
            <br />
            <br />
            <asp:Label ID="LblProveedor" runat="server" style="display:inline-block;color:#336699;font-weight:bold;TEXT-ALIGN: right;  font-family:Verdana, Arial, Helvetica, sans-serif; font-size:8.5pt" ></asp:Label>
        </td>
         </tr>--%>
    <tr>
        <td colspan="2" style="display: inline-block; color: #336699; font-family:Arial ">
            <asp:Label ID="lblVe1" runat="server" Text="Vendido A: " Font-Bold="true" ></asp:Label><asp:Label ID="LblEmpresa" runat="server"
                Width="750px"></asp:Label>
            <asp:Label ID="Lblf1" runat="server" Text="Factura: " Font-Bold="true"/>
            <asp:Label ID="folio_fac" runat="server"></asp:Label>
            <asp:Label ID="lbls1" runat="server" Text=" Serie: " Font-Bold="true"></asp:Label>
            <asp:Label ID="serie_fac" runat="server"></asp:Label>
            <asp:Label ID="lblu1" runat="server" Text="Folio Fiscal (UUID):" Font-Bold="true"></asp:Label>
            <asp:Label ID="uuid" runat="server"></asp:Label>
               
            <br />
            <br />
        </td>
   </tr>
</table>


        <%--    <asp:Label ID="TxtMensajes" runat="server" Text="" Font-Bold="True" ForeColor="Maroon"></asp:Label>
            <br />--%>

<table id="DatosGenerales" style="width: 861px; margin-left: 20px" border="0">

 <%--   <tr>
        
        <td></td>
        <td></td>
        <td></td>
        <td></td>
       
    </tr>

    <tr>
        <td></td>
        <td></td>
        <td></td>
    </tr>--%>

    <tr>
        <td>
            <asp:HiddenField ID="hfRfc" Value="" runat="server" />
            <asp:HiddenField ID="hfSerie" Value="" runat="server" />
            <asp:HiddenField ID="hfFolio" Value="" runat="server" />
            <asp:HiddenField ID="hfUUID" Value="" runat="server" />
        </td>
        <td ></td>
        <td ></td>
    </tr>

    
    <tr>
         <td></td><td>&nbsp;</td><td></td>
       <td></td>
        <td></td>
        <td></td>
    </tr>

    <tr>
       <td>
            <%--GCM 05012015--%>
           <%--<asp:Label ID="Label1" runat="server" Text="Metodo de Pago"></asp:Label>--%>
        </td>
        <td>
        <%--GCM 05012015--%>
            <%--<asp:DropDownList ID="ddlMetodo" runat="server"  Width="235px" DataSourceID="xmldsMetodo" DataTextField="nombre" DataValueField="ID">
            </asp:DropDownList>--%>
        </td>
        <td></td>
        <td>
           </td>
        <td></td>
        <td> 
            <%--<asp:UpdatePanel ID="upnlCargos" runat="server">
            <ContentTemplate>--%>
                <asp:Button class="btnCommand" ID="btnCargos" runat="server" Text="Cargos Extra" />

           <%-- </ContentTemplate>
        </asp:UpdatePanel>--%>
        </td>
        
    </tr>

    <tr>
        <td>
        <%--GCM 05012015--%>
            <%--<asp:Label ID="Label2" runat="server" Text="Moneda"></asp:Label>--%>
        </td>
        <td>
        <%--GCM 05012015--%>
            <%--<asp:DropDownList ID="ddlMoneda" runat="server"  Width="235px" DataSourceID="xmldsMoneda" DataTextField="mostrar" DataValueField="ID">
            </asp:DropDownList>--%>
        </td>
        <td></td>
        <td>
           </td>
        <td></td>
        <td></td>
    </tr>

    <tr>
        <td>
            <asp:Label ID="lblEt1" runat="server" Text="Subtotal:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txtSubtotal" runat="server" Text="0.0000" Enabled="False"  style="text-align:right"  ></asp:TextBox>
        </td>
        <td></td>
        <td>
            <asp:Label ID="lblEt7" runat="server" Text="Impuestos:" />
        </td>
        <td>
            

        </td>
        <td>
         &nbsp;</td>
    </tr>

    <tr>
        <td>
            <asp:Label ID="lblEt2" runat="server" Text="Descuento:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txtDcto" runat="server" Text="0.0000" Enabled="False"  style="text-align:right" ></asp:TextBox>
        </td>
        <td></td>
        <td>
            <%--<asp:Label ID="lblEt8" runat="server" Text="Trasladados:" />--%>
        </td>
        <td>
            
            <asp:Label ID="lblEt11" runat="server" Text="Total Trasladado:" />
        </td>
        <td>
            <asp:TextBox ID="txtImporteT" runat="server" Enabled="False" Text="0.0000" style="text-align:right" />
        </td>
    </tr>

    <tr>
         <td>
            <asp:Label ID="lblEt3" runat="server" Text="Total:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txtTotal" runat="server" Text="0.0000" Enabled="False"  style="text-align:right" ></asp:TextBox>
        </td>
        <td></td>
        <td></td>
        <td>
            <asp:Label ID="lblEt12" runat="server" Text="Total Retenido:" />
        </td>
        <td>
            
            <asp:TextBox ID="txtImporteR" runat="server" Enabled="False" Text="0.0000"  style="text-align:right"    />
            
        </td>
    </tr>

    <tr>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
    </tr>
    <tr>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
    </tr>
</table>
<asp:XmlDataSource ID="xmldsMetodo" runat="server" DataFile="~/App_Data/Catalogos.xml" XPath="Catalogos/metodosDePago/metodoDePago" ></asp:XmlDataSource>
<asp:XmlDataSource ID="xmldsMoneda" runat="server" DataFile="~/App_Data/Catalogos.xml" XPath="Catalogos/Monedas/Moneda" ></asp:XmlDataSource>


<%-- <asp:GridView ID="grvEncabezados" runat="server" AutoGenerateColumns="True" Visible =" true"
                    HeaderStyle-Font-Bold="true" HeaderStyle-ForeColor="white" HeaderStyle-Height="40px"
                    HeaderStyle-BackColor="#336699" Font-Size="XX-Small"
                    HeaderStyle-HorizontalAlign="Center" ShowHeaderWhenEmpty="true"
                    HorizontalAlign="Center"  >
                    <Columns>
                        <asp:TemplateField HeaderText="Folio Recepción CFD" SortExpression="referencia" ItemStyle-Width="80px" />
                        <asp:TemplateField HeaderText="Partida" SortExpression="partida" ItemStyle-Width="50px" />    
                        <asp:TemplateField HeaderText="Clave" SortExpression="clave" ItemStyle-Width="200px" />    
                        <asp:TemplateField HeaderText="UM" SortExpression="um" ItemStyle-Width="80px" />    
                        <asp:TemplateField HeaderText="Precio" SortExpression="precio" ItemStyle-Width="80px" />    
                        <asp:TemplateField HeaderText="Porcentaje de descuento" SortExpression="pctdesc" ItemStyle-Width="80px" />     
                        <asp:TemplateField HeaderText="Importe" SortExpression="importe" ItemStyle-Width="80px" />     
                    </Columns>
                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                </asp:GridView>--%>

<%--style="font-size:xx-small;"--%>
<table  class="mGrid2"  >
    <tr>
        <th style="width:92px;" > Folio Recepción CFD <%--<asp:Label Width="100px"    ID="lblRef" runat="server" Text="Folio Recepción CFD" Font-Bold="true"></asp:Label>--%></th>
        <th style="width:65px;">Partida<%--<asp:Label Width="65px"    ID="lblNoP" runat="server" Text="Partida" Font-Bold="true"></asp:Label>--%></th>
        <th style="width:185px;">Clave<%--<asp:Label Width="185px"   ID="lblCve" runat="server" Text="Clave" Font-Bold="true"></asp:Label>--%></th>
        <th style="width:90px;">Cantidad<%--<asp:Label Width="85px"    ID="lbluns" runat="server" Text="Cantidad" Font-Bold="true"></asp:Label>--%></th>
        <th style="width:110px;">UM<%--<asp:Label Width="100px"   ID="lblUM" runat="server" Text="UM" Font-Bold="true"></asp:Label>--%></th>
        <th style="width:110px;">Precio<%--<asp:Label Width="100px"    ID="lblPrecio" runat="server" Text="Precio" Font-Bold="true"></asp:Label>--%></th>
        <th style="width:90px;" >Porcentaje de descuento<%--<asp:Label Width="100px"    ID="lblPct" runat="server" Text="Porcentaje de descuento" Font-Bold="true"></asp:Label>--%></th>
        <th style="width:110px;">Importe<%--<asp:Label Width="90px"    ID="lblImporte" runat="server" Text="Importe" Font-Bold="true"></asp:Label>--%></th>
    </tr>
</table>

<asp:UpdatePanel ID="upnl1" runat="server">
    <ContentTemplate>
        <table>
            <%--<tr>
                <td ><asp:Label Width="80px"    ID="lblRef" runat="server" Text="Folio Recepción CFD" Font-Bold="true"></asp:Label></td>
                <td ><asp:Label Width="50px"    ID="lblNoP" runat="server" Text="Partida" Font-Bold="true"></asp:Label></td>
                <td ><asp:Label Width="200px"   ID="lblCve" runat="server" Text="Clave" Font-Bold="true"></asp:Label></td>
                <td ><asp:Label Width="80px"    ID="lbluns" runat="server" Text="Cantidad" Font-Bold="true"></asp:Label></td>
                <td ><asp:Label Width="100px"   ID="lblUM" runat="server" Text="UM" Font-Bold="true"></asp:Label></td>
                <td ><asp:Label Width="80px"    ID="lblPrecio" runat="server" Text="Precio" Font-Bold="true"></asp:Label></td>
                <td ><asp:Label Width="80px"    ID="lblPct" runat="server" Text="Porcentaje de descuento" Font-Bold="true"></asp:Label></td>
                <td ><asp:Label Width="80px"    ID="lblImporte" runat="server" Text="Importe" Font-Bold="true"></asp:Label></td>
                <td ></td>
            </tr>--%>
            <tr>
                <td>
                    <asp:TextBox Width="90px" ID="txtReferencia" Text='' runat="server" ToolTip="Folio Recepción CFD"  MaxLength="16"  ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvReferencia" runat="server" 
                        Style="color: #FF6600; font-size: xx-small; font-family: Arial;" Font-Bold="True"
                        ControlToValidate="txtReferencia"
                        ErrorMessage="Referencia requerida"
                        ToolTip="El formato de la referencia es documento@folio ejemplo itrmn@145."
                        ValidationGroup="GrupoDatos">*
                    </asp:RequiredFieldValidator>
                </td>
                <td>
                    <%--impideletras(event);--%>
                    <%--soloEnteros(this,event);--%>
                    <asp:TextBox  Width="50px" ID="txtPartida" ToolTip="No Partida" runat="server" 
                        OnTextChanged="txtPartida_TextChanged" onkeypress="onlyNumbersF(event);"
                        onblur="__doPostBack('txtPartida','OnBlur');"
                        MaxLength="5"></asp:TextBox><%-- OnTextChanged="txtPartida_TextChanged"  --%>  <%--AutoPostBack="True"  onblur="TriggerPostBack('textBox', document.getElementById('txtPartida').value)"--%>
                   <%-- Text='0' --%>
                      <%--onkeypress=" return AllowNumericOnly(this);" --%>
                    <%-- AutoPostBack="True" --%>
                   <%-- onkeyup="var pattern = /[^0-9]/g; this.value = this.value.replace(pattern, '');"--%>
                   <asp:RequiredFieldValidator ID="rfvPartida" runat="server"
                        Style="color: #FF6600; font-size: xx-small; font-family: Arial;" Font-Bold="True"
                        ControlToValidate="txtPartida" 
                        ErrorMessage="Partida requerida"
                        ToolTip="La partida es requerida para desplegar las claves de articulo." 
                        ValidationGroup="GrupoDatos">*
                    </asp:RequiredFieldValidator>
                </td>
                <td>
                    <asp:DropDownList Width="180px" ID="ddlNoIdentificacion" runat="server" OnSelectedIndexChanged="ddlNoIdentificacion_SelectedIndexChanged" ToolTip="Clave de Articulo"></asp:DropDownList>
                   
                </td>
                <td>
                 <%--   onkeyup="var pattern = /[^0-9\.]/g; this.value = this.value.replace(pattern, '');" --%>
                    <asp:TextBox Width="80px" ID="txtUns" 
                         Text='0' runat="server" Height="16px" onkeypress="fieldNumber(this, event)" OnBlur="sumar()" MaxLength="20"></asp:TextBox>
                     <asp:RequiredFieldValidator ID="rfvUns" runat="server"
                        Style="color: #FF6600; font-size: xx-small; font-family: Arial;" Font-Bold="True"
                        ControlToValidate="txtUns" 
                        ErrorMessage="Cantidad requerida" 
                        ToolTip="Por favor de la cantidad" 
                        ValidationGroup="GrupoDatos">*
                    </asp:RequiredFieldValidator>
                </td>
                
                <td>
                   
                    <%--<asp:DropDownList Width="100px" ID="ddUM" runat="server"  DataSourceID="XmlDataSource4"  DataTextField="mostrar" DataValueField="ID" Enabled="False">
                    </asp:DropDownList>--%>
                    <asp:DropDownList Width="100px" ID="ddUM" runat="server"  Enabled="False">
                    </asp:DropDownList>
                     <asp:RequiredFieldValidator ID="rfvUM" runat="server" 
                        Style="color: #FF6600; font-size: xx-small; font-family: Arial;" Font-Bold="True"
                        ControlToValidate ="ddUM" 
                        ErrorMessage="Unidad de medida requerida" 
                        ToolTip="Unidad de Medida" 
                        ValidationGroup="GrupoDatos">*
                    </asp:RequiredFieldValidator>
                </td>
                <td>
                    <%--onkeyup="var pattern = /[^0-9\.]/g; this.value = this.value.replace(pattern, '');"--%>
                    <asp:TextBox Width="80px" ID="txtPrecio" Text='0' 
                        runat="server"   onkeypress="fieldNumber(this, event)" OnBlur="sumar()" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPrecio" runat="server" 
                        Style="color: #FF6600; font-size: xx-small; font-family: Arial;" Font-Bold="True"
                        ControlToValidate ="txtPrecio" 
                        ErrorMessage="Precio requerido"   
                        ToolTip="Precio" 
                        ValidationGroup="GrupoDatos">*
                    </asp:RequiredFieldValidator>
                </td>
                <td>
                   <%-- onkeyup="var pattern = /[^0-9\.]/g; this.value = this.value.replace(pattern, '');"--%>
                    <asp:TextBox Width="80px" ID="txtPctDesc" Text='0' OnBlur="sumar()"
                        runat="server"   onkeypress="fieldNumber(this, event)"></asp:TextBox>
                      <asp:RequiredFieldValidator ID="rfvPctDesc" runat="server"
                        Style="color: #FF6600; font-size: xx-small; font-family: Arial;" Font-Bold="True"
                        ControlToValidate="txtPctDesc" 
                        ErrorMessage="Porcentaje de descuento requerido" 
                        ToolTip="Por favor de el porcentaje de descuento requerido" 
                        ValidationGroup="Grupo<Datos">*
                      </asp:RequiredFieldValidator>
                </td>
                <td>
                    <%--onkeyup="var pattern = /[^0-9\.]/g; this.value = this.value.replace(pattern, '');"--%>
                    <%--Text='0' --%>
                    <asp:TextBox Width="80px" ID="txtImporte" 
                        runat="server"   onkeypress="impideletras2(event);"  Enabled="false"></asp:TextBox>
                    <br />
                </td>
                <td>
                    <%--<asp:Button ID="btnAgregar" runat="server" Text="Agregar" OnClick="btnAgregar_Click"  ValidationGroup="GrupoDatos" />--%>
                    <asp:ImageButton ID="imgbtnAgregar" runat="server" ImageUrl="~/Layouts/images/botones/add.png" OnClick="imgbtnAgregar_Click" ValidationGroup="GrupoDatos" style="width: 16px; height: 16px;"/>
                    <%--<asp:Button ID="btnAdd" runat="server" Text="+"  OnClick="btnAdd_Click" ValidationGroup="GrupoDatos" Width="8px" Height ="15px" />--%>
                    <%--<asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Layouts/images/botones/add.png" OnClick="ImageButton1_Click" />--%>
                </td>
            </tr>
           
            <tr>
                <td colspan="9">

                <asp:GridView ID="gvArticulos" runat="server" OnSelectedIndexChanged="gvArticulos_SelectedIndexChanged" OnRowDeleting = "gvArticulos_RowDeleting" 
                    OnRowDeleted = "gvArticulos_RowDeleted"  AutoGenerateColumns="false" ShowHeader="false"  OnRowDataBound="gvArticulos_RowDataBound"
                     HeaderStyle-Font-Bold="true" HeaderStyle-ForeColor="white" HeaderStyle-Height="40px" ShowFooter="False"   CssClass="mGrid"   
                    HeaderStyle-BackColor="#336699" Font-Size="XX-Small" > 
                  <Columns>  

                     <asp:TemplateField HeaderText="Referencia" SortExpression="referencia" ItemStyle-Width="95px" 
                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lbl1" Text='<%# Eval("Referencia") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Partida" SortExpression="partida" ItemStyle-Width="70px" 
                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lbl2" Text='<%# Eval("Partida") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Clave" SortExpression="clave" ItemStyle-Width="185px" 
                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lbl3" Text='<%# Eval("NoIdentificacion") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Cantidad" SortExpression="uns" ItemStyle-Width="83px" 
                            ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lbl4" Text=' <%# String.Format("{0:f4}", DataBinder.Eval(Container.DataItem, "Uns")) %>' runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="UM" SortExpression="um" ItemStyle-Width="110px" 
                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lbl5" Text='<%# Eval("UM") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Precio" SortExpression="precio" ItemStyle-Width="90px" 
                            ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lbl6" Text=' <%# String.Format("{0:f4}", DataBinder.Eval(Container.DataItem, "Precio")) %>' runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField> 
                                 
                    <asp:TemplateField HeaderText="Descuento" SortExpression="pctdesc" ItemStyle-Width="90px" 
                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right" HeaderStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label ID="lbl7" Text='<%# String.Format("{0:f4}", DataBinder.Eval(Container.DataItem, "PctDesc")) %>' runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
    

                    <asp:TemplateField HeaderText="Importe" SortExpression="Importe" ItemStyle-Width="90px" 
                            ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <%--<asp:Label ID="lbl8" Text='<%# Eval("Importe") %>' runat="server"></asp:Label>--%>
                            <asp:Label ID="lbl8" Text='<%# String.Format("{0:f4}", DataBinder.Eval(Container.DataItem, "Importe")) %>' runat="server"></asp:Label>
                        </ItemTemplate>
                       <%-- <FooterTemplate>
                            <asp:Label ID="lblSubtAart" runat="server" Text="Subtotal:" />
                        </FooterTemplate>--%>
                    </asp:TemplateField>


                    <asp:TemplateField  ItemStyle-Width="10px">
                        <ItemTemplate>
                            <%--<asp:LinkButton ID="linkDeleteCust" CommandName="Delete" runat="server">Borrar</asp:LinkButton>--%>
                            <asp:ImageButton ID="imgbtnDeleteA" runat="server" CommandName="Delete" ImageUrl="~/Layouts/images/botones/gtk-remove.png" />
                          <%--  <asp:Button ID="btndelete" runat="server" Text="-"  CommandName="Delete" Width="8px" Height ="15px" />--%>
                       </ItemTemplate>
                   <%--    <FooterTemplate>
                            <asp:TextBox ID="txtSubtArt" runat="server" Text="0.00"></asp:TextBox>
                        </FooterTemplate>--%>
                    </asp:TemplateField>

                  </Columns>
                </asp:GridView>

                </td>
            </tr> 
            <tr>
                <td colspan="8" style="text-align:right;" class="auto-style2">
                     <asp:Label Width="100px" ID="lblTotImpA" runat="server" Text="Importe:" />
                    <asp:TextBox ID="txtTotImpA" Text ="0.0000" runat="server" Width="80px" Enabled="false"  style="text-align:right" ></asp:TextBox>
                </td>
            </tr> 
             <tr>
                <td colspan="8">
                    <asp:Label ID="lblErr" runat="server" Text="" Font-Bold="True"  Style="color: #FF6600; font-size: medium; font-family: Arial;"></asp:Label>
                    <span id="Error"></span>
                  
                     
                    <asp:ValidationSummary ID="ValSum" runat="server"
                        Style="color: #FF6600; font-size: medium; font-family: Arial;" Font-Bold="True"
                        ValidationGroup="GrupoDatos" DisplayMode="List" 
                        ForeColor="Maroon"  />
                </td>
            </tr>  
        </table>
        <asp:XmlDataSource ID="XmlDataSource4" runat="server" 
                    DataFile="~/App_Data/Catalogos.xml"  XPath="Catalogos/ums/um" ></asp:XmlDataSource>
       <%-- <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:developConnectionString %>" SelectCommand="sp_consInfXML" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:SessionParameter Name="rfc_emisor" SessionField="doc_1" Type="String" />
                <asp:SessionParameter Name="serie" SessionField="ef_cve" Type="String" />
                <asp:SessionParameter Name="folio_factura" SessionField="fol_1" Type="Int64" />
                <asp:SessionParameter Name="num_info" SessionField="4" Type="Int16" />
                <asp:Parameter DefaultValue="Hola" Name="uuid" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>--%>

       
        
             
       





    </ContentTemplate>
    <Triggers> 
    <asp:AsyncPostBackTrigger 
        ControlID="txtPartida" 
        EventName="TextChanged" /> 
</Triggers>
</asp:UpdatePanel>


<%--Style="display: none;"--%>
<asp:Panel ID="pnlSeleccionarDatos" runat="server" CssClass="CajaDialogo" Style="display: none;" >
    <%--<div style="padding: 10px; background-color: #336699; color: #FFFFFF;">--%>
        <div style="padding: 10px;" >
        <asp:Label ID="Label4" runat="server" Text="Cargos Extra" Font-Bold="true" />
    </div>
    <div>
        <asp:UpdatePanel ID="upnl2" runat="server">
        <ContentTemplate>
           <table class="mGrid2" >
                <tr>
                <th style="width:100px;">Clave
                    <%--<asp:Label  Width="100px" ID="lblpcve" runat="server" Text="Clave" />--%>
                </th>
                <th style="width:200px;">Cargo
                   <%-- <asp:Label Width="200px" ID="lblpCargo" runat="server" Text="Cargo" />--%>
                </th>
                <th style="width:120px;">Importe
                  <%--  <asp:Label Width="100px" ID="lblimpC" runat="server" Text="Importe" />--%>
                </th>
             </tr>
            </table>

        <table>
            <tr>
                <td style="width:173px;"></td>
                <td style="width:210px;">
                    <asp:DropDownList Width="150px" ID="ddlCargo" runat="server" DataSourceID="xmlCargos"  DataTextField="nombre" DataValueField="ID" OnSelectedIndexChanged="ddlCargo_SelectedIndexChanged"/>
                    <asp:RequiredFieldValidator ID="rqvddlCargo" runat="server" 
                        Style="color: #FF6600; font-size: xx-small; font-family: Arial;" Font-Bold="True"
                        ControlToValidate ="ddlCargo" 
                        ErrorMessage="Cargo requerido" 
                        ToolTip="Cargos que se abonan al total de la factura" 
                        ValidationGroup="GrupoDatosCargos">*
                    </asp:RequiredFieldValidator>

                    
                </td>
         
                
                <td style="width:100px;">
                    <asp:TextBox Width="80px" ID="txtImpCargo" runat="server" Text='0' 
                         onkeypress="fieldNumber(this, event)"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rqvImpCargo" runat="server" 
                        Style="color: #FF6600; font-size: xx-small; font-family: Arial;" Font-Bold="True"
                        ControlToValidate ="txtImpCargo" 
                        ErrorMessage="Importe de cargo requerido" 
                        ToolTip="Importe del cargo seleccionado" 
                        ValidationGroup="GrupoDatosCargos">*
                    </asp:RequiredFieldValidator>
                    <%--onkeyup="var pattern = /[^0-9\.]/g; this.value = this.value.replace(pattern, '');"--%>
                    <%--onkeyup="var pattern = /[^0-9\.]/g; this.value = this.value.replace(pattern, '');"--%>
                </td>
                <td>
                    <%--<asp:Button ID="btnAgregarCargo" runat="server" Text="+" OnClick="btnAgregarCargo_Click" ValidationGroup="GrupoDatosCargos" Width="16px" Height ="15px" />--%>
                    <asp:ImageButton ID="imgbtnAgregarC" runat="server" ImageUrl="~/Layouts/images/botones/add.png"  OnClick="imgbtnAgregarC_Click" ValidationGroup="GrupoDatosCargos" />
                    
                </td>
            </tr>
          
            <tr>
                <td colspan ="3">
                     <asp:Label ID="lblErrCargo" runat="server" Text="" Font-Bold="True"  Style="color: #FF6600; font-size: smaller; font-family: Arial;"></asp:Label>
                     <asp:ValidationSummary ID="vsumCargo" runat="server"
                        Style="color: #FF6600; font-size: smaller; font-family: Arial;" Font-Bold="True"
                        ValidationGroup="GrupoDatosCargos" DisplayMode="List" 
                        ForeColor="Maroon"  />
                </td>
                
            </tr>
            <tr>

                <td>
                     <asp:XmlDataSource ID="xmlCargos" runat="server" 
                    DataFile="~/App_Data/Catalogos.xml"  XPath="Catalogos/CargosExtra/CargoExtra" ></asp:XmlDataSource>
                </td>
            </tr>

            <%-- <tr>
                <td colspan ="3">--%>
                
                <%--</td>
            </tr>--%>
        </table>

            <asp:GridView ID="gvCargos" runat="server" OnRowDeleting = "gvCargos_RowDeleting"  ShowHeader="false"
                    OnRowDeleted = "gvCargos_RowDeleted"  AutoGenerateColumns="false" ShowFooter="False" OnRowDataBound="gvCargos_RowDataBound"
                          HeaderStyle-Font-Bold="true" HeaderStyle-ForeColor="white" CssClass="mGrid"
                    HeaderStyle-BackColor="#336699" Font-Size="XX-Small"
                        >
                        <Columns>
                            <asp:TemplateField HeaderText="Clave" SortExpression="clave" ItemStyle-Width="92px" 
                                ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:Label ID="lblc1" Text='<%# Eval("NoIdentificacion") %>' runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                             <asp:TemplateField HeaderText="Cargo" SortExpression="clave" ItemStyle-Width="210px" 
                                ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:Label ID="lblc2" Text='<%# Eval("Description") %>' runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Importe" SortExpression="Importe" ItemStyle-Width="80px" 
                                    ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:Label ID="lblc3" Text='<%# String.Format("{0:f2}", DataBinder.Eval(Container.DataItem, "Importe")) %>' runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField >
                                <ItemTemplate>
                                    <%--<asp:LinkButton ID="linkDeleteCust" CommandName="Delete" runat="server">Borrar</asp:LinkButton>--%>
                                    <asp:ImageButton ID="imgbtnDeleteC" runat="server" CommandName="Delete" ImageUrl="~/Layouts/images/botones/gtk-remove.png"   />
                                    <%--<asp:Button ID="btnDeleteC" runat="server" Text="-" CommandName="Delete" Width="8px" Height ="15px"  />--%>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                    </asp:GridView>
            
            <table>
                <tr>
                    <td style="width:400px; text-align:right; ">
                        <asp:Label Width="100px" ID="lblTotImpC" runat="server" Text="Importe:" />
                    <asp:TextBox ID="txtTotImpC" Text ="0.00" runat="server" Width="80px" Enabled="false"  style="text-align:right" ></asp:TextBox>
                    </td>    
                </tr>
            </table>
        </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div>
        <asp:Button class="btnCommand" ID="btnPAceptar" runat="server" Text="Regresar" />
        &nbsp;&nbsp;
        <%--<asp:Button ID="btnPCancela" runat="server" Text="Cancelar" Visible="false" />--%>
    </div>
</asp:Panel>



 <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server"
     TargetControlID="btnCargos"
    PopupControlID="pnlSeleccionarDatos" 
    OkControlID="btnPAceptar" 
    OnOkScript="mpeSeleccionOnOk()"
    DropShadow="True"
    BackgroundCssClass="FondoAplicacion"
     ></ajaxToolkit:ModalPopupExtender>

<%--<asp:LinkButton ID="linkDeleteCust" CommandName="Delete" runat="server">Borrar</asp:LinkButton>--%>
    <%--              
<asp:UpdatePanel ID="pnlCargosIni" runat="server" >
    <ContentTemplate>
       
    </ContentTemplate>
</asp:UpdatePanel>--%>

<asp:UpdatePanel ID="pdnlTotales" runat="server" CssClass="CajaDialogo">
    <ContentTemplate>
        
        

        <table id="TotalesCapturados" >
          <tr>
              <td style="width:570px;"></td>
                <td class="auto-style2"><asp:Label ID="lblSubTcap0" runat="server" Text="Captura:"></asp:Label></td><td></td>
                 
                </tr>
  
                <tr>
                    <td ></td>
                    <td  style="text-align:right" ><asp:Label ID="lblCargotot" runat="server" Text="Cargos Extra:"></asp:Label></td>
                    <td class="style1">
                        <asp:TextBox ID="txtCargos" runat="server" Text="0.0000" Enabled="False" style="text-align:right" ></asp:TextBox>
                    </td>
       
                </tr>

  
                <tr>
                     <td ></td>
                     <td  style="text-align:right" ><asp:Label ID="lblSubTcap" runat="server" Text="Subtotal:"></asp:Label></td>
                     <td class="style1">
                         <asp:TextBox ID="txtSubTcap" runat="server" Text="0.0000" Enabled="False" style="text-align:right" ></asp:TextBox>
                     </td>
                </tr>

                <tr>
                    <td ></td>
                    <td style="text-align:right">
                    <asp:Label ID="lblDescap" runat="server" Text="Descuento:"></asp:Label>
                    </td>
                    <td class="style1">
                    <asp:TextBox ID="txtDescap" runat="server" Text="0.0000" Enabled="False"  style="text-align:right" ></asp:TextBox>
                    </td>
                </tr>

                <tr>
                    <td ></td>
                    <td style="text-align:right">
                        <asp:Label ID="lblTotcap" runat="server" Text="Total:"></asp:Label>
                    </td>
                    <td class="style1">
                         <asp:TextBox ID="txtTotcap" runat="server" Text="0.0000" Enabled="False"  style="text-align:right" ></asp:TextBox>
                    </td>
                </tr>

                <tr>
                   <td ></td>
                   <td class="auto-style2">
                        <asp:Label ID="lblIvacap" runat="server" Text="Total Iva de Captura:" Visible="False"></asp:Label>
                    </td>
                    <td class="style1">
                         <asp:TextBox ID="txtIvacap" runat="server" Text="0.0000" Enabled="False" Visible="False"  style="text-align:right" ></asp:TextBox>
                    </td>
                   
                    
                </tr>

                <tr>
                    <td ></td>
                    <td ></td>
                    <td ></td>
                </tr>
           
              <%-- <%# String.Format("{0:f2}", DataBinder.Eval(Container.DataItem, "cantidad"))%>--%>
                </table>
   


   <table style="width: 861px; text-align:center;  " border="0">
    <tr>
        <td style =" width:60px "></td>
        <td>
        <asp:Button class="btnCommand" ID="LnkGenerarFactura" runat="server" Text="Generar Factura"  OnClientClick="ShowProgress()" onclick="LnkGenerarFactura_Click" />
        </td>
        <%--
             <ul class="tabs" id=”button”>
 	                    <li><a><asp:LinkButton ID="LnkGenerarFactura" runat="server"  ForeColor =White 
                               OnClientClick="ShowProgress()"  onclick="LnkGenerarFactura_Click">Generar Factura</asp:LinkButton></a></li>
                    </ul>
        </td>--%>
        <td></td>
    </tr>
    </table>
    </ContentTemplate>
     <Triggers>
        <%--<asp:PostBackTrigger ControlID="LnkGenerarFactura" />--%>
        <asp:AsyncPostBackTrigger ControlID="LnkGenerarFactura" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>


<asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0" AssociatedUpdatePanelID="upnl2">
    <ProgressTemplate>
       <div class="overlay" />
        <div class="overlayContent">
            <h2>
                La factura está siendo procesada espere un momento...</h2>
            <img src="Layouts/images/img/loading.gif" alt="Loading" border="0" />
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
