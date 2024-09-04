[<RequireQualifiedAccess>]
module App

open Colors
open Feliz

type Reminder =
  { Id : int
    Task : string 
    IsCompleted : bool }

type ReminderList =
  { Id : int
    Name : string
    Color : string
    Reminders : Reminder list }

type NewReminderList =
  { Name : string
    Color : string }

type SelectedListState =
  { List : ReminderList
    NewReminder : string option }

type State =
  { Lists : ReminderList list
    SelectedListState : SelectedListState option
    NewReminderList : NewReminderList option }

type Msg = 
  | SetSelectedListState of SelectedListState
  | SetNewReminderList of NewReminderList
  | CommitNewReminderList
  | CommitNewReminder
  | ToggleReminderIsCompleted of ReminderList * Reminder

let init () =
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
    SelectedListState = None 
    NewReminderList = None }

let update msg state =
  match msg with 
  | SetSelectedListState slState ->
    { state with SelectedListState = Some slState }
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
  | CommitNewReminder ->
    state.SelectedListState
    |> Option.bind (fun sl ->
      sl.NewReminder
      |> Option.map(fun nr ->
        let nextId =
          match sl.List.Reminders with
          | [ ] -> 1
          | elems ->
            elems
            |> List.maxBy (fun l -> l.Id)
            |> fun l -> l.Id + 1
        
        let newReminder =
          { Id = nextId
            Task = nr
            IsCompleted = false }

        let newReminders = List.append sl.List.Reminders [newReminder]

        let newList = { sl.List with Reminders = newReminders }

        let newLists =
          state.Lists
          |> List.map (fun l ->
            if l.Id = sl.List.Id
            then newList
            else l
          )
        let curSl = state.SelectedListState |> Option.get

        { state with 
            Lists = newLists
            SelectedListState = 
              Some { curSl with List = newList; NewReminder = None } 
        }
      )
    )
    |> Option.defaultValue state
  | ToggleReminderIsCompleted (list, reminder) ->
    let nextReminder =
      { reminder with IsCompleted = not reminder.IsCompleted }

    let nextReminders = 
      list.Reminders
      |> List.map (fun r -> if r.Id = nextReminder.Id then nextReminder else r)

    let nextList = { list with Reminders = nextReminders }

    let nextLists =
      state.Lists
      |> List.map (fun l -> if l.Id = list.Id then nextList else l)

    let nextSelectedList =
      state.SelectedListState
      |> Option.map (fun sl ->
        { sl with List = nextList }
      )

    { state with 
        Lists = nextLists
        SelectedListState = nextSelectedList }

// Render

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
    prop.onClick (fun _ -> onClick list.Id)
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

let private newListListItem newList (onNameChange : string -> unit) onCommit =
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
                state.SelectedListState
                |> Option.map (fun sl -> sl.List = reminderList)
                |> Option.defaultValue false
              )
              (fun _ -> dispatch (SetSelectedListState { List = reminderList; NewReminder = None }))

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

// Selected list
let private selectedListListItem (list : ReminderList) reminder onClick =
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
        prop.onChange (fun newValue -> onNameChange(newValue))
        prop.onKeyUp (fun e ->
          if e.key = "Enter"
          then onCommit()
        )
        prop.onBlur (fun _ -> onCommit())
      ]
    ]
  ]

let private selectedList (state : SelectedListState) dispatch =
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
            selectedListListItem 
              state.List 
              reminder
              (fun () -> dispatch (ToggleReminderIsCompleted (state.List, reminder)))
            
          yield 
            state.NewReminder
            |> Option.map (fun nr -> 
                newReminderListItem 
                  nr 
                  (fun name -> dispatch (SetSelectedListState { state with NewReminder = Some name } ))
                  (fun _ -> dispatch CommitNewReminder)
            )
            |> Option.defaultValue (
              Html.div [
                prop.className [ "flex"; "flex-row"; "items-center"; "p-2"; "cursor-pointer" ]
                prop.onClick (fun _ -> 
                  dispatch (SetSelectedListState { state with NewReminder = Some "New reminder" })  
                )
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
          state.SelectedListState
          |> Option.map (fun st -> 
            selectedList st dispatch
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
