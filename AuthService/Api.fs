namespace AuthService

open Giraffe
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive
open System
open Microsoft.Extensions.Configuration
open Service

[<RequireQualifiedAccess>]
module Api = 

    let private getConnectionString (ctx: HttpContext) = 
        let config = ctx.GetService<IConfiguration>()
        config.GetConnectionString("mongodb")
    
    let registerApp appName : HttpHandler = fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            try
                let constr = getConnectionString ctx
                let! app = Service.registerApp constr appName
                match app.SecretToken with
                | None -> return failwith "something went wrong"
                | Some st -> let appreg: Models.AppRegistration = {AppId = app.AppId ; Token = st.Token}
                             return! Successful.OK appreg next ctx
            with
            | :? AppAlreadyRegistered -> return! RequestErrors.BAD_REQUEST "400" next ctx
        }
        
    //let getToken appName appId : HttpHandler = fun (next : HttpFunc) (ctx : HttpContext) ->
    //    task {
    //        let! tokenString = Service.getToken appName appId
    //        match tokenString with
    //        | None -> return! (RequestErrors.NOT_FOUND "404") next ctx
    //        | Some v -> let t:Models.Token = {TokenString = v } 
    //                    return! (json t ) next ctx
    //    }

    //let verifyToken appName token : HttpHandler =  fun (next : HttpFunc) (ctx : HttpContext) ->
    //    task {
    //        let! verified = Service.verifyToken appName (token)
    //        match verified with
    //        | true -> let verification:Models.TokenVerification = {Verified = true}
    //                  return! (Successful.OK verification) next ctx
    //        | false -> let verification:Models.TokenVerification = {Verified = false}
    //                   return! (Successful.OK verification) next ctx
    //    }

    //let refreshToken appName appId token : HttpHandler = fun (next : HttpFunc) (ctx : HttpContext) ->
    //    task {
    //        let! token = Service.refreshToken appName appId token
    //        match token with
    //        | None -> return! (RequestErrors.NOT_FOUND "404") next ctx
    //        | Some v -> let t:Models.Token = {TokenString = v } 
    //                    return! (Successful.OK t ) next ctx

    //    }
