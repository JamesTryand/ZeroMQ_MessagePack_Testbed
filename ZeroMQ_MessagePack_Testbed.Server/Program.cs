using System;
using System.IO;
using System.Threading;
using MsgPack.Serialization;
using ZeroMQ_MessagePack_Testbed.Models;
using ZMQ;

namespace ZeroMQ_MessagePack_Testbed.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new Context(1))
            {
                using (var publisher = context.Socket(SocketType.PUB))
                {
                    publisher.Bind("tcp://*:5556");

                    var randomizer = new Random(DateTime.Now.Millisecond);

                    var serializer = MessagePackSerializer.Create<Message>();

                    while (true)
                    {
                        Thread.Sleep(500);

                        var message = new Message
                        {
                            Id = Guid.NewGuid(),
                            Priority = Priority.Medium,
                            Subject = "Test Message" + randomizer.Next(0, 100),
                            Body = "This is a test message " + randomizer.Next(0, 100000),
                            Timestamp = DateTime.UtcNow
                        };

                        Console.WriteLine("SERVER: {0} {1}", message.Id, message.Timestamp);

                        using (var messageStream = new MemoryStream())
                        {
                            //write 4 byte client id first
                            messageStream.Write(new byte[] { 7, 7, 7, 7 }, 0, 4);

                            //write the packed message to the stream
                            serializer.Pack(messageStream, message);

                            //   Send message to 0..N subscribers via a pub socket
                            publisher.Send(messageStream.GetBuffer());
                        }
                    }
                }
            }
        }
    }
}