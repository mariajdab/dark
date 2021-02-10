module LibExecution.StdLib.LibInt

open System.Threading.Tasks
open System.Numerics
open FSharp.Control.Tasks
open FSharpPlus

open LibExecution.RuntimeTypes
open Prelude

module Errors = LibExecution.Errors

let fn = FQFnName.stdlibName

let err (str : string) = Value(Dval.errStr str)

let incorrectArgs = LibExecution.Errors.incorrectArgs

let varA = TVariable "a"
let varB = TVariable "b"

let fns : List<BuiltInFn> =
  [ { name = fn "Int" "mod" 0
      parameters = [ Param.make "a" TInt ""; Param.make "b" TInt "" ]
      returnType = TInt
      description = "Returns the result of wrapping `a` around so that `0 <= res < b`.
         The modulus `b` must be 0 or negative.
         Use `Int::remainder` if you want the remainder after division, which has a different behavior for negative numbers."
      fn =
        InProcess
          (function
          | state, [ DInt v; DInt m as mdv ] ->
              (try
                Value(DInt(v % m))
               with e ->
                 if m = bigint 0 then
                   err (Errors.argumentWasnt "positive" "b" mdv)
                 else
                   // FSTODO
                   // In case there's another failure mode, rollbar
                   failwith "mod error")
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      (*
         * TODO: Deprecate this when we can version infix operators and when infix operators support Result return types.
         * The current function returns DError (it used to rollbar) on negative `b`.
         *)
      deprecated = NotDeprecated }
    (*  (* See above for when to uncomment this *)
  ; { name = fn "Int" "mod" 1
    ; infix_names = ["%_v1"]
    ; parameters = [Param.make "value" TInt; Param.make "modulus" TInt]
    ; returnType = TResult
    ; description =
        "Returns the result of wrapping `value` around so that `0 <= res < modulus`, as a Result.
         If `modulus` is positive, returns `Ok res`. Returns an `Error` if `modulus` is 0 or negative.
         Use `Int::remainder` if you want the remainder after division, which has a different behavior for negative numbers."
    ; fn =
        (* TODO: A future version should support all non-zero modulus values and should include the infix "%" *)

          (function
          | _, [DInt v; DInt m] ->
            ( try DResult (ResOk (DInt (Dint.modulo_exn v m)))
              with e ->
                if m <= Dint.of_int 0
                then
                  DResult
                    (ResError
                       (Dval.dstr_of_string_exn
                          ( "`modulus` must be positive but was "
                          ^ Dval.to_developer_repr_v0 (DInt m) )))
                else (* In case there's another failure mode, rollbar *)
                  raise e )
          | args ->
              incorrectArgs ())
    ; sqlSpec = NotYetImplementedTODO
      ; previewable = Pure
    ; deprecated = NotDeprecated } *)
    { name = fn "Int" "remainder" 0
      parameters = [ Param.make "value" TInt ""; Param.make "divisor" TInt "" ]
      returnType = TInt
      description = "Returns the integer remainder left over after dividing `value` by `divisor`, as a Result.
          For example, `Int::remainder 15 6 == Ok 3`. The remainder will be negative only if `value < 0`.
          The sign of `divisor` doesn't influence the outcome.
          Returns an `Error` if `divisor` is 0."
      fn =
        InProcess
          (function
          | _, [ DInt v; DInt d ] ->
              (try
                BigInteger.Remainder(v, d) |> DInt |> Value
               with e ->
                 if d = bigint 0 then
                   Value(Dval.errStr (Errors.dividingByZero "divisor"))
                 else (* In case there's another failure mode, rollbar *)
                   raise e)
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "add" 0
      parameters = [ Param.make "a" TInt ""; Param.make "b" TInt "" ]
      returnType = TInt
      description = "Adds two integers together"
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] -> Value(DInt(a + b))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "subtract" 0
      parameters = [ Param.make "a" TInt ""; Param.make "b" TInt "" ]
      returnType = TInt
      description = "Subtracts two integers"
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] -> Value(DInt(a - b))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "multiply" 0
      parameters = [ Param.make "a" TInt ""; Param.make "b" TInt "" ]
      returnType = TInt
      description = "Multiplies two integers"
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] -> Value(DInt(a * b))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "power" 0
      parameters = [ Param.make "base" TInt ""; Param.make "exponent" TInt "" ]
      returnType = TInt
      description = "Raise `base` to the power of `exponent`"
      fn =
        InProcess
          (function
          | state, [ DInt number; DInt exp as expdv ] ->
              (try
                Value(DInt(number ** (int exp)))
               with e ->
                 if exp < bigint 0 then
                   err (Errors.argumentWasnt "positive" "exponent" expdv)
                 else
                   // FSTODO
                   // In case there's another failure mode, rollbar
                   failwith "mod error")
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "divide" 0
      parameters = [ Param.make "a" TInt ""; Param.make "b" TInt "" ]
      returnType = TInt
      description = "Divides two integers"
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] -> Value(DInt(a / b))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "absoluteValue" 0
      parameters = [ Param.make "a" TInt "" ]
      returnType = TInt
      description =
        "Returns the absolute value of `a` (turning negative inputs into positive outputs)."
      fn =
        InProcess
          (function
          | _, [ DInt a ] -> Value(DInt(abs a))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "negate" 0
      parameters = [ Param.make "a" TInt "" ]
      returnType = TInt
      description = "Returns the negation of `a`, `-a`."
      fn =
        InProcess
          (function
          | _, [ DInt a ] -> Value(DInt(-a))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "greaterThan" 0
      parameters = [ Param.make "a" TInt ""; Param.make "b" TInt "" ]
      returnType = TBool
      description = "Returns true if a is greater than b"
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] -> Value(DBool(a > b))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "greaterThanOrEqualTo" 0
      parameters = [ Param.make "a" TInt ""; Param.make "b" TInt "" ]
      returnType = TBool
      description = "Returns true if a is greater than or equal to b"
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] -> Value(DBool(a >= b))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "lessThan" 0
      parameters = [ Param.make "a" TInt ""; Param.make "b" TInt "" ]
      returnType = TBool
      description = "Returns true if a is less than b"
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] -> Value(DBool(a < b))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "lessThanOrEqualTo" 0
      parameters = [ Param.make "a" TInt ""; Param.make "b" TInt "" ]
      returnType = TBool
      description = "Returns true if a is less than or equal to b"
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] -> Value(DBool(a <= b))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "random" 0
      parameters = [ Param.make "start" TInt ""; Param.make "end" TInt "" ]
      returnType = TInt
      description = "Returns a random integer between a and b (inclusive)"
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] ->
              a + bigint (Prelude.random.Next((b - a) |> int)) |> DInt |> Value
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Impure
      deprecated = ReplacedBy(fn "Int" "random" 1) }
    { name = fn "Int" "random" 1
      parameters = [ Param.make "start" TInt ""; Param.make "end" TInt "" ]
      returnType = TInt
      description = "Returns a random integer between `start` and `end` (inclusive)."
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] ->
              let lower, upper = if a > b then (b, a) else (a, b)

              lower + (Prelude.random.Next((upper - lower) |> int) |> bigint)
              |> DInt
              |> Value
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Impure
      deprecated = NotDeprecated }
    { name = fn "Int" "sqrt" 0
      parameters = [ Param.make "a" TInt "" ]
      returnType = TFloat
      description = "Get the square root of an Int"
      fn =
        InProcess
          (function
          | _, [ DInt a ] -> Value(DFloat(sqrt (float a)))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "toFloat" 0
      parameters = [ Param.make "a" TInt "" ]
      returnType = TFloat
      description = "Converts an Int to a Float"
      fn =
        InProcess
          (function
          | _, [ DInt a ] -> Value(DFloat(float a))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    // ; { name = fn "Int" "sum" 0
    //
    //   ; parameters = [Param.make "a" TList]
    //   ; returnType = TInt
    //   ; description = "Returns the sum of all the ints in the list"
    //   ; fn =
    //
    //         (function
    //         | _, [DList l] ->
    //             l
    //             |> list_coerce Dval.to_int
    //             >>| List.fold_left Dint.( + ) Dint.zero
    //             >>| (fun x -> DInt x)
    //             |> Result.map_error (fun (result, example_value) ->
    //                    RT.error
    //                      (DList result)
    //                      (DList result)
    //
    //                        ( "Int::sum requires all values to be integers, but "
    //                        ^ Dval.to_developer_repr_v0 example_value
    //                        ^ " is a "
    //                        ^ Dval.pretty_tipename example_value )
    //                      "every list item to be an int "
    //                      "Sum expects you to pass a list of ints")
    //             |> Result.ok_exn
    //         | args ->
    //             incorrectArgs ())
    //   ; sqlSpec = NotYetImplementedTODO
    //     ; previewable = Pure
    //   ; deprecated = NotDeprecated }
    { name = fn "Int" "max" 0
      parameters = [ Param.make "a" TInt ""; Param.make "b" TInt "" ]
      returnType = TInt
      description = "Returns the higher of a and b"
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] -> Value(DInt(max a b))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "min" 0
      parameters = [ Param.make "a" TInt ""; Param.make "b" TInt "" ]
      returnType = TInt
      description = "Returns the lower of `a` and `b`"
      fn =
        InProcess
          (function
          | _, [ DInt a; DInt b ] -> Value(DInt(min a b))
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated }
    { name = fn "Int" "clamp" 0
      parameters =
        [ Param.make "value" TInt ""
          Param.make "limitA" TInt ""
          Param.make "limitB" TInt "" ]
      returnType = TInt
      description = "If `value` is within the range given by `limitA` and `limitB`, returns `value`.
   If `value` is outside the range, returns `limitA` or `limitB`, whichever is closer to `value`.
   `limitA` and `limitB` can be provided in any order."
      fn =
        InProcess
          (function
          | _, [ DInt v; DInt a; DInt b ] ->
              let min, max = if a < b then (a, b) else (b, a)

              if v < min then Value(DInt min)
              else if v > max then Value(DInt max)
              else Value(DInt v)
          | args -> incorrectArgs ())
      sqlSpec = NotYetImplementedTODO
      previewable = Pure
      deprecated = NotDeprecated } ]