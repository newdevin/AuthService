namespace AuthService
    open System

    module Service = 
        
        let private generateToken applicationName (applicationId:Guid) (createDate:DateTime) (expiryDate:DateTime) key iv = 
            sprintf "appName:%s;appId:%A;createDate:%s;expiryDate:%s" applicationName applicationId (createDate.ToString "yyyy-MM-dd hh:mm") (expiryDate.ToString "yyyy-MM-dd hh:mm")
            |> Crypto.encrypt key iv 

        let private createSecret applicationName applicationId key iv = 
            let createDate = DateTime.Now
            let expiryDate = createDate.AddDays(7.)
            let token = generateToken applicationName applicationId createDate expiryDate key iv
            Domain.createSecret applicationName applicationId token createDate expiryDate

        let getToken applicationName applicationId = 
            async {
                
                let! secret = Database.getToken applicationName applicationId

                match secret with
                | Some s -> return Some s.Token
                | None -> return! 
                            async{
                            let key,iv = Database.getSecretKey
                            let! p = createSecret applicationName applicationId (Convert.FromBase64String key) ( Convert.FromBase64String iv)
                                     |> Database.createToken
                            return p |> Option.map (fun s -> s.Token)
                          }             
            }

