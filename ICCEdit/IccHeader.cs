using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICCEdit
{
    class IccHeader
    {
        private byte[] bytes;

        public const int ProfileSignature = 0x61637370; // 'acsp'

        public IccHeader()
        {
            bytes = new byte[128];
            BitConverter.GetBytes(ProfileSignature).Reverse().CopyTo(bytes, 36);
        }

        public IccHeader(byte[] data)
        {
            if (data.Length != 128) {
                throw new ArgumentException("Array must contain 128 bytes.");
            } else {
                bytes = data;
            }
        }

        public byte[] Bytes
        {
            get { return bytes; }
        }

        public void StoreUInt32BE(int index, uint value)
        {
            byte[] valBytes = BitConverter.GetBytes(value);
            valBytes.Reverse();
            valBytes.CopyTo(bytes, index);
        }

        public uint GetUInt32BE(int index)
        {
            byte[] valBytes = new byte[4];
            Array.Copy(bytes, index, valBytes, 0, 4);
            valBytes.Reverse();
            return BitConverter.ToUInt32(valBytes, 0);
        }

        public void StoreInt32BE(int index, int value)
        {
            byte[] valBytes = BitConverter.GetBytes(value);
            valBytes.Reverse();
            valBytes.CopyTo(bytes, index);
        }

        public int GetInt32BE(int index)
        {
            byte[] valBytes = new byte[4];
            Array.Copy(bytes, index, valBytes, 0, 4);
            valBytes.Reverse();
            return BitConverter.ToInt32(valBytes, 0);
        }

        public uint ProfileSize
        {
            get { return GetUInt32BE(0); }
            set { StoreUInt32BE(0, value); }
        }

        #region Code to be used in a later version

        /*public struct ProfileVersion
        {
            public ProfileVersion(byte major, byte minor)
            {
                this.major = major;
                this.minor = minor;
            }

            public byte major, minor;

            public int Encoded
            {
                get
                {
                    return (major << 24) | (minor << 16);
                }
            }
        }

        public enum DeviceClassConstants
        {
            InputDevice   = 0x73636E72, // 'scnr'
            DisplayDevice = 0x6D6E7472, // 'mntr'
            OutputDevice  = 0x70727472, // 'prtr'
            DeviceLink    = 0x6C696E6B, // 'link'
            ColorSpace    = 0x73706163, // 'spac'
            Abstract      = 0x61627374, // 'abst'
            NamedColor    = 0x6E6D636C, // 'nmcl'
        }

        public int CMMType;
        public ProfileVersion Version;
        public DeviceClassConstants DeviceClass;*/

        #endregion
    }
}
