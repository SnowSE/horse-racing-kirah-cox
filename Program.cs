public class Program
{
    const int StepsToWin = 50;
    public static bool RaceOver = false;
    public static object RandomLock = new object();

    static void Main()
    {
        List<Horse> horses = new List<Horse>();
        List<Thread> threads = new List<Thread>();

        Console.CursorVisible = false;

        for (int i = 0; i < 4; i++)
        {
            horses.Add(new Horse(i + 1));

            Thread horse = new Thread(horses[i].TakeStep);
            threads.Add(horse);
            horse.Start();
        }

        while (!RaceOver)
        {
            for (int i = 0; i < 4; i++)
            {
                horses[i].GoHorse.Set();
            }

            for (int i = 0; i < 4; i++)
            {
                horses[i].GoMain.WaitOne();
            }

            Console.SetCursorPosition(0, 0);

            Console.WriteLine("Welcome to the Racing Program:");

            foreach (var horse in horses)
            {
                Console.Write($"Horse {horse.Number}:");
                for (int i = 0; i < horse.StepsTaken; i++)
                {
                    Console.Write("-");
                }
                Console.WriteLine();
            }

            Console.WriteLine();

            foreach (var horse in horses)
            {
                if (horse.StepsTaken >= StepsToWin)
                {
                    Console.WriteLine($"Horse {horse.Number} wins!");
                    RaceOver = true;
                    horse.GoHorse.Set();
                }
            }
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}

public class Horse
{
    public int Number { get; set; }
    public int StepsTaken { get; set; }

    public AutoResetEvent GoMain = new AutoResetEvent(false);
    public AutoResetEvent GoHorse = new AutoResetEvent(false);

    Random random = new Random();

    public Horse(int number)
    {
        Number = number;
        StepsTaken = 0;
    }

    public void TakeStep()
    {
        while (!Program.RaceOver)
        {
            int randomNumber;

            lock (Program.RandomLock)
            {
                randomNumber = random.Next(1, 1001);
            }

            GoHorse.WaitOne();
            if (Program.RaceOver) break;
            if (randomNumber == 1)
            {
                StepsTaken++;
            }
            GoMain.Set();
        }
    }
}