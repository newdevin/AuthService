namespace AuthService

open Giraffe
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive
open System

[<RequireQualifiedAccess>]
module Api = 
    let getToken appName appId : HttpHandler = fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! tokenString = Service.getToken appName appId
            match tokenString with
            | None -> return! (RequestErrors.NOT_FOUND "404") next ctx
            | Some v -> let t:Models.Token = {TokenString = v } 
                        return! (Successful.OK t ) next ctx
        }

    let verifyToken appName token : HttpHandler =  fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! verified = Service.verifyToken appName (token)
            match verified with
            | Some t -> let verification:Models.TokenVerification = {Verified = t}
                        return! (Successful.OK verification) next ctx
            | None -> let verification:Models.TokenVerification = {Verified = false}
                      return! (Successful.OK verification) next ctx
        }
