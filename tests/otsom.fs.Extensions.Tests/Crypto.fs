module Crypto

open Xunit
open otsom.fs.Extensions.Crypto

[<Fact>]
let ``Should encrypt and decrypt string`` () =
  let key = "test-key"
  let plainText = "hello world"

  let encrypted = encrypt key plainText
  let decrypted = decrypt key encrypted

  Assert.Equal(plainText, decrypted)

[<Fact>]
let ``Different IVs for same text and key`` () =
  let key = "test-key"
  let plainText = "hello world"

  let encrypted1 = encrypt key plainText
  let encrypted2 = encrypt key plainText

  Assert.NotEqual<string>(encrypted1, encrypted2)

[<Fact>]
let ``Should fail to decrypt with wrong key`` () =
  let key1 = "key1"
  let key2 = "key2"
  let plainText = "hello world"
  let encrypted = encrypt key1 plainText

  Assert.ThrowsAny<System.Security.Cryptography.CryptographicException>(fun () -> decrypt key2 encrypted |> ignore)
  |> ignore