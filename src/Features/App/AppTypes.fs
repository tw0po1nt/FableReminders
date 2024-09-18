namespace Features.App

open Common
open Features
open Models

type State =
  { Lists : ReminderList list
    NewReminderList : NewReminderList option 
    SelectedList : ReminderList.State option }

type Msg = 
  | OnAppear
  | SetLists of ReminderList list
  | SetNewReminderList of NewReminderList
  | CommitNewReminderList
  | CreateNewReminderListResult of Eager.Create<int, ReminderList, string>
  | SelectList of ReminderList
  | SelectedList of ReminderList.Msg
