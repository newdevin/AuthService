namespace AuthService
open FSharp.Data.Sql
open FSharp.Data.Sql.Transactions

[<RequireQualifiedAccess>]
module Database = 

    [<Literal>]
    let conStr = "Server = . ; Database = Auth ; Trusted_Connection = true";
    type Authdb = SqlDataProvider<DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER, ConnectionString = conStr, UseOptionTypes = true >

    let options:TransactionOptions = { IsolationLevel = IsolationLevel.DontCreateTransaction ; Timeout = TimeSpan.FromSeconds(15.0)}
    let ctx = Authdb.GetDataContext(options);

    let applicationNameExists appName =
        query {
            for name in ctx.Dbo.Application do
            where (name = appName)
            head
        }

