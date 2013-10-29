using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ICCEdit
{
    class IccProfileWriter
    {
        private Stream st;
        private BinaryWriter bw;

        public IccProfileWriter(Stream outputStream)
        {
            if (!outputStream.CanWrite) {
                throw new InvalidOperationException("Stream does not support output.");
            }
            st = outputStream;
            bw = new BinaryWriter(st);
        }

        public void WriteProfile(IccProfile profile)
        {
            // Write the header
            profile.UpdateSizeField();
            st.Write(profile.Header.Bytes, 0, 128);

            // Write the tag table
            int numTags = profile.Tags.Count;
            uint dataOffset = 132 + 12 * (uint)numTags;
            bw.Write(BigEndian.SwitchEndianness(numTags));
            foreach (IccTag tag in profile.Tags) {
                st.Write(Encoding.ASCII.GetBytes(tag.TagSignature), 0, 4);
                bw.Write(BigEndian.SwitchEndianness(dataOffset));
                bw.Write(BigEndian.SwitchEndianness(tag.DataArray.Length));
                dataOffset += (uint)tag.DataArray.Length;
            }

            // Write the tags
            foreach (IccTag tag in profile.Tags) {
                bw.Write(tag.DataArray);
            }
        }
    }
}
