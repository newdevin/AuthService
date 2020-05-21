namespace AuthService

open Giraffe

[<RequireQualifiedAccess>]
module Api = 
    let getToken appName = 
        async {
            try
                let! token = Service.getToken appName
                return Successful.OK token
            with 
                | Domain.UnsuppotedException s -> return setStatusCode 400 
        }
