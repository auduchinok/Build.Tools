#r    @"./fake/fakelib.dll"

open Fake
open System
open System.IO
open System.Xml
open System.Collections.Generic
open Microsoft.FSharp.Collections

let nunitRunners = @"./NUnit.Runners.2.6.4/tools"
let specFlowRunners = "./SpecFlow/tools"
let nuget = @"./nuget/nuget.exe"

type Map<'Key,'Value when 'Key : comparison> with
    member this.get (name: 'Key) =
        this |> Map.find name

let ensureNunitRunner (config : Map<string, string>) =
  let dir = config.get "core:tools" @@ nunitRunners
  if not (directoryExists <| config.get "core:tools" @@ nunitRunners) then
     let args =
         sprintf "Install NUnit.Runners -Version 2.6.4 -OutputDirectory \"%s\""
             (config.get "core:tools")
     let result =
         ExecProcess (fun info ->
             info.FileName <- config.get "core:tools" @@ nuget
             info.Arguments <- args) (TimeSpan.FromMinutes 5.)

     if result <> 0 then
         failwith "NUnit.Runners directory not found, and NuGet install failed."

let ensureSpecFlowRunner (config : Map<string, string>) =
  if not (directoryExists <| config.get "core:tools" @@ specFlowRunners) then
     let args =
         sprintf "install SpecFlow -ExcludeVersion -OutputDirectory \"%s\""
             (config.get "core:tools")
     let result =
         ExecProcess (fun info ->
             info.FileName <- config.get "core:tools" @@ nuget
             info.Arguments <- args) (TimeSpan.FromMinutes 5.)

     if result <> 0 then
         failwith "SpecFlow Runner directory not found, and NuGet install failed."

let mapOfDict (dict : Dictionary<_,_>) =
    dict |> Seq.map (fun kvp -> kvp.Key, kvp.Value) |> Map.ofSeq

let runCmd cmd workingDir args =
    let result = 
        ExecProcess (fun info ->
            info.FileName <- Path.GetFullPath cmd
            info.WorkingDirectory <- Path.GetFullPath workingDir
            info.Arguments <- args) (TimeSpan.FromMinutes 20.)
    if result <> 0 then failwithf "Error running cmd script. File: %s" cmd

let private correctAddinsXml (config : Map<string, string>) file = 
    let xdoc = new XmlDocument()
    ReadFileAsString file |> xdoc.LoadXml
    let pathToAddins = xdoc.SelectSingleNode "/Addins/Directory"
    pathToAddins.InnerText <- config.get "monoaddins:pathtonodes"
    WriteStringToFile false file (xdoc.OuterXml.ToString().Replace("><",">\n<"))

let correctPathToAddins (config : Map<string, string>) _ =
    !! (config.get "monoaddins:pathtoconfigxml")
        |> Seq.iter (correctAddinsXml config)