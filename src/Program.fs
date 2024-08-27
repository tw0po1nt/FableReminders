module Program

open Elmish
open Elmish.React

Program.mkSimple App.init App.update App.render
|> Program.withReactSynchronous "app"
|> Program.run
