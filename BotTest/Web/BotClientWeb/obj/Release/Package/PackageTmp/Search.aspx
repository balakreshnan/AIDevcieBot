<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Search.aspx.cs" Inherits="BotClientWeb.Search" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />

    <asp:TextBox ID="TextBox2" runat="server" Text="Search text here ..."></asp:TextBox><asp:Button ID="Button1" runat="server" Text="Search" OnClick="Button1_Click" />
    <br />
    <asp:TextBox ID="TextBox1" runat="server" Height="277px" Width="900px" 
        Wrap="true" TextMode="MultiLine"></asp:TextBox>
</asp:Content>
