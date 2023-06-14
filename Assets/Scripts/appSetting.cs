using System;
using System.IO;
using UnityEngine;

public static class appSetting
{
    public static readonly string baseURL = "http://43.201.215.208:8000/hongbo/";
    public static readonly Uri baseURI = new("http://43.201.215.208:8000/hongbo/");
    public static readonly string tokenPath = Path.Combine(Application.persistentDataPath, "token.txt");
    public static readonly string userInfoPath = Path.Combine(Application.persistentDataPath, "playerInfo.json");
    public static readonly string dataPath = Path.Combine(Application.persistentDataPath, "maindata.json");
}