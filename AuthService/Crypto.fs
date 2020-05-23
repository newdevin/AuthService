namespace AuthService
open System.IO
open System.Security.Cryptography
open System

[<RequireQualifiedAccess>]
module Crypto = 

    let generateKey = 
        use aes = new RijndaelManaged()
        aes.GenerateKey()
        aes.GenerateIV()
        aes.Key, aes.IV

    let encrypt (key:byte[]) (iv:byte[]) (data:string) = 
        use aesAlg = Aes.Create()
        aesAlg.Key <- key
        aesAlg.IV <- iv
        let encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV)
        use msEncrypt = new MemoryStream()
        (
            use csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
            use swEncrypt = new StreamWriter(csEncrypt)
            swEncrypt.Write(data)
        )
        let s = msEncrypt.ToArray()
                |> Convert.ToBase64String 
        s.Replace("+","_").Replace("/","-")
        
        

    let decrypt (key:byte[]) (iv:byte[]) (encryptedData:string) = 
        let enc = encryptedData.Replace("_","+").Replace("-","/")
        let data = Convert.FromBase64String(enc)
        use aesAlg = Aes.Create()
        aesAlg.Key <- key
        aesAlg.IV <- iv
        let encryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV)
        use msEncrypt = new MemoryStream(data)
        (
            use csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Read)
            use swEncrypt = new StreamReader(csEncrypt)
            swEncrypt.ReadToEnd()
        )
        
