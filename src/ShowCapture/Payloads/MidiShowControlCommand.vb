Namespace Payloads

    Public Class MidiShowControl
        Inherits Payload

        Public DeviceID As Byte
        Public CommandFormat As Byte
        Public Command As Byte
        Public Data As Byte()

    End Class

End Namespace