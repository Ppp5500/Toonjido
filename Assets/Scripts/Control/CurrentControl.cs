using System;

namespace ToonJido.Control
{
    public static class CurrentControl
    {
        public enum State
        {
            Overlook,
            Eyelevel,
            SearchResult,
            Profile,
            Weather
        }

        public static State state = State.Overlook;
        public static State lastState;

        /// <summary>
        /// 부감 상태로 전환
        /// </summary>
        public static Action overlookAction;

        /// <summary>
        /// 아이레벨 상태로 전환
        /// </summary>
        public static Action eyelevelAction;

        /// <summary>
        /// 검색 결과를 보여주는 상태로 전환할 때
        /// </summary>
        public static Action searchResultAction;

        /// <summary>
        /// 프로필을 보여주는 상태로 전환할 때
        /// </summary>
        public static Action profileAction;

        /// <summary>
        /// 날씨를 보여주는 상태로 전환할 때
        /// </summary>
        public static Action weatherAction;

        public static void ChangeToOverlook()
        {
            lastState = state;
            state = State.Overlook;
            overlookAction();
        }

        public static void ChangeToEyelevel()
        {
            lastState = state;
            state = State.Eyelevel;
            eyelevelAction();
        }

        public static void ChangeToSearchResult()
        {
            lastState = state;
            state = State.SearchResult;
            searchResultAction();
        }

        public static void ChangeToProfile()
        {
            lastState = state;
            state = State.Profile;
            profileAction();
        }

        public static void ChangeToWeather()
        {
            lastState = state;
            state = State.Weather;
            weatherAction();
        }

        public static void ChangeToLastState()
        {
            state = lastState;
            switch (state)
            {
                case State.Overlook:
                    overlookAction();
                    break;
                case State.Eyelevel:
                    eyelevelAction();
                    break;
                case State.SearchResult:
                    searchResultAction();
                    break;
                case State.Profile:
                    profileAction();
                    break;
            }
        }
    }
}