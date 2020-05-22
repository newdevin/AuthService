namespace AuthService

[<RequireQualifiedAccess>]
module Models = 
    [<CLIMutable>]
    type Token = {TokenString :string}

    [<CLIMutable>]
    type TokenVerification = {Verified: bool} 

