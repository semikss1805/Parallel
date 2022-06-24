import java.util.Random;

public class Main {

    public static void main(String[] args) {
        int dim = 10000000;
        int threadNum = 4;
        ArrClass arrClass = new ArrClass(dim, threadNum);
        for (var param :
                arrClass.partMin(0,dim)) {
            System.out.println(param);
        }

        for (var param :
                arrClass.threadMin()) {
            System.out.println(param);
        }
    }
}

class ArrClass {
    private final int dim;
    private final int threadNum;
    public final int[] arr;

    private int min = Integer.MAX_VALUE;
    private int minIndex = -1;

    public ArrClass(int dim, int threadNum) {
        this.dim = dim;
        arr = new int[dim];
        this.threadNum = threadNum;
        for(int i = 0; i < dim; i++){
            arr[i] = i;
        }
        var random = new Random();
        arr[random.nextInt(0, dim)] = -1;
    }

    public int[] partMin(int startIndex, int finishIndex){
        int min = Integer.MAX_VALUE;
        int minIndex = -1;
        for(int i = startIndex; i < finishIndex; i++){
            if (arr[i] < min){
                min = arr[i];
                minIndex = i;
            }
        }
        int[] res = new int[]{min, minIndex};
        return res;
    }


    synchronized private int[] getMin() {
        while (getThreadCount()<threadNum){
            try {
                wait();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
        int[] res = new int[]{min, minIndex};
        return res;
    }

    synchronized public void collectMin(int[] res){
        if (res[0] < min) {
            this.min = res[0];
            this.minIndex = res[1];
        }
    }

    private int threadCount = 0;
    synchronized public void incThreadCount(){
        threadCount++;
        notify();
    }

    private int getThreadCount() {
        return threadCount;
    }

    public int[] threadMin(){
        ThreadMin[] threadMin = new ThreadMin[threadNum];
        for(int i = 0; i < threadNum; i++)
        {
            int step = dim / threadNum;
            threadMin[i] = new ThreadMin(i*step, (i+1)*step, this);
            threadMin[i].start();
        }

        return getMin();
    }
}

class ThreadMin extends Thread{
    private final int startIndex;
    private final int finishIndex;
    private final ArrClass arrClass;

    public ThreadMin(int startIndex, int finishIndex, ArrClass arrClass) {
        this.startIndex = startIndex;
        this.finishIndex = finishIndex;
        this.arrClass = arrClass;
    }

    @Override
    public void run() {
        int[] res = arrClass.partMin(startIndex, finishIndex);
        arrClass.collectMin(res);
        arrClass.incThreadCount();
    }
}
