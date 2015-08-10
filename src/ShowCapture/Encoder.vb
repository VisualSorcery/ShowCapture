Imports System.IO
Imports ShowCapture.Codec
Imports ShowCapture.Codec.InternalPayloads
Imports ShowCapture.Payloads

Public Class Encoder
    Implements IDisposable

    Private frame As New ShowCaptureFrame

    Private currentDMX As New List(Of DMXUniverse)
    Private dmxHistory As New List(Of DMXUniverse)

    Private keyframeFrequency As Short
    Private currentFrame As Integer
    Private framerate As Single
    Private path As String
    Private glossary As New KeyframeGlossary
    Private output As System.IO.FileStream

    Public Property IsCapturing As Boolean

    Public Sub New()

    End Sub

#Region "Public Methods"

    Public Sub Initialize(path As String, framerate As Single, keyframeFrequency As Integer)

        If IsCapturing Then
            Throw New InvalidOperationException("A capture is already in progress.")
        End If

        Me.keyframeFrequency = keyframeFrequency
        Me.framerate = framerate
        Me.path = path

        IsCapturing = True
        output = New FileStream(path, FileMode.Create)
        OutputHeader()
        StartFrame()


    End Sub

    Public Sub AddPayload(universe As DMXUniverse)

        frame.Payloads.Add(CreateUniverse(universe))
        currentDMX.Add(universe)

    End Sub

    Public Sub AddPayload(msc As MidiShowControl)

        Dim mscPayload As New MidiShowControlPayload()
        mscPayload.SetCommand(msc)
        frame.Payloads.Add(mscPayload)

    End Sub

    Public Sub AddPayload(ltc As LinearTimeCode)

        Dim ltcPayload As New LinearTimeCodePayload()
        ltcPayload.SetValue(ltc)
        frame.Payloads.Add(ltcPayload)

    End Sub

    Public Sub CloseFile()

        If IsCapturing = False Then
            Throw New InvalidOperationException("There is no capture currently in progress.")
        End If

        OutputGlossary()
        OutputFrameCount()
        output.Close()
        IsCapturing = False

    End Sub

    Public Sub AdvanceFrame()

        CloseFrame()
        StartFrame()

    End Sub

#End Region

    Private Sub StartFrame()

        Dim isKeyframe As Boolean = currentFrame Mod keyframeFrequency = 0
        frame = New ShowCaptureFrame(isKeyframe)
        frame.FrameNumber = currentFrame

    End Sub

    Private Sub CloseFrame()

        If frame.IsKeyFrame Then
            Dim glossaryItem As New KeyframeGlossaryItem(frame.FrameNumber, output.Position)
            glossary.Items.Add(glossaryItem)
        End If

        OutputFrame()
        currentFrame += 1

        dmxHistory.Clear()

        For i = 0 To currentDMX.Count - 1
            Dim universe As New DMXUniverse(currentDMX(i).UniverseNumber)
            universe.DMX = currentDMX(i).DMX
            dmxHistory.Add(universe)
        Next

        currentDMX = New List(Of DMXUniverse)

    End Sub

    Private Sub OutputHeader()

        output.Position = 0
        Dim buffer(19) As Byte
        Array.Copy(BitConverter.GetBytes(framerate), 0, buffer, 0, 4)
        Array.Copy(BitConverter.GetBytes(keyframeFrequency), 0, buffer, 4, 2)
        output.Write(buffer, 0, buffer.Length)

    End Sub

    Private Sub OutputFrameCount()

        output.Position = 6
        Dim buffer() As Byte = BitConverter.GetBytes(currentFrame)
        output.Write(buffer, 0, buffer.Length)

    End Sub

    Private Sub OutputGlossary()

        output.Position = output.Length

        Dim buffer() As Byte = glossary.ToBytes
        output.Write(buffer, 0, buffer.Length)

    End Sub

    Private Sub OutputFrame()

        Dim buffer() As Byte = frame.ToBytes
        output.Write(buffer, 0, buffer.Length)

    End Sub

    Private Function CreateUniverse(universe As DMXUniverse) As DMXUniversePayload

        Dim payload As New DMXUniversePayload()
        Dim isKeyframe As Boolean = currentFrame Mod keyframeFrequency = 0

        Dim lastUniverse As New DMXUniverse(universe.UniverseNumber)
        For j = 0 To dmxHistory.Count - 1
            If dmxHistory(j).UniverseNumber = universe.UniverseNumber Then
                lastUniverse.UniverseNumber = dmxHistory(j).UniverseNumber
                lastUniverse.DMX = dmxHistory(j).DMX
            End If
        Next

        payload.SetUniverse(universe, lastUniverse, isKeyframe)

        Return payload

    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                output.Close()
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
