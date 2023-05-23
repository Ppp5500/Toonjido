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
            RijndaelManaged aes = new RijndaelManaged();       //AES �˰���
            Rfc2898DeriveBytes key = CreateKey(password);            //Ű�� ����
            Rfc2898DeriveBytes vector = CreateVector("ZaWmAcu1C2fbgJa4cPuZrT6MhuWmx6GE");   //���� ���� 

            aes.BlockSize = 128;            //AES�� ��� ũ��� 128 �����̴�.
            aes.KeySize = 256;              //AES�� Ű ũ��� 128, 192, 256�� �����Ѵ�.
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(32);     //AES-256�� ����ϹǷ� Ű���� ���̴� 32���� �Ѵ�.
            aes.IV = vector.GetBytes(16);   //�ʱ�ȭ ���ʹ� ������ ���̰� 16�̾�� �Ѵ�.

            //Ű���� �ʱ�ȭ ���͸� ������� ��ȣȭ �۾��� �ϴ� Ŭ���� ������ ����
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            //using������� ������ ����ϸ� ��Ͽ��� ���ö� �ڵ����� ������ �������÷��� �ȴ�. 
            using (MemoryStream ms = new MemoryStream()) //����� ���� ��Ʈ�� 
            {
                //encryptor �������� ��ȣȭ�� �����͸� ����� ���� ��Ʈ��
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(origin, 0, origin.Length);
                }
                return ms.ToArray();    //��ȣȭ�� ����Ʈ �迭 ��ȯ
            }
        }

        public byte[] Decrypt(byte[] origin, string password)
        {
            RijndaelManaged aes = new RijndaelManaged();       //AES �˰���
            Rfc2898DeriveBytes key = CreateKey(password);            //Ű�� ����
            Rfc2898DeriveBytes vector = CreateVector("ZaWmAcu1C2fbgJa4cPuZrT6MhuWmx6GE");   //���� ���� 

            aes.BlockSize = 128;            //AES�� ��� ũ��� 128 �����̴�.
            aes.KeySize = 256;              //AES�� Ű ũ��� 128, 192, 256�� �����Ѵ�.
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(32);     //AES-256�� ����ϹǷ� Ű���� ���̴� 32���� �Ѵ�.
            aes.IV = vector.GetBytes(16);   //�ʱ�ȭ ���ʹ� ������ ���̰� 16�̾�� �Ѵ�.

            //Ű���� �ʱ�ȭ ���͸� ������� ��ȣȭ �۾��� �ϴ� Ŭ���� ������ ����
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            //using������� ������ ����ϸ� ��Ͽ��� ���ö� �ڵ����� ������ �������÷��� �ȴ�. 
            using (MemoryStream ms = new MemoryStream()) //����� ���� ��Ʈ�� 
            {
                //encryptor �������� ��ȣȭ�� �����͸� ����� ���� ��Ʈ��
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(origin, 0, origin.Length);
                }
                return ms.ToArray();    //��ȣȭ�� ����Ʈ �迭 ��ȯ
            }
        }


        public Rfc2898DeriveBytes CreateKey(string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);         //Ű�� ����
            byte[] saltBytes = SHA512.Create().ComputeHash(keyBytes);   //��Ʈ��(���� Ű���� �˱� ��ư� �ϴ� ��)

            Rfc2898DeriveBytes result = new Rfc2898DeriveBytes(keyBytes, saltBytes, 10);    //Ű���� ��Ʈ���� ����� ���ο� Ű ����, �������� ���� ���� �ؽ� ������ �ݺ� Ƚ���̴�.

            return result;  //Ű�� ��ȯ
        }

        public Rfc2898DeriveBytes CreateVector(string vector)
        {
            byte[] vectorBytes = Encoding.UTF8.GetBytes(vector);        //���� ����
            byte[] saltBytes = SHA512.Create().ComputeHash(vectorBytes);   //��Ʈ��(���� ���͸� �˱� ��ư� �ϴ� ��)

            Rfc2898DeriveBytes result = new Rfc2898DeriveBytes(vectorBytes, saltBytes, 10);    //���Ϳ� ��Ʈ���� ����� ���ο� Ű ����, �������� ���� ���� �ؽ� ������ �ݺ� Ƚ���̴�.

            return result;  //���� ��ȯ
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: ������ ����(������ ��ü)�� �����մϴ�.
                }

                // TODO: ������� ���ҽ�(������� ��ü)�� �����ϰ� �����ڸ� �������մϴ�.
                // TODO: ū �ʵ带 null�� �����մϴ�.
                disposedValue = true;
            }
        }

        // // TODO: ������� ���ҽ��� �����ϴ� �ڵ尡 'Dispose(bool disposing)'�� ���Ե� ��쿡�� �����ڸ� �������մϴ�.
        // ~PlayerDataSaver()
        // {
        //     // �� �ڵ带 �������� ������. 'Dispose(bool disposing)' �޼��忡 ���� �ڵ带 �Է��մϴ�.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // �� �ڵ带 �������� ������. 'Dispose(bool disposing)' �޼��忡 ���� �ڵ带 �Է��մϴ�.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}