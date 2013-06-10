<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PushBid.aspx.cs" Inherits="GovBids.PushBid" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>GovBids PUSH</title>
</head>
<body style="background-color: #ffffff; background-image: url('../../APP_THEMES/Images/Master_Bkgrd.png'); background-repeat: repeat-x; font-size: 1em; font-family: Helvetica Neue, Lucida Grande, Segoe UI, Arial, Helvetica, Verdana, sans-serif; margin: 0px; padding: 0px; color: #000000;">
    <form id="form1" runat="server" style="text-align: center;">
        <div style="width: 100%; display: block; float: left;">
            <br />
            <br />
            <br />
            <br />
            <br />
            <p>
                <asp:Label ID="lblBidID" runat="server" Text="Label">Available Bids:</asp:Label>
                &nbsp;&nbsp;&nbsp;
                <asp:DropDownList ID="ddlBids" runat="server" Style="width: 500px; border: 1px solid #000000; vertical-align: middle;" Height="25px" AutoPostBack="True" OnSelectedIndexChanged="ddlBids_SelectedIndexChanged"></asp:DropDownList>
                &nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnPush" runat="server" Text="Push" OnClick="btnPush_Click" Enabled="False" />
            </p>
        </div>
        <%--<asp:LinkButton ID="lnkTest" runat="server" OnClick="lnkTest_Click">Test</asp:LinkButton>--%>
    </form>
</body>
</html>
