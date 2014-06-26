﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;


namespace LottoSimulator
{
    class PowerBall : Control
    {
        // TODO: FISHER-YATES INTO SELFPICK/EASYPICK (STILL DRAWING DUPLICATES)
        // TODO: MULTITHREADING

        #region Class declarations / Accessors
        private Random rngMainRandom = new Random();
        private double totalWalletAmount = 100.00;
        private int[] selfPickWhiteBallArray = new int[MAX_PICK_NUM];
        private int[] selfPickPBArray = new int[1];
        private int[] computerPickWhiteBallArray = new int[MAX_PICK_NUM];
        private int[] computerPickPBArray = new int[1];
        private bool selfPick = false;
        private bool easyPick = false;
        private bool simulationMode = false;
        // MTTEST
        private static Dictionary<String, Object> LockObjects = new Dictionary<string, object>();

        public double TotalWalletAmount
        {
            get { return totalWalletAmount; }
            set { totalWalletAmount = value; }
        }
        #endregion

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
            for (int i = 0; i < MAX_PICK_NUM; i++)
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
                Console.WriteLine("JACKPOT");
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

        #region Simulation
        // Simulation Mode - This is a disgusting function and is not intended for user access
        private void Simulation()
        {
            // Clearing the log.txt file for each new write
            File.Create(@"C:\Users\ajordan\Desktop\log.txt").Close();

            Console.WriteLine("\n\n                           ***SIMULATION MODE***");
            Console.Write("Runs: ");
            long runNumber = Convert.ToInt64(Console.ReadLine());

            #region Local declarations

            int[] userArray = new int[MAX_PICK_NUM];
            int[] winningNumbers = new int[MAX_PICK_NUM];
            int userPB = 0;
            int winningPB = 0;
            int totalHits = 0;
            int totalPBHits = 0;
            int gameHits = 0;
            int oneHit = 0;
            int twoHit = 0;
            int threeHit = 0;
            int fourHit = 0;
            int fiveHit = 0;
            double totalPlayWinnings = 0;
            double totalPlayLosses = 0;

            #endregion

            // Stopwatch for benchmarking purposes
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Fill winningNumbers, Fisher-Yates
            ShuffleWinningNumbers(winningNumbers);

            // Fill winningPB
            winningPB = rngMainRandom.Next(1, 36);

            // Print winningNumbers to log.txt
            using (var fileWriter = new StreamWriter(@"C:\Users\ajordan\Desktop\log.txt", true))
            {
                fileWriter.WriteLine("Winning numbers: [{0}] - PowerBall: [{1}]", string.Join(", ", winningNumbers.Select(v => v.ToString())), winningPB);
            }

            // Sort winningNumbers for faster comparison (sorted | unsorted) vs (unsorted | unsorted)
            Array.Sort(winningNumbers);

            // Flow through selected number of runs (top of loop-stack)
            SimulatorLogic(runNumber, totalPlayLosses, userArray, userPB, winningNumbers, totalHits, winningPB, totalHits, totalPlayWinnings, oneHit, twoHit, threeHit, fourHit, fiveHit, gameHits);

            // Stop benchmarking
            stopWatch.Stop();

            // Write stopWatch timings to times.txt for benchmarking -- @"C:\Users\ajordan\Desktop\times.txt" -- is hard coded
            using (StreamWriter fileWriter = new StreamWriter(@"C:\Users\ajordan\Desktop\times.txt", true))
            {
                fileWriter.WriteLine("{0} simulations processed in {1}", runNumber, stopWatch.Elapsed);
            }

            #region Output
            // Display useful statistics
            Console.WriteLine("\n{0} plays total", runNumber);
            Console.WriteLine("{0} total hits - {1} total PowerBall hits", totalHits, totalPBHits);
            Console.WriteLine("\n{0} - 0 hit game(s)", (runNumber - gameHits));
            Console.WriteLine("{0} - 1 hit game(s)", oneHit);
            Console.WriteLine("{0} - 2 hit game(s)", twoHit);
            Console.WriteLine("{0} - 3 hit game(s)", threeHit);
            Console.WriteLine("{0} - 4 hit game(s)", fourHit);
            Console.WriteLine("{0} - 5 hit game(s)", fiveHit);
            Console.WriteLine("{0} of games with hits - {1} of games without hits", gameHits, (runNumber - gameHits));

            Console.WriteLine("\n${0} total money won - ${1} total money lost", totalPlayWinnings, totalPlayLosses);
            Console.WriteLine("Final wallet amount: {0}", TotalWalletAmount);
            Console.WriteLine("Time elapsed: {0}", stopWatch.Elapsed);
            Console.Write("Done..");
            Console.ReadLine();
            Environment.Exit(0);
            #endregion
        }

        public void ShuffleWinningNumbers(int[] winningNumbers)
        {
            var winningNumbersShuffleArray = Enumerable.Range(1, 59).ToArray();
            for (int i = winningNumbersShuffleArray.Length; i > 0; i--)
            {
                int j = rngMainRandom.Next(i);

                var tmp = winningNumbersShuffleArray[j];
                winningNumbersShuffleArray[j] = winningNumbersShuffleArray[i - 1];
                winningNumbersShuffleArray[i - 1] = tmp;
            }
            for (int i = 0; i < MAX_PICK_NUM; i++)
            {
                winningNumbers[i] = winningNumbersShuffleArray[i];
            }
        }

        public void SimulatorLogic(long runNumber, double totalPlayLosses, int[] userArray, int userPB, int[] winningNumbers, int totalHits, int winningPB, int totalPBHits, double totalPlayWinnings, int oneHit, int twoHit, int threeHit, int fourHit, int fiveHit, int gameHits)
        {
            // Write to log.txt @ desktop -- @"C:\Users\ajordan\Desktop\log.txt" -- is hard coded
            string outputFile = "C:\\Users\\ajordan\\Desktop\\log.txt";
            using (FileStream fs = new FileStream(outputFile, FileMode.Append, FileSystemRights.AppendData, FileShare.Read, 4096, FileOptions.None))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    Parallel.For(0, runNumber, i =>
                    {
                        int localHits = 0;
                        double playWinnings = 0;
                        bool pbHit = false;
                        totalPlayLosses -= 3;
                        TotalWalletAmount -= 3;

                        // Fisher-Yates
                        var userShuffleArray = Enumerable.Range(1, 59).ToArray();
                        for (int b = userShuffleArray.Length; b > 0; b--)
                        {
                            int j = rngMainRandom.Next(b);

                            var tmp = userShuffleArray[j];
                            userShuffleArray[j] = userShuffleArray[b - 1];
                            userShuffleArray[b - 1] = tmp;
                        }
                        for (int a = 0; a < MAX_PICK_NUM; a++)
                        {
                            userArray[a] = userShuffleArray[a];
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

                        // Compare PowerBalls
                        if (winningPB == userPB)
                        {
                            pbHit = true;
                            totalPBHits++;
                        }

                        #region Wallet logic

                        // All winnings logic
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
                                    Console.WriteLine("Defaulted in wallet logic - Simulation()");
                                    break;
                            }
                        }

                        #endregion

                        // Get total amount of winnings
                        totalPlayWinnings += playWinnings;

                        #region localHits

                        switch (localHits)
                        {
                            case 0:
                                break;
                            case 1:
                                oneHit++;
                                gameHits++;
                                break;
                            case 2:
                                twoHit++;
                                gameHits++;
                                break;
                            case 3:
                                threeHit++;
                                gameHits++;
                                break;
                            case 4:
                                fourHit++;
                                gameHits++;
                                break;
                            case 5:
                                fiveHit++;
                                gameHits++;
                                break;
                            default:
                                Console.WriteLine("Defaulted in StreamWriter - Simulator()");
                                break;
                        }

                        #endregion

                        //userArrayTransfer(winningNumbers, winningPB, localHits, pbHit, playWinnings, userArray, userPB, i);
                        WriteToLog(winningNumbers, winningPB, localHits, pbHit, playWinnings, userArray, userPB, i, outputFile, sw);
                    });
                }
            }
        }

        public static void WriteToLog(int[] winningNumbers, int winningPB, int localHits, bool pbHit, double playWinnings, int[] userArray, int userPB, long runningNumbersI, string outputFile, StreamWriter sw)
        {
            if (!LockObjects.ContainsKey(outputFile))
            {
                LockObjects.Add(outputFile, new object());
            }

            lock (LockObjects[outputFile])
            {                                  
                sw.WriteLine("Play {0}: {1} hit(s) - {2} PowerBall - Won ${3} - [{4}] - [{5}] - Thread: [{6}]", (runningNumbersI + 1), localHits, pbHit, playWinnings, string.Join(", ", userArray.Select(v => v.ToString(CultureInfo.CurrentCulture))), userPB, Thread.CurrentThread.ManagedThreadId);                              
            }
        }

        //public void userArrayTransfer(int[] winningNumbers, int winningPB, int localHits, bool pbHit, double playWinnings, int[] userArray, int userPB, long i)
        //{
        //    WriteToLog(winningNumbers, winningPB, localHits, pbHit, playWinnings, userArray, userPB, i);
        //}
        #endregion
    }
}
