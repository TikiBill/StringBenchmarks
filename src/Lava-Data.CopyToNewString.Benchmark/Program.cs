// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace Lava_Data.LoopOverStringSpan.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var titles = new List<string>();
            var summaries = new List<BenchmarkDotNet.Reports.Summary>();

            var logger = new ConsoleLogger();


            titles.Add("Very Short String (" + (new CopyStringToCharArrayVeryShortString()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<CopyStringToCharArrayVeryShortString>());

            titles.Add("Short String (" + (new CopyStringToCharArrayShortString()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<CopyStringToCharArrayShortString>());

            titles.Add("Long String (" + (new CopyStringToCharArrayLongString()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<CopyStringToCharArrayLongString>());

            titles.Add("Longer String (" + (new CopyStringToCharArrayLongerString()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<CopyStringToCharArrayLongerString>());

            logger.WriteLineHeader("-------------------");
            for (int i = 0; i < summaries.Count; ++i)
            {
                {
                    logger.WriteLine("");
                    logger.WriteLine("");
                    logger.WriteHeader("*****     " + titles[i] + "    *****");
                    MarkdownExporter.Console.ExportToLog(summaries[i], logger);
                }
            }
        }
    }


    public class CopyStringToCharArrayVeryShortString : CopyStringToCharArray
    {
        public CopyStringToCharArrayVeryShortString() : base("FOUR") { }
    }


    public class CopyStringToCharArrayShortString : CopyStringToCharArray
    {
        public CopyStringToCharArrayShortString() : base("Hello ye' World") { }
    }


    public class CopyStringToCharArrayLongString : CopyStringToCharArray
    {
        // From the benchmarkdotnet site.
        public static string lstr = "A method which is marked by the [IterationSetup] attribute will be executed only once before each an iteration. It's not recommended to use this attribute in microbenchmarks because it can spoil the results. However, if you are writing a macrobenchmark (e.g. a benchmark which takes at least 100ms) and you want to prepare some data before each iteration, [IterationSetup] can be useful. BenchmarkDotNet doesn't support setup/cleanup method for a single method invocation (an operation), but you can perform only one operation per iteration. It's recommended to use RunStrategy.Monitoring for such cases. Be careful: if you allocate any objects in the [IterationSetup] method, the MemoryDiagnoser results can also be spoiled. ";

        public CopyStringToCharArrayLongString() : base(lstr) { }
    }

    public class CopyStringToCharArrayLongerString : CopyStringToCharArray
    {
        public static string lstr = "A method which is marked by the [IterationSetup] attribute will be executed only once before each an iteration. It's not recommended to use this attribute in microbenchmarks because it can spoil the results. However, if you are writing a macrobenchmark (e.g. a benchmark which takes at least 100ms) and you want to prepare some data before each iteration, [IterationSetup] can be useful. BenchmarkDotNet doesn't support setup/cleanup method for a single method invocation (an operation), but you can perform only one operation per iteration. It's recommended to use RunStrategy.Monitoring for such cases. Be careful: if you allocate any objects in the [IterationSetup] method, the MemoryDiagnoser results can also be spoiled. ";

        public CopyStringToCharArrayLongerString() : base(lstr + lstr + lstr + lstr + lstr + lstr + lstr) { }
    }


    [CoreJob]
    [RPlotExporter, RankColumn]
    [MemoryDiagnoser]
    public class CopyStringToCharArray
    {
        [Params(10000)]
        public int N;


        public string str { get; set; } = "";

        public CopyStringToCharArray() { }

        public CopyStringToCharArray(string testString)
        {
            str = testString;
        }

        [GlobalSetup]
        public void Setup()
        {
            Console.WriteLine("// NOTE: String Length is " + str.Length);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            Console.WriteLine("// NOTE: String Length is " + str.Length);
        }


        [Benchmark]
        public string CopyString_TempCharOutsideLoop_String()
        {
            var src = str.AsSpan();
            var dest = new char[str.Length];
            var dest_i = 0;
            char c;
            for (int i = 0; i < src.Length; ++i)
            {
                c = src[i];
                if (c == '\'')
                    dest[dest_i++] = '_';
                else if (c == 'i')
                    dest[dest_i++] = 'I';
                else
                    dest[dest_i++] = c;
            }
            return new string(dest);
        }


        [Benchmark]
        public string CopyString_SpanIndex_Unsafe_String()
        {
            var src = str.AsSpan();
            var dest_i = 0;
            char c;
            unsafe
            {
                var dest = stackalloc char[str.Length];
                for (int i = 0; i < src.Length; ++i)
                {
                    c = src[i];
                    if (c == '\'')
                        dest[dest_i++] = '_';
                    else if (c == 'i')
                        dest[dest_i++] = 'I';
                    else
                        dest[dest_i++] = c;
                }
                return new string(dest, 0, dest_i);
            }
        }

    }
}
