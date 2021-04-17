// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO

let getBiggestFile folder searchPattern =
    let getFileSize fileName =
        let bytes = File.ReadAllBytes fileName
        (fileName, bytes.LongLength)

    let biggestFile (currentBiggest:string*int64) (newFile:string*int64): string*int64 =
        if snd currentBiggest >= snd newFile then
            currentBiggest
        else
            newFile

    let defaultState = ("default", 1L)

    if Directory.Exists folder then
        let recurseOptions = EnumerationOptions()
        recurseOptions.RecurseSubdirectories <- true

        Directory.EnumerateFiles(folder, searchPattern, recurseOptions)
        |> Seq.map getFileSize
        |> Seq.fold biggestFile defaultState
    else
        raise (DirectoryNotFoundException(folder))

let searchForBiggestFiles (config:Config.Config) =
    let printResults res =
        printfn "File: %s; Size: %i bytes" (fst res) (snd res)
    
    let getBiggestFile' = getBiggestFile config.RootFolder

    config.SearchPatterns
    |> List.map getBiggestFile'
    |> List.iter printResults

[<EntryPoint>]
let main argv =
    let main' (argCon:ArgParsing.ArgContainer) =
        if argCon.Init then
            Config.createDefaultConfig()

        if argCon.ConfigFilePath <> "" then
            let configRes = 
                argCon.ConfigFilePath
                |> Config.loadConfig

            match configRes with
            | Ok config -> 
                config
                |> searchForBiggestFiles
                Ok 0
            | Error e -> 
                eprintfn "%s" e
                Error -1
        else
            Error 0

    let returnCode (result:Result<int,int>) =
        match result with
        | Ok o -> o
        | Error e -> e


    argv
    |> ArgParsing.parseArguments
    |> Result.bind main'
    |> returnCode