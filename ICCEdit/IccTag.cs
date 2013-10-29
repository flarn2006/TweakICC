using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICCEdit
{
    class IccTag
    {
        private string sig;
        private byte[] data;

        public IccTag(string tagSignature, byte[] dataArray)
        {
            SetSignature(tagSignature);
            data = dataArray;
        }

        public IccTag(string tagSignature, uint dataLength)
        {
            if (dataLength < 0) {
                throw new ArgumentOutOfRangeException("dataLength can't be negative.");
            }
            SetSignature(tagSignature);
            data = new byte[dataLength];
        }

        private void SetSignature(string tagSignature)
        {
            if (tagSignature.Length == 4) {
                sig = tagSignature;
            } else {
                throw new ArgumentException("Tag signature must contain exactly 4 characters.");
            }
        }

        public string TagSignature
        {
            get { return sig; }
            set { SetSignature(value); }
        }

        public byte[] DataArray
        {
            get { return data; }
            set { data = value; }
        }

        public void Resize(int newSize)
        {
            byte[] newArray = new byte[newSize];
            data.CopyTo(newArray, 0);
            data = newArray;
        }
    }
}
