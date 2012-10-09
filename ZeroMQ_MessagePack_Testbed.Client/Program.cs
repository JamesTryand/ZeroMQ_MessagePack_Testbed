using System;
using System.IO;
using MsgPack.Serialization;
using ZeroMQ_MessagePack_Testbed.Models;
using ZMQ;

namespace ZeroMQ_MessagePack_Testbed
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new Context(1))
            {
                using (var subscriber = context.Socket(SocketType.SUB))
                {
                    subscriber.Subscribe(new byte[] { 7, 7, 7, 7 });
                    subscriber.Connect("tcp://localhost:5556");

                    var serializer = MessagePackSerializer.Create<Message>();

                    while (true)
                    {
                        var response = subscriber.Recv();

                        using (var messageStream = new MemoryStream(response))
                        {
                            //trim the 4 byte header off
                            messageStream.Seek(4, SeekOrigin.Begin);

                            //deserialize from the stream
                            var message = serializer.Unpack(messageStream);

                            Console.WriteLine("CLIENT: {0} {1} {2}", message.Id, message.Timestamp, DateTime.UtcNow);
                        }
                    }
                }
            }
        }
    }
}