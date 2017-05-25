<%@ control language="C#" autoeventwireup="true" inherits="FacturaEmbarque, App_Web_tkxgaow2" %>

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
    .style1
    {
        width: 161px;
    }
    .style2
    {
        width: 133px;
    }
</style>
<script type="text/javascript">

    function fieldNumber(objeto, e) {
        var valorCampo;
        var val = (document.all);
        evento_key = val ? e.keyCode : e.which;



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

    function ShowProgress() {
        document.getElementById('<%=UpdateProgress1.ClientID %>').style.display = "inline";
    }
    
</script>


<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="false">
<%--<asp:ScriptManager ID="ScriptManager1" runat="server" >--%>
</asp:ScriptManager>
<table style="width: 861px;" border="0">
    <tr>
        <td colspan="2" style="display: inline-block; color: #336699; font-family: Arial">
            <asp:Label ID="lblVe1" runat="server" Text="Vendido A: " Font-Bold="true"></asp:Label><asp:Label
                ID="LblEmpresa" runat="server" Width="750px"></asp:Label>
            <asp:Label ID="Lblf1" runat="server" Text="Factura: " Font-Bold="true" />
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
<asp:UpdatePanel ID="upnl1" runat="server">
    <ContentTemplate>
        <table id="DatosGenerales" style="width: 861px; margin-left: 20px" border="0">
            <tr>
                <td class="style2">
                    <asp:HiddenField ID="hfRfc" Value="" runat="server" />
                    <asp:HiddenField ID="hfSerie" Value="" runat="server" />
                    <asp:HiddenField ID="hfFolio" Value="" runat="server" />
                    <asp:HiddenField ID="hfUUID" Value="" runat="server" />
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="style2">
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    <asp:Label ID="Label5" runat="server" Text="Transporte:"></asp:Label>
                </td>
                <td>
                    <%--<asp:DropDownList ID="ddlTransporte" runat="server"  Width="235px" onblur="__doPostBack('ddlTransporte','OnBlur');" ></asp:DropDownList> --%>
                    <asp:DropDownList ID="ddlTransporte" runat="server" Width="235px" OnSelectedIndexChanged="ddlTransporte_SelectedIndexChanged"
                        AutoPostBack="True">
                    </asp:DropDownList>
                </td>
                <td>
                </td>
                <td>
                    <asp:Label ID="lblEt1" runat="server" Text="Subtotal:"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                    <asp:TextBox ID="txtSubtotal" runat="server" Text="0.0000" Enabled="False" Style="text-align: right"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    <asp:Label ID="Label3" runat="server" Text="Placa:"></asp:Label>
                </td>
                <td>
                    <%--<asp:DropDownList ID="ddlPlaca" runat="server"  Width="235px" onblur="__doPostBack('ddlPlaca','OnBlur');" ></asp:DropDownList> --%>
                    <asp:DropDownList ID="ddlPlaca" runat="server" Width="235px" OnSelectedIndexChanged="ddlPlaca_SelectedIndexChanged"
                        AutoPostBack="True">
                    </asp:DropDownList>
                </td>
                <td>
                </td>
                <td>
                    <asp:Label ID="lblEt2" runat="server" Text="Descuento:"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                    <asp:TextBox ID="txtDcto" runat="server" Text="0.0000" Enabled="False" Style="text-align: right"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    <%--GCM 05012015--%>
                    <%--<asp:Label ID="Label1" runat="server" Text="Metodo de Pago:"></asp:Label>--%>
                </td>
                <td>
                    <%--GCM 05012015--%>
                    <%--<asp:DropDownList ID="ddlMetodo" runat="server" Width="235px" DataSourceID="xmldsMetodo"
                        DataTextField="nombre" DataValueField="ID">
                    </asp:DropDownList> --%>
                </td>
                <td>
                </td>
                <td>
                    <asp:Label ID="lblEt3" runat="server" Text="Total:"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                    <asp:TextBox ID="txtTotal" runat="server" Text="0.0000" Enabled="False" Style="text-align: right"></asp:TextBox>
                </td>
            </tr>
            <tr>

                <td class="style2">
                    <%--GCM 05012015--%>
                    <%--<asp:Label ID="Label2" runat="server" Text="Moneda:"></asp:Label>--%>
                </td>
                <td>
                    <%--<asp:DropDownList ID="ddlMoneda" runat="server" Width="235px" DataSourceID="xmldsMoneda"
                        DataTextField="mostrar" DataValueField="ID">
                    </asp:DropDownList>--%>
                </td>
                <td>
                </td>
                <td>
                    <asp:Label ID="lblEt7" runat="server" Text="Impuestos:" />
                </td>
                <td>
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="style2">
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                    <asp:Label ID="lblEt11" runat="server" Text="Total Trasladado:" />
                </td>
                <td>
                    <asp:TextBox ID="txtImporteT" runat="server" Enabled="False" Text="0.0000" Style="text-align: right" />
                </td>
            </tr>
            <tr>
                <td class="style2">
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                    <asp:Label ID="lblEt12" runat="server" Text="Total Retenido:" />
                </td>
                <td>
                    <asp:TextBox ID="txtImporteR" runat="server" Enabled="False" Text="0.0000" Style="text-align: right" />
                </td>
            </tr>
            <tr>
                <td class="style2">
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
        </table>
        <asp:XmlDataSource ID="xmldsMetodo" runat="server" DataFile="~/App_Data/Catalogos.xml"
            XPath="Catalogos/metodosDePago/metodoDePago"></asp:XmlDataSource>
        <asp:XmlDataSource ID="xmldsMoneda" runat="server" DataFile="~/App_Data/Catalogos.xml"
            XPath="Catalogos/Monedas/Moneda"></asp:XmlDataSource>
        <table class="mGrid2">
            <tr>
                <th style="width: 92px;">
                    Folio Recepción CFD
                </th>
                <th style="width: 605px;">
                    Clave
                </th>
                <th style="width: 140px;">
                    Importe
                </th>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
<%--<a href="#Captura">--%>
<asp:UpdatePanel ID="upnl2" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
    <ContentTemplate>
        <table>
            <tr>
                <td>
                    <asp:DropDownList Width="150px" ID="ddlReferencia" runat="server" Enabled="True">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvReferencia" runat="server" Style="color: #FF6600;
                        font-size: xx-small; font-family: Arial;" Font-Bold="True" ControlToValidate="ddlReferencia"
                        ErrorMessage="Folio de Recención CFD requerido" ToolTip="Folio de Recención CFD"
                        ValidationGroup="GrupoDatos">*
                    </asp:RequiredFieldValidator>
                </td>
                <td>
                    <%--<asp:TextBox Width="50px" ID="txtPartida" ToolTip="No Partida" runat="server" OnTextChanged="txtPartida_TextChanged"
                        onkeypress="onlyNumbersF(event);" onblur="__doPostBack('txtPartida','OnBlur');"
                        MaxLength="5"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPartida" runat="server" Style="color: #FF6600;
                        font-size: xx-small; font-family: Arial;" Font-Bold="True" ControlToValidate="txtPartida"
                        ErrorMessage="Partida requerida" ToolTip="La partida es requerida para desplegar las claves de articulo."
                        ValidationGroup="GrupoDatos">*
                    </asp:RequiredFieldValidator>--%>
                </td>
                <td>
                    <asp:DropDownList Width="520px" ID="ddlNoIdentificacion" runat="server" ToolTip="Clave de Articulo">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:TextBox Width="110px" ID="txtImporte" runat="server" onkeypress="fieldNumber(this, event)"
                        Enabled="True" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvImporte" runat="server" Style="color: #FF6600;
                        font-size: xx-small; font-family: Arial;" Font-Bold="True" ControlToValidate="txtImporte"
                        ErrorMessage="Importe requerido" ToolTip="Importe" ValidationGroup="GrupoDatos">*
                    </asp:RequiredFieldValidator>
                    <br />
                </td>
                <td>
                    <asp:ImageButton ID="imgbtnAgregar" runat="server" ImageUrl="~/Layouts/images/botones/add.png"
                        OnClick="imgbtnAgregar_Click" ValidationGroup="GrupoDatos" 
                        Style="height: 16px;" />
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <asp:GridView ID="gvTransportes" runat="server" OnRowDeleting="gvTransportes_RowDeleting"
                        OnRowDeleted="gvTransportes_RowDeleted" AutoGenerateColumns="false" ShowHeader="false"
                        OnRowDataBound=" gvTransportes_RowDataBound" HeaderStyle-Font-Bold="true" HeaderStyle-ForeColor="white"
                        HeaderStyle-Height="40px" ShowFooter="False" CssClass="mGrid" HeaderStyle-BackColor="#336699"
                        Font-Size="XX-Small" 
                        onselectedindexchanged="gvTransportes_SelectedIndexChanged">
                        <Columns>
                            <asp:TemplateField HeaderText="Referencia" SortExpression="referencia" ItemStyle-Width="150px"
                                ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:Label ID="lbl1" Text='<%# Eval("Referencia") %>' runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Clave" SortExpression="clave" ItemStyle-Width="520px"
                                ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:Label ID="lbl3" Text='<%# Eval("NoIdentificacion") %>' runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importe" SortExpression="Importe" ItemStyle-Width="115px"
                                ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" HeaderStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:Label ID="lbl8" Text='<%# String.Format("{0:f4}", DataBinder.Eval(Container.DataItem, "Importe")) %>'
                                        runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="25px">
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgbtnDeleteA" runat="server" CommandName="Delete" ImageUrl="~/Layouts/images/botones/gtk-remove.png" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td colspan="5" style="text-align: right;" class="auto-style2">
                    <%--<asp:Label Width="100px" ID="lblTotImpA" runat="server" Text="Importe:" />
                    <asp:TextBox ID="txtTotImpA" Text="0.0000" runat="server" Width="80px" Enabled="false"
                        Style="text-align: right"></asp:TextBox>--%>
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <asp:Label ID="lblErr" runat="server" Text="" Font-Bold="True" Style="color: #FF6600;
                        font-size: medium; font-family: Arial;"></asp:Label>
                    <asp:ValidationSummary ID="ValSum" runat="server" Style="color: #FF6600; font-size: medium;
                        font-family: Arial;" Font-Bold="True" ValidationGroup="GrupoDatos" DisplayMode="List"
                        ForeColor="Maroon" />
                </td>
            </tr>
        </table>
        <asp:XmlDataSource ID="XmlDataSource4" runat="server" DataFile="~/App_Data/Catalogos.xml"
            XPath="Catalogos/ums/um"></asp:XmlDataSource>
        <table id="TotalesCapturados">
            <tr>
                <td style="width: 590px;">
                </td>
                <td class="auto-style2">
                    <%--<asp:Label ID="lblSubTcap0" runat="server" Text="Captura:"></asp:Label>--%>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td style="text-align: right">
                </td>
                <td class="style1">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td style="text-align: left">
                    <asp:Label ID="lblSubTcap" runat="server" Text="Subtotal:"></asp:Label>
                </td>
                <td class="style1">
                    <asp:TextBox ID="txtSubTcap" runat="server" Text="0.0000" Enabled="False" Style="text-align: right"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td style="text-align: left">
                    <asp:Label ID="lblIva" runat="server" Text="Iva:"></asp:Label>
                </td>
                <td class="style1">
                    <asp:TextBox ID="txtIva" runat="server" Text="0.0000" Enabled="False" Style="text-align: right"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td style="text-align: left">
                    <asp:Label ID="lblRetenciones" runat="server" Text="Retenciones:"></asp:Label>
                </td>
                <td class="style1">
                    <asp:TextBox ID="txtRetenciones" runat="server" Text="0.0000" Enabled="False" Style="text-align: right"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td class="auto-style2">
                    <asp:Label ID="lblTotalSum" runat="server" Text="Total:" ></asp:Label>
                </td>
                <td class="style1">
                    <asp:TextBox ID="txtTotalSum" runat="server" Text="0.0000" Enabled="False" 
                        Style="text-align: right"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
        </table>


     <table style="width: 861px; text-align: center" border="0">
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
