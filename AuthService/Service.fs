namespace AuthService
open System
    

module Service = 

    exception AppAlreadyRegistered
    exception VerificationException
            
    let registerApp constr appName = 
        async{
            let! mayBeApp = Mongo.getApp constr appName
            match mayBeApp with
            | None -> let! secret = Mongo.getSecret constr
                      let appId, appSecret = (Guid.NewGuid(), Guid.NewGuid())
                      let token = Domain.createToken appName appId appSecret secret
                      let secretToken = Domain.createSecretToken token DateTime.UtcNow.Date (DateTime.UtcNow.AddDays(2.).Date)
                      let app = Domain.createApp appName appId appSecret secretToken
                      return! Mongo.createApp constr app
            | Some a -> return raise AppAlreadyRegistered
        }

    let private generateToken constr appName appId = 
        async{
            let! secret = Mongo.getSecret constr
            let appSecret = Guid.NewGuid()
            let token = Domain.createToken appName appId appSecret secret
            let secretToken = Domain.createSecretToken token DateTime.UtcNow.Date (DateTime.UtcNow.AddDays(2.).Date)
            let app = Domain.createApp appName appId appSecret secretToken
            return! Mongo.createApp constr app
        }

    let private generateAndReturnTokenWithExpiry constr (app:Domain.App) =
        async {
            match app.SecretToken with
            | None -> let! app = generateToken constr app.Name app.AppId
                      return app.SecretToken |> Option.map(fun st -> (st.Token, st.ExpiryOn))
            | Some t -> if t.ExpiryOn.Date >= DateTime.Now.Date then
                            return Some (t.Token , t.ExpiryOn)
                        else
                            let! app = generateToken constr app.Name app.AppId
                            return app.SecretToken |> Option.map(fun st -> (st.Token , st.ExpiryOn))
        }

    let getTokenWithExpiry constr appName appId = 
        async {
            let! mayBeApp = Mongo.getApp constr appName
            match mayBeApp with
            | None -> return raise VerificationException
            | Some app -> if app.AppId <> appId then
                            return raise VerificationException 
                          else 
                            return! generateAndReturnTokenWithExpiry constr app
        }

    let verifyToken constr appName token = 
        async{
            let! mayBeApp = Mongo.getApp constr appName
            match mayBeApp with
            | None -> return raise VerificationException
            | Some app -> 
                match app.SecretToken with
                | None -> return raise VerificationException
                | Some s -> 
                    let! secret = Mongo.getSecret constr
                    return Domain.verifyToken secret  app.Name app.AppId app.AppSecret token
                            
        }
           
    let refreshToken constr appName appId token = 
        async{
            let! verified = verifyToken constr appName token
            match verified with
            | false -> return raise VerificationException
            | _ -> 
                do! Mongo.expireToken constr appName |> Async.Ignore
                return! getTokenWithExpiry constr appName appId
        }
