namespace MessagePack.Internal
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct GuidBits
    {
        [FieldOffset(0)]
        public readonly Guid Value;

        [FieldOffset(0)]
        public readonly byte Byte0;
        [FieldOffset(1)]
        public readonly byte Byte1;
        [FieldOffset(2)]
        public readonly byte Byte2;
        [FieldOffset(3)]
        public readonly byte Byte3;
        [FieldOffset(4)]
        public readonly byte Byte4;
        [FieldOffset(5)]
        public readonly byte Byte5;
        [FieldOffset(6)]
        public readonly byte Byte6;
        [FieldOffset(7)]
        public readonly byte Byte7;
        [FieldOffset(8)]
        public readonly byte Byte8;
        [FieldOffset(9)]
        public readonly byte Byte9;
        [FieldOffset(10)]
        public readonly byte Byte10;
        [FieldOffset(11)]
        public readonly byte Byte11;
        [FieldOffset(12)]
        public readonly byte Byte12;
        [FieldOffset(13)]
        public readonly byte Byte13;
        [FieldOffset(14)]
        public readonly byte Byte14;
        [FieldOffset(15)]
        public readonly byte Byte15;

        // string.Join(", ", Enumerable.Range(0, 256).Select(x => (int)BitConverter.ToString(new byte[] { (byte)x }).ToLower()[0]))
        static byte[] byteToHexStringHigh = new byte[256] { 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 51, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 52, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 53, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 54, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 97, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 98, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102 };

        // string.Join(", ", Enumerable.Range(0, 256).Select(x => (int)BitConverter.ToString(new byte[] { (byte)x }).ToLower()[1]))
        static byte[] byteToHexStringLow = new byte[256] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102 };

        public GuidBits(ref Guid value)
        {
            this = default(GuidBits);
            this.Value = value;
        }

        // 4-pattern, lower/upper and '-' or no
        public GuidBits(ReadOnlySpan<byte> utf8string)
        {
            this = default(GuidBits);

            ref byte pinnableAddr = ref MemoryMarshal.GetReference(utf8string);
            var offset = (IntPtr)0;

            // 32
            if (utf8string.Length == 32)
            {
                if (BitConverter.IsLittleEndian)
                {
                    this.Byte0 = Parse(ref pinnableAddr, offset + 6);
                    this.Byte1 = Parse(ref pinnableAddr, offset + 4);
                    this.Byte2 = Parse(ref pinnableAddr, offset + 2);
                    this.Byte3 = Parse(ref pinnableAddr, offset + 0);

                    this.Byte4 = Parse(ref pinnableAddr, offset + 10);
                    this.Byte5 = Parse(ref pinnableAddr, offset + 8);

                    this.Byte6 = Parse(ref pinnableAddr, offset + 14);
                    this.Byte7 = Parse(ref pinnableAddr, offset + 12);
                }
                else
                {
                    this.Byte0 = Parse(ref pinnableAddr, offset + 0);
                    this.Byte1 = Parse(ref pinnableAddr, offset + 2);
                    this.Byte2 = Parse(ref pinnableAddr, offset + 4);
                    this.Byte3 = Parse(ref pinnableAddr, offset + 6);

                    this.Byte4 = Parse(ref pinnableAddr, offset + 8);
                    this.Byte5 = Parse(ref pinnableAddr, offset + 10);

                    this.Byte6 = Parse(ref pinnableAddr, offset + 12);
                    this.Byte7 = Parse(ref pinnableAddr, offset + 14);
                }
                this.Byte8 = Parse(ref pinnableAddr, offset + 16);
                this.Byte9 = Parse(ref pinnableAddr, offset + 18);

                this.Byte10 = Parse(ref pinnableAddr, offset + 20);
                this.Byte11 = Parse(ref pinnableAddr, offset + 22);
                this.Byte12 = Parse(ref pinnableAddr, offset + 24);
                this.Byte13 = Parse(ref pinnableAddr, offset + 26);
                this.Byte14 = Parse(ref pinnableAddr, offset + 28);
                this.Byte15 = Parse(ref pinnableAddr, offset + 30);
                return;
            }
            else if (utf8string.Length == 36)
            {
                // '-' => 45
                if (BitConverter.IsLittleEndian)
                {
                    this.Byte0 = Parse(ref pinnableAddr, offset + 6);
                    this.Byte1 = Parse(ref pinnableAddr, offset + 4);
                    this.Byte2 = Parse(ref pinnableAddr, offset + 2);
                    this.Byte3 = Parse(ref pinnableAddr, offset + 0);

                    if (Unsafe.AddByteOffset(ref pinnableAddr, offset + 8) != '-') goto ERROR;

                    this.Byte4 = Parse(ref pinnableAddr, offset + 11);
                    this.Byte5 = Parse(ref pinnableAddr, offset + 9);

                    if (Unsafe.AddByteOffset(ref pinnableAddr, offset + 13) != '-') goto ERROR;

                    this.Byte6 = Parse(ref pinnableAddr, offset + 16);
                    this.Byte7 = Parse(ref pinnableAddr, offset + 14);
                }
                else
                {
                    this.Byte0 = Parse(ref pinnableAddr, offset + 0);
                    this.Byte1 = Parse(ref pinnableAddr, offset + 2);
                    this.Byte2 = Parse(ref pinnableAddr, offset + 4);
                    this.Byte3 = Parse(ref pinnableAddr, offset + 6);

                    if (Unsafe.AddByteOffset(ref pinnableAddr, offset + 8) != '-') goto ERROR;

                    this.Byte4 = Parse(ref pinnableAddr, offset + 9);
                    this.Byte5 = Parse(ref pinnableAddr, offset + 11);

                    if (Unsafe.AddByteOffset(ref pinnableAddr, offset + 13) != '-') goto ERROR;

                    this.Byte6 = Parse(ref pinnableAddr, offset + 14);
                    this.Byte7 = Parse(ref pinnableAddr, offset + 16);
                }

                if (Unsafe.AddByteOffset(ref pinnableAddr, offset + 18) != '-') goto ERROR;

                this.Byte8 = Parse(ref pinnableAddr, offset + 19);
                this.Byte9 = Parse(ref pinnableAddr, offset + 21);

                if (Unsafe.AddByteOffset(ref pinnableAddr, offset + 23) != '-') goto ERROR;

                this.Byte10 = Parse(ref pinnableAddr, offset + 24);
                this.Byte11 = Parse(ref pinnableAddr, offset + 26);
                this.Byte12 = Parse(ref pinnableAddr, offset + 28);
                this.Byte13 = Parse(ref pinnableAddr, offset + 30);
                this.Byte14 = Parse(ref pinnableAddr, offset + 32);
                this.Byte15 = Parse(ref pinnableAddr, offset + 34);
                return;
            }

        ERROR:
            ThrowHelper.ThrowArgumentException_Guid_Pattern(); ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte Parse(ref byte pinnableAddr, IntPtr highOffset)
        {
            return unchecked((byte)(SwitchParse(Unsafe.AddByteOffset(ref pinnableAddr, highOffset)) * 16 + SwitchParse(Unsafe.AddByteOffset(ref pinnableAddr, highOffset + 1))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static byte SwitchParse(byte b)
        {
            // '0'(48) ~ '9'(57) => -48
            // 'A'(65) ~ 'F'(70) => -55
            // 'a'(97) ~ 'f'(102) => -87
            switch (b)
            {
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                    return unchecked((byte)((b - 48)));
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                    return unchecked((byte)((b - 55)));
                case 97:
                case 98:
                case 99:
                case 100:
                case 101:
                case 102:
                    return unchecked((byte)((b - 87)));
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 78:
                case 79:
                case 80:
                case 81:
                case 82:
                case 83:
                case 84:
                case 85:
                case 86:
                case 87:
                case 88:
                case 89:
                case 90:
                case 91:
                case 92:
                case 93:
                case 94:
                case 95:
                case 96:
                default:
                    ThrowHelper.ThrowArgumentException_Guid_Pattern(); return default;
            }
        }

        // 4(x2) - 2(x2) - 2(x2) - 2(x2) - 6(x2)
        public void Write(ref byte pinnableAddr, ref int idx)
        {
            IntPtr offset = (IntPtr)idx;
            if (BitConverter.IsLittleEndian)
            {
                // int(_a)
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 6) = byteToHexStringHigh[Byte0];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 7) = byteToHexStringLow[Byte0];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 4) = byteToHexStringHigh[Byte1];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 5) = byteToHexStringLow[Byte1];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 2) = byteToHexStringHigh[Byte2];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 3) = byteToHexStringLow[Byte2];
                Unsafe.AddByteOffset(ref pinnableAddr, offset) = byteToHexStringHigh[Byte3];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = byteToHexStringLow[Byte3];

                Unsafe.AddByteOffset(ref pinnableAddr, offset + 8) = (byte)'-';

                // short(_b)
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 11) = byteToHexStringHigh[Byte4];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 12) = byteToHexStringLow[Byte4];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 9) = byteToHexStringHigh[Byte5];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 10) = byteToHexStringLow[Byte5];

                Unsafe.AddByteOffset(ref pinnableAddr, offset + 13) = (byte)'-';

                // short(_c)
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 16) = byteToHexStringHigh[Byte6];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 17) = byteToHexStringLow[Byte6];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 14) = byteToHexStringHigh[Byte7];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 15) = byteToHexStringLow[Byte7];
            }
            else
            {
                Unsafe.AddByteOffset(ref pinnableAddr, offset) = byteToHexStringHigh[Byte0];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = byteToHexStringLow[Byte0];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 2) = byteToHexStringHigh[Byte1];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 3) = byteToHexStringLow[Byte1];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 4) = byteToHexStringHigh[Byte2];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 5) = byteToHexStringLow[Byte2];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 6) = byteToHexStringHigh[Byte3];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 7) = byteToHexStringLow[Byte3];

                Unsafe.AddByteOffset(ref pinnableAddr, offset + 8) = (byte)'-';

                Unsafe.AddByteOffset(ref pinnableAddr, offset + 9) = byteToHexStringHigh[Byte4];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 10) = byteToHexStringLow[Byte4];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 11) = byteToHexStringHigh[Byte5];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 12) = byteToHexStringLow[Byte5];

                Unsafe.AddByteOffset(ref pinnableAddr, offset + 13) = (byte)'-';

                Unsafe.AddByteOffset(ref pinnableAddr, offset + 14) = byteToHexStringHigh[Byte6];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 15) = byteToHexStringLow[Byte6];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 16) = byteToHexStringHigh[Byte7];
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 17) = byteToHexStringLow[Byte7];
            }

            Unsafe.AddByteOffset(ref pinnableAddr, offset + 18) = (byte)'-';

            Unsafe.AddByteOffset(ref pinnableAddr, offset + 19) = byteToHexStringHigh[Byte8];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 20) = byteToHexStringLow[Byte8];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 21) = byteToHexStringHigh[Byte9];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 22) = byteToHexStringLow[Byte9];

            Unsafe.AddByteOffset(ref pinnableAddr, offset + 23) = (byte)'-';

            Unsafe.AddByteOffset(ref pinnableAddr, offset + 24) = byteToHexStringHigh[Byte10];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 25) = byteToHexStringLow[Byte10];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 26) = byteToHexStringHigh[Byte11];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 27) = byteToHexStringLow[Byte11];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 28) = byteToHexStringHigh[Byte12];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 29) = byteToHexStringLow[Byte12];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 30) = byteToHexStringHigh[Byte13];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 31) = byteToHexStringLow[Byte13];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 32) = byteToHexStringHigh[Byte14];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 33) = byteToHexStringLow[Byte14];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 34) = byteToHexStringHigh[Byte15];
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 35) = byteToHexStringLow[Byte15];

            idx += 36;
        }
    }
}