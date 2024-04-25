#nowarn "20"

open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Hubs
open Services

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    builder.Services.AddSignalR().AddMessagePackProtocol()
    builder.Services.AddHostedService<MQTTSubscribeService>()

    builder.Services.AddCors(fun options ->
        options.AddDefaultPolicy(fun policyBuilder ->
            policyBuilder
                .SetIsOriginAllowed(fun origin -> Uri(origin).Host = "localhost" || true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()

            ())

        ())

    let app = builder.Build()
    app.UseCors()
    app.MapGet("/", Func<string>(fun () -> "Hello World!")) |> ignore
    app.MapHub<BroadcastHub>("/signalr")

    app.Run()

    0 // Exit code
