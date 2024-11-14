[<RequireQualifiedAccess>]
module App

open Feliz

type Reminder =
  { Id: int
    Task: string
    IsCompleted: bool }

type ReminderList =
  { Id : int
    Name : string
    Color : string
    Reminders : Reminder list }

type State =
  { Lists : ReminderList list
    SelectedList : ReminderList option }

type Msg = exn

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
      ]}
    ]
    SelectedList = None }

let update msg state = state

let render state dispatch =
  Html.div [
    prop.className [ "container"; "mx-auto"; "flex"; "flex-row"; "bg-zinc-900"; "overflow-clip"; "h-screen"; "max-w-5xl"; "rounded-xl"; "lg:my-8" ]
    prop.children [
      Html.aside [
        prop.className [ "h-full"; "w-1/3"; "min-w-fit"; "bg-zinc-800"; "border-r"; "border-black"; "p-4" ]
        prop.children [
          Html.p [
            prop.className [ "text-white" ]
            prop.text "[Insert sidebar content here]"
          ]
        ]
      ]

      Html.section [
        prop.className [ "h-full"; "grow" ]
        prop.children [
          Html.p [
            prop.className [ "text-white" ]
            prop.text "[Insert selected list content here]"
          ]
        ]
      ]
    ]
  ]
