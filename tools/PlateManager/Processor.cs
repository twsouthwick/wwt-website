﻿using System;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace PlateManager
{
    internal class Processor
    {
        private readonly IEnumerable<IWorkItemGenerator> _generators;
        private readonly ILogger<Processor> _logger;

        public Processor(IEnumerable<IWorkItemGenerator> generators, ILogger<Processor> logger)
        {
            _generators = generators;
            _logger = logger;
        }

        public async Task ProcessAsync(ProcessorOptions options, CancellationToken token)
        {
            var files = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
            {
                SingleWriter = true
            });

            foreach (var file in options.Files)
            {
                while (!files.Writer.TryWrite(file))
                {
                }
            }

            files.Writer.Complete();

            var tasks = Channel.CreateBounded<Func<int, int, Task>>(new BoundedChannelOptions(10000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = false,
                SingleWriter = false
            });

            int total = 0;

            var fileProcessing = Enumerable.Range(0, options.FileProcessorCount).Select(async _ =>
            {
                while (!files.Reader.Completion.IsCompleted)
                {
                    var file = await files.Reader.ReadAsync(token);

                    _logger.LogInformation("Adding {File}", file);

                    foreach (var generator in _generators)
                    {
                        foreach (var task in generator.GenerateWorkItems(file, options.BaseUrl, token))
                        {
                            await tasks.Writer.WriteAsync(task, token);

                            Interlocked.Increment(ref total);
                        }
                    }
                }
            }).ToList();

            var c = 0;

            var uploadProcessing = Enumerable.Range(0, options.UploadingCount).Select(async _ =>
            {
                while (!tasks.Reader.Completion.IsCompleted)
                {
                    var count = Interlocked.Increment(ref c);
                    var task = await tasks.Reader.ReadAsync();

                    try
                    {
                        await task(count, total);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected error running task");
                    }
                }
            }).ToList();

            await Task.WhenAll(fileProcessing);

            tasks.Writer.Complete();

            await Task.WhenAll(uploadProcessing);
        }
    }
}
