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

let pathToMinimalSolution = @"..\src\YC.Minimal.sln"

let pathToYardFrontendSolution = @"..\src\YC.YardFrontend.sln"

//let pathToRNGLRAbstractParserTestGenLex = @"..\src\RNGLRAbstractParser.Test\gen_lex.cmd"
let pathToWorkingDirForRNGLRAbstractParserTestGenLex = @"..\src\RNGLRAbstractParser.Test"
let argsForRNGLRAbstractParserTestGenLex = @"40"

let pathToRNGLRAbstractParserTestGen = @"..\src\RNGLRAbstractParser.Test\gen.cmd"
let pathToWorkingDirForRNGLRAbstractParserTestGen = @"..\src\RNGLRAbstractParser.Test"
let argsForRNGLRAbstractParserTestGen = @""

let pathToRNGLRParserErrorRecoveryTestGen = @"..\src\RNGLRParser.ErrorRecoveryTest\gen.cmd"
let pathToWorkingDirForRNGLRParserErrorRecoveryTestGen = @"..\src\RNGLRParser.ErrorRecoveryTest"
let argsForRNGLRParserErrorRecoveryTestGen = @""

let pathToRNGLRParserSimpleTestGen = @"..\src\RNGLRParser.SimpleTest\gen.cmd"
let pathToWorkingDirForRNGLRParserSimpleTestGen = @"..\src\RNGLRParser.SimpleTest"
let argsForRNGLRParserSimpleTestGen = @""

let pathToGLLParserSimpleTestGen = @"..\src\GLLParser.SimpleTest\gen.cmd "
let pathToWorkingDirForGLLParserSimpleTestGen = @"..\src\GLLParser.SimpleTest"
let argsForGLLParserSimpleTestGen = @""

let pathToOtherSPPFTestGen = @"..\src\RNGLR.OtherSppfTest\gen.cmd"
let pathToWorkingDirForOtherSPPFTestGen = @"..\src\RNGLR.OtherSppfTest"
let argsForOtherSPPFTestGen = @""

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
Target "RNGLR:GenTest" (fun _ ->
    //runCmd pathToRNGLRAbstractParserTestGenLex pathToWorkingDirForRNGLRAbstractParserTestGenLex argsForRNGLRAbstractParserTestGenLex
    runCmd pathToRNGLRAbstractParserTestGen pathToWorkingDirForRNGLRAbstractParserTestGen argsForRNGLRAbstractParserTestGen
    runCmd pathToRNGLRParserErrorRecoveryTestGen pathToWorkingDirForRNGLRParserErrorRecoveryTestGen argsForRNGLRParserErrorRecoveryTestGen
    runCmd pathToRNGLRParserSimpleTestGen pathToWorkingDirForRNGLRParserSimpleTestGen argsForRNGLRParserSimpleTestGen
    runCmd pathToOtherSPPFTestGen pathToWorkingDirForOtherSPPFTestGen argsForOtherSPPFTestGen
)

Target "GLL:GenTest" (fun _ ->
    runCmd pathToGLLParserSimpleTestGen pathToWorkingDirForGLLParserSimpleTestGen argsForGLLParserSimpleTestGen
)

Target "HighLighting:GenTest" (fun _ ->
    runCmd pathToCalcHighLightingGen pathToWorkingDirForCalcHighLightingGen argsForCalcHighLightingGen
    runCmd pathToJSONHighLightingGen pathToWorkingDirForJSONHighLightingGen argsForJSONHighLightingGen
    runCmd pathToTSQLHighLightingGen pathToWorkingDirForTSQLHighLightingGen argsForTSQLHighLightingGen   
)

Target "Solution:BuildYardFrontend" <| Solution.buildSpec (mapOfDict config) pathToYardFrontendSolution
Target "Solution:CleanYardFrontend" <| Solution.cleanSpec (mapOfDict config) pathToYardFrontendSolution


Target "Start" <| DoNothing

let pathToSolution = @"..\src\YC.SDK.sln"
let pathToNuspec = @"..\src\FsYacc\YC.Tools.nuspec"
let pathToNuspecFromRoot = @"src\FsYacc\YC.Tools.nuspec"
let pathToAssembleyInfo = @"..\src\FsYacc\AssemblyInfo.fs"
let pathToAssembleyInfoFromRoot = @"src\FsYacc\AssemblyInfo.fs"
let gitRepo = "code.google.com/p/recursive-ascent/"

let specConfig = new SpecificConfig(pathToSolution, pathToNuspec, pathToNuspecFromRoot, pathToAssembleyInfo, pathToAssembleyInfoFromRoot, gitRepo)
commonConfig specConfig

"Packaging:Restore"
    ==> "Solution:CleanMinimal"
    ==> "Solution:CleanYardFrontend"
    ==> "Solution:BuildMinimal"
    ==> "YardFrontend:Gen"
    ==> "Solution:BuildYardFrontend"
    ==> "RNGLR:GenTest"
    ==> "GLL:GenTest"
    ==> "HighLighting:GenTest"
    ==> "Solution:Clean"
    ==> "Solution:Build"
    ==> "Test:Run"
    ==> "Mono.Addins:Xml"
    //==> "Versioning:Update"
    //==> "Packaging:Package"
    //=?> ("Packaging:Push", not isLocalBuild)
    //=?> ("Git:Commit", not isLocalBuild)
    //=?> ("Git:Push", not isLocalBuild)
    ==> "Default"

RunParameterTargetOrDefault "target" "Default"