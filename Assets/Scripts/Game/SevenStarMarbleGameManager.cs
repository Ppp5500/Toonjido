using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using DG.Tweening;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ToonJido.Data.Model;
using ToonJido.Data.Saver;
using ToonJido.Game;

using UnityEngine;
using UnityEngine.Playables;

public class SevenStarMarbleGameManager : MonoBehaviour
{
    [SerializeField] GameObject hDragon;
    public MarbleGameData marbleGameData = new();

    // 미사일 프리팹.
    public List<GameObject> MarblePrefs;

    // 미사일 도착 지점들 부모 오브젝트
    [SerializeField] GameObject targetParant;

    // 미사일 도착 지점들
    List<GameObject> targets = new();
    Dictionary<int, int> indexDic = new();

    // 게임 시작 시간
    private const int startTime01 = 10;
    private const int startTime02 = 19;
    private const int eventTime = 1;

    // 흥룡이 움직임 관련 변수
    const float dragonMoveInterval = 10;
    bool dragonAppearChecker = false;

    [Header("Dragon Move Points")]
    [SerializeField] Transform initPos;
    [SerializeField] Transform firstTarget;
    [SerializeField] Transform secondTarget;
    [SerializeField] Transform thirdTarget;
    [SerializeField] Transform forthTarget;
    [SerializeField] Transform fifthTarget;
    [SerializeField] Transform sixthTarget;


    // common
    private static SevenStarMarbleGameManager instance;
    static readonly Timer timer;

#if DEVELOPMENT
    private async void OnGUI() {

        if(GUI.Button(new Rect(50, 250, 150, 150), "marble save!")){
            print("click!");
            await SaveMarbleData();
        }

        if(GUI.Button(new Rect(50, 450, 150, 150), "marble load!")){
            print("click!");
            await LoadMarbleData();
        }

        if(GUI.Button(new Rect(50, 650, 150, 150), "marble found!")){
            print("click!");
            FoundMarble("3");
        }
    }
#endif

    void Awake() {
        // Singleton
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(this);
        }
        marbleGameData.marbles = new();
    }

    // Singleton
    public static SevenStarMarbleGameManager GetInstance(){
        return instance;
    }

    // Start is called before the first frame update
     async Task Start()
    {
        for(int i = 0; i < targetParant.transform.childCount; i++){
            targets.Add(targetParant.transform.GetChild(i).gameObject);
        }
        // FlyingAround();
        // EventTimeCheck();
        print(DateTime.Now.Date);
        await DragonEventAsyncForToday(DateTime.Now);
    }

    async void EventTimeCheck(){
        DateTime now = DateTime.Now;

        DateTime todayNoon = new DateTime(now.Year, now.Month, now.Day, startTime01, 0, 0);
        DateTime todayAfternoon = todayNoon.AddHours(eventTime);

        DateTime today8pm = new DateTime(now.Year, now.Month, now.Day, startTime02, 0, 0);
        DateTime todayAfter8pm = today8pm.AddHours(eventTime);

        if(now < todayNoon){
            // 오전 타이머
            SetEventTimer(timer, todayNoon, now);
        }
        else if(now < todayAfternoon){
            // 점심 이벤트
            await DragonEventAsync(todayNoon);
        }
        else if(now < today8pm){
            // 오후 타이머
            SetEventTimer(timer, today8pm, now);
        }
        else if(now < todayAfter8pm){
            // 저녁 이벤트
            await DragonEventAsync(today8pm);
        }
        else{
            // 내일 점심 이벤트
            DateTime tomorrowNoon = todayNoon.AddDays(1);
            SetEventTimer(timer, tomorrowNoon, now);
        }
    }

    void SetEventTimer(Timer _timer, DateTime _targetTime, DateTime _startTime){
        double interval = (_targetTime - _startTime).TotalMilliseconds;
        _timer = new Timer(interval);
        _timer.Elapsed += (source, e) => DragonEventAsync(source, e, _startTime);
        _timer.Enabled = true;
    }

    /// <summary>
    /// Save current marble data
    /// </summary>
    /// <returns></returns>
    async Task SaveMarbleData(){
        if(marbleGameData != null){
            using(PlayerDataSaver saver = new()){
                await saver.SaveMarbleData(marbleGameData);
            }
        }
    }

    /// <summary>
    /// Load saved marble data and generate(not shoot) marbles
    /// </summary>
    /// <returns></returns>
    async Task LoadMarbleData(){
        using(PlayerDataSaver saver = new()){
            marbleGameData = await saver.LoadMarbleData();
        }

        foreach(var item in marbleGameData.marbles){
            Transform tras = targets[int.Parse(item.target)].transform;
            int starNum = int.Parse(item.stars);
            var marble = Instantiate(MarblePrefs[starNum]);
            Destroy(marble.GetComponent<BezierMissile>());

            marble.GetComponent<MarbleToCollect>().stars = starNum;
            marble.transform.position = tras.position;
            // print($"t: {item.target}, s: {item.stars}");
        }

        hDragon.SetActive(true);
    }

    /// <summary>
    /// Delete a marble from current marblegameddata and update save file
    /// </summary>
    /// <param name="_stars">Number of star</param>
    /// <returns></returns>
    public async void FoundMarble(string _stars){
        if(marbleGameData != null){
            print("input: " + _stars);
            var dataToRemove = marbleGameData.marbles.Where(x => x.stars == _stars);
            marbleGameData.marbles = marbleGameData.marbles.Except(dataToRemove).ToList();

            await SaveMarbleData();
        }
    }

    // 흥룡이를 돌아다니게 하는 메소드
    public void FlyingAround(){
        if(!dragonAppearChecker){
            StartCoroutine(FlyingAroundIenum());
            dragonAppearChecker = true;
        }
    }

    IEnumerator FlyingAroundIenum(){
            float t = 0f;

            hDragon.SetActive(true);

            hDragon.transform.position = initPos.position;
            hDragon.transform.rotation = initPos.rotation;
        while(true){
            hDragon.transform.DOMove(firstTarget.position, 3);
            hDragon.transform.DORotate(firstTarget.eulerAngles, 3);

            while(t < dragonMoveInterval){
                t += Time.deltaTime;
                yield return null;
            }
            t = 0;

            hDragon.transform.DOMove(secondTarget.position, 5);
            hDragon.transform.DORotate(secondTarget.eulerAngles, 5);

            while(t < dragonMoveInterval){
                t += Time.deltaTime;
                yield return null;
            }
            t = 0;

            hDragon.transform.DOMove(thirdTarget.position, 3);
            hDragon.transform.DORotate(thirdTarget.eulerAngles, 3);

            while(t < dragonMoveInterval){
                t += Time.deltaTime;
                yield return null;
            }
            t = 0;

            hDragon.transform.DOMove(forthTarget.position, 3);
            hDragon.transform.DORotate(forthTarget.eulerAngles, 3);

            while(t < dragonMoveInterval){
                t += Time.deltaTime;
                yield return null;
            }
            t = 0;

            hDragon.transform.DOMove(fifthTarget.position, 3);
            hDragon.transform.DORotate(fifthTarget.eulerAngles, 3);

            while(t < dragonMoveInterval){
                t += Time.deltaTime;
                yield return null;
            }
            t = 0;

            hDragon.transform.DOMove(sixthTarget.position, 3);
            hDragon.transform.DORotate(sixthTarget.eulerAngles, 3);
        }
    }

    async void DragonEventAsync(System.Object source, ElapsedEventArgs e, DateTime _startTime){
        print("흥룡이 쿠아앙아 하고 울부짖었다.");
        using(PlayerDataSaver saver = new()){
            bool temp = await saver.CheckDragonEventRecord(_startTime);

            if(!temp)
            {
                FlyingAround();
                await InitMarbleGame();
            }
            else
            {
                await LoadMarbleData();
            }
        }
    }

    async Task DragonEventAsync(DateTime _startTime){
        print("흥룡이 쿠아앙아 하고 울부짖었다.");
        using(PlayerDataSaver saver = new()){
            bool temp = await saver.CheckDragonEventRecord(_startTime);
            print(temp);

            if(!temp)
            {
                FlyingAround();
                await InitMarbleGame();
            }
            else
            {
                await LoadMarbleData();
            }
        }
    }

    async Task DragonEventAsyncForToday(DateTime _startTime)
    {
        print("흥룡이 쿠아앙아 하고 울부짖었다.");
        using (PlayerDataSaver saver = new())
        {
            bool temp = await saver.CheckDragonEventRecordForToday(_startTime);
            print($"is access today? {temp}");

            if (!temp)
            {
                FlyingAround();
                await InitMarbleGame();
            }
            else
            {
                await LoadMarbleData();
            }
        }
    }

    async Task InitMarbleGame(){
        using PlayerDataSaver saver = new();
        hDragon.gameObject.GetComponent<Shooter>().ShootStarMarble(out indexDic);

        foreach (var item in indexDic)
        {
            marbleGameData.marbles.Add(new Marble()
            {
                target = item.Key.ToString(),
                stars = item.Value.ToString()
            });

            // print($"t: {item.Key}, s: {item.Value}");
        }

        await saver.SaveMarbleData(marbleGameData);
    }
}
