[<AutoOpen>]
module ElmishExtensions

open Elmish

module Cmd =
  let fromAsync (operation : Async<'msg>) : Cmd<'msg> =
    let op () = async {
      return! operation
    } 
    Cmd.batch [
      Cmd.OfAsync.perform op () id
    ]
