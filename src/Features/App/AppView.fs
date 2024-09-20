namespace Features

open Colors
open Features.App
open Lit
open Models

[<RequireQualifiedAccess>]
module AppView =
  // Sidebar
  let private sidebarListItem (list : ReminderList) isSelected onClick =
    html $"""
      <div 
        class={sprintf "flex flex-row items-center rounded-md p-2 cursor-pointer %s" (if isSelected then "bg-[#3164B6]" else "") }
        @click={Ev (ignore >> onClick)}>
        <div 
          class="flex flex-row justify-center items-center w-8 h-8 rounded-full mr-2"
          style={ sprintf "background-color: %s" list.Color }>
          <i class="fa-solid fa-list-ul text-white"></i>
        </div>
        <p class="text-white text-base grow">{list.Name}</p>
        <p class="text-zinc-500 text-base ml-2">{sprintf "%d" list.Reminders.Length}</p>
      </div>
    """

  let private newListListItem (newList : NewReminderList) (onNameChange : string -> unit) onCommit =
    html $"""
      <div class="flex flex-row items-center rounded-md p-2 cursor-pointer">
        <div 
          class="flex flex-row justify-center items-center w-8 h-8 rounded-full mr-2"
          style={sprintf "background-color: %s;" newList.Color}>
          <i class="fa-solid fa-list-ul text-white"></i>
        </div>
        <input
          class="grow bg-zinc-800 text-white focus:ring-0 focus:ring-offset-0"
          type="text"
          value={newList.Name}
          @change={EvVal onNameChange}
          @keyup={Ev (fun e ->
            let keyboardEvent = e :> Browser.Types.KeyboardEvent
            if keyboardEvent.code = "Enter"
            then onCommit()
          )}
          @blur={Ev (ignore >> onCommit)} />
      </div>
    """

  let private sidebar state dispatch =
    html $"""
      <section class="w-full">
        <div class="flex flex-row items-baseline">
          <h1 class="grow text-2xl text-white font-bold ml-2 mb-2">
            My Lists
          </h1>
          <div 
            class="flex flex-row items-center cursor-pointer"
            @click={(fun _ ->
              let newList = 
                  { Name  = "New list"
                    Color = getRandomColor() }

              dispatch (SetNewReminderList newList) 
            )}>
            <i class="fa-solid fa-circle-plus text-zinc-400 mr-1"></i>
            <p class="text-zinc-400">Add list</p>
          </div>
        </div>
        <div class="w-full">
          {state.Lists 
          |> Lit.mapUnique 
            (fun l -> sprintf "%d" l.Id) 
            (fun list -> 
              sidebarListItem 
                list 
                (
                  state.SelectedList
                  |> Option.map (fun sl -> sl.List = list)
                  |> Option.defaultValue false
                )
                (fun _ -> list |> (SelectList >> dispatch))
            )
          }
          {state.NewReminderList
          |> Option.map (fun nl -> 
            newListListItem 
              nl 
              (fun name -> dispatch (SetNewReminderList { nl with Name = name } ))
              (fun _ -> dispatch CommitNewReminderList)
            )
            |> Option.defaultValue Lit.nothing
          }
        </div>
      </section>
    """

  let render state dispatch =
    let noSelectedList =
      html $"""
        <div class="flex flex-row justify-center items-center h-full">
          <p class="text-zinc-500">Select a list to get started</p>
        </div>
      """

    html $"""
      <div class="container mx-auto flex flex-row bg-zinc-900 overflow-clip h-screen max-w-5xl rounded-xl lg:my-8">
        <aside class="h-full w-1/3 min-w-fit bg-zinc-800 border-r border-black p-4">
          {sidebar state dispatch}
        </aside>
        <section class="h-full grow">
          {state.SelectedList
          |> Option.map (fun st -> 
            ReminderListView.render st (SelectedList >> dispatch)
          )
          |> Option.defaultValue noSelectedList
          }
        </section>
      </div>
    """
