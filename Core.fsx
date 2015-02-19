#r    @"./fake/fakelib.dll"
#load "./Utils.fsx"
#load "./Packaging.fsx"
#load "./Versioning.fsx"
#load "./Solution.fsx"
#load "./Test.fsx"
#load "./Specflow.fsx"

open System
open System.IO
open System.Collections.Generic
open Fake
open Fake.Git
open Utils

let config = 
    let dict = new Dictionary<_,_>()
    [
        "bin:path",                     environVarOrDefault "bin"                   ""
        "build:configuration",          environVarOrDefault "configuration"         "Release"
        "build:solution",               environVar          "solution"
        "core:tools",                   environVar          "tools"
        "monoaddins:pathtonodes",       environVarOrDefault "addins"                ""
        "monoaddins:pathtoconfigxml",   environVarOrDefault "addinsxml"             ""
        "git:user",                     environVar "user_name"
        "git:password",                 environVar "user_password"
        "git:reftorepo",                environVar "ref_to_repo"
        "packaging:output",             environVarOrDefault "output"                (sprintf "%s\output" (Path.GetFullPath(".")))
        "packaging:deployoutput",       environVarOrDefault "deployoutput"          (sprintf "%s\packages" (Path.GetFullPath(".")))
        "packaging:outputsubdirs",      environVarOrDefault "outputsubdirs"         "false"
        "packaging:updateid",           environVarOrDefault "updateid"              ""
        "packaging:pushto",             environVarOrDefault "pushto"                ""
        "packaging:pushdir",            environVarOrDefault "pushdir"               ""
        "packaging:pushurl",            environVarOrDefault "pushurl"               ""
        "packaging:apikey",             environVarOrDefault "apikey"                ""
        "packaging:deploypushto",       environVarOrDefault "deploypushto"          ""
        "packaging:deploypushdir",      environVarOrDefault "deploypushdir"         ""
        "packaging:deploypushurl",      environVarOrDefault "deploypushurl"         ""
        "packaging:deployapikey",       environVarOrDefault "deployapikey"          ""
        "packaging:packages",           environVarOrDefault "packages"              ""
        "packaging:assemblyinfopath",   environVarOrDefault "assemblyinfo"          ""
        "packaging:nuspecpath",         environVarOrDefault "nuspec"                ""
        "packaging:nugetconfig",        environVarOrDefault "nugetconfig"           ""
        "repo:path",                    environVarOrDefault "repo"                  ""    
        "test:path",                    environVarOrDefault "tests"                 ""
        "versioning:build",             environVarOrDefault "build_number"          "0"
        "versioning:branch",            match environVar "teamcity_build_branch" with
                                            | "<default>" -> environVar "vcsroot_branch"
                                            | _ -> environVar "teamcity_build_branch"
        "vs:version",                   environVarOrDefault "vs_version"            "11.0"
    ]
    |> List.iter (fun (k,v) -> dict.Add(k,v))
    dict


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
        val PathToNugetConfig: string
        val PathToAddinsNodesFromConfigXML: string
        val PathToAddinsConfigXML: string

        new(sol: string, nusp: string, nusproot: string, assem: string, assemroot: string, //gitrepo: string, 
            ?repo: string, ?dll: string, ?test: string, ?tool: string, ?pack: string, ?nugetconf: string, ?addins: string, ?addinsxml: string) = {           
            PathToSolution = sol
            PathToNuspec = nusp
            PathToNuspecFromRoot = nusproot
            PathToAssembleyInfo = assem
            PathToAssembleyInfoFromRoot = assemroot           

            PathToRepository =
                match repo with
                | Some x -> x
                | None -> @".."

            PathToDll = 
                match dll with
                | Some x -> x
                | None -> @"..\Bin\Release\v40"

            PathToTests = 
                match test with
                | Some x -> x
                | None -> @"..\Bin\Release\v40\*.Test.dll"

            PathToTools =
                match tool with
                | Some x -> x
                | None -> @"..\tools\Build.Tools"

            PathToPackages =
                match pack with
                | Some x -> x
                | None -> @"..\src\packages"

            PathToNugetConfig = 
                match nugetconf with
                | Some x -> x
                | None -> @"..\src\NuGet.config"

            PathToAddinsNodesFromConfigXML = 
                match addins with
                | Some x -> x
                | None -> @"../../../../../Bin/Release/v40/"

            PathToAddinsConfigXML = 
                match addinsxml with
                | Some x -> x
                | None -> @"..\src\packages\YaccConstructor.*\**\*.addins"    
        }
    end

let pushURL = @"https://www.myget.org/F/yc/api/v2/package"
let pushApiKey = @"f6ba9139-9d42-4cf1-acaf-344f963ff807"

let commitMessage = @"Change version of package in AssemblyInfo and Nuspec files"

let commonConfig (tools : SpecificConfig) =
    let gitCommandToCommit = sprintf "commit -m \"%s\" \"%s\" \"%s\"" commitMessage tools.PathToAssembleyInfoFromRoot tools.PathToNuspecFromRoot
    let gitCommandToPush = sprintf "push --repo https://\"%s\":\"%s\"@\"%s\"" config.["git:user"] config.["git:password"] config.["git:reftorepo"]

    config.["bin:path"] <- tools.PathToDll
    config.["build:solution"] <- tools.PathToSolution
    config.["core:tools"] <- tools.PathToTools
    config.["monoaddins:pathtonodes"] <- tools.PathToAddinsNodesFromConfigXML
    config.["monoaddins:pathtoconfigxml"] <- tools.PathToAddinsConfigXML
    config.["packaging:nuspecpath"] <- tools.PathToNuspec
    config.["packaging:packages"] <- tools.PathToPackages
    config.["packaging:assemblyinfopath"] <- tools.PathToAssembleyInfo
    config.["packaging:nugetconfig"] <- tools.PathToNugetConfig
    config.["packaging:deploypushurl"] <- pushURL
    config.["packaging:deployapikey"] <- pushApiKey
    config.["repo:path"] <- tools.PathToRepository
    config.["test:path"] <- tools.PathToTests

    Target "Versioning:Update" (fun x ->
        Versioning.updateDeploy (mapOfDict config) x
        Versioning.update (mapOfDict config) x
    )
    Target "Git:Commit" (fun _ ->
        gitCommand tools.PathToRepository gitCommandToCommit
    )
    Target "Git:Push" (fun _ ->
        gitCommand tools.PathToRepository gitCommandToPush
    )

    Target "Default"            <| DoNothing
    Target "Packaging:Package"  <| Packaging.packageDeploy (mapOfDict config)
    Target "Packaging:Restore"  <| Packaging.restore (mapOfDict config)
    Target "Packaging:Update"   <| Packaging.update (mapOfDict config)
    Target "Packaging:Push"     <| Packaging.pushDeploy (mapOfDict config)
    Target "Solution:Build"     <| Solution.build (mapOfDict config)
    Target "Solution:Clean"     <| Solution.clean (mapOfDict config)
    Target "Test:Run"           <| Test.run (mapOfDict config)
    Target "Mono.Addins:Xml"    <| Utils.correctPathToAddins (mapOfDict config)
    //Target "SpecFlow:Run"      <| Specflow.run (mapOfDict config)