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
            Console.WriteLine("Aaron's Lottery Simulator");
            Console.WriteLine("You have $100.00 to start, be prudent");
            Console.WriteLine("\nWelcome to PowerBall! ");

            PowerBall powerBall = new PowerBall();
            powerBall.PlayLotto();

            //Console.WriteLine(pb.TotalWalletAmount);


            Console.ReadLine();
        }
    }
}
