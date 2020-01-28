open Prelude

type inputEvent =
  { data : string
  ; inputType : string }

let isInfixSymbol (s : string) : bool =
  match s with
  | "+" | "%" | "-" | "*" | "/" | "<" | ">" | "&" | "!" | "^" | "=" | "|" ->
      true
  | _ ->
      false


let fromInputEvent (evt : Web.Node.event) : msg option =
  let open Tea.Json.Decoder in
  let decoder =
    map2
      (fun data inputType -> {data; inputType})
      (field "data" string)
      (field "inputType" string)
  in
  decodeEvent decoder evt
  |> Tea_result.result_to_option
  |> Option.andThen ~f:(function
         | {inputType = "insertText"; data = " "} ->
             (* space is bound up with autocomplete and a bunch of other stuff
              * that's hard to detangle, so it's still handled as a Keypress
              * for now (see FluidKeyboard) *)
             None
         | {inputType = "insertText"; data} ->
             evt##preventDefault () ;
             Some (FluidMsg (FluidInputEvent (InsertText data)))
         | _ ->
             None)


let fromCompositionEndEvent (evt : Web.Node.event) : msg option =
  let open Tea.Json.Decoder in
  decodeEvent (field "data" string) evt
  |> Tea_result.result_to_option
  |> Option.map ~f:(fun data ->
         evt##preventDefault () ;
         FluidMsg (FluidInputEvent (InsertText data)))