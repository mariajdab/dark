module LibBackend.Serialization

open System.Runtime.InteropServices
open System.Threading.Tasks
open FSharp.Control.Tasks
open FSharpPlus
open Npgsql.FSharp
open Ply
open Npgsql
open Db
open Prelude
open FSharp.Data

open LibExecution.Runtime
open LibExecution.Framework

let gid = Shortcuts.gid


module OCamlInterop =
  module Binary =
    // FSTODO if we have segfaults, we might need to use this:
    // https://docs.microsoft.com/en-us/dotnet/standard/native-interop/best-practices#keeping-managed-objects-alive

    // initialize OCaml runtime
    [<DllImport("./libserialization.so",
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "dark_init_ocaml")>]
    extern string darkInitOcaml()

    // take a binary rep of a handler from the DB and convert it into JSON
    [<DllImport("./libserialization.so",
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "handler_bin2json")>]
    extern string handlerBin2Json(byte[] bytes, int length)

    [<DllImport("./libserialization.so",
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "handler_json2bin")>]
    extern int handlerJson2Bin(string str, System.IntPtr& byteArray)

    [<DllImport("./libserialization.so",
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "digest")>]
    extern string digest()

    let init () =
      printfn "serialization_init"
      let str = darkInitOcaml ()
      printfn "serialization_inited: %s" str
      ()


  module Yojson =
    // Yojson is the OCaml automated JSON format. This module generates and
    // parses JSON in that format, so that it can be sent to OCaml.

    // FSTODO: if we need speed, switch to transferring using protobufs instead of
    // JSON, where do we get the protobuf files?
    // - 1) generate using ocaml types (maybe deprecated? ocaml-protoc)
    // - 2) generate using FSharp types (possible?)
    // - 3) hand-write them (converters on both ends)
    type J = JsonValue

    let ofTLID (tlid : tlid) : JsonValue = J.String(tlid.ToString())
    let ofID (id : id) : JsonValue = J.String(id.ToString())

    let rec ofExpr (e : Expr) : JsonValue =
      let v (name : string) (args : JsonValue list) =
        (J.String name :: args) |> List.toArray |> J.Array

      match e with
      | EBlank id -> v "EBlank" [ ofID id ]
      | EPartial (id, name, expr) ->
          v "EPartial" [ ofID id; J.String name; ofExpr expr ]
      | ERightPartial (id, name, expr) ->
          v "ELeftPartial" [ ofID id; J.String name; ofExpr expr ]
      | ELeftPartial (id, name, expr) ->
          v "ERightPartial" [ ofID id; J.String name; ofExpr expr ]
      | EString (id, s) -> v "EString" [ ofID id; J.String s ]
      | _ -> failwith $"Not supported yet in ofExpr: {e}"
    // | EPipeTarget id ->
    // | ELet (_id, lhs, rhs, body) ->
    // | EBool (_id, b) ->
    // | EInteger (_id, i) ->
    // | EFloat (_id, whole, fractional) ->
    // | ENull _id ->
    // | ECharacter (_id, s) ->
    // | EList (_id, exprs) ->
    // | EVariable (_id, name) ->
    // | ERecord (id, pairs) ->
    // | EFnCall (id, desc, exprs, ster) ->
    // | EBinOp (id, desc, arg1, arg2, ster) ->
    // | EFieldAccess (id, _, _) ->
    // | EFeatureFlag (id, _, cond, oldcode, newcode) ->
    // | ELambda (_id, parameters, body) ->
    // | EPipe (id, e1, e2, rest) ->
    // | EMatch (id, matchExpr, cases) ->
    //       match pattern with
    //       | PInteger (pid, i) ->
    //       | PBool (pid, bool) ->
    //       | PCharacter (pid, c) ->
    //       | PString (pid, str) ->
    //       | PFloat (pid, whole, fraction) ->
    //       | PNull (pid) ->
    //       | PVariable (pid, v) ->
    //       | PBlank (_pid) ->
    //       | PConstructor (pid, name, args) ->
    // | EIf (_id, cond, thenbody, elsebody) ->
    // | EConstructor (id, name, args) ->
    //

    let ofHandler (h : Handler.T) : JsonValue =
      // { module_ : string or_blank [@key "module"]
      // ; name : string or_blank
      // ; modifier : string or_blank
      // ; types : spec_types }

      let spec =
        let blankOr (id : id) (v : string) : JsonValue =
          if v = "" then
            J.Array [| J.String "Blank"; ofID id |]
          else
            J.Array [| J.String "Filled"; ofID id; J.String v |]

        // deprecated but still needed :( to be the right shape
        let specTypes =
          J.Record [| "input", blankOr (gid ()) ""; "output", blankOr (gid ()) "" |]

        match h.spec with
        | Handler.REPL (name, ids) ->
            J.Record [| "module", blankOr ids.moduleID "REPL"
                        "name", blankOr ids.nameID name
                        "modifier", blankOr ids.modifierID "_"
                        "types", specTypes |]
        | Handler.HTTP (path, modifier, ids) ->
            J.Record [| "module", blankOr ids.moduleID "HTTP"
                        "name", blankOr ids.nameID path
                        "modifier", blankOr ids.modifierID modifier
                        "types", specTypes |]
        | _ -> failwith $"More to handle in ofHandler {h.spec}"

      let result =
        J.Record [| "tlid", ofTLID h.tlid; "ast", ofExpr h.ast; "spec", spec |]

      printfn $"json of handler: {result}"
      result


    let rec toExpr (j : JsonValue) : Expr =

      let e = toExpr
      let es exprs = exprs |> Array.map e |> Array.toList

      let toRail (j : JsonValue) : LibExecution.Runtime.SendToRail =
        match j with
        | J.Array [| J.String "Rail" |] -> Rail
        | J.Array [| J.String "NoRail" |] -> NoRail
        | _ -> failwith $"Unimplemented {j}"

      let rec toPattern (j : JsonValue) : Pattern =
        printfn $"toPattern {j}"
        let constructor = j.Item(0).AsString()
        if constructor = "FPString" then
          // FPString uses a different format than the others
          let obj = j.Item(1)
          PString(obj.Item("patternID").AsInteger64(), obj.Item("str").AsString())
        else
          // skip the matchID in j.Item(1), we know from context
          let id = j.Item(2).AsInteger64()
          match constructor, j.AsArray().[3..] |> Array.toList with
          | "FPBlank", _ -> PBlank id
          | "FPInteger", [ J.String i ] -> PInteger(id, i)
          | "FPBool", [ J.Boolean bool ] -> PBool(id, bool)
          | "FPFloat", [ J.String whole; J.String fraction ] ->
              PFloat(id, whole, fraction)
          | "FPNull", _ -> PNull id
          | "FPVariable", [ J.String name ] -> PVariable(id, name)
          | "FPConstructor", [ J.String name; J.Array pats ] ->
              PConstructor(id, name, Array.map toPattern pats |> Array.toList)

          | _ -> failwith $"Unimplemented {j}"




      let constructor = j.Item(0).AsString()
      let id = j.Item(1).AsInteger64()
      match constructor, j.AsArray().[2..] |> Array.toList with
      | "EString", [ J.String str ] -> EString(id, str)
      | "EInteger", [ J.String i ] -> EInteger(id, i)
      | "EBool", [ J.Boolean bool ] -> EBool(id, bool)
      | "EFloat", [ J.String whole; J.String fraction ] ->
          EFloat(id, whole, fraction)
      | "ENull", _ -> ENull id
      | "EBlank", _ -> EBlank(id)
      | "EPipeTarget", _ -> EPipeTarget(id)
      | "EVariable", [ J.String var ] -> EVariable(id, var)
      | "EList", [ J.Array exprs ] -> EList(id, es exprs)
      | "ELet", [ J.String name; rhs; body ] -> ELet(id, name, e rhs, e body)
      | "EIf", [ cond; thenBody; elseBody ] ->
          EIf(id, e cond, e thenBody, e elseBody)

      | "EPipe", [ J.Array exprs ] ->
          match es exprs with
          | [] -> failwith "empty pipe"
          | e :: [] -> e
          | e1 :: e2 :: rest -> EPipe(id, e1, e2, rest)
      | "ERecord", [ J.Array rows ] ->
          let rows =
            rows
            |> Array.toList
            |> List.map (fun (row : JsonValue) ->
                 match row with
                 | J.Array [| J.String key; value |] -> (key, e value)
                 | _ -> failwith "unexpected record row shape")

          ERecord(id, rows)
      | "EMatch", [ cond; J.Array rows ] ->
          let rows =
            rows
            |> Array.toList
            |> List.map (fun (row : JsonValue) ->
                 match row with
                 | J.Array [| pat; value |] -> (toPattern pat, e value)
                 | _ -> failwith "unexpected record row shape")

          EMatch(id, e cond, rows)


      | "EConstructor", [ J.String name; J.Array exprs ] ->
          EConstructor(id, name, es exprs)

      // FSTODO: sometimes these are binops, probably cause we don't load the
      // library in OCaml. Nevermind, we can do it over here.
      | "EFnCall", [ J.String name; J.Array args; rail ] ->
          let desc =
            match name with
            | Regex "(.+)/(.+)/(.+)::(.+)_v(\d+)"
                    [ owner; package; module'; name; version ] ->
                FnDesc.fnDesc owner package module' name (int version)
            | Regex "(.+)::(.+)_v(\d+)" [ module'; name; version ] ->
                FnDesc.stdFnDesc module' name (int version)
            | Regex "(.+)::(.+)" [ module'; name ] -> FnDesc.stdFnDesc module' name 0
            | Regex "(.+)_v(\d+)" [ name; version ] ->
                FnDesc.stdFnDesc "" name (int version)
            | Regex "(.+)" [ name ] -> FnDesc.stdFnDesc "" name 0
            | _ -> failwith $"Bad format in function name: \"{name}\""

          EFnCall(id, desc, Array.map e args |> Array.toList, toRail rail)
      | _ -> failwith $"Unimplemented expr {j}"

    let toHandler (j : JsonValue) : Handler.T =
      let toHandlerSpec (j : JsonValue) : Handler.Spec =
        let toBlankOr (j : JsonValue) : (id * string) =
          // element 0 is "filled" or "blank"
          let value =
            j.AsArray()
            |> Array.tryItem 2
            |> Option.map (fun j -> j.AsString())
            |> Option.defaultValue ""

          (j.Item(1).AsInteger64(), value)

        let moduleID, module_ = j.Item("module") |> toBlankOr
        let nameID, name = j.Item("name") |> toBlankOr
        let modifierID, modifier = j.Item("modifier") |> toBlankOr

        let (ids : Handler.ids) =
          { moduleID = moduleID; nameID = nameID; modifierID = modifierID }

        if module_ = "" then
          Handler.REPL(name = name, ids = ids)
        else if module_ = "REPL" then
          Handler.REPL(name = name, ids = ids)
        else if module_ = "HTTP" then
          Handler.HTTP(path = name, method = modifier, ids = ids)
        else if module_ = "CRON" then
          Handler.Cron(name = name, interval = modifier, ids = ids)
        else if module_ = "WORKER" then
          Handler.Worker(name = name, ids = ids)
        else
          Handler.OldWorker(modulename = module_, name = name, ids = ids)

      { tlid = j.Item("tlid").AsInteger64()
        ast = j.Item("ast") |> toExpr
        spec = j.Item("spec") |> toHandlerSpec }

let toplevelOfCachedBinary ((data, pos) : (byte array * string option)) : Toplevel =
  OCamlInterop.Binary.handlerBin2Json (data, data.Length)
  |> FSharp.Data.JsonValue.Parse
  |> OCamlInterop.Yojson.toHandler
  |> TLHandler

let toplevelToCachedBinary (toplevel : Toplevel) : byte array =
  match toplevel with
  | TLHandler h ->
      let json = (OCamlInterop.Yojson.ofHandler h).ToString()
      // printfn $"json: {json}"

      // FSTODO: note that we pass an IntPtr - bytes are a different size to ints. Does this matter?
      let mutable destPtr = System.IntPtr()
      let length = OCamlInterop.Binary.handlerJson2Bin (json, &destPtr)
      // printfn $"length: {length}"


      let mutable (bytes : byte array) = Array.zeroCreate length
      Marshal.Copy(destPtr, bytes, 0, length)
      // let newJson = OCamlInterop.Binary.handlerBin2Json (bytes, length)
      // printfn $"newJson: {newJson}"
      bytes

  | _ -> failwith $"toplevel not supported yet {toplevel}"


let fetchReleventTLIDsForHTTP (host : string)
                              (canvasID : CanvasID)
                              (path : string)
                              (method : string)
                              : Task<List<tlid>> =

  // The pattern `$2 like name` is deliberate, to leverage the DB's
  // pattern matching to solve our routing.
  Sql.query "SELECT tlid
             FROM toplevel_oplists
             WHERE canvas_id = @canvasID
               AND ((module = 'HTTP'
                     AND @path like name
                     AND modifier = @method)
               OR tipe <> 'handler'::toplevel_type)"
  |> Sql.parameters [ "path", Sql.string path
                      "method", Sql.string method
                      "canvasID", Sql.uuid canvasID ]
  |> Sql.executeAsync (fun read -> read.int64 "tlid")

let canvasIDForCanvas (owner : UserID) (canvasName : string) : Task<CanvasID> =
  printfn $"calling canvasIDForCanvas {owner} {canvasName}"
  if canvasName.Length > 64 then
    failwith $"Canvas name was length {canvasName.Length}, must be <= 64"
  else
    // TODO: we create the canvas if it doesn't exist here, seems like a poor choice
    Sql.query "SELECT canvas_id(@newUUID, @owner, @canvasName)"
    |> Sql.parameters [ "newUUID", Sql.uuid (System.Guid.NewGuid())
                        "owner", Sql.uuid owner
                        "canvasName", Sql.string canvasName ]
    |> Sql.executeRowAsync (fun read -> read.uuid "canvas_id")
    |> debugTask "canvasID"

// split into owner and canvasName
let ownerNameFromHost (host : string) : string =
  match host.Split [| '-' |] |> Seq.toList with
  | owner :: _rest -> owner
  | _ -> host

let userIDForUsername (user : string) : Task<UserID> =
  let owner = String.toLower user
  if List.contains owner Account.bannedUsernames then
    failwith "Banned username"
  else
    Sql.query "SELECT id
               FROM accounts
               WHERE accounts.username = @username"
    |> Sql.parameters [ "username", Sql.string user ]
    |> Sql.executeRowAsync (fun read -> read.uuid "id")


let canvasNameFromCustomDomain host : Task<Option<string>> =
  Sql.query "SELECT canvas
             FROM custom_domains
             WHERE host = @host"
  |> Sql.parameters [ "host", Sql.string host ]
  |> Sql.executeRowOptionAsync (fun read -> read.string "canvas")




let loadUncachedToplevels (host : string)
                          (canvasID : CanvasID)
                          (tlids : List<tlid>)
                          : Task<List<byte array>> =

  Sql.query "SELECT data
             FROM toplevel_oplists
             WHERE canvas_id = @canvasID
             AND tlid = ANY(@tlids)"
  |> Sql.parameters [ "canvasID", Sql.uuid canvasID; "tlids", Sql.tlidArray tlids ]
  |> Sql.executeAsync (fun read -> read.bytea "data")

let fetchCachedToplevels (host : string)
                         (canvasID : CanvasID)
                         (tlids : List<tlid>)
                         : Task<List<byte array * string option>> =
  Sql.query "SELECT rendered_oplist_cache, pos FROM toplevel_oplists
             WHERE canvas_id = @canvasID
             AND tlid = ANY (@tlids)
             AND deleted IS FALSE
             AND (((tipe = 'handler'::toplevel_type) AND pos IS NOT NULL))"
  |> Sql.parameters [ "canvasID", Sql.uuid canvasID; "tlids", Sql.tlidArray tlids ]
  |> Sql.executeAsync (fun read ->
       (read.bytea "rendered_oplist_cache", read.stringOrNone "pos"))


let loadHttpHandlersFromCache (host : string)
                              (canvasID : CanvasID)
                              (owner : UserID)
                              (path : string)
                              (method : string)
                              : Task<List<Toplevel>> =
  task {
    let! tlids = fetchReleventTLIDsForHTTP host canvasID path method
    let! binaryTLs = fetchCachedToplevels host canvasID tlids
    let tls = List.map toplevelOfCachedBinary binaryTLs

    return tls
  }

// FSTODO This is for testing only as it blows away the old oplist, which is
// needed for undos.
let saveCachedToplevelForTestingOnly (canvasID : CanvasID)
                                     (ownerID : UserID)
                                     (tl : Toplevel)
                                     : Task<int> =
  let module_, path, modifier =
    match tl with
    | TLDB _
    | TLFunction _
    | TLType _ -> None, None, None
    | TLHandler h ->
        // FSTODO munge path for postgres, see munge_name in canvas.ml
        match h.spec with
        | Handler.HTTP (path, modifier, _) -> Some "HTTP", Some path, Some modifier
        | Handler.Worker (name, _) -> Some "Worker", Some name, Some "_"
        | Handler.OldWorker (modulename, name, _) ->
            Some modulename, Some name, Some "_"
        | Handler.Cron (name, interval, _) -> Some "CRON", Some name, Some interval
        | Handler.REPL (name, _) -> Some "REPL", Some name, Some "_"

  let sqlType : string =
    match tl with
    | TLDB _ -> "db"
    | TLHandler _ -> "handler"
    | TLFunction _ -> "user_function"
    | TLType _ -> "user_tipe"

  let cacheBinary = toplevelToCachedBinary tl // FSTODO pos
  let (oplistBinary : byte array) = [||] // FSTODO get an actual oplist
  let digest = OCamlInterop.Binary.digest () // FSTODO digest is wrong
  let pos = Some "{ \"x\": 0, \"y\": 0 }"
  printfn $"digest {digest}"
  Sql.query "INSERT INTO toplevel_oplists
               (canvas_id, account_id, tlid, digest, tipe, name, module, modifier,
                data, rendered_oplist_cache, deleted, pos)
             VALUES (@canvasID, @ownerID, @tlid, @digest, @tipe::toplevel_type,
                     @path, @module, @modifier, @oplistData, @cacheData, @deleted, @pos)
             ON CONFLICT (canvas_id, tlid) DO UPDATE
             SET account_id = @ownerID,
                 digest = @digest,
                 tipe = @tipe::toplevel_type,
                 name = @path,
                 module = @module,
                 modifier = @modifier,
                 data = @oplistData,
                 rendered_oplist_cache = @cacheData,
                 deleted = @deleted,
                 pos = @pos"
  |> Sql.parameters [ "canvasID", Sql.uuid canvasID
                      "ownerID", Sql.uuid ownerID
                      "tlid", Sql.int64 (tl.toTLID ())
                      "digest", Sql.string digest
                      "tipe", Sql.string sqlType
                      "path", Sql.stringOrNone path
                      "module", Sql.stringOrNone module_
                      "modifier", Sql.stringOrNone modifier
                      "oplistData", Sql.bytea oplistBinary
                      "@cacheData", Sql.bytea cacheBinary
                      "deleted", Sql.bool false // FSTODO
                      "pos", Sql.jsonbOrNone pos ] // FSTODO
  |> Sql.executeNonQueryAsync

let saveHttpHandlersToCache (canvasID : CanvasID)
                            (ownerID : UserID)
                            (tls : List<Toplevel>)
                            : Task<unit> =
  task {
    let results =
      tls
      |> List.map (fun tl ->
           (saveCachedToplevelForTestingOnly canvasID ownerID tl) :> Task)
      |> List.toArray

    return Task.WaitAll(results)
  }
