namespace AuthService
open Saturn
open Giraffe
open System

module Router = 

    let apiRouter = router {
        
        get "/api" (text "Hello world")
        getf "/api/%s/%s" (fun nameAndId -> let name, id = nameAndId 
                                            Api.getToken name (Guid.Parse id))
    }

