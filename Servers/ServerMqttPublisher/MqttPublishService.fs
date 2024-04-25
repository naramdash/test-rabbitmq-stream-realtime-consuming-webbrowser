module Services

#nowarn "20"

open MQTTnet
open MQTTnet.Client
open Microsoft.Extensions.Hosting
open Samples

let vhlCount = 250
let interval = 100

type MQTTPublishService() =
    let client = (new MqttFactory()).CreateMqttClient()

    let clientOptions = MqttClientOptionsBuilder().WithTcpServer("localhost").Build()

    let timers =
        [ for i in 1..vhlCount -> new System.Timers.Timer(interval, AutoReset = true, Enabled = true) ]

    let mutable tick: uint64 = 0UL
    let tickTimer = new System.Timers.Timer(200, AutoReset = true, Enabled = true)


    do

        timers
        |> List.iteri (fun i timer ->
            timer.Elapsed.Add(fun _ ->
                MqttApplicationMessageBuilder()
                    .WithTopic($"iot.sensor.v34.id-{i}.status")
                    .WithPayload(getJsonSample (i.ToString()) (tick.ToString()))
                    .Build()
                |> client.PublishAsync
                |> ignore))

        tickTimer.Elapsed.Add(fun _ ->
            tick <- tick + 1UL
            printfn "tick %d" tick)


    interface IHostedService with
        member this.StartAsync(cancellationToken) =
            task {
                let! _ = (client.ConnectAsync(clientOptions))
                timers |> List.iter _.Start()
                tickTimer.Start()
            }


        member this.StopAsync(cancellationToken) =
            task {
                do! client.DisconnectAsync()
                client.Dispose()


                timers
                |> List.iter (fun timer ->
                    timer.Stop()
                    timer.Dispose())

                tickTimer.Stop()
                tickTimer.Dispose()
            }
