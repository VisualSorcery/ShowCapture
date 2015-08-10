Imports ShowCapture.Payloads

Public Class Frame

    Private _payloads As List(Of Payload)

    Public ReadOnly Property Payloads As List(Of Payload)
        Get
            Return _payloads
        End Get
    End Property

    Private _frameNumber As Integer
    Public ReadOnly Property FrameNumber As Integer
        Get
            Return _frameNumber
        End Get
    End Property

    Public Sub New(frameNumber As Integer, payloads As List(Of Payload))
        _frameNumber = frameNumber
        _payloads = payloads
    End Sub


End Class
