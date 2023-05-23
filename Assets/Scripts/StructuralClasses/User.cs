using System;

namespace ToonJido.Data.Model
{
    [Serializable]
    public class User
    {
        private uint uid;
        private int gender;
        private uint age;
        private string nickName;
        private byte[] profilePic;
        private uint walkCount;
        private int point;

        // 이게 모임?
        private int review;
        private int wishList;
    }
}

