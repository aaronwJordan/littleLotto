using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LottoSimulator
{
    class PowerBall : Control
    {
        // TODO: SIMULATION MODE
        // TODO: MULTITHREADING
        // TODO: UP TO FIVE PLAYS PER PLAY

        private Random rngMainRandom = new Random();
        private double totalWalletAmount = 100.00;
        private int[] selfPickWhiteBallArray = new int[MAX_PICK_NUM];
        private int[] selfPickPBArray = new int[1];
        private int[] computerPickWhiteBallArray = new int[MAX_PICK_NUM];
        private int[] computerPickPBArray = new int[1];
        private bool selfPick = false;
        private bool easyPick = false;
        private bool simulationMode = false;

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

            // --- Simulation Mode
            if (userChoice.Equals("SIMULATION"))
            {
                simulationMode = true;
                Simulation();
            }
            // --- Simulation Mode


            if (userChoice.Equals("y") || userChoice.Equals("Y"))
                SelfPick();
            else if (userChoice.Equals("n") || userChoice.Equals("N"))
                EasyPick();
            else if (!userChoice.Equals("y") && !userChoice.Equals("n"))
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
            selfPick = true;
            Console.Write("Please enter five numbers between (1-59): ");
            for (int i = 0; i < MAX_PICK_NUM; i ++)
            {
                Console.Write("\n" + (i + 1) + ".) ");
                selfPickWhiteBallArray[i] = Convert.ToInt16(Console.ReadLine());
            }

            Console.Write("\nPlease enter your PowerBall number (1-35): ");
            selfPickPBArray[0] = Convert.ToInt16(Console.ReadLine());

            TotalWalletAmount -= 3.00; // Takes money to make money
            TimeToDraw();
        }

        // Computer selects user's numbers automatically
        private void EasyPick()
        {
            easyPick = true;
            for (int i = 0; i < MAX_PICK_NUM; i++)
            {
                computerPickWhiteBallArray[i] = WhiteBallRandomNumberGenerator(rngMainRandom);
            }

            computerPickPBArray[0] = RedBallRandomNumberGenerator(rngMainRandom);

            TotalWalletAmount -= 3.00;
            TimeToDraw();
        }

        // Computer selects winning lottery numbers
        private void TimeToDraw()
        {
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
            bool matchingPB = false;

            if (selfPick)
            {
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
                    matchingPB = true;
                    matchingNumbers++;
                }
            }

            if (easyPick)
            {
                for (int i = 0; i < MAX_PICK_NUM; i++)
                {
                    for (int j = 0; j < MAX_PICK_NUM; j++)
                    {
                        if (computerWhiteBallArray[j] == computerPickWhiteBallArray[i])
                        {
                            matchingNumbers++;
                        }
                    }
                }
                if (computerRedBallArray[0] == computerPickPBArray[0])
                {
                    matchingPB = true;
                    matchingNumbers++;
                }
            }
            WalletExchange(matchingNumbers, matchingPB);
            OutputResults(matchingNumbers, computerWhiteBallArray, computerRedBallArray);
        }

        // Output results
        private void OutputResults(int matchingNumbers, int[] computerWhiteBallArray, int[] computerRedBallArray)
        {
            Console.WriteLine("\nYou have {0} hits", matchingNumbers);

            // Output user selection
            Console.Write("Your numbers are: ");
            if (selfPick)
            {
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
            }

            if (easyPick)
            {
                for (int i = 0; i < MAX_PICK_NUM; i++)
                {
                    if (i == (MAX_PICK_NUM - 1))
                    {
                        Console.Write(computerPickWhiteBallArray[i]);
                        break;
                    }
                    Console.Write(computerPickWhiteBallArray[i] + ", ");
                }
                Console.WriteLine(" and a PowerBall of {0}", computerPickPBArray[0]);
            }

            selfPick = false;
            easyPick = false;
            
            // Output computer selection
            Console.Write("\nThe winning lottery numbers are: ");
            for (int i = 0; i < MAX_PICK_NUM; i++)
            {
                if (i == (MAX_PICK_NUM - 1))
                {
                    Console.Write(computerWhiteBallArray[i]);
                    break;
                }
                Console.Write(computerWhiteBallArray[i] + ", ");
            }
            Console.WriteLine(" with a PowerBall of {0}", computerRedBallArray[0]);

            Console.WriteLine("Your current wallet amount is: ${0}", TotalWalletAmount);
        }

        // Handles wallet
        private void WalletExchange(int matchingNumbers, bool matchingPB)
        {

            if (matchingPB && matchingNumbers == 0)
                TotalWalletAmount += 4.00;
            else if (matchingPB && matchingNumbers == 1)
                TotalWalletAmount += 4.00;
            else if (matchingPB && matchingNumbers == 2)
                TotalWalletAmount += 7.00;
            else if (matchingPB && matchingNumbers == 3)
                TotalWalletAmount += 100.00;
            else if (matchingPB && matchingNumbers == 4)
                TotalWalletAmount += 10000.00; // 10,000
            else if (matchingPB && matchingNumbers == 5)
            {
                TotalWalletAmount += JackPot();
                Console.WriteLine("Daaaaaaamn, you should play the real lottery");
            }

            if (!matchingPB)
            {
                switch (matchingNumbers)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        TotalWalletAmount += 7.00;
                        break;
                    case 4:
                        TotalWalletAmount += 100.00;
                        break;
                    case 5:
                        TotalWalletAmount += 1000000.00; // 1 million
                        break;
                    default:
                        Console.WriteLine("Call the Coast Guard");
                        break;
                }
            }
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

        // Jackpot is struck
        private int JackPot()
        {
            int jackPot = rngMainRandom.Next(100000000, 750000000); // 100 million / 750 million

            return jackPot;
        }

        // Simulation Mode
        private void Simulation()
        {
            // Play 1: # hit(s) - 0 PowerBall - Won $0.00
            // Play 2: # hit(s) - 1 PowerBall - Won $7.00
            // 
            // # of total hits - # of total PowerBall hits
            // # of games with hits - # of games without hits
            // # of total money won - # of total money lost
            // 
            // Time elapsed: xx.xxx seconds

            Console.WriteLine("\n\n                           ***SIMULATION MODE***");
            Console.Write("Runs: ");
            long runNumber = Convert.ToInt64(Console.ReadLine());

            int[] userArray = new int[MAX_PICK_NUM];
            int[] winningNumbers = new int[MAX_PICK_NUM];
            int userPB = 0;
            int winningPB = 0;
            int totalHits = 0;
            int totalPBHits = 0;
            int gameHit = 0;
            double totalPlayWinnings = 0;
            double totalPlayLosses = 0;
            double simulatorWallet = 100.00;
            File.Create(@"C:\Users\ajordan\Desktop\log.txt").Close();

            // Fill winningNumbers
            for (int i = 0; i < MAX_PICK_NUM; i++)
            {
                winningNumbers[i] = rngMainRandom.Next(1, 60);
            }
            // Fill winningPB
            winningPB = rngMainRandom.Next(1, 36);

            // Sort winningNumbers for faster comparison (sorted | unsorted) vs (unsorted | unsorted)
            Array.Sort(winningNumbers);

            // Stopwatch for benchmarking purposes
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();    
            // Flow through selected number of runs
            for (int i = 0; i < runNumber; i++)
            {
                int localHits = 0;
                double playWinnings = 0;
                bool pbHit = false;
                totalPlayLosses -= 3;
                TotalWalletAmount -= 3;

                // Generate userNumbers and userPowerBalls automatically
                for (int a = 0; a < MAX_PICK_NUM; a++)
                {
                    userArray[a] = rngMainRandom.Next(1, 60);
                }
                userPB = rngMainRandom.Next(1, 36);

                // Compare userArray to winningNumbers
                for (int j = 0; j < MAX_PICK_NUM; j++)
                {
                    for (int k = 0; k < MAX_PICK_NUM; k++)
                    {
                        if (winningNumbers[j] == userArray[k])
                        {
                            localHits++;
                            totalHits++;
                        }
                    }
                }

                #region
                // Compare PowerBalls
                if (winningPB == userPB)
                {
                    pbHit = true;
                    totalPBHits++;
                }

                if (pbHit && localHits == 0)
                {
                    TotalWalletAmount += 4.00;
                    playWinnings = 4;
                }    
                else if (pbHit && localHits == 1)
                {
                    TotalWalletAmount += 4.00;
                    playWinnings = 4;
                }
                    
                else if (pbHit && localHits == 2)
                {
                    TotalWalletAmount += 7.00;
                    playWinnings = 7;
                }
                    
                else if (pbHit && localHits == 3)
                {
                    TotalWalletAmount += 100.00;
                    playWinnings = 100;
                }
                    
                else if (pbHit && localHits == 4)
                {
                    TotalWalletAmount += 10000.00; // 10,000
                    playWinnings = 10000;
                }
                    
                else if (pbHit && localHits == 5)
                {
                    long tempJackPot = JackPot();
                    TotalWalletAmount += tempJackPot;
                    playWinnings = tempJackPot;
                    Console.WriteLine("JACKPOT");
                }

                if (!pbHit)
                {
                    switch (localHits)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            TotalWalletAmount += 7.00;
                            playWinnings = 7;
                            break;
                        case 4:
                            TotalWalletAmount += 100.00;
                            playWinnings = 100;
                            break;
                        case 5:
                            TotalWalletAmount += 1000000.00; // 1 million
                            playWinnings = 1000000;
                            break;
                        default:
                            Console.WriteLine("Call the Coast Guard");
                            break;
                    }
                }
                #endregion

                totalPlayWinnings += playWinnings;
                using (StreamWriter fileWriter = new StreamWriter(@"C:\Users\ajordan\Desktop\log.txt", true))
                {
                    fileWriter.WriteLine("Play {0}: {1} hit(s) - {2} PowerBall - Won ${3}", (i + 1), localHits, pbHit, playWinnings);
                }
            }

            stopWatch.Stop();

            Console.WriteLine("\n{0} plays total", runNumber);
            Console.WriteLine("{0} total hits - {1} total PowerBall hits", totalHits, totalPBHits);
            //Console.WriteLine("{0} of games with hits - {1} of games without hits"); Read file in and count 0's up for # of games without hits,  
            //then subtract from total amount of games for # of games with hits
            Console.WriteLine("${0} total money won - ${1} total money lost", totalPlayWinnings, totalPlayLosses);
            
            Console.WriteLine("\nFinal wallet amount: {0}", TotalWalletAmount);
            Console.WriteLine("Time elapsed: {0}", stopWatch.Elapsed);
            Console.Write("Done..");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
