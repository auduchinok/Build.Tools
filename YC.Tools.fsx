#r @"Fake/FakeLib.dll"
#load "Core.fsx"

open Fake
open Fake.Git
open Core
open System.IO

let pathToRepository = @".."
let pathToDll = @"..\Bin\Release\v40"
let pathToSolution = @"..\src\YC.Tools.sln"
let pathToTests = @"..\Bin\Release\v40\*.Test.dll"
let pathToTools = @"..\tools\Build.Tools"
let pathToPackages = @"..\src\packages"
let pathToNuspec = @"..\src\YC.FsYacc\YC.Tools.nuspec"
let pathToNuspecFromRoot = @"src\YC.FsYacc\YC.Tools.nuspec"
let pathToAssembleyInfo = @"..\src\YC.FsYacc\AssemblyInfo.fs"
let pathToAssembleyInfoFromRoot = @"src\YC.FsYacc\AssemblyInfo.fs"

let specConfig = new SpecificConfig(pathToRepository, pathToDll, pathToSolution, pathToTests, pathToTools, pathToPackages, pathToNuspec, pathToNuspecFromRoot, pathToAssembleyInfo, pathToAssembleyInfoFromRoot)
commonConfig specConfig

"Packaging:Restore"
    ==> "Solution:Clean"
    ==> "Solution:Build"
    //==> "Test:Run"
    ==> "Versioning:Update"
    ==> "Packaging:Package"
    =?> ("Packaging:Push", not isLocalBuild)
    =?> ("Git:Commit", not isLocalBuild)
    =?> ("Git:Push", not isLocalBuild)
    ==> "Default"

RunParameterTargetOrDefault "target" "Default"