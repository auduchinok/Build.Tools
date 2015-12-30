#r @"Fake/FakeLib.dll"
#load "Utils.fsx"
#load "Core.fsx"

open Fake
open Fake.Git
open Core
open Utils

let pathToCoreSolution = @"..\src\YC.Core.sln"

let pathToYardFrontendGen = @"..\src\YardFrontend\gen.cmd"
let pathToWorkingDirForYardFrontendGen = @"..\src\YardFrontend"
let argsForYardFrontendGen = @""

let pathToAntlrFrontendGen = @"..\src\AntlrFrontend\gen.cmd"
let pathToWorkingDirForAntlrFrontendGen = @"..\src\AntlrFrontend"
let argsForAntlrFrontendGen = @""

let pathToFsYaccFrontendGen = @"..\src\FsYaccFrontend\gen.cmd"
let pathToWorkingDirForFsYaccFrontendGen = @"..\src\FsYaccFrontend"
let argsForFsYaccFrontendGen = @""

let pathToMinimalSolution = @"..\src\YC.Minimal.sln"

let pathToYardFrontendSolution = @"..\src\YC.YardFrontend.sln"

let pathToRNGLRAbstractParserTestGenLex = @"..\src\RNGLRAbstractParser.Test\gen_lex.cmd"
let pathToWorkingDirForRNGLRAbstractParserTestGenLex = @"..\src\RNGLRAbstractParser.Test"
let argsForRNGLRAbstractParserTestGenLex = @""

let pathToRNGLRAbstractParserTestGen = @"..\src\RNGLRAbstractParser.Test\gen.cmd"
let pathToWorkingDirForRNGLRAbstractParserTestGen = @"..\src\RNGLRAbstractParser.Test"
let argsForRNGLRAbstractParserTestGen = @""

let pathToRNGLRApplication = @"..\src\RNGLRApplication\gen.cmd"
let pathToWorkingDirForRNGLRApplication = @"..\src\RNGLRApplication"
let argsForRNGLRApplication = @""

let pathToRNGLRParserErrorRecoveryTestGen = @"..\src\RNGLRParser.ErrorRecoveryTest\gen.cmd"
let pathToWorkingDirForRNGLRParserErrorRecoveryTestGen = @"..\src\RNGLRParser.ErrorRecoveryTest"
let argsForRNGLRParserErrorRecoveryTestGen = @""

let pathToRNGLRParserSimpleTestGen = @"..\src\RNGLRParser.SimpleTest\gen.cmd"
let pathToWorkingDirForRNGLRParserSimpleTestGen = @"..\src\RNGLRParser.SimpleTest"
let argsForRNGLRParserSimpleTestGen = @""

let pathToGLLParserSimpleTestGen = @"..\src\GLLParser.SimpleTest\gen.cmd "
let pathToWorkingDirForGLLParserSimpleTestGen = @"..\src\GLLParser.SimpleTest"
let argsForGLLParserSimpleTestGen = @""

let pathToGLLParserAbstrTestGen = @"..\src\GLL.AbstractParser.SimpleTest\gen.cmd "
let pathToWorkingDirForGLLParserAbstrTestGen = @"..\src\GLL.AbstractParser.SimpleTest"
let argsForGLLParserAbstrTestGen = @""

let pathToGLLParserAppAbstrGen = @"..\src\GLLAbstractApplication\gen.cmd"
let pathToWorkingDirForGLLParserAppAbstrGen = @"..\src\GLLAbstractApplication"
let argsForGLLParserAppAbstrGen = @""

let pathToGLLParserAppGen = @"..\src\GLLApplication\gen.cmd"
let pathToWorkingDirForGLLParserAppGen = @"..\src\GLLApplication"
let argsForGLLParserAppGen = @""

let pathToASTTestGen = @"..\src\AST.Test\gen.cmd"
let pathToWorkingDirForASTTestGen = @"..\src\AST.Test"
let argsForASTTestGen = @""

let pathToOtherSPPFTestGen = @"..\src\OtherSPPF.Test\gen.cmd"
let pathToWorkingDirForOtherSPPFTestGen = @"..\src\OtherSPPF.Test"
let argsForOtherSPPFTestGen = @""

let pathToCfgTestGen = @"..\src\ControlFlowGraph.Test\gen.cmd"
let pathToWorkingDirCfgTestGen = @"..\src\ControlFlowGraph.Test"
let argsForCfgTestGen = @""

let pathToCalcHighLightingGen = @"..\src\Calc\gen_highlighting.cmd"
let pathToWorkingDirForCalcHighLightingGen = @"..\src\Calc"
let argsForCalcHighLightingGen = @""

let pathToExtCalcHighLightingGen = @"..\src\ExtCalc\gen_highlighting.cmd"
let pathToWorkingDirForExtCalcHighLightingGen = @"..\src\ExtCalc"
let argsForExtCalcHighLightingGen = @""

let pathToJSONHighLightingGen = @"..\src\JSON_Parser\gen_highlighting.cmd"
let pathToWorkingDirForJSONHighLightingGen = @"..\src\JSON_Parser"
let argsForJSONHighLightingGen = @""

//let pathToTSQLHighLightingGen = @"..\src\TSQL\gen_highlighting.cmd"
let pathToTSQLAbstractHighLightingGen = @"..\src\TSQL\gen_abstract_highlighting.cmd"
let pathToTSQLNonAbstractGen = @"..\src\TSQL\gen_rnglr_non_abstract.cmd"
let pathToWorkingDirForTSQLGen = @"..\src\TSQL"
let argsForTSQLAbstractHighLightingGen = @""
let argsForTSQLNonAbstractGen = @""

let packagesConfigDirForSubmodules = [(@"..\FST", @"..\FST\FST\packages"); (@"..\facio", @"..\facio\packages")]
//let packagesConfigDirForSubmodule = @"..\FST"  
//let outputPackageDirForSubmodule = @"..\FST\FST\packages"

let pathToVersionFileFromRoot = @"VERSION"

let pathToSolution = @"..\src\YC.SDK.sln"
let pathToNuspec = @"..\src\FsYacc\YC.Tools.nuspec"
let pathToNuspecFromRoot = @"src\FsYacc\YC.Tools.nuspec"
let pathToAssembleyInfo = @"..\src\FsYacc\AssemblyInfo.fs"
let pathToAssembleyInfoFromRoot = @"src\FsYacc\AssemblyInfo.fs"

let specConfig = new SpecificConfig(pathToSolution, pathToNuspec, pathToNuspecFromRoot, pathToAssembleyInfo, pathToAssembleyInfoFromRoot, versfile  = pathToVersionFileFromRoot)
commonConfig specConfig


Target "Packaging:RestoreForSubmodule" <| (fun () ->
    List.iter (fun (dir, out) -> Packaging.restoreSpecOutput (mapOfDict config) dir out ()) packagesConfigDirForSubmodules 
)
//Target "Packaging:RestoreForSubmodule" <| Packaging.restoreSpecOutput (mapOfDict config) packagesConfigDirForSubmodule outputPackageDirForSubmodule

Target "YardFrontend:Gen" (fun _ ->
    runCmd pathToYardFrontendGen pathToWorkingDirForYardFrontendGen argsForYardFrontendGen
)
Target "AntlrFrontend:Gen" (fun _ ->
    runCmd pathToAntlrFrontendGen pathToWorkingDirForAntlrFrontendGen argsForAntlrFrontendGen
)
Target "FsYaccFrontend:Gen" (fun _ ->
    runCmd pathToFsYaccFrontendGen pathToWorkingDirForFsYaccFrontendGen argsForFsYaccFrontendGen
)
Target "Solution:BuildCore" <| Solution.buildSpec (mapOfDict config) pathToCoreSolution
Target "Solution:CleanCore" <| Solution.cleanSpec (mapOfDict config) pathToCoreSolution

Target "Solution:BuildMinimal" <| Solution.buildSpec (mapOfDict config) pathToMinimalSolution
Target "Solution:CleanMinimal" <| Solution.cleanSpec (mapOfDict config) pathToMinimalSolution
Target "RNGLR:GenTest" (fun _ ->
    runCmd pathToRNGLRAbstractParserTestGenLex pathToWorkingDirForRNGLRAbstractParserTestGenLex argsForRNGLRAbstractParserTestGenLex
    runCmd pathToRNGLRAbstractParserTestGen pathToWorkingDirForRNGLRAbstractParserTestGen argsForRNGLRAbstractParserTestGen
    runCmd pathToRNGLRParserErrorRecoveryTestGen pathToWorkingDirForRNGLRParserErrorRecoveryTestGen argsForRNGLRParserErrorRecoveryTestGen
    runCmd pathToRNGLRParserSimpleTestGen pathToWorkingDirForRNGLRParserSimpleTestGen argsForRNGLRParserSimpleTestGen
    runCmd pathToRNGLRApplication pathToWorkingDirForRNGLRApplication argsForRNGLRApplication
    
)

Target "ASTTest:Gen" (fun _ ->
    runCmd pathToASTTestGen pathToWorkingDirForASTTestGen argsForASTTestGen
    runCmd pathToOtherSPPFTestGen pathToWorkingDirForOtherSPPFTestGen argsForOtherSPPFTestGen
)

Target "ControlFlowGraph:GenTest" (fun _ -> 
    runCmd pathToCfgTestGen pathToWorkingDirCfgTestGen argsForCfgTestGen
)

Target "GLL:GenTest" (fun _ ->
    runCmd pathToGLLParserSimpleTestGen pathToWorkingDirForGLLParserSimpleTestGen argsForGLLParserSimpleTestGen
    runCmd pathToGLLParserAbstrTestGen pathToWorkingDirForGLLParserAbstrTestGen argsForGLLParserAbstrTestGen
    runCmd pathToGLLParserAppGen pathToWorkingDirForGLLParserAppGen argsForGLLParserAppGen
    runCmd pathToGLLParserAppAbstrGen pathToWorkingDirForGLLParserAppAbstrGen argsForGLLParserAppAbstrGen
)

Target "HighLighting:GenTest" (fun _ ->
    runCmd pathToCalcHighLightingGen pathToWorkingDirForCalcHighLightingGen argsForCalcHighLightingGen
    runCmd pathToExtCalcHighLightingGen pathToWorkingDirForExtCalcHighLightingGen argsForExtCalcHighLightingGen
    //runCmd pathToJSONHighLightingGen pathToWorkingDirForJSONHighLightingGen argsForJSONHighLightingGen
    runCmd pathToTSQLAbstractHighLightingGen pathToWorkingDirForTSQLGen argsForTSQLAbstractHighLightingGen   
)

Target "TSQLNonAbstract:GenTest" (fun _ ->
    runCmd pathToTSQLNonAbstractGen pathToWorkingDirForTSQLGen argsForTSQLNonAbstractGen   
)

Target "QuickGraph:InstallTools" (fun _ ->
    runCmd @"msiexec.exe" @"..\QuickGraph" """ /i "lib\Pex\pex.academic.x86.msi" /quiet /norestart"""
)


Target "QuickGraph:Build" (fun _ ->
    runCmd @"..\QuickGraph\build.cmd" @"..\QuickGraph" ""
)


Target "Solution:BuildYardFrontend" <| Solution.buildSpec (mapOfDict config) pathToYardFrontendSolution
Target "Solution:CleanYardFrontend" <| Solution.cleanSpec (mapOfDict config) pathToYardFrontendSolution

Target "Start" <| DoNothing


"Packaging:RestoreForSubmodule"
    //=?> ("QuickGraph:InstallTools", not isLocalBuild)
    ==> "QuickGraph:Build"
    ==> "Packaging:Restore"
    ==> "Versioning:UpdateAssemblyInfo"
    ==> "Solution:CleanCore"
    ==> "Solution:CleanMinimal"
    ==> "Solution:CleanYardFrontend"
    ==> "Solution:BuildCore"
    ==> "FsYaccFrontend:Gen"
    ==> "Solution:BuildMinimal"
    ==> "YardFrontend:Gen"
    ==> "AntlrFrontend:Gen"
    ==> "Solution:BuildYardFrontend"
    ==> "RNGLR:GenTest"
    ==> "GLL:GenTest"
    ==> "ASTTest:Gen"
    ==> "ControlFlowGraph:GenTest"
    ==> "HighLighting:GenTest"
    ==> "TSQLNonAbstract:GenTest"
    ==> "Solution:Clean"
    ==> "Solution:Build"
    ==> "Test:Run"
    ==> "Mono.Addins:Xml"
    ==> "Versioning:UpdateNuspecAndDll"
    ==> "Packaging:Package"
    =?> ("Packaging:Push", not isLocalBuild)
    =?> ("Versioning:IncrementCommonVersion", not isLocalBuild)
    =?> ("Git:Commit", not isLocalBuild)
    =?> ("Git:Push", not isLocalBuild)
    ==> "Default"

RunParameterTargetOrDefault "target" "Default"