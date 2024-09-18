module Program

open Elmish
open Elmish.React
open Features

Program.mkProgram AppFeature.init AppFeature.update AppView.render
|> Program.withReactSynchronous "app"
|> Program.run
