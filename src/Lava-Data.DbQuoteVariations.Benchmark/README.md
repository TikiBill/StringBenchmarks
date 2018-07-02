# Benchmarking Different ANSI Quote Methods

There are performance reasons to not use prepared statements, which will not be covered here.
(However, unless you are very well versed in preventing [SQL Injection](https://www.owasp.org/index.php/SQL_Injection), use prepared statements. Always.) These benchmarks set to explore different ways to ANSI quote strings along with their performance across different
string lengths.

As with nearly every engineering problem, the "best" solution depends on the expected inputs.
Is the most common input going to be long strings, most often with a single quote? Then it is probably
not worth the overhead of checking and short-circuiting.

## Benchmarks

Below are the benchmark results for different length strings, and some with and without single quotes. These were run
with screen blanking disabled (set to 1 hour) and no other foreground programs running.

```misc
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-4770 CPU 3.40GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3312656 Hz, Resolution=301.8726 ns, Timer=TSC
.NET Core SDK=2.1.300
  [Host] : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Core   : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT

Job=Core  Runtime=Core
```

### Very Short String, No Single Quotes (4)

| Method                                | N     | Mean     | Error     | StdDev    | Rank | Gen 0  | Allocated |
| ------------------------------------- | ----- | -------: | --------: | --------: | ---: | -----: | --------: |
| PlusOperator_NoReplace                | 10000 | 20.19 ns | 0.0476 ns | 0.0445 ns | 3    | 0.0095 | 40 B      |
| Concat_NoReplace                      | 10000 | 19.93 ns | 0.0608 ns | 0.0569 ns | 2    | 0.0095 | 40 B      |
| PlusOperator_HandleSingleQuote        | 10000 | 33.70 ns | 0.0803 ns | 0.0670 ns | 7    | 0.0094 | 40 B      |
| Concat_HandleSingleQuote              | 10000 | 33.94 ns | 0.0374 ns | 0.0350 ns | 8    | 0.0095 | 40 B      |
| Baseline_HandleSingleQuote_HandleNull | 10000 | 49.45 ns | 0.1939 ns | 0.1514 ns | 9    | 0.0095 | 40 B      |
| Span_NoCountSpecials_StackallocUnsafe | 10000 | 17.02 ns | 0.0295 ns | 0.0262 ns | 1    | 0.0095 | 40 B      |
| Span_NoCountSpecials_Safe             | 10000 | 20.25 ns | 0.0358 ns | 0.0335 ns | 4    | 0.0210 | 88 B      |
| Span_CountSpecials_ReturnAsIfNone     | 10000 | 25.88 ns | 0.0569 ns | 0.0504 ns | 6    | 0.0095 | 40 B      |
| Span_CountSpecials_ReturnAsIfSome     | 10000 | 23.43 ns | 0.0906 ns | 0.0756 ns | 5    | 0.0190 | 80 B      |

### Very Short String, With Single Quotes (4)

| Method                                | N     | Mean     | Error     | StdDev    | Rank | Gen 0  | Allocated |
| ------------------------------------- | ----- | -------: | --------: | --------: | ---: | -----: | --------: |
| PlusOperator_NoReplace                | 10000 | 20.21 ns | 0.0729 ns | 0.0570 ns | 2    | 0.0095 | 40 B      |
| Concat_NoReplace                      | 10000 | 20.42 ns | 0.4743 ns | 0.4205 ns | 2    | 0.0095 | 40 B      |
| PlusOperator_HandleSingleQuote        | 10000 | 66.62 ns | 0.1392 ns | 0.1302 ns | 5    | 0.0190 | 80 B      |
| Concat_HandleSingleQuote              | 10000 | 66.62 ns | 0.2997 ns | 0.2803 ns | 5    | 0.0190 | 80 B      |
| Baseline_HandleSingleQuote_HandleNull | 10000 | 83.61 ns | 0.1000 ns | 0.0886 ns | 6    | 0.0209 | 88 B      |
| Span_NoCountSpecials_StackallocUnsafe | 10000 | 16.66 ns | 0.0440 ns | 0.0390 ns | 1    | 0.0095 | 40 B      |
| Span_NoCountSpecials_Safe             | 10000 | 20.60 ns | 0.0237 ns | 0.0185 ns | 2    | 0.0210 | 88 B      |
| Span_CountSpecials_ReturnAsIfNone     | 10000 | 26.33 ns | 0.0688 ns | 0.0575 ns | 4    | 0.0095 | 40 B      |
| Span_CountSpecials_ReturnAsIfSome     | 10000 | 24.06 ns | 0.0237 ns | 0.0171 ns | 3    | 0.0210 | 88 B      |

### Short String, No Single Quotes (15)

| Method                                | N     | Mean     | Error     | StdDev    | Rank | Gen 0  | Allocated |
| ------------------------------------- | ----- | -------: | --------: | --------: | ---: | -----: | --------: |
| PlusOperator_NoReplace                | 10000 | 20.62 ns | 0.0916 ns | 0.0812 ns | 2    | 0.0152 | 64 B      |
| Concat_NoReplace                      | 10000 | 20.34 ns | 0.0580 ns | 0.0514 ns | 1    | 0.0152 | 64 B      |
| PlusOperator_HandleSingleQuote        | 10000 | 44.24 ns | 0.0339 ns | 0.0317 ns | 6    | 0.0152 | 64 B      |
| Concat_HandleSingleQuote              | 10000 | 45.03 ns | 0.3454 ns | 0.3062 ns | 7    | 0.0152 | 64 B      |
| Baseline_HandleSingleQuote_HandleNull | 10000 | 68.15 ns | 0.2069 ns | 0.1834 ns | 9    | 0.0151 | 64 B      |
| Span_NoCountSpecials_StackallocUnsafe | 10000 | 30.16 ns | 0.0963 ns | 0.0854 ns | 3    | 0.0152 | 64 B      |
| Span_NoCountSpecials_Safe             | 10000 | 34.52 ns | 0.0684 ns | 0.0571 ns | 4    | 0.0362 | 152 B     |
| Span_CountSpecials_ReturnAsIfNone     | 10000 | 41.41 ns | 0.1495 ns | 0.1398 ns | 5    | 0.0152 | 64 B      |
| Span_CountSpecials_ReturnAsIfSome     | 10000 | 51.07 ns | 0.1097 ns | 0.1027 ns | 8    | 0.0305 | 128 B     |

### Short String, With Single Quotes (15)

| Method                                | N     | Mean      | Error     | StdDev    | Rank | Gen 0  | Allocated |
| ------------------------------------- | ----- | --------: | --------: | --------: | ---: | -----: | --------: |
| PlusOperator_NoReplace                | 10000 | 20.53 ns  | 0.0518 ns | 0.0485 ns | 2    | 0.0152 | 64 B      |
| Concat_NoReplace                      | 10000 | 20.34 ns  | 0.0425 ns | 0.0377 ns | 1    | 0.0152 | 64 B      |
| PlusOperator_HandleSingleQuote        | 10000 | 77.95 ns  | 0.3352 ns | 0.3135 ns | 8    | 0.0304 | 128 B     |
| Concat_HandleSingleQuote              | 10000 | 77.44 ns  | 0.0801 ns | 0.0749 ns | 7    | 0.0304 | 128 B     |
| Baseline_HandleSingleQuote_HandleNull | 10000 | 102.29 ns | 0.1832 ns | 0.1530 ns | 9    | 0.0323 | 136 B     |
| Span_NoCountSpecials_StackallocUnsafe | 10000 | 30.10 ns  | 0.1894 ns | 0.1771 ns | 3    | 0.0152 | 64 B      |
| Span_NoCountSpecials_Safe             | 10000 | 34.80 ns  | 0.1936 ns | 0.1811 ns | 4    | 0.0362 | 152 B     |
| Span_CountSpecials_ReturnAsIfNone     | 10000 | 38.43 ns  | 0.1254 ns | 0.1112 ns | 5    | 0.0152 | 64 B      |
| Span_CountSpecials_ReturnAsIfSome     | 10000 | 50.96 ns  | 0.1695 ns | 0.1585 ns | 6    | 0.0305 | 128 B     |

### Long String, No Single Quotes (729)

| Method                                | N     | Mean       | Error      | StdDev     | Rank | Gen 0  | Allocated |
| ------------------------------------- | ----- | ---------: | ---------: | ---------: | ---: | -----: | --------: |
| PlusOperator_NoReplace                | 10000 | 114.1 ns   | 0.2273 ns  | 0.2126 ns  | 1    | 0.3546 | 1.45 KB   |
| Concat_NoReplace                      | 10000 | 114.1 ns   | 0.6144 ns  | 0.5747 ns  | 1    | 0.3546 | 1.45 KB   |
| PlusOperator_HandleSingleQuote        | 10000 | 704.4 ns   | 0.9482 ns  | 0.7918 ns  | 2    | 0.3538 | 1.45 KB   |
| Concat_HandleSingleQuote              | 10000 | 704.4 ns   | 2.5743 ns  | 2.4080 ns  | 2    | 0.3538 | 1.45 KB   |
| Baseline_HandleSingleQuote_HandleNull | 10000 | 1,291.1 ns | 1.3280 ns  | 1.0368 ns  | 5    | 0.3529 | 1.45 KB   |
| Span_NoCountSpecials_StackallocUnsafe | 10000 | 797.0 ns   | 1.9486 ns  | 1.6271 ns  | 3    | 0.3538 | 1.45 KB   |
| Span_NoCountSpecials_Safe             | 10000 | 996.0 ns   | 1.7527 ns  | 1.6395 ns  | 4    | 1.0548 | 4.33 KB   |
| Span_CountSpecials_ReturnAsIfNone     | 10000 | 703.8 ns   | 1.2775 ns  | 1.1950 ns  | 2    | 0.3538 | 1.45 KB   |
| Span_CountSpecials_ReturnAsIfSome     | 10000 | 1,547.0 ns | 12.7626 ns | 11.9382 ns | 6    | 0.7076 | 2.91 KB   |

### Long String, With Single Quotes (726)

| Method                                | N     | Mean       | Error     | StdDev    | Rank | Gen 0  | Allocated |
| ------------------------------------- | ----- | ---------: | --------: | --------: | ---: | -----: | --------: |
| PlusOperator_NoReplace                | 10000 | 113.4 ns   | 0.5283 ns | 0.4941 ns | 1    | 0.3546 | 1.45 KB   |
| Concat_NoReplace                      | 10000 | 113.5 ns   | 0.2891 ns | 0.2704 ns | 1    | 0.3546 | 1.45 KB   |
| PlusOperator_HandleSingleQuote        | 10000 | 869.4 ns   | 1.8532 ns | 1.7335 ns | 3    | 0.7086 | 2.91 KB   |
| Concat_HandleSingleQuote              | 10000 | 870.8 ns   | 0.6878 ns | 0.4974 ns | 4    | 0.7086 | 2.91 KB   |
| Baseline_HandleSingleQuote_HandleNull | 10000 | 1,453.3 ns | 3.6188 ns | 3.2080 ns | 7    | 0.7133 | 2.93 KB   |
| Span_NoCountSpecials_StackallocUnsafe | 10000 | 909.8 ns   | 2.4254 ns | 2.2687 ns | 5    | 0.3538 | 1.45 KB   |
| Span_NoCountSpecials_Safe             | 10000 | 1,012.0 ns | 1.7818 ns | 1.4879 ns | 6    | 1.0529 | 4.32 KB   |
| Span_CountSpecials_ReturnAsIfNone     | 10000 | 733.9 ns   | 1.8060 ns | 1.6894 ns | 2    | 0.3538 | 1.45 KB   |
| Span_CountSpecials_ReturnAsIfSome     | 10000 | 1,550.1 ns | 1.0882 ns | 1.0179 ns | 8    | 0.7114 | 2.92 KB   |

### Very Long String, No Single Quotes (5103)

| Method                                | N     | Mean        | Error     | StdDev    | Rank | Gen 0  | Allocated |
| ------------------------------------- | ----- | ----------: | --------: | --------: | ---: | -----: | --------: |
| PlusOperator_NoReplace                | 10000 | 653.4 ns    | 1.313 ns  | 1.164 ns  | 1    | 2.4443 | 10 KB     |
| Concat_NoReplace                      | 10000 | 655.9 ns    | 5.931 ns  | 5.548 ns  | 1    | 2.4443 | 10 KB     |
| PlusOperator_HandleSingleQuote        | 10000 | 4,677.3 ns  | 17.303 ns | 15.339 ns | 2    | 2.4414 | 10 KB     |
| Concat_HandleSingleQuote              | 10000 | 4,669.5 ns  | 5.745 ns  | 5.374 ns  | 2    | 2.4414 | 10 KB     |
| Baseline_HandleSingleQuote_HandleNull | 10000 | 8,693.2 ns  | 23.070 ns | 19.265 ns | 6    | 2.4414 | 10 KB     |
| Span_NoCountSpecials_StackallocUnsafe | 10000 | 6,474.5 ns  | 93.120 ns | 87.104 ns | 4    | 2.4414 | 10 KB     |
| Span_NoCountSpecials_Safe             | 10000 | 6,703.4 ns  | 24.737 ns | 20.657 ns | 5    | 7.2937 | 29.96 KB  |
| Span_CountSpecials_ReturnAsIfNone     | 10000 | 4,761.5 ns  | 7.053 ns  | 6.597 ns  | 3    | 2.4414 | 10 KB     |
| Span_CountSpecials_ReturnAsIfSome     | 10000 | 10,691.3 ns | 41.715 ns | 36.979 ns | 7    | 4.8828 | 20 KB     |
