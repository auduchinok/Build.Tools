#r @"Fake/FakeLib.dll"
#load "Core.fsx"

open Fake
open Fake.Git
open Core
open System.IO

let pathToRepository = @".."
let pathToDll = @"..\Bin\Release\v40"
let pathToSolution = @"..\src\YC.Core.sln"
let pathToTests = @"..\Bin\Release\v40\*.Test.dll"
let pathToTools = @"..\tools\Build.Tools"
let pathToPackages = @"..\src\packages"
let pathToNuspec = @"..\src\YaccConstructor\YC.Core.nuspec"
let pathToNuspecFromRoot = @"src\YaccConstructor\YC.Core.nuspec"
let pathToAssembleyInfo = @"..\src\YaccConstructor\AssemblyInfo.fs"
let pathToAssembleyInfoFromRoot = @"src\YaccConstructor\AssemblyInfo.fs"
let gitUserName = "yc.TeamCity"
let gitPassword = "my9UX2ka7XB3"
let gitRepo = "code.google.com/p/recursive-ascent/"

let specConfig = new SpecificConfig(pathToRepository, pathToDll, pathToSolution, pathToTests, pathToTools, pathToPackages, pathToNuspec, pathToNuspecFromRoot, pathToAssembleyInfo, pathToAssembleyInfoFromRoot, gitUserName, gitPassword, gitRepo)
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