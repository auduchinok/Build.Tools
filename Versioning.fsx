#r @"./fake/fakelib.dll"
#load "./Utils.fsx"

open System
open System.Text.RegularExpressions
open System.Xml
open System.IO
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
                "." + (fileVersion.Revision + 1).ToString() + "-" + ((getBranchName (DirectoryName file)) |> escapeBranchName) + "-local"
            | _ ->
                match config.get "versioning:branch" with
                    | "master" -> 
                        "." + (fileVersion.Revision + 1).ToString()
                    | _ -> 
                        "." + (fileVersion.Revision + 1).ToString() + "-" + (config.get "versioning:branch" |> escapeBranchName) + "-ci"

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

let private renameDll (config : Map<string,string>) file finalVersion =
    let allFiles = Directory.GetFiles(config.get "bin:path")

    for fileName in allFiles do
        let test = "Test"
        let fileNameWithoutExtension = Path.GetFileNameWithoutExtension fileName
        if fileNameWithoutExtension.Contains(file) && not (fileNameWithoutExtension = file) && not (fileNameWithoutExtension.Contains(test)) then
            File.Delete fileName

    for fileName in allFiles do
        if (file = Path.GetFileNameWithoutExtension fileName) then
            let fileExtensionLength = Path.GetExtension fileName |> String.length
            let newName = fileName.Insert(fileName.Length - fileExtensionLength, sprintf ".%s" finalVersion)
            File.Move(fileName, newName)    

let private setVersionToDll config (node : XmlNode) finalVersion = 
    let path = node.Attributes.Item(0).InnerText
    let file = Path.GetFileNameWithoutExtension path
    if (file.Substring(0, 2) = "YC") then
        renameDll config file finalVersion

let private setVersionToReferences (node : XmlNode) (finalVersion : string) = 
    let file = node.Attributes.Item(0).InnerText
    let fileExtension = Path.GetExtension file
    let fileNameWithoutExtension = Path.GetFileNameWithoutExtension file

    let fileNameSplitByDot = fileNameWithoutExtension.Split('.')
    let finalVersionSplitByDot = finalVersion.Split('.')
    fileNameSplitByDot.[fileNameSplitByDot.Length - 1] <- finalVersionSplitByDot.[finalVersionSplitByDot.Length - 1]

    String.concat "." fileNameSplitByDot + fileExtension


let private updateDeployNuspec config (file:string) =
    let xdoc = new XmlDocument()
    ReadFileAsString file |> xdoc.LoadXml
    
    let versionNode = xdoc.SelectSingleNode "/package/metadata/version"

    let semVer = SemVerHelper.parse(versionNode.InnerText.ToString())

    let semVerSplitByDot = versionNode.InnerText.Split('.')
    let suffix = semVerSplitByDot.[semVerSplitByDot.Length - 1]
    let suffixSplitByHyphen = suffix.Split('-')
    let buildNumber = suffixSplitByHyphen.[0] |> int

    let fileVersion = new Version(semVer.Major, semVer.Minor, semVer.Patch, buildNumber)
    let finalVersion = constructInfoVersion config fileVersion file

    versionNode.InnerText <- finalVersion

    let listFileNodes = xdoc.SelectNodes("/package/files/file")
    for node in listFileNodes do 
        setVersionToDll config node finalVersion

    let listRefNodes = xdoc.SelectNodes("/package/metadata/references/reference")
    if listRefNodes.Count <> 0 then
        for node in listRefNodes do
            let newName = setVersionToReferences node finalVersion
            node.Attributes.Item(0).InnerText <- newName

    WriteStringToFile false file (xdoc.OuterXml.ToString().Replace("><",">\n<"))


let update (config : Map<string, string>) _ =
    //!+ "./**/AssemblyInfo.cs"
    //++ "./**/AssemblyInfo.vb"
    !! (sprintf @"%s" (config.get "packaging:assemblyinfopath"))
    //++ "./**/AssemblyInfo.vb"
        //|> Scan
        |> Seq.iter (updateAssemblyInfo config)

let updateDeploy (config : Map<string, string>) _ =
    !! (sprintf @"%s" (config.get "packaging:nuspecpath"))
        |> Seq.iter (updateDeployNuspec config)

let readCommonVersion (config : Map<string, string>) =
    (config.get "versioning:path" |> File.ReadAllLines).[0]

let writeCommonVersion (config : Map<string, string>) =
    let ver = readCommonVersion config
    let lastDigit = ver.Substring (ver.LastIndexOf '.' + 1) |> Int32.Parse
    let vers = ver.Substring (0, ver.LastIndexOf '.' + 1) + (lastDigit + 1).ToString()
    File.WriteAllText (config.get "versioning:path", vers)