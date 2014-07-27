#r @"Fake/FakeLib.dll"
#load "Packaging.fsx"

open Fake 


let pathToFakeExe = "../tools/Build.Tools/Fake/FAKE.exe"

Target "Default" <| DoNothing

"BuildWithoutTests"
    ==> "Default"

RunParameterTargetOrDefault "target" "Default"