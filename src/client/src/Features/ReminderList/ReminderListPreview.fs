module ReminderListPreview

open Elmish
open Elmish.React
open Features

let private previewUpdate msg state = ReminderListFeature.update msg state |> fst

Program.mkSimple (ReminderListFeature.init None) previewUpdate ReminderListView.render
|> Program.withReactSynchronous "app"
|> Program.run

