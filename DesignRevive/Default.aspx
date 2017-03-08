<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="DesignRevive._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

   <div class="container">
       <h2>Lead List</h2>
       <p>Below is a live feed of Leads</p>
   </div>
    <table class="table">
    <thead>
      <tr>
        <th>Business</th>
        <th>Website</th>
        <th>Email</th>
        <th>Telephone</th>
        <th>How Shit</th>
        <th>Action</th>
      </tr>
    </thead>
        <tbody>
            <tr class="info">
                <td>RBS Mentor</td>
                <td>www.rbsmentor.co.uk</td>
                <td>kevin@rbsmentor.co.uk</td>
                <td>8009709814</td>
                <td>Super Shit</td>
                <td><input type="checkbox" value="Action"></td>
            </tr>
            <tr class="info">
                <td>Portsmouth City Council</td>
                <td>www.portsmouth.gov.uk</td>
                <td>trevor@portsmouth.gov.uk</td>
                <td>023 9282 2251</td>
                <td>Not too bad fam</td>
                <td><input type="checkbox" value="Action"></td>
            </tr>
        </tbody>
    </table>

</asp:Content>
