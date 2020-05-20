namespace AuthService
    open System

    module Service = 
        
        let private generateToken applicationName (createDate:DateTime) (expiryDate:DateTime) = 
            sprintf "app:%s;createDate:%s;expiryDate:%s" applicationName (createDate.ToString "yyyy-MM-dd hh:mm") (expiryDate.ToString "yyyy-MM-dd hh:mm")
            |> System.Text.Encoding.ASCII.GetBytes
            |> System.Convert.ToBase64String


        let getToken applicationName = 
            async {
                let! secret = Database.getToken applicationName

                let token = 
                    match secret with
                    | Some s -> s.Token
                    | None -> let createDate = DateTime.Now
                              let expiryDate = createDate.AddDays(7.)
                              let token = generateToken applicationName createDate expiryDate
                              let secret = Domain.createSecret applicationName token createDate expiryDate
                              async {
                                return!  Database.createToken secret
                                }|> ignore
                              token
                return token
            }

