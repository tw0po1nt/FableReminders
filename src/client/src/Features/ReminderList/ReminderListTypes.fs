namespace Features.ReminderList

open Models

type State =
  { List : ReminderList
    NewReminder : string option }

type Msg =
  | SetNewReminder of string
  | ToggleReminderIsCompleted of Reminder
  | CommitNewReminder

type Intent =
  | ReminderToggled of ReminderList * Reminder
  | NewReminderAdded of ReminderList * Reminder
  | DoNothing
