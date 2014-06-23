using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LottoSimulator
{
    class Control
    {
        protected const int MAX_PICK_NUM = 5;

        static void Main(string[] args)
        {
            Console.WriteLine("Aaron's Lottery Simulator .1 beta");
            Console.WriteLine("You have $100.00 to start, be prudent. Each is play $3.00");
            Console.WriteLine("\nWelcome to PowerBall! ");

            PowerBall powerBall = new PowerBall();
            powerBall.PlayLotto();

            Console.WriteLine("Thanks for playing!");
            Console.WriteLine("Your final wallet amount: ${0}", powerBall.TotalWalletAmount);

            Console.ReadLine();
        }
    }
}
