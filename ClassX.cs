using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class X
{
    public int name { get; set; }
    public double value { get; set; }

    public X(int name, double value)
    {
        this.name = name;
        this.value = value;
    }

    public static void print_table(ref List<List<double>> simplex_table, List<int> basis, List<int> not_basis) //метод вывода симплекс таблицы в консоль
    {
        string temp = "";
        for (int i = 0; i < simplex_table.Count; ++i)
        {
            if (i == 0) //вывод "шапки" симплекс таблицы
            {
                for (int k = 0; k < not_basis.Count; ++k)
                {
                    if (k == 0) Console.Write("\t Sio");
                    Console.Write($"\t X{not_basis[k]}");
                }
                Console.WriteLine();
            }
            temp = "";
            for (int j = 0; j < simplex_table[i].Count; ++j) //вывод всех остальных строк симплекс таблицы
            {
                if (j == 0 && i != simplex_table.Count - 1) Console.Write($"   X{basis[i]}");
                if (j == 0 && i == simplex_table.Count - 1) Console.Write($"   F");
                if (simplex_table[i][j] == -0) simplex_table[i][j] = 0;
                if (simplex_table[i][j] < 0) Console.Write($"\t{Math.Round(simplex_table[i][j], 1)}");
                if (simplex_table[i][j] >= 0) Console.Write($"\t {Math.Round(simplex_table[i][j], 1)}");
            }
            Console.WriteLine();
        }
        Console.WriteLine("\t");
    }

    public static bool Finding_a_reference_solution(ref List<List<double>> simplex_table, ref List<X> free_memb, ref List<int> basis, ref List<int> not_basis, ref int sign) //метод поиска опорного решения
    {
        int column = 0, negative_numbers = 0;
        for (int i = 0; i < simplex_table.Count - 1; ++i) //-1, так как в строке F значение может быть < 0
        {
            if (simplex_table[i][0] < 0) //ищем отрицательный элемент в столбце свободных членов
            {
                ++negative_numbers;
                for (int j = 1; j < simplex_table[i].Count; ++j)
                {
                    if (simplex_table[i][j] < 0 && sign == 1) //пропускаем столбец, если он приводит к зацикливанию поиска
                    {
                        sign = 0;
                        continue;
                    }
                    if (simplex_table[i][j] < 0) //ищем разрешающий столбец
                    {
                        column = j;
                        simplex_table = solution(ref simplex_table, ref free_memb, ref basis, ref not_basis, column); //меняем текущую симплекс таблицу на новую
                        return false; //выходим из функции, чтобы заново её запустить и проверить, получили ли мы опорное решение
                    }
                    //если разрешающий столбец не нашелся, то ищем следующий отрицательный элемент
                }
            }
        }
        if (negative_numbers != 0) throw new Exception("Опорного решения не существует"); //если в столбце свободных членов есть отрицательные элементы, но разрешающий столбец не находится, то значит решений нет
        else return true; //если отрицательных элементов в столбце свободных членов нет, то значит мы нашли опорное решение
    }

    public static int Finding_a_optimal_solution(ref List<List<double>> simplex_table, ref List<X> free_memb, ref List<int> basis, ref List<int> not_basis, ref int sign) //метод поиска оптимального решения
    {
        int column;
        for (int g = 0; g < simplex_table.Count - 1; ++g) //проверяем, не перестало ли наше решение быть опорным
        {
            sign = 1; //ставим знак на 1, чтобы не допустить зацикливания поиска решения(не будем принимать разрешающим столбец, при котором решение перестает быть опорным)
            if (simplex_table[g][0] < 0) return 2; //если решение перестает быть опорным, то возвращаемся к поиску опорного решения
        }
        for (int i = 1; i < simplex_table[0].Count; ++i) //перебираем все элементы строки функции F, кроме первого(так как он - свободный член)
        {
            if (simplex_table[simplex_table.Count - 1][i] > 0) //ищем положительный элемент в строке функции F
            {
                column = i;
                simplex_table = solution(ref simplex_table, ref free_memb, ref basis, ref not_basis, column); //меняем текущую симплекс таблицу на новую
                return 0; //выходим из функции, чтобы заново её запустить и проверить, получили ли мы оптимальное решение
            }
        }
        return 1; //если мы не нашли положительных элементов в строке F, то значит решение оптимальное
    }

    private static List<List<double>> solution(ref List<List<double>> simplex_table, ref List<X> free_memb, ref List<int> basis, ref List<int> not_basis, int column) //прохождение одной итерации с симплекс таблицей
    {
        int line = 0;
        double min_positive = 1000000000;
        List<double> ratios = new List<double>();
        for (int k = 0; k < simplex_table.Count - 1; ++k) //составляем список из отношений свободных членов
        {                                                 //к соответственным членам разрешающего столбца
            ratios.Add(simplex_table[k][0] / simplex_table[k][column]);
        }
        for (int k = 0; k < ratios.Count; ++k) //определяем минимальное положительное число в списке
        {
            if (ratios[k] > 0 && ratios[k] < min_positive) min_positive = ratios[k];
        }
        if (min_positive == 1000000000) throw new Exception("Положительных чисел среди отношений элементов нет -> решения не существует"); //решений нет, так как нам не получится найти разрешающую строку
        for (int k = 0; k < ratios.Count; ++k) //определяем строку, в которой находится минимальное положительное число в списке
        {                                      //эта строка будет разрешающей
            if (ratios[k] == min_positive)
            {
                line = k;
                break;
            }
        }
        List<List<double>> new_simplex_table = new List<List<double>>(); //создаем новую симплекс таблицу
        for (int k = 0; k < simplex_table.Count; ++k) //инициализируем все строки и столбцы новой таблицы
        {
            new_simplex_table.Add(new List<double>());
            for (int f = 0; f < simplex_table[k].Count; ++f)
            {
                new_simplex_table[k].Add(0);
            }
        }
        new_simplex_table[line][column] = 1 / simplex_table[line][column]; //меняем значение элемента на пересечении разрешающих строки и столбца
        for (int c = 0; c < simplex_table[line].Count; ++c) //меняем значения всех элементов разрешающей строки
        {
            if (c == column) continue;
            new_simplex_table[line][c] = simplex_table[line][c] / simplex_table[line][column];
        }
        for (int c = 0; c < simplex_table.Count; ++c) //меняем значения всех элементов разрешающего столбца
        {
            if (c == line) continue;
            new_simplex_table[c][column] = (-1) * simplex_table[c][column] / simplex_table[line][column];
        }
        for (int k = 0; k < simplex_table.Count; ++k) //меняем значения всех остальных элементов
        {
            if (k == line) continue;
            for (int f = 0; f < simplex_table[k].Count; ++f)
            {
                if (f == column) continue;
                new_simplex_table[k][f] = simplex_table[k][f] - simplex_table[k][column] * simplex_table[line][f] / simplex_table[line][column];
            }
        }
        int temp = basis[line];
        basis[line] = not_basis[column - 1]; //так как мы поменяли местами строку и столбец,
        not_basis[column - 1] = temp;        //то вносим изменения в списки базисных и небазисных переменных
        for (int k = 0; k < new_simplex_table.Count - 1; ++k) //меняем список со свободными членами и названиями переменных, которым они принадлежат
        {
            if (k == line)
            {
                X another_x = new X(basis[line], new_simplex_table[k][0]);
                free_memb[k] = another_x;
                continue;
            }
            X x = new X(basis[k], new_simplex_table[k][0]);
            free_memb[k] = x;
        }
        return new_simplex_table; //возвращаем новую симплекс таблицу
    }

    public static void Simplex_method(double[] initial_vec_c, double[] vec_b, double[][] initial_matrix_A, string aspiration, string[] chars) //метод, реализующий симплекс метод
    {
        if (initial_matrix_A.Length != vec_b.Length) throw new Exception("Ошибка, количество строк матрицы должно совпадать с размерностью вектора b");
        if (chars.Length != vec_b.Length) throw new Exception("Ошибка, количество знаков сравнения должно совпадать с размерностью вектора b");
        List<List<X>> matrix_A = new List<List<X>>();
        X x;
        for (int i = 0; i < initial_matrix_A.Length; ++i)  //создаем матрицу A без фиктивных переменных
        {
            matrix_A.Add(new List<X>());
            for (int j = 0; j < initial_matrix_A[0].Length; ++j)
            {
                x = new(j + 1, initial_matrix_A[i][j]);
                matrix_A[i].Add(x);
            }
        }
        int flag = 0;
        double[] vec_c = new double[initial_vec_c.Length];
        Array.Copy(initial_vec_c, 0, vec_c, 0, vec_c.Length);
        if (aspiration == "max")
        {
            for (int i = 0; i < vec_c.Length; ++i) vec_c[i] = (-1) * vec_c[i]; //меняем F->max на F->min
            flag = 1; //чтобы после нахождения оптимального решения перевести назад F->min на F->max
        }
        int count = matrix_A[0].Count; //количество переменных
        List<int> fictive = new List<int>(); //список с фиктивными переменными
        for (int i = 0; i < matrix_A.Count; ++i) //определяем, нужно ли добавлять в строки фиктивные переменные и с каким знаком,
        {                                        //при необходимости добавляем
            if (chars[i] == ">=")
            {
                ++count;
                for (int j = 0; j < matrix_A[i].Count; ++j) //так как фиктивная переменная будет с минусом, мы домножаем всю строку на -1
                {
                    x = new X(matrix_A[i][j].name, (-1) * matrix_A[i][j].value);
                    matrix_A[i][j] = x;
                }
                vec_b[i] = (-1) * vec_b[i]; //так как фиктивная переменная будет с минусом, то и свободный член домножаем на -1
                x = new X(count, 1);
                matrix_A[i].Add(x);
                fictive.Add(count);
            }
            if (chars[i] == "<=")
            {
                ++count;
                x = new X(count, 1);
                matrix_A[i].Add(x);
                fictive.Add(count);
            }
        }
        Console.WriteLine("\nКанонический вид задачи:");
        int k = 0;
        Console.Write("F = ");
        for (int i = 0; i < vec_c.Length; ++i) //печатаем F= X1 + X2 +... ->min/max
        {
            Console.Write($"{vec_c[i]}X{i + 1}");
            if (i != vec_c.Length - 1) Console.Write(" + ");
        }
        Console.Write(" ->min\n");
        for (int i = 0; i < matrix_A.Count; ++i) //печатаем все ограничения
        {
            for (int j = 0; j < matrix_A[i].Count; ++j)
            {
                if (matrix_A[i][j].value == -0) matrix_A[i][j].value = 0;
                if (j < vec_c.Length) Console.Write($"{matrix_A[i][j].value}X{j + 1}");
                if (j >= vec_c.Length)
                {
                    Console.Write($"{matrix_A[i][j].value}X{fictive[k]}");
                    ++k;
                }
                if (j != matrix_A[i].Count - 1) Console.Write(" + ");
            }
            Console.Write($" = ");
            Console.Write($"{vec_b[i]}\n");
        }
        Console.WriteLine("Xi >= 0");
        //На этом этапе мы записали все данные в канонической форме, переходим к созданию симплекс таблицы
        List<int> basis = new List<int>(); //список с базисными переменными
        List<int> not_basis = new List<int>(); //список с небазисными переменными
        for (int i = 0; i < vec_c.Length; ++i)
        {
            if (vec_c[i] != 0) not_basis.Add(i + 1);
        }
        for (int i = 1; i <= count; ++i)
        {
            if (not_basis.Contains(i) == false) basis.Add(i);
        }
        List<X> free_memb = new List<X>(); //список со свободными членами и названиями переменных, которым они принадлежат
        for (int i = 0; i < basis.Count; ++i)
        {
            x = new X(basis[i], vec_b[i]);
            free_memb.Add(x);
        }
        List<List<double>> simplex_table = new List<List<double>>(); //сама симплекс таблица
        for (int i = 0; i < basis.Count + 1; ++i) //создаем симплекс таблицу, +1 строка, так как есть еще строка F
        {
            simplex_table.Add(new List<double>());
            for (int j = 0; j < not_basis.Count + 1; ++j) //+1, так как есть еще столбец свободных членов
            {
                if (i == basis.Count) //заполнение последней строки симплекс таблицы
                {
                    if (j == 0) //первый член последней строки(функции F) всегда равен 0, по условию F не содержит свободных членов
                    {
                        simplex_table[i].Add(0);
                        continue;
                    }
                    simplex_table[i].Add((-1) * vec_c[j - 1]);
                    continue;
                }
                if (j == 0) //первым добавляем свободный член, так как первый столбец состоит из них
                {
                    simplex_table[i].Add(vec_b[i]);
                    continue;
                }
                for (int g = 0; g < not_basis.Count; ++g) //проверяем, является ли переменная из матрицы базисной или нет, 
                {                                         //добавляем только небазисные переменные
                    if (matrix_A[i][j - 1].name == not_basis[g]) simplex_table[i].Add(matrix_A[i][j - 1].value);
                }
            }
        }
        double F = simplex_table[simplex_table.Count - 1][0]; //начальное значение функции F
        double F_new = simplex_table[simplex_table.Count - 1][0]; //конечное значение функции F
        Console.WriteLine("Начальная симплекс таблица:"); //Переходим к поиску опорного решения
        X.print_table(ref simplex_table, basis, not_basis); //начальная симплекс таблица
        bool reference_solution;
        int optimal_solution = 2, sign = 0;
        Console.WriteLine("Поиск решения: "); //Переходим к поиску оптимального решения
        while (optimal_solution != 1) //проходим итерации, пока не получим оптимальное решение
        {
            if(optimal_solution == 2) //сначала ищем опорное решение или если наше решение перестало быть опорным, то снова запускаем цикл его поиска
            {
                reference_solution = false;
                while (reference_solution == false) //проходим итерации, пока не получим опорное решение
                {
                    reference_solution = X.Finding_a_reference_solution(ref simplex_table, ref free_memb, ref basis, ref not_basis, ref sign);
                    if (reference_solution != true) X.print_table(ref simplex_table, basis, not_basis); //вывод симплекс таблицы в консоль после каждой итерации
                }
                Console.WriteLine("Опорное решение найдено");
                Console.WriteLine("Переходим к поиску оптимального решения");
            }
            F = simplex_table[simplex_table.Count - 1][0];
            optimal_solution = X.Finding_a_optimal_solution(ref simplex_table, ref free_memb, ref basis, ref not_basis, ref sign);
            if (optimal_solution != 1) X.print_table(ref simplex_table, basis, not_basis); //вывод симплекс таблицы в консоль после каждой итерации
            F_new = simplex_table[simplex_table.Count - 1][0];
            if (F_new > F) throw new Exception("Значение функции F прямой задачи увеличилось, значит оптимального решения не существует");
        }
        List<X> answer_list = new List<X>(); //список, в котором будут лежать переменные и их значения для оптимального решения
        Console.WriteLine("Ответ: "); //пишем ответ
        for (int i = 0; i < basis.Count; ++i)
        {
            if (fictive.Contains(free_memb[i].name) == false)
            {
                Console.WriteLine($"X{free_memb[i].name}={Math.Round(free_memb[i].value, 1)}"); //записываем значения нефиктивных переменных
                x = new X(free_memb[i].name, free_memb[i].value);
                answer_list.Add(x);
            }
        }
        for (int i = 0; i < not_basis.Count; ++i)
        {
            if (fictive.Contains(not_basis[i]) == false)
            {
                Console.WriteLine($"X{not_basis[i]}=0"); //записываем значения переменных, которые равны 0
                x = new X(not_basis[i], 0);
                answer_list.Add(x);
            }
        }
        if (flag == 1) simplex_table[simplex_table.Count - 1][0] = (-1) * simplex_table[simplex_table.Count - 1][0]; //чтобы перевести назад F->min на F->max
        Console.WriteLine($"F={Math.Round(simplex_table[simplex_table.Count - 1][0], 1)}");
        var sorted_answer_list = answer_list.OrderBy(i => i.name).ToList(); //сортируем список с ответами в порядке возрастания(от X1 до Xn)
        Console.WriteLine($"Выполняем проверку подстановкой и получаем: ");
        double comparison_number = 0, F_value = 0;
        Console.Write("F = ");
        if (flag == 1) for (int i = 0; i < sorted_answer_list.Count; ++i) //так как меняли F->max на F->min
            {
                Console.Write($"{((-1) * vec_c[i])} * {Math.Round(sorted_answer_list[i].value, 1)}");
                if (i != sorted_answer_list.Count - 1) Console.Write(" + ");
                F_value += sorted_answer_list[i].value * ((-1) * vec_c[i]);
            }
        else for (int i = 0; i < sorted_answer_list.Count; ++i)
            {
                Console.Write($"{(vec_c[i])} * {Math.Round(sorted_answer_list[i].value, 1)}");
                if (i != sorted_answer_list.Count - 1) Console.Write(" + ");
                F_value += sorted_answer_list[i].value * (vec_c[i]);
            }
        Console.Write($" = {Math.Round(F_value, 1)}\n");
        for (int i = 0; i < initial_matrix_A.Length; ++i) //проверяем правильность значения функции F путем подстановки значений переменных в ограничения
        {
            comparison_number = 0;
            for (int j = 0; j < initial_matrix_A[0].Length; ++j)
            {
                if (chars[i] == ">=") //для корректного отображения ограничения, так как ранее мы меняли в строке знаки
                {
                    matrix_A[i][j].value = (-1) * matrix_A[i][j].value;
                    if(j == 0) vec_b[i] = (-1) * vec_b[i];
                }
                if (matrix_A[i][j].value == -0) matrix_A[i][j].value = 0;
                Console.Write($"{Math.Round(matrix_A[i][j].value, 1)} * {Math.Round(sorted_answer_list[j].value, 1)}");
                comparison_number += matrix_A[i][j].value * sorted_answer_list[j].value;
                if (j != initial_matrix_A[0].Length - 1) Console.Write(" + ");
            }
            Console.Write($" {chars[i]} {vec_b[i]} \t{Math.Round(comparison_number, 1)} {chars[i]} {vec_b[i]}");
            if (chars[i] == ">=" && Math.Round(comparison_number, 1) >= vec_b[i]) Console.Write(" - верно\n");
            if (chars[i] == "<=" && Math.Round(comparison_number, 1) <= vec_b[i]) Console.Write(" - верно\n");
            if (chars[i] == "==" && Math.Round(comparison_number, 1) == vec_b[i]) Console.Write(" - верно\n");
            if (chars[i] == ">=" && Math.Round(comparison_number, 1) < vec_b[i] || chars[i] == "<=" && Math.Round(comparison_number, 1) > vec_b[i] || chars[i] == "==" && Math.Round(comparison_number, 1) != vec_b[i]) Console.Write(" - неверно!!\n");
        }
        for (int i = 0; i < sorted_answer_list.Count; ++i)
        {
            Console.Write($"X{sorted_answer_list[i].name}={Math.Round(sorted_answer_list[i].value, 1)} >= 0");
            if (sorted_answer_list[i].value >= 0) Console.Write(" - верно\n");
            if (sorted_answer_list[i].value < 0) Console.Write(" - неверно!!\n");
        }
    }
}
