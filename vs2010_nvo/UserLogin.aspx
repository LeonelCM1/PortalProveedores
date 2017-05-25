<%@ Page Title="" Language="VB" MasterPageFile="~/mpLogin.master" AutoEventWireup="false" CodeFile="UserLogin.aspx.vb" Inherits="UserLogin" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType VirtualPath="~/mpLogin.master" %>
<%@ Register Src="~/UserAccount.ascx" TagName="ctlLoginUser" TagPrefix="ua1" %>
<%@ OutputCache Location="None" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
        <table>
        <tr align="right">
            <td></td>
            <td></td>
            <td></td>
            <td style=" width: 900px;" ><asp:HyperLink ID="Doc" runat="server" NavigateUrl="http://www.skytex.com.mx/Documentacion/">Documentación Recepción Factura Electrónica</asp:HyperLink></td> </tr>
        </table>
    <table>
        <tr>
            <td style="height: 66px; width: 650px;" colspan="2"></td>
            <td style="width: 700px">
            </td>
            <td >
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td style=" height: 20px; width: 400px;"></td>
            <td colspan="2" style="height: 20px ">
                <div>
                    <ua1:ctlLoginUser ID="ctlLoginUser" runat="server" />
                </div>
            </td>
            <td style="height: 40px;"></td>
        </tr>
        <tr>
            <td colspan="1" style="width: 100px"></td>
            <td></td>
        </tr>
    </table>
    <br/>
    <br />
    <br />
    <br />
    <table>
        <tr align="left">
            <td>
                <!--<h3><a href="Files/FechaLimiteFacturas2016.pdf" target="_blank" style=" padding: 20px; ">Fechas límite para recibir facturas electrónicas 2016 </a></h3>-->
            </td>
        </tr>
    </table>
    <!--<br />
    <br />
    <br />
    <br />
    <br />-->
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
</asp:Content>

