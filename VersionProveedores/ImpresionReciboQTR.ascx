<%@ control language="C#" autoeventwireup="true" inherits="ImpresionReciboQTR, App_Web_m0crdngr" %>
<table style="width: 861px;" border="0">
    <tr>
        <td colspan="2" style="display: inline-block; color: #336699; font-weight: bold;
            width: 835px; text-align: left">
            <asp:Label ID="LblFactura" runat="server" Width="835px" />
            <asp:Label ID="LblFolio" runat="server" Width="835px" />
            <br />
            <br />
        </td>
    </tr>
</table>
<table>
    <tr>
        <td>
            <asp:Label runat="server" ID="TxtMensajes" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                ForeColor="Maroon" TextMode="MultiLine" Font-Names="Calibri" Font-Size="Small" />
        </td>
        <td>
            <asp:Label runat="server" ID="TxtMensajeExitoso" BorderStyle="None" BorderWidth="0px"
                Font-Bold="True" ForeColor="Green" TextMode="MultiLine" Font-Names="Calibri"
                Font-Size="Small" />
        </td>
    </tr>
</table>
<table>
    <tr>
        <td>
            <br />
            <br />
            <br />
            <asp:Button class="btnCommand" ID="BtnImprimir" runat="server" Text="Imprimir Contra Recibo" OnClick="BtnImprimir_Click" />
        </td>
    </tr>
</table>
