namespace AuthService

open Saturn
open Giraffe
open FSharp.Control.Tasks.V2

[<RequireQualifiedAccess>]
module Api = 
    let getToken appName = 
        task {
            try
                let! token = Service.getToken appName
                return Successful.OK token
            with 
                | Domain.UnsuppotedException s -> return setStatusCode 400 
        }
