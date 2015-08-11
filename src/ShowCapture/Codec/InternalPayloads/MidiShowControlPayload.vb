Imports ShowCapture.Payloads

Namespace Codec.InternalPayloads

    Friend Class MidiShowControlPayload
        Inherits InternalPayload

        'Overridden properties
        Public Overrides ReadOnly Property PayloadType As PayloadType
            Get
                Return PayloadType.MidiShowControl
            End Get
        End Property

        Private _command As MidiShowControlCommand
        Public ReadOnly Property Command As MidiShowControlCommand
            Get
                Return _command
            End Get
        End Property

        Public Sub New()

        End Sub

        Public Sub New(buffer() As Byte)

            ReadFromBytes(buffer)

        End Sub

        Private Sub ReadFromBytes(buffer() As Byte)

            _command = New MidiShowControlCommand

            Command.DeviceID = buffer(0)
            Command.CommandFormat = buffer(1)
            Command.Command = buffer(2)

            Dim dataLength As Integer = buffer.Length - 3
            Dim dataBuffer(dataLength - 1) As Byte

            Array.Copy(buffer, 3, dataBuffer, 0, dataLength)

            Command.Data = dataBuffer

        End Sub

        Public Function GetContainer() As MidiShowControlCommand

            Dim msc As New MidiShowControlCommand

            msc.DeviceID = Command.DeviceID
            msc.CommandFormat = Command.CommandFormat
            msc.Command = Command.Command
            msc.Data = Command.Data

            Return msc

        End Function

        Public Sub SetCommand(command As MidiShowControlCommand)

            _command = command

        End Sub

        Public Overrides Function ToBytes() As Byte()

            Dim headerSize As Integer = 8
            Dim dataSize = Command.Data.Length
            Dim bufferSize As Integer = headerSize + dataSize
            Dim buffer(bufferSize - 1) As Byte

            'header
            'PayloadType
            Array.Copy(BitConverter.GetBytes(PayloadType), 0, buffer, 0, 1)
            'PayloadLength
            Array.Copy(BitConverter.GetBytes(bufferSize), 0, buffer, 1, 4)

            buffer(5) = Command.DeviceID
            buffer(6) = Command.CommandFormat
            buffer(7) = Command.Command

            'Data
            Array.Copy(Command.Data, 0, buffer, 8, dataSize)

            Return buffer

        End Function

    End Class

End Namespace