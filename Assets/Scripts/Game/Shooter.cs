using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToonJido.Game
{
    public class Shooter : MonoBehaviour
    {
        // 미사일 프리팹.
        public List<GameObject> marblePrefs;
        // 도착 지점들 부모
        [SerializeField] GameObject targetParant;
        // 도착 지점들
        List<GameObject> targets = new();
        List<int> targetIndex = new();
        List<int> marbleIndex = new();
        // Dictionary<int, int> indexDic = new();
        // 계산용 임시 속성
        GameObject m_target;

        [Header("미사일 설정")]
        public float m_speed = 2;

        [Space(10f)]
        // 시작 지점을 기준으로 얼마나 꺾일지.
        public float m_distanceFromStart = 6.0f;
        // 도착 지점을 기준으로 얼마나 꺾일지.
        public float m_distanceFromEnd = 3.0f;

        [Range(0, 1)]
        // 각 발사 간격
        public float m_interval = 0.15f;
        // 한번에 몇 개씩 발사할건지.
        public int m_shotCountEveryInterval = 2;
        

        void Start() {
            for(int i = 0; i < targetParant.transform.childCount; i++){
                targets.Add(targetParant.transform.GetChild(i).gameObject);
            }
        }

        public void ShootStarMarble(out Dictionary<int, int> indexDic)
        {
            indexDic = new();
            var targetIndex = GenerateTargetIndex();
            var marbleIndex = GenerateMarbleIndex();

            for(int i = 0; i < marbleIndex.Count; i ++){
                indexDic.Add(targetIndex[i], marbleIndex[i]);
            }

            StartCoroutine(CreateMissileVer2(
                targetIndex,
                marbleIndex
                )
            );
        }

        private List<int> GenerateTargetIndex()
        {
            targetIndex = new List<int>(){0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14};
            targetIndex = ShuffleList(targetIndex);
            return targetIndex;
        }

        private List<int> GenerateMarbleIndex(){
            marbleIndex = new List<int>(){0, 1, 2, 3, 4, 5, 6};
            marbleIndex = ShuffleList(marbleIndex);
            return marbleIndex;
        }

        /// <summary>
        /// 미사일 프리팹의 갯 수 만큼 공 발사,
        /// 도착 위치는 targetParent의 자식 중 겹치지 않게 랜덤,
        /// </summary>
        /// <returns></returns>
        IEnumerator CreateMissileVer2(List<int> _targetIndex, List<int> _marbleIndex)
        {
            int _shotCount = _marbleIndex.Count;

            for(int i = 0; i < _shotCount; i ++){
                m_target = targets[targetIndex[i]];
                GameObject missile = Instantiate(marblePrefs[_marbleIndex[i]]);
                missile
                    .GetComponent<BezierMissile>()
                    .Init(
                        this.gameObject.transform,
                        m_target.transform,
                        m_speed,
                        m_distanceFromStart,
                        m_distanceFromEnd
                    );
                    print($"t name: {m_target.name}, s name: {missile.name}");
                yield return new WaitForSeconds(m_interval);
            }
        }

        /// <summary>
        /// 미사일 프리팹의 갯 수 만큼 공 발사,
        /// 도착 위치는 targetParent의 자식 중 겹치지 않게 랜덤,
        /// </summary>
        /// <returns></returns>
        IEnumerator CreateMissile(List<int> _targetIndex)
        {
            int _shotCount = marblePrefs.Count;
            List<GameObject> randomTargets = ShuffleList(targets);
            randomTargets.RemoveRange(marblePrefs.Count, randomTargets.Count - marblePrefs.Count);

            while (_shotCount > 0)
            {
                m_target = randomTargets[_shotCount - 1];
                for (int i = 0; i < m_shotCountEveryInterval; i++)
                {
                    if (_shotCount > 0)
                    {
                        GameObject missile = Instantiate(marblePrefs[_shotCount - 1]);
                        missile
                            .GetComponent<BezierMissile>()
                            .Init(
                                this.gameObject.transform,
                                m_target.transform,
                                m_speed,
                                m_distanceFromStart,
                                m_distanceFromEnd
                            );

                        _shotCount--;
                    }
                }
                yield return new WaitForSeconds(m_interval);
            }
            yield return null;
        }

        private List<T> ShuffleList<T>(List<T> list)
        {
            int random1, random2;
            T temp;

            for (int i = 0; i < list.Count; ++i)
            {
                random1 = Random.Range(0, list.Count);
                random2 = Random.Range(0, list.Count);

                temp = list[random1];
                list[random1] = list[random2];
                list[random2] = temp;
            }

            return list;
        }
    }
}
