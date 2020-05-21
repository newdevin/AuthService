// Learn more about F# at http://fsharp.org

open System
open Saturn
open Giraffe
open AuthService

let api = pipeline {
    plug acceptJson
    set_header "x-pipeline-type" "Api"
}

let app = application {
   // pipe_through api
    url "https://0.0.0.0:8085/" 
    url "http://0.0.0.0:8086/"
    //force_ssl
    use_router Router.apiRouter
}

[<EntryPoint>]
let main argv =
    run app
    0 // return an integer exit code
