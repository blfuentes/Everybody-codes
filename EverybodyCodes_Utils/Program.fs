namespace EverybodyCodes_Utils

open System.Net.Http
open System
open System.Net
open System.Threading.Tasks
open System.Security.Cryptography
open System.Text
open System.Text.Json
open System.Collections.Generic

module EverybodyCodes_Utils =
    // api-session.txt lives next to this source file (key=value lines)
    let private sessionSettings =
        IO.File.ReadAllLines(__SOURCE_DIRECTORY__ + @"\api-session.txt")
        |> Array.choose (fun line ->
            match line.IndexOf '=' with
            | i when i > 0 -> Some (line.Substring(0, i).Trim(), line.Substring(i + 1).Trim())
            | _ -> None)
        |> dict

    let private setting key =
        match sessionSettings.TryGetValue key with
        | true, v -> v
        | _ -> failwith $"'{key}' is missing in api-session.txt"

    let seedValue = int (setting "seed_value")
    let session_cookie_header = setting "session_cookie_header"
    let session_cookie_value = setting "session_cookie_value"
    let private handler =
        let h = new SocketsHttpHandler()
        h.PooledConnectionLifetime <- TimeSpan.FromMinutes(10.0)
        h.AutomaticDecompression <- DecompressionMethods.GZip ||| DecompressionMethods.Deflate
        h

    let client =
        let c = new HttpClient(handler, disposeHandler = false)
        c.Timeout <- TimeSpan.FromMinutes(30.0)
        c

    let fetch (url: string) : Task<string> =
        task {
            use! resp = client.GetAsync(url, HttpCompletionOption.ResponseContentRead)
            resp.EnsureSuccessStatusCode() |> ignore
            return! resp.Content.ReadAsStringAsync()
        }

    let getContentAsync (eventStory: int) (quest: int) : Async<string> =
        fetch $"https://everybody.codes/assets/{eventStory}/{quest}/input/{seedValue}.json"
        |> Async.AwaitTask

    let getAESKey (eventStory: int) (quest: int) : Async<string> =
        task {
            use req = new HttpRequestMessage(HttpMethod.Get, $"https://api.everybody.codes/event/{eventStory}/quest/{quest}")
            req.Headers.Add("Cookie", $"{session_cookie_header}={session_cookie_value}")
            use! resp = client.SendAsync(req)
            resp.EnsureSuccessStatusCode() |> ignore
            return! resp.Content.ReadAsStringAsync()
        }
        |> Async.AwaitTask
        
    // key: 32-char string used as UTF8 bytes; IV is its first 16 bytes; cipherHex: encrypted bytes as hex
    let decodeAesCbcHex (key: string) (cipherHex: string) : string =
        let keyBytes = Encoding.UTF8.GetBytes key
        let cipherBytes = Convert.FromHexString cipherHex

        use aes = Aes.Create()
        aes.Mode <- CipherMode.CBC
        aes.Padding <- PaddingMode.PKCS7
        aes.Key <- keyBytes
        aes.IV <- keyBytes[0..15]

        use decryptor = aes.CreateDecryptor()
        let plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length)
        Encoding.UTF8.GetString plainBytes

    // keyBase64: 16/24/32-byte AES key encoded as Base64
    // ivBase64: 16-byte IV encoded as Base64
    // cipherBase64: encrypted bytes encoded as Base64
    let decodeAesCbcBase64 (keyBase64: string) (ivBase64: string) (cipherBase64: string) : string =
        let key = Convert.FromBase64String keyBase64
        let iv = Convert.FromBase64String ivBase64
        let cipherBytes = Convert.FromBase64String cipherBase64

        use aes = Aes.Create()
        aes.Mode <- CipherMode.CBC
        aes.Padding <- PaddingMode.PKCS7
        aes.Key <- key
        aes.IV <- iv

        use decryptor = aes.CreateDecryptor()
        let plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length)
        Encoding.UTF8.GetString plainBytes

    let retrieveContent (eventStory: int) (quest: int) (part: int) =
        let content = getContentAsync eventStory quest |> Async.RunSynchronously
        let data = (JsonSerializer.Deserialize<Dictionary<string, string>>(content))[string part]
        let key =
            use doc = getAESKey eventStory quest |> Async.RunSynchronously |> JsonDocument.Parse
            match doc.RootElement.TryGetProperty($"key{part}") with
            | true, k -> k.GetString()
            | _ -> failwith $"No AES key for event {eventStory} quest {quest} part {part}: that part is not solved on this account yet, or the session cookie is invalid."
        decodeAesCbcHex key data

