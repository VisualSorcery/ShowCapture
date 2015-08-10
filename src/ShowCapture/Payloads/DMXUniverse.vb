Imports System.Text

Namespace Payloads

    Public Class DMXUniverse
        Inherits Payload

        Public UniverseNumber As Short
        Public DMX(511) As Byte

        Public Sub New(universe As Short)
            UniverseNumber = universe
        End Sub

        Public Overrides Function ToString() As String

            Dim output As New StringBuilder()
            output.Append("Universe: ")
            output.Append(UniverseNumber)
            output.Append(" ")

            For i = 0 To DMX.Length - 1
                output.Append(i + 1)
                output.Append(" ")
                output.Append(DMX(i))
                output.Append(" ")
            Next

            Return output.ToString
        End Function

    End Class

End Namespace