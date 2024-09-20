module ReminderListPreview

open Elmish
open Lit.Elmish
open Features

let private previewUpdate msg state = 
  let (st, _, cmd) = ReminderListFeature.update msg state
  st, cmd

Program.mkProgram (ReminderListFeature.init None) previewUpdate ReminderListView.render
|> Program.withLit "app"
|> Program.run

