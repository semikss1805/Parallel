
class Program
{
    private bool canStop = false;

    static void Main(string[] args)
    {
        (new Program()).Start();
    }

    void Start()
    {
        new Thread(() => Calculator(1)).Start();

        new Thread(() => Calculator(2)).Start();

        new Thread(() => Calculator(3)).Start();


        (new Thread(Stoper)).Start();
    }

    void Calculator(int step)
    {
        long sum = 0;
        long amount = 0;
        do
        {
            sum += step;
            amount++;
        } while (!canStop);

        Console.WriteLine($"{Environment.CurrentManagedThreadId} сума: {sum} крок: {step} кiлькiсть доданкiв: {amount}");
    }

    public void Stoper()
    {
        Thread.Sleep(15 * 1000);
        canStop = true;
    }

}