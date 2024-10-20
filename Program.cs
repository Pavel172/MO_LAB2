using System;
using System.Runtime.InteropServices;

namespace MO_LAB2
{
    public class Programm
    {
        public static void Main()
        {
            Console.WriteLine("F = cx -> max/min; Ax <=/>=/= b; x >= 0"); //условие задачи
            double[] vec_c = { 1, 3, 8 };
            double[] vec_b = { 7, 2, 4 };
            double[][] matrix_A = { new double[] { 1, 1, 1 }, new double[] { 1, 1, 0 }, new double[] { 0, 0.5, 2 } };
            string aspiration = "max";
            string[] chars = { "<=", "<=", "<=" };
            Console.WriteLine("\nПрямая задача: "); 
            try //запускаем симплекс метод и ищем оптимальное решение, если оно существует
            {
                X.Simplex_method(vec_c, vec_b, matrix_A, aspiration, chars);
            }
            catch (Exception exep) //если не существует опорного или оптимального решения, то прекращаем поиск и выводим сообщение об ошибке
            {
                Console.WriteLine(exep.Message);
                return;
            }
            Console.WriteLine("\n\nДвойственная задача: ");
            double[] vec_c_dual = vec_b; //меняем вектора местами для двойственной задачи
            double[] vec_b_dual = vec_c;
            string aspiration_dual = "max";
            if (aspiration == "max") aspiration_dual = "min"; //при необходимости меняем стремление функции
            double[][] matrix_A_dual = new double[matrix_A[0].Length][];
            for(int i = 0; i < matrix_A_dual.Length; ++i) //создаем матрицу A двойственной задачи на основе матрицы прямой задачи
            {
                matrix_A_dual[i] = new double[matrix_A.Length];
                for(int j = 0; j < matrix_A_dual[i].Length; ++j) 
                {
                    matrix_A_dual[i][j] = matrix_A[j][i];
                }
            }
            string[] chars_dual = new string[matrix_A_dual.Length];
            for (int i = 0; i < matrix_A_dual.Length; i++) //создаем массив знаков сравнений на основе массива для прямой задачи
            {
                if(chars.Distinct().Count() == 1 && matrix_A_dual.Length > matrix_A.Length) 
                {
                    if (chars[0] == ">=") chars_dual[i] = "<=";
                    if (chars[0] == "<=") chars_dual[i] = ">=";
                    continue;
                }
                if (chars[i] == ">=") chars_dual[i] = "<=";
                if (chars[i] == "<=") chars_dual[i] = ">=";
                if (chars[i] == "==") chars_dual[i] = "==";
            }
            try
            {
                X.Simplex_method(vec_c_dual, vec_b_dual, matrix_A_dual, aspiration_dual, chars_dual);
            }
            catch (Exception exep)
            {
                Console.WriteLine(exep.Message);
                return;
            }
        }
    }
}