namespace Features

open Common
open Dependencies
open Dependencies.Api
open Elmish
open Fable.Core
open Features.App
open Models

[<RequireQualifiedAccess>]
module AppFeature =
  let init () : App.State * Cmd<App.Msg> =
    { Lists = []
      NewReminderList = None
      SelectedList = None }, Cmd.ofMsg OnAppear

  let update msg state =
    match msg with 
    | OnAppear ->
      let getAllLists = async {
        let! result = 
          ApiDependency.getAllReminderLists()
          |> Async.AwaitPromise
        match result with
        | Ok res -> 
          return res
            |> List.map (fun l ->
              { Id = l.Id
                Name = l.Name
                Color = l.Color
                Reminders =
                  l.Reminders
                  |> List.map (fun r ->
                    { Id = r.Id
                      Task = r.Task
                      IsCompleted = r.IsCompleted }
                  )
              } : ReminderList
            )
            |> SetLists
        | Error _ ->
          return SetLists []
      } 

      state, Cmd.fromAsync getAllLists
    | SetLists lists ->
      { state with Lists = lists }, Cmd.none
    | SetNewReminderList newList ->
      { state with NewReminderList = Some newList }, Cmd.none
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
        
        let eagerNewList : ReminderList =
          { Id = nextId
            Name = nl.Name
            Color = nl.Color
            Reminders = [] }

        let createNewReminderList = async {
          let! result = 
            ApiDependency.createNewReminderList nl
            |> Async.AwaitPromise
          match result with
          | Ok res ->
            let newList : ReminderList = 
              { Id = res.Id
                Name = res.Name
                Color = res.Color 
                Reminders = 
                  res.Reminders
                  |> List.map (fun r ->
                    { Id = r.Id
                      Task = r.Task
                      IsCompleted = r.IsCompleted }
                  ) 
              }
            let created = Eager.Create.Success { Id = eagerNewList.Id; Created = newList }
            return CreateNewReminderListResult created
          | Error err ->
            let failure = Eager.Create.Failure { Id = eagerNewList.Id; Error = ApiError.msg err }
            return CreateNewReminderListResult failure
        }

        { state with 
            Lists = List.append state.Lists [eagerNewList]
            NewReminderList = None }, Cmd.fromAsync createNewReminderList
      )
      |> Option.defaultValue (state, Cmd.none)
    | CreateNewReminderListResult result ->
      match result with
      | Eager.Create.Success success ->
        printfn "Success: %A" success
        { state with
            Lists =
              state.Lists
              |> List.map (fun l ->
                if l.Id = success.Id
                then success.Created 
                else l
              )
        }, Cmd.none
      | Eager.Create.Failure failure ->
        // TODO: Show error msg
        printfn "Falure: %A" failure
        let previousLists =
          state.Lists
          |> List.filter (fun r -> r.Id <> failure.Id)
        { state with Lists = previousLists }, 
        Cmd.none
    | SelectList list ->
      let initReminderList, initReminderListCmd = ReminderListFeature.init (Some list) ()
      let cmd = initReminderListCmd |> Cmd.map (SelectedList)
      { state with SelectedList = Some initReminderList }, cmd
    | SelectedList msg -> handleReminderListMsg msg state