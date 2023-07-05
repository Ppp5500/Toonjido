using System;

namespace ToonJido.Data.Model
{
    [Serializable]
    public class User
    {
        public string token {get; set;}
        public string user_social_id{get; set;}
        public string email {get; set;}
        public string gender {get; set;}
        public string age {get; set;}
        public int profile_img {get; set;} = 1;
        public int point {get; set;}
        public int step {get; set;}
        public string nickname {get; set;}
        public int[] review {get; set;}
        public int[] wishList {get; set;}

        public IdType idType {get; set;}
    }

    public enum IdType{
            apple,
            kakao
    }
}
