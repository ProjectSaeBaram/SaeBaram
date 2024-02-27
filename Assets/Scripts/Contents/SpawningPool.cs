using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// 게임 내에서 몬스터를 주기적으로 생성하고 관리하는 클래스입니다.
/// 이 클래스는 몬스터의 현재 수를 추적하고, 지정된 수량을 유지하기 위해 몬스터를 리스폰합니다.
/// </summary>
public class SpawningPool : MonoBehaviour
{
    [SerializeField] private int _monsterCount = 0; // 게임 내에 현재 활성화된 몬스터의 수입니다.
    private int reserveCount = 0; // 리스폰 대기 중인 몬스터의 수입니다.

    [SerializeField] private int _keepMonsterCount = 0; // 게임에서 유지하려는 몬스터의 총 수입니다.

    [SerializeField] private Vector3 spawnPos; // 몬스터가 리스폰될 기준 위치입니다.
    [SerializeField] private float _spawnRadius = 15; // 리스폰될 몬스터가 spawnPos로부터 퍼져 나갈 수 있는 최대 반경입니다.

    [SerializeField] private float spawnDelay = 5.0f; // 몬스터가 리스폰되는 데까지 걸리는 딜레이 시간입니다.

    /// <summary>
    /// 현재 몬스터 수를 증가시키는 메서드입니다.
    /// </summary>
    /// <param name="value">증가시킬 몬스터 수</param>
    public void AddMonsterCount(int value)
    {
        _monsterCount += value;
    }

    /// <summary>
    /// 유지해야 할 몬스터 수를 설정하는 메서드입니다.
    /// </summary>
    /// <param name="count">게임 내에 유지될 몬스터의 총 수</param>
    public void SetKeepMonsterCount(int count)
    {
        _keepMonsterCount = count;
    }

    void Start()
    {
        // 몬스터 리스폰 이벤트에 현재 클래스의 AddMonsterCount 메서드를 연결합니다.
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        // 현재 몬스터 수와 대기 중인 몬스터 수의 합이 유지해야 할 몬스터 수보다 적다면 리스폰을 진행합니다.
        while (reserveCount + _monsterCount < _keepMonsterCount)
        {
            StartCoroutine(nameof(ReserveSpawn)); // 리스폰 프로세스를 비동기적으로 실행합니다.
        }
    }

    /// <summary>
    /// 몬스터를 리스폰하는 코루틴입니다.
    /// 이 메서드는 몬스터를 생성하고, 무작위 위치에 배치합니다.
    /// </summary>
    IEnumerator ReserveSpawn()
    {
        reserveCount++; // 대기 중인 몬스터 수를 증가시킵니다.

        // 리스폰 딜레이를 적용합니다.
        yield return new WaitForSeconds(Random.Range(0, spawnDelay));

        // 몬스터를 생성하고 NavMeshAgent 컴포넌트를 가져옵니다.
        GameObject monster = Managers.Game.Spawn(Define.WorldObject.Monster, "DarkKnight");
        var nma = monster.GetOrAddComponent<NavMeshAgent>();

        Vector3 randPos;
        while (true)
        {
            // spawnPos 기준으로 _spawnRadius 내의 무작위 위치를 선택합니다.
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            randDir.y = 0;
            randPos = spawnPos + randDir;

            // 선택된 위치가 유효한지 확인합니다.
            NavMeshPath path = new NavMeshPath();
            if (nma.CalculatePath(randPos, path))
                break; // 유효한 위치가 확인되면 루프를 종료합니다.
        }

        // 몬스터를 무작위 위치에 배치합니다.
        monster.transform.position = randPos;
        reserveCount--; // 대기 중인 몬스터 수를 감소시킵니다.
    }
}