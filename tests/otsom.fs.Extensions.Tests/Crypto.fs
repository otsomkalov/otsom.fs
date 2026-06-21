module Crypto

open Xunit
open FsUnit.Xunit
open otsom.fs.Extensions.Crypto

[<Fact>]
let ``Should encrypt and decrypt string`` () =
  let key = "test-key"
  let plainText = "hello world"

  let encrypted = encrypt key plainText
  let decrypted = decrypt key encrypted

  decrypted |> should equal plainText

[<Fact>]
let ``Different IVs for same text and key`` () =
  let key = "test-key"
  let plainText = "hello world"

  let encrypted1 = encrypt key plainText
  let encrypted2 = encrypt key plainText

  encrypted1 |> should not' (equal encrypted2)

[<Fact>]
let ``Should fail to decrypt with wrong key`` () =
  let key1 = "key1"
  let key2 = "key2"
  let plainText = "hello world"
  let encrypted = encrypt key1 plainText

  (fun () -> decrypt key2 encrypted |> ignore)
  |> should throw typeof<System.Security.Cryptography.CryptographicException>