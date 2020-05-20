namespace AuthService

open System

    [<RequireQualifiedAccess>]
    module Domain = 
        exception UnsuppotedException of string
        type Application = {Name : string}
        type Secret = {Application : Application ; Token : string ; CreatedOn : DateTime ; ExpiryOn : DateTime}
        
        let createApplication applicationName = 
            {Name = applicationName}

        let createSecret applicationName token createdOn expiryOn = 
            { Application = (createApplication applicationName ); Token =  token ; CreatedOn = createdOn ; ExpiryOn = expiryOn }
            
            