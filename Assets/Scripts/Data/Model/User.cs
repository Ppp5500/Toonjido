using System;

namespace ToonJido.Data.Model
{
    [Serializable]
    public class User
    {
        public uint uid {get; set;}
        public string user_social_id{get; set;}
        public string token {get; set;}
        private int gender;
        private uint age;
        private string nickName;
        private byte[] profilePic;
        private uint walkCount;
        private int point;

        // ??? ?????
        private int review;
        private int wishList;
    }
}
