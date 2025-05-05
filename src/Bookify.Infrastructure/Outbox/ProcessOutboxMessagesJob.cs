using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System.Data;

namespace Bookify.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxMessagesJob : IJob
{
    private readonly JsonSerializerSettings JsonSerializerSettings = new()
    { 
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly ISqlConnectionFactory _sqlConncetionFactory;
    private readonly IPublisher _publisher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly OutboxOptions _outboxOptions;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        ISqlConnectionFactory sqlConnectionFactory,
        IPublisher publisher,
        IDateTimeProvider dateTimeProvider,
        OutboxOptions outboxOptions,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _sqlConncetionFactory = sqlConnectionFactory;
        _publisher = publisher;
        _dateTimeProvider = dateTimeProvider;
        _outboxOptions = outboxOptions;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Beginning to process outbox messages");

        using var connection = _sqlConncetionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    JsonSerializerSettings)!;

                await _publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception caughtException)
            {
                _logger.LogError(
                    caughtException,
                    "Exception while processing outbox message {OutboxMessageId}",
                    outboxMessage.Id);

                exception = caughtException;
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }

        transaction.Commit();

        _logger.LogInformation("Completed processing outbox messages");
    }


    private async Task<IEnumerable<OutboxMessageResponse>> GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql = $"""
                    SELECT id, content 
                    FROM outbox_messages
                    WHERE processed_on_utc
                    ORDER BY occurred_on_utc
                    LIMIT {_outboxOptions.BatchSize}
                    FOR UPDATE
                    """;

        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);
        return outboxMessages.ToList();
    }

    private async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        const string sql = @"
                UPDATE outbox_messages
                SET processed_on_utc = @ProcessedOnUtc,
                error = @Error
                WHERE id = @Id";

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOnUtc = _dateTimeProvider.UtcNow,
                Error = exception?.ToString()
            },
            transaction: transaction);
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
