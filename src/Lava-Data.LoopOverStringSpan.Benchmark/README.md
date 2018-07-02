# Benchmark Temp Variables, Classifying Characters, and Copying a String

This benchmark sets to answer the questions:

1. If one is referencing a character by span[i] multiple times in a loop, is it faster to use a temporary
    variable, and does it matter if that variable is defined inside the loop?
2. What is the baseline time to scan a string for a couple different characters of interest?
3. How much time overhead does calling Char.IsCharClass (IsControl in this test) add?
4. How much time overhead does copying a span into a character array while altering two characters have?
5. Does the copy time change with different temporary character strategies?
6. How does using a stackalloc character array compare (in time, copying to the character array) to a heap allocated character array?

The benchmarks are run over four string lengths to determine if string length effects the ranking of each.
(Obviously it is expected that longer string will take longer time to scan and copy so it is the relative
time between the same-length string which we are interested.)

## Results

```misc
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-4770 CPU 3.40GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3312658 Hz, Resolution=301.8724 ns, Timer=TSC
.NET Core SDK=2.1.300
  [Host] : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Core   : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT

Job=Core  Runtime=Core
```

### Very Short String (4)

| Method                                 | N     | Mean       | Error     | StdDev    | Rank |
| -------------------------------------- | ----- | ---------: | --------: | --------: | ---: |
| CheckStringLength                      | 10000 | 0.4017 ns  | 0.0021 ns | 0.0020 ns | 1    |
| CountQuotes_CharBySpanIndex            | 10000 | 8.9156 ns  | 0.0243 ns | 0.0215 ns | 4    |
| CountQuotes_CharBySpanIndex_Lambda     | 10000 | 9.4216 ns  | 0.0083 ns | 0.0078 ns | 7    |
| CountQuotes_TempCharInLoop             | 10000 | 8.8985 ns  | 0.0086 ns | 0.0077 ns | 3    |
| CountQuotes_TempCharOutsideLoop        | 10000 | 8.8921 ns  | 0.0077 ns | 0.0069 ns | 3    |
| CountQuotesCtrlRangeLookup_BySpanIndex | 10000 | 8.9668 ns  | 0.0145 ns | 0.0121 ns | 5    |
| CountQuotesCtrlLocalLookup_BySpanIndex | 10000 | 13.1351 ns | 0.0108 ns | 0.0101 ns | 8    |
| CountQuotesCtrl_BySpanIndex            | 10000 | 14.1452 ns | 0.0468 ns | 0.0366 ns | 10   |
| CountQuotesCtrl_TempCharInLoop         | 10000 | 13.3180 ns | 0.0218 ns | 0.0204 ns | 9    |
| CountQuotesCtrl_TempCharOutsideLoop    | 10000 | 13.3293 ns | 0.0090 ns | 0.0075 ns | 9    |
| CopyString_SpanIndex                   | 10000 | 8.8951 ns  | 0.0141 ns | 0.0110 ns | 3    |
| CopyString_TempCharInLoop              | 10000 | 8.9803 ns  | 0.0363 ns | 0.0340 ns | 5    |
| CopyString_TempCharOutsideLoop         | 10000 | 9.0182 ns  | 0.0106 ns | 0.0094 ns | 6    |
| CopyString_SpanIndex_Unsafe            | 10000 | 7.0566 ns  | 0.0255 ns | 0.0226 ns | 2    |

### Short String (15)

| Method                                 | N     | Mean       | Error     | StdDev    | Rank |
| -------------------------------------- | ----- | ---------: | --------: | --------: | ---: |
| CheckStringLength                      | 10000 | 0.4004 ns  | 0.0031 ns | 0.0029 ns | 1    |
| CountQuotes_CharBySpanIndex            | 10000 | 17.3253 ns | 0.0657 ns | 0.0615 ns | 3    |
| CountQuotes_CharBySpanIndex_Lambda     | 10000 | 17.8230 ns | 0.0229 ns | 0.0214 ns | 4    |
| CountQuotes_TempCharInLoop             | 10000 | 17.3052 ns | 0.0486 ns | 0.0431 ns | 3    |
| CountQuotes_TempCharOutsideLoop        | 10000 | 17.1774 ns | 0.0563 ns | 0.0527 ns | 2    |
| CountQuotesCtrlRangeLookup_BySpanIndex | 10000 | 18.3850 ns | 0.0667 ns | 0.0624 ns | 5    |
| CountQuotesCtrlLocalLookup_BySpanIndex | 10000 | 32.1279 ns | 0.1825 ns | 0.1707 ns | 9    |
| CountQuotesCtrl_BySpanIndex            | 10000 | 35.3569 ns | 0.0282 ns | 0.0264 ns | 10   |
| CountQuotesCtrl_TempCharInLoop         | 10000 | 36.1721 ns | 0.0564 ns | 0.0527 ns | 11   |
| CountQuotesCtrl_TempCharOutsideLoop    | 10000 | 35.3458 ns | 0.0261 ns | 0.0231 ns | 10   |
| CopyString_SpanIndex                   | 10000 | 23.9304 ns | 0.1313 ns | 0.1025 ns | 8    |
| CopyString_TempCharInLoop              | 10000 | 23.9259 ns | 0.0338 ns | 0.0316 ns | 8    |
| CopyString_TempCharOutsideLoop         | 10000 | 23.8839 ns | 0.0538 ns | 0.0503 ns | 7    |
| CopyString_SpanIndex_Unsafe            | 10000 | 21.5046 ns | 0.0249 ns | 0.0221 ns | 6    |

### Long String (726)

| Method                                 | N     | Mean          | Error     | StdDev    | Rank |
| -------------------------------------- | ----- | ------------: | --------: | --------: | ---: |
| CheckStringLength                      | 10000 | 0.3834 ns     | 0.0032 ns | 0.0030 ns | 1    |
| CountQuotes_CharBySpanIndex            | 10000 | 801.7362 ns   | 1.9299 ns | 1.8052 ns | 3    |
| CountQuotes_CharBySpanIndex_Lambda     | 10000 | 802.7637 ns   | 1.1323 ns | 1.0038 ns | 4    |
| CountQuotes_TempCharInLoop             | 10000 | 801.7514 ns   | 0.9323 ns | 0.8720 ns | 3    |
| CountQuotes_TempCharOutsideLoop        | 10000 | 801.7403 ns   | 0.8795 ns | 0.8227 ns | 3    |
| CountQuotesCtrlRangeLookup_BySpanIndex | 10000 | 622.6341 ns   | 2.1516 ns | 2.0126 ns | 2    |
| CountQuotesCtrlLocalLookup_BySpanIndex | 10000 | 1,548.3170 ns | 0.8551 ns | 0.7580 ns | 8    |
| CountQuotesCtrl_BySpanIndex            | 10000 | 1,743.9412 ns | 2.1788 ns | 2.0380 ns | 10   |
| CountQuotesCtrl_TempCharInLoop         | 10000 | 1,737.6425 ns | 5.9510 ns | 5.5665 ns | 9    |
| CountQuotesCtrl_TempCharOutsideLoop    | 10000 | 1,743.5559 ns | 1.7176 ns | 1.5226 ns | 10   |
| CopyString_SpanIndex                   | 10000 | 999.8718 ns   | 1.6796 ns | 1.4889 ns | 6    |
| CopyString_TempCharInLoop              | 10000 | 996.2968 ns   | 1.8406 ns | 1.6316 ns | 5    |
| CopyString_TempCharOutsideLoop         | 10000 | 1,000.7873 ns | 2.5172 ns | 2.2314 ns | 6    |
| CopyString_SpanIndex_Unsafe            | 10000 | 1,017.4773 ns | 1.7178 ns | 1.5227 ns | 7    |

### Longer String (5082)

| Method                                 | N     | Mean           | Error      | StdDev     | Rank |
| -------------------------------------- | ----- | -------------: | ---------: | ---------: | ---: |
| CheckStringLength                      | 10000 | 0.3803 ns      | 0.0024 ns  | 0.0023 ns  | 1    |
| CountQuotes_CharBySpanIndex            | 10000 | 5,539.6184 ns  | 25.4822 ns | 23.8361 ns | 3    |
| CountQuotes_CharBySpanIndex_Lambda     | 10000 | 5,528.9804 ns  | 17.2488 ns | 14.4035 ns | 3    |
| CountQuotes_TempCharInLoop             | 10000 | 5,538.2572 ns  | 15.9563 ns | 14.9255 ns | 3    |
| CountQuotes_TempCharOutsideLoop        | 10000 | 5,525.1029 ns  | 15.0058 ns | 14.0365 ns | 3    |
| CountQuotesCtrlRangeLookup_BySpanIndex | 10000 | 4,229.3681 ns  | 13.7865 ns | 11.5124 ns | 2    |
| CountQuotesCtrlLocalLookup_BySpanIndex | 10000 | 10,766.7797 ns | 10.2193 ns | 9.5591 ns  | 7    |
| CountQuotesCtrl_BySpanIndex            | 10000 | 12,127.2281 ns | 19.5313 ns | 15.2488 ns | 8    |
| CountQuotesCtrl_TempCharInLoop         | 10000 | 12,117.2160 ns | 10.8792 ns | 10.1764 ns | 8    |
| CountQuotesCtrl_TempCharOutsideLoop    | 10000 | 12,143.7959 ns | 29.2083 ns | 27.3214 ns | 8    |
| CopyString_SpanIndex                   | 10000 | 7,289.2902 ns  | 9.3612 ns  | 7.8170 ns  | 5    |
| CopyString_TempCharInLoop              | 10000 | 7,303.7052 ns  | 23.6255 ns | 20.9434 ns | 6    |
| CopyString_TempCharOutsideLoop         | 10000 | 7,288.1622 ns  | 17.7972 ns | 16.6475 ns | 5    |
| CopyString_SpanIndex_Unsafe            | 10000 | 7,263.2342 ns  | 7.8388 ns  | 7.3324 ns  | 4    |

## Observations

1. When the same index value is referenced multiple times, having a temporary variable defined outside
    the loop does give a minuscule performance improvement. (Side project: Have the JIT make this optimization
    if the value is only ever read.) My guess (untested) is that the more dereferencing that happens in a loop,
    the greater the impact. My use case only needs to dereference a few times so that is what I am testing.
2. Calling Char.IsClass is costly and having a local lookup table (which is how IsControl works internally)
    does not offer significant performance improvement. Using a range to determine if a character is a control
    character is fast, and in some cases faster then without.
    (It is unclear as to why it is faster with more comparisons. I need to research this more.)
3. Using an unsafe stackalloc for a character array is normally faster, and if run with the MemoryDiagnoser configuration,
    it can be seen that the copy does not cause a GC nor allocation (as to be expected).
