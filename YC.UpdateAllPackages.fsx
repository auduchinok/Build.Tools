#r @"Fake/FakeLib.dll"
#load "Core.fsx"
#load "Utils.fsx"

open Fake
open Fake.Git
open Core
open System.IO

let specConfig = new SpecificConfig("", "", "", "", "", "")
commonConfig specConfig

"Packaging:Restore"
    ==> "Packaging:Update"
    ==> "Solution:Clean"
    ==> "Default"

RunParameterTargetOrDefault "target" "Default"