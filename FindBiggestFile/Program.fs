// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System.IO
open Config

let getBiggestFile folder (searchPattern:SearchPatternType) =
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

        match searchPattern with
        | Basic sPat ->
            Directory.EnumerateFiles(folder, sPat, recurseOptions)
            |> Seq.map getFileSize
            |> Seq.fold biggestFile defaultState
        | BiggestFileInFolder bCont ->
            let findFile rFolder =
                Directory.EnumerateFiles(rFolder, bCont.SearchPattern(), recurseOptions)

            Directory.EnumerateDirectories(folder, bCont.Folder, recurseOptions)
            |> Seq.collect findFile
            |> Seq.map getFileSize
            |> Seq.fold biggestFile defaultState
    else
        raise (DirectoryNotFoundException(folder))

let searchForBiggestFiles (config:Config) =
    let printResults res =
        printfn "File: %s; Size: %i bytes" (fst res) (snd res)
    
    let ignoreEmpty (searchPattern:SearchPatternType) =
        let isEmpty s =
            System.String.IsNullOrWhiteSpace s

        let ShouldIgnore =
            match searchPattern with
            | Basic pattern -> 
                isEmpty pattern
            | BiggestFileInFolder bCont ->
               isEmpty bCont.Folder || isEmpty bCont.FileType
        ShouldIgnore
        |> not

    let getBiggestFile' = getBiggestFile config.RootFolder

    config.SearchPatterns
    |> List.filter ignoreEmpty
    |> List.map getBiggestFile'
    |> List.iter printResults

[<EntryPoint>]
let main argv =
    let main' (argCon:ArgParsing.ArgContainer) =
        if argCon.Init then
            createDefaultConfig()

        if argCon.ConfigFilePath <> "" then
            let configRes = 
                argCon.ConfigFilePath
                |> loadConfig

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