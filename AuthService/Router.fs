namespace AuthService
open Saturn
open FSharp.Control.Tasks.V2
open Giraffe

module Router = 

    let apiRouter = router {
        
        getf "/api/%s" (fun name next ctx  ->  
            task {
                let! token = Service.getToken name
                return! json token next ctx
            }
        )
    }

