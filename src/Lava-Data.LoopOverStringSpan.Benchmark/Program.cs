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


            titles.Add("Very Short String (" + (new CharRefMethodsVeryShortString()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<CharRefMethodsVeryShortString>());

            titles.Add("Short String (" + (new CharRefMethodsShortString()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<CharRefMethodsShortString>());

            titles.Add("Long String (" + (new CharRefMethodsLongString()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<CharRefMethodsLongString>());

            titles.Add("Longer String (" + (new CharRefMethodsLongerString()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<CharRefMethodsLongerString>());

            logger.WriteLineHeader("-------------------");
            for (int i = 0; i < summaries.Count; ++i)
            {
                {
                    logger.WriteLine("");
                    logger.WriteLine("");
                    logger.WriteHeader("### " + titles[i]);
                    MarkdownExporter.Console.ExportToLog(summaries[i], logger);
                    //foreach (var lines in summary.Table.FullContentWithHeader)
                    //    Console.WriteLine(string.Join("|", lines));
                }
            }
        }
    }

    public class CharRefMethodsVeryShortString : CharRefMethods
    {
        public CharRefMethodsVeryShortString() : base("FOUR") { }
    }


    public class CharRefMethodsShortString : CharRefMethods
    {
        public CharRefMethodsShortString() : base("Hello ye' World") { }
    }


    public class CharRefMethodsLongString : CharRefMethods
    {
        // From the benchmarkdotnet site.
        public static string lstr = "A method which is marked by the [IterationSetup] attribute will be executed only once before each an iteration. It's not recommended to use this attribute in microbenchmarks because it can spoil the results. However, if you are writing a macrobenchmark (e.g. a benchmark which takes at least 100ms) and you want to prepare some data before each iteration, [IterationSetup] can be useful. BenchmarkDotNet doesn't support setup/cleanup method for a single method invocation (an operation), but you can perform only one operation per iteration. It's recommended to use RunStrategy.Monitoring for such cases. Be careful: if you allocate any objects in the [IterationSetup] method, the MemoryDiagnoser results can also be spoiled. ";

        public CharRefMethodsLongString() : base(lstr) { }
    }

    public class CharRefMethodsLongerString : CharRefMethods
    {
        public static string lstr = "A method which is marked by the [IterationSetup] attribute will be executed only once before each an iteration. It's not recommended to use this attribute in microbenchmarks because it can spoil the results. However, if you are writing a macrobenchmark (e.g. a benchmark which takes at least 100ms) and you want to prepare some data before each iteration, [IterationSetup] can be useful. BenchmarkDotNet doesn't support setup/cleanup method for a single method invocation (an operation), but you can perform only one operation per iteration. It's recommended to use RunStrategy.Monitoring for such cases. Be careful: if you allocate any objects in the [IterationSetup] method, the MemoryDiagnoser results can also be spoiled. ";

        public CharRefMethodsLongerString() : base(lstr + lstr + lstr + lstr + lstr + lstr + lstr) { }
    }


    [CoreJob]
    [RPlotExporter, RankColumn]
    // [MemoryDiagnoser]
    public class CharRefMethods
    {
        [Params(10000)]
        public int N;


        public string str { get; set; } = "";

        public CharRefMethods() { }

        public CharRefMethods(string testString)
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
        public int CheckStringLength()
        {
            if (str == null)
                throw new ArgumentException("String is null!");

            if (str.Length < 2)
                throw new ArgumentException("String Too Short!");

            return str.Length;
        }

        [Benchmark]
        public (int, int) CountQuotes_CharBySpanIndex()
        {
            var src = str.AsSpan();
            var singleQuotes = 0;
            var nulls = 0;
            for (int i = 0; i < src.Length; ++i)
            {
                if (src[i] == '\'')
                    ++singleQuotes;
                else if (src[i] == '\0')
                    ++nulls;
            }
            return (singleQuotes, nulls);
        }

        [Benchmark]
        public (int, int) CountQuotes_CharBySpanIndex_Lambda() => CountQuotes_CharBySpanIndex();

        [Benchmark]
        public (int, int) CountQuotes_TempCharInLoop()
        {
            var src = str.AsSpan();
            var singleQuotes = 0;
            var nulls = 0;
            for (int i = 0; i < src.Length; ++i)
            {
                char c = src[i];
                if (c == '\'')
                    ++singleQuotes;
                else if (c == '\0')
                    ++nulls;
            }
            return (singleQuotes, nulls);
        }

        [Benchmark]
        public (int, int) CountQuotes_TempCharOutsideLoop()
        {
            var src = str.AsSpan();
            var singleQuotes = 0;
            var nulls = 0;
            char c;
            for (int i = 0; i < src.Length; ++i)
            {
                c = src[i];
                if (c == '\'')
                    ++singleQuotes;
                else if (c == '\0')
                    ++nulls;
            }
            return (singleQuotes, nulls);
        }

        /// <summary>
        /// For comparison to IsControl, which is a robust lookup table, see
        /// how fast it is to look it up with ranges of character values.
        /// </summary>
        [Benchmark]
        public (int, int) CountQuotesCtrlRangeLookup_BySpanIndex()
        {
            var src = str.AsSpan();
            var singleQuotes = 0;
            var ctrlChars = 0;
            char c;
            for (int i = 0; i < src.Length; ++i)
            {
                c = src[i];
                if (c == '\'')
                    ++singleQuotes;
                else if (c <= '\x001f' || (c >= '\x007f' && c <= '\x009f'))
                    ++ctrlChars;
            }
            return (singleQuotes, ctrlChars);
        }

        /// <summary>
        /// For comparison to IsControl, which is a robust lookup table, see
        /// how fast it is to look it up in our own table for comparison.
        /// </summary>
        [Benchmark]
        public (int, int) CountQuotesCtrlLocalLookup_BySpanIndex()
        {
            var src = str.AsSpan();
            var singleQuotes = 0;
            var ctrlChars = 0;
            char c;
            for (int i = 0; i < src.Length; ++i)
            {
                c = src[i];
                if (c == '\'')
                    ++singleQuotes;
                else if (c <= '\x009f' && IsControlChar[c])
                    ++ctrlChars;
            }
            return (singleQuotes, ctrlChars);
        }

        [Benchmark]
        public (int, int) CountQuotesCtrl_BySpanIndex()
        {
            var src = str.AsSpan();
            var singleQuotes = 0;
            var ctrlChars = 0;
            for (int i = 0; i < src.Length; ++i)
            {
                if (src[i] == '\'')
                    ++singleQuotes;
                else if (Char.IsControl(src[i]))
                    ++ctrlChars;
            }
            return (singleQuotes, ctrlChars);
        }

        [Benchmark]
        public (int, int) CountQuotesCtrl_TempCharInLoop()
        {
            var src = str.AsSpan();
            var singleQuotes = 0;
            var ctrlChars = 0;
            for (int i = 0; i < src.Length; ++i)
            {
                char c = src[i];
                if (c == '\'')
                    ++singleQuotes;
                else if (Char.IsControl(c))
                    ++ctrlChars;
            }
            return (singleQuotes, ctrlChars);
        }

        [Benchmark]
        public (int, int) CountQuotesCtrl_TempCharOutsideLoop()
        {
            var src = str.AsSpan();
            var singleQuotes = 0;
            var ctrlChars = 0;
            char c;
            for (int i = 0; i < src.Length; ++i)
            {
                c = src[i];
                if (c == '\'')
                    ++singleQuotes;
                else if (Char.IsControl(c))
                    ++ctrlChars;
            }
            return (singleQuotes, ctrlChars);
        }


        /// <summary>
        /// Copy a string into a character array, altering two
        /// characters. The destination index is its own to match
        /// how this idea will be used in the ANSI quote method.
        /// </summary>
        [Benchmark]
        public char[] CopyString_SpanIndex()
        {
            var src = str.AsSpan();
            var dest = new char[str.Length];
            var dest_i = 0;
            for (int i = 0; i < src.Length; ++i)
            {
                if (src[i] == '\'')
                    dest[dest_i++] = '_';
                else if (src[i] == 'i')
                    dest[dest_i++] = 'I';
                else
                    dest[dest_i++] = src[i];
            }
            return dest;
        }

        [Benchmark]
        public char[] CopyString_TempCharInLoop()
        {
            var src = str.AsSpan();
            var dest = new char[str.Length];
            var dest_i = 0;
            for (int i = 0; i < src.Length; ++i)
            {
                char c = src[i];
                if (c == '\'')
                    dest[dest_i++] = '_';
                else if (c == 'i')
                    dest[dest_i++] = 'I';
                else
                    dest[dest_i++] = c;
            }
            return dest;
        }

        [Benchmark]
        public char[] CopyString_TempCharOutsideLoop()
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
            return dest;
        }


        [Benchmark]
        public char CopyString_SpanIndex_Unsafe()
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
                // We cannot return the reference to dest since that would
                // be outside the unsafe block.
                // And yes this could throw if our test string is less than three characters long.
                return dest[2];
            }
        }


        public static readonly bool[] IsControlChar =
        {
            true, true, true, true, true, true, true, true,  // 0x00 - 0x07
            true, true, true, true, true, true, true, true,  // 0x08 - 0x0f
            true, true, true, true, true, true, true, true,  // 0x10 - 0x17
            true, true, true, true, true, true, true, true,  // 0x18 - 0x1f
            false, false, false, false, false, false, false, false, // 0x20 - 0x27
            false, false, false, false, false, false, false, false, // 0x28 - 0x2f
            false, false, false, false, false, false, false, false, // 0x30 - 0x37
            false, false, false, false, false, false, false, false, // 0x38 - 0x3f
            false, false, false, false, false, false, false, false, // 0x40 - 0x47
            false, false, false, false, false, false, false, false, // 0x48 - 0x4f
            false, false, false, false, false, false, false, false, // 0x50 - 0x57
            false, false, false, false, false, false, false, false, // 0x58 - 0x5f
            false, false, false, false, false, false, false, false, // 0x60 - 0x67
            false, false, false, false, false, false, false, false, // 0x68 - 0x6f
            false, false, false, false, false, false, false, false, // 0x70 - 0x77
            false, false, false, false, false, false, false, true, // 0x78 - 0x7f
            true, true, true, true, true, true, true, true,  // 0x80 - 0x87
            true, true, true, true, true, true, true, true,  // 0x88 - 0x8f
            true, true, true, true, true, true, true, true,  // 0x90 - 0x97
            true, true, true, true, true, true, true, true,  // 0x98 - 0x9f
        };

    }
}
