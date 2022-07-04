
#include<iostream>
#include<locale>
#include"omp.h"

using namespace std;

const int arr_size = 100;
const int arr_size2 = 200;
int arr[arr_size][arr_size2];

void init_arr();
long long parallel_sum(int, int, int, int, int);
long long parallel_min(int, int, int, int, int);

int main() {
    init_arr();

    omp_set_nested(1);
    double t1 = omp_get_wtime();
#pragma omp parallel sections
    {
#pragma omp section
        {   
        cout << "sum 1 = " << parallel_sum(0, arr_size, 0, arr_size2, 1) << endl;
        cout << "sum 2 = " << parallel_sum(0, arr_size, 0, arr_size2, 2) << endl;
        cout << "sum 3 = " << parallel_sum(0, arr_size, 0, arr_size2, 3) << endl;
        cout << "sum 4 = " << parallel_sum(0, arr_size, 0, arr_size2, 4) << endl;
        cout << "sum 8 = " << parallel_sum(0, arr_size, 0, arr_size2, 8) << endl;
        cout << "sum 16 = " << parallel_sum(0, arr_size, 0, arr_size2, 16) << endl;
        }

#pragma omp section
        {
            cout << "min row 1 = " << parallel_min(0, arr_size, 0, arr_size2, 1) << endl;
            cout << "min row 2 = " << parallel_min(0, arr_size, 0, arr_size2, 2) << endl;
            cout << "min row 3 = " << parallel_min(0, arr_size, 0, arr_size2, 3) << endl;
            cout << "min row 4 = " << parallel_min(0, arr_size, 0, arr_size2, 4) << endl;
            cout << "min row 8 = " << parallel_min(0, arr_size, 0, arr_size2, 8) << endl;
            cout << "min row 16 = " << parallel_min(0, arr_size, 0, arr_size2, 16) << endl;
        }
    }


    double t2 = omp_get_wtime();
    cout << "Total time - " << t2 - t1 << " seconds" << endl;
    return 0;
}

void init_arr() {
    for (int i = 0; i < arr_size; i++) {
        for (int j = 0; j < arr_size2; j++) {
            arr[i][j] = i + j;
        }
    }
    arr[arr_size / 2][arr_size2 / 2] = -1;
}

long long parallel_sum(int start_index, int finish_index, int start_index2, int finish_index2, int num_threads) {
    long long sum = 0;
    double t1 = omp_get_wtime();
#pragma omp parallel for reduction(+:sum) num_threads(num_threads)  
    for (int i = start_index; i < finish_index; i++) {
        for (int j = start_index2; j < finish_index2; j++) {
            sum += arr[i][j];
        }

    }

    double t2 = omp_get_wtime();
    cout << "Total sum of all elements, " << "threads worked - " << num_threads << ", time - " << t2 - t1 << " seconds" << endl;

    return sum;
}

long long parallel_min(int start_index, int finish_index, int start_index2, int finish_index2, int num_threads) {
    int arrSum[arr_size];
    double t1 = omp_get_wtime();
    long long sum;
    int minSum = 0;
    int rowMinIndex = -1;
#pragma omp parallel for num_threads(num_threads)

    for (int i = start_index; i < finish_index; i++) {
        sum = 0;
        for (int j = start_index2; j < finish_index2; j++) {
            sum += arr[i][j];
        }
        arrSum[i] = sum;
        if (arrSum[i] < minSum)
        {
#pragma omp critical
            if (arrSum[i] < minSum)
            {
                minSum = arrSum[i];
                rowMinIndex = i;
            }
        }
    }

    double t2 = omp_get_wtime();

    cout << "The minimum sum is in a row " << rowMinIndex + 1 << ", threads worked - " << num_threads << ", time - " << t2 - t1 << " seconds" << endl;

    return minSum;
}