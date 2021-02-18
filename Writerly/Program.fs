type Sentences =
    | Sentences of string list

type WriterlyCommand =
    | ParseText of string
    | ProcessSentences of Sentences

type WriterlyResponse =
    | WriterlyResponse of Sentences

type ParseResponse =
    | Ok
    | Fail

type ProcessResponse =
    | Ok
    | Fail


type Text =
    | Text of string

type Writerly =
    | Stop
    | Parse     of Text         * (ParseResponse -> Writerly)
    | Process   of (* none *)     (unit -> Writerly)
    | Print     of                (unit -> Writerly)

module Lookup =
    open FSharp.Data

    type LookupResponse = JsonProvider<""" [{"word":"wandered","score":72428,"tags":["v"]}] """, SampleIsList=true>

    let lookup log word =
         async {
             let! jsonResponse =
                Http.AsyncRequestString
                    ( "https://api.datamuse.com/words", httpMethod = "GET",
                      query = ["ml", word; "tags", "v"],
                      headers = ["Accept", "application/json"])

             let isVerb (r: LookupResponse.Root) =
                 let tags = r.Tags |> List.ofArray
                 List.contains "v" tags

             let result =
                 LookupResponse.ParseList(jsonResponse)
                 |> List.ofArray
                 |> List.filter isVerb

             let newWord =
                match result with
                | x::xs -> x.Word
                | [] -> word

             log (sprintf "original = %s; replacement = %s;" word newWord)

             return newWord
         } |> Async.RunSynchronously

let program =
    Parse (Text "I walked home. I entered the house. I didn't want to do it, but I decided I had to. I took a huge shit on the dining room table.", fun response ->
    Process (fun () ->
    Print (fun () ->
    Stop)))

[<AutoOpen>]
module Helpers =
    let tee f x =
        do f x
        x

module Writerly =
    type WriterlyState = {
        originalText: string
        sentences: Sentences
    }

    let private toSentences log (text : string) : Sentences =
        text.Split(",")
        |> List.ofArray
        |> Sentences
        |> tee (fun (Sentences s) ->
            s
            |> String.concat "\n\r"
            |> sprintf "%s"
            |> log
           )

    let start log (Text str) (state: WriterlyState) =
        log (sprintf "Start %s" str)
        let sentences = toSentences log str
        let newState = { state with sentences = sentences; originalText = str  }
        (ParseResponse.Ok, newState)

    let processSentences log (state: WriterlyState) =
        let (Sentences s) = state.sentences
        let split (s : string) =
            s.Split(" ")
            |> List.ofArray

        let newSentences =
            s
            |> List.map split
            |> List.map (fun listOfStringArrays -> List.map (Lookup.lookup log) listOfStringArrays )
            |> List.map (fun listOfStringArrays -> String.concat " " listOfStringArrays)
        let newState = { state with sentences = (Sentences newSentences) }
        (ProcessResponse.Ok, newState)


let rec interpretAsWriterly state program =
    let log = printfn "%s"

    match program with
    | Parse (t, next) ->
        let result, newState = Writerly.start log t state
        let nextProgram = next result
        interpretAsWriterly newState nextProgram
    | Process (next) ->
        let result, newState = Writerly.processSentences log state
        let nextProgram = next ()
        interpretAsWriterly newState nextProgram
    | Print (next) ->
        let (Sentences s) = state.sentences
        s
        |> List.map (sprintf "%s")
        |> List.map (printf "%s")
        |> ignore
        let nextProgram = next ()
        interpretAsWriterly state nextProgram

    | Stop ->
        state

open Writerly

[<EntryPoint>]
let main argv =


    let blobOfText =
        argv
        |> String.concat " "

    let initialState: WriterlyState = {
        sentences = (Sentences List.empty)
        originalText = ""
    }

    let result = interpretAsWriterly initialState program




    0 // return an integer exit code
