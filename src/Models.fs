module Models

type NewReminderList =
  { Name : string
    Color : string }

type Reminder =
  { Id : int
    Task : string 
    IsCompleted : bool }

type ReminderList =
  { Id : int
    Name : string
    Color : string
    Reminders : Reminder list }
