<%@ control language="VB" autoeventwireup="false" inherits="Login12, App_Web_2zohwe2f" %>

<style type="text/css">

    .style1
    {
        width: 166px;
    }

    .style2
    {
        width: 84px;
        height: 33px;
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

<asp:Login ID="Login1" runat="server"
    FailureText="Cuenta no activada o no existe"
    RememberMeText="Recordar la próxima vez." Width="379px">
    <LayoutTemplate>
        <table width="500px" border="0">
            <tr>
                <td align="right" valign="top" class="style3">
                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"
                        ForeColor="#336699" Font-Bold="True" Font-Size="11pt">RFC:</asp:Label>
                </td>
                <td align="left" class="style4">
                    <asp:TextBox ID="UserName" runat="server" MaxLength="15" Width="147px" ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server"
                        ControlToValidate="UserName" ErrorMessage="Ingrese su Usuario"
                        ToolTip="Usuario es requerido." ValidationGroup="ctl00$Login1">*</asp:RequiredFieldValidator>
                </td>
                <td align="center" rowspan="5">
                    <asp:ValidationSummary ID="ValidationSummary2" runat="server"
                        Style="color: #FF6600; font-size: smaller; font-family: Arial;"
                        ValidationGroup="ctl00$Login1" DisplayMode="List" Font-Bold="True"
                        ForeColor="Maroon" Height="65px" />
                    <asp:Literal ID="Literal1" runat="server" EnableViewState="False" Visible="true"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td align="right" valign="top" class="style2">
                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password"
                        ForeColor="#336699" Font-Bold="True" Font-Size="11pt">Contraseña:</asp:Label>
                </td>
                <td align="left" class="style1">
                    <asp:TextBox ID="Password" runat="server" TextMode="Password" MaxLength="16" Width="147px" onpaste="return false"
                        onContextMenu="javascript:return false;"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server"
                        ControlToValidate="Password" ErrorMessage="Ingrese su Contraseña"
                        ToolTip="Contraseña es requerida." ValidationGroup="ctl00$Login1">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td  align="center">
                    &nbsp;</td>
                <td class="style2">
                    
                    <asp:Button ID="LoginButton" runat="server" class="btnCommand"  Width="150px"
                        CommandName="Login" Text="Ingresar" ValidationGroup="ctl00$Login1" />
                    
                </td>
            </tr>
        </table>
    </LayoutTemplate>
</asp:Login>
<asp:Label ID="lblError" runat="server" Text="" Visible="false" ForeColor="#FF3300" Style="font-size: smaller; font-family: Arial;" Font-Bold:True></asp:Label>