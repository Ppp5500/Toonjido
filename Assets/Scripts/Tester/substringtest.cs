using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToonJido.Data.Model;
using ToonJido.Data.Saver;

public class substringtest : MonoBehaviour
{
    public string deeplinkURL;
    public string token;

    // Start is called before the first frame update
    void Start()
    {
        //onDeepLinkActivated("toonjido://mylink?eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6NDJ9.EJqq5a9mTYg5wr3E_kcdL7LExnx3QavbS9ZZ8V47cYg2701232134");
        LoadId();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onDeepLinkActivated(string url)
        {
            // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
            deeplinkURL = url;

            // Decode the URL to determine action. 
            // In this example, the app expects a link formatted like this:
            // unitydl://mylink?scene1
            token = url.Split("?"[0])[1];
            int length = token.Length;
            string myuser_social_id = token.Substring(length-10, 10);
            token = token.Remove(length - 10, 10);
            deeplinkURL = "id: "+ myuser_social_id + "token: " + token;

            using (PlayerDataSaver saver = new())
            {
                User user = new(){
                    user_social_id = myuser_social_id
                };
                saver.SaveToken(token);
                saver.SavePlayerInfo(user);
            }
        }

        public async void LoadId(){
            using(PlayerDataSaver saver = new()){
                string id = await saver.LoadUserSocialIdAsync();
                print(id);
            }
        }
}
