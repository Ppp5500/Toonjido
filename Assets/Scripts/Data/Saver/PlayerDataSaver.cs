using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static appSetting;

using ToonJido.Data.Model;

using UnityEngine;

namespace ToonJido.Data.Saver
{
    public class PlayerDataSaver : IDisposable
    {
        private const string tokenPW = "rmdwjd17!!";
        private bool disposedValue;
        private byte[] buffer = new byte[1024];

        public async Task SavePlayerInfo(User user)
        {
            string saveData = JsonConvert.SerializeObject(user);
            await File.WriteAllTextAsync(userInfoPath, saveData);
        }

        public async Task<string> LoadUserSocialIdAsync(){
            string loadData = await File.ReadAllTextAsync(userInfoPath);
            User user = JsonConvert.DeserializeObject<User>(loadData);
            return user.user_social_id;
        }

        public async Task<User> LoadUserAsync(){
            string loadData = await File.ReadAllTextAsync(userInfoPath);
            User user = JsonConvert.DeserializeObject<User>(loadData);
            return user;
        }

        public bool DeleteUserSocialId(){
            try{
                File.Delete(userInfoPath);
            }
            catch(DirectoryNotFoundException ex){
                Console.WriteLine(ex.Message);
                return true;
            }
            catch(IOException ex){
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public async Task SaveToken(string token)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(token);
            var encToken = Encrypt(bytes, tokenPW);

            using (FileStream outputFile = new FileStream(tokenPath, FileMode.OpenOrCreate))
            {
                await outputFile.WriteAsync(encToken);
            }
        }

        public string LoadToken()
        {
            byte[] bytes;

            using(FileStream loadFile = new FileStream(tokenPath, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(loadFile))
                {
                    bytes = reader.ReadBytes(1024);
                }
            }

            bytes = Decrypt(bytes, tokenPW);

            return Encoding.UTF8.GetString(bytes);
        }

        public bool DeleteToken(){
            try{
                File.Delete(tokenPath);
            }
            catch(DirectoryNotFoundException ex){
                Console.WriteLine(ex.Message);
                return true;
            }
            catch(IOException ex){
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

#region MarbleGameMethod
        public async Task SaveMarbleData(MarbleGameData _data){
            string dataString = JsonConvert.SerializeObject(_data);
            
            await File.WriteAllTextAsync(marbleGameDataPath ,dataString);
        }

        public async Task<MarbleGameData> LoadMarbleData(){
            var dataString = await File.ReadAllTextAsync(marbleGameDataPath);

            return JsonConvert.DeserializeObject<MarbleGameData>(dataString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>return true when already access today, else return false. Update last access time.</returns>
        public async Task<bool> CheckDragonEventRecord(){
            // 저장된 파일이 있는지 검사
            bool haveFile = File.Exists(lastAccessDatePath);

            if(!haveFile){
                DateTime today = DateTime.Now;
                await File.WriteAllTextAsync(lastAccessDatePath, today.ToString());
                return false;
            }
            else{
                string loadData = await File.ReadAllTextAsync(lastAccessDatePath);
                DateTime loadDate = DateTime.Parse(loadData);

                var today = DateTime.Now;
                var result = today.Day.CompareTo(loadDate.Day);

                if (result < 0) {
                    Console.WriteLine("how did you do that?");
                    return false;
                }
                else if (result > 0) {
                    return false;
                }
                else {
                    return true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_startTime">Time for check</param>
        /// <returns>Returns true if it is within one hour of the entered time.</returns>
        public async Task<bool> CheckDragonEventRecord(DateTime _startTime){
            // 저장된 파일이 있는지 검사
            bool haveFile = File.Exists(lastAccessDatePath);

            var now = DateTime.Now;

            if(!haveFile){
                await File.WriteAllTextAsync(lastAccessDatePath, now.ToString());
                return false;
            }
            else{
                string loadData = await File.ReadAllTextAsync(lastAccessDatePath);
                DateTime loadDate = DateTime.Parse(loadData);
                var endTime = _startTime.AddHours(1);

                // 최종 접속 시간 업데이트
                await File.WriteAllTextAsync(lastAccessDatePath, now.ToString());
                if (loadDate < _startTime) {
                    return false;
                }
                else if (loadDate < endTime) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
#endregion

#region EncryptionMethod

        public byte[] Encrypt(byte[] origin, string password)
        {
            RijndaelManaged aes = new RijndaelManaged();       //AES ???????
            Rfc2898DeriveBytes key = CreateKey(password);            //??? ????
            Rfc2898DeriveBytes vector = CreateVector("ZaWmAcu1C2fbgJa4cPuZrT6MhuWmx6GE");   //???? ???? 

            aes.BlockSize = 128;            //AES?? ???? ???? 128 ???????.
            aes.KeySize = 256;              //AES?? ? ???? 128, 192, 256?? ???????.
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(32);     //AES-256?? ??????? ????? ????? 32???? ???.
            aes.IV = vector.GetBytes(16);   //???? ????? ?????? ????? 16???? ???.

            //????? ???? ????? ??????? ???? ????? ??? ????? ?????? ????
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            //using???????? ?????? ?????? ??????? ????? ??????? ?????? ???????��??? ???. 
            using (MemoryStream ms = new MemoryStream()) //????? ???? ????? 
            {
                //encryptor ???????? ?????? ??????? ????? ???? ?????
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(origin, 0, origin.Length);
                }
                return ms.ToArray();    //?????? ????? ?�� ???
            }
        }

        public byte[] Decrypt(byte[] origin, string password)
        {
            RijndaelManaged aes = new RijndaelManaged();       //AES ???????
            Rfc2898DeriveBytes key = CreateKey(password);            //??? ????
            Rfc2898DeriveBytes vector = CreateVector("ZaWmAcu1C2fbgJa4cPuZrT6MhuWmx6GE");   //???? ???? 

            aes.BlockSize = 128;            //AES?? ???? ???? 128 ???????.
            aes.KeySize = 256;              //AES?? ? ???? 128, 192, 256?? ???????.
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(32);     //AES-256?? ??????? ????? ????? 32???? ???.
            aes.IV = vector.GetBytes(16);   //???? ????? ?????? ????? 16???? ???.

            //????? ???? ????? ??????? ???? ????? ??? ????? ?????? ????
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            //using???????? ?????? ?????? ??????? ????? ??????? ?????? ???????��??? ???. 
            using (MemoryStream ms = new MemoryStream()) //????? ???? ????? 
            {
                //encryptor ???????? ?????? ??????? ????? ???? ?????
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(origin, 0, origin.Length);
                }
                return ms.ToArray();    //?????? ????? ?�� ???
            }
        }

        public Rfc2898DeriveBytes CreateKey(string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);         //??? ????
            byte[] saltBytes = SHA512.Create().ComputeHash(keyBytes);   //?????(???? ????? ??? ???? ??? ??)

            Rfc2898DeriveBytes result = new Rfc2898DeriveBytes(keyBytes, saltBytes, 10);    //????? ??????? ????? ???��? ? ????, ???????? ????? ???? ??? ?????? ??? ??????.

            return result;  //??? ???
        }

        public Rfc2898DeriveBytes CreateVector(string vector)
        {
            byte[] vectorBytes = Encoding.UTF8.GetBytes(vector);        //???? ????
            byte[] saltBytes = SHA512.Create().ComputeHash(vectorBytes);   //?????(???? ????? ??? ???? ??? ??)

            Rfc2898DeriveBytes result = new Rfc2898DeriveBytes(vectorBytes, saltBytes, 10);    //????? ??????? ????? ???��? ? ????, ???????? ????? ???? ??? ?????? ??? ??????.

            return result;  //???? ???
        }
#endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}