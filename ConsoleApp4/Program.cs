using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace ConsoleApp4
{
    class Puzzle
    {
        public int[,] v;
        public int g;
        public int hamming;
        public int manhattan;
        public int fHam;
        public int fManhattan;
        public int x_zero, y_zero;
        public string actionState;
        public Puzzle parent;
    };

    internal class Program
    {
         public static Dictionary<int, int> dic = new Dictionary<int, int>();
         //public static HashSet<string> hash = new HashSet<string>();

        public static bool isHamming;

        private static bool isSolvable(int[,] vec1)
        {
            int y=0;
            int n = vec1.GetLength(0);
            int invCount = getInvCount(vec1,ref y);
            Console.WriteLine("Count Inv = " + invCount);
            if (n % 2 == 0) {
                if (invCount % 2 == 0)
                {
                    if ((n - y) % 2 == 1)
                    {

                        return true;
                    }
                }
                else {
                    if ((n - y) % 2 == 0) {
                        return true;
                    }
                }
            } 
            else
                return (invCount%2==0);

            return false;
        }

        private static int getInvCount(int[,] v,ref int x)
        {
            int n = v.GetLength(0);
            int size = n * n, c = 0;
            int[] list = new int[size];
            int inv_count = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (v[i,j]==0) {
                        x = i;
                    }
                    list[c] = v[i, j];
                    c++;
                }
            }

            for (int i = 0; i < size - 1; i++)
                for (int j = i + 1; j < size; j++)
                    if (list[j] > 0 && list[i] > 0 && list[i] > list[j])
                        inv_count++;

            return inv_count;
        }  

        private static int hashFunc(int[,] puzzle)
        {
            int hash = 83;
            for (int i = 0; i < puzzle.GetLength(0); i++)
            {
                for (int j = 0; j < puzzle.GetLength(0); j++)
                {
                    hash += hash *97/17+ 17 + (puzzle[i, j] * (i + 1) * (j + 1))*(i+j);
                }
            }
            return hash;
        }
        
        private static string hashFuncString(int[,] puzzle)
        {
            string s="";
            for (int i = 0; i < puzzle.GetLength(0); i++)
            {
                for (int j = 0; j < puzzle.GetLength(0); j++)
                {
                    s += puzzle[i, j].ToString();
                }
            }
            return s;
        }

        private static void DisplayMatrix(int[,] puzzle)
        {
            for (int i = 0; i < puzzle.GetLength(0); i++)
            {
                for (int j = 0; j < puzzle.GetLength(0); j++)
                {
                    Console.Write(puzzle[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void DisplayPuzzle(Puzzle p)
        {
            DisplayMatrix(p.v);
            Console.WriteLine("Level = " + p.g);
            if (isHamming)
            {
                Console.WriteLine("No OF Misplaced Tiels = " + p.hamming);
                Console.WriteLine("F(n) OF Hamming Distance = " + p.fHam);
            }
            else
            {
                Console.WriteLine("Tolal Number OF Moves = " + p.manhattan);
                Console.WriteLine("F(n) OF Manhattan Distance = " + p.fManhattan);
            }
            Console.WriteLine("State OF The Puzzle : " + p.actionState);
            Console.WriteLine("=================================================");
            Console.WriteLine();
        }

        private static int[,] setMatrix(int[] num)
        {
            int count = 0;
            int n = Convert.ToInt32(Math.Sqrt(num.Length));
            int[,] vec1 = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    vec1[i, j] = num[count];
                    count++;
                }
            }
            return vec1;
        }

        private static void IntializePuzzle(int[,] vec1)
        {

            PriorityQueue<Puzzle, int> q = new PriorityQueue<Puzzle, int>();
            PriorityQueue<Puzzle, int> Steps = new PriorityQueue<Puzzle, int>();

            if (!isSolvable(vec1))
            {
                Console.WriteLine("Not Solvable");
                Console.WriteLine();
                DisplayMatrix(vec1);
            }
            else
            {
                Console.WriteLine("Solvable");
                int[,] Ideal = SetIdeal(vec1);
                int x = 0, y = 0;

                //--------set Intail Puzzl----------------
                Puzzle p;
                p = new Puzzle();
                p.v = vec1;
                p.g = 0;
                p.hamming = HammaingDistance(vec1, Ideal);
                p.manhattan = ManhattanDistance(vec1);
                DetectZero(p.v, ref x, ref y);
                p.x_zero = x; p.y_zero = y;
                p.actionState = "Intial";
                p.fHam = p.g + p.hamming;
                p.fManhattan = p.g + p.manhattan;
                p.parent = null;
                q.Enqueue(p, ChooseDistance(p, isHamming));
                //-----------------------------------------

                bool check = false;
                while (!check)
                {
                    check = Savingpriority_queue(ref q, q.Peek().v, Ideal);
                }
                dic.Clear();

                if (p.v.GetLength(0)<=3)
                {
                    Steps = GoalSteps(q.Peek());
                    while (Steps.Count != 0)
                    {
                        DisplayPuzzle(Steps.Peek());
                        Steps.Dequeue();
                    }
                }
                else
                {
                    DisplayMatrix(vec1);
                }
            }
        }

        private static int[] importProblems(ref string[] Lines)
        {
            if (Lines[0] == null) ChooseFileToRead();
            string input = Lines[0];
            int n = int.Parse(input), count = -1;

            try {

                n= int.Parse(input);
            }
            catch{
                    
                }
            int numToRemove = 0;
            Lines = Lines.Where((source, index) => index != numToRemove).ToArray();
            
            int size = n * n;
            int[] num = new int[size];

            for (int i = 0; i < n; i++)
            {
                input = Lines[i];
                while (input.Contains(" "))
                {
                    count++;
                    string s = input.Substring(0, input.IndexOf(" "));
                    num[count] = int.Parse(s);
                    int c = 0;
                    while (input[c] != ' ')
                    {
                        c++;
                    }
                    c++;
                    input = input.Remove(0, c);

                }
                count++;
                num[count] = int.Parse(input);

            }
            /////next problem
            for (int i = 0; i < n; i++)
            {
                Lines = Lines.Where((source, index) => index != numToRemove).ToArray();
            }

            return num;
        }

        static void Main(string[] args)
        {
            string[] Lines = ChooseCompleteOrSample();
            int len = Lines.Length;
            int problemCount = 1;
            while (Lines.Length != 0)
            {
                int[] num = importProblems(ref Lines);
                Console.Clear();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Console.WriteLine("Puzzle Problem " + problemCount);
                Console.WriteLine();
                IntializePuzzle(setMatrix(num));
                stopwatch.Stop();
                Console.WriteLine("Time Taken: " + stopwatch.ElapsedMilliseconds / 1000 + " Second " + stopwatch.ElapsedMilliseconds + " Milli Second");
                Console.WriteLine();
                Console.WriteLine("Press any key to go to the next Example...");

                Console.ReadKey();
                Console.Clear();
                problemCount++;

                if (Lines.Length == 0)
                {
                    Lines = ChooseCompleteOrSample();
                    problemCount = 1;
                }
            }

        }

        private static PriorityQueue<Puzzle, int> GoalSteps(Puzzle p)
        {
            PriorityQueue<Puzzle, int> steps = new PriorityQueue<Puzzle, int>();

            while (p != null)
            {
                steps.Enqueue(p, p.g);
                p = p.parent;
            }
            return steps;
        }

        private static bool Savingpriority_queue(ref PriorityQueue<Puzzle, int> q, int[,] v, int[,] Ideal)
        {
            int g = q.Peek().g + 1; bool checkGoal = false;
            Puzzle p = new Puzzle();
            Puzzle parent = new Puzzle();
            parent = q.Peek();
            p.v = v;
            int x = q.Peek().x_zero, y = q.Peek().y_zero, ham = q.Peek().hamming, man = q.Peek().manhattan;

            if (isGoal(v, Ideal))
            {
                Console.WriteLine("That's Your Goal\n\n");
                Console.WriteLine("#Steps : " + q.Peek().g);
                Console.WriteLine();
                checkGoal = true;
                return checkGoal;
            }

            //steps.push(q.top());
            //  dic[hashFunc(p.v)] = 1;
           // string str = string.Join("", v.OfType<int>().Select((value, index) => new { value, index }).GroupBy(x => x.index / v.GetLength(1), x => x.value, (i, ints) => $"{string.Join("", ints)}"));
           
            dic.TryAdd(hashFunc(v),1);
            q.Dequeue();

            int[,] cloned = (int[,])v.Clone();
            //Going to Up
            movesUp(ref q, cloned, g, x, y, ham, man, Ideal, parent);
            cloned = (int[,])v.Clone();
            //Going to down
            movesDown(ref q, cloned, g, x, y, ham, man, Ideal, parent);
            cloned = (int[,])v.Clone();
            //Going to left
            movesLeft(ref q, cloned, g, x, y, ham, man, Ideal, parent);
            cloned = (int[,])v.Clone();
            //Going to right
            movesRight(ref q, cloned, g, x, y, ham, man, Ideal, parent);
           

            return checkGoal;
        }

        private static void movesRight(ref PriorityQueue<Puzzle, int> q, int[,] v, int g, int x, int y, int ham, int man, int[,] Ideal, Puzzle parent)
        {
            int lastX, lastY;
            lastX = x + 1;
            lastY = y;
            Puzzle p = new Puzzle();
            if (x + 1 < v.GetLength(0))
            {
                int temp = v[lastY, lastX];
                v[lastY, lastX] = v[y, x];
                v[y, x] = temp;

            //  string str = string.Join("", v.OfType<int>().Select((value, index) => new { value, index }).GroupBy(x => x.index / v.GetLength(1), x => x.value, (i, ints) => $"{string.Join("", ints)}"));
                //check duplicate
                if (!dic.ContainsKey(hashFunc(v)))
                {
                    int resAfter = Math.Abs(y - (temp - 1) / v.GetLength(0)) + Math.Abs(x - (temp - 1) % v.GetLength(0));
                    int resBefore = Math.Abs(lastY - (temp - 1) / v.GetLength(0)) + Math.Abs(lastX - (temp - 1) % v.GetLength(0));

                    //manhattan
                    if (resAfter < resBefore)
                    {
                        man--;
                        p.manhattan = man;
                    }
                    else
                    {
                        man++;
                        p.manhattan = man;
                    }
                    //hamming
                    if (v[y, x] == Ideal[y, x])
                    {
                        ham--;
                        p.hamming = ham;
                    }
                    else
                    {
                        if (temp == Ideal[lastY, lastX])
                        {
                            ham++;
                            p.hamming = ham;
                        }
                        else
                        {
                            p.hamming = ham;
                        }
                    }

                }
                else return;

                p.x_zero = lastX;
                p.y_zero = lastY;
                p.actionState = "Went Right";
                p.g = g;
                p.fHam = p.g + p.hamming;
                p.fManhattan = p.g + p.manhattan;
                p.parent = parent;
                p.v = (int[,])v.Clone();

                q.Enqueue(p, ChooseDistance(p, isHamming));
            }
        }

        private static void movesLeft(ref PriorityQueue<Puzzle, int> q, int[,] v, int g, int x, int y, int ham, int man, int[,] Ideal, Puzzle parent)
        {
            int lastX, lastY;

            lastX = x - 1;
            lastY = y;
            Puzzle p = new Puzzle();
            if (x - 1 >= 0)
            {
                int temp = v[lastY, lastX];
                v[lastY, lastX] = v[y, x];
                v[y, x] = temp;

             //  string str = string.Join("", v.OfType<int>().Select((value, index) => new { value, index }).GroupBy(x => x.index / v.GetLength(1), x => x.value, (i, ints) => $"{string.Join("", ints)}"));
                if (!dic.ContainsKey(hashFunc(v)))
                {
                    int resAfter = Math.Abs(y - (temp - 1) / v.GetLength(0)) + Math.Abs(x - (temp - 1) % v.GetLength(0));
                    int resBefore = Math.Abs(lastY - (temp - 1) / v.GetLength(0)) + Math.Abs(lastX - (temp - 1) % v.GetLength(0));

                    if (resAfter < resBefore)
                    {
                        man--;
                        p.manhattan = man;
                    }
                    else
                    {
                        man++;
                        p.manhattan = man;
                    }
                    if (v[y, x] == Ideal[y, x])
                    {
                        ham--;
                        p.hamming = ham;

                    }
                    else
                    {
                        if (temp == Ideal[lastY, lastX])
                        {
                            ham++;
                            p.hamming = ham;
                        }
                        else
                        {
                            p.hamming = ham;
                        }
                    }
                }
                else return;

                p.x_zero = lastX;
                p.y_zero = lastY;
                p.actionState = "Went Left";
                p.g = g;
                p.fHam = p.g + p.hamming;
                p.fManhattan = p.g + p.manhattan;
                p.parent = parent;
                p.v = (int[,])v.Clone();

                q.Enqueue(p, ChooseDistance(p, isHamming));
            }
        }

        private static void movesDown(ref PriorityQueue<Puzzle, int> q, int[,] v, int g, int x, int y, int ham, int man, int[,] Ideal, Puzzle parent)
        {
            int lastX, lastY;

            lastX = x;
            lastY = y + 1;
            Puzzle p = new Puzzle();
            if (y + 1 < v.GetLength(0))
            {
                int temp = v[lastY, lastX];
                v[lastY, lastX] = v[y, x];
                v[y, x] = temp;

              // string str = string.Join("", v.OfType<int>().Select((value, index) => new { value, index }).GroupBy(x => x.index / v.GetLength(1), x => x.value, (i, ints) => $"{string.Join("", ints)}"));
                if (!dic.ContainsKey(hashFunc(v)))
                {
                    int resAfter = Math.Abs(y - (temp - 1) / v.GetLength(0)) + Math.Abs(x - (temp - 1) % v.GetLength(0));
                    int resBefore = Math.Abs(lastY - (temp - 1) / v.GetLength(0)) + Math.Abs(lastX - (temp - 1) % v.GetLength(0));

                    if (resAfter < resBefore)
                    {
                        man--;
                        p.manhattan = man;
                    }
                    else
                    {
                        man++;
                        p.manhattan = man;
                    }
                    if (v[y, x] == Ideal[y, x])
                    {
                        ham--;
                        p.hamming = ham;
                    }
                    else
                    {
                        if (temp == Ideal[lastY, lastX])
                        {
                            ham++;
                            p.hamming = ham;
                        }
                        else
                        {
                            p.hamming = ham;
                        }
                    }
                }
                else return;
                p.x_zero = lastX;
                p.y_zero = lastY;
                p.actionState = "Went Down";
                p.g = g;
                p.fHam = p.g + p.hamming;
                p.fManhattan = p.g + p.manhattan;
                p.parent = parent;
                p.v = (int[,])v.Clone();

                q.Enqueue(p, ChooseDistance(p, isHamming));
            }
        }

        private static void movesUp(ref PriorityQueue<Puzzle, int> q, int[,] v, int g, int x, int y, int ham, int man, int[,] Ideal, Puzzle parent)
        {
            int lastX, lastY;

            lastX = x;
            lastY = y - 1;
            Puzzle p = new Puzzle();
            if (y - 1 >= 0)
            {
                int temp = v[lastY, lastX];
                v[lastY, lastX] = v[y, x];
                v[y, x] = temp;
              
             // string str = string.Join("", v.OfType<int>().Select((value, index) => new { value, index }).GroupBy(x => x.index / v.GetLength(1), x => x.value, (i, ints) => $"{string.Join("", ints)}"));
                if (!dic.ContainsKey(hashFunc(v)))
                {
                    int resAfter = Math.Abs(y - (temp - 1) / v.GetLength(0)) + Math.Abs(x - (temp - 1) % v.GetLength(0));
                    int resBefore = Math.Abs(lastY - (temp - 1) / v.GetLength(0)) + Math.Abs(lastX - (temp - 1) % v.GetLength(0));

                    if (resAfter < resBefore)
                    {
                        man--;
                        p.manhattan = man;
                    }
                    else
                    {
                        man++;
                        p.manhattan = man;
                    }
                    if (v[y, x] == Ideal[y, x])
                    {
                        ham--;
                        p.hamming = ham;

                    }
                    else
                    {
                        if (temp == Ideal[lastY, lastX])
                        {
                            ham++;
                            p.hamming = ham;
                        }
                        else
                        {
                            p.hamming = ham;
                        }
                    }
                }
                else return;

                p.x_zero = lastX;
                p.y_zero = lastY;
                p.actionState = "Went Up";
                p.g = g;
                p.fHam = p.g + p.hamming;
                p.fManhattan = p.g + p.manhattan;
                p.parent = parent;
                p.v = (int[,])v.Clone();
                q.Enqueue(p, ChooseDistance(p, isHamming));

            }
        }

        private static bool isGoal(int[,] v, int[,] Ideal)
        {
            for (int i = 0; i < v.GetLength(0); i++)
            {
                for (int j = 0; j < v.GetLength(1); j++)
                {
                    if (v[i, j] != Ideal[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static void DetectZero(int[,] v, ref int x, ref int y)
        {
            for (int i = 0; i < v.GetLength(0); i++)
            {
                for (int j = 0; j < v.GetLength(0); j++)
                {
                    if (v[i, j] == 0)
                    {
                        y = i;
                        x = j;
                    }
                }
            }
        }

        private static int ManhattanDistance(int[,] puzzle)
        {
            int n = puzzle.GetLength(0);
            int expected = 0;
            int x = 0, y = 0, res = 0, sum = 0;

            for (int row = 0; row < n; row++)
            {

                for (int col = 0; col < n; col++)
                {
                    int value = puzzle[row, col];
                    expected++;

                    if (value != 0)
                    {
                        x = Math.Abs(row - (value - 1) / n);
                        y = Math.Abs(col - (value - 1) % n);
                        res = x + y;
                        sum += res;
                    }
                }
            }
            return sum; ;
        }

        private static int HammaingDistance(int[,] vec1, int[,] Ideal)
        {
            int h = 0;
            for (int i = 0; i < vec1.GetLength(0); i++)
            {
                for (int j = 0; j < vec1.GetLength(0); j++)
                {
                    if (vec1[i, j] > 0 && vec1[i, j] != Ideal[i, j])
                        h++;
                }
            }
            return h;
        }

        private static int ChooseDistance(Puzzle p, bool ishamming)
        {

            if (ishamming)
            {
                return p.fHam;
            }
            else
            {
                return p.fManhattan;
            }
        }

        private static void userDistace()
        {
            Console.Clear();
            Console.WriteLine("Please Press 1 to using the Hamming Distace.");
            Console.WriteLine("Please Press 2 to using the Manthattan Distace.");
            Console.WriteLine("Please Press 3 to Back to Main Program.");
            Console.WriteLine();
            Console.Write("Please Choose of choice from the previos choices : ");
            int choice;
            choice = Convert.ToInt32(Console.ReadLine());
            if (choice == 1)
                isHamming = true;
            else if(choice == 2)
                isHamming = false;
            else if (choice == 3)
            {
                ChooseCompleteOrSample();
            }
        }

        private static int[,] SetIdeal(int[,] vec1)
        {
            int n = vec1.GetLength(0);
            int[,] Ideal = new int[n, n];
            int count = 1;
            for (int i = 0; i < vec1.GetLength(0); i++)
            {
                for (int j = 0; j < vec1.GetLength(0); j++)
                {
                    Ideal[i, j] = count;
                    count++;
                }
            }
            Ideal[n - 1, n - 1] = 0;

            return Ideal;
        }

        private static string[] ChooseFileToRead()
        {
            Console.Clear();
            int choice;
            string source = "Sample_solvable.txt";
            FileStream fs = File.OpenRead(source);
            var sr = new StreamReader(fs);
            string[] data = new string[File.ReadLines(source).Count()];
            Console.WriteLine("Press 1 to see the SOLVABLE PUZZLES.");
            Console.WriteLine("Press 2 to see the UNSOLVABLE Puzzles.");
            Console.WriteLine("Press 3 to go to Main program.");
            Console.WriteLine();
            Console.Write("Choose one choice of the pervious choices :  ");
            choice = Convert.ToInt16(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    source = "Sample_solvable.txt";
                    fs = File.OpenRead(source);
                    sr = new StreamReader(fs);
                    data = new string[File.ReadLines(source).Count()];
                    for (int i = 0; i < File.ReadLines(source).Count(); i++)
                    {
                        data[i] = sr.ReadLine();
                    }
                    userDistace();
                    break;

                case 2:
                    source = "Sample_Unsolvable.txt";
                    fs = File.OpenRead(source);
                    sr = new StreamReader(fs);
                    data = new string[File.ReadLines(source).Count()];
                    for (int i = 0; i < File.ReadLines(source).Count(); i++)
                    {
                        data[i] = sr.ReadLine();
                    }
                    break;
                case 3:
                    ChooseCompleteOrSample();
                    break;

                default:
                    //   Console.WriteLine("GameOver");
                    break;
            };
            return data;

        }

        private static string[] ChooseFileToReadComplete()
        {
            Console.Clear();
            int choice;
            string source = "Sample_solvable.txt";
            FileStream fs = File.OpenRead(source);
            var sr = new StreamReader(fs);
            string[] data = new string[File.ReadLines(source).Count()];
            Console.WriteLine("Press 1 to see the Manhatten only PUZZLES.");
            Console.WriteLine("Press 2 to see the Manhatten and Hamming Puzzles.");
            Console.WriteLine("Press 3 to see the very large Puzzles.");
            Console.WriteLine("Press 4 to see the UnSolvable Puzzles.");
            Console.WriteLine("Press 5 to go to Main program.");
            Console.WriteLine();
            Console.Write("Choose one choice of the pervious choices :  ");
            choice = Convert.ToInt16(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    source = "Complete_ManhattanOnly_Solvable.txt";
                    fs = File.OpenRead(source);
                    sr = new StreamReader(fs);
                    data = new string[File.ReadLines(source).Count()];
                    for (int i = 0; i < File.ReadLines(source).Count(); i++)
                    {
                        data[i] = sr.ReadLine();
                    }
                    isHamming = false;
                    break;

                case 2:
                    source = "Complete_Manhattan&Hamming_Solvable.txt";
                    fs = File.OpenRead(source);
                    data = new string[File.ReadLines(source).Count()];
                    sr = new StreamReader(fs);
                    for (int i = 0; i < File.ReadLines(source).Count(); i++)
                    {
                        data[i] = sr.ReadLine();
                    }
                    userDistace();
                    break;

                case 3:
                    source = "Comlete_VeryLarge_Case.txt";
                    fs = File.OpenRead(source);
                    sr = new StreamReader(fs);
                    data = new string[File.ReadLines(source).Count()];
                    for (int i = 0; i < File.ReadLines(source).Count(); i++)
                    {
                        data[i] = sr.ReadLine();
                    }
                    isHamming = false;
                    break;


                case 4:
                    source = "Complete_Unsolvable.txt";
                    fs = File.OpenRead(source);
                    sr = new StreamReader(fs);
                    data = new string[File.ReadLines(source).Count()];
                    for (int i = 0; i < File.ReadLines(source).Count(); i++)
                    {
                        data[i] = sr.ReadLine();
                    }
                    break;
                case 5:
                    ChooseCompleteOrSample();
                    break;
                default:
                    //   Console.WriteLine("GameOver");
                    break;
            };
            return data;
        }

        private static string[] ChooseCompleteOrSample() {
            Console.Clear();
            int choice;
            Console.WriteLine("Press 1 to run COMPLETE TESTS.");
            Console.WriteLine("Press 2 to run SAMPLE TESTS.");
            Console.WriteLine("Press 3 to run User's TESTS.");
            Console.WriteLine("Press 4 Exit TESTS.");
            Console.WriteLine();
            choice = Convert.ToInt16(Console.ReadLine());

            switch (choice) { 
               case 1:
                    return ChooseFileToReadComplete();
               case 2:
                    return ChooseFileToRead(); 
                case 3:
                   string source = "User's Gmae.txt";
                   FileStream fs = File.OpenRead(source);
                   var sr = new StreamReader(fs);
                   string[] data = new string[File.ReadLines(source).Count()];
                    for (int i = 0; i < File.ReadLines(source).Count(); i++)
                    {
                        data[i] = sr.ReadLine();
                    }
                    userDistace();
                    return data;
                case 4:
                    Environment.Exit(0);
                  break;
            }

            return null;
        }
    }
}
//adding exit
//comparison

