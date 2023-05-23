using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using ToonJido.Data.Model;

namespace ToonJido.Data.Saver
{
    public class PlayerDataSaver : IDisposable
    {
        private readonly string infoFileName = "player_info.json";
        private readonly string tokenPW = "rmdwjd17!!";
        string dataPath = Path.Combine(Application.persistentDataPath, "token.txt");

        private bool disposedValue;

        private byte[] buffer = new byte[1024];

        public void SavePlayerInfo(User user)
        {
            string path = Path.Combine(Application.persistentDataPath, "playerInfo.json");

            string saveData = JsonConvert.SerializeObject(user);

            File.WriteAllTextAsync(path, saveData);
        }


        public async void SaveToken(string token)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(token);
            var encToken = Encrypt(bytes, tokenPW);

            using (FileStream outputFile = new FileStream(dataPath, FileMode.OpenOrCreate))
            {
                await outputFile.WriteAsync(encToken);
            }
        }

        public string LoadToken()
        {
            byte[] bytes;

            using(FileStream loadFile = new FileStream(dataPath, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(loadFile))
                {
                    bytes = reader.ReadBytes(1024);
                }
            }

            bytes = Decrypt(bytes, tokenPW);

            return Encoding.UTF8.GetString(bytes);
        }


        public byte[] Encrypt(byte[] origin, string password)
        {
            RijndaelManaged aes = new RijndaelManaged();       //AES 알고리즘
            Rfc2898DeriveBytes key = CreateKey(password);            //키값 생성
            Rfc2898DeriveBytes vector = CreateVector("ZaWmAcu1C2fbgJa4cPuZrT6MhuWmx6GE");   //벡터 생성 

            aes.BlockSize = 128;            //AES의 블록 크기는 128 고정이다.
            aes.KeySize = 256;              //AES의 키 크기는 128, 192, 256을 지원한다.
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(32);     //AES-256을 사용하므로 키값의 길이는 32여야 한다.
            aes.IV = vector.GetBytes(16);   //초기화 벡터는 언제나 길이가 16이어야 한다.

            //키값과 초기화 벡터를 기반으로 암호화 작업을 하는 클래스 변수를 생성
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            //using블록으로 변수를 사용하면 블록에서 나올때 자동으로 변수가 가비지컬렉팅 된다. 
            using (MemoryStream ms = new MemoryStream()) //결과를 담을 스트림 
            {
                //encryptor 변수에서 암호화된 데이터를 결과에 쓰는 스트림
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(origin, 0, origin.Length);
                }
                return ms.ToArray();    //암호화된 바이트 배열 반환
            }
        }

        public byte[] Decrypt(byte[] origin, string password)
        {
            RijndaelManaged aes = new RijndaelManaged();       //AES 알고리즘
            Rfc2898DeriveBytes key = CreateKey(password);            //키값 생성
            Rfc2898DeriveBytes vector = CreateVector("ZaWmAcu1C2fbgJa4cPuZrT6MhuWmx6GE");   //벡터 생성 

            aes.BlockSize = 128;            //AES의 블록 크기는 128 고정이다.
            aes.KeySize = 256;              //AES의 키 크기는 128, 192, 256을 지원한다.
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(32);     //AES-256을 사용하므로 키값의 길이는 32여야 한다.
            aes.IV = vector.GetBytes(16);   //초기화 벡터는 언제나 길이가 16이어야 한다.

            //키값과 초기화 벡터를 기반으로 복호화 작업을 하는 클래스 변수를 생성
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            //using블록으로 변수를 사용하면 블록에서 나올때 자동으로 변수가 가비지컬렉팅 된다. 
            using (MemoryStream ms = new MemoryStream()) //결과를 담을 스트림 
            {
                //encryptor 변수에서 복호화된 데이터를 결과에 쓰는 스트림
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(origin, 0, origin.Length);
                }
                return ms.ToArray();    //복호화된 바이트 배열 반환
            }
        }


        public Rfc2898DeriveBytes CreateKey(string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);         //키값 생성
            byte[] saltBytes = SHA512.Create().ComputeHash(keyBytes);   //솔트값(원본 키값을 알기 어렵게 하는 값)

            Rfc2898DeriveBytes result = new Rfc2898DeriveBytes(keyBytes, saltBytes, 10);    //키값에 솔트값을 사용해 새로운 키 생성, 마지막에 들어가는 수는 해시 생성의 반복 횟수이다.

            return result;  //키값 반환
        }

        public Rfc2898DeriveBytes CreateVector(string vector)
        {
            byte[] vectorBytes = Encoding.UTF8.GetBytes(vector);        //벡터 생성
            byte[] saltBytes = SHA512.Create().ComputeHash(vectorBytes);   //솔트값(원본 벡터를 알기 어렵게 하는 값)

            Rfc2898DeriveBytes result = new Rfc2898DeriveBytes(vectorBytes, saltBytes, 10);    //벡터에 솔트값을 사용해 새로운 키 생성, 마지막에 들어가는 수는 해시 생성의 반복 횟수이다.

            return result;  //벡터 반환
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~PlayerDataSaver()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}