/* Copyright 2013 Chris Wilson

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

window.AudioContext = window.AudioContext || window.webkitAudioContext;

var audioContext = new AudioContext();
var audioInput = null,
    realAudioInput = null,
    inputPoint = null,
    audioRecorder = null;
var rafID = null;
var analyserContext = null;
var canvasWidth, canvasHeight;
var recIndex = 0;

/* TODO:

- offer mono option
- "Monitor input" switch
*/

function saveAudio() {
    audioRecorder.exportWAV( doneEncoding );
    // could get mono instead by saying
    // audioRecorder.exportMonoWAV( doneEncoding );
}

function gotBuffers( buffers ) {
    var canvas = document.getElementById("wavedisplay");
    //alert("canvas");
    

    drawBuffer( canvas.width, canvas.height, canvas.getContext('2d'), buffers[0] );

    // the ONLY time gotBuffers is called is right after a new recording is completed - 
    // so here's where we should set up the download.
    audioRecorder.exportWAV(doneEncoding);
    //once the coding is done then save the value
    //audioRecorder.exportWAV(function (blob) {
        //the generated blob contains the wav file
    //    var formfield = document.getElementById("formfield");
    //    formfield.value = blob;
        //alert(blob);
    //});
    //var formfield = document.getElementById("formfield");
    //formfield.value = buffers[0];
    
    
}


/*
// Provides a Stream for a file in webpage, inheriting from NodeJS Readable stream.
var Buffer = require('buffer').Buffer;
var Stream = require('stream');
var util = require('util');

function FileStream(file, opt) {
    Stream.Readable.call(this, opt);

    this.fileReader = new FileReader(file);
    this.file = file;
    this.size = file.size;
    this.chunkSize = 1024 * 1024 * 4; // 4MB
    this.offset = 0;
    var _me = this;

    this.fileReader.onloadend = function loaded(event) {
        var data = event.target.result;
        var buf = Buffer.from(data);
        _me.push(buf);
    }
}
util.inherits(FileStream, Stream.Readable);
FileStream.prototype._read = function () {
    if (this.offset > this.size) {
        this.push(null);
    } else {
        var end = this.offset + this.chunkSize;
        var slice = this.file.slice(this.offset, end);
        this.fileReader.readAsArrayBuffer(slice);
        this.offset = end;
    }
};

function refreshProgress() {
    setTimeout(function () {
        if (!finishedOrError) {
            var process = speedSummary.getCompletePercent();
            displayProcess(process);
            refreshProgress();
        }
    }, 200);
}*/

function doneEncoding(blob) {

    Recorder.setupDownload( blob, "myRecording" + ((recIndex<10)?"0":"") + recIndex + ".wav" );
    recIndex++;

    var formfield = document.getElementById("formfield");
    var url = (window.URL || window.webkitURL).createObjectURL(blob);
    //formfield.value = url;
    /*
    var arrayBuffer;
    var fileReader = new FileReader();
    fileReader.onload = function () {
        arrayBuffer = this.result;
    };
    fileReader.readAsArrayBuffer(blob);
    formfield.value = arrayBuffer;
    */
    formfield.value = blob;
    //formfield.value = reader.readAsArrayBuffer(blob);

    var audio = document.getElementById('test');
    audio.src = url;
    audio.load();

    var reader = new FileReader(url);
    reader.onload = function (event) {
        formfield.value = event.target.result;
    };
    reader.readAsDataURL(blob);


    /*

    var blobUri = 'https://' + 'bbiotstore' + '.blob.core.windows.net';
    var blobService = AzureStorage.createBlobServiceWithSas(blobUri, '?sv=2016-05-31&ss=bfqt&srt=sco&sp=rwdlacup&se=2017-06-09T00:59:28Z&st=2017-06-08T16:59:28Z&spr=https,http&sig=T1%2FlyqgC9ThfnXiNqUQBz0rfPFR%2FZ%2Bh4YApO0nanSvs%3D');

    var fileStream = new FileStream(url);

    var customBlockSize = file.size > 1024 * 1024 * 32 ? 1024 * 1024 * 4 : 1024 * 512;
    blobService.singleBlobPutThresholdInBytes = customBlockSize;

    var finishedOrError = false;
    var speedSummary = blobService.createBlockBlobFromStream('bbvoice1', file.name, fileStream, file.size, { blockSize: customBlockSize }, function (error, result, response) {
        finishedOrError = true;
        if (error) {
            // Upload blob failed
        } else {
            // Upload successfully
            alert("Upload successful");
        }
    });
    refreshProgress();*/
    
}

function toggleRecording( e ) {
    if (e.classList.contains("recording")) {
        // stop recording
        audioRecorder.stop();
        e.classList.remove("recording");
        audioRecorder.getBuffers( gotBuffers );
    } else {
        // start recording
        if (!audioRecorder)
            return;
        e.classList.add("recording");
        audioRecorder.clear();
        audioRecorder.record();
    }
}

function convertToMono( input ) {
    var splitter = audioContext.createChannelSplitter(2);
    var merger = audioContext.createChannelMerger(2);

    input.connect( splitter );
    splitter.connect( merger, 0, 0 );
    splitter.connect( merger, 0, 1 );
    return merger;
}

function cancelAnalyserUpdates() {
    window.cancelAnimationFrame( rafID );
    rafID = null;
}

function updateAnalysers(time) {
    if (!analyserContext) {
        var canvas = document.getElementById("analyser");
        canvasWidth = canvas.width;
        canvasHeight = canvas.height;
        analyserContext = canvas.getContext('2d');
    }

    // analyzer draw code here
    {
        var SPACING = 3;
        var BAR_WIDTH = 1;
        var numBars = Math.round(canvasWidth / SPACING);
        var freqByteData = new Uint8Array(analyserNode.frequencyBinCount);

        analyserNode.getByteFrequencyData(freqByteData); 

        analyserContext.clearRect(0, 0, canvasWidth, canvasHeight);
        analyserContext.fillStyle = '#F6D565';
        analyserContext.lineCap = 'round';
        var multiplier = analyserNode.frequencyBinCount / numBars;

        // Draw rectangle for each frequency bin.
        for (var i = 0; i < numBars; ++i) {
            var magnitude = 0;
            var offset = Math.floor( i * multiplier );
            // gotta sum/average the block, or we miss narrow-bandwidth spikes
            for (var j = 0; j< multiplier; j++)
                magnitude += freqByteData[offset + j];
            magnitude = magnitude / multiplier;
            var magnitude2 = freqByteData[i * multiplier];
            analyserContext.fillStyle = "hsl( " + Math.round((i*360)/numBars) + ", 100%, 50%)";
            analyserContext.fillRect(i * SPACING, canvasHeight, BAR_WIDTH, -magnitude);
        }
    }
    
    rafID = window.requestAnimationFrame( updateAnalysers );
}

function toggleMono() {
    if (audioInput != realAudioInput) {
        audioInput.disconnect();
        realAudioInput.disconnect();
        audioInput = realAudioInput;
    } else {
        realAudioInput.disconnect();
        audioInput = convertToMono( realAudioInput );
    }

    audioInput.connect(inputPoint);
}

function gotStream(stream) {
    inputPoint = audioContext.createGain();

    // Create an AudioNode from the stream.
    realAudioInput = audioContext.createMediaStreamSource(stream);
    audioInput = realAudioInput;
    audioInput.connect(inputPoint);

//    audioInput = convertToMono( input );

    analyserNode = audioContext.createAnalyser();
    analyserNode.fftSize = 2048;
    inputPoint.connect( analyserNode );

    audioRecorder = new Recorder( inputPoint );

    zeroGain = audioContext.createGain();
    zeroGain.gain.value = 0.0;
    inputPoint.connect( zeroGain );
    zeroGain.connect( audioContext.destination );
    updateAnalysers();
}

function initAudio() {
        if (!navigator.getUserMedia)
            navigator.getUserMedia = navigator.webkitGetUserMedia || navigator.mozGetUserMedia;
        if (!navigator.cancelAnimationFrame)
            navigator.cancelAnimationFrame = navigator.webkitCancelAnimationFrame || navigator.mozCancelAnimationFrame;
        if (!navigator.requestAnimationFrame)
            navigator.requestAnimationFrame = navigator.webkitRequestAnimationFrame || navigator.mozRequestAnimationFrame;

    navigator.getUserMedia(
        {
            "audio": {
                "mandatory": {
                    "googEchoCancellation": "false",
                    "googAutoGainControl": "false",
                    "googNoiseSuppression": "false",
                    "googHighpassFilter": "false"
                },
                "optional": []
            },
        }, gotStream, function(e) {
            alert('Error getting audio');
            console.log(e);
        });
}

window.addEventListener('load', initAudio );
