Namespace Codec.InternalPayloads

    Friend MustInherit Class InternalPayload

        Private _payloadType As PayloadType

        Public MustOverride ReadOnly Property PayloadType As PayloadType

        Public MustOverride Function ToBytes() As Byte()

    End Class

    Friend Enum PayloadType

        DMXUniverse = 1
        MidiShowControl = 2
        LinearTimeCode = 3

    End Enum

End Namespace