#r "Fake/FakeLib.dll"
#load "Core.fsx"

open Fake
open Fake.Git
open Core
open System.IO

let pathToBuildScripts = @"."
let pathToRepesitory = @".."
let pathToSolution = @"..\src\YC.Utils.SourceText.sln"
let pathToTests = @"..\Bin\Release\v40\Utils.SourceText.Tests.dll"
let pathToTools = @"..\tools\Build.Tools"
let pathToNuspec = @"..\src\YC.Utils.SourceText\Utils.SourceText.nuspec"
let pathToAssembleyInfo = @"..\src\YC.Utils.SourceText\AssemblyInfo.fs"
let pushURL = @"https://www.myget.org/F/yc/api/v2/package"
let pushApiKey = @"f6ba9139-9d42-4cf1-acaf-344f963ff807"

let commitMessage = @"Change version of package in AssemblyInfo and Nuspec files (beta)"
let gitCommandToCommit = sprintf "commit -m \"%s\" \"%s\" \"%s\"" commitMessage pathToAssembleyInfo pathToNuspec
let gitUsername = "YcGeneralUser"
let gitPassword = "yc2GeneralUser2014"
let gitCommandToPush = sprintf "push --repo https://\"%s\":\"%s\"@github.com/YaccConstructor/YC.Utils.SourceText.git" gitUsername gitPassword

config.["build:solution"] <- pathToSolution
config.["core:tools"] <- pathToTools
config.["test:path"] <- pathToTests
config.["packaging:nuspecpath"] <- pathToNuspec
config.["packaging:assemblyinfopath"] <- pathToAssembleyInfo
config.["packaging:deploypushurl"] <- pushURL
config.["packaging:deployapikey"] <- pushApiKey

Target "Version" (fun x ->
    Versioning.update (mapOfDict config) x
    Versioning.updateDeploy (mapOfDict config) x
)
Target "Commit" (fun _ ->
    gitCommand pathToBuildScripts gitCommandToCommit
)
Target "PushChanges" (fun _ ->
    gitCommand pathToRepesitory gitCommandToPush
)
Target "Clean"          <| Solution.clean (mapOfDict config)
Target "Build"          <| Solution.build (mapOfDict config)
Target "TestRun"        <| Test.run (mapOfDict config)
Target "Package"        <| Packaging.packageDeploy (mapOfDict config)
Target "PushPackage"    <| Packaging.pushDeploy (mapOfDict config)
Target "Def"            <| DoNothing

"Clean"
    ==> "Build"
    //==> "TestRun"
    //==> "Version"
    //==> "Package"
    //==> "PushPackage"
    ==> "Commit"
    ==> "PushChanges"
    ==> "Def"

RunParameterTargetOrDefault "target" "Def"