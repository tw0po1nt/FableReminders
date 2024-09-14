namespace Features

open Features.App
open Models

[<RequireQualifiedAccess>]
module AppFeature =
  let init () : App.State =
    { Lists = [
      { Id = 1
        Name = "Reminders"
        Color = "#F19A38"
        Reminders = [
          { Id = 1
            Task = "Do the dishes"
            IsCompleted = false };
          { Id = 2
            Task = "Hang up the clothes"
            IsCompleted = true }
        ]};
        { Id = 2
          Name = "Final project tasks"
          Color = "#3B82F7"
          Reminders = [
            { Id = 1
              Task = "Write an F# app"
              IsCompleted = false }
          ]
        }]
      NewReminderList = None
      SelectedList = None }

  let update msg state =
    match msg with 
    | SetNewReminderList newList ->
      { state with NewReminderList = Some newList }
    | CommitNewReminderList ->
      state.NewReminderList
      |> Option.map (fun nl ->
        let nextId =
          match state.Lists with
          | [ ] -> 1
          | elems ->
              elems
              |> List.maxBy (fun l -> l.Id)
              |> fun l -> l.Id + 1
        
        let newList =
          { Id = nextId
            Name = nl.Name
            Color = nl.Color
            Reminders = [] }

        { state with 
            Lists = List.append state.Lists [newList]
            NewReminderList = None }
      )
      |> Option.defaultValue state
    | SelectList list ->
      { state with SelectedList = Some (ReminderListFeature.init (Some list) ()) }
    | SelectedList msg -> handleReminderListMsg msg state