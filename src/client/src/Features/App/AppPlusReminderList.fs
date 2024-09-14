namespace Features.App

open Features
open Models

[<AutoOpen>]
module AppPlusReminderList =
  let private handleReminderListIntent intent state  =
    match intent with
    | ReminderList.Intent.DoNothing -> state
    | ReminderList.Intent.NewReminderAdded (list, reminder) ->
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
    | ReminderList.Intent.ReminderToggled (list, reminder) ->
      let nextLists =
        state.Lists
        |> List.map (fun l ->
          if l.Id = list.Id
          then
            let nextReminders =
              l.Reminders
              |> List.map (fun r ->
                if r.Id = reminder.Id
                then { r with IsCompleted = not r.IsCompleted }
                else r
              )
            { l with Reminders = nextReminders }
          else l
        )
      { state with Lists = nextLists }

  let handleReminderListMsg msg state =
    match state.SelectedList with
      | None -> state
      | Some selectedList ->
        let updatedSelectedList, intent = ReminderListFeature.update msg selectedList
        let updatedState = handleReminderListIntent intent state
        { updatedState with SelectedList = Some updatedSelectedList }
