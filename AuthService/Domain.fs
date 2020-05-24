namespace AuthService

open System

    [<RequireQualifiedAccess>]
    module Domain = 
        
        type Application = {Name : string ; Id :Guid ; AppSecret : Guid}
        type Secret = {Application : Application ; Token : string ; CreatedOn : DateTime ; ExpiryOn : DateTime}
        
        let createApplication applicationName applicationId applicationSecret = 
            {Name = applicationName ; Id = applicationId ; AppSecret = applicationSecret}

        let createSecret applicationName applicationId applicationSecret token createdOn expiryOn = 
            { Application = (createApplication applicationName applicationId applicationSecret); Token = token ; CreatedOn = createdOn ; ExpiryOn = expiryOn }
            
            