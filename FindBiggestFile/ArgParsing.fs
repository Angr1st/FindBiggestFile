module ArgParsing

open Argu

type CliArguments =
    | [<Mandatory; Unique>] ConfigFilePath of string
    | Init
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | ConfigFilePath _ -> "Path to a configuration file!"
            | Init -> "Create a template config file!"
            
type ArgContainer =
    {
        ConfigFilePath:string
        Init:bool
    }

let parser = ArgumentParser<CliArguments> "FindBiggestFile.exe"

let defaultArgContainer =
    {
        ConfigFilePath=""
        Init=false
    }

let toArgContainer arguments =
    let rec parseArguments (list: CliArguments list) (acc:ArgContainer) =
        match list with
        | [] -> acc
        | x::xs ->
            match x with
            | ConfigFilePath z -> {acc with ConfigFilePath=z} |> parseArguments xs
            | Init -> {acc with Init=true} |> parseArguments xs

    
    parseArguments arguments defaultArgContainer

let parseArguments args =
    let parsedResults = parser.Parse args
    let results = parsedResults.GetAllResults()
    results
    |> toArgContainer