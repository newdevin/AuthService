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
            select (sec.Key , sec.Iv)
            head
        }
        

    let getToken appName appId =
        async {
            let ctx = Authdb.GetDataContext(options);
            let! result = 
                query {
                    for secret in ctx.Dbo.Secret do
                    join application in ctx.Dbo.Application 
                        on (secret.ApplicationId = application.Id)
                    where (application.Name = appName && application.AppId = appId)
                    select (secret, application)
                }
                |> Seq.tryHeadAsync  

            return result 
                    |> Option.map (fun (s,a) -> Domain.createSecret a.Name a.AppId s.Token s.CreatedOn s.ExpiryOn )

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
                   let newSecret = ctx.Dbo.Secret.``Create(ApplicationId, CreatedOn, ExpiryOn, Token)``(a.Id, secret.CreatedOn, secret.ExpiryOn, secret.Token)
                   let a = async {
                            do! ctx.SubmitUpdatesAsync()
                           }
                   Async.RunSynchronously a
                   Domain.createSecret secret.Application.Name secret.Application.Id newSecret.Token newSecret.CreatedOn newSecret.ExpiryOn)
        }


