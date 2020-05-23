namespace AuthService
    open System
    

    module TokenService = 
        let createToken key iv appName appId = 
            sprintf "%s;%A" appName appId
            |> Crypto.encrypt key iv

        let verifyToken key iv token applicationName applicationId =
            let token = Crypto.decrypt key iv token
            let appName = token.Split(";") |> Array.head
            let appId = token.Split(";") |> Array.tail |> Array.head |> Guid.Parse
            appName = applicationName && appId = applicationId 
    
    module Service = 
        
        let private createSecret applicationName applicationId token = 
            let createDate = DateTime.Now
            let expiryDate = createDate.AddDays(7.)
            Domain.createSecret applicationName applicationId token createDate expiryDate

        let private createToken applicationName applicationId = 
            async {
                let! key,iv = Database.getSecretKey
                let token = TokenService.createToken key iv applicationName applicationId
                let secret = createSecret applicationName applicationId token
                let! x = Database.deleteToken applicationName
                x |> ignore
                let! r = Database.createToken secret
                return r |> Option.map(fun t -> t.Token)
                }

        let private verifySecretToken (secret:Domain.Secret) applicationName applicationId = 
            async {
                if secret.ExpiryOn <= DateTime.Now then
                    return false
                else
                    let! key,iv = Database.getSecretKey
                    let verified = TokenService.verifyToken key iv secret.Token applicationName applicationId
                    match verified with
                    | true -> return true
                    | false -> return false
            }

        let getToken applicationName applicationId = 
            async {
                let! secret = Database.getSecret applicationName
                match secret with
                | Some s -> let! verified = verifySecretToken s applicationName applicationId
                            match verified with
                            | true -> return Some s.Token
                            | false -> return! createToken applicationName applicationId
                | None -> return! createToken applicationName applicationId
            }

        let verifyToken applicationName token =  
            async {
                let! secret = Database.getSecret applicationName
                match secret with
                | None -> return false
                | Some s -> if s.Token <> token then 
                                return false 
                            else 
                                return! verifySecretToken s applicationName s.Application.Id
            }