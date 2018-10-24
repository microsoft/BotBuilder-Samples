// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace Microsoft.BotBuilderSamples
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging

module Program =
    let exitCode = 0
    let BuildWebHost args =
        WebHost
            .CreateDefaultBuilder(args)
            .ConfigureLogging(fun hostingContext logging ->
                // Add Azure Logging
                logging.AddAzureWebAppDiagnostics() |> ignore
                // Logging Options.
                // There are other logging options available:
                // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1
                // logging.AddDebug();
                // logging.AddConsole();
            )
            .UseStartup<Startup>()
            .Build()

    [<EntryPoint>]
    let main args =
        BuildWebHost(args).Run()
        exitCode