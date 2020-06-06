namespace AuthService
open Saturn
open Giraffe
open System


module Router = 

    let apiRouter = router {
        
        get "/api" (text "Hello world")
        getf "/api/register/%s" (fun name -> Api.registerApp name)
        getf "/api/token/%s/%s" (fun nameAndId -> let name, id = nameAndId
                                                  let valid, guid = Guid.TryParse id
                                                  if not valid then
                                                    RequestErrors.badRequest (text "bad request")
                                                  else
                                                    Api.getToken name guid)

        getf "/api/verify/%s/%s" (fun nameAndToken -> let name, token = nameAndToken 
                                                      Api.verifyToken name token)

        getf "/api/refresh/%s/%s/%s" (fun nameIdAndToken -> let name, id, token = nameIdAndToken
                                                            let valid, appId = Guid.TryParse id
                                                            if not valid then
                                                                RequestErrors.badRequest (text "bad request")
                                                            else
                                                                Api.refreshToken name appId token)
    }

