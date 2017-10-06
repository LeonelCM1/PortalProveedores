<%@ control language="VB" autoeventwireup="false" inherits="UserControlsGridAcuse, App_Web_xg4me3vo" %>

<%--<style type="text/css">
    .style2
    {
        height: 33px;
        width: auto;
    }

    .dialogzone
    {
        background-color: #E4E4EC;
        color: #2D62B0;
        font-family: "arial","sans-serif";
        font-size: 11px;
    }

    .dialogzone
    {
        background-color: #E4E4EC;
        color: #2D62B0;
        font-family: "arial","sans-serif";
        font-size: 11px;
    }

    .iconnocheck
    {
        background-color: #E4E4EC;
        border: 1px solid #E4E4EC;
    }

    .iconnocheck
    {
        background-color: #E4E4EC;
        border: 1px solid #E4E4EC;
    }

    .iconText
    {
        font-family: "arial","sans-serif";
        font-size: 11px;
        color: black;
    }

    .iconText
    {
        font-family: "arial","sans-serif";
        font-size: 11px;
        color: black;
    }

    .iconcheck
    {
        background-color: #B8C5D1;
        border-top: 1px solid #6A85AE;
        border-left: 1px solid #6A85AE;
        border-bottom: 1px solid white;
        border-right: 1px solid white;
    }

    .iconcheck
    {
        background-color: #B8C5D1;
        border-top: 1px solid #6A85AE;
        border-left: 1px solid #6A85AE;
        border-bottom: 1px solid white;
        border-right: 1px solid white;
    }

    .textinputs
    {
        font-family: "arial","sans-serif";
        font-size: 11px;
        background-color: white;
        border: 1px solid #636384;
        padding-left: 2px;
        padding-right: 2px;
    }

    .textinputs
    {
        font-family: "arial","sans-serif";
        font-size: 11px;
        background-color: white;
        border: 1px solid #636384;
        padding-left: 2px;
        padding-right: 2px;
    }

    .comboEditable
    {
        background-color: white;
        border: 1px solid #636384;
        font-family: "arial","sans-serif";
        font-size: 11px;
    }

    .comboEditable
    {
        background-color: white;
        border: 1px solid #636384;
        font-family: "arial","sans-serif";
        font-size: 11px;
    }

    .thumbtxtsel
    {
        color: #12397A;
        font-family: "arial","sans-serif";
        font-size: 11px;
        text-decoration: none;
        font-weight: bold;
    }

    .thumbtxtsel
    {
        color: #12397A;
        font-family: "arial","sans-serif";
        font-size: 11px;
        text-decoration: none;
        font-weight: bold;
    }

    A
    {
        color: #0000e0;
    }

    A
    {
        color: #0000e0;
    }

    .palette
    {
        background-color: #E4E4EC;
        border: 1px solid #BEBED1;
    }

    .palette
    {
        background-color: #E4E4EC;
        border: 1px solid #BEBED1;
    }

    .insetBorder
    {
        background-color: white;
        border-bottom: 2px solid #F0F0F0;
        border-right: 2px solid #F0F0F0;
        border-top: 2px solid #808080;
        border-left: 2px solid #808080;
    }

    .insetBorder
    {
        background-color: white;
        border-bottom: 2px solid #F0F0F0;
        border-right: 2px solid #F0F0F0;
        border-top: 2px solid #808080;
        border-left: 2px solid #808080;
    }

    .trElt
    {
        white-space: nowrap;
        padding: 0px;
        margin: 0px;
        width: 1px;
    }

    .trElt
    {
        white-space: nowrap;
        padding: 0px;
        margin: 0px;
        width: 1px;
    }

    .treeNormal
    {
        text-decoration: none;
        color: black;
        font-family: "arial","sans-serif";
        font-size: 11px;
        font-style: normal;
        padding: 1px;
        cursor: pointer;
        height: 16px;
    }

    .treeNormal
    {
        text-decoration: none;
        color: black;
        font-family: "arial","sans-serif";
        font-size: 11px;
        font-style: normal;
        padding: 1px;
        cursor: pointer;
        height: 16px;
    }

    div.crystalstyle div
    {
        position: absolute;
        z-index: 25;
    }

    .ad22a2609d-949a-448f-9c0e-f3f2df31786d-0
    {
        border-color: #000000;
        border-left-width: 0px;
        border-right-width: 0px;
        border-top-width: 0px;
        border-bottom-width: 0px;
    }

    .fc2ef19a81-dfb2-490f-afc9-f63a7f836e15-0
    {
        font-size: 9pt;
        color: #000000;
        font-family: 16 cpi;
        font-weight: normal;
    }

    div.crystalstyle a
    {
        text-decoration: none;
    }

    .fc2ef19a81-dfb2-490f-afc9-f63a7f836e15-1
    {
        font-size: 9pt;
        color: #000000;
        font-family: 13.3 cpi;
        font-weight: normal;
    }

    .ad22a2609d-949a-448f-9c0e-f3f2df31786d-1
    {
        border-left-width: 0px;
        border-right-width: 0px;
        border-top-width: 0px;
        border-bottom-width: 0px;
    }

    .fc2ef19a81-dfb2-490f-afc9-f63a7f836e15-2
    {
        font-size: 9pt;
        color: #FF0000;
        font-family: 16 cpi;
        font-weight: bold;
    }

    .menuShadow
    {
        position: absolute;
        background-color: #a0a0a0;
    }

    .menuShadow
    {
        position: absolute;
        background-color: #a0a0a0;
    }

    .style5
    {
        height: 33px;
        width: 100px;
    }
</style>
--%>
<%--<script src="../Resources/Scripts/jquery-1.3.2.js" type="text/javascript"></script>
<script src="../Resources/Scripts/jquery.MultiFile.js" type="text/javascript">  </script>--%>

<table>
    <tr>
        <td  valign="top" font-size="XX-Small">Acuse :</td>
        <td  valign="top">
            <asp:DropDownList ID="ddlRecibo" runat="server" Height="22px" Width="360px">
            </asp:DropDownList>
        </td>
        <td valign="top" font-size="XX-Small">Empresa:</td>
        <td  valign="top">
            <asp:DropDownList ID="ddlEmpresa" runat="server" Height="22px" Width="207px">
            </asp:DropDownList>
        </td>
        
       
        <td valign="top">
            <asp:Button class="btnCommand" ID="btnCons" runat="server" Text="consultar" Width="107px"
                OnClick="btnCons_Click" Style="margin-top: 0px" />
        </td>
    </tr>
    <tr>
        <td colspan="7" align="right" valign="top">
            <asp:Label ID="lblRFC" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lblErr" runat="server" Font-Bold="True"
                Font-Size="10pt" Visible="False" ForeColor="Maroon"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="7" align="justify" valign="top">
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" OnRowCommand="GridView1_RowCommand"
                HeaderStyle-Font-Bold="true" HeaderStyle-ForeColor="white" CssClass="mGrid"
                HeaderStyle-BackColor="#336699" Font-Size="XX-Small" Width="350px" Height="250"
                HeaderStyle-HorizontalAlign="Center" RowHeaderColumn="0"
                HorizontalAlign="Center" AllowPaging="True" PageSize="8" OnPageIndexChanging="GridView1_PageIndexChanging">
                <Columns>
                    <asp:BoundField HeaderText="Acuse Recibo Mercancia" DataField="AcuseReciboMercancia"></asp:BoundField>
                    <asp:ButtonField ButtonType="Link" CommandName="Imprimir" DataTextField="Folio"
                        HeaderText="Folio" Text="Imprimir" />
                </Columns>
                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
            </asp:GridView>
            <br />
            <asp:Panel ID="Panel1" runat="server" Height="200px">
            </asp:Panel>
        </td>
    </tr>

</table>
<p>
    &nbsp;
</p>
