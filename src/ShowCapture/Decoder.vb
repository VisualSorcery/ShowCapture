Imports System.IO
Imports ShowCapture.Codec
Imports ShowCapture.Codec.InternalPayloads
Imports ShowCapture.Payloads

Public Class Decoder
    Implements IDisposable

    Private input As FileStream

    Private _frameRate As Single
    Public ReadOnly Property FrameRate As Single
        Get
            Return _frameRate
        End Get
    End Property

    Private _duration As Double
    Public ReadOnly Property Duration As Double
        Get
            Return _duration
        End Get
    End Property

    Private _frameCount As Integer
    Public ReadOnly Property FrameCount As Integer
        Get
            Return _frameCount
        End Get
    End Property

    Private _keyframeFrequency As Short
    Public ReadOnly Property KeyframeFrequency As Short
        Get
            Return _keyframeFrequency
        End Get
    End Property

    Private glossary As KeyframeGlossary

    Public Sub Load(file As String)

        input = New FileStream(file, FileMode.Open)

        glossary = KeyframeGlossary.ReadFromStream(input)

        LoadHeader()

    End Sub

    Public Sub Close()
        input.Close()
    End Sub

    Public Function GetFrame(frameNumber As Integer) As Frame

        If frameNumber < FrameCount And frameNumber >= 0 Then

            Dim payloads As New List(Of Payload)

            Dim keyframeIndex As Integer = Math.Floor(frameNumber / KeyframeFrequency)
            Dim closestKeyframe As KeyframeGlossaryItem

            Dim ancillaryData() As Byte = Nothing

            If glossary.Items.Count > keyframeIndex Then
                closestKeyframe = glossary.Items(keyframeIndex)

                Dim keyframe As ShowCaptureFrame = ShowCaptureFrame.ReadFromStream(input, closestKeyframe.ByteIndex)

                'merge the keyframe with an empty universe to create a new universe
                For Each payload In keyframe.Payloads

                    If TypeOf payload Is DMXUniversePayload Then

                        Dim universe As DMXUniversePayload = CType(payload, DMXUniversePayload)

                        Dim newUniverse As New DMXUniverse(universe.Universe)
                        LatestTakesPrecedenceMerge(newUniverse, universe)
                        payloads.Add(newUniverse)
                    End If

                Next

                'merge all subsequent frames after the keyframe with the keyframe to create the result
                If frameNumber <> closestKeyframe.FrameNumber Then
                    For i = closestKeyframe.FrameNumber + 1 To frameNumber

                        Dim captureFrame As ShowCaptureFrame = ShowCaptureFrame.ReadFromStream(input, input.Position)
                        ancillaryData = captureFrame.AncillaryData
                        For j = 0 To payloads.Count - 1
                            For k = 0 To captureFrame.Payloads.Count - 1

                                If TypeOf captureFrame.Payloads(k) Is DMXUniversePayload And TypeOf payloads(j) Is DMXUniverse Then

                                    Dim universePayload As DMXUniversePayload = CType(captureFrame.Payloads(k), DMXUniversePayload)

                                    Dim container As DMXUniverse = CType(payloads(j), DMXUniverse)

                                    If container.UniverseNumber = universePayload.Universe Then
                                        LatestTakesPrecedenceMerge(payloads(j), universePayload)
                                    End If

                                ElseIf TypeOf captureFrame.Payloads(k) Is MidiShowControlPayload And i = frameNumber Then

                                    Dim msc As MidiShowControlPayload = CType(captureFrame.Payloads(j), MidiShowControlPayload)
                                    payloads.Add(msc.GetContainer)

                                ElseIf TypeOf captureFrame.Payloads(k) Is LinearTimeCodePayload And i = frameNumber Then

                                    Dim ltc As LinearTimeCodePayload = CType(captureFrame.Payloads(j), LinearTimeCodePayload)
                                    payloads.Add(ltc.GetContainer)

                                End If

                            Next
                        Next
                    Next
                Else
                    ancillaryData = keyframe.AncillaryData
                End If

            End If

            Dim frame As New Frame(frameNumber, payloads, ancillaryData)
            Return frame

        Else
            Throw New IndexOutOfRangeException("Requested frame outside of valid range")
        End If
    End Function

    Private Sub LoadHeader()

        Dim headerBuffer(19) As Byte

        input.Position = 0

        input.Read(headerBuffer, 0, 20)

        _frameRate = BitConverter.ToSingle(headerBuffer, 0)
        _keyframeFrequency = BitConverter.ToInt16(headerBuffer, 4)
        _frameCount = BitConverter.ToInt32(headerBuffer, 6)
        _duration = FrameCount / FrameRate

    End Sub

    Private Sub LatestTakesPrecedenceMerge(originalUniverse As DMXUniverse, newUniverse As DMXUniversePayload)

        For Each item In newUniverse.Items
            originalUniverse.DMX(item.Address) = item.Value
        Next

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                input.Close()
            End If
        End If
        Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
