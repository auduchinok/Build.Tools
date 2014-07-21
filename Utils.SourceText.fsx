#r "Fake/FakeLib.dll"
#load "Core.fsx"

open Fake
open Core

let pathToSolution = @"..\src\YC.Utils.SourceText.sln"
let pathToTests = @"..\Bin\Release\v40\Utils.SourceText.Tests.dll"
let pathToTools = @"..\tools\Build.Tools"
let pathToNuspec = @"..\src\YC.Utils.SourceText\Utils.SourceText.fsproj"

config.["build:solution"] <- pathToSolution
config.["core:tools"] <- pathToTools
config.["test:path"] <- pathToTests
config.["packaging:nuspecpath"] <- pathToNuspec

Target "Clean"      <| Solution.clean (mapOfDict config)
Target "Build"      <| Solution.build (mapOfDict config)
Target "TestRun"    <| Test.run (mapOfDict config)
Target "Package"    <| Packaging.package (mapOfDict config)
Target "Def"        <| DoNothing

//"Clean"
    //==> "Build"
    //==> "TestRun"
    //==> "Package"
"Package"
    ==> "Def"

RunParameterTargetOrDefault "target" "Def"