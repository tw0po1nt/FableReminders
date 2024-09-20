module Program

open Elmish
open Lit.Elmish
open Features

Program.mkProgram AppFeature.init AppFeature.update AppView.render
|> Program.withLit "app"
|> Program.run
