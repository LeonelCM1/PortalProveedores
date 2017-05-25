<%@ page language="VB" autoeventwireup="false" inherits="CartaFacturas, App_Web_ervqt4b3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Skytex Mexico S.A. de C.V.</title>
    <link rel="Stylesheet" href="Layouts/MenuTabs.css" media="screen" />
    <script type="text/javascript" language="JavaScript">
        javascript: window.history.forward(1); //Esto es para cuando le pulse al botón de Atrás 
</script>
</head>
<body>
    <div id="main_container">
        <div id="top_banner">
            <div id="Div1">
                <img src="Layouts/images/fondos/logo_trasp.png" width="248" height="100" border="0" alt="" title="" />
            </div>
        </div>
        <div id="center_content">
            <form id="exA" class="fValidator-form" runat="server" enctype="multipart/form-data">
                <div id="div-Close">
                    <asp:LinkButton ID="LnkRegresar" runat="server" ForeColor="Blue" ToolTip="Regresar">
                Regresar portal proveedores
                    </asp:LinkButton>
                </div>
                <div>
                <br />
                <%--<table>
                    <tr>
                        <td style=" width: 120px;" ></td>
                        <td><img src="Layouts/images/img/logosky.gif" width="148" height="80" border="0" alt="" title="" float:"left" /></td>
                        <td style=" width: 120px;" ></td>
                        <td><img src="Layouts/images/img/logofelt.gif" width="148" height="80" border="0" alt="" title="" float:"left" /></td>
                        <td style=" width: 120px;" ></td>
                        <td><img src="Layouts/images/img/logeps1.gif" width="148" height="80" border="0" alt="" title="" float:"left" /></td>
                    </tr>
                    <tr>
                        <td style=" width: 120px;" ></td>
                    </tr>                
                    <tr>
                        <td style=" width: 120px;" ></td>
                        <td><img src="Layouts/images/img/logomatt.gif" width="148" height="80" border="0" alt="" title="" /></td>
                        <td style=" width: 120px;" ></td>
                        <td><img src="Layouts/images/img/logorug.gif" width="148" height="100" border="0" alt="" title=""/></td>
                        <td style=" width: 120px;" ></td>
                        <td><img src="Layouts/images/img/logeps2.gif" width="148" height="80" border="0" alt="" title="" /></td>
                    </tr>                               
                </table>
                --%>
                <br />
                <br />

                <table>                    
                    <tr>
                        <td style=" width: 150px; " ></td>
                        <td><img src="Layouts/images/img/Carta.png" width="700" height="900" border="0" alt="" title="" /></td>
                    </tr> 
                </table>
                <br />
                <br />
                <%--<br />
                <br />
                <table>
                    <tr>
                       <td style=" width: 100px; " ></td> 
                       <td style=" width: 750px; font-size: small; " > </td>
                    </tr>
                </table>--%>
                </div>
                
            </form>
        </div>
        <div id="footer">
            Grupo Skytex Mexico S.A. de C.V.&copy; Todos Los Derechos Reservados 2013
        <br />
            <a href="http://validator.w3.org/check?uri=referer" title="This site is W3C compliant"></a>
        </div>

    </div>
</body>
</html>
