Imports ShowCapture.Codec.InternalPayloads.Values
Imports ShowCapture.Payloads

Namespace Codec.InternalPayloads

    Friend Class DMXUniversePayload
        Inherits InternalPayload

        'Overridden properties
        Public Overrides ReadOnly Property PayloadType As PayloadType
            Get
                Return PayloadType.DMXUniverse
            End Get
        End Property

        Private _universe As Integer
        Public ReadOnly Property Universe As Integer
            Get
                Return _universe
            End Get
        End Property


        Public Items As New List(Of DMXPayloadValue)

        Public Sub New()

        End Sub

        Public Sub New(buffer() As Byte)
            ReadFromBytes(buffer)
        End Sub

        Private Sub ReadFromBytes(buffer() As Byte)

            _universe = BitConverter.ToInt16(buffer, 0)

            For i = 2 To buffer.Length - 1 Step 3

                Dim address As Short = BitConverter.ToInt16(buffer, i)
                Dim value As Byte = buffer(i + 2)

                Dim dmxValue As New DMXPayloadValue(address, value)

                Items.Add(dmxValue)

            Next

        End Sub

        Public Sub SetUniverse(inputUniverse As DMXUniverse, lastUniverse As DMXUniverse, isKeyframe As Boolean)

            _universe = inputUniverse.UniverseNumber

            For i = 0 To inputUniverse.DMX.Count - 1

                If inputUniverse.DMX(i) <> lastUniverse.DMX(i) Or isKeyframe Then

                    Dim value As New DMXPayloadValue(i, inputUniverse.DMX(i))

                    Items.Add(value)

                End If

            Next

        End Sub

        Public Overrides Function ToBytes() As Byte()

            Dim headerSize As Integer = 7
            Dim DMXSize As Integer = Items.Count * DMXPayloadValue.ByteSize
            Dim bufferSize As Integer = headerSize + DMXSize
            Dim buffer(bufferSize - 1) As Byte

            'header
            'PayloadType
            Array.Copy(BitConverter.GetBytes(PayloadType), 0, buffer, 0, 1)
            'PayloadLength
            Array.Copy(BitConverter.GetBytes(bufferSize), 0, buffer, 1, 4)
            'Universe number
            Array.Copy(BitConverter.GetBytes(Universe), 0, buffer, 5, 2)

            'values
            For i = 0 To Items.Count - 1

                Dim index As Integer = (i * DMXPayloadValue.ByteSize) + headerSize
                Array.Copy(Items(i).ToBytes, 0, buffer, index, DMXPayloadValue.ByteSize)

            Next

            Return buffer

        End Function

    End Class

End Namespace