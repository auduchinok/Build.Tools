#r "./fake/fakelib.dll"
#load "./Utils.fsx"

open Fake
open Utils
open System
open System.IO

let run (config : Map<string, string>) _ =
    let testDlls = !! (sprintf @"%s" (config.get "test:path"))
    if Seq.length testDlls > 0 then
        ensureNunitRunner config
        testDlls
        |> NUnit 
            (fun defaults ->
                { defaults with 
                    ToolPath = config.get "core:tools" @@ nunitRunners
                 })
