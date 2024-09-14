module Program
#nowarn "20"

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting

let webApp =
  choose [
    route "/ping" >=> text "pong"
  ]

[<EntryPoint>]
let main args =
  let exitCode = 0

  let builder = WebApplication.CreateBuilder(args)

  builder.Services.AddGiraffe()

  let app = builder.Build()

  app.UseGiraffe webApp

  app.Run()

  exitCode
