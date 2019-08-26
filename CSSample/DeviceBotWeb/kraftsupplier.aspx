<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="kraftsupplier.aspx.cs" Inherits="DeviceBotWeb.kraftsupplier" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <br />
    <br />

    <table style="width: auto">
                    <tr>
                        <td>
                <iframe id="I1" name="I1" src="https://webchat.botframework.com/embed/kraftpocv1?s=uzjpCsqD4y8.cwA.t00.dwc6tNp_3ktp-1F3KQN9p8-vO1esY9hOyV4S29c7Mc4" 
                    style="width: 400px; height: 540px;"></iframe>
                        </td>
                        <td style="width: 50px"></td>
                        <td>
                           <div>
                               <b>Help - How to communicate with Bot </b><br />
                           </div>
                            <br />
                            <div style="height: 540px; overflow:scroll; width: 608px;">
                                <p><span style="margin: 0px; color: black; line-height: 107%; font-family: 'Verdana',sans-serif; font-size: 8.5pt;">Note: Italics are commands to connect to device and disconnect from device. The Bullet lines are Natural Language that you can type to bot for response</span></p>
<p>&nbsp;</p>
<p>Type Hello or Hi or Hey</p>
<ul style="list-style-type: disc;">
<li>&nbsp;&nbsp; <em>Type the supplier name: (ex: supplier1)</em></li>
</ul>
<p>now it will ask: What can i do for you?</p>
<ul style="list-style-type: disc;">
<li>&nbsp;&nbsp; <em>type: show my forecast</em></li>
</ul>
<p>Now that you should see the forecast</p>
<ul style="list-style-type: disc;">
<li>&nbsp;&nbsp; i can do it</li>
<li>&nbsp;&nbsp; I cant do it</li>
<li>&nbsp;&nbsp; i cannot fulfill the forecast</li>
<li>&nbsp;&nbsp; i can fulfill the forecast</li>
</ul>
<p>&nbsp;</p>
<p>Follow the prompt</p>

                            </div>
                        </td>
                    </tr>
                </table>

</asp:Content>
