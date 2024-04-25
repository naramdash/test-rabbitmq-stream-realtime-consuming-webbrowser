namespace Services

open System
open System.Net
open System.Text.Json
open System.Threading.Tasks

open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.SignalR

open RabbitMQ.Stream.Client
open RabbitMQ.Stream.Client.Reliable
open Hubs
open Types
open System.Text


type StreamConsumeService(broadcastHub: IHubContext<BroadcastHub>) =
    let endPoint = DnsEndPoint("localhost", 5552)

    let config =
        StreamSystemConfig(
            UserName = "guest",
            Password = "guest",
            Endpoints = [| endPoint |],
            AddressResolver = AddressResolver(endPoint)
        )


    let streamName = "iot-sensor-v34-status"

    do printfn "StreamConsumerService created"

    let mutable streamSystem: StreamSystem option = Option.None
    let mutable consumer: Consumer option = Option.None

    interface IHostedService with
        member this.StartAsync(cancellationToken) =
            backgroundTask {
                let! ss = StreamSystem.Create(config)
                streamSystem <- Some ss

                let config =
                    ConsumerConfig(
                        ss,
                        streamName,
                        Reference = "StreamConsumer-DOTNET",
                        OffsetSpec = OffsetTypeNext(),
                        MessageHandler =
                            Func<string, RawConsumer, MessageContext, RabbitMQ.Stream.Client.Message, Task>
                                (fun _ _ _ msg ->
                                    backgroundTask {
                                        let raw =
                                            if msg.AmqpValue = null then
                                                msg.Data.Contents.FirstSpan.ToArray() |> Encoding.UTF8.GetString
                                            else
                                                msg.AmqpValue :?> string

                                        let msg = JsonSerializer.Deserialize<Message>(raw)

                                        printfn "stream to signalr: %A" msg.tick

                                        broadcastHub.Clients.All.SendAsync("Pub", msg) |> ignore
                                    })

                    )

                let! c = Consumer.Create(config)
                consumer <- Some c

                printfn "StreamConsumerService started"
            }

        member this.StopAsync(cancellationToken) =
            backgroundTask {
                if consumer.IsSome then
                    do! consumer.Value.Close()

                if streamSystem.IsSome then
                    do! streamSystem.Value.Close()

                printfn "StreamConsumerService stopped"
            }
