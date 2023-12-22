using System.Threading.Channels;

namespace labX
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Задание параметров работы
            int n = 10; // Количество работ           
            int[] T = FillArray(10); // Вектор длительностей выполнения работ
            int[] d = FillArray(10); // Вектор моментов поступлений работ
            int[] D = FillArray(10); // Вектор директивных сроков
            int[] F = FillArray(10); // Вектор коэффициентов штрафов

            // Создание начального расписания
            int[] X = new int[n];
            int[] Y = new int[n];
            
            Random rand = new Random();
            string path = @"D:\100 решений .txt";
            File.Create(path).Close();

            double[] solutionsMBS = new double[100];
            double[] solutionsMPP = new double[100];

            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < n; i++)
                {

                    X[i] = rand.Next(1, 10);
                    X[i] = (int)(0.4 * (Math.Pow(Math.E, -(0.5 / 5) * (X[i] + 5) * (X[i]) + 5)));//Нормальное распределение                
                    Y[i] = X[i] + T[i]; 
                }

                
                {
                    

                    // Вычисление текущего значения функции штрафа
                    double currentPenalty = CalculatePenalty(Y, D, F);

                    // Проход по всем перестановкам работ
                    for (int i = 0; i < n - 1; i++)
                    {
                        for (int j = i + 1; j < n; j++)
                        {
                            // Создание нового расписания путем обмена работами i и j
                            int[] newX = (int[])X.Clone();
                            int[] newY = (int[])Y.Clone();
                            Swap(ref newX[i], ref newX[j]);
                            Swap(ref newY[i], ref newY[j]);

                            // Вычисление нового значения функции штрафа
                            int newPenalty = CalculatePenalty(newY, D, F);

                            // Если новое значение функции штрафа меньше текущего, то обновляем расписание
                            if (newPenalty < currentPenalty)
                            {
                                X = newX;
                                Y = newY;
                                currentPenalty = newPenalty;
                                
                            }
                        }
                    }
                }

                File.AppendAllText(path, $"Оптимальное расписание: для решения #{k + 1} (МБС)\n ");
                for (int i = 0; i < n; i++)
                {
                    File.AppendAllText(path, $"Работа {i + 1}: начало - {X[i]}, окончание - {Y[i]}\n");
                }
                solutionsMBS[k] = CalculatePenalty(Y, D, F);
                File.AppendAllText(path, $"Общая сумма штрафов: {solutionsMBS[k]}\n\n");


                //метод ближайшего соседа
                bool improvement = true; // Флаг, показывающий, было ли улучшение расписания на данной итерации
                while (improvement)
                {
                    improvement = false;

                    // Вычисление текущего значения функции штрафа
                    double currentPenalty = CalculatePenalty(Y, D, F);

                    // Проход по всем перестановкам работ
                    for (int i = 0; i < n - 1; i++)
                    {
                        for (int j = i + 1; j < n; j++)
                        {
                            // Создание нового расписания путем обмена работами i и j
                            int[] newX = (int[])X.Clone();
                            int[] newY = (int[])Y.Clone();
                            Swap(ref newX[i], ref newX[j]);
                            Swap(ref newY[i], ref newY[j]);

                            // Вычисление нового значения функции штрафа
                            int newPenalty = CalculatePenalty(newY, D, F);

                            // Если новое значение функции штрафа меньше текущего, то обновляем расписание
                            if (newPenalty < currentPenalty)
                            {
                                X = newX;
                                Y = newY;
                                currentPenalty = newPenalty;
                                improvement = true;
                            }
                        }
                    }
                }

                // Вывод результатов
                File.AppendAllText(path, $"Оптимальное расписание: для решения #{k + 1} (МПП)\n");
                for (int i = 0; i < n; i++)
                {
                    File.AppendAllText(path, $"Работа {i + 1}: начало - {X[i]}, окончание - {Y[i]}\n");
                }
                solutionsMPP[k] = CalculatePenalty(Y, D, F);
                File.AppendAllText(path, $"Общая сумма штрафов: {solutionsMPP[k]}\n\n");



            }
           
            RelativeDeviations(ref solutionsMBS);
            RelativeDeviations(ref solutionsMPP);

            

            for (int i = 0; i < solutionsMBS.Length; i++)
            {
                solutionsMBS[i] = Math.Round(((solutionsMBS[i] - solutionsMBS.Min()) / (solutionsMBS.Max() - solutionsMBS.Min()) * 100), 2);
                solutionsMPP[i] = Math.Round(((solutionsMPP[i] - solutionsMPP.Min()) / (solutionsMPP.Max() - solutionsMPP.Min()) * 100), 2);
                
            }
         

            for (int i = 0; i < solutionsMBS.Length; i++)
                Console.WriteLine($"{solutionsMBS[i]}");

            Console.WriteLine("----------------------");

            for (int i = 0; i < solutionsMPP.Length; i++)
                Console.WriteLine($"{solutionsMPP[i]}");
        }


        static double[] RangeCounters(double[] deviations)
        {
            double[] rangeCounters = new double[10];
            for (int i = 0; i < deviations.Length; i++)
            {
                if (deviations[i] >= 0 && (deviations[i]) <= 10)
                    rangeCounters[0] += 1;

                else if (deviations[i] > 10 && deviations[i] <= 20)
                    rangeCounters[1] += 1;

                else if (deviations[i] > 20 && deviations[i] <= 30)
                    rangeCounters[2] += 1;

                else if (deviations[i] > 30 && deviations[i] <= 40)
                    rangeCounters[3] += 1;

                else if (deviations[i] > 40 && deviations[i] <= 50)
                    rangeCounters[4] += 1;

                else if (deviations[i] > 50 && deviations[i] <= 60)
                    rangeCounters[5] += 1;

                else if (deviations[i] > 60 && deviations[i] <= 70)
                    rangeCounters[6] += 1;

                else if (deviations[i] > 70 && deviations[i] <= 80)
                    rangeCounters[7] += 1;

                else if (deviations[i] > 80 && deviations[i] <= 90)
                    rangeCounters[8] += 1;

                else if (deviations[i] > 90 && deviations[i] <= 100)
                    rangeCounters[9] += 1;
            }

            return rangeCounters;

        }

        static int[] FillArray(int cont)
        {
            Random rnd = new Random();
            int[] arr = new int[cont];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = rnd.Next(1, 13);

            return arr;
        }

        static void RelativeDeviations(ref double[] solutions)
        {
            for (int i = 0; i < solutions.Length; i++)
            {
                solutions[i] -= solutions.Min();

                //solutions[i] /= solutions.Contains(0) ? solutions.Length : solutions.Min();
            }

        }

        // Функция для обмена двух значений
        static void Swap(ref int x, ref int y)
        {
            int temp = x;
            x = y;
            y = temp;
        }

        // Функция для вычисления значения функции штрафа
        static int CalculatePenalty(int[] Y, int[] D, int[] F)
        {
            int penalty = 0;
            for (int i = 0; i < Y.Length; i++)
            {
                if (Y[i] > D[i])
                {
                    penalty += (Y[i] - D[i]) * F[i];
                }
            }
            return penalty;
        }
    }

}
