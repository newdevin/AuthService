namespace AuthService

open System

    module Domain = 
        type Application = {Name : string}
        type Secret = {Application : Application ; Token : string ; CretedOn : DateTime ; ExpiryOn : DateTime}

        //let createSecret applicationName =
            