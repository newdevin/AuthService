// Learn more about F# at http://fsharp.org

open System
open Saturn
open Giraffe
open AuthService


let app = application {
   
    url "https://0.0.0.0:8085/" 
    force_ssl
    use_router Router.apiRouter
    
}

[<EntryPoint>]
let main argv =
    run app
    0 // return an integer exit code
