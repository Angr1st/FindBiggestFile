module Config

type BiggestFileInFolderContainer =
    {
        Folder:string
        FileType:string
    }
    with
        member this.SearchPattern ()=
            sprintf "*.%s" this.FileType

type SearchPatternType =
    | Basic of string
    | BiggestFileInFolder of BiggestFileInFolderContainer

type Config =
    {
        SearchPatterns:SearchPatternType list
        RootFolder:string
    }


open System.IO
open System.Text.Json
open System.Text.Json.Serialization

let private options = 
    let serializerOptions = JsonSerializerOptions()
    serializerOptions.Converters.Add(JsonFSharpConverter())
    serializerOptions

let loadConfig path =
    if System.String.IsNullOrWhiteSpace path then
        Error "Config file Path was empty!"
    elif File.Exists path then
        try
            let text = File.ReadAllText path
            JsonSerializer.Deserialize<Config>(text, options)
            |> Ok
        with
            | :? System.Text.Json.JsonException as je -> Error je.Message
    else
        Error (sprintf "Config file did not exist at path: %s!" path)

let createDefaultConfig ()=
    let defaultConfig =
        {
            SearchPatterns= [Basic "example.txt"; BiggestFileInFolder {Folder="Test"; FileType="txt"}]
            RootFolder="."
        }
    let initOptions = JsonSerializerOptions(options)
    initOptions.WriteIndented <- true
    let text = JsonSerializer.Serialize(defaultConfig, initOptions)
    File.WriteAllText("./Example_Config.json", text)
