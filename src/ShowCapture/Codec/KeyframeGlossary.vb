Imports System.IO

Namespace Codec

    Friend Class KeyframeGlossary

        Public Items As New List(Of KeyframeGlossaryItem)


        Public Shared Function ReadFromStream(stream As FileStream) As KeyframeGlossary

            Dim keyframeCount As Integer

            Dim footerLength As Integer = 4

            'get the number of keyframes in the glossary
            Dim keyframeCountBuffer(footerLength - 1) As Byte
            stream.Position = stream.Length - footerLength
            stream.Read(keyframeCountBuffer, 0, footerLength)

            keyframeCount = BitConverter.ToInt32(keyframeCountBuffer, 0)

            'find glossary dimensions
            Dim glossarySize As Integer = keyframeCount * KeyframeGlossaryItem.ByteSize + footerLength
            Dim glossaryStartIndex As Long = stream.Length - glossarySize

            Dim glossary As New KeyframeGlossary

            'read glossary entries into the glossary
            stream.Position = glossaryStartIndex

            For i = 0 To keyframeCount - 1

                Dim itemBuffer(KeyframeGlossaryItem.ByteSize - 1) As Byte

                stream.Read(itemBuffer, 0, KeyframeGlossaryItem.ByteSize)

                Dim glossaryItem As New KeyframeGlossaryItem(itemBuffer)

                glossary.Items.Add(glossaryItem)

            Next

            Return glossary

        End Function

        Public Function ToBytes()

            Dim glossaryLength As Integer = Items.Count * KeyframeGlossaryItem.ByteSize
            Dim footerLength As Integer = 4
            Dim buffer(glossaryLength + footerLength - 1) As Byte

            For i = 0 To Items.Count - 1
                Dim index As Integer = i * KeyframeGlossaryItem.ByteSize
                Array.Copy(Items(i).ToBytes, 0, buffer, index, KeyframeGlossaryItem.ByteSize)
            Next

            Array.Copy(BitConverter.GetBytes(Items.Count), 0, buffer, buffer.Length - footerLength, footerLength)

            Return buffer

        End Function

    End Class

End Namespace