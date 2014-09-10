#r @"Fake/FakeLib.dll"
#load "Core.fsx"

open Fake
open Fake.Git
open Core
open System.IO

let pathToSolution = @"..\src\YC.Tools.sln"
let pathToNuspec = @"..\src\FsYacc\YC.Tools.nuspec"
let pathToNuspecFromRoot = @"src\FsYacc\YC.Tools.nuspec"
let pathToAssembleyInfo = @"..\src\FsYacc\AssemblyInfo.fs"
let pathToAssembleyInfoFromRoot = @"src\FsYacc\AssemblyInfo.fs"
let gitRepo = "code.google.com/p/recursive-ascent/"

let specConfig = new SpecificConfig(pathToSolution, pathToNuspec, pathToNuspecFromRoot, pathToAssembleyInfo, pathToAssembleyInfoFromRoot, gitRepo)
commonConfig specConfig

"Packaging:Restore"
    ==> "Solution:Clean"
    ==> "Solution:Build"
    ==> "Test:Run"
    ==> "Versioning:Update"
    ==> "Packaging:Package"
    =?> ("Packaging:Push", not isLocalBuild)
    =?> ("Git:Commit", not isLocalBuild)
    =?> ("Git:Push", not isLocalBuild)
    ==> "Default"

RunParameterTargetOrDefault "target" "Default"