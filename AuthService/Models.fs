namespace AuthService

[<RequireQualifiedAccess>]
module Models = 
    [<CLIMutable>]
    type Token = {Token :string}

    [<CLIMutable>]
    type ErrorMessage = {Message : string} 

