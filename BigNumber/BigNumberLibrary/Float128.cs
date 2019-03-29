using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigNumberLibrary
{
    public class Float128 : BitArray
    {
        new const uint TotalBits = 128u;
        new const uint TotalBytes = 16u;
        public Float128() : base(TotalBits) { }

        /// <summary>
        /// Create an instance with all bit set to 0
        /// </summary>
        /// <returns>BitArray instance with all bit set to 0</returns>
        public static Float128 AllZero()
        {
            return AllZero(TotalBits) as Float128;
        }

        /// <summary>
        /// Create an instance with all bit set to 1
        /// </summary>
        /// <returns>BitArray instance with all bit set to 1</returns>
        public static Float128 AllOne()
        {
            return AllOne(TotalBits) as Float128;
        }
    }


}
