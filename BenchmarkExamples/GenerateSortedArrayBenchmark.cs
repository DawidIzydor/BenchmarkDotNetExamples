using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace BenchmarkExamples
{
    public class GenerateSortedArrayBenchmark
    {
        private Random _random;

        [Params(10, 1_000, 100_000, 1_000_000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _random = new Random(42);
        }

        [Benchmark(Baseline = true)]
        public int[] ClassicGenerateAndSortUsingLinq()
        {
            var array = new int[N];
            for (var i = 0; i < array.Length; ++i)
            {
                array[i] = _random.Next();
            }

            return array.OrderBy(i => i).ToArray();
        }

        [Benchmark]
        public int[] GenerateUsingSortedList()
        {
            var dictionary = new SortedDictionary<int, int>();
            for (var i = 0; i < N; ++i)
            {
                var randomNr = _random.Next();
                if (dictionary.ContainsKey(randomNr))
                {
                    dictionary[randomNr]++;
                }
                else
                {
                    dictionary.Add(randomNr, 1);
                }
            }

            return dictionary.Keys.ToArray();
        }

        [Benchmark]
        public int[] GenerateAndQuickSort()
        {
            var array = new int[N];
            for (var i = 0; i < array.Length; ++i)
            {
                array[i] = _random.Next();
            }

            QuickSort(array, 0, array.Length - 1);
            return array;
        }

        private static void QuickSort(int[] array, int left, int right)
        {
            var i = left;
            var j = right;
            var pivot = array[(left + right) / 2];
            while (i < j)
            {
                while (array[i] < pivot)
                {
                    ++i;
                }

                while (array[j] > pivot)
                {
                    --j;
                }

                if (i <= j)
                {
                    var tmp = array[i];
                    array[i++] = array[j];
                    array[j--] = tmp;
                }
            }

            if (left < j)
            {
                QuickSort(array, left, j);
            }

            if (i < right)
            {
                QuickSort(array, i, right);
            }
        }

        [Benchmark]
        public int[] GenerateAndQuickSortParallel()
        {
            var array = new int[N];
            for (var i = 0; i < array.Length; ++i)
            {
                array[i] = _random.Next();
            }

            QuickSortParallel(array, 0, array.Length - 1);
            return array;
        }

        private static void QuickSortParallel(int[] array, int left, int right)
        {
            if (right - left >= 10_000)
            {
                var middle = (right + left) / 2;
                var leftCpy = left;
                var rightCpy = right;
                var taskLeft = Task.Run(() => QuickSortParallel(array, leftCpy, middle));
                var taskRight = Task.Run(() => QuickSortParallel(array, middle + 1, rightCpy));
                taskLeft.Wait();
                taskRight.Wait();
            }
            else
            {
                while (true)
                {
                    var i = left;
                    var j = right;
                    var pivot = array[(left + right) / 2];
                    while (i < j)
                    {
                        while (array[i] < pivot)
                        {
                            ++i;
                        }

                        while (array[j] > pivot)
                        {
                            --j;
                        }

                        if (i > j)
                        {
                            continue;
                        }

                        var tmp = array[i];
                        array[i++] = array[j];
                        array[j--] = tmp;
                    }

                    if (left < j)
                    {
                        QuickSortParallel(array, left, j);
                    }

                    if (i >= right)
                    {
                        break;
                    }

                    left = i;
                }
            }
        }
    }
}