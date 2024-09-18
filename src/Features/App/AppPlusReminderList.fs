namespace Features.App

open Elmish
open Features
open Models

[<AutoOpen>]
module AppPlusReminderList =
  let private handleReminderListIntent intent state  =
    match intent with
    | ReminderList.Intent.DoNothing -> state
    | ReminderList.Intent.ReminderAdded (list, reminder) ->
      let nextLists =
        state.Lists
        |> List.map (fun l ->
          if l.Id = list.Id
          then
            let nextReminders = List.append l.Reminders [reminder]
            { l with Reminders = nextReminders } 
          else l
        )
      { state with Lists = nextLists }
    | ReminderList.Intent.ReminderUpdated (list, reminder) ->
      let nextLists =
        state.Lists
        |> List.map (fun l ->
          if l.Id = list.Id
          then
            let nextReminders =
              l.Reminders
              |> List.map (fun r ->
                if r.Id = reminder.Id
                then reminder
                else r
              )
            { l with Reminders = nextReminders }
          else l
        )
      { state with Lists = nextLists }
    | ReminderList.Intent.ReminderRemoved (list, reminder) ->
      let nextLists =
        state.Lists
        |> List.map (fun l ->
          if l.Id = list.Id
          then
            let nextReminders =
              l.Reminders
              |> List.filter (fun r -> r.Id <> reminder.Id)
            { l with Reminders = nextReminders }
          else l
        )
      { state with Lists = nextLists }

  let handleReminderListMsg msg state =
    match state.SelectedList with
      | None -> state, Cmd.none
      | Some selectedList ->
        let updatedSelectedList, intent, reminderListCmd = ReminderListFeature.update msg selectedList
        let updatedState = handleReminderListIntent intent state
        let cmd = Cmd.map (fun reminderListMsg -> SelectedList reminderListMsg) reminderListCmd
        { updatedState with SelectedList = Some updatedSelectedList }, cmd
