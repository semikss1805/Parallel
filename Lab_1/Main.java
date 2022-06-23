public class Main {

    public static void main(String[] args) {
        BreakThread breakThread = new BreakThread();

        new MainThread(1, breakThread,1).start();
        new MainThread(2, breakThread, 2).start();
        new MainThread(3, breakThread,3).start();

        new Thread(breakThread).start();
    }
}

class BreakThread implements Runnable{
    private boolean canBreak = false;
    @Override
    public void run() {
        try {
            Thread.sleep(15 * 1000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        canBreak = true;
    }

    synchronized public boolean isCanBreak() {
        return canBreak;
    }
}

class MainThread extends Thread{
    private final int id;
    private final BreakThread breakThread;
    private final int step;

    public MainThread(int id, BreakThread breakThread, int step) {
        this.id = id;
        this.breakThread = breakThread;
        this.step = step;
    }

    @Override
    public void run() {
        long sum = 0;
        long amount = 0;
        boolean isStop = false;

        do{
            sum += step;
            amount++;
            isStop = breakThread.isCanBreak();
        } while (!isStop);

        System.out.println(id + " sum:" + sum + " step:" + step + " amount of steps:" + amount);
    }
}


