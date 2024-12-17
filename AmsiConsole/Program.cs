using System;
using System.Text;
using AmsiWrapper;

namespace AmsiConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string eicar = "X5O!P%@AP[4\\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*";
            byte[] eicar_bytes = Encoding.ASCII.GetBytes(eicar);

            using (var amsi = new AmsiWrapper.AmsiWrapper("AmsiConsole"))
            {
                AMSI_RESULT result = amsi.ScanBuffer(eicar_bytes, "eicar.txt");
                //Amsi.AMSI_RESULT result = amsi.ScanString(eicar, "eicar.txt");
                Console.WriteLine("Result: {0}", result);
            }
        }
    }
}
