namespace AuthService
open FSharp.Data.Sql
open FSharp.Data.Sql.Transactions
open System

[<RequireQualifiedAccess>]
module Database = 

    [<Literal>]
    let conStr = "Server = . ; Database = Auth ; Trusted_Connection = true";
    type Authdb = SqlDataProvider<DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER, ConnectionString = conStr, UseOptionTypes = true >

    let options:TransactionOptions = { IsolationLevel = IsolationLevel.DontCreateTransaction ; Timeout = TimeSpan.FromSeconds(15.0)}

    let getSecretKey = 
        let ctx = Authdb.GetDataContext()
        query {
            for sec in ctx.Dbo.SecretKey do
            select (Convert.FromBase64String sec.Key , Convert.FromBase64String sec.Iv)
        }
        |> Seq.headAsync
        
    let deleteToken appName = 
        async{
            let ctx = Authdb.GetDataContext(options)
            let! appId = 
                query {
                    for app in ctx.Dbo.Application do
                    where (app.Name = appName)
                    select app.Id
                }|> Seq.tryHeadAsync
        
            appId 
            |> Option.map( fun id -> 
                                    query {
                                        for secret in ctx.Dbo.Secret do
                                        where (secret.ApplicationId = id)
                                        select secret
                                    }|> Seq.``delete all items from single table``
                         )
            |> ignore
            return appId
        }

         
         


    let getSecret appName = 
        async {
            let ctx = Authdb.GetDataContext(options);
            return! query {
                for secret in ctx.Dbo.Secret do
                    join application in ctx.Dbo.Application 
                        on (secret.ApplicationId = application.Id)
                    where (application.Name = appName )
                    select (Domain.createSecret application.Name application.AppId secret.Token secret.CreatedOn secret.ExpiryOn)
            }|> Seq.tryHeadAsync
        }

    let createToken (secret : Domain.Secret) = 
        async {
            let ctx = Authdb.GetDataContext (options)
            let! app = 
                query {
                    for application in ctx.Dbo.Application do
                    where (application.Name = secret.Application.Name)
                    select application
                }
                |>Seq.tryHeadAsync

            return app
                |> Option.map (fun a -> 
                   ctx.Dbo.Secret.``Create(ApplicationId, CreatedOn, ExpiryOn, Token)``(a.Id, secret.CreatedOn, secret.ExpiryOn, secret.Token)
                   |> ignore
                   let a = async {
                            do! ctx.SubmitUpdatesAsync()
                           }
                   Async.RunSynchronously a
                   secret
                   )
        }

    

