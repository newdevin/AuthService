namespace AuthService
open Saturn
open Giraffe
open System

module Router = 

    let apiRouter = router {
        
        get "/api" (text "Hello world")
        getf "/api/token/%s/%s" (fun nameAndId -> let name, id = nameAndId 
                                                  Api.getToken name (Guid.Parse id))
        getf "/api/verify/%s/%s" (fun nameAndToken -> let name, token = nameAndToken 
                                                      Api.verifyToken name token)
    }

