namespace Services

#nowarn "20"

open MQTTnet
open MQTTnet.Client
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.SignalR
open Hubs
open System.Text.Json
open Types


type MQTTSubscribeService(broadcastHub: IHubContext<BroadcastHub>) =
    let client = (new MqttFactory()).CreateMqttClient()

    let clientOptions = MqttClientOptionsBuilder().WithTcpServer("localhost").Build()

    let subscriptionOptions =
        MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter("iot/sensor/v34/+/status")
            .Build()

    interface IHostedService with
        member this.StartAsync(cancellationToken) =
            task {

                client.add_ApplicationMessageReceivedAsync (fun e ->
                    backgroundTask {
                        e.ApplicationMessage.PayloadSegment.ToArray()
                        |> System.Text.Encoding.UTF8.GetString
                        |> JsonSerializer.Deserialize<Message>
                        |> fun msg ->
                            broadcastHub.Clients.All.SendAsync("Pub", msg)
                            printfn "mqtt to signalr: %A" msg.tick
                    })

                let! _ = (client.ConnectAsync(clientOptions))
                let! _ = client.SubscribeAsync(subscriptionOptions)
                ()
            }


        member this.StopAsync(cancellationToken) =
            task {
                do! client.DisconnectAsync()
                client.Dispose()
            }
