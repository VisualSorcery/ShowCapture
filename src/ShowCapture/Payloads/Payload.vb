Public MustInherit Class Payload

    Private _payloadType As PayloadType

    Public MustOverride ReadOnly Property PayloadType As PayloadType

    Public MustOverride Function ToBytes() As Byte()

End Class

Public Enum PayloadType

    DMXUniverse = 1
    MidiShowControl = 2
    LinearTimeCode = 3

End Enum
