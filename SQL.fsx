#r @"Fake/FakeLib.dll"
#load "Utils.fsx"
#load "Core.fsx"

open Fake
open Fake.Git
open Core
open Utils

let pathToSolution = @"..\src\SQLParsers.sln"

let pathToPLSqlGen = @"..\src\PLSql\gen.cmd"
let pathToWorkingDirPLSqlGen = @"..\src\PLSql"
let argsForPLSqlGen = @""

let pathToTSqlGen = @"..\src\TSQL\gen.cmd"
let pathToWorkingDirTSqlGen = @"..\src\TSQL"
let argsForTSqlGen = @""

let pathToATSqlGen = @"..\src\AbstractTSqlReSharper\gen.cmd"
let pathToWorkingDirATSqlGen = @"..\src\AbstractTSqlReSharper"
let argsForATSqlGen = @""


let pathToNuspec = @""
let pathToNuspecFromRoot = @""
let pathToAssembleyInfo = @""
let pathToAssembleyInfoFromRoot = @""


let specConfig = new SpecificConfig(pathToSolution, pathToNuspec, pathToNuspecFromRoot, pathToAssembleyInfo, pathToAssembleyInfoFromRoot)
commonConfig specConfig

Target "Parsers:Gen" (fun _ ->
    runCmd pathToPLSqlGen pathToWorkingDirPLSqlGen argsForPLSqlGen
    runCmd pathToTSqlGen pathToWorkingDirTSqlGen argsForTSqlGen
    runCmd pathToATSqlGen pathToWorkingDirATSqlGen argsForATSqlGen
)

Target "Start" <| DoNothing

"Packaging:Restore"
    ==> "Solution:Clean"
    ==> "Parsers:Gen"
    ==> "Solution:Build"
    ==> "Test:Run"
    ==> "Default"

RunParameterTargetOrDefault "target" "Default"