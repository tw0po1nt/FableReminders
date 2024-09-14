namespace Features.App

open Features
open Models

type NewReminderList =
  { Name : string
    Color : string }

type State =
  { Lists : ReminderList list
    NewReminderList : NewReminderList option 
    SelectedList : ReminderList.State option }

type Msg = 
  | SetNewReminderList of NewReminderList
  | CommitNewReminderList
  | SelectList of ReminderList
  | SelectedList of ReminderList.Msg
