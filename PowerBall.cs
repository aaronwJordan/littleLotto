using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LottoSimulator
{
    class PowerBall : Control
    {
        private double totalWalletAmount = 100.00;
        private int[] selfPickWhiteBallArray = new int[MAX_PICK_NUM];
        private int[] selfPickPBArray = new int[1];

        public double TotalWalletAmount
        {
            get { return totalWalletAmount; }
            set { totalWalletAmount = value; }
        }

        // Main control logic
        public void PlayLotto()
        {
            Retry:
            Console.Write("\nWould you like to choose your own numbers? If you select no a computer will generate them for you (y/n): ");
            string userChoice = Console.ReadLine();

            if (userChoice.Equals("y") || userChoice.Equals("Y"))
                SelfPick();
            else if (userChoice.Equals("n") || userChoice.Equals("N"))
                EasyPick();
            else
            {
                Console.Clear();
                Console.WriteLine("\nPlease enter either (y) for Yes or (n) for No");
                goto Retry; // Please don't kill me, it's a local goto.
            }

            Console.Write("\nWould you like to play again? (y/n): ");
            userChoice = Console.ReadLine();
            if (userChoice.Equals("y") || userChoice.Equals("Y"))
                goto Retry; // Last one I promise
            return;
        }

        // User selects their numbers manually
        private void SelfPick()
        {
            Console.Write("Please enter five numbers between (1-59): ");
            for (int i = 0; i < MAX_PICK_NUM; i ++)
            {
                Console.Write("\n" + (i + 1) + ".) ");
                selfPickWhiteBallArray[i] = Convert.ToInt16(Console.ReadLine());
            }

            Console.Write("\nPlease enter your PowerBall number (1-35): ");
            selfPickPBArray[0] = Convert.ToInt16(Console.ReadLine());

           TimeToDraw();
        }

        // Computer selects user's numbers automatically
        private void EasyPick()
        {

        }

        // Computer selects winning lottery numbers
        private void TimeToDraw()
        {
            Random rngMainRandom = new Random();
            int[] tempWhiteBallArray = new int[MAX_PICK_NUM];
            int[] tempRedBallArray = new int[1];

            for (int i = 0; i < MAX_PICK_NUM; i++)
            {
                tempWhiteBallArray[i] = WhiteBallRandomNumberGenerator(rngMainRandom);
            }

            tempRedBallArray[0] = RedBallRandomNumberGenerator(rngMainRandom);

            TimeToCompare(tempWhiteBallArray, tempRedBallArray);
        }

        // User's numbers are compared with winning lottery number
        private void TimeToCompare(int[] computerWhiteBallArray, int[] computerRedBallArray)
        {
            int matchingNumbers = 0;
            for (int i = 0; i < MAX_PICK_NUM; i++)
            {
                for (int j = 0; j < MAX_PICK_NUM; j++)
                {
                    if (computerWhiteBallArray[j] == selfPickWhiteBallArray[i])
                    {
                        matchingNumbers++;
                    }
                }
            }

            if (computerRedBallArray[0] == selfPickPBArray[0])
            {
                // We'll go ahead and count this as a hit
                // In the future, it should separate out and tell if the PB 
                // had a hit itself
                matchingNumbers++; 
            }

            OutputResults(matchingNumbers, computerWhiteBallArray, computerRedBallArray);
        }

        // Output results
        private void OutputResults(int matchingNumbers, int[] computerWhiteBallArray, int[] computerRedBallArray)
        {
            Console.WriteLine("\nYou have {0} hits", matchingNumbers);

            // Output user selection
            Console.Write("Your numbers are: ");
            for (int i = 0; i < MAX_PICK_NUM; i++)
            {
                // Doing this so there isn't a comma at the end (5, 4, 3, 2, 1,)
                if (i == (MAX_PICK_NUM - 1))
                {
                    Console.Write(selfPickWhiteBallArray[i]);
                    break;
                }
                Console.Write(selfPickWhiteBallArray[i] + ", ");
            }
            Console.WriteLine(" and a PowerBall of {0}", selfPickPBArray[0]);
            
            // Output computer selection
            Console.Write("\nThe winning lottery numbers are: ");
            for (int i = 0; i < MAX_PICK_NUM; i++)
            {
                // Doing this so there isn't a comma at the end (5, 4, 3, 2, 1,)
                if (i == (MAX_PICK_NUM - 1))
                {
                    Console.Write(computerWhiteBallArray[i]);
                    break;
                }
                Console.Write(computerWhiteBallArray[i] + ", ");
            }
            Console.WriteLine(" with a PowerBall of {0}", computerRedBallArray[0]);
        }

        // White ball = normal picks
        private int WhiteBallRandomNumberGenerator(Random fromMainRandom)
        {
            int randomWhite = fromMainRandom.Next(1, 60);

            return randomWhite;
        }

        // Red ball = PowerBall
        private int RedBallRandomNumberGenerator(Random fromMainRandom)
        {
            int randomRed = fromMainRandom.Next(1, 36);

            return randomRed;
        }
    }
}
