class Program
{
    private static readonly int dim = 10000000;
    private static readonly int threadNum = 4;
    private int min = int.MaxValue;
    private int minIndex = -1; 

    private readonly Thread[] thread = new Thread[threadNum];

    static void Main(string[] args)
    {
        Program main = new Program();
        main.InitArr();
        foreach(var param in main.PartMin(0, dim))
        {
            Console.WriteLine(param);
        }

        foreach (var param in main.ParallelMin())
        {
            Console.WriteLine(param);
        }
        Console.ReadKey();
    }

    private int threadCount = 0;

    private int[] ParallelMin()
    {
        for(int i = 0; i < threadNum; i++)
        {
            int step = dim / threadNum;
            thread[i] = new Thread(StarterThread);
            thread[i].Start(new Bound(i*step, (i+1)*step));
        }

        lock (lockerForCount)
        {
            while (threadCount < threadNum)
            {
                Monitor.Wait(lockerForCount);
            }
        }
        int[] result = new int[] {min, minIndex};
        return result;
    }

    private readonly int[] arr = new int[dim];

    private void InitArr()
    {
        for (int i = 0; i < dim; i++)
        {
            arr[i] = i;
        }
        var random = new Random();
        arr[random.NextInt64(0, dim - 1)] = -1;
    }

    class Bound
    {
        public Bound(int startIndex, int finishIndex)
        {
            StartIndex = startIndex;
            FinishIndex = finishIndex;
        }

        public int StartIndex { get; set; }
        public int FinishIndex { get; set; }
    }

    private readonly object lockerForMin = new object();
    private void StarterThread(object param)
    {
        if (param is Bound)
        {
            int[] result = PartMin((param as Bound).StartIndex, (param as Bound).FinishIndex);

            lock (lockerForMin)
            {
                CollectMin(result);
            }
            IncThreadCount();
        }
    }

    private readonly object lockerForCount = new object();
    private void IncThreadCount()
    {
        lock (lockerForCount)
        {
            threadCount++;
            Monitor.Pulse(lockerForCount);
        }
    }

    public void CollectMin(int[] result)
    {
        if (result[0] < this.min)
        {
            this.min = result[0];
            this.minIndex = result[1];
        }
    }

    public int[] PartMin(int startIndex, int finishIndex)
    {
        int min = int.MaxValue;
        int minIndex = -1;
        for (int i = startIndex; i < finishIndex; i++)
        {
            if(arr[i] < min)
            {
                minIndex = i;
                min = arr[i];
            }
        }
        int[] result = new int[] {min, minIndex};
        return result;
    }
}
