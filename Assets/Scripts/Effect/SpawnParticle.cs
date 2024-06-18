using UnityEngine;

/// <summary>
/// GameObject가 삭제되거나 비활성화될 때 지정한 Particle을 소환하는 클래스
/// </summary>
public class SpawnParticle : MonoBehaviour
{
    public GameObject ParticleToSpawn;
    private bool isGameActive;

    private void Start()
    {
        // 게임이 시작될 때 isGameActive를 true로 설정
        isGameActive = true;
    }

    private void OnDisable()
    {
        // 게임이 활성 상태일 때만 파티클을 생성
        if (isGameActive)
        {
            InstantiateParticle();
        }
    }

    private void OnDestroy()
    {
        // 게임이 활성 상태일 때만 파티클을 생성
        if (isGameActive)
        {
            InstantiateParticle();
        }
    }

    private void InstantiateParticle()
    {
        GameObject go = Instantiate(ParticleToSpawn, transform.position, Quaternion.identity);
        ParticleSystem particle = go.GetComponent<ParticleSystem>();
        particle.Play();
    }

    private void OnApplicationQuit()
    {
        // 게임이 종료될 때 isGameActive를 false로 설정
        isGameActive = false;
    }
}