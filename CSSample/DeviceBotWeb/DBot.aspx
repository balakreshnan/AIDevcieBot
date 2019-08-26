<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="DBot.aspx.cs" Inherits="DeviceBotWeb.DBot" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <br />

    <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" Height="616px"
        Width="1147px"
        ActiveTabIndex="0"        
        OnDemand="true"        
        AutoPostBack="false"
        TabStripPlacement="Top"
        CssClass="ajax__tab_xp"
        ScrollBars="Vertical"
        VerticalStripWidth="120px"
        >
        <ajaxToolkit:TabPanel ID="TabPanel1" 
            HeaderText="TabPanel1" runat="server">
            <HeaderTemplate>
                Bot Connector
            </HeaderTemplate>
            <ContentTemplate>
                <table style="width: auto">
                    <tr>
                        <td>
                <iframe id="I1" name="I1" src="https://webchat.botframework.com/embed/devicebot?s=qwhrMKWs0Jo.cwA.iFk.pBDKzbvhy_H5GMBqwVWCZzps4Nmkvr3_E8YwlVUkfY8" 
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
<p>First connect to the device. For that please type</p>
<ul style="list-style-type: disc;">
<li>&nbsp;&nbsp; <em>connect</em></li>
</ul>
<p>When it asks for serial number please type</p>
<ul style="list-style-type: disc;">
<li>&nbsp;&nbsp; <em>device1</em></li>
</ul>
<p>To get the temperature data these are some of the Utterance you can type in</p>
<ul style="list-style-type: disc;">
<li>&nbsp;&nbsp; Show me the temp</li>
<li>&nbsp;&nbsp; Show temp</li>
<li>&nbsp;&nbsp; What is the temperature</li>
<li>&nbsp;&nbsp; Show me the temperature</li>
</ul>
<p>&nbsp;</p>
<p>To get the Humidity data these are some of the Utterance you can type in</p>
<ul style="list-style-type: disc;">
<li>Show me humidity</li>
<li>What is humidity</li>
</ul>
<p>&nbsp;</p>
<p>To get the Pressure data these are some of the Utterance you can type in</p>
<ul style="list-style-type: disc;">
<li>What is the weight of the air</li>
<li>Show me the weight of the air</li>
</ul>
<p>&nbsp;</p>
<p>To get the Speed data these are some of the Utterance you can type in</p>
<ul style="list-style-type: disc;">
<li>What is the speed</li>
<li>Show me the speed</li>
</ul>
<p>&nbsp;</p>
<p>To get the Compass data these are some of the Utterance you can type in</p>
<ul style="list-style-type: disc;">
<li>What is the compass reading</li>
<li>Show me compass</li>
</ul>
<p>&nbsp;</p>
<p>To get the Device Information including Device Twin data you can type in</p>
<ul style="list-style-type: disc;">
<li>What is the device information</li>
<li>What is the device info</li>
<li>Show me device info</li>
</ul>
<p>&nbsp;</p>
<p>To see the fault list you can type in</p>
<ul style="list-style-type: disc;">
<li>Show me faults</li>
<li>Show me top 10 faults</li>
<li>Show critical faults</li>
</ul>
<p>&nbsp;</p>
<p>To get the device properties you can type in</p>
<ul style="list-style-type: disc;">
<li>Show me device properties</li>
<li>List device properties</li>
</ul>
<p>&nbsp;</p>
<p>Now to send command into the device you can send commands to be processed these are some commands to type.</p>
<ul style="list-style-type: disc;">
<li>Set Temperature to 10</li>
<li>Configure temperature to 10</li>
<li>Set Humidity to 20</li>
<li>Configure Humidity to 20</li>
</ul>
<p>&nbsp;</p>
<p>To Analyze a image - Computer Vision API type the below sentence.</p>
<ul style="list-style-type: disc;">
<li>Analyze Image</li>
<li>Click the image upload button on the left side of where you are typing</li>
<li>Select the jpeg file to upload</li>
<li>the Bot will display the results</li>
</ul>
<p>&nbsp;</p>
<p>To Detect Object - Custom vision type the below sentence.</p>
<ul style="list-style-type: disc;">
<li>Detect Image</li>
<li>Click the image upload button on the left side of where you are typing</li>
<li>Select the jpeg file to upload</li>
<li>the Bot will display the results</li>
</ul>
<p>&nbsp;</p>
<p>To list all the devices availble</p>
<ul style="list-style-type: disc;">
<li>Show device</li>
<li>List all devices</li>
<li>List devices</li>
</ul>
<p>&nbsp;</p>
<p>To display Availability</p>
<ul style="list-style-type: disc;">
<li>Show Availability</li>
<li>what is Availability</li>
<li>List Availability</li>
</ul>
<p>&nbsp;</p>
<p>To display Yield</p>
<ul style="list-style-type: disc;">
<li>Show Yield</li>
<li>what is Yield</li>
<li>List Yield</li>
</ul>
<p>&nbsp;</p>
<p>To display performance</p>
<ul style="list-style-type: disc;">
<li>Show performance</li>
<li>what is performance</li>
<li>List performance</li>
</ul>
<p>&nbsp;</p>
<p>To display Quality</p>
<ul style="list-style-type: disc;">
<li>Show Quality</li>
<li>what is Quality</li>
<li>List Quality</li>
</ul>

<p>&nbsp;</p>
<p>To display energy consumption</p>
<ul style="list-style-type: disc;">
<li>Show energy consumption</li>
<li>what is energy consumption</li>
<li>List energy consumption</li>
</ul>
<p>&nbsp;</p>
<p>To display productivity</p>
<ul style="list-style-type: disc;">
<li>Show productivity</li>
<li>what is productivity</li>
<li>List productivity</li>
</ul>

<p>&nbsp;</p>
<p>To exit or quit out the above device1 please type (This will exit out from device1)</p>
<ul style="list-style-type: disc;">
<li><em>Exit</em></li>
<li><em>Quit</em></li>
</ul>
                            </div>
                        </td>
                    </tr>
                </table>

            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2" Visible="false">
            <HeaderTemplate>
                Speech - LUIS
            </HeaderTemplate>
            <ContentTemplate>
                <iframe id="I2" name="I2" src="index.htm" style="width: 772px; height: 540px;"></iframe>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="TabPanel3" runat="server" HeaderText="TabPanel3" Visible="true">
            <HeaderTemplate>
                Face Api
            </HeaderTemplate>
            <ContentTemplate>
                <iframe id="I3" name="I3" src="face.aspx" style="width: 772px; height: 540px;"></iframe>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
       <ajaxToolkit:TabPanel ID="TabPanel4" runat="server" HeaderText="TabPanel4" Visible="false">
            <HeaderTemplate>
                Face Verify
            </HeaderTemplate>
            <ContentTemplate>
                <iframe id="I4" name="I4" src="FaceIdentify.aspx" style="width: 772px; height: 640px;"></iframe>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="TabPanel5" runat="server" HeaderText="TabPanel5">
            <HeaderTemplate>
                Face Train
            </HeaderTemplate>
            <ContentTemplate>
                <iframe id="I5" name="I5" src="FaceTrain.aspx" style="width: 772px; height: 640px;"></iframe>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="TabPanel6" runat="server" HeaderText="TabPanel6">
            <HeaderTemplate>
                Face Detect
            </HeaderTemplate>
            <ContentTemplate>
                <iframe id="I6" name="I6" src="FaceDetect.aspx" style="width: 772px; height: 640px;"></iframe>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="TabPanel7" runat="server" HeaderText="TabPanel7">
            <HeaderTemplate>
                Digital Twin Architecture
            </HeaderTemplate>
            <ContentTemplate>
                <img alt="Digital Twin AI Architecture" src="images/DeviceBot.jpg" 
                    style="width: 716px; height: 530px; vertical-align: middle; align-content:center; align-items:center" />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
         <ajaxToolkit:TabPanel ID="TabPanel8" runat="server" HeaderText="TabPanel8" Visible="false">
            <HeaderTemplate>
                Voice Identification
            </HeaderTemplate>
            <ContentTemplate>
                <iframe id="I8" name="I8" src="VoiceIdentification.aspx" style="width: 772px; height: 640px;"></iframe>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
         <ajaxToolkit:TabPanel ID="TabPanel9" runat="server" HeaderText="TabPanel9" Visible="false">
            <HeaderTemplate>
                Voice Verify
            </HeaderTemplate>
            <ContentTemplate>
                <iframe id="I9" name="I9" src="VoiceVerify.aspx" style="width: 772px; height: 640px;"></iframe>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="TabPanel10" runat="server" HeaderText="TabPanel10" Visible="true">
            <HeaderTemplate>
                Object Detection
            </HeaderTemplate>
            <ContentTemplate>
                <iframe id="I10" name="I10" src="customvision.aspx" style="width: 772px; height: 640px;"></iframe>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>

    

</asp:Content>
