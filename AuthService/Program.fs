// Learn more about F# at http://fsharp.org

open System
open Saturn
open Giraffe
open AuthService
let route = router {
    getf "/token/%s" (fun app -> let! token = Service.getToken app)
}

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
