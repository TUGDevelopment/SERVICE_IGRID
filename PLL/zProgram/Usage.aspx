<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Usage.aspx.cs" Inherits="PLL.zProgram.Usage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Usage report</title>
    <style>

        table.blueTable {
  border: 1px solid #1C6EA4;
  background-color: #EEEEEE;
  width: 100%;
  text-align: left;
  border-collapse: collapse;
}
table.blueTable td, table.blueTable th {
  border: 1px solid #AAAAAA;
  padding: 3px 2px;
}
table.blueTable tbody td {
  font-size: 13px;
}
table.blueTable tr:nth-child(even) {
  background: #D0E4F5;
}
table.blueTable thead {
  background: #1C6EA4;
  background: -moz-linear-gradient(top, #5592bb 0%, #327cad 66%, #1C6EA4 100%);
  background: -webkit-linear-gradient(top, #5592bb 0%, #327cad 66%, #1C6EA4 100%);
  background: linear-gradient(to bottom, #5592bb 0%, #327cad 66%, #1C6EA4 100%);
  border-bottom: 2px solid #444444;
}
table.blueTable thead th {
  font-size: 15px;
  font-weight: bold;
  color: #FFFFFF;
  border-left: 2px solid #D0E4F5;
}
table.blueTable thead th:first-child {
  border-left: none;
}

table.blueTable tfoot {
  font-size: 14px;
  font-weight: bold;
  color: #FFFFFF;
  background: #D0E4F5;
  background: -moz-linear-gradient(top, #dcebf7 0%, #d4e6f6 66%, #D0E4F5 100%);
  background: -webkit-linear-gradient(top, #dcebf7 0%, #d4e6f6 66%, #D0E4F5 100%);
  background: linear-gradient(to bottom, #dcebf7 0%, #d4e6f6 66%, #D0E4F5 100%);
  border-top: 2px solid #444444;
}
table.blueTable tfoot td {
  font-size: 14px;
}
table.blueTable tfoot .links {
  text-align: right;
}
table.blueTable tfoot .links a{
  display: inline-block;
  background: #1C6EA4;
  color: #FFFFFF;
  padding: 2px 8px;
  border-radius: 5px;
}
    </style>
</head>

<body>
    <form id="form1" runat="server">


        <div>
            <label runat="server" id="lbG" style="color: red"><u>***For internal use only***</u></label>
            <br />
            ===================================================================================<br />
            <br />
            Date  From
            <asp:TextBox TextMode="Date" runat="server" ID="txtDateStart"></asp:TextBox>
            <br />
            Date To 
            <asp:TextBox TextMode="Date" runat="server" ID="txtDateTo"></asp:TextBox>
            <br />
            <asp:Button runat="server" ID="btnSearch" Text="Query" OnClick="btnSearch_Click" />
            <br />
            <br />
            ===================================================================================<br />
        
            ===================================================================================
        <br />
            <u>Department</u>
            <br />  

            <asp:Table ID="Table1" runat="server" Height="123px" Width="567px" class="blueTable">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell>Department</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Count</asp:TableHeaderCell>
                </asp:TableHeaderRow>

                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">Packaging</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="lblPackaging"></asp:Label></asp:TableCell>

                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">Marketing</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="lblMarketing"></asp:Label></asp:TableCell>

                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">QC</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="lblQC"></asp:Label>
                    </asp:TableCell>

                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">RD</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="lblRD"></asp:Label></asp:TableCell>

                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">PN</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="lblPN"></asp:Label></asp:TableCell>

                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">WH</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="lblWH"></asp:Label></asp:TableCell>

                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">T-Holding</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="lblTH"></asp:Label></asp:TableCell>

                </asp:TableRow>

                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">Vendor</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="lblVen"></asp:Label></asp:TableCell>

                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">Customer</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="lblCus"></asp:Label>
                        <br />
                    </asp:TableCell>

                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">Customer Dummy</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="LblCusDummy"></asp:Label>
                        <br />
                    </asp:TableCell>

                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">Etc.</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="lblEtc"></asp:Label> <asp:ListView runat="server" ID="lsEtc"></asp:ListView>
                        <br />
                    </asp:TableCell>

                </asp:TableRow>

                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">Total</asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:Label runat="server" ID="lblTotaltb"></asp:Label>
                        <br />
                    </asp:TableCell>

                </asp:TableRow>
            </asp:Table>

            <br />

                <br />
            <b>Dashboard opened</b>  :
            <asp:Label runat="server" ID="lblTotal"></asp:Label>
            <br />
            <br />
            ===================================================================================<br />
            <B><U>Customer & Vendor Usage</U></B><br />
            <br />
            <br />
            <br />
                   
             <table class="blueTable" style="width:300px">
        
                <thead>
                    <tr>
                        <th>Cutomer/Vendor</th>
                        <th>Total in xECM</th>
                        <th>Active in Artwork</th>
                        <th>Logon to Artwork</th>
                    </tr>
                            
               </thead>
                 <asp:Label runat="server" ID="lblCuslogin"></asp:Label>
                            <asp:Label runat="server" ID="lblVenlogin"></asp:Label>
                 </table>




    
            <br />
             <br />
            Customer Start A/W list 
                   
             <table class="blueTable" style="width:300px">
        
                <thead>
                    <tr>
                        <th>No.</th>
                        <th>Customer Code</th>
                        <th>Customer Description</th>
                    </tr>
                    
               </thead>
                <asp:Label runat="server" ID="lblCustomerList"></asp:Label>
                         
           </table>
            
             <br />
            <br />
            
           Total : <b><asp:Label runat="server" ID="lblCusTotal"> </asp:Label></b>
            <br />

            <br />

         
                 ===================================================================================<br />

                        <B>Artwork Terminate reason</B><br />
            <br />
            <br />

             <table class="blueTable" style="width:300px">
        
                <thead>
                    <tr>
                        <th>No</th>
                        <th>Reason</th>
                        <th>Total</th>
                        
                    </tr>
                    
               </thead>

                <asp:Label runat="server" ID="lblTerminateReason"></asp:Label>
                         
           </table>
               <br />
            <br />

             <B>Mockup Terminate reason</B><br />
            <br />
            <br />

           <table class="blueTable" style="width:300px">
                <thead>
                    <tr>
                        <th>No</th>
                        <th>Reason</th>
                        <th>Total</th>
                        
                    </tr>
                    
               </thead>

                <asp:Label runat="server" ID="lblMoterminateReason"></asp:Label>
                         
           </table>
            

            <br />
            ===================================================================================<br />
            <u>Workflows</u>

            <br />

               <table class="blueTable">
                <thead>
                    <tr>
                        <th>Workflow Type</th>
                        <th>In Process</th>
                        <th>Completed</th>
                        <th>Terminated</th>
                        <th>Total</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Mockup Normal</td>
                        <td><asp:Label runat="server" ID="lblMNp"></asp:Label></td>
                        <td><asp:Label runat="server" ID="lblMNc"></asp:Label></td>
                       <td><asp:Label runat="server" ID="lblMNt"></asp:Label></td>
                        <td><asp:Label runat="server" ID="lblMNa"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>Mockup for Die line</td>
                         <td><asp:Label runat="server" ID="lblMDp"></asp:Label></td>
                        <td><asp:Label runat="server" ID="lblMDc"></asp:Label></td>
                       <td><asp:Label runat="server" ID="lblMDt"></asp:Label></td>
                        <td><asp:Label runat="server" ID="lblMDa"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>Mockup for Project</td>
                         <td><asp:Label runat="server" ID="lblMPp"></asp:Label></td>
                        <td><asp:Label runat="server" ID="lblMPc"></asp:Label></td>
                       <td><asp:Label runat="server" ID="lblMPt"></asp:Label></td>
                        <td><asp:Label runat="server" ID="lblMPa"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>Artwork New</td>
                         <td><asp:Label runat="server" ID="lblAWNp"></asp:Label></td>
                          <td><asp:Label runat="server" ID="lblAWNc"></asp:Label></td>
                          <td><asp:Label runat="server" ID="lblAWNt"></asp:Label></td>
                          <td><asp:Label runat="server" ID="lblAWNa"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>Artwork Repeat</td>
                         <td><asp:Label runat="server" ID="lblAWRp"></asp:Label></td>
                          <td><asp:Label runat="server" ID="lblAWRc"></asp:Label></td>
                          <td><asp:Label runat="server" ID="lblAWRt"></asp:Label></td>
                          <td><asp:Label runat="server" ID="lblAWRa"></asp:Label></td>
                    </tr>
                      <tr>
                        <td>Artwork Repeat R6</td>
                                   <td><asp:Label runat="server" ID="lblAWR6p"></asp:Label></td>
                          <td><asp:Label runat="server" ID="lblAWR6c"></asp:Label></td>
                          <td><asp:Label runat="server" ID="lblAWR6t"></asp:Label></td>
                          <td><asp:Label runat="server" ID="lblAWR6a"></asp:Label></td>
                    </tr>
                      <tr>
                        <td>Total</td>
                         <td><asp:Label runat="server" ID="lblInTotal"></asp:Label></td>
                         <td><asp:Label runat="server" ID="lblComTotal"></asp:Label></td>
                       <td><asp:Label runat="server" ID="lblTerTotal"></asp:Label></td>
                          <td><asp:Label runat="server" ID="lblGrandTotal"></asp:Label></td>
                    </tr>
                </tbody>
            </table>

            <br />
            <%--<asp:Table runat="server" ID="tbDeparment" ></asp:Table>--%>
        </div>

         <p>
        Password Decrypt</p><br />
    
     Username: <asp:TextBox runat="server" ID="txtUsername"></asp:TextBox>  
    <asp:Button runat="server" ID="btnDecrypt" Text="Decrypt" OnClick="btnDecrypt_Click"/>
            <br />
    Password: <asp:Label runat="server" ID="lblPassword"></asp:Label>

        <br />
        <br />
        <br />
        <br />
        Mark SO Completed<br />
        <br />
        SO number <asp:TextBox runat="server" ID="txtSO"></asp:TextBox>  
    &nbsp;<asp:Button runat="server" ID="btnSOcomplete" Text="Mark Complete" OnClick="btnSOcomplete_Click"/>
        <br />
        Status :
        <asp:Label runat="server" ID="lblSOresult" Text=""></asp:Label>

        <br />

    </form>
   

   
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
</body>
</html>
