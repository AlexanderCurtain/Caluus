using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public static class Program
    {
        private static void Main()
        {
            using (window game = new window(700, 700, "TK"))
            {
                game.Run(60.0);
            }
        }
    }

}
