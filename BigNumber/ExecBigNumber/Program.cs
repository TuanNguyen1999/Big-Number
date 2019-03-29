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
            Float128 fA = new Float128();
            for (uint i = 0u; i < 50u; i++)
            {
                fA.SetBitAt(i, true);
                PrintBits(fA);
            }
            Float128 fB = Float128.AllOne();
            return;
        }
        static public void PrintBits(in Float128 value)
        {
            for (int i = 50; i >= 0; i--)
            {
                Console.Write(value.GetBitAt((uint)i) ? '1' : '0');
            }
            Console.Write("\n");
        }
    }
}
