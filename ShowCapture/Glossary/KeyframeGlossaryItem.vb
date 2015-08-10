Public Class KeyframeGlossaryItem

    Private _frameNumber As Integer
    Public ReadOnly Property FrameNumber
        Get
            Return _frameNumber
        End Get
    End Property

    Private _byteIndex As Long
    Public ReadOnly Property ByteIndex
        Get
            Return _byteIndex
        End Get
    End Property

    Public Const ByteSize = 12

    Public Sub New(frameNumber, byteIndex)
        _frameNumber = frameNumber
        _byteIndex = byteIndex
    End Sub

    Public Sub New(bytes() As Byte)

        _frameNumber = BitConverter.ToInt32(bytes, 0)
        _byteIndex = BitConverter.ToInt64(bytes, 4)

    End Sub

    Public Function ToBytes() As Byte()

        Dim buffer(ByteSize - 1) As Byte

        Array.Copy(BitConverter.GetBytes(FrameNumber), 0, buffer, 0, 4)
        Array.Copy(BitConverter.GetBytes(ByteIndex), 0, buffer, 4, 8)

        Return buffer

    End Function

End Class
