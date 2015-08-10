Imports ShowCapture.Payloads

Namespace Codec.InternalPayloads

    Friend Class LinearTimeCodePayload
        Inherits InternalPayload

        'Overridden properties
        Public Overrides ReadOnly Property PayloadType As PayloadType
            Get
                Return PayloadType.LinearTimeCode
            End Get
        End Property

        Private _value As LinearTimeCode
        Public ReadOnly Property Value As LinearTimeCode
            Get
                Return _value
            End Get
        End Property

        Public Sub New()

        End Sub

        Public Sub New(buffer() As Byte)

            ReadFromBytes(buffer)

        End Sub

        Public Function GetContainer() As LinearTimeCode

            Dim ltc As New LinearTimeCode

            ltc.FrameUnits = Value.FrameUnits
            ltc.FrameTens = Value.FrameTens
            ltc.SecondUnits = Value.SecondUnits
            ltc.SecondTens = Value.SecondTens
            ltc.MinuteUnits = Value.MinuteUnits
            ltc.MinuteTens = Value.MinuteTens
            ltc.HourUnits = Value.HourUnits
            ltc.HourTens = Value.HourTens
            ltc.SyncWord = Value.SyncWord

            Return ltc

        End Function

        Private Sub ReadFromBytes(buffer() As Byte)

            _value = New LinearTimeCode

            Value.FrameUnits = buffer(0)
            Value.FrameTens = buffer(1)
            Value.SecondUnits = buffer(2)
            Value.SecondTens = buffer(3)
            Value.MinuteUnits = buffer(4)
            Value.MinuteTens = buffer(5)
            Value.HourUnits = buffer(6)
            Value.HourTens = buffer(7)
            Value.SyncWord = BitConverter.ToInt16(buffer, 8)


        End Sub

        Public Sub SetValue(value As LinearTimeCode)

            _value = value

        End Sub

        Public Overrides Function ToBytes() As Byte()

            Dim headerSize As Integer = 15
            Dim buffer(headerSize - 1) As Byte

            'header
            'PayloadType
            Array.Copy(BitConverter.GetBytes(PayloadType), 0, buffer, 0, 1)
            'PayloadLength
            Array.Copy(BitConverter.GetBytes(headerSize), 0, buffer, 1, 4)
            'Frame Unit
            buffer(5) = Value.FrameUnits
            buffer(6) = Value.FrameTens
            buffer(7) = Value.SecondUnits
            buffer(8) = Value.SecondTens
            buffer(9) = Value.MinuteUnits
            buffer(10) = Value.MinuteTens
            buffer(11) = Value.HourUnits
            buffer(12) = Value.HourTens
            Array.Copy(BitConverter.GetBytes(Value.SyncWord), 0, buffer, 13, 2)

            Return buffer

        End Function

    End Class

End Namespace