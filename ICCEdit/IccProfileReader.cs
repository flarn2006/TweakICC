using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ICCEdit
{
    class IccProfileReader
    {
        private Stream st;
        private BinaryReader br;
        
        public IccProfileReader(Stream inputStream)
        {
            if (!inputStream.CanRead) {
                throw new InvalidOperationException("Stream does not support input.");
            }
            st = inputStream;
            br = new BinaryReader(st);
        }

        public IccTag[] ReadTagTable()
        {
            st.Position = 128;
            int tagCount = br.ReadInt32BE();
            IccTag[] tags = new IccTag[tagCount];
            uint[] offsets = new uint[tagCount];

            for (int i=0; i<tagCount; i++) {
                string sig = Encoding.ASCII.GetString(br.ReadBytes(4));
                offsets[i] = br.ReadUInt32BE();
                uint size = br.ReadUInt32BE();
                tags[i] = new IccTag(sig, size);
            }

            for (int i = 0; i < tagCount; i++) {
                st.Position = offsets[i];
                st.Read(tags[i].DataArray, 0, tags[i].DataArray.Length);
            }

            return tags;
        }

        public IccProfile ReadProfile()
        {
            // Check to see if this is in fact an ICC profile
            st.Position += 36;
            try {
                if (br.ReadInt32BE() != IccHeader.ProfileSignature) {
                    // not really EOF here, but jump to the catch block
                    throw new EndOfStreamException();
                }
            } catch (EndOfStreamException) {
                throw new InvalidDataException("This is not a valid ICC profile.");
            }

            // If we get here, it's most likely valid.
            st.Position -= 40;
            IccProfile profile = new IccProfile();
            st.Read(profile.Header.Bytes, 0, 128);
            foreach (IccTag tag in ReadTagTable()) {
                profile.Tags.Add(tag);
            }
            return profile;
        }
    }
}
