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
        /// <summary> 
        /// <para>Represent all possible byte values which contain only one bit 1.
        /// Indexing is equivalent to placing a bit 1 to that index in byte.</para>
        /// <example>For example:
        /// <c>ByteWithBitOneAt[0]</c> is equal to 0000 0001
        /// </example> 
        /// </summary>
        static public readonly List<byte> ByteWithBitOneAt = new List<byte>() { 1, 2, 4, 8, 16, 32, 64, 128 };

        /// <summary>
        /// Create an instance with all bit set to 0
        /// </summary>
        /// <param name="totalbits">Represent the amount of bits in array</param>
        /// <returns>BitArray instance with all bit set to 0</returns>
        protected static BitArray AllZero(uint totalbits)
        {
            BitArray result = new BitArray(totalbits);
            byte min = 0x00; // 0000 0000
            for (uint i = 0; i < result.TotalBytes; i++)
            {
                result.BytesArray[i] = min;
            }
            return result;
        }

        /// <summary>
        /// Create an instance with all bit set to 1
        /// </summary>
        /// <param name="totalbits">Represent the amount of bits in array</param>
        /// <returns>BitArray instance with all bit set to 1</returns>
        protected static BitArray AllOne(uint totalbits)
        {
            BitArray result = new BitArray(totalbits);
            byte max = 0xff; // 1111 1111
            for (uint i = 0; i < result.TotalBytes; i++)
            {
                result.BytesArray[i] = max;
            }
            return result;
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

        /// <summary>Represent the amount of bits in array</summary>
        public uint TotalBits { get; private set; }
        /// <summary>An array of bytes which represents the bits serie</summary>
        public byte[] BytesArray { get; private set; }
        /// <summary>Represent the amount of bytes needed to store the bits serie</summary>
        public uint TotalBytes { get; private set; }

        /// <summary>
        /// Extract bit at given position. Note that for easier when doing arithmetic and bitwise operations,
        /// <para>Indexing will start from right to left, 
        /// which means last bit of last array will have the index of 0,
        /// and the bit next to it will be 1 and so on</para>
        /// </summary>
        /// <param name="i">Position of bit to be extracted</param>
        /// <returns>true if requested bit is 1, otherwise false</returns>
        public bool GetBitAt(uint i)
        {
            // Guarantee i is not out of range
            i %= TotalBits;

            // Workout the index of byte containing requested bit
            uint byte_index = i / 8u;
            // But because first bit is in the right most of array,
            // Reverse byte order to get the actual index
            byte_index = TotalBytes - 1 - byte_index;

            byte get_byte = BytesArray[TotalBytes - 1 - i / 8u];

            // Using the received index to find corresponding byte
            // and perform & operator to extract that bit
            // Example if we want to check the fourth bit from the right in 0110 0001,
            //                 v           v                                     ^
            // we perform 0110 0001 & 0000 1000 to extract that bit 0 from the serie
            int bit = get_byte & ByteWithBitOneAt[(int)(i % 8u)];

            return bit != 0;
        }

        /// <summary>
        /// Set bit at position <c><paramref name="i"/> to <paramref name="state"/></c>
        /// </summary>
        /// <param name="i">The bit to be set</param>
        /// <param name="state">true = set to 1, otherwise 0</param>
        public void SetBitAt(uint i,bool state)
        {
            if (state)
                TurnOnBit(i);
            else
                TurnOffBit(i);
        }

        /// <summary>
        /// Set the bit at position <paramref name="i"/> to 0
        /// </summary>
        /// <param name="i">Position of bit to be turn off</param>
        public void TurnOffBit(uint i)
        {
            i %= TotalBits;

            // Workout actual index of byte containing requested bit
            uint get_byte_index = TotalBytes - 1 - i / 8u;
            ref byte get_byte = ref BytesArray[get_byte_index];

            // Perform & with corresponding byte       v
            // Example: To turn of the fifth bit of 0011 1100
            //        v           v              v             v              v
            // Do (0011 1100 & 1110 1111) or (0011 1100 & ~(0001 0000)) => 0010 1100
            int new_byte = get_byte & ~ByteWithBitOneAt[(int)(i % 8u)];

            get_byte = (byte)new_byte;
        }

        /// <summary>
        /// Set the bit at position <paramref name="i"/> to 1
        /// </summary>
        /// <param name="i">Position of bit to be turn on</param>
        public void TurnOnBit(uint i)
        {
            i %= TotalBits;

            uint get_byte_index = TotalBytes - 1u - i / 8u;
            ref byte get_byte = ref BytesArray[get_byte_index];

            // Perform | with corresponding byte       v
            // Example: To turn on the fifth bit of 0010 1100                 v
            //        v           v             v
            // Do (0010 1100 | 0001 0000) => 0010 1100
            int new_byte = get_byte | ByteWithBitOneAt[(int)(i % 8u)];

            get_byte = (byte)new_byte;
        }
    }


}
