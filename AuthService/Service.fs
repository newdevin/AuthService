namespace AuthService
open System
    

module Service = 

    exception AppAlreadyRegistered
    
    //let private createSecret applicationName applicationId applicationSecret token = 
    //    let createDate = DateTime.Now
    //    let expiryDate = createDate.AddDays(7.)
    //    Domain.createSecret applicationName applicationId applicationSecret token createDate expiryDate

    //let private createToken applicationName applicationId applicationSecret= 
    //    async {
    //        let! key,iv = Database.getSecretKey
    //        let token = TokenService.createToken key iv applicationName applicationId applicationSecret
    //        let secret = createSecret applicationName applicationId applicationSecret token
    //        let! x = Database.deleteToken applicationName
    //        x |> ignore
    //        let! r = Database.createToken secret
    //        return r |> Option.map(fun t -> t.Token)
    //        }

    //let private verifySecretToken (secret:Domain.Secret) applicationName applicationId applicationSecret = 
    //    async {
    //        if secret.ExpiryOn <= DateTime.Now then
    //            return false
    //        else
    //            let! key,iv = Database.getSecretKey
    //            let verified = TokenService.verifyToken key iv secret.Token applicationName applicationId applicationSecret
    //            match verified with
    //            | true -> return true
    //            | false -> return false
    //    }

    //let getToken appName appId = 
    //    async {
    //        let! secret = Database.getSecret appName
    //        match secret with
    //        | Some s -> let! verified = verifySecretToken s appName appId s.Application.AppSecret
    //                    match verified with
    //                    | true -> return Some s.Token
    //                    | false -> return! createToken appName appId s.Application.AppSecret
    //        | None -> return! createToken appName appId (Guid.NewGuid())
    //    }

    //let verifyToken applicationName token =  
    //    async {
    //        let! secret = Database.getSecret applicationName
    //        match secret with
    //        | None -> return false
    //        | Some s -> if s.Token <> token then 
    //                        return false 
    //                    else 
    //                        return! verifySecretToken s applicationName s.Application.Id s.Application.AppSecret
    //    }

    //let refreshToken applicationName applicationId token = 
    //    async {
    //        let! verified = verifyToken applicationName token
    //        if verified then
    //            let newSecret = Guid.NewGuid()
    //            let! u = Database.updateApplicationSecret applicationName newSecret
    //            let! token =  createToken applicationName applicationId newSecret
    //            return token
    //        else
    //            return None
    //    }
    
    let registerApp constr appName = 
        async{
            let! mayBeApp = Mongo.getApp constr appName
            match mayBeApp with
            | None -> let! secret = Mongo.getSecret constr
                      let appId = Guid.NewGuid()
                      let appSecret = Guid.NewGuid()
                      let token = Domain.createToken appName appId appSecret secret
                      let secretToken = Domain.createSecretToken token DateTime.Now.Date (DateTime.Now.AddDays(2.).Date)
                      let app = Domain.createApp appName appId appSecret secretToken
                      return! Mongo.registerApp constr app
            | Some a -> return raise AppAlreadyRegistered
        }
