using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BigNumberLibrary
{
    /// <summary>
    /// Class for creating, manipulating a serie of bits.
    /// This class is used as base class for other numeric types
    /// </summary>
    public class BitArray
    {
        public enum BitMode
        {
            Standard = 0,
            ComplimentTwo = 2
        }

        /// <summary>Represent the amount of bits in array</summary>
        public uint TotalBits { get; protected set; }
        /// <summary>An array of bytes which represents the bits serie</summary>
        public byte[] BytesArray { get; protected set; }
        /// <summary>Represent the amount of bytes needed to store the bits serie</summary>
        public uint TotalBytes { get; protected set; }

        /// <summary> 
        /// <para>Represent all possible byte values which contain only one bit 1.
        /// Indexing is equivalent to placing a bit 1 to that index in array.</para>
        /// <example>e.g:
        /// <c>ByteWithBitOneAt[0]</c> return 1000 0000
        /// </example> 
        /// </summary>
        static public readonly List<byte> ByteWithBitOneAt = new List<byte>() { 128, 64, 32, 16, 8, 4, 2, 1 };

        /// <summary>
        /// Create an array with all bit set to 0
        /// </summary>
        /// <param name="totalbits">Represent the amount of bits in array</param>
        /// <returns>BitArray instance with all bit set to 0</returns>
        protected static byte[] AllZero(uint totalbits)
        {
            BitArray result = new BitArray(totalbits);
            byte min = 0x00; // 0000 0000
            for (uint i = 0; i < result.TotalBytes; i++)
            {
                result.BytesArray[i] = min;
            }
            return result.BytesArray;
        }

        /// <summary>
        /// Create an array with all bit set to 1
        /// </summary>
        /// <param name="totalbits">Represent the amount of bits in array</param>
        /// <returns>BitArray instance with all bit set to 1</returns>
        protected static byte[] AllOne(uint totalbits)
        {
            BitArray result = new BitArray(totalbits);
            byte max = 0xff; // 1111 1111
            for (uint i = 0; i < result.TotalBytes; i++)
            {
                result.BytesArray[i] = max;
            }
            return result.BytesArray;
        }

        /// <summary>
        /// Class for creating, manipulating a serie of bits.
        /// This class is used as base class for other numeric types
        /// </summary>
        /// <param name="totalbits">Represent the amount of bits in array</param>
        public BitArray(uint totalbits)
        {
            TotalBits = totalbits;

            //Add one extra byte if the amount of bits is not a multiple of 8
            TotalBytes = (TotalBits % 8u == 0) ? TotalBits / 8u : TotalBits / 8u + 1;

            BytesArray = (byte[])System.Array.CreateInstance(typeof(byte), TotalBytes);
        }

        /// <summary>
        /// Set byte at <paramref name="i"/> postion to the value of <paramref name="value"/>.
        /// Note that for easier when doing arithmetic, <paramref name="reverse"/> is set to <see langword="true"/> by default
        /// </summary>
        /// <param name="i">position of byte to be set. Start at 0</param>
        /// <param name="value">new value</param>
        /// <param name="reverse">Indicate that indexing starts from the right instead of left</param>
        public void SetByte(uint i, byte value, bool reverse = true)
        {
            if(!reverse)
                BytesArray[i % TotalBytes] = value;
            else
                BytesArray[TotalBytes - 1 - i % TotalBytes] = value;
        }

        /// <summary>
        /// Get byte at <paramref name="i"/> postion.
        /// Note that for easier when doing arithmetic, <paramref name="reverse"/> is set to <see langword="true"/> by default
        /// </summary>
        /// <param name="i">position of requested byte</param>
        /// <param name="reverse">Indicate that indexing starts from the right instead of left</param>
        /// <returns>byte value at position <paramref name="i"/></returns>
        public byte GetByte(uint i, bool reverse = true)
        {
            return (!reverse ? BytesArray[i % TotalBytes] : BytesArray[TotalBytes - 1 - i % TotalBytes]);
        }

        /// <summary>
        /// Extract requested bit at position <paramref name="i"/>.
        /// Note that for easier when doing arithmetic, <paramref name="reverse"/> is set to <see langword="true"/> by default
        /// </summary>
        /// <param name="i">position of requested bit</param>
        /// <param name="reverse">Indicate that indexing starts from the right instead of left</param>
        /// <returns>True if requested bit is 1, false otherwise</returns>
        public bool GetBit(uint i,bool reverse = true)
        {
            // Guarantee i is not out of range
            i %= TotalBits;
            uint byte_index = i / 8u;
            // Workout the byte containing requested bit
            byte _byte = GetByte(byte_index, reverse);

            // Find the corresponding byte in <ByteWithBitOneAt> list
            // and perform & operator to extract that bit
            // Example if we want to check the fourth bit from the right in 0110 0001,
            //                 v           v                                     ^
            // we perform 0110 0001 & 0000 1000 to extract that bit 0 from the serie
            // Note that if reverse is set, indexing starts from the right instead of left
            int _index = (reverse ? ByteWithBitOneAt.Count - 1 - (int)(i % 8u) : (int)(i % 8u));
            int bit = _byte & ByteWithBitOneAt[_index];

            return bit != 0;
        }

        /// <summary>
        /// Set bit at position <c><paramref name="i"/> to <paramref name="state"/></c>
        /// </summary>
        /// <param name="i">The bit to be set</param>
        /// <param name="state">true = set to 1, otherwise 0</param>
        /// <param name="reverse">Indicate that indexing starts from the right instead of left</param>
        public void SetBit(uint i,bool state, bool reverse = true)
        {
            if (state)
                TurnOnBit(i, reverse);
            else
                TurnOffBit(i, reverse);
        }

        /// <summary>
        /// Set the bit at position <paramref name="i"/> to 0
        /// </summary>
        /// <param name="i">Position of bit to be turn off</param>
        /// <param name="reverse">Indicate that indexing starts from the right instead of left</param>
        public void TurnOffBit(uint i, bool reverse = true)
        {
            i %= TotalBits;
            uint byte_index = i / 8u;

            int _byte = GetByte(byte_index, reverse);

            // if reverse is set, indexing starts from the right instead of left
            int _index = (reverse ? ByteWithBitOneAt.Count - 1 - (int)(i % 8u) : (int)(i % 8u));

            // Perform & with corresponding byte       v
            // Example: To turn of the fifth bit of 0011 1100
            //        v           v              v             v              v
            // Do (0011 1100 & 1110 1111) or (0011 1100 & ~(0001 0000)) => 0010 1100
            int new_byte = _byte & ~ByteWithBitOneAt[_index];

            SetByte(byte_index, (byte)new_byte, reverse);
        }

        /// <summary>
        /// Set the bit at position <paramref name="i"/> to 1
        /// </summary>
        /// <param name="i">Position of bit to be turn on</param>
        /// <param name="reverse">Indicate that indexing starts from the right instead of left</param>
        public void TurnOnBit(uint i, bool reverse = true)
        {
            i %= TotalBits;
            uint byte_index = i / 8u;

            int _byte = GetByte(byte_index, reverse);

            // if reverse is set, indexing starts from the right instead of left
            int _index = (reverse ? ByteWithBitOneAt.Count - 1 - (int)(i % 8u) : (int)(i % 8u));

            // Perform | with corresponding byte       v
            // Example: To turn on the fifth bit of 0010 1100                 v
            //        v           v             v
            // Do (0010 1100 | 0001 0000) => 0010 1100
            int new_byte = _byte | ByteWithBitOneAt[_index];

            SetByte(byte_index, (byte)new_byte, reverse);
        }
    }


}
