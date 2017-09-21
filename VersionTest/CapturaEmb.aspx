<%@ page language="VB" masterpagefile="~/mpMenu.master" autoeventwireup="false" inherits="CapturaEmb, App_Web_0j02cehq" maintainscrollpositiononpostback="True" %>
<%@ MasterType VirtualPath="~/mpMenu.master" %>
<%@ Register Src="~/FacturaEmbarque.ascx" TagName="ctlFacturaEmbarque" TagPrefix="ucFEmb" %>
<%@ OutputCache Location="None" %>


<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
     <div id="lblUsr">
        <asp:Label ID="lblUsuarioActivo" runat="server" Text="Label" Visible ="false"></asp:Label>
        </div> 
    <div id="menuFc" >
        
        <ul class="tabs3" id="button3">
            <li ><a>
                <asp:LinkButton Visible="false" ID="LnkCambiaRFC" runat="server">&nbsp&nbsp Cambia Proveedor &nbsp&nbsp</asp:LinkButton>
                </a></li>
            <li><a >   
                <asp:LinkButton Visible="False" ID="LnkPreEntrega" runat="server" >&nbsp&nbsp Etiquetas &nbsp&nbsp</asp:LinkButton>
                </a></li>
            <li><a>
                <asp:LinkButton visible="false" ID="LinkAcuse" runat="server">&nbsp&nbsp Acuses de Mercancía &nbsp&nbsp</asp:LinkButton>
                </a></li>            
            <li><a>
                <asp:LinkButton ID="LnkSeguimientoCap" runat="server" OnClick="LnkSeguimiento_Click">&nbsp&nbsp Seguimiento &nbsp&nbsp</asp:LinkButton>
                </a></li>
            <li><a>
                <asp:LinkButton ID="LinkFactura" runat="server">&nbsp&nbsp Subir Facturas &nbsp&nbsp</asp:LinkButton>
                </a></li>           
        </ul>
        </div>
        <br />
<%--       <div id="button2" runat="server">
        <ul class="tabs3" id="butt2" >
            <li ><a>
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
                                        <ucFEmb:ctlFacturaEmbarque ID="ctlFact_emb" runat="server" />
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