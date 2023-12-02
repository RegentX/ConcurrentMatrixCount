// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

namespace AsyncLabMinor3;

class Program
{
    static void Main(string[] args)
    {
        int N = 11;
        int[,] matrix = GenerateMatrix(N);

        // Матрица для проверки :))
        // int rows = 10; 
        // int cols = 10; 
        // int[,] matrix = new int[rows, cols];
        //
        // for (int col = 0; col < cols; col++)
        // {
        //     matrix[0, col] = col + 1; 
        // }
        //
        // for (int row = 1; row < rows; row++)
        // {
        //     for (int col = 0; col < cols; col++)
        //     {
        //         matrix[row, col] = matrix[0, col];
        //     }
        // }
        //конец матрицы для проверки :((
        
        Stopwatch stopwatch = new Stopwatch();
        
        stopwatch.Start();
        int determinant = CalculateDeterminantParallel(matrix, N);
        stopwatch.Stop();
        TimeSpan elapsed = stopwatch.Elapsed;
        Console.WriteLine($"Время многопоточного подсчета определителя матрицы {elapsed.Minutes} : {elapsed.Seconds}");
        Console.WriteLine($"Определитель матрицы: {determinant}");
        
        stopwatch.Start();
        int determinant2 = CalculateDeterminantUsual(matrix, N);
        stopwatch.Stop();
        TimeSpan elapsed2 = stopwatch.Elapsed;
        Console.WriteLine($"Время обычного подсчета определителя матрицы {elapsed2.Minutes} : {elapsed2.Seconds}");
        Console.WriteLine($"Определитель матрицы: {determinant2}");
    }

    static int[,] GenerateMatrix(int size)
    {
        int[,] matrix = new int[size, size];
        Random random = new Random();

        for (int i = 0; i < size; i++)
        for (int j = 0; j < size; j++)
            matrix[i, j] = random.Next(3);

        return matrix;
    }

    static int CalculateDeterminantParallel(int[,] matrix, int size)
    {
        if (size == 1) return matrix[0, 0];
        if (size == 2) return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

        int determinant = 0;
        object lockObj = new object();

        Parallel.For(0, size, i =>
        {
            int[,] minor = GetMinorMatrix(matrix, size, i);
            int minorDeterminant = CalculateDeterminant(minor, size - 1);

            
            int prom = matrix[0, i] * minorDeterminant * (i % 2 == 0 ? 1 : -1);
            Interlocked.Add(ref determinant, prom);
        });

        return determinant;
    }

    static int CalculateDeterminant(int[,] matrix, int size)
    {
        if (size == 1) return matrix[0, 0];
        if (size == 2) return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

        int determinant = 0;

        Parallel.For(0, size, i =>
        {
            int[,] minor = GetMinorMatrix(matrix, size, i);
            int minorDeterminant = CalculateDeterminant(minor, size - 1);
            // determinant += matrix[0, i] * CalculateDeterminant(minor, size - 1) * (i % 2 == 0 ? 1 : -1);
            
            int prom = matrix[0, i] * minorDeterminant * (i % 2 == 0 ? 1 : -1);
            Interlocked.Add(ref determinant, prom);
        });

        return determinant;
    }

    static int[,] GetMinorMatrix(int[,] matrix, int size, int n)
    {
        int[,] minor = new int[size - 1, size - 1];
        for (int i = 1; i < size; i++)
        {
            for (int j = 0, col = 0; j < size; j++)
            {
                if (j == n) continue;
                minor[i - 1, col++] = matrix[i, j];
            }
        }

        return minor;
    }


    static int CalculateDeterminantUsual(int[,] matrix, int size)
    {
        if (size == 1) return matrix[0, 0];
        if (size == 2) return matrix[0, 0] * matrix[1, 1] - matrix[1, 0] * matrix[0, 1];

        int determinant = 0;
        for (int i = 0; i < size; i++)
        {
            int[,] minorMatrix = GetMinorMatrix(matrix, size, i);
            int minorDeterminant = CalculateDeterminantUsual(minorMatrix, size - 1);
            determinant += matrix[0, i] * minorDeterminant * (i % 2 == 0 ? 1 : -1);
        }

        return determinant;
    }

  
}