﻿namespace Scripts

open Fake
open System.IO
open Commandline

module Benchmarker =

    let private testsProjectDirectory = Path.GetFullPath(Paths.TestsSource("Tests.Benchmarking"))

    let Run args =
        
        let url = match args.CommandArguments with | Benchmark b -> Some b.Endpoint | _ -> None
        let username = match args.CommandArguments with | Benchmark b -> b.Username | _ -> None
        let password = match args.CommandArguments with | Benchmark b -> b.Password | _ -> None
        let runInteractive = not args.NonInteractive
        let credentials  = (username, password)
        let runCommandPrefix = "run -f netcoreapp2.1 -c Release"
        let runCommand =
            match (runInteractive, url, credentials) with
            | (false, Some url, (Some username, Some password)) -> sprintf "%s -- --all \"%s\" \"%s\" \"%s\"" runCommandPrefix url username password
            | (false, Some url, _) -> sprintf "%s -- --all \"%s\"" runCommandPrefix url
            | (false, _, _) -> sprintf "%s -- --all" runCommandPrefix 
            | (true, _, _) -> runCommandPrefix
        
        DotNetCli.RunCommand(fun p -> { p with WorkingDir = testsProjectDirectory }) runCommand
