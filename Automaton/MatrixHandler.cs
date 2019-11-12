using System;

namespace Automaton
{
    internal class MatrixHandler
    {
        public int[,] AddLine(int[] newLine, int[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            int[,] result = new int[n + 1, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    result[i, j] = matrix[i, j];
                }
            }
            for (int k = 0; k < newLine.Length; k++)
            {
                result[result.GetLength(0) - 1, k] = newLine[k];
            }
            return result;
        }

        public void ShowMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public int[,] AddColumn(int[] newColumn, int[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            int[,] result = new int[n, m + 1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    result[i, j] = matrix[i, j];
                }
            }
            for (int k = 0; k < newColumn.Length; k++)
            {
                result[k, result.GetLength(1) - 1] = newColumn[k];
            }
            return result;
        }
    }
}