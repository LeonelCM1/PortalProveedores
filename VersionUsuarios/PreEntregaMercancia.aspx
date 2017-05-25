<%@ page masterpagefile="~/mpMenu.master" language="vb" autoeventwireup="false" inherits="PreEntregaMercancia, App_Web_kijgklmz" title="Proveedores" enableviewstate="true" %>
<%@ MasterType VirtualPath="~/mpMenu.master"  %>
<%@ Register src="~/GeneraEtiqueta.ascx" tagname="ctlGenEtiq" tagprefix="ucGS" %>
<%@ OutputCache Location="None" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    
    <div id="lblUsr">
        <asp:Label ID="lblUsuarioActivo" runat="server" Text="Label" Visible="false"></asp:Label>
    </div>
    <br />
    <div id="menuFc" >
        <ul class="tabs3" id="button3">
            <li><a>
                <asp:LinkButton Visible="false" ID="LnkCambiaRFC" runat="server">&nbsp&nbsp Cambia Proveedor &nbsp&nbsp</asp:LinkButton>
                </a></li>     
            <li><a >   
                <asp:LinkButton class="Ligth"  Visible="True" ID="LnkPreEntrega" runat="server" >&nbsp&nbsp Etiquetas &nbsp&nbsp</asp:LinkButton>
                </a></li>
            <li><a>                   
                    <asp:LinkButton visible="false"  ID="LinkAcuse" runat="server" ToolTip="Acuses de Mercancía">&nbsp&nbsp Acuses de Mercancía &nbsp&nbsp</asp:LinkButton>
                 </a></li>
            <li><a>
                <asp:LinkButton   ID="LnkSeguimiento" runat="server">&nbsp&nbsp Seguimiento &nbsp&nbsp</asp:LinkButton>
                </a></li>     
            <li><a>
                <asp:LinkButton   ID="LinkFacturas" runat="server">&nbsp&nbsp Subir Facturas &nbsp&nbsp</asp:LinkButton>
                </a></li>
        </ul>
    </div>
    <br />
            <table style="width: 861px; height: 389px; margin-left: 20px" border="0">
                <tr>
                    <td colspan="2" valign="top">
                        <br />
                        <br />
                        <asp:Label ID="LblProveedor" runat="server" Font-Bold="True" ForeColor="#336699"
                            Width="835px"></asp:Label>
                        <br />
                        <br />
                        <table style="font-size: small; margin-left: 10px;">
                            <tr>
                                <td>
                                    <div>
                                        <ucGS:ctlGenEtiq ID="ctlGenEtiq" runat="server" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
       <%-- </div>
    </div>--%>
    <div></div>
</asp:Content>

