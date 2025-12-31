using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Columns;
using TraDe.Benchmarks;

// Using P95 and Max to define the tail latency profile
var config = ManualConfig.Create(DefaultConfig.Instance)
    .AddColumn(StatisticColumn.P95)
    .AddColumn(StatisticColumn.Max);

BenchmarkRunner.Run<MatchingBenchmarks>(config);