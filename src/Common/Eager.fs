namespace Common

[<RequireQualifiedAccess>]
module Eager =
  /// An eager, failable creation operation
  type Create<'id, 'entity, 'error> =
    | Success of EagerCreateSuccess<'id, 'entity>
    | Failure of EagerCreateFailure<'id, 'error> 

  and EagerCreateSuccess<'id, 'entity> =
    { Id : 'id
      Created: 'entity }

  and EagerCreateFailure<'id, 'error> =
    { Id : 'id
      Error : 'error }

  /// An eager, failable update operation
  type Update<'id, 'entity, 'error> =
    | Success of EagerUpdateSuccess<'id, 'entity>
    | Failure of EagerUpdateFailure<'id, 'entity, 'error>

  and EagerUpdateSuccess<'id, 'updated> =
    { Id : 'id
      Updated : 'updated }

  and EagerUpdateFailure<'id, 'entity, 'error> =
    { Id : 'id
      Previous: 'entity
      Error: 'error }
  