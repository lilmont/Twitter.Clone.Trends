﻿namespace Twitter.Clone.Trends.EventHandler;

public class HashtagConsumer(InboxHashtagRepository inboxHashtagRepository,
    ILogger<HashtagConsumer> logger)
    : IConsumer<HashtagsEvent>
{
    private readonly InboxHashtagRepository _inboxHashtagRepository = inboxHashtagRepository;
    private readonly ILogger<HashtagConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<HashtagsEvent> context)
    {
        HashtagEventValidator validator = new();
        try
        {
            var validationResult = validator.Validate(context.Message);
            if (!validationResult.IsValid)
                throw new Exception(validationResult.Errors.ToString());

            await _inboxHashtagRepository.CreateAsync(Inbox.CreateMessage(context.Message), context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while processing the message with IP: {IPAddress}",
                context.Message.IPAddress);
        }
    }
}
