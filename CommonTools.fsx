#r @"Fake/FakeLib.dll"
#load "Core.fsx"

open Fake
open Fake.Git
open Core
open System.IO

type SpecificConfig = 
    struct
        val PathToRepository: string
        val PathToDll: string
        val PathToSolution: string
        val PathToTests: string
        val PathToTools: string
        val PathToPackages: string
        val PathToNuspec: string
        val PathToNuspecFromRoot: string
        val PathToAssembleyInfo: string
        val PathToAssembleyInfoFromRoot: string

        new(repo: string, dll: string, sol: string, test: string, tool: string, pack: string, nusp: string, nusproot: string, assem: string, assemroot: string) = { 
             PathToRepository = repo;
             PathToDll = dll;
             PathToSolution = sol;
             PathToTests = test;
             PathToTools = tool;
             PathToPackages = pack;
             PathToNuspec = nusp;
             PathToNuspecFromRoot = nusproot;
             PathToAssembleyInfo = assem;
             PathToAssembleyInfoFromRoot = assemroot
        }
    end

let pushURL = @"https://www.myget.org/F/yc/api/v2/package"
let pushApiKey = @"f6ba9139-9d42-4cf1-acaf-344f963ff807"

let commitMessage = @"Change version of package in AssemblyInfo and Nuspec files"
let gitUserName = "YcGeneralUser"
let gitPassword = "yc2GeneralUser2014"
let gitCommandToPush = sprintf "push --repo https://\"%s\":\"%s\"@github.com/YaccConstructor/YC.Utils.SourceText.git" gitUserName gitPassword

let commonConfig (tools : SpecificConfig) = 
    let gitCommandToCommit = sprintf "commit -m \"%s\" \"%s\" \"%s\"" commitMessage tools.PathToAssembleyInfoFromRoot tools.PathToNuspecFromRoot

    config.["build:solution"] <- tools.PathToSolution
    config.["core:tools"] <- tools.PathToTools
    config.["bin:path"] <- tools.PathToDll
    config.["repo:path"] <- tools.PathToRepository
    config.["test:path"] <- tools.PathToTests
    config.["packaging:nuspecpath"] <- tools.PathToNuspec
    config.["packaging:packages"] <- tools.PathToPackages
    config.["packaging:assemblyinfopath"] <- tools.PathToAssembleyInfo

    config.["packaging:deploypushurl"] <- pushURL
    config.["packaging:deployapikey"] <- pushApiKey

    Target "Version" (fun x ->
        Versioning.update (mapOfDict config) x
        Versioning.updateDeploy (mapOfDict config) x
    )
    Target "Commit" (fun _ ->
        gitCommand tools.PathToRepository gitCommandToCommit
    )
    Target "PushChanges" (fun _ ->
        gitCommand tools.PathToRepository gitCommandToPush
    )
    Target "RestorePackages"    <| Packaging.restore (mapOfDict config)
    Target "Clean"              <| Solution.clean (mapOfDict config)
    Target "Build"              <| Solution.build (mapOfDict config)
    Target "TestRun"            <| Test.run (mapOfDict config)
    Target "Package"            <| Packaging.packageDeploy (mapOfDict config)
    Target "PushPackage"        <| Packaging.pushDeploy (mapOfDict config)
    Target "DefaultTarget"      <| DoNothing