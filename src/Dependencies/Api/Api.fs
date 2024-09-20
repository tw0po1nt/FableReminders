namespace Dependencies

open Dependencies.Api
open Fable.SimpleHttp
open Fable.Core
open Fable.Core.JS
open FsToolkit.ErrorHandling
open Models
open Thoth.Json

[<RequireQualifiedAccess>]
module ApiDependency =
  let private baseUrl = "http://localhost:3000"

  let getAllReminderLists () : Promise<Result<ReminderListResponse list, ApiError>> = promise {
    let! response =
      Http.request $"{baseUrl}/api/lists"
      |> Http.method GET
      |> Http.header (Headers.accept "application/json")
      |> Http.send
      |> Async.StartAsPromise
    
    return result {
      return!
        Decode.Auto.fromString<ApiResponse<ReminderListResponse list>>(response.responseText, caseStrategy = CamelCase)
        |> Result.mapError (fun e -> BadRequest e)
        |> Result.map (fun r -> r.Data |> Option.defaultValue [])
    }
  }

  let createNewReminderList (newReminderList : NewReminderList) : Promise<Result<ReminderListResponse, ApiError>> = promise {
    let! response =
      Http.request $"{baseUrl}/api/lists"
      |> Http.method POST
      |> Http.header (Headers.accept "application/json")
      |> Http.header (Headers.contentType "application/json")
      |> Http.content (
        BodyContent.Text (Encode.Auto.toString(newReminderList, caseStrategy = CamelCase))
      )
      |> Http.send
      |> Async.StartAsPromise

    return result {
      return!
        Decode.Auto.fromString<ApiResponse<ReminderListResponse>>(response.responseText, caseStrategy = CamelCase)
        |> Result.mapError (fun e -> BadRequest e)
        |> Result.bind (fun r ->
          match response.statusCode with
          | 201 ->
            r.Data 
            |> Option.map (Ok)
            |> Option.defaultValue ("Bad request" |> (BadRequest >> Error))
          | 400 -> r.Message |> Option.defaultValue "Bad request" |> (BadRequest >> Error)
          | _   -> r.Message |> Option.defaultValue "Internal server error" |> (InternalServerError >> Error)
        )
    }
  }

  let createNewReminder (reminderListId : int) (newReminder : string) = promise {
    let! response =
      Http.request $"{baseUrl}/api/lists/{reminderListId}/reminders"
      |> Http.method POST
      |> Http.header (Headers.accept "application/json")
      |> Http.header (Headers.contentType "application/json")
      |> Http.content (
        BodyContent.Text (Encode.Auto.toString({| Task = newReminder |}, caseStrategy = CamelCase))
      )
      |> Http.send
      |> Async.StartAsPromise

    

    return result {
      return!
        Decode.Auto.fromString<ApiResponse<ReminderResponse>>(response.responseText, caseStrategy = CamelCase)
        |> Result.mapError (fun e -> BadRequest e)
        |> Result.bind (fun r ->
          match response.statusCode with
          | 201 ->
            r.Data 
            |> Option.map (Ok)
            |> Option.defaultValue ("Bad request" |> (BadRequest >> Error))
          | 400 -> r.Message |> Option.defaultValue "Bad request" |> (BadRequest >> Error)
          | _   -> r.Message |> Option.defaultValue "Internal server error" |> (InternalServerError >> Error)
        )
    }
  }

  let updateReminder (reminderListId : int) (reminderId : int) (isCompleted : bool) = promise {
    let! response =
      Http.request $"{baseUrl}/api/lists/{reminderListId}/reminders/{reminderId}"
      |> Http.method PATCH
      |> Http.header (Headers.accept "application/json")
      |> Http.header (Headers.contentType "application/json")
      |> Http.content (
        BodyContent.Text (Encode.Auto.toString({| IsCompleted = isCompleted |}, caseStrategy = CamelCase))
      )
      |> Http.send
      |> Async.StartAsPromise

    return result {
      return!
        Decode.Auto.fromString<ApiResponse<ReminderResponse>>(response.responseText, caseStrategy = CamelCase)
        |> Result.mapError (fun e -> BadRequest e)
        |> Result.bind (fun r ->
          match response.statusCode with
          | 200 ->
            r.Data 
            |> Option.map (Ok)
            |> Option.defaultValue ("Bad request" |> (BadRequest >> Error))
          | 400 -> r.Message |> Option.defaultValue "Bad request" |> (BadRequest >> Error)
          | _   -> r.Message |> Option.defaultValue "Internal server error" |> (InternalServerError >> Error)
        )
    }
  }

