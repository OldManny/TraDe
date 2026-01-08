using Prometheus;

namespace TraDe.Server;

public class TraDeMetrics
{
    // Metric 1: Histogram for Latency
    public static readonly Histogram MatchingDuration = Metrics.CreateHistogram(
        "trade_matching_duration_seconds",
        "Time taken to process a single order match",
        new HistogramConfiguration
        {
            // Custom buckets for high-frequency trading (in seconds)
            Buckets = new[] { 0.000001, 0.00001, 0.00005, 0.0001, 0.0005, 0.001, 0.005 }
        });

    // Metric 2: Counter for Throughput
    public static readonly Counter OrdersProcessed = Metrics.CreateCounter(
        "trade_orders_processed_total",
        "Total number of orders processed by the engine");

    // Metric 3: Gauge for Backpressure
    public static readonly Gauge QueueDepth = Metrics.CreateGauge(
        "trade_channel_queue_depth",
        "Current number of orders waiting in the processing channel");
}