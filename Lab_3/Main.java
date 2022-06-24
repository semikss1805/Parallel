import java.util.ArrayList;
import java.util.concurrent.Semaphore;

public class Main {

    public static void main(String[] args) {
        Main main = new Main();
        int storageSize = 3;
        int itemNumbers = 10;
        int producers = 3;
        int consumers = 6;
        main.starter(storageSize, itemNumbers, producers, consumers);
    }

    private void starter(int storageSize, int itemNumbers, int producers, int consumers) {
        Manager manager = new Manager(storageSize);

        for (int i = 0; i < producers - 1; i++)
        {
            new Producer(itemNumbers / producers, manager);
        }
        new Producer(itemNumbers - itemNumbers / producers * (producers - 1), manager);

        for (int i = 0; i < consumers - 1; i++)
        {
            new Consumer(itemNumbers/consumers, manager);
        }
        new Consumer(itemNumbers - itemNumbers / consumers * (consumers - 1), manager);
    }
}

class Producer implements Runnable{
    private final int itemNumbers;
    private final Manager manager;

    public Producer(int itemNumbers, Manager manager) {
        this.itemNumbers = itemNumbers;
        this.manager = manager;

        new Thread(this).start();
    }

    @Override
    public void run() {
        for (int i = 0; i < itemNumbers; i++) {
            try {
                manager.full.acquire();
                manager.access.acquire();

                manager.storage.add("item " + i);
                System.out.println("Added item " + i);

                manager.access.release();
                manager.empty.release();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}

class Consumer implements Runnable {
    private final int itemNumbers;
    private final Manager manager;

    public Consumer(int itemNumbers, Manager manager) {
        this.itemNumbers = itemNumbers;
        this.manager = manager;

        new Thread(this).start();
    }

    @Override
    public void run() {
        for (int i = 0; i < itemNumbers; i++) {
            String item;
            try {
                manager.empty.acquire();
                Thread.sleep(1000);
                manager.access.acquire();

                item = manager.storage.get(0);
                manager.storage.remove(0);
                System.out.println("Took " + item);

                manager.access.release();
                manager.full.release();

            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}

class Manager {

    public Semaphore access;
    public Semaphore full;
    public Semaphore empty;

    public ArrayList<String> storage = new ArrayList<>();

    public Manager(int storageSize) {
        access = new Semaphore(1);
        full = new Semaphore(storageSize);
        empty = new Semaphore(0);
    }
}