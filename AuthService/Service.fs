namespace AuthService
    open System
    

    module TokenService = 
        let createToken key iv (secret:Domain.Secret) = 
            sprintf "%s;%A" secret.Application.Name secret.Application.Id
            |> Crypto.encrypt key iv

        let verifyToken key iv token applicationName applicationId =
            let token = Crypto.decrypt key iv token
            let appName = token.Split(";") |> Array.head
            let appId = token.Split(";") |> Array.tail |> Array.head |> Guid.Parse
            appName = applicationName && appId = applicationId 
    
    module Service = 
        
        let private createSecret applicationName applicationId = 
            let createDate = DateTime.Now
            let expiryDate = createDate.AddDays(7.)
            Domain.createSecret applicationName applicationId createDate expiryDate

        let getToken applicationName applicationId = 
            async {
                let! token = Database.getToken applicationName 
                match token with
                | Some s -> let! key,iv = Database.getSecretKey
                            let verified = TokenService.verifyToken key iv s applicationName applicationId
                            match verified with
                            | true -> return Some s
                            | false -> return None
                | None -> return! 
                            async{
                                let! key,iv = Database.getSecretKey
                                let secret = createSecret applicationName applicationId 
                                let token = TokenService.createToken key iv secret
                                let! r = Database.createToken secret token
                                return r
                                   |> Option.map (fun t -> t)
                          }             
            }

        let verifyToken applicationName token =  // verify application name and not expired
            async {
                let! savedToken = Database.getToken applicationName
                return savedToken |> Option.map( fun t -> t = token)
            }

    




