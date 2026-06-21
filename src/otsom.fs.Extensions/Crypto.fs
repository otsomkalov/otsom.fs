module otsom.fs.Extensions.Crypto

open System
open System.Security.Cryptography
open System.Text

[<Literal>]
let private SaltSize = 16

[<Literal>]
let private NonceSize = 12

[<Literal>]
let private TagSize = 16

let encrypt (key: string) (plaintext: string) =
  let salt = RandomNumberGenerator.GetBytes SaltSize
  let nonce = RandomNumberGenerator.GetBytes NonceSize // Recommended size for GCM

  let key =
    Rfc2898DeriveBytes.Pbkdf2(key, salt, 100_000, HashAlgorithmName.SHA256, 32)

  let plaintextBytes = Encoding.UTF8.GetBytes plaintext
  let ciphertext = Array.zeroCreate<byte> plaintextBytes.Length
  let tag = Array.zeroCreate<byte> TagSize

  use aes = new AesGcm(key, TagSize)
  aes.Encrypt(nonce, plaintextBytes, ciphertext, tag)

  Array.concat [ salt; nonce; tag; ciphertext ] |> Convert.ToBase64String

let decrypt (key: string) (encrypted: string) =
  let data = Convert.FromBase64String encrypted

  let salt = data.[0..15]
  let nonce = data.[16..27]
  let tag = data.[28..43]
  let ciphertext = data.[44..]

  let key =
    Rfc2898DeriveBytes.Pbkdf2(key, salt, 100_000, HashAlgorithmName.SHA256, 32)

  let plaintext = Array.zeroCreate<byte> ciphertext.Length

  use aes = new AesGcm(key, TagSize)
  aes.Decrypt(nonce, ciphertext, tag, plaintext)

  Encoding.UTF8.GetString plaintext