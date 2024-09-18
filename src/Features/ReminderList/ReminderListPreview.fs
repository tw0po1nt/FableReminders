module ReminderListPreview

open Elmish
open Elmish.React
open Features

let private previewUpdate msg state = 
  let (st, _, cmd) = ReminderListFeature.update msg state
  st, cmd

Program.mkProgram (ReminderListFeature.init None) previewUpdate ReminderListView.render
|> Program.withReactSynchronous "app"
|> Program.run

