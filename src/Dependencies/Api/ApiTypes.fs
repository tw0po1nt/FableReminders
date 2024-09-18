namespace Dependencies.Api

type ApiResponse<'data> =
  { Success : bool
    Message : string option
    Data: 'data option }

type ApiError =
  | NotFound of string
  | BadRequest of string
  | InternalServerError of string

[<RequireQualifiedAccess>]
module ApiError =
  let msg = function
    | NotFound err -> err
    | BadRequest err -> err
    | InternalServerError err -> err

type ReminderListResponse =
  { Id : int
    Name : string
    Color : string
    Reminders : ReminderResponse list }
and ReminderResponse =
  { Id : int
    Task : string
    IsCompleted : bool }
