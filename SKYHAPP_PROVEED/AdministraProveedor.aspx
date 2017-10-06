<%@ page title="" language="VB" masterpagefile="~/mpOptionsUser.master" autoeventwireup="false" inherits="AdministraProveedor, App_Web_bqe4lr4o" maintainscrollpositiononpostback="true" %>
<%@ MasterType VirtualPath="~/mpOptionsUser.master"  %>
<%@ Register src="~/AdministraControl.ascx" tagname="ctlAdmCtrl" tagprefix="ucAC" %>
<%@ OutputCache Location="None" %>



<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
   
    <table>
        <tr>
            <td style="height: 50px; width: 150px;" colspan="2"></td>
            <td style="width: 600px">
            </td>
            <td >
               
            </td>

        </tr>
        <tr>
            <td style=" height: 5px; width: 50px;"></td>
            <td colspan="2" style="height: 20px ">
                <div>
                    <ucAC:ctlAdmCtrl ID="ctlAdmCtrl" runat="server" />
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
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />

</asp:Content>



