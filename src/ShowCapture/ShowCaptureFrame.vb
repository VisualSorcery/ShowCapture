Imports System.IO

Public Class ShowCaptureFrame

    Public Payloads As New List(Of Payload)
    Public FrameNumber As Integer

    Private _isKeyframe
    Public ReadOnly Property IsKeyFrame
        Get
            Return _isKeyframe
        End Get
    End Property

    Public Property AncillaryData As Byte()

    Public Sub New()

    End Sub

    Public Sub New(isKeyframe As Boolean)
        _isKeyframe = isKeyframe
    End Sub

    Public Function ToBytes() As Byte()

        Dim payloadBytes As New List(Of Byte())
        Dim headerLength As Integer = 10

        Dim ancillaryDataLength As Integer = 0

        If IsNothing(AncillaryData) = False Then
            ancillaryDataLength = AncillaryData.Length
        End If

        Dim bufferSize As Integer = headerLength + ancillaryDataLength + 4

        For Each p In Payloads
            payloadBytes.Add(p.ToBytes)
            bufferSize += payloadBytes(payloadBytes.Count - 1).Length
        Next


        Dim buffer(bufferSize - 1) As Byte

        'header
        'Frame Number
        Array.Copy(BitConverter.GetBytes(FrameNumber), 0, buffer, 0, 4)
        'Payload Count
        Array.Copy(BitConverter.GetBytes(CShort(Payloads.Count)), 0, buffer, 4, 2)

        'Ancillary Data Length

        Array.Copy(BitConverter.GetBytes(ancillaryDataLength), 0, buffer, 10, 4)

        'Ancillary Data
        If ancillaryDataLength > 0 Then
            Array.Copy(AncillaryData, 0, buffer, 14, AncillaryData.Length)
        End If

        'Payloads
        Dim index As Integer = headerLength + ancillaryDataLength + 4
        For i = 0 To payloadBytes.Count - 1
            Array.Copy(payloadBytes(i), 0, buffer, index, payloadBytes(i).Count)
            index += payloadBytes(i).Count
        Next

        Return buffer

    End Function

    Public Shared Function ReadFromStream(stream As FileStream, offset As Long) As ShowCaptureFrame

        Dim frame As New ShowCaptureFrame()

        'Read Frame Header
        Dim frameHeaderSize As Integer = 10
        Dim frameHeaderBuffer(frameHeaderSize - 1) As Byte

        stream.Position = offset

        stream.Read(frameHeaderBuffer, 0, frameHeaderSize)

        Dim frameNumber As Integer = BitConverter.ToInt32(frameHeaderBuffer, 0)
        Dim payloadCount As Integer = BitConverter.ToInt16(frameHeaderBuffer, 4)


        'read frame ancillary data
        Dim ancillaryLengthBuffer(3) As Byte
        stream.Read(ancillaryLengthBuffer, 0, 4)

        Dim ancillaryLength As Integer = BitConverter.ToInt32(ancillaryLengthBuffer, 0)


        If ancillaryLength > 0 Then
            Dim ancillaryDataBuffer(ancillaryLength - 1) As Byte
            stream.Read(ancillaryDataBuffer, 0, ancillaryLength)
            frame.AncillaryData = ancillaryDataBuffer
        End If

        frame.FrameNumber = frameNumber

        'Read payload
        For i = 0 To payloadCount - 1

            'Read header
            Dim payloadHeaderSize As Integer = 5
            Dim payloadHeaderBuffer(payloadHeaderSize - 1) As Byte
            stream.Read(payloadHeaderBuffer, 0, payloadHeaderSize)

            Dim payloadType As PayloadType = CType(payloadHeaderBuffer(0), PayloadType)
            Dim payloadLength As Integer = BitConverter.ToInt32(payloadHeaderBuffer, 1)


            'Read payload
            Dim payloadBuffer(payloadLength - payloadHeaderSize - 1) As Byte
            stream.Read(payloadBuffer, 0, payloadLength - payloadHeaderSize)

            Dim payload As Payload = Nothing

            Select Case payloadType
                Case payloadType.DMXUniverse
                    payload = New DMXUniversePayload(payloadBuffer)
                Case payloadType.MidiShowControl
                    payload = New MidiShowControlPayload(payloadBuffer)
            End Select

            If payload Is Nothing = False Then
                frame.Payloads.Add(payload)
            End If

        Next

        Return frame

    End Function

End Class
