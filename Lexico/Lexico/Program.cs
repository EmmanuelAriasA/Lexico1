/*Emmanuel Arias Aguilar*/
using System;

namespace Lexico
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lexic l = new Lexic())
                {
                    while (!l.FinDeArchivo())
                    {
                        l.NextToken();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
