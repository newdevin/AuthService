namespace AuthService
open MongoDB.Driver
open MongoDB.Bson
open System

[<RequireQualifiedAccess>]
module Mongo = 

    [<CLIMutable>]
    type MongoSecret = {Id :ObjectId ; Key: string ; Iv : string}
    [<CLIMutable>]
    type MongoApp = {Id : ObjectId ; App: Domain.App}
    
    let getCollection<'T> (constr:string) name = 
        let client = MongoClient(constr);
        let db = client.GetDatabase("Auth");
        db.GetCollection<'T>(name);

    let getSecret constr = 
        async {
            let col = getCollection<MongoSecret> constr "Secret"
            let! stream = col.FindAsync(fun _ -> true) |> Async.AwaitTask
            let! mongoSecret =  stream.FirstAsync() |> Async.AwaitTask
            let sec:Domain.Secret = { Key = Convert.FromBase64String(mongoSecret.Key) ; Iv = Convert.FromBase64String(mongoSecret.Iv)}
            return sec
             
        }

    let getApp constr appName = 
        async{
            let col = getCollection<MongoApp> constr appName
            let! stream = col.FindAsync(fun app -> app.App.Name = appName) |> Async.AwaitTask 
            let! app =  stream.FirstOrDefaultAsync() |> Async.AwaitTask
            match box app with
            | null -> return None
            | _ -> return Some app.App
        }

    let registerApp constr (app:Domain.App) = 
        async {
            let col = getCollection<MongoApp> constr "App"
            let mongoApp = {Id = ObjectId.GenerateNewId() ; App = app}
            do! col.InsertOneAsync(mongoApp) |> Async.AwaitTask
            let! stream = col.FindAsync(fun a -> a.App.Name = app.Name) |> Async.AwaitTask 
            let! foundApp =  stream.FirstAsync() |> Async.AwaitTask
            return foundApp.App
        }


