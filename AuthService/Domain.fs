namespace AuthService

open System

    [<RequireQualifiedAccess>]
    module Domain = 
        
        type Application = {Name : string ; Id :Guid}
        type Secret = {Application : Application ; CreatedOn : DateTime ; ExpiryOn : DateTime}
        
        let createApplication applicationName applicationId= 
            {Name = applicationName ; Id = applicationId}

        let createSecret applicationName applicationId createdOn expiryOn = 
            { Application = (createApplication applicationName applicationId); CreatedOn = createdOn ; ExpiryOn = expiryOn }
            
            