namespace AuthService
open Saturn
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe

module Router = 

    let apiRouter = router {
        
        getf "/api/%s" (fun name -> Api.getToken name) 
        
    }

