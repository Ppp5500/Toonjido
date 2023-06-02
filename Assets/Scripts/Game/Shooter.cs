using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToonJido.Game
{
    public class Shooter : MonoBehaviour
    {
        // 미사일 프리팹.
        public GameObject m_missilePrefab;
        public List<GameObject> targets;
        // 도착 지점.
        private GameObject m_target;

        [Header("????? ??? ????")]
        public float m_speed = 2;

        [Space(10f)]
        // 시작 지점을 기준으로 얼마나 꺾일지.
        public float m_distanceFromStart = 6.0f;
        // 도착 지점을 기준으로 얼마나 꺾일지.
        public float m_distanceFromEnd = 3.0f;

        [Space(10f)]
        // 총 몇 개 발사할건지.
        public int m_shotCount = 12;

        [Range(0, 1)]
        public float m_interval = 0.15f;
        // 한번에 몇 개씩 발사할건지.
        public int m_shotCountEveryInterval = 2;

        public void ShootStarBall()
        {
            StartCoroutine(CreateMissile());
        }

        IEnumerator CreateMissile()
        {
            m_target = targets[Random.Range(0, targets.Count)];
            int _shotCount = m_shotCount;
            while (_shotCount > 0)
            {
                for (int i = 0; i < m_shotCountEveryInterval; i++)
                {
                    if (_shotCount > 0)
                    {
                        GameObject missile = Instantiate(m_missilePrefab);
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
    }
}
