// Learn more about F# at http://fsharp.org

open System
open Saturn
open Giraffe
open Giraffe.Serialization
open AuthService
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder


let hostconfig (builder:IWebHostBuilder) =
    builder.ConfigureAppConfiguration(fun ctx config ->
            config.AddJsonFile("appSettings.json",false,true) |> ignore)

let port = Environment.GetEnvironmentVariable("port")
let apiUrl =  sprintf "https://0.0.0.0:%s" port

let app = application {

    url apiUrl
    force_ssl
    use_router Router.apiRouter
    service_config (fun services -> services.AddSingleton<Giraffe.Serialization.Json.IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer()))
    host_config  hostconfig
    
    
}

[<EntryPoint>]
let main argv =
    run app
    0 // return an integer exit code

