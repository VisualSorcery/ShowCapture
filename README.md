# ShowCapture
A .NET library for use in encoding and decoding Visual Sorcery's Show Capture file format.

### What is ShowCapture?
ShowCapture is a lossless temporally compressed multi-payload show recording format. It allows you to record as many universes of DMX as you would like, as well as Midi Show Control commands and Linear Timecode values to be played back later exactly as they were recorded. This library allows you to easily encode to and decode from the ShowCapture format. An overview of the file format can be found [here](https://docs.google.com/drawings/d/1ZFihuPPBTzsGAcpb7UglUr6_0VAqNu2fDR1VL0ZtNjw/edit?usp=sharing).

## Usage

### Encoding

To encode, simply create an Encoder object, add the desired payload to the frame, and then advance to the next frame, or close the file

```vb.net
Dim showEncode as New Encoder()

'initialize the encoder
Dim framerate as Single = 29.97f
Dim keyframeFrequency as Integer = 30

showEncode.Initialize(framerate, keyframeFrequency)

'add a payload to the current frame
Dim payloadA as New DMXUniverse(1) 'create a dmx universe to contain the data for universe 1

showEncode.AddPayload(payload)

'Advance to the next frame
showEncode.AdvanceFrame()

Dim payloadB as New DMXUniverse(1) 'create a dmx universe to contain the data for universe 1

showEncode.AddPayload(payload)

'close the file
showEncode.closeFile()
```

### Decoding

To decode, create a Decoder object, specify the file to load, and then request a frame.

```vb.net
Dim showDecode as New Decoder()

showDecode.Load("**YourFilename**")

Dim requestedFrame as Frame = showDecode.GetFrame(125)

'Access the payloads
For Each payload in requestedFrame.Payloads
	'do something with the payload
Next

```

## Payloads
ShowCapture currently supports the following payloads:

- DMX - DMXUniverse
- Midi Show Control - MidiShowControlCommand
- Linear Timecode - LinearTimeCodeFrame