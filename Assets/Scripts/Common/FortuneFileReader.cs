using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ToonJido.Data.Model;
using UnityEngine;

namespace ToonJido.Common{
    public class FortuneFileReader : IDisposable
    {
        const string fortuneFilePath = "FortuneCookie";
        List<Fortune> Fortunes = new();
        private bool disposedValue;

        public List<Fortune> Load(){
            var fortuneString = Resources.Load<TextAsset>(fortuneFilePath);
            StringReader reader = new StringReader(fortuneString.text);
            string[] sitems;
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                sitems = line.Split(",");
                Fortunes.Add(new Fortune(sitems[0], sitems[1]));
            }
            return Fortunes;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~FortuneFileReader()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

