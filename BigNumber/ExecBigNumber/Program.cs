using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigNumberLibrary;
namespace ExecBigNumber
{
    class Program
    {
        static void Main(string[] args)
        {
            var rand = new System.Random();
            string charSet = "1234567890aAbBcCdDeEfF+-,.";
            string base10CharSet = "1234567890AB,.";
            string base2CharSet = "10";
            List<int> NumerBases = new List<int> { 2, 16 };
            for(int i = 0;i<1000;i++)
            {
                int baseNum = NumerBases[rand.Next() % NumerBases.Count];
                string test = "";
                for(int k =0;k<rand.Next() % 200 + 10;k++)
                {
                    test += base2CharSet[rand.Next() % base2CharSet.Length];
                }
                test += "    ";
                Console.WriteLine(Float128.GetFloat128(test, baseNum).ToString(2));
            }
            //Console.WriteLine(string.Format("Test string: {0}, base {2}\nResult: {1}\n\n", "    1234.2134.14", CheckString("    1234.2134.14", 10), 10));
            return;
        }
        static public void PrintBits(in Float128 value)
        {
            for (int i = 0; i < Float128.TotalBytes;i++)
            {
                Console.Write(value.GetBit((uint)i) ? '1' : '0');
            }
            Console.Write("\n");
        }
        static public string CheckString(in string value,int numBase)
        {
            try
            {
                return Float128.SyntaxCheck(value, numBase)?? "Syntax Error";
            }
            catch (Exception e)
            {
                return "Exception detected at " + e.Message;
            }
        }
    }
}
