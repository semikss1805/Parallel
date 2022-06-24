class Program
{
    static void Main(string[] args)
    {
        Program program = new Program();
        program.Starter(4, 16, 4, 8);

        Console.ReadKey();
    }


    private void Starter(int storageSize, int itemNumbers, int producers, int consumers)
    {
        Access = new Semaphore(1, 1);
        Full = new Semaphore(storageSize, storageSize);
        Empty = new Semaphore(0, storageSize);

        for (int i = 0; i < producers - 1; i++)
        {
            new Thread(Producer).Start(itemNumbers/ producers);
        }
        (new Thread(Producer)).Start(itemNumbers - itemNumbers / producers * (producers - 1));

        for (int i = 0;i < consumers - 1; i++)
        {
            new Thread(Consumer).Start(itemNumbers/ consumers);
        }
        (new Thread(Consumer)).Start(itemNumbers - itemNumbers / consumers * (consumers - 1));
    }

    private Semaphore Access;
    private Semaphore Full;
    private Semaphore Empty;

    private readonly List<string> storage = new List<string>();


    private void Producer(Object itemNumbers)
    {
        int maxItem = 0;
        if (itemNumbers is int)
        {
            maxItem = (int)itemNumbers;
        }
        for (int i = 0; i < maxItem; i++)
        {
            Full.WaitOne();
            Access.WaitOne();

            storage.Add("item " + i);
            Console.WriteLine("Added item " + i);

            Access.Release();
            Empty.Release();

        }
    }

    private void Consumer(Object itemNumbers)
    {
        int maxItem = 0;
        if (itemNumbers is int)
        {
            maxItem = (int)itemNumbers;
        }
        for (int i = 0; i < maxItem; i++)
        {
            Empty.WaitOne();
            Thread.Sleep(1000);
            Access.WaitOne();

            string item = storage.ElementAt(0);
            storage.RemoveAt(0);

            //------------------
            //Normaly it mast be changed
            Full.Release();



            Access.Release();
            //-------------

            Console.WriteLine("Took " + item);
        }
    }
}