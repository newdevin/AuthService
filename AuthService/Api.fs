namespace AuthService

open Giraffe
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive


[<RequireQualifiedAccess>]
module Api = 
    let getToken appName appId : HttpHandler = fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! tokenString = Service.getToken appName appId
            match tokenString with
            | None -> return! (RequestErrors.NOT_FOUND "404") next ctx
            | Some v -> let t:Models.Token = {Token = v } 
                        return! (Successful.OK t ) next ctx
        }
