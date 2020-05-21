﻿namespace AuthService
open FSharp.Data.Sql
open FSharp.Data.Sql.Transactions
open System

[<RequireQualifiedAccess>]
module Database = 

    [<Literal>]
    let conStr = "Server = . ; Database = Auth ; Trusted_Connection = true";
    type Authdb = SqlDataProvider<DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER, ConnectionString = conStr, UseOptionTypes = true >

    let options:TransactionOptions = { IsolationLevel = IsolationLevel.DontCreateTransaction ; Timeout = TimeSpan.FromSeconds(15.0)}

    let isValidApplication appName = 
        async {
            let ctx = Authdb.GetDataContext(options);
            let! qry = 
                query {
                    for app in ctx.Dbo.Application do
                    where (app.Name = appName)
                }|> Seq.tryHeadAsync
            
            return qry |> Option.map (fun a -> true)
        }

    let getToken appName =
        async {
            let ctx = Authdb.GetDataContext(options);
            let! result = 
                query {
                    for secret in ctx.Dbo.Secret do
                    join application in ctx.Dbo.Application 
                        on (secret.ApplicationId = application.Id)
                    where (application.Name = appName )
                    select (secret, application)
                }
                |> Seq.tryHeadAsync  

            return result 
                    |> Option.map (fun (s,a) -> Domain.createSecret a.Name s.Token s.CreatedOn s.ExpiryOn )

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

            if Option.isNone(app) then
                let msg = sprintf "'%s' is not supported" secret.Application.Name
                raise ( Domain.UnsuppotedException msg)

            return app
                |> Option.map (fun a -> 
                   let newSecret = ctx.Dbo.Secret.``Create(ApplicationId, CreatedOn, ExpiryOn, Token)``(a.Id, secret.CreatedOn, secret.ExpiryOn, secret.Token)
                   let a = async {
                            do! ctx.SubmitUpdatesAsync()
                           }
                   Async.RunSynchronously a
                   Domain.createSecret secret.Application.Name newSecret.Token newSecret.CreatedOn newSecret.ExpiryOn)
        }


