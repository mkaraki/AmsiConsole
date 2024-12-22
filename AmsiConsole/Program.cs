using System;
using System.Text;
using AmsiWrapper;

namespace AmsiConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                args = new string[] { "eicar" };
            }

            switch (args[0])
            {
                case "eicar":
                    {
                        string eicar_p1 = "X5O!P%@AP[4\\PZX54(P^)7CC)7}$";
                        string eicar_p2 = "EICAR-STANDARD-ANTIVIRUS-TEST-FILE!";
                        string eicar_p3 = "$H+H*";
                        byte[] eicar_bytes = Encoding.ASCII.GetBytes(eicar_p1 + eicar_p2 + eicar_p3);

                        using (var amsi = new AmsiWrapper.AmsiWrapper("AmsiConsole"))
                        {
                            AMSI_RESULT result = amsi.ScanBuffer(eicar_bytes, "eicar.txt");
                            Console.WriteLine("INTERNAL_EICAR_PATTERN_ASCII_BYTES: Result: {0}", result);
                        }

                        break;
                    }

                case "eicar_string":
                    {
                        string eicar_p1 = "X5O!P%@AP[4\\PZX54(P^)7CC)7}$";
                        string eicar_p2 = "EICAR-STANDARD-ANTIVIRUS-TEST-FILE!";
                        string eicar_p3 = "$H+H*";

                        using (var amsi = new AmsiWrapper.AmsiWrapper("AmsiConsole"))
                        {
                            AMSI_RESULT result = amsi.ScanString(eicar_p1 + eicar_p2 + eicar_p3, "eicar.txt");
                            Console.WriteLine("Result: {0}", result);
                        }
                        break;
                    }

                case "url":
                    {
                        if (args.Length != 2)
                        {
                            Console.WriteLine("This will scan URL");
                            Console.WriteLine("Note: This will use Invoke-WebRequest commandlet in PowerShell and scan as script. Some anti-malware solution won't scan this.");
                            Console.WriteLine("Usage: url <url>");
                            break;
                        }

                        string url = args[1];
                        string urlEncode = Uri.EscapeUriString(url);

                        string scanString = "Invoke-WebRequest '" + urlEncode + "'";

                        using (var amsi = new AmsiWrapper.AmsiWrapper("AmsiConsole"))
                        {
                            AMSI_RESULT result = amsi.ScanString(scanString, url);
                            Console.WriteLine("Result: {0}", result);
                        }

                        break;
                    }

                default:
                    {
                        Console.WriteLine("No such commands.");
                        Console.WriteLine("Available commands: eicar, eicar_string, url");
                        break;
                    }
            }
        }
    }
}
