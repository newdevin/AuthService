namespace AuthService

open System

    [<RequireQualifiedAccess>]
    module Domain = 
        
        type Application = {Name : string ; Id :Guid}
        type Secret = {Application : Application ; Token : string ; CreatedOn : DateTime ; ExpiryOn : DateTime}
        
        let createApplication applicationName applicationId= 
            {Name = applicationName ; Id = applicationId}

        let createSecret applicationName applicationId token createdOn expiryOn = 
            { Application = (createApplication applicationName applicationId); Token =  token ; CreatedOn = createdOn ; ExpiryOn = expiryOn }
            
            