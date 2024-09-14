namespace Features

open Features.ReminderList
open Models

[<RequireQualifiedAccess>]
module ReminderListFeature =
  let init (list : ReminderList option) () =
    let placeholderList = 
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
        ]}

    { List = list |> Option.defaultValue placeholderList
      NewReminder = None }

  let update msg state =
    match msg with
    | SetNewReminder reminder ->
      { state with NewReminder = Some reminder }, DoNothing
    | ToggleReminderIsCompleted reminder ->
      let nextReminder =
        { reminder with IsCompleted = not reminder.IsCompleted }
      let nextReminders = 
        state.List.Reminders
        |> List.map (fun r -> if r.Id = nextReminder.Id then nextReminder else r)
      let nextList = { state.List with Reminders = nextReminders }
      { state with List = nextList }, ReminderToggled (nextList, reminder)
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

        let newReminder =
          { Id = nextId
            Task = nr
            IsCompleted = false }

        let nextReminders = List.append state.List.Reminders [newReminder]
        let nextList = { state.List with Reminders = nextReminders }

        { state with 
            List = nextList
            NewReminder = None }, NewReminderAdded (nextList, newReminder)
      )
      |> Option.defaultValue (state, DoNothing)
  