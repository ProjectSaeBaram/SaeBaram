using System;
using System.Collections.Generic;

namespace Data
{
    /// <summary>
    /// 캐릭터의 능력치를 정의하는 클래스.
    /// Json파일을 읽고 쓰는 포맷이 되어준다.
    /// </summary>
    [Serializable]
    public class Stat
    {
        public int level;   // 캐릭터의 레벨
        public int hp;      // 캐릭터의 체력
        public int attack;  // 캐릭터의 공격력
    }

    /// <summary>
    /// Stat 클래스의 인스턴스를 리스트로 관리하고, 이를 딕셔너리로 변환하는 기능을 제공하는 클래스.
    /// </summary>
    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>(); // Stat 인스턴스를 저장하는 리스트
        
        /// <summary>
        /// 리스트에 저장된 Stat 인스턴스를 바탕으로 딕셔너리를 생성하고 반환합니다.
        /// 딕셔너리의 키는 캐릭터의 레벨, 값은 해당 레벨의 Stat 인스턴스입니다.
        /// </summary>
        /// <returns>레벨을 키로 하고, 해당 레벨의 Stat 인스턴스를 값으로 하는 딕셔너리</returns>
        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();

            foreach (var item in stats)
            {
                dict.Add(item.level, item);
            }

            return dict;
        }
    }
}