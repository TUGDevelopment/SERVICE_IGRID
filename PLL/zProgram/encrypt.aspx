<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="encrypt.aspx.cs" Inherits="PLL.zProgram.encrypt" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>
        Password Decrypt</p><br />
    
     Username: <asp:TextBox runat="server" ID="txtUsername"></asp:TextBox>  
    <asp:Button runat="server" ID="btnDecrypt" Text="Decrypt" OnClick="btnDecrypt_Click"/>
            <br />
    Password: <asp:Label runat="server" ID="lblPassword"></asp:Label>
        </div>
    </form>
</body>
</html>
