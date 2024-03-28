<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="zPIC.aspx.cs" Inherits="PLL.zPIC" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        Status : <asp:Label runat="server" ID ="lblStatus"></asp:Label><br />
        <br />
        Username : <asp:TextBox runat="server" ID ="txtUsername" Enabled="false"></asp:TextBox><br />
        Zone : <asp:TextBox runat="server" ID ="txtZone" Enabled="false"></asp:TextBox><br />
        Sold-To : <asp:TextBox runat="server" ID ="txtSoldTo" Enabled="false"></asp:TextBox><br />
        Ship-To : <asp:TextBox runat="server" ID ="txtShipTo" Enabled="false"></asp:TextBox><br />
        Country : <asp:TextBox runat="server" ID ="txtCountry" Enabled="false"></asp:TextBox>
        <asp:Label runat="server" ID="lblID" Visible="false" Enabled="false"></asp:Label>
        <br />
        <br />
        <br />
        <asp:Button runat="server" ID="btnAdd" Text="Update" OnClick="btnAdd_Click" Enabled="false" />
        <asp:Button runat="server" ID="btnCommit" Text="Manual add" OnClick="btnCommit_Click" Enabled="false"/>
        <asp:Label runat="server" ID="txtG"></asp:Label>
        <br />
        <br />
        -------------------------------------------------------------------------<br />
        PIC Data<br />
&nbsp;<br />
        <asp:GridView ID="gvPIC" runat="server" CellPadding="4" GridLines="None" ForeColor="#333333" Height="191px" Width="727px" AllowPaging="true" AllowSorting="true" pagesize="5" Visible="false" >
            <AlternatingRowStyle BackColor="White" />
            <Columns>
            <asp:TemplateField HeaderText="Edit">
                <ItemTemplate>
                    <asp:LinkButton runat="server" Text='<%#Eval("ID") %>' id="linkEdit" OnClick="linkEdit_Click"></asp:LinkButton>
                </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" ForeColor="White" Font-Bold="True" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
         
        </asp:GridView>
        </div>
    </form>
</body>
</html>
