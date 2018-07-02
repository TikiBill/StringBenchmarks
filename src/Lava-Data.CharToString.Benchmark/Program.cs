// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;

namespace LavaData.CharToString.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkDotNet.Reports.Summary summary;
            summary = BenchmarkRunner.Run<CharToStringBenchmarks>();
        }
    }


    [CoreJob]
    [RPlotExporter, RankColumn]
    public class CharToStringBenchmarks
    {
        //[Params(1000, 10000)]
        [Params(1000)]
        public int N;

        public char[] ShortCharArray;


        public char[] MediumCharArray;

        public char[] LongCharArray;

        // From the benchmarkdotnet site.
        static string lStr = "A method which is marked by the [IterationSetup] attribute will be executed only once before each an iteration. It's not recommended to use this attribute in microbenchmarks because it can spoil the results. However, if you are writing a macrobenchmark (e.g. a benchmark which takes at least 100ms) and you want to prepare some data before each iteration, [IterationSetup] can be useful. BenchmarkDotNet doesn't support setup/cleanup method for a single method invocation (an operation), but you can perform only one operation per iteration. It's recommended to use RunStrategy.Monitoring for such cases. Be careful: if you allocate any objects in the [IterationSetup] method, the MemoryDiagnoser results can also be spoiled. ";

        static string veryLongString = lStr + lStr + lStr + lStr + "\0";

        public char[] LongCharArrayFromScratch;

        [GlobalSetup]
        public void Setup()
        {
            ShortCharArray = "A short string".ToCharArray();
            ShortCharArray[0] = 'a';

            MediumCharArray = "A medium string with a sentence or two. Just so we can compare how a longer paragraph might fair in the conversion routines.".ToCharArray();
            MediumCharArray[0] = 'a';

            LongCharArray = veryLongString.ToCharArray();
            LongCharArray[0] = 'a'; //Change a character to make sure that the string -> char[] -> string does not get optimized to assignment.

            //Create a char array from scratch that does not have an associated string in memory.
            LongCharArrayFromScratch = new char[LongCharArray.Length];
            for (int i = 0; i < LongCharArrayFromScratch.Length; ++i)
            {
                LongCharArrayFromScratch[i] = 'a';
            }

            Console.WriteLine("//  Short Char Array Length: " + ShortCharArray.Length);
            Console.WriteLine("// Medium Char Array Length: " + MediumCharArray.Length);
            Console.WriteLine("//   Long Char Array Length: " + LongCharArray.Length);


        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Console.WriteLine("//  Short Char Array Length: " + ShortCharArray.Length);
            Console.WriteLine("// Medium Char Array Length: " + MediumCharArray.Length);
            Console.WriteLine("//   Long Char Array Length: " + LongCharArray.Length);
        }


        [Benchmark]
        public string CharArrayAsSpanToString_Short() => ShortCharArray.AsSpan().ToString();

        [Benchmark]
        public string NewStringCharArray_Short() => new string(ShortCharArray);

        [Benchmark]
        public string NewStringCharArrayStartLen_Short() => new string(ShortCharArray, 0, ShortCharArray.Length);

        [Benchmark]
        public string NewStringCharArrayAsSpan_Short() => new string(ShortCharArray.AsSpan());

        [Benchmark]
        public string NewStringCharArrayAsSpanSliced_Short() => new string(ShortCharArray.AsSpan().Slice(0, ShortCharArray.Length));

        [Benchmark]
        public string CharArrayAsSpanToString_Medium() => MediumCharArray.AsSpan().ToString();

        [Benchmark]
        public string NewStringCharArray_Medium() => new string(MediumCharArray);

        [Benchmark]
        public string NewStringCharArrayStartLen_Medium() => new string(MediumCharArray, 0, MediumCharArray.Length);


        [Benchmark]
        public string NewStringCharArrayAsSpan_Medium() => new string(MediumCharArray.AsSpan());


        [Benchmark]
        public string NewStringCharArrayAsSpanSliced_Medium() => new string(MediumCharArray.AsSpan().Slice(0, MediumCharArray.Length));

        /// <summary>
        /// Change a character in the array.
        /// It introduces a conditional so it will be slower, but it should not be by much.
        /// </summary>
        [Benchmark]
        public string NewStringCharArrayChangeChar_Long()
        {
            LongCharArray[0] = LongCharArray[0] == 'a' ? 'A' : 'a';
            return new string(LongCharArray);
        }

        [Benchmark]
        public string CharArrayAsSpanToString_Long() => LongCharArray.AsSpan().ToString();

        [Benchmark]
        public string CharArrayAsSpanSliceToString_Long() => LongCharArray.AsSpan().Slice(0, LongCharArray.Length).ToString();


        [Benchmark]
        public string NewStringCharArray_Long() => new string(LongCharArray);

        [Benchmark]
        public string NewStringCharArrayStartLen_Long() => new string(LongCharArray, 0, LongCharArray.Length);

        [Benchmark]
        public string NewStringStringAsSpan_Long() => new string(veryLongString.AsSpan());


        [Benchmark]
        public string NewStringCharArrayAsSpan_Long() => new string(LongCharArray.AsSpan());


        [Benchmark]
        public string NewStringCharArrayAsSpanSliced_Long() => new string(LongCharArray.AsSpan().Slice(0, LongCharArray.Length));


        [Benchmark]
        public string NewStringCharPtr_Long()
        {
            unsafe
            {
                fixed (char* cPtr = &LongCharArray[0])
                {
                    return new string(cPtr);
                }
            }
        }

        [Benchmark]
        public string NewStringCharPtrStartLen_Long()
        {
            unsafe
            {
                fixed (char* cPtr = &LongCharArray[0])
                {
                    return new string(cPtr, 0, LongCharArray.Length);
                }
            }
        }

        /// <summary>
        /// Get a baseline for copying a string one character at a time.
        /// 
        /// This one should be way *slower* than the internal new string() methods
        /// because internally wstrcpy copies the string, which uses Buffer.Memmove,
        /// which uses memmove, i.e. expect more than 2x time.
        /// </summary>
        [Benchmark]
        public string NewStringCharArrayCopyAsString_Long()
        {
            var newChars = new char[LongCharArray.Length];
            unsafe
            {
                fixed (char* src = &LongCharArray[0], dest = &newChars[0])
                {
                    for (int i = 0; i < newChars.Length; ++i)
                    {
                        dest[i] = src[i];
                    }
                }
            }
            return new string(newChars);
        }

        /// <summary>
        /// Try using the stack during the character array copy.
        /// </summary>
        [Benchmark]
        public string NewStringCharArrayStackCopyAsString_Long()
        {
            unsafe
            {
                var newChars = stackalloc char[LongCharArray.Length];
                fixed (char* src = &LongCharArray[0])
                {
                    for (int i = 0; i < LongCharArray.Length; ++i)
                    {
                        newChars[i] = src[i];
                    }
                }
                return new string(newChars, 0, LongCharArray.Length);
            }
        }

    }
}
