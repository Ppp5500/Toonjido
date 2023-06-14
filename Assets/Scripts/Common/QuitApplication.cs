using UnityEngine;
using ToonJido.Data.Saver;
using ToonJido.UI;

public class QuitApplication : MonoBehaviour
{
    NoticeManager noticeManager;

    private void Start() {
        noticeManager = NoticeManager.GetInstance();
    }
    public void Quit()
    {
        noticeManager.SetConfirmButton( () => Application.Quit());
        noticeManager.SetCancelButtonDefault();
        noticeManager.ShowNotice("천안 원도심 탐험을 멈추시겠습니까?");
    }

    public void Logout(){
        noticeManager.SetConfirmButton( () => DeleteUserData());
        noticeManager.SetCancelButtonDefault();
        noticeManager.ShowNotice("로그아웃 하시면 사용자 정보가 삭제됩니다.");
    }

    private void DeleteUserData(){
        using(PlayerDataSaver saver = new()){
            saver.DeleteToken();
            saver.DeleteUserSocialId();
        }

        Application.Quit();
    }
}
