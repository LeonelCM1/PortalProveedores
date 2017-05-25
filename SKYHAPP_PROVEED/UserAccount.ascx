<%@ control language="VB" autoeventwireup="false" inherits="UserAccount, App_Web_gf5jyiiv" %>

<style type="text/css">

    .style1
    {
        width: 166px;
    }

    .style2
    {
        width: 84px;
    }

    .style3
    {
        width: 84px;
        height: 30px;
    }

    .style4
    {
        width: 166px;
        height: 30px;
    }

</style>



        <table width="500px" border="0">
           

            <tr>
                <td align="right"  class="style3">
                    <asp:Label for="usuario" runat="server" ForeColor="#336699" Font-Bold="True" Font-Size="11pt">
                                Usuarios:</asp:Label></td>
                <td align="left" class="style4">
                    <asp:TextBox ID="UserName" runat="server" Visible="false"></asp:TextBox>
                    <asp:DropDownList ID="ddlUser" runat="server"  Width="217px" Height="25px">
                    </asp:DropDownList></td>
            </tr>

            <tr>
                <td align="right"  class="style2">
                    <asp:Label for="password" runat="server" ForeColor="#336699" Font-Bold="True" Font-Size="11pt">
                                Password:</asp:Label>
                </td>
                <td align="left" class="style1">
                    <asp:TextBox ID="txtPassword" Type="password" runat="server"  Width="213px" Height="19px" MaxLength="16" onpaste="return false" onContextMenu="javascript:return false;"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td colspan="2" align="center">
                    <asp:Label ID="lblError" runat="server" Text="" ></asp:Label>
                </td>
                <td></td>
            </tr>

            <tr>
                <td  align="center">
                    &nbsp;</td>
                <td>
                    <asp:Button ID="btnLogin" runat="server" class="btnCommand" Text="Entrar" 
                        Width="216px" />
                </td>
            </tr>

        </table>


