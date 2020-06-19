module DomainTest

open System
open Xunit
open FsUnit
open AuthService

[<Fact>]
let ``createSecretToken should return none `` () =
    let result = Domain.createSecretToken "" (DateTime.Now.Date) (DateTime.Now.Date.AddDays(2.))
    result |> should equal None

let ``createSecretToken should return secret `` () =
    let token = "token"
    let createdOn = DateTime.Now.Date
    let expiryOn = DateTime.Now.Date.AddDays(2.)
    let result = Domain.createSecretToken token createdOn expiryOn
    result.IsSome |> should equal true

