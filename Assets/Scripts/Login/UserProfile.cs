using ToonJido.Data.Model;

namespace ToonJido.Login{
    public static class UserProfile
    {
#if DEVELOPMENT
        public static string social_login_id {get; set;}= "2774886049";
#else
        public static string social_login_id {get; set;}
#endif
        public static IdType curr_id_type {get; set;}
        public static string token {get; set;}
    }
}

