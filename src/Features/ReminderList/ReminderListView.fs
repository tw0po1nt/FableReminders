namespace Features

open Features.ReminderList
open Feliz
open Models

[<RequireQualifiedAccess>]
module ReminderListView =
  let private reminderListItem list reminder onClick =
    Html.div [
      prop.className [ "flex"; "flex-row"; "items-center"; "p-2" ]
      prop.children [
        Html.div [
          prop.className [ "flex"; "flex-row"; "justify-center"; "items-center"; "border"; "w-5"; "h-5"; "rounded-full"; "ml-4"; "mr-2"; "cursor-pointer" ]
          prop.style [ style.borderColor (if reminder.IsCompleted then list.Color else "rgb(113,113,122)" ) ]
          prop.onClick (ignore >> onClick)
          prop.children [
            if reminder.IsCompleted
            then
              Html.div [
                prop.className [ "w-3"; "h-3"; "rounded-full" ]
                prop.style [ 
                  style.borderColor list.Color 
                  style.backgroundColor list.Color
                ]
              ]
          ]
        ]

        Html.p [
          prop.className [ "text-white"; "grow" ]
          prop.text reminder.Task
        ]
        
        // TODO: Show delete button on hover
      ]
    ]

  let private newReminderListItem (newReminder : string) (onNameChange : string -> unit) onCommit =
    Html.div [
      prop.className [ "flex"; "flex-row"; "items-center"; "p-2"; "cursor-pointer" ]
      prop.children [
        Html.i [
          prop.className [ "fa-solid"; "fa-circle-plus"; "text-zinc-400"; "ml-4"; "mr-2"; "w-5"; "h-5" ]
        ]

        Html.input [
          prop.className [ "grow"; "bg-zinc-800"; "text-white"; "focus:ring-0"; "focus:ring-offset-0" ]
          prop.type' "text"
          prop.valueOrDefault newReminder
          prop.onChange onNameChange
          prop.onKeyUp (fun e ->
            if e.key = "Enter"
            then onCommit()
          )
          prop.onBlur (ignore >> onCommit)
        ]
      ]
    ]

  let render state dispatch =
    Html.section [
      prop.className [ "w-full" ]
      prop.children [
        Html.div [
          prop.className [ "flex"; "flex-row"; "items-baseline"; "px-4"; "pt-4" ]
          prop.children [
            Html.p [
              prop.className [ "grow"; "text-2xl"; "text-white"; "font-bold"; "ml-2"; "mb-2" ]
              prop.style [ style.color state.List.Color ]
              prop.text state.List.Name
            ]

            Html.p [
              prop.className [ "text-2xl"; "text-white"; "font-bold"; "ml-2"; "mb-2" ]
              prop.style [ style.color state.List.Color ]
              prop.text (sprintf "%d" state.List.Reminders.Length)
            ]
          ]
        ]

        Html.div [
          prop.className "w-full"
          prop.children [
            for reminder in state.List.Reminders ->
              reminderListItem 
                state.List 
                reminder
                (fun () -> reminder |> (ToggleReminderIsCompleted >> dispatch))
              
            yield 
              state.NewReminder
              |> Option.map (fun nr -> 
                  newReminderListItem 
                    nr 
                    (SetNewReminder >> dispatch)
                    (fun _ -> dispatch CommitNewReminder)
              )
              |> Option.defaultValue (
                Html.div [
                  prop.className [ "flex"; "flex-row"; "items-center"; "p-2"; "cursor-pointer" ]
                  prop.onClick (fun _ -> "New reminder" |> (SetNewReminder >> dispatch))
                  prop.children [
                    Html.i [
                      prop.className [ "fa-solid"; "fa-circle-plus"; "text-zinc-400"; "ml-4"; "mr-3" ]
                    ]

                    Html.p [
                      prop.className [ "text-white" ]
                      prop.text "Add new task"
                    ]
                  ]
                ]
              )
          ]
        ]
      ]
    ]