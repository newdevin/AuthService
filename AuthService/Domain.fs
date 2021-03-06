﻿namespace AuthService

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

        let verifyToken secret appName appId appSecret token =
            try
                let token = Crypto.decrypt secret.Key secret.Iv token
                let arr = token.Split(";");
                if arr.Length <> 3 then
                    false
                else
                    let name = arr.[0] 
                    let id = arr.[1] |> Guid.Parse
                    let secret = arr.[2] |> Guid.Parse
                    name = appName && id = appId  && secret = appSecret
            with
            | _ -> false;

        
        
