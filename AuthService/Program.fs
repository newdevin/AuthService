// Learn more about F# at http://fsharp.org

open System
open Saturn
open Giraffe
open Giraffe.Serialization
open AuthService
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection




let app = application {
   
    url "https://0.0.0.0:8085/" 
    force_ssl
    use_router Router.apiRouter
    service_config (fun services -> services.AddSingleton<Giraffe.Serialization.Json.IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer()))
    host_config (fun builder ->
        builder.ConfigureAppConfiguration(fun ctx config ->
            config.AddJsonFile("appSettings.json",false,true) |> ignore
        ))
    
}

[<EntryPoint>]
let main argv =
    run app
    0 // return an integer exit code

