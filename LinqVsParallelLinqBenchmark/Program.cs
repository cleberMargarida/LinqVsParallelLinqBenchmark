using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Bogus;
using System;
using System.Linq;

/// <summary>
/// Entry point for the benchmark application.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<LinqVsParallelLinq>();
    }
}

// <summary>
/// Class for benchmarking LINQ vs Parallel LINQ operations.
/// </summary>
[MemoryDiagnoser]
public class LinqVsParallelLinq
{
    private string[] _shortCollection;
    private string[] _largeCollection;

    /// <summary>
    /// Sets up the benchmarking environment. Initializes collections with random words.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        // Seed ensures the random generation is consistent across runs
        Randomizer.Seed = new Random(8675309);
        // Short collection for quick operations and to see how overhead affects smaller datasets
        _shortCollection = new Faker().Random.WordsArray(10);
        // Large collection for testing the scalability and performance in larger datasets
        _largeCollection = new Faker().Random.WordsArray(100000);
    }

    /// <summary>
    /// Benchmarks parallel OrderBy operation on a short collection.
    /// Parallel LINQ can optimize sorting operations by distributing the workload across multiple threads.
    /// </summary>
    [Benchmark]
    public string[] Parallel_OrderBy_Short()
    {
        return _shortCollection.AsParallel().OrderBy(x => x).ToArray();
    }

    /// <summary>
    /// Benchmarks sequential OrderBy operation on a short collection.
    /// Regular LINQ operates in a single thread, which can be more efficient for small datasets due to lower overhead.
    /// </summary>
    [Benchmark]
    public string[] Sequential_OrderBy_Short()
    {
        return _shortCollection.OrderBy(x => x).ToArray();
    }

    /// <summary>
    /// Benchmarks parallel OrderBy operation on a large collection.
    /// This is where PLINQ can significantly outperform regular LINQ by using multiple cores for large datasets.
    /// </summary>
    [Benchmark]
    public string[] Parallel_OrderBy_Large()
    {
        return _largeCollection.AsParallel().OrderBy(x => x).ToArray();
    }

    /// <summary>
    /// Benchmarks sequential OrderBy operation on a large collection.
    /// This serves as a baseline to compare against the parallel version.
    /// </summary>
    [Benchmark]
    public string[] Sequential_OrderBy_Large()
    {
        return _largeCollection.OrderBy(x => x).ToArray();
    }

    /// <summary>
    /// Benchmarks parallel Where operation on a short collection.
    /// In this operation, PLINQ can filter elements across multiple threads.
    /// However, for small datasets, the overhead might not justify parallelization.
    /// </summary>
    [Benchmark]
    public string[] Parallel_Where_Short()
    {
        return _shortCollection.AsParallel().Where(x => x.Length > 4).ToArray();
    }

    /// <summary>
    /// Benchmarks sequential Where operation on a short collection.
    /// Regular LINQ processes the filter operation in a single thread, which can be more efficient for small datasets.
    /// </summary>
    [Benchmark]
    public string[] Sequential_Where_Short()
    {
        return _shortCollection.Where(x => x.Length > 4).ToArray();
    }

    /// <summary>
    /// Benchmarks parallel Where operation on a large collection.
    /// PLINQ can efficiently handle large datasets by filtering elements concurrently.
    /// </summary>
    [Benchmark]
    public string[] Parallel_Where_Large()
    {
        return _largeCollection.AsParallel().Where(x => x.Length > 4).ToArray();
    }

    /// <summary>
    /// Benchmarks sequential Where operation on a large collection.
    /// This provides a performance comparison point to assess the effectiveness of parallel processing.
    /// </summary>
    [Benchmark]
    public string[] Sequential_Where_Large()
    {
        return _largeCollection.Where(x => x.Length > 4).ToArray();
    }

    /// <summary>
    /// Benchmarks parallel Aggregate operation on a short collection.
    /// PLINQ's Aggregate uses multiple threads to accumulate results, but overhead might outweigh benefits for small datasets.
    /// </summary>
    [Benchmark]
    public string Parallel_Aggregate_Short()
    {
        return _shortCollection.AsParallel().Aggregate((x, y) => x + y);
    }

    /// <summary>
    /// Benchmarks sequential Aggregate operation on a short collection.
    /// Regular LINQ processes accumulation in a single thread, which can be more efficient due to less overhead.
    /// </summary>
    [Benchmark]
    public string Sequential_Aggregate_Short()
    {
        return _shortCollection.Aggregate((x, y) => x + y);
    }

    /// <summary>
    /// Benchmarks parallel Aggregate operation on a large collection.
    /// PLINQ can improve performance for large datasets by performing the accumulation concurrently across multiple threads.
    /// </summary>
    [Benchmark]
    public string Parallel_Aggregate_Large()
    {
        return _largeCollection.AsParallel().Aggregate((x, y) => x + y);
    }

    /// <summary>
    /// Benchmarks sequential Aggregate operation on a large collection.
    /// Serves as a baseline for comparing the performance gain, if any, of using parallel processing.
    /// </summary>
    [Benchmark]
    public string Sequential_Aggregate_Large()
    {
        return _largeCollection.Aggregate((x, y) => x + y);
    }
}