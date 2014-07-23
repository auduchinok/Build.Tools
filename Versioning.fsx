#r @"./fake/fakelib.dll"
#load "./Utils.fsx"

open System
open System.Text.RegularExpressions
open System.Xml
open Fake
open Fake.Git
open Utils

let private readAssemblyVersion file =
    ReadFile file
        |> Seq.find (fun line -> not (line.StartsWith("//") || line.StartsWith("'")) && line.Contains "AssemblyVersion")
        |> (fun line -> Regex.Match(line, @"(?<=\().+?(?=\))").Value)
        |> (fun version -> Version (version.Trim [|'"'|]))

let private escapeBranchName rawName =
    let regex = System.Text.RegularExpressions.Regex(@"[^0-9A-Za-z-]")
    let safeChars = regex.Replace(rawName, "-")
    safeChars.[0..(min 9 <| String.length safeChars - 1)]

    
let private constructInfoVersion (config: Map<string, string>) (fileVersion: Version) file =
    let infoVersion = 
        Version (
            fileVersion.Major, 
            fileVersion.Minor, 
            fileVersion.Build)

    let suffix =
        match isLocalBuild with
            | true -> 
                "-" + ((getBranchName (DirectoryName file)) |> escapeBranchName) + "-" + (fileVersion.Revision + 1).ToString() + "-local"
            | _ ->
                match config.get "versioning:branch" with
                    | "master" -> 
                        "." + (fileVersion.Revision + 1).ToString()
                    | _ -> 
                        "-" + (config.get "versioning:branch" |> escapeBranchName) + "-" + (fileVersion.Revision + 1).ToString() + "-ci"

    infoVersion.ToString() + suffix


let private constructVersions (config: Map<string, string>) file =
    let fileVersion = readAssemblyVersion file

    let assemblyVersion = 
        Version (
            fileVersion.Major, 
            fileVersion.Minor,
            fileVersion.Build, 
            int <| fileVersion.Revision + 1)

    assemblyVersion.ToString(), (constructInfoVersion config fileVersion file)

let private updateAssemblyInfo config file =
    let versions = constructVersions config file

    ReplaceAssemblyInfoVersions (fun x ->
        {
            x with
                 OutputFileName = file
                 AssemblyConfiguration = config.get "build:configuration"
                 AssemblyVersion = fst versions
                 AssemblyFileVersion = fst versions
                 AssemblyInformationalVersion = snd versions
        })

let private updateDeployNuspec config (file:string) =
    let xdoc = new XmlDocument()
    ReadFileAsString file |> xdoc.LoadXml
    
    let versionNode = xdoc.SelectSingleNode "/package/metadata/version"

    let semVer = SemVerHelper.parse(versionNode.InnerText.ToString())

    let semVerSplitByDot = versionNode.InnerText.Split('.')
    let suffix = semVerSplitByDot.[semVerSplitByDot.Length - 1]
    let suffixSplitByHyphen = suffix.Split('-')
    let mutable buildNumber = 0
    if suffixSplitByHyphen.Length > 1 then 
        buildNumber <- (suffixSplitByHyphen.[2] |> int)
    else 
        buildNumber <- (suffixSplitByHyphen.[0] |> int)

    let fileVersion = new Version(semVer.Major, semVer.Minor, semVer.Patch, buildNumber)

    versionNode.InnerText <- (constructInfoVersion config fileVersion file)
    
    WriteStringToFile false file (xdoc.OuterXml.ToString().Replace("><",">\n<"))

let update (config : Map<string, string>) _ =
    //!+ "./**/AssemblyInfo.cs"
    //++ "./**/AssemblyInfo.vb"
    !! (sprintf @"%s" (config.get "packaging:assemblyinfopath"))
    //++ "./**/AssemblyInfo.vb"
        |> Scan
        |> Seq.iter (updateAssemblyInfo config)

let updateDeploy (config : Map<string, string>) _ =
    !! (sprintf @"%s" (config.get "packaging:nuspecpath"))
        |> Seq.iter (updateDeployNuspec config)
