using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelsExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Channel<uint> unsignedIntegersChannel = Channel.CreateUnbounded<uint>();

            Task writerTask = Task.Run(async () =>
            {
                for (uint i = uint.MinValue; i <= uint.MaxValue; i++)
                {
                    uint tempIndex = i;
                    Console.WriteLine($"Publishing: {tempIndex}");
                    await unsignedIntegersChannel.Writer.WriteAsync(tempIndex);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (tempIndex == uint.MaxValue)
                    {
                        unsignedIntegersChannel.Writer.Complete();
                    }
                }

            });

            Task readerTask = Task.Run(async () =>
            {
                while (true)
                {
                    uint subscribedValue = await unsignedIntegersChannel.Reader.ReadAsync();
                    Console.WriteLine($"Subscribing: {subscribedValue}");
                    // OR
                    //await foreach (var subscribedValue in unsignedIntegersChannel.Reader.ReadAllAsync())
                    //{
                    //    Console.WriteLine($"Subscribing: {subscribedValue}");
                    //}
                }
            });

            await Task.WhenAll(writerTask, readerTask);
        }
    }
}
