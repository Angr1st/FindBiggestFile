module Config

type Config =
    {
        SearchPatterns:string list
        RootFolder:string
    }

open System.IO
open System.Text.Json
open System.Text.Json.Serialization

let loadConfig path =
    let options = JsonSerializerOptions()
    options.Converters.Add(JsonFSharpConverter())

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