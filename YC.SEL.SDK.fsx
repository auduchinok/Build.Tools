#r @"Fake/FakeLib.dll"
#load "Utils.fsx"
#load "Core.fsx"

open Fake
open Fake.Git
open Core
open Utils

let pathToYardFrontendGen = @"..\src\YardFrontend\gen.cmd"
let pathToWorkingDirForYardFrontendGen = @"..\src\YardFrontend"
let argsForYardFrontendGen = @""

let pathToMinimalSolution = @"..\src\YC.SEL.SDK.Minimal.sln"

let pathToRNGLRAbstractParserTestGenLex = @"..\src\RNGLRAbstractParser.Test\gen_lex.cmd"
let pathToWorkingDirForRNGLRAbstractParserTestGenLex = @"..\src\YardFrontend"
let argsForRNGLRAbstractParserTestGenLex = @"40"

let pathToRNGLRAbstractParserTestGen = @"..\src\RNGLRAbstractParser.Test\gen.cmd"
let pathToWorkingDirForRNGLRAbstractParserTestGen = @"..\src\YardFrontend"
let argsForRNGLRAbstractParserTestGen = @""

let pathToRNGLRParserErrorRecoveryTestGen = @"..\src\RNGLRParser.ErrorRecoveryTest\gen.cmd"
let pathToWorkingDirForRNGLRParserErrorRecoveryTestGen = @"..\src\RNGLRParser.ErrorRecoveryTest"
let argsForRNGLRParserErrorRecoveryTestGen = @""

let pathToRNGLRParserSimpleTestGen = @"..\src\RNGLRParser.SimpleTest\gen.cmd"
let pathToWorkingDirForRNGLRParserSimpleTestGen = @"..\src\RNGLRParser.SimpleTest"
let argsForRNGLRParserSimpleTestGen = @""

let pathToCalcHighLightingGen = @"..\src\Calc\gen_highlighting.cmd"
let pathToWorkingDirForCalcHighLightingGen = @"..\src\Calc"
let argsForCalcHighLightingGen = @""

let pathToJSONHighLightingGen = @"..\src\JSON_Parser\gen_highlighting.cmd"
let pathToWorkingDirForJSONHighLightingGen = @"..\src\JSON_Parser"
let argsForJSONHighLightingGen = @""

let pathToTSQLHighLightingGen = @"..\src\TSQL\gen_highlighting.cmd"
let pathToWorkingDirForTSQLHighLightingGen = @"..\src\TSQL"
let argsForTSQLHighLightingGen = @""


Target "YardFrontend:Gen" (fun _ ->
    runCmd pathToYardFrontendGen pathToWorkingDirForYardFrontendGen argsForYardFrontendGen
)
Target "Solution:BuildMinimal" <| Solution.buildSpec (mapOfDict config) pathToMinimalSolution
Target "Solution:CleanMinimal" <| Solution.cleanSpec (mapOfDict config) pathToMinimalSolution
Target "RNGLR:Test" (fun _ ->
    runCmd pathToRNGLRAbstractParserTestGenLex pathToWorkingDirForRNGLRAbstractParserTestGenLex argsForRNGLRAbstractParserTestGenLex
    runCmd pathToRNGLRAbstractParserTestGen pathToWorkingDirForRNGLRAbstractParserTestGen argsForRNGLRAbstractParserTestGen
    runCmd pathToRNGLRParserErrorRecoveryTestGen pathToWorkingDirForRNGLRParserErrorRecoveryTestGen argsForRNGLRParserErrorRecoveryTestGen
    runCmd pathToRNGLRParserSimpleTestGen pathToWorkingDirForRNGLRParserSimpleTestGen argsForRNGLRParserSimpleTestGen
)
Target "HighLighting:Run" (fun _ ->
    runCmd pathToCalcHighLightingGen pathToWorkingDirForCalcHighLightingGen argsForCalcHighLightingGen
    runCmd pathToJSONHighLightingGen pathToWorkingDirForJSONHighLightingGen argsForJSONHighLightingGen
    runCmd pathToTSQLHighLightingGen pathToWorkingDirForTSQLHighLightingGen argsForTSQLHighLightingGen   
)

Target "Start" <| DoNothing


let pathToSolution = @"..\src\YC.SEL.SDK.sln"
let pathToNuspec = @"..\src\FsYacc\YC.Tools.nuspec"
let pathToNuspecFromRoot = @"src\FsYacc\YC.Tools.nuspec"
let pathToAssembleyInfo = @"..\src\FsYacc\AssemblyInfo.fs"
let pathToAssembleyInfoFromRoot = @"src\FsYacc\AssemblyInfo.fs"
let gitUserName = "yc.TeamCity"
let gitPassword = "my9UX2ka7XB3"
let gitRepo = "code.google.com/p/recursive-ascent/"

let specConfig = new SpecificConfig(pathToMinimalSolution, pathToNuspec, pathToNuspecFromRoot, pathToAssembleyInfo, pathToAssembleyInfoFromRoot, gitUserName, gitPassword, gitRepo)
commonConfig specConfig

"Start"
//"Packaging:Restore"
    //==> "YardFrontend:Gen"
    //==> "Mono.Addins:Xml"
    //==> "Solution:CleanMinimal"
    ==> "Solution:BuildMinimal"
    //==> "RNGLR:Test"
    //==> "HighLighting:Run"
    //==> "Solution:Clean"
    //==> "Solution:Build"
    //==> "Test:Run"
    //==> "Versioning:Update"
    //==> "Packaging:Package"
    //=?> ("Packaging:Push", not isLocalBuild)
    //=?> ("Git:Commit", not isLocalBuild)
    //=?> ("Git:Push", not isLocalBuild)
    ==> "Default"

RunParameterTargetOrDefault "target" "Default"