using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Windows.Forms;

namespace ICCEdit
{
    class ArrayByteProvider : IByteProvider
    {
        private byte[] array;

        public ArrayByteProvider(byte[] array)
        {
            this.array = array;
        }

        public byte[] ByteArray
        {
            get { return array; }
            set { array = value; OnLengthChanged(); OnChanged(); }
        }

        public void ApplyChanges() { }

        public void DeleteBytes(long index, long length)
        {
            throw new NotSupportedException("Can't delete bytes from an array.");
        }

        public bool HasChanges()
        {
            return false;
        }

        public void InsertBytes(long index, byte[] bs)
        {
            throw new NotSupportedException("Can't insert bytes in an array.");
        }

        public byte ReadByte(long index)
        {
            return array[index];
        }

        public bool SupportsDeleteBytes()
        {
            return false;
        }

        public bool SupportsInsertBytes()
        {
            return false;
        }

        public bool SupportsWriteByte()
        {
            return true;
        }

        public void WriteByte(long index, byte value)
        {
            array[index] = value;
            OnChanged();
        }

        public long Length
        {
            get { return array.Length; }
        }

        public event EventHandler Changed;
        protected virtual void OnChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        public event EventHandler LengthChanged;
        protected virtual void OnLengthChanged()
        {
            if (LengthChanged != null)
                LengthChanged(this, EventArgs.Empty);
        }
    }
}
