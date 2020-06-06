namespace AuthService

open System

[<RequireQualifiedAccess>]
module Models = 
    [<CLIMutable>]
    type Token = {TokenString :string ; ExpiryOn : DateTime}

    [<CLIMutable>]
    type TokenVerification = {Verified: bool} 

    [<CLIMutable>]
    type AppRegistration = {AppId : Guid ; Token : Token}

