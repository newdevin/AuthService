namespace AuthService

open System

[<RequireQualifiedAccess>]
module Models = 
    [<CLIMutable>]
    type Token = {TokenString :string}

    [<CLIMutable>]
    type TokenVerification = {Verified: bool} 

    [<CLIMutable>]
    type AppRegistration = {AppId : Guid ; Token : string}

