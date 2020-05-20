namespace AuthService

open Saturn

[<RequireQualifiedAccess>]
module Api = 
    let getToken appName = 
        async{
            try
                let! token = Service.getToken appName
                return Ok (token)
            with 
                | Domain.UnsuppotedException s -> return BadRequest
        }
