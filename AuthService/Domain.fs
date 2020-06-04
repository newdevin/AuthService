namespace AuthService

open System

    [<RequireQualifiedAccess>]
    module Domain = 
        
        [<CLIMutable>]
        type SecretToken = {Token: string ; CreatedOn:DateTime ; ExpiryOn : DateTime}
        [<CLIMutable>]
        type App = {Name : string ; AppId :Guid ; AppSecret : Guid ; SecretToken : SecretToken option}
        [<CLIMutable>]
        type Secret = {Key : byte[] ; Iv : byte[]}
        
        let createApp name id appSecret secretToken = 
            {Name = name ; AppId = id ; AppSecret = appSecret ; SecretToken = secretToken}

        let createSecretToken token createdOn expiryOn = 
            if String.IsNullOrEmpty(token) then
                None
            else Some {Token = token ; CreatedOn = createdOn ; ExpiryOn = expiryOn}
            
        let createToken appName appId appSecret secret = 
            sprintf "%s;%A;%A" appName appId appSecret
            |> Crypto.encrypt secret.Key secret.Iv

        let verifyToken secret app =
            match app.SecretToken with
            | None -> false
            | Some st ->let token = Crypto.decrypt secret.Key secret.Iv st.Token
                        let arr = token.Split(";");
                        if arr.Length <> 3 then
                            false
                        else
                            let appName = arr.[0] 
                            let appId = arr.[1] |> Guid.Parse
                            let appSecret = arr.[2] |> Guid.Parse
                            appName = app.Name && appId = app.AppId  && appSecret = app.AppSecret
        
        
