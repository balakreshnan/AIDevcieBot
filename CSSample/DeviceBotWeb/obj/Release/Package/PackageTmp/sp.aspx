<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sp.aspx.cs" Inherits="DeviceBotWeb.sp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <meta http-equiv="X-UA-Compatible" content="IE=Edge">
    <script src="Scripts/speech.1.0.0.js" type="text/javascript"></script>
    <script type="text/javascript">
        var client;
        var request;

        function useMic() {
            return document.getElementById("useMic").checked;
        }

        function getMode() {
            return Microsoft.CognitiveServices.SpeechRecognition.SpeechRecognitionMode.shortPhrase;
        }

        function getKey() {
            //return document.getElementById("key").value;
            return "d41475ca4ca04d7883daf52bf0d5d3f3";
        }

        function getLanguage() {
            return "en-us";
        }

        function clearText() {
            document.getElementById("output").value = "";
        }

        function setText(text) {
            document.getElementById("output").value += text;
        }

        function getLuisConfig() {
            //var appid = document.getElementById("luis_appid").value;
            //var subid = document.getElementById("luis_subid").value;
            var appid = "fb463f12-f18d-4e5a-944e-2fa83471c16e";
            var subid = "3b01c054ef664909939099959d2c4586";

            if (appid.length > 0 && subid.length > 0) {
                return { appid: appid, subid: subid };
            }

            return null;
        }

        function start() {
            var mode = getMode();
            var luisCfg = getLuisConfig();

            clearText();

            if (useMic()) {
                if (luisCfg) {
                    client = Microsoft.CognitiveServices.SpeechRecognition.SpeechRecognitionServiceFactory.createMicrophoneClientWithIntent(
                        getLanguage(),
                        getKey(),
                        luisCfg.appid,
                        luisCfg.subid);
                } else {
                    client = Microsoft.CognitiveServices.SpeechRecognition.SpeechRecognitionServiceFactory.createMicrophoneClient(
                        mode,
                        getLanguage(),
                        getKey());
                }
                client.startMicAndRecognition();
                setTimeout(function () {
                    client.endMicAndRecognition();
                }, 5000);
            } else {
                if (luisCfg) {
                    client = Microsoft.CognitiveServices.SpeechRecognition.SpeechRecognitionServiceFactory.createDataClientWithIntent(
                        getLanguage(),
                        getKey(),
                        luisCfg.appid,
                        luisCfg.subid);
                } else {
                    client = Microsoft.CognitiveServices.SpeechRecognition.SpeechRecognitionServiceFactory.createDataClient(
                        mode,
                        getLanguage(),
                        getKey());
                }
                request = new XMLHttpRequest();
                request.open(
                    'GET',
                    (mode == Microsoft.CognitiveServices.SpeechRecognition.SpeechRecognitionMode.shortPhrase) ? "whatstheweatherlike.wav" : "batman.wav",
                    true);
                request.responseType = 'arraybuffer';
                request.onload = function () {
                    if (request.status !== 200) {
                        setText("unable to receive audio file");
                    } else {
                        client.sendAudio(request.response, request.response.length);
                    }
                };

                request.send();
            }

            client.onPartialResponseReceived = function (response) {
                setText(response);
            }

            client.onFinalResponseReceived = function (response) {
                setText(JSON.stringify(response));
            }

            client.onIntentReceived = function (response) {
                setText(response);
            };
        }
    </script>
</head>
<body style="font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif">
    <form id="form1">
        <table width="100%">
        <tr><td/><td><h1>Speech.JS</h1><h2>Microsoft Cognitive Services</h2></td></tr>
        <tr>
            <td align="right"/>
            <td><input id="useMic" type="checkbox">Use Microphone</td>
        </tr>

        <tr>
            <!--<td align="right"><A href="https://www.microsoft.com/cognitive-services/en-us/sign-up" target="_blank">Subscription</A>:</td>-->
            <td><input id="key" type="hidden" value="d41475ca4ca04d7883daf52bf0d5d3f3" size="40"></td>
        </tr>
        <tr>
            <!--<td align="right">LUIS AppId:</td>-->
            <td><input id="luis_appid" type="hidden" value="fb463f12-f18d-4e5a-944e-2fa83471c16e" size="40" ></td>
        </tr>
        <tr>
            <!--<td align="right">LUIS SubscriptionId:</td>-->
            <td><input id="luis_subid" type="hidden" value="3b01c054ef664909939099959d2c4586" size="40" ></td>
        </tr>

        <tr>
            <td />
            <td><button onclick="start()">Start</button></td>
        </tr>
        <tr><td/>
            <td>
                <textarea id="output" style='width:400px;height:200px'></textarea>
            </td>
        </tr>
        <tr><td />
            <td><a href="docs/speech.1.0.0.js/index.html" target="_blank">Speech.JS Documentation</a></td>
        </tr>
    </table>
    </form>
</body>
</html>
