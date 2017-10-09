<%@ page masterpagefile="~/mpOptionsUser.master" language="VB" autoeventwireup="false" inherits="CajaChica, App_Web_xsrxeqlv" %>
<%@ MasterType VirtualPath="~/mpOptionsUser.master"  %>
<%@ Register src="~/CajaFacturas.ascx" tagname="ctlCajaFact" tagprefix="ucCF" %>
<%@ OutputCache Location="None" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="menu" style="height: 20px; width: 980px;">
        <%--<div id="Div1" style="height: 20px; width: 800px;">--%>
        <div id="lblUsr">
            <asp:Label ID="lblUsuarioActivo" runat="server" Text="Label" Visible="false"></asp:Label>
        </div>
        <br />
        <ul class="tabs" id="button">
            <li><a>
                <asp:LinkButton Visible="false" ID="LnkRetornaMenuOpc" runat="server">&nbsp&nbsp Regresar opciones de usuario &nbsp&nbsp</asp:LinkButton></a></li>
            <li><a>
                <asp:LinkButton ID="LnkSegCajaChica" runat="server">&nbsp&nbsp Seguimiento caja chica</asp:LinkButton></a></li>
            <%--<li><a>
                    <asp:LinkButton Visible="false" ID="LnkRetornaMenuOpc" runat="server">Regresar Opciones de Usuario</asp:LinkButton></a></li>    --%>
            <li><a>
                <%--<asp:Label ID="lblSubir" runat="server" Text="Subir Facturas Caja Chica" ></asp:Label></a></li>--%>
                <asp:LinkButton  class="Ligth" ID="lnkEtiqueta" runat="server">&nbsp&nbsp Subir facturas caja chica</asp:LinkButton></a></li>
            
        </ul>
        <br />
        <br />
<%--        <div id="button2" runat="server">
            <ul class="tabs3" id="butt2">
                <li><a>
                    <asp:LinkButton Visible="false" ID="LnkRetornaMenuOpc" runat="server">Regresar opciones de usuario</asp:LinkButton></a></li>
            </ul>
        </div>--%>
    </div>
    <%--<table style="width: 900px; height: 389px; margin-left: 20px" border="0">--%>
    <table style="width: 910px; height: 389px; margin-left: 20px" border="0">
        <tr>
            <td colspan="2" valign="top">
                <br />
                <table style="font-size: small; margin-left: 10px; width: 100%;">
                
                    <tr>
                        <td>
                            <div>
                                <ucCF:ctlCajaFact ID="ctlCajaFact" runat="server" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div>
    </div>
</asp:Content>
