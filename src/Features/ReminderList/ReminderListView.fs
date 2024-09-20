namespace Features

open Features.ReminderList
open Lit
open Models

[<RequireQualifiedAccess>]
module ReminderListView =
  let private reminderListItem list reminder onClick =
    let checkboxFill =
      if reminder.IsCompleted
      then html $"""
        <div 
          class="w-3 h-3 rounded-full"
          style={sprintf "border-color: %s; background-color: %s" list.Color list.Color }></div>
      """
      else Lit.nothing

    html $"""
      <div class="flex flex-row items-center p-2">
        <div 
          style={sprintf "border-color: %s;" (if reminder.IsCompleted then list.Color else "rgb(113,113,122)")}
          class="flex flex-row justify-center items-center border w-5 h-5 rounded-full ml-4 mr-2 cursor-pointer"
          @click={Ev(ignore >> onClick)}>
          {checkboxFill}
        </div>
        <p class="text-white grow">{reminder.Task}</p>
      </div>
    """

  let private newReminderListItem (newReminder : string) (onNameChange : string -> unit) onCommit =
    html $"""
      <div class="flex flex-row items-center p-2 cursor-pointer">
        <i class="fa-solid fa-circle-plus text-zinc-400 ml-4 mr-2 w-5 h-5"></i>
        <input 
          class="grow bg-zinc-800 text-white focus:ring-0 focus:ring-offset-0"
          type="text"
          value={newReminder}
          @change={EvVal onNameChange}
          @keyup={Ev (fun e ->
            let keyboardEvent = e :> Browser.Types.KeyboardEvent
            if keyboardEvent.code = "Enter"
            then onCommit()
          )}
          @blur={Ev (ignore >> onCommit)} />
      </div>
    """

  let private addNewTaskItem onClick =
    html $"""
      <div 
        class="flex flex-row items-center p-2 pointer-cursor"
        @click={Ev (ignore >> onClick)}>
        <i class="fa-solid fa-circle-plus text-zinc-400 ml-4 mr-3"></i>
        <p class="text-white">Add new task</p>
      </div>
    """

  let render state dispatch =
    html $"""
      <section class="w-full">
        <div class="flex flex-row items-baseline px-4 pt-4">
          <p 
            class="grow text-2xl text-white font-bold ml-2 mb-2"
            style={ sprintf "color: %s;" state.List.Color }>
            {state.List.Name}
          </p>
          <p 
            class="text-2xl text-white font-bold ml-2 mb-2"
            style={ sprintf "color: %s;" state.List.Color }>
            {sprintf "%d" state.List.Reminders.Length}
          </p>
        </div>
        <div class="w-full">
          {state.List.Reminders 
          |> Lit.mapUnique 
            (fun r -> sprintf "%d" r.Id) 
            (fun reminder -> 
              reminderListItem 
                state.List 
                reminder
                (fun () -> reminder |> (ToggleReminderIsCompleted >> dispatch))
            )
          }
          {state.NewReminder
          |> Option.map (fun nr ->
            newReminderListItem 
              nr 
              (SetNewReminder >> dispatch)
              (fun _ -> dispatch CommitNewReminder)
          )
          |> Option.defaultValue (
            addNewTaskItem 
              (fun () -> "New reminder" |> (SetNewReminder >> dispatch))
          )}
        </div>
      </section>
    """
