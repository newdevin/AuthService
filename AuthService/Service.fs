namespace AuthService
    open System
    

    module TokenService = 

        let createToken key iv appName appId appSecret = 
            sprintf "%s;%A;%A" appName appId appSecret
            |> Crypto.encrypt key iv

        let verifyToken key iv token applicationName applicationId applicationSecret=
            let token = Crypto.decrypt key iv token
            let arr = token.Split(";");
            if arr.Length <> 3 then
                false
            else
                let appName = arr|> Array.head
                let appId = arr |> Array.tail |> Array.head |> Guid.Parse
                let appSecret = arr |> Array.tail |>Array.tail |> Array.head |> Guid.Parse
                appName = applicationName && appId = applicationId  && appSecret = applicationSecret
    
    module Service = 
        
        let private createSecret applicationName applicationId applicationSecret token = 
            let createDate = DateTime.Now
            let expiryDate = createDate.AddDays(7.)
            Domain.createSecret applicationName applicationId applicationSecret token createDate expiryDate

        let private createToken applicationName applicationId applicationSecret= 
            async {
                let! key,iv = Database.getSecretKey
                let token = TokenService.createToken key iv applicationName applicationId applicationSecret
                let secret = createSecret applicationName applicationId applicationSecret token
                let! x = Database.deleteToken applicationName
                x |> ignore
                let! r = Database.createToken secret
                return r |> Option.map(fun t -> t.Token)
                }

        let private verifySecretToken (secret:Domain.Secret) applicationName applicationId applicationSecret = 
            async {
                if secret.ExpiryOn <= DateTime.Now then
                    return false
                else
                    let! key,iv = Database.getSecretKey
                    let verified = TokenService.verifyToken key iv secret.Token applicationName applicationId applicationSecret
                    match verified with
                    | true -> return true
                    | false -> return false
            }

        let getToken applicationName applicationId = 
            async {
                let! secret = Database.getSecret applicationName
                match secret with
                | Some s -> let! verified = verifySecretToken s applicationName applicationId s.Application.AppSecret
                            match verified with
                            | true -> return Some s.Token
                            | false -> return! createToken applicationName applicationId s.Application.AppSecret
                | None -> return! createToken applicationName applicationId (Guid.NewGuid())
            }

        let verifyToken applicationName token =  
            async {
                let! secret = Database.getSecret applicationName
                match secret with
                | None -> return false
                | Some s -> if s.Token <> token then 
                                return false 
                            else 
                                return! verifySecretToken s applicationName s.Application.Id s.Application.AppSecret
            }