using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

    class Program
    {
        private const int count = 5;

        static void Main(string[] args)
        {

            var philosophers = InitializePhilosophers();


            Console.WriteLine("Обiд розпочато!");

            var philosopherThreads = new List<Thread>();
            foreach (var philosopher in philosophers)
            {
                var philosopherThread = new Thread(new ThreadStart(philosopher.EatAll));
                philosopherThreads.Add(philosopherThread);
                philosopherThread.Start();
            }

            foreach (var thread in philosopherThreads)
            {
                thread.Join();
            }


            Console.WriteLine("Обiд завершено!");
        }

        private static List<Philosopher> InitializePhilosophers()
        {

            var philosophers = new List<Philosopher>(count);
            for (int i = 0; i < count; i++)
            {
                philosophers.Add(new Philosopher(philosophers, i));
            }

            foreach (var philosopher in philosophers)
            {
                philosopher.LeftFork = philosopher.LeftPhilosopher.RightFork ?? new Fork();

                philosopher.RightFork = philosopher.RightPhilosopher.LeftFork ?? new Fork();
            }

            return philosophers;
        }
    }

    [DebuggerDisplay("Name = {Name}")]
    public class Philosopher
    {
        private const int TIMES_TO_EAT = 5;
        private int _timesEaten = 0;
        private readonly List<Philosopher> _allPhilosophers;
        private readonly int _index;

        public Philosopher(List<Philosopher> allPhilosophers, int index)
        {
            _allPhilosophers = allPhilosophers;
            _index = index;
            this.Name = string.Format("Фiлософ {0}", _index);
            this.State = State.Thinking;
        }

        public string Name { get; private set; }
        public State State { get; private set; }
        public Fork LeftFork { get; set; }
        public Fork RightFork { get; set; }

        public Philosopher LeftPhilosopher
        {
            get
            {
                if (_index == 0)
                    return _allPhilosophers[_allPhilosophers.Count - 1];
                else
                    return _allPhilosophers[_index - 1];
            }
        }

        public Philosopher RightPhilosopher
        {
            get
            {
                if (_index == _allPhilosophers.Count - 1)
                    return _allPhilosophers[0];
                else
                    return _allPhilosophers[_index + 1];
            }
        }

        public void EatAll()
        {

            while (_timesEaten < TIMES_TO_EAT)
            {
                this.Think();
                if (this.PickUp())
                {
                    this.Eat();

                    this.PDLeft();
                    this.PDRight();
                }
            }
        }

        private bool PickUp()
        {
            if (Monitor.TryEnter(this.LeftFork))
            {
                Console.WriteLine(this.Name + " обрав лiву виделку.");

                if (Monitor.TryEnter(this.RightFork))
                {
                    Console.WriteLine(this.Name + " обрав праву виделку.");


                    return true;
                }
                else
                {
                    this.PDLeft();
                }
            }


            return false;
        }

        private void Eat()
        {
            this.State = State.Eating;
            _timesEaten++;
            Console.WriteLine(this.Name + " обiдає.");
        }

        private void PDLeft()
        {
            Monitor.Exit(this.LeftFork);
            Console.WriteLine(this.Name + " поклав лiву виделку.");
        }

        private void PDRight()
        {
            Monitor.Exit(this.RightFork);
            Console.WriteLine(this.Name + " поклав праву виделку.");
        }


        private void Think()
        {
            this.State = State.Thinking;
        }
    }

    public enum State
    {
        Thinking = 0,
        Eating = 1
    }

    [DebuggerDisplay("Name = {Name}")]
    public class Fork
    {
        private static int _count = 1;
        public string Name { get; private set; }

        public Fork()
        {
            this.Name = "Виделка " + _count++;
        }
    }