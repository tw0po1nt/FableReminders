namespace Features

open Colors
open Features.App
open Feliz
open Models

[<RequireQualifiedAccess>]
module AppView =
  // Sidebar
  let private sidebarListItem (list : ReminderList) isSelected onClick =
    Html.div [
      prop.className [
        "flex"; 
        "flex-row"; 
        "items-center"; 
        "rounded-md"; 
        "p-2"; 
        "cursor-pointer";
        if isSelected then "bg-[#3164B6]"
      ]
      prop.onClick (ignore >> onClick)
      prop.children [
        Html.div [
          prop.className [ "flex"; "flex-row"; "justify-center"; "items-center"; "w-8"; "h-8"; "rounded-full"; "mr-2" ]
          prop.style [ style.backgroundColor list.Color ]
          prop.children [
            Html.i [
              prop.className [ "fa-solid"; "fa-list-ul"; "text-white" ]
            ]
          ]
        ]

        Html.p [
          prop.className [ "text-white"; "text-base"; "grow" ]
          prop.text list.Name
        ]

        Html.p [
          prop.className [ "text-zinc-500"; "text-base"; "ml-2" ]
          prop.text (sprintf "%d" list.Reminders.Length)
        ]
      ]
    ]

  let private newListListItem (newList : NewReminderList) (onNameChange : string -> unit) onCommit =
    Html.div [
      prop.className [ "flex"; "flex-row"; "items-center"; "rounded-md"; "p-2"; "cursor-pointer" ]
      prop.children [
        Html.div [
          prop.className [ "flex"; "flex-row"; "justify-center"; "items-center"; "w-8"; "h-8"; "rounded-full"; "mr-2" ]
          prop.style [ style.backgroundColor newList.Color ]
          prop.children [
            Html.i [
              prop.className [ "fa-solid"; "fa-list-ul"; "text-white" ]
            ]
          ]
        ]

        Html.input [
          prop.className [ "grow"; "bg-zinc-800"; "text-white"; "focus:ring-0"; "focus:ring-offset-0" ]
          prop.type' "text"
          prop.valueOrDefault newList.Name
          prop.onChange (fun newValue -> onNameChange(newValue))
          prop.onKeyUp (fun e ->
            if e.key = "Enter"
            then onCommit()
          )
          prop.onBlur (fun _ -> onCommit())
        ]
      ]
    ]

  let private sidebar state dispatch =
    Html.section [
      prop.className "w-full"
      prop.children [
        Html.div [
          prop.className [ "flex"; "flex-row"; "items-baseline" ]
          prop.children [
            Html.h1 [
              prop.className [ "grow"; "text-2xl"; "text-white"; "font-bold"; "ml-2"; "mb-2" ]
              prop.text "My Lists"
            ]

            Html.div [
              prop.className [ "flex"; "flex-row"; "items-center"; "cursor-pointer" ]
              prop.onClick (fun _ -> 
                let newList = 
                  { Name  = "New list"
                    Color = getRandomColor() }

                dispatch (SetNewReminderList newList)  
              )
              prop.children [
                Html.i [
                  prop.className [ "fa-solid"; "fa-circle-plus"; "text-zinc-400"; "mr-1" ]
                ]

                Html.p [
                  prop.className "text-zinc-400"
                  prop.text "Add List"
                ]
              ]
            ]
          ]
        ]

        Html.div [
          prop.className "w-full"
          prop.children [
            for reminderList in state.Lists ->              
              sidebarListItem 
                reminderList 
                (
                  state.SelectedList
                  |> Option.map (fun sl -> sl.List = reminderList)
                  |> Option.defaultValue false
                )
                (fun _ -> reminderList |> (SelectList >> dispatch))

            yield state.NewReminderList
            |> Option.map (fun nl -> 
              newListListItem 
                nl 
                (fun name -> dispatch (SetNewReminderList { nl with Name = name } ))
                (fun _ -> dispatch CommitNewReminderList)
              )
              |> Option.defaultValue (React.fragment [])
          ]
        ]
      ]
    ]

  let render state dispatch =
    Html.div [
      prop.className [ "container"; "mx-auto"; "flex"; "flex-row"; "bg-zinc-900"; "overflow-clip"; "h-screen"; "max-w-5xl"; "rounded-xl"; "lg:my-8" ]
      prop.children [
        Html.aside [
          prop.className [ "h-full"; "w-1/3"; "min-w-fit"; "bg-zinc-800"; "border-r"; "border-black"; "p-4" ]
          prop.children [
            sidebar state dispatch
          ]
        ]

        Html.section [
          prop.className [ "h-full"; "grow" ]
          prop.children [
            state.SelectedList
            |> Option.map (fun st -> 
              ReminderListView.render st (SelectedList >> dispatch)
            )
            |> Option.defaultValue (
              Html.div [
                prop.className [ "flex"; "flex-row"; "justify-center"; "items-center"; "h-full" ]
                prop.children [
                  Html.p [
                    prop.className [ "text-zinc-500" ]
                    prop.text "Select a list to get started"
                  ]
                ]
              ]
            )
          ]
        ]
      ]
    ]
