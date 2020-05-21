namespace AuthService
open Saturn
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe

module Router = 

    let apiRouter = router {
        
        getf "/api/%s" (fun name next ctx  ->  
            task {
                return! json (Api.getToken name) next ctx
                //let! token = Api.getToken name
                //return! json token next ctx
            }
        )
    }

