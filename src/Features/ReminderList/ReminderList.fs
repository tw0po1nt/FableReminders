namespace Features

open Common
open Dependencies
open Dependencies.Api
open Elmish
open Fable.Core
open Features.ReminderList
open Models

[<RequireQualifiedAccess>]
module ReminderListFeature =
  let init (list : ReminderList option) () =
    let placeholderList = 
      { Id = 1
        Name = "F# Series"
        Color = "#89C1FA"
        Reminders = [
          { Id = 1
            Task = "Write a proper API"
            IsCompleted = false }
        ]}

    { List = list |> Option.defaultValue placeholderList
      NewReminder = None }, Cmd.none

  let update msg state =
    match msg with
    | SetNewReminder reminder ->
      { state with NewReminder = Some reminder }, DoNothing, Cmd.none
    | ToggleReminderIsCompleted reminder ->
      let nextReminder =
        { reminder with IsCompleted = not reminder.IsCompleted }
      let nextReminders = 
        state.List.Reminders
        |> List.map (fun r -> if r.Id = nextReminder.Id then nextReminder else r)
      let nextList = { state.List with Reminders = nextReminders }

      let updateReminder = async {
        let! result = 
          ApiDependency.updateReminder state.List.Id reminder.Id nextReminder.IsCompleted
          |> Async.AwaitPromise
        match result with
        | Ok res -> 
          let updated : Reminder = { Id = res.Id; Task = res.Task; IsCompleted = res.IsCompleted }
          let created = Eager.Update.Success { Id = updated.Id; Updated = updated }
          return UpdateReminderResult created 
        | Error err ->
          let previous = reminder
          let updateFailed = Eager.Update.Failure { Id = reminder.Id; Previous = previous; Error = ApiError.msg err  }
          return UpdateReminderResult updateFailed
      } 

      { state with List = nextList },
      ReminderUpdated (nextList, nextReminder), 
      Cmd.fromAsync updateReminder
    | UpdateReminderResult result ->
      match result with
      | Eager.Update.Success success ->
        state, ReminderUpdated (state.List, success.Updated), Cmd.none
      | Eager.Update.Failure failure ->
        // TODO: Show error msg
        let previousReminders =
          state.List.Reminders
          |> List.map (fun r ->
            if r.Id = failure.Id
            then failure.Previous
            else r
          )
        let previousList =
          { state.List with Reminders = previousReminders }
        { state with List = previousList }, 
        ReminderUpdated (previousList, failure.Previous),
        Cmd.none

    | CommitNewReminder ->
      state.NewReminder
      |> Option.map(fun nr ->
        let nextId =
          match state.List.Reminders with
          | [ ] -> 1
          | elems ->
            elems
            |> List.maxBy (fun l -> l.Id)
            |> fun l -> l.Id + 1

        let eagerNewReminder : Reminder =
          { Id = nextId
            Task = nr
            IsCompleted = false }

        let nextReminders = List.append state.List.Reminders [eagerNewReminder]
        let nextList = { state.List with Reminders = nextReminders }

        let addNewReminder = async {
          let! result = 
            ApiDependency.createNewReminder state.List.Id nr
            |> Async.AwaitPromise
          match result with
          | Ok res -> 
            let newReminder : Reminder = { Id = res.Id; Task = res.Task; IsCompleted = res.IsCompleted }
            let created = Eager.Create.Success { Id = eagerNewReminder.Id; Created = newReminder }
            return AddNewReminderResult created 
          | Error err ->
            let failure = Eager.Create.Failure { Id = eagerNewReminder.Id; Error = ApiError.msg err }
            return AddNewReminderResult failure
        }

        { state with List = nextList; NewReminder = None }, 
        ReminderAdded (nextList, eagerNewReminder), 
        Cmd.fromAsync addNewReminder
      )
      |> Option.defaultValue (state, DoNothing, Cmd.none)
    | AddNewReminderResult result ->
      match result with
      | Eager.Create.Success success ->
        state, ReminderUpdated (state.List, success.Created), Cmd.none
      | Eager.Create.Failure failure ->
        let reminderRemovedOrDoNothingIntent = 
          state.List.Reminders
          |> List.tryFind (fun r -> r.Id = failure.Id)
          |> Option.map (fun r -> ReminderRemoved (state.List, r))
          |> Option.defaultValue DoNothing

        let previousReminders =
          state.List.Reminders
          |> List.filter (fun r -> r.Id <> failure.Id)
        let previousList =
          { state.List with Reminders = previousReminders }
        { state with List = previousList },
        reminderRemovedOrDoNothingIntent, 
        Cmd.none
  