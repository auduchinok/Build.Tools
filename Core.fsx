#r    "./fake/fakelib.dll"
#load "./Utils.fsx"
#load "./Packaging.fsx"
#load "./Versioning.fsx"
#load "./Solution.fsx"
#load "./Test.fsx"
#load "./Specflow.fsx"

open System.IO
open System.Collections.Generic
open Fake

let config = 
    let dict = new Dictionary<_,_>()
    [
        "build:configuration",          environVarOrDefault "configuration"         "Release"
        "build:solution",               environVar          "solution"
        "core:tools",                   environVar          "tools"
        "packaging:output",             environVarOrDefault "output"                (sprintf "%s\output" (Path.GetFullPath(".")))
        "packaging:deployoutput",       environVarOrDefault "deployoutput"          (sprintf "%s\deploy" (Path.GetFullPath(".")))
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
        "test:path",                    environVarOrDefault "tests"                 ".\..\src\*\bin\Release\**\*.Tests.dll"
        "versioning:build",             environVarOrDefault "build_number"          "0"
        "versioning:branch",            match environVar "teamcity_build_branch" with
                                            | "<default>" -> environVar "vcsroot_branch"
                                            | _ -> environVar "teamcity_build_branch"
        "vs:version",                   environVarOrDefault "vs_version"            "11.0" 
    ]
    |> List.iter (fun (k,v) -> dict.Add(k,v))
    dict

let mapOfDict (dict : Dictionary<_,_>) =
    dict |> Seq.map (fun kvp -> kvp.Key, kvp.Value) |> Map.ofSeq  

// Target definitions
Target "Default"           <| DoNothing
Target "Packaging:Package" <| Packaging.package (mapOfDict config)
Target "Packaging:Restore" <| Packaging.restore (mapOfDict config)
Target "Packaging:Update"  <| Packaging.update (mapOfDict config)
Target "Packaging:Push"    <| Packaging.push (mapOfDict config)
Target "Solution:Build"    <| Solution.build (mapOfDict config)
Target "Solution:Clean"    <| Solution.clean (mapOfDict config)
Target "Versioning:Update" <| Versioning.update (mapOfDict config)
Target "Test:Run"          <| Test.run (mapOfDict config)
Target "SpecFlow:Run"      <| Specflow.run (mapOfDict config)

// Build order
"Packaging:Restore"
    ==> "Solution:Clean"
    ==> "Versioning:Update"
    ==> "Solution:Build"
    ==> "Packaging:Package"
    ==> "SpecFlow:Run"
    ==> "Test:Run"
    =?> ("Packaging:Push", not isLocalBuild)
    ==> "Default"

//RunParameterTargetOrDefault "target" "Default"