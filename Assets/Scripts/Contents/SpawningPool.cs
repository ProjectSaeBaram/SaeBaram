using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// 몬스터의 리스폰을 관리하는 클래스
/// </summary>
public class SpawningPool : MonoBehaviour
{
    // 현재 존재하는 몬스터의 수
    // GameManager의 OnSpawnEvent를 호출하여 이 숫자를 관리한다.
    [SerializeField] 
    private int _monsterCount = 0;
    private int reserveCount = 0;
    
    // 유지시켜야 하는 몬스터의 수
    [SerializeField] 
    private int _keepMonsterCount = 0;

    // 스폰 기준점과 반경
    [SerializeField] 
    private Vector3 spawnPos;
    [SerializeField] 
    private float _spawnRadius = 15;

    // 스폰 딜레이
    [SerializeField] 
    private float spawnDelay = 5.0f;
    
    public void AddMonsterCount(int value) { _monsterCount += value; }
    public void SetKeepMonsterCount(int count) { _keepMonsterCount = count; }
    
    void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        while (reserveCount + _monsterCount < _keepMonsterCount)
        {
            StartCoroutine(nameof(ReserveSpawn));
        }
    }

    IEnumerator ReserveSpawn()
    {
        reserveCount++;
        
        // 딜레이 적용
        yield return new WaitForSeconds(Random.Range(0, spawnDelay));
        
        GameObject monster = Managers.Game.Spawn(Define.WorldObject.Monster,"DarkKnight");
        var nma = monster.GetOrAddComponent<NavMeshAgent>();

        Vector3 randPos;
        while (true)
        {
            // 정해진 지점으로부터 일정 반경 내의 무작위 장소에 Spawn.
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            randDir.y = 0;
            randPos = spawnPos + randDir;

            // 정해진 장소가 존재할 수 있는 곳인지 검사
            NavMeshPath path = new NavMeshPath();
            if (nma.CalculatePath(randPos, path))
                break;
        }

        monster.transform.position = randPos;
        reserveCount--;
    }
}