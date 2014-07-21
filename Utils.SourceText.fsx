#r "Fake/FakeLib.dll"
#load "Core.fsx"

open Fake
open Core

let pathToSolution = @"..\src\YC.Utils.SourceText.sln"
let pathToTests = @"..\Bin\Release\v40\Utils.SourceText.Tests.dll"
let pathToTools = @"..\tools\Build.Tools"
let pathToNuspec = @"..\src\YC.Utils.SourceText\Utils.SourceText.nuspec"
let pathToAssembleyInfo = @"..\src\YC.Utils.SourceText\AssemblyInfo.fs"

config.["build:solution"] <- pathToSolution
config.["core:tools"] <- pathToTools
config.["test:path"] <- pathToTests
config.["packaging:nuspecpath"] <- pathToNuspec
config.["packaging:assemblyinfopath"] <- pathToAssembleyInfo

Target "Version" (fun x ->
    Versioning.update (mapOfDict config) x
    Versioning.updateDeploy (mapOfDict config) x
)
Target "Clean"      <| Solution.clean (mapOfDict config)
Target "Build"      <| Solution.build (mapOfDict config)
Target "TestRun"    <| Test.run (mapOfDict config)
Target "Package"    <| Packaging.packageDeploy (mapOfDict config)
Target "Def"        <| DoNothing

"Clean"
    ==> "Build"
    ==> "TestRun"
    ==> "Version"
    ==> "Package" 
    ==> "Def"

RunParameterTargetOrDefault "target" "Def"