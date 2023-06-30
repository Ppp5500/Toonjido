namespace ToonJido.Login{

        public enum IdType{
                apple,
                kakao
        }
    public static class UserProfile
    {
        public static string id;
#if DEVELOPMENT
        public static string social_login_id = "2774886049";
#else
        public static string social_login_id;
#endif
        public static IdType curr_id_type;
        public static string token;
    }
}

