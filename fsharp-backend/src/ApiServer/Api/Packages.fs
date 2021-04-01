module ApiServer.Packages

// API endpoints for Packages

open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.EndpointRouting

open System.Threading.Tasks
open FSharp.Control.Tasks
open FSharpPlus
open Prelude
open Tablecloth

module PT = LibBackend.ProgramTypes
module RT = LibExecution.RuntimeTypes
module OT = LibBackend.OCamlInterop.OCamlTypes
module Convert = LibBackend.OCamlInterop.Convert

module Auth = LibBackend.Authorization

module Packages =
  type T = List<OT.PackageManager.fn>

  let packages (ctx : HttpContext) : Task<T> =
    task {
      let t = Middleware.startTimer ctx
      let! fns = Lazy.force LibBackend.PackageManager.cachedForAPI
      t "loadFunctions"
      let result = fns |> List.map Convert.pt2ocamlPackageManagerFn
      t "convertFunctions"
      return result
    }

let endpoints : Endpoint list =
  let h = Middleware.apiHandler

  [ POST [ routef "/api/%s/packages" (h Packages.packages Auth.Read) ] ]

// | `POST, ["api"; canvas; "packages"; "upload_function"] when user.admin ->
//     when_can_edit ~canvas (fun _ ->
//         wrap_editor_api_headers
//           (upload_function ~execution_id ~user parent body))
