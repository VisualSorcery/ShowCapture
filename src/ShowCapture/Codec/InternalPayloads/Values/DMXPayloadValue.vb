Namespace Codec.InternalPayloads.Values

    Friend Class DMXPayloadValue

        Public Address As Short
        Public Value As Byte

        Public Const ByteSize = 3

        Public Sub New(address As Short, value As Byte)
            Me.Address = address
            Me.Value = value
        End Sub

        Public Function ToBytes() As Byte()

            Dim buffer(ByteSize - 1) As Byte
            Array.Copy(BitConverter.GetBytes(Address), 0, buffer, 0, 2)
            buffer(2) = Value

            Return buffer

        End Function

    End Class

End Namespace