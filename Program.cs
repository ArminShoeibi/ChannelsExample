using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelsExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Channel<uint> unsignedIntegersChannel = Channel.CreateUnbounded<uint>();
            CancellationTokenSource cts = new CancellationTokenSource();

            Task writerTask = Task.Run(async () =>
            {
                for (uint i = uint.MinValue; i <= uint.MaxValue; i++)
                {
                    uint tempIndex = i;
                    Console.WriteLine($"Publishing: {tempIndex}");
                    await unsignedIntegersChannel.Writer.WriteAsync(tempIndex);

                    if (tempIndex == uint.MaxValue) // if you are reading this code and want to test it, change the right hand value to 100-1000
                    {
                        unsignedIntegersChannel.Writer.Complete();
                    }
                }

            }, cts.Token);

            Task readerTask = Task.Run(async () =>
            {
                while (true)
                {
                    bool isCompleted = unsignedIntegersChannel.Reader.Completion.IsCompleted;
                    if (isCompleted is false)
                    {
                        uint subscribedValue = await unsignedIntegersChannel.Reader.ReadAsync();
                        Console.WriteLine($"Subscribing: {subscribedValue}");
                    }
                    else
                    {
                        cts.CancelAfter(TimeSpan.FromSeconds(10));
                    }
                    // OR
                    //await foreach (var subscribedValue in unsignedIntegersChannel.Reader.ReadAllAsync())
                    //{
                    //    Console.WriteLine($"Subscribing: {subscribedValue}");
                    //}
                }
            }, cts.Token);

            await Task.WhenAny(writerTask, readerTask);
        }
    }
}
