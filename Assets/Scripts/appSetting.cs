using System;
using System.IO;
using UnityEngine;

public static class appSetting
{
    public const string AppleUserIdKey = "AppleUserId";
    public const string KakaoUserIdKey = "KakaoUserId";
    public const string baseURL = "http://43.201.215.208:8000/hongbo/";
    public static readonly string tokenPath = Path.Combine(Application.persistentDataPath, "token.txt");
    public static readonly string userInfoPath = Path.Combine(Application.persistentDataPath, "playerInfo.json");
    public static readonly string dataPath = Path.Combine(Application.persistentDataPath, "maindata.json");
    public static readonly string lastAccessDatePath = Path.Combine(Application.persistentDataPath, "lastaccessdate.txt");
    public static readonly string marbleGameDataPath = Path.Combine(Application.persistentDataPath, "marblegamedata.txt");
}