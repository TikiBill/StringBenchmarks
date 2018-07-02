
# Benchmarking Character Array to String Conversions

In c# (and many other languages), strings are immutable. While this topic has been covered
elsewhere in depth, the takeaway for the purposes of this experiment is that manipulating strings means either creating new strings
(i.e. by concatenating them together), using [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder?view=netcore-2.1),
or using a character array.

These benchmarks explore if all character array to string conversions are equal.
You will find the tests in Program.cs.

You will find the the implementation of `new string()`, along with the definitions of `wcslen` and `wstrcpy` in
[coreclr/src/System.Private.CoreLib/shared/System/String.cs](https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/String.cs).

## Benchmark Results

---------------------------------------------------

```misc
Total time: 00:08:17 (497.33 sec)

// * Summary *

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-4770 CPU 3.40GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3312659 Hz, Resolution=301.8723 ns, Timer=TSC
.NET Core SDK=2.1.300
  [Host] : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Core   : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT

Job=Core  Runtime=Core
```

| Method                                   | N    | Mean         | Error      | StdDev     | Rank |
| ---------------------------------------- | ---- | -----------: | ---------: | ---------: | ---: |
| CharArrayAsSpanToString_Short            | 1000 | 10.653 ns    | 0.0758 ns  | 0.0633 ns  | 5    |
| NewStringCharArray_Short                 | 1000 | 8.865 ns     | 0.2394 ns  | 0.1999 ns  | 1    |
| NewStringCharArrayStartLen_Short         | 1000 | 10.190 ns    | 0.2178 ns  | 0.2037 ns  | 4    |
| NewStringCharArrayAsSpan_Short           | 1000 | 9.216 ns     | 0.0325 ns  | 0.0272 ns  | 2    |
| NewStringCharArrayAsSpanSliced_Short     | 1000 | 9.863 ns     | 0.2822 ns  | 0.2898 ns  | 3    |
| CharArrayAsSpanToString_Medium           | 1000 | 33.105 ns    | 0.7363 ns  | 1.0079 ns  | 7    |
| NewStringCharArray_Medium                | 1000 | 31.294 ns    | 0.1893 ns  | 0.1678 ns  | 6    |
| NewStringCharArrayStartLen_Medium        | 1000 | 33.908 ns    | 0.7400 ns  | 0.6922 ns  | 8    |
| NewStringCharArrayAsSpan_Medium          | 1000 | 32.714 ns    | 0.7146 ns  | 1.0474 ns  | 7    |
| NewStringCharArrayAsSpanSliced_Medium    | 1000 | 32.623 ns    | 0.4977 ns  | 0.4412 ns  | 7    |
| NewStringCharArrayChangeChar_Long        | 1000 | 385.077 ns   | 8.4571 ns  | 7.4970 ns  | 10   |
| CharArrayAsSpanToString_Long             | 1000 | 381.592 ns   | 5.2652 ns  | 4.9251 ns  | 10   |
| CharArrayAsSpanSliceToString_Long        | 1000 | 380.459 ns   | 7.2870 ns  | 7.1568 ns  | 10   |
| NewStringCharArray_Long                  | 1000 | 379.758 ns   | 7.1600 ns  | 6.6975 ns  | 10   |
| NewStringCharArrayStartLen_Long          | 1000 | 380.768 ns   | 7.7480 ns  | 9.7987 ns  | 10   |
| NewStringStringAsSpan_Long               | 1000 | 366.176 ns   | 3.4921 ns  | 3.2665 ns  | 9    |
| NewStringCharArrayAsSpan_Long            | 1000 | 372.877 ns   | 2.5139 ns  | 2.2285 ns  | 10   |
| NewStringCharArrayAsSpanSliced_Long      | 1000 | 376.105 ns   | 6.4214 ns  | 6.0066 ns  | 10   |
| NewStringCharPtr_Long                    | 1000 | 724.900 ns   | 4.7094 ns  | 4.4052 ns  | 11   |
| NewStringCharPtrStartLen_Long            | 1000 | 371.189 ns   | 3.3777 ns  | 3.1595 ns  | 10   |
| NewStringCharArrayCopyAsString_Long      | 1000 | 1,684.567 ns | 21.0790 ns | 19.7173 ns | 12   |
| NewStringCharArrayStackCopyAsString_Long | 1000 | 2,217.260 ns | 25.4193 ns | 22.5335 ns | 13   |

## Takeaways

* __Creating a string from a char* is slow without the start and length.__

    This makes sense since the conversion code scans for a null-terminator. All of the
    other methods copy based on the known length of a string, and nulls are allowed within strings.
    (As a side note that also means that one must be aware if a .NET string is getting passed to another
    language/system that uses null as the string terminator.)

* __Otherwise conversion of char to string is dependent on the length.__

    This makes sense since they are all bound by how quickly they can copy the data.

* __Span and Slice are Fast!__

    As they are intended to be.
