using JasperFx.Core;
using Marten.Events.Daemon;
using Marten.Events.Projections;
using Marten.Exceptions;
using Npgsql;

namespace DocumentManagementStore.Common;

public static class MartenExtensions
{
    public static void AddDefaultProjectionExceptionHandling(
        this ProjectionOptions projectionOptions,
        ILogger logger
    )
    {
        projectionOptions
            .OnException<EventFetcherException>(e =>
            {
                logger.LogError(e, "EventFetcherException");
                return true;
            })
            .RetryLater(250.Milliseconds(), 500.Milliseconds(), 1.Seconds())
            .Then.Pause(30.Seconds());
        projectionOptions
            .OnException<ShardStopException>(e =>
            {
                logger.LogError(e, "ShardStopException");
                return true;
            })
            .DoNothing();
        projectionOptions
            .OnException<ShardStartException>(e =>
            {
                logger.LogError(e, "ShardStartException");
                return true;
            })
            .RetryLater(250.Milliseconds(), 500.Milliseconds(), 1.Seconds())
            .Then.DoNothing();
        projectionOptions
            .OnException<NpgsqlException>(e =>
            {
                logger.LogError(e, "NpgsqlException");
                return true;
            })
            .RetryLater(250.Milliseconds(), 500.Milliseconds(), 1.Seconds())
            .Then.Pause(30.Seconds());
        projectionOptions
            .OnException<MartenCommandException>(e =>
            {
                logger.LogError(e, "MartenCommandException");
                return true;
            })
            .RetryLater(250.Milliseconds(), 500.Milliseconds(), 1.Seconds())
            .Then.Pause(30.Seconds());
        projectionOptions
            .OnException<ProgressionProgressOutOfOrderException>(e =>
            {
                logger.LogInformation(e, "ProgressionProgressOutOfOrderException");
                return true;
            })
            .Pause(10.Seconds());
        projectionOptions
            .OnException<OperationCanceledException>(e =>
            {
                logger.LogError(e, "OperationCanceledException");
                return true;
            })
            .Stop();
    }
}