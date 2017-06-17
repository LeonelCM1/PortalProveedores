<%@ page masterpagefile="~/mpMenu.master" language="VB" autoeventwireup="false" inherits="Facturas, App_Web_nykhp1yw" title="Proveedores" %>

<%@ MasterType VirtualPath="~/mpMenu.master" %>
<%@ Register Src="~/Factura_Elec.ascx" TagName="ctlFacturaElect" TagPrefix="ucFE" %>
<%@ OutputCache Location="None" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    
    <div id="lblUsr">
        <asp:Label ID="lblUsuarioActivo" runat="server" Text="Label" Visible ="false"></asp:Label>
    </div>    
    <br />
    <div id="menuFc">
   <%--GCM 06012015--%>
            <ul class="tabs3" id="button3">
                <li><a >   
                    <asp:LinkButton Visible="false" ID="LnkCambiaRFC" runat="server" >&nbsp&nbsp Cambia Proveedor &nbsp&nbsp</asp:LinkButton></a></li>
                <li><a >   
                    <asp:LinkButton Visible="False" ID="LnkPreEntrega" runat="server" >&nbsp&nbsp Etiquetas &nbsp&nbsp</asp:LinkButton></a></li>                
                <li><a  >
                    <asp:LinkButton ID="LinkAcuse" runat="server" Visible="false">&nbsp&nbsp Acuses de Mercancía &nbsp&nbsp</asp:LinkButton></a></li>                    
                <li><a >
                    <asp:LinkButton ID="LnkSeguimiento" runat="server" >&nbsp&nbsp Seguimiento &nbsp&nbsp</asp:LinkButton></a></li>
                <li><a >
                    <asp:LinkButton class="Ligth" ID="lnkEtiqueta" runat="server" Visible="true" >&nbsp&nbsp Subir Facturas &nbsp&nbsp</asp:LinkButton></a></li>               
            </ul>
       
    </div>
    <br />
   <%-- GCM 06012015 <div id="button2" runat="server">
        <ul class="tabs3" id="butt2">
            <li><a>
                <asp:LinkButton Visible="false" ID="LnkCambiaRFC" runat="server">Cambia Proveedor</asp:LinkButton></a></li>
        </ul>
    </div>--%>
            <table style="width: 861px; height: 389px; margin-left: 20px" border="0">
                <tr>
                    <td colspan="2" valign="top">
                        <br />
                        <br />
                        <asp:Label ID="LblProveedor" runat="server" Font-Bold="True" ForeColor="#336699"
                            Width="835px"></asp:Label>
                        <br />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" valign="top">
                        <table style="font-size: small; margin-left: 10px;">
                            <tr>
                                <td>
                                    <div>
                                        <ucFE:ctlFacturaElect ID="ctlFacturaElect" runat="server" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <br />
                    </td>
                </tr>
            </table>
         
</asp:Content>
