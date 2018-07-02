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

namespace LavaData.DbQuoteVariations.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var titles = new List<string>();
            var summaries = new List<BenchmarkDotNet.Reports.Summary>();

            var logger = new ConsoleLogger();


            titles.Add("Very Short String, No Single Quotes (" + (new QuoteMethodVeryShortNoSingleQuote()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<QuoteMethodVeryShortNoSingleQuote>());

            titles.Add("Very Short String, With Single Quotes (" + (new QuoteMethodVeryShortWithSingleQuote()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<QuoteMethodVeryShortWithSingleQuote>());

            titles.Add("Short String, No Single Quotes (" + (new QuoteMethodShortNoSingleQuote()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<QuoteMethodShortNoSingleQuote>());

            titles.Add("Short String, With Single Quotes (" + (new QuoteMethodShortWithSingleQuote()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<QuoteMethodShortWithSingleQuote>());

            titles.Add("Long String, No Single Quotes (" + (new QuoteMethodLongNoSingleQuote()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<QuoteMethodLongNoSingleQuote>());

            titles.Add("Long String, With Single Quotes (" + (new QuoteMethodLongWithSingleQuote()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<QuoteMethodLongWithSingleQuote>());

            titles.Add("Very Long String, No Single Quotes (" + (new QuoteMethodVeryLongNoSingleQuote()).str.Length + ")");
            summaries.Add(BenchmarkRunner.Run<QuoteMethodVeryLongNoSingleQuote>());

            logger.WriteLineHeader("-------------------");
            for (int i = 0; i < summaries.Count; ++i)
            {
                {
                    logger.WriteLine("");
                    logger.WriteLine("");
                    logger.WriteHeader("### " + titles[i]);
                    MarkdownExporter.Console.ExportToLog(summaries[i], logger);
                }
            }

        }
    }


    // All the different string length tests, being careful on the short strings that the string length is exactly the same.
    public static class TestStrings
    {
        // From the benchmarkdotnet site, single quotes removed.
        public static string LongString = "A method which is marked by the [IterationSetup] attribute will be executed only once before each an iteration. It is not recommended to use this attribute in microbenchmarks because it can spoil the results. However, if you are writing a macrobenchmark (e.g. a benchmark which takes at least 100ms) and you want to prepare some data before each iteration, [IterationSetup] can be useful. BenchmarkDotNet does not support setup/cleanup method for a single method invocation (an operation), but you can perform only one operation per iteration. It is recommended to use RunStrategy.Monitoring for such cases. Be careful: if you allocate any objects in the [IterationSetup] method, the MemoryDiagnoser results can also be spoiled. ";


        // From the benchmarkdotnet site, contains single quotes.
        public static string LongStringWithQuote = "A method which is marked by the [IterationSetup] attribute will be executed only once before each an iteration. It's not recommended to use this attribute in microbenchmarks because it can spoil the results. However, if you are writing a macrobenchmark (e.g. a benchmark which takes at least 100ms) and you want to prepare some data before each iteration, [IterationSetup] can be useful. BenchmarkDotNet doesn't support setup/cleanup method for a single method invocation (an operation), but you can perform only one operation per iteration. It's recommended to use RunStrategy.Monitoring for such cases. Be careful: if you allocate any objects in the [IterationSetup] method, the MemoryDiagnoser results can also be spoiled. ";

    }

    public class QuoteMethodVeryShortNoSingleQuote : DbQuoteMethods
    {
        public QuoteMethodVeryShortNoSingleQuote() : base("Four") { }
    }

    public class QuoteMethodVeryShortWithSingleQuote : DbQuoteMethods
    {
        public QuoteMethodVeryShortWithSingleQuote() : base("Fo'r") { }
    }

    public class QuoteMethodShortNoSingleQuote : DbQuoteMethods
    {
        public QuoteMethodShortNoSingleQuote() : base("Hello you World") { }
    }

    public class QuoteMethodShortWithSingleQuote : DbQuoteMethods
    {
        public QuoteMethodShortWithSingleQuote() : base("Hello ye' World") { }
    }

    public class QuoteMethodLongNoSingleQuote : DbQuoteMethods
    {
        public QuoteMethodLongNoSingleQuote() : base(TestStrings.LongString) { }
    }

    public class QuoteMethodLongWithSingleQuote : DbQuoteMethods
    {
        public QuoteMethodLongWithSingleQuote() : base(TestStrings.LongStringWithQuote) { }
    }

    public class QuoteMethodVeryLongNoSingleQuote : DbQuoteMethods
    {
        public QuoteMethodVeryLongNoSingleQuote() : base(TestStrings.LongString
            + TestStrings.LongString + TestStrings.LongString + TestStrings.LongString
            + TestStrings.LongString + TestStrings.LongString + TestStrings.LongString)
        { }
    }


    [CoreJob]
    [RPlotExporter, RankColumn]
    [MemoryDiagnoser]
    public class DbQuoteMethods
    {
        [Params(10000)]
        public int N;

        public string str { get; set; } = string.Empty;

        public DbQuoteMethods() { }

        public DbQuoteMethods(string testString)
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

        /// <summary>
        /// NOT a valid way to quote a string (this method is subject to SQL injection),
        /// but here to see how much overhead the string.Replace method adds compared to
        /// just putting quotes around the string.
        /// 
        /// This is also our baseline to compare against if the source string has no single quotes
        /// nor null/control characters. I.e. in some methods we count the number of single quotes
        /// and control characters and if there are none, we use this for quoting the string. Thus
        /// we know how much of the time in those methods is from simply quoting the string.
        /// </summary>
        [Benchmark]
        public string PlusOperator_NoReplace() => str == null ? "NULL" : "'" + str + "'";


        /// <summary>
        /// Check if calling string.Concat performs differently than the plus operator.
        /// Does NOT replace NULL nor control characters (subject to SQL injection).
        /// </summary>
        [Benchmark]
        public string Concat_NoReplace() => str == null ? "NULL" : string.Concat("'", str, "'");

        /// <summary>
        /// Normal way folks recommend ANSI quoting a string for a database. However this on is still
        /// not quite right because NULL really should not be allowed as a NULL in the string, which
        /// is allowed in c#, Java, etc. could cause a security issue on a database whose language
        /// does use null as the string terminator.
        /// </summary>
        [Benchmark]
        public string PlusOperator_HandleSingleQuote() => str == null ? "NULL" : "'" + str.Replace("'", "''") + "'";

        /// <summary>
        /// Comparing concat to plus operator, does NOT remove NULL nor control characters.
        /// </summary>
        [Benchmark]
        public string Concat_HandleSingleQuote() => str == null ? "NULL" : string.Concat("'", str.Replace("'", "''"), "'");

        /// <summary>
        /// A proper ANSI quote string that also removes nulls.
        /// 
        /// See Common/src/CoreLib/System/String.Manipulation.cs for how Replace is implemented.
        /// </summary>
        [Benchmark]
        public string Baseline_HandleSingleQuote_HandleNull() => str == null ? "NULL" : string.Concat("'", str.Replace("\0", string.Empty).Replace("'", "''''"), "'");


        /// <summary>
        /// Use span and an unsafe block with a stackalloc buffer. This could overflow the stack for
        /// very long string esp. if the call stack is deep.
        /// </summary>
        [Benchmark]
        public string Span_NoCountSpecials_StackallocUnsafe()
        {
            if (str == null) return "NULL";

            var src = str.AsSpan();
            var bufLen = src.Length * 2 + 2;  //If the string was all single quotes, plus starting and ending quote.
            int dest = 1; //starts at 1 because we set the first character to the single quote.
            int i = 0;
            char c; // Temp var outside the loop gives a slight performance boost.
            unsafe
            {
                var cPtr = stackalloc char[bufLen];
                cPtr[0] = '\'';
                for (; i < src.Length; ++i)
                {
                    c = src[i];
                    if (c == '\'')
                    {
                        cPtr[dest + 0] = '\'';
                        cPtr[dest + 1] = '\'';
                        dest += 2;
                    }
                    else if (c <= '\x001f' || (c >= '\x007f' && c <= '\x009f'))
                    {
                        // Silently Ignore.
                        // Calling IsControl adds significant time (almost doubles) the time to loop.
                    }
                    else
                    {
                        cPtr[dest++] = c;
                    }
                }
                cPtr[dest] = '\'';
                return new string(cPtr, 0, dest);
            }
        }


        /// <summary>
        /// Use span an a buffer 2x+2 length of the string and loop over
        /// the characters and return a new string.
        /// </summary>
        [Benchmark]
        public string Span_NoCountSpecials_Safe()
        {
            if (str == null) return "NULL";

            var src = str.AsSpan();
            var bufLen = src.Length * 2 + 2;  //If the string was all single quotes, plus starting and ending quote.
            int dest = 1; //starts at 1 because we set the first character to the single quote.
            char c; // Temp var outside the loop gives a slight performance boost.
            var newChars = new char[bufLen];
            newChars[0] = '\'';
            for (int i = 0; i < src.Length; ++i)
            {
                c = src[i];
                if (c == '\'')
                {
                    newChars[dest + 0] = '\'';
                    newChars[dest + 1] = '\'';
                    dest += 2;
                }
                else if (c <= '\x001f' || (c >= '\x007f' && c <= '\x009f'))
                {
                    // Calling IsControl adds significant time (almost doubles) the time to loop.
                }
                else
                {
                    newChars[dest++] = c;
                }
            }
            newChars[dest] = '\'';
            return new string(newChars, 0, dest);
        }

        /// <summary>
        /// Use span and count the number of characters that will be removed. Always goes on the
        /// short-circuit path as if there was not any single quotes.
        /// </summary>
        [Benchmark]
        public string Span_CountSpecials_ReturnAsIfNone()
        {
            if (str == null) return "NULL";

            char c; // Temp var outside the loop gives a slight performance boost.

            var src = str.AsSpan();
            var singleQuotes = 0;
            var controlChars = 0;
            for (int i = 0; i < src.Length; ++i)
            {
                c = str[i];
                if (c == '\'') ++singleQuotes;
                else if (c <= '\x001f' || (c >= '\x007f' && c <= '\x009f')) ++controlChars;
            }

            if (singleQuotes + controlChars < 20000)
            {
                // Pretend there is nothing to replace, just put quotes around the string.
                return "'" + str + "'";
            }
            throw new ArgumentException("Code path should not be reached");
        }


        /// <summary>
        /// Use span and count the number of characters that will be removed. Always skips
        /// the short-circuit path as if there were single quotes and/or control characters.
        /// </summary>
        [Benchmark]
        public string Span_CountSpecials_ReturnAsIfSome()
        {
            if (str == null) return "NULL";

            char c; // Temp var outside the loop gives a slight performance boost.

            var src = str.AsSpan();
            var singleQuotes = 0;
            var controlChars = 0;
            for (int i = 0; i < src.Length; ++i)
            {
                c = str[i];
                if (c == '\'') ++singleQuotes;
                else if (c <= '\x001f' || (c >= '\x007f' && c <= '\x009f')) ++controlChars;
            }

            if (singleQuotes + controlChars > 20000)
            {
                // Pretend there is something to replace, this path should not get triggered in benchmarking.
                throw new ArgumentException("Code path should not be reached");
            }

            var bufLen = src.Length + (2 * singleQuotes) - controlChars + 2;  //If the string was all single quotes, plus starting and ending quote.
            int dest = 1; //starts at 1 because we set the first character to the single quote.
            var newChars = new char[bufLen];
            newChars[0] = '\'';
            for (int i = 0; i < src.Length; ++i)
            {
                c = src[i];
                if (c == '\'')
                {
                    newChars[dest + 0] = '\'';
                    newChars[dest + 1] = '\'';
                    dest += 2;
                }
                else if (c <= '\x001f' || (c >= '\x007f' && c <= '\x009f'))
                {
                    // Calling IsControl adds significant time (almost doubles) the time to loop.
                }
                else
                {
                    newChars[dest++] = c;
                }
            }
            newChars[dest] = '\'';
            return new string(newChars);
        }
    }
}
