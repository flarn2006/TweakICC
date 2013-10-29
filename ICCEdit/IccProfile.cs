using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICCEdit
{
    class IccProfile
    {
        private IccHeader header;
        private List<IccTag> tags;

        public IccProfile()
        {
            header = new IccHeader();
            tags = new List<IccTag>();
        }

        public IccHeader Header
        {
            get { return header; }
        }

        public List<IccTag> Tags
        {
            get { return tags; }
        }

        public uint UpdateSizeField()
        {
            uint size = 132; // header + tag count field
            foreach (IccTag tag in tags) {
                size += 12; // signature + offset + size
                size += (uint)tag.DataArray.Length;
            }

            header.ProfileSize = size;
            return size;
        }
    }
}
