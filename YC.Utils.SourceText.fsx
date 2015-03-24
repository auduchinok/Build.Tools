#r @"Fake/FakeLib.dll"
#load "Core.fsx"

open Fake
open Fake.Git
open Core
open System.IO

let pathToSolution = @"..\src\YC.Utils.SourceText.sln"
let pathToNuspec = @"..\src\Utils.SourceText\Utils.SourceText.nuspec"
let pathToNuspecFromRoot = @"src\Utils.SourceText\Utils.SourceText.nuspec"
let pathToAssembleyInfo = @"..\src\Utils.SourceText\AssemblyInfo.fs"
let pathToAssembleyInfoFromRoot = @"src\Utils.SourceText\AssemblyInfo.fs"
let pathToNugetConfig = ""

let specConfig = new SpecificConfig(pathToSolution, pathToNuspec, pathToNuspecFromRoot, pathToAssembleyInfo, pathToAssembleyInfoFromRoot, nugetconf = pathToNugetConfig)
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