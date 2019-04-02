using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BigNumberLibrary
{
    /// <summary>
    /// 128 bits floating point number with:
    /// <para>sign    : 1 bit</para>
    /// <para>exponent: 15 bits</para>
    /// <para>mantissa: 112 bits</para>
    /// </summary>
    public class Float128 : BitArray
    {
        new public const uint TotalBits = 128u;
        new public const uint TotalBytes = 16u;
        public enum Bases
        {
            Binary = 2,
            Decimal = 10,
            Hexadecimal = 16
        };

        public static uint MaxInputLength(int numBase)
        {
            if (AllowBases.IndexOf(numBase) == -1) throw new ArgumentOutOfRangeException("MaxInputLength: numBase");
            if (numBase == 2) return TotalBits;
            if (numBase == 10) return TotalBytes * 3;
            else return TotalBytes * 2;
        }
        public static readonly Dictionary<char, int> ValidChars = new Dictionary<char, int>
        {
            {'0',0},{'1',1},{'2',2},{'3',3},{'4',4},{'5',5},{'6',6},{'7',7},{'8',8},{'9',9},
            {'A',10 },{'a',10},
            {'B',11 },{'b',11},
            {'C',12 },{'c',12},
            {'D',13 },{'d',13},
            {'E',14 },{'e',14},
            {'F',15 },{'f',15},
            {'+', '+'},{'-','-'},{',',','},{'.', '.'}
        };
        public static readonly List<int> AllowBases = new List<int> { 2, 10, 16 };

        public Float128() : base(TotalBits) { }

        /// <summary>
        /// Create an instance with all bit set to 0
        /// </summary>
        /// <returns>Float128 instance with all bit set to 0</returns>
        public static Float128 AllZero()
        {
            return new Float128 { BytesArray = AllZero(TotalBits) };
        }

        /// <summary>
        /// Create an instance with all bit set to 1
        /// </summary>
        /// <returns>Float128 instance with all bit set to 1</returns>
        public static Float128 AllOne()
        {
            return new Float128 { BytesArray = AllOne(TotalBits) };
        }

        /// <summary>
        /// Converting a new 128 bits floating point number from given <paramref name="value"/> in base <paramref name="_base"/>
        /// <para>Exception is thrown if given string is not in valid format or numBase not in allowed handling range</para>
        /// </summary>
        /// <param name="value">Represent numeric value in string format</param>
        /// <param name="_base">Base number of input string</param>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Float128 GetFloat128(string value, int numBase)
        {
            // remove heading and trailing whitespace
            value = value.Trim();

            value = SyntaxCheck(value, numBase);

            // throw or init with value 0
            if (Equals(value, null)) throw new FormatException("GetFloat128");

            if (numBase != 10)
                OverFlowCheck(value, numBase);

            // Convert string to Float128
            switch (numBase)
            {
                case 2:
                    return FromBinString(value);
                case 10:
                    return FromDecString(value);
                case 16:
                    return FromHexString(value);
                default:
                    throw new ArgumentOutOfRangeException("GetFloat128: numBase");

            }
        }

        /// <summary>
        /// Convert a hex string to an instance of Float128.
        /// Method converts each pair of hexadecimal digits to its byte representation and add to BytesArray
        /// </summary>
        /// <param name="hex">Hexadecimal string to be converted</param>
        /// <returns>Float128 value with each byte is filled with 2 hexadecimal digits</returns>
        static private Float128 FromHexString(string hex)
        {
            Float128 result = new Float128();

            // Expand string so that it can fit right into 16 bytes array
            if (hex.Length < (int)(TotalBits / 4u))
            {
                hex = hex.Insert(0, new string('0', (int)(TotalBits / 4u) - hex.Length));
            }

            // A hex digit can be writen as 0000 xxxx
            // So we will take each pair of hex values and convert it to a byte
            int i = 0;
            while (i / 2 < TotalBytes)
            {
                int high = ValidChars[hex[i]];
                int low = ValidChars[hex[i + 1]];

                // Get the <high> value and offset it by 4 bits to the left to get xxxx 0000
                // then add to the <low> value which is 0000 yyyy
                int dec = (high << 4) + low;

                result.SetByte((uint)(i / 2), (byte)dec, false);

                i += 2;
            }

            return result;
        }

        /// <summary>
        /// Convert a decimal string to an instance of Float128.
        /// </summary>
        /// <param name="dec">Decimal string to be converted</param>
        /// <param name="result">ouput after conversion</param>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        static private Float128 FromDecString(string dec)
        {
            throw new NotImplementedException("FromDecString");
        }

        /// <summary>
        /// Convert a binary string to an instance of Float128. 
        /// Method assigns each bit of the array to the value of its corresponding character from given string
        /// </summary>
        /// <param name="bin">Binary string to be converted</param>
        /// <returns>Float128 value</returns>
        static private Float128 FromBinString(string bin)
        {
            Float128 result = new Float128();

            // Insert or truncate string to fit 128 bits
            if (bin.Length < TotalBits)
            {
                bin = bin.Insert(0, new string('0', (int)TotalBits - bin.Length));
            }

            // Take a group of 8 bits to form 1 byte
            int i = 0;
            while (i < TotalBits)
            {
                int dec = 0;
                for (int k = 7; k >= 0; k--)
                {
                    int temp = ValidChars[bin[i++]];
                    dec += temp << k;
                }

                result.SetByte((uint)(i / 8), (byte)dec, false);
            }

            return result;
        }

        /// <summary>
        /// Check for syntax error and remove redundant characters.
        /// </summary>
        /// <param name="input">string to be checked</param>
        /// <param name="strBase">number base</param>
        /// <returns>null if syntax error happens, otherwise new modified string</returns>
        static public string SyntaxCheck(in string input, int strBase)
        {
            if (AllowBases.IndexOf(strBase) == -1) throw new ArgumentOutOfRangeException("SyntaxCheck");
            if (string.IsNullOrWhiteSpace(input)) return null;

            try
            {
                string output = null;
                int i = 0;
                string Signs = "+-";
                string Delims = ",.";
                bool FoundDelim = false;
                Stack<char> stkSign = new Stack<char>();
                stkSign.Push('+');  //Place holder

                // Remove signs with the following rules:
                //  1. Remove any pair of "--"
                //  2. Remove any '+'
                while (Signs.IndexOf(input[i]) != -1)
                {
                    if (input[i] == '-')
                    {
                        if (stkSign.Peek() == '-') stkSign.Pop();
                        else stkSign.Push(input[i]);
                    }
                    ++i;
                }
                if (stkSign.Count == 2) output += stkSign.Pop();

                // Return null if one of these following rules is true:
                //  1. there is a sign character (session above already handled sign characters)
                //  2. there is an character which is neither belong to base's digits or delimiter
                //  3. there is more than one delimiter 
                //  4. there is only one delimiter but base is not decimal
                while (i < input.Length)
                {
                    int chValue = ValidChars[input[i]];

                    if (chValue >= 0 && chValue < strBase) output += input[i];
                    else if (Delims.IndexOf(input[i]) != -1)
                    {
                        if (strBase != 10 || FoundDelim) return null;
                        output += '.';
                        FoundDelim = true;
                    }
                    else return null;

                    ++i;
                }

                // Add 0 for fraction if there is a trailing delimiter
                if (output.Last() == '.') output += '0';
                return output;
            }
            catch (KeyNotFoundException)
            {
                return null; ;
            }
        }

        /// <summary>
        /// Check if the length of input string exceeds the maximum allowed characters in the given base number
        /// </summary>
        /// <param name="input">string to be checked</param>
        /// <param name="strBase">base number for its equivalent max characters</param>
        static void OverFlowCheck(in string input,int strBase)
        {
            if (AllowBases.IndexOf(strBase) == -1) throw new ArgumentOutOfRangeException("OverFlowCheck");
            if (input.Length > MaxInputLength(strBase))
                throw new OverflowException(string.Format("OverFlowCheck! Allow: {0} but received {1}",MaxInputLength(strBase),input.Length));

        }

        /// <summary>
        /// Convert Float128 to string.
        /// </summary>
        /// <param name="strBase">Base number for ouput string</param>
        /// <returns>New string represents Float128 value</returns>
        public string ToString(int strBase)
        {
            if (AllowBases.IndexOf(strBase) == -1) throw new ArgumentOutOfRangeException("ToString");
            string output = "";
            if(strBase == 2)
            {
                for (uint i = 0; i < TotalBits; i++)
                    output += GetBit(i, false)? '0':'1';
            }
            else if(strBase == 10)
            {
                throw new NotImplementedException();
            }
            else
            {
                output = BitConverter.ToString(BytesArray);
            }
            return output;
        }
        public string BeautyString(int strBase)
        {
            throw new NotImplementedException();
        }
    }

}

