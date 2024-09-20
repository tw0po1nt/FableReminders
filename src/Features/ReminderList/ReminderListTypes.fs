namespace Features.ReminderList

open Common
open Models

type State =
  { List : ReminderList
    NewReminder : string option }

type Msg =
  | SetNewReminder of string
  | ToggleReminderIsCompleted of Reminder
  | UpdateReminderResult of Eager.Update<int, Reminder, string>
  | CommitNewReminder
  | AddNewReminderResult of Eager.Create<int, Reminder, string>

type Intent =
  | ReminderUpdated of ReminderList * Reminder
  | ReminderAdded of ReminderList * Reminder
  | ReminderRemoved of ReminderList * Reminder
  | DoNothing
