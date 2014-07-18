#r "./fake/fakelib.dll"
#load "./Utils.fsx"

open Fake
open Utils
open System
open System.IO

let run (config : Map<string, string>) _ =
    //let testDlls = !! (sprintf @"%s" (config.get "test:path"))
    let pa = sprintf @"%s" (config.get "test:path")
    printf "\n\nPATH  =  %s\n\n" pa
    let pat = Path.GetFullPath(pa)
    printf "PATH  =  %s\n\n" pat

    let testDlls = !! (sprintf @"%s" (config.get "test:path"))

    printf "Count  =  %A\n\n" (Seq.length testDlls)

    if Seq.length testDlls > 0 then
        ensureNunitRunner config
        testDlls
        |> NUnit 
            (fun defaults ->
                { defaults with 
                    ToolPath = config.get "core:tools" @@ nunitRunners
                 })
