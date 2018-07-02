# Benchmark GC with a Character Array in Stack vs. Heap

```misc
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-4770 CPU 3.40GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3312658 Hz, Resolution=301.8724 ns, Timer=TSC
.NET Core SDK=2.1.300
  [Host] : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Core   : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT

Job=Core  Runtime=Core
```

*****     Very Short String (4)    *****

| Method                                | N     | Mean     | Error     | StdDev    | Rank | Gen 0  | Allocated |
| ------------------------------------- | ----- | -------: | --------: | --------: | ---: | -----: | --------: |
| CopyString_TempCharOutsideLoop_String | 10000 | 17.74 ns | 0.0217 ns | 0.0193 ns | 2    | 0.0171 | 72 B      |
| CopyString_SpanIndex_Unsafe_String    | 10000 | 15.02 ns | 0.0484 ns | 0.0404 ns | 1    | 0.0095 | 40 B      |

*****     Short String (15)    *****

| Method                                | N     | Mean     | Error     | StdDev    | Rank | Gen 0  | Allocated |
| ------------------------------------- | ----- | -------: | --------: | --------: | ---: | -----: | --------: |
| CopyString_TempCharOutsideLoop_String | 10000 | 29.85 ns | 0.0291 ns | 0.0258 ns | 2    | 0.0266 | 112 B     |
| CopyString_SpanIndex_Unsafe_String    | 10000 | 27.86 ns | 0.1215 ns | 0.1077 ns | 1    | 0.0133 | 56 B      |

*****     Long String (726)    *****

| Method                                | N     | Mean     | Error     | StdDev    | Rank | Gen 0  | Allocated |
| ------------------------------------- | ----- | -------: | --------: | --------: | ---: | -----: | --------: |
| CopyString_TempCharOutsideLoop_String | 10000 | 1.085 us | 0.0148 us | 0.0138 us | 1    | 0.7038 | 2.89 KB   |
| CopyString_SpanIndex_Unsafe_String    | 10000 | 1.218 us | 0.0042 us | 0.0040 us | 2    | 0.3510 | 1.45 KB   |

*****     Longer String (5082)    *****

| Method                                | N     | Mean     | Error     | StdDev    | Rank | Gen 0  | Allocated |
| ------------------------------------- | ----- | -------: | --------: | --------: | ---: | -----: | --------: |
| CopyString_TempCharOutsideLoop_String | 10000 | 7.439 us | 0.0403 us | 0.0377 us | 1    | 4.8599 | 19.91 KB  |
| CopyString_SpanIndex_Unsafe_String    | 10000 | 8.162 us | 0.0199 us | 0.0166 us | 2    | 2.4261 | 9.95 KB   |

## Observations

As expected, the stackalloc character array has less managed memory allocated and lower GC usage.

Also, as seen before, the stackalloc version can be slower for longer strings, although
it is unclear to me as to why that is the case.
