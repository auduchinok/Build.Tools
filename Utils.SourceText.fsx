#r "Fake/FakeLib.dll"
#load "Core.fsx"

open Fake
open Core

let pathToSolution = @"..\src\YC.Utils.SourceText.sln"
let pathToTests = @"..\Bin\Release\v40\Utils.SourceText.Tests.dll"
let pathToTools = @"..\tools\Build.Tools"

let conf = config
conf.["build:solution"] <- pathToSolution
conf.["core:tools"] <- pathToTools
conf.["test:path"] <- pathToTests

Target "Clean"      <| Solution.clean (mapOfDict config)
Target "Build"      <| Solution.build (mapOfDict config)
Target "TestRun"    <| Test.run (mapOfDict config)
Target "Def"        <| DoNothing

"Clean"
    ==> "Build"
    ==> "TestRun"
    ==> "Def"

RunParameterTargetOrDefault "target" "Def"