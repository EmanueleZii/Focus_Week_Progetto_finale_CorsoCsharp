using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Utility per la cifratura e decifratura dei dati usando AES
// Questa utility utilizza una chiave e un vettore di inizializzazione fissi per
// cifrare e decifrare i dati. È importante notare che l'uso di chiavi fisse
// non è sicuro per applicazioni reali, ma è accettabile per scopi di test o
// per applicazioni che non richiedono un alto livello di sicurezza.
public class EncryptionUtility
{
    // Chiave e vettore di inizializzazione per la cifratura AES
    private readonly byte[] chiave = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
    private readonly byte[] iv = Encoding.UTF8.GetBytes("InizialVector123");

     // Cifra i dati usando AES
    public byte[] Cripta(byte[] dati)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = chiave;
            aes.IV = iv;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                return encryptor.TransformFinalBlock(dati, 0, dati.Length);
            }
        }
    }
    // Decifra i dati usando AES
    public byte[] Decripta(byte[] dati)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = chiave;
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                return decryptor.TransformFinalBlock(dati, 0, dati.Length);
            }
        }
    }
    

}
