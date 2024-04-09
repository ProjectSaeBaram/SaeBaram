using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MobController))]
public class MobControllerEditor : Editor
{
    private float _handleSize = 30f; // 핸들 크기를 조정하기 위한 값
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();      // 기존 인스펙터 GUI 요소를 그린다.
        
        if (GUILayout.Button("Initialize Patrol Points"))
        {
            MobController mob = (MobController)target;
            // 순찰 지점 초기화 (기본값)
            
            mob.patrolPointA = mob.transform.position + new Vector3(-350f, 0f, 0f);
            mob.patrolPointB = mob.transform.position + new Vector3(350f, 0f, 0f); 
            
            // 변경 사항을 적용하기 위해 에디터에 알린다.
            EditorUtility.SetDirty(mob);
        }
    }

    protected void OnSceneGUI()
    {
        MobController mob = (MobController)target;
        
        EditorGUI.BeginChangeCheck(); // 변화 감지 시작 (추가됨)
        
        // 순찰 지점 가져오기
        Vector3 pointAWorld = mob.patrolPointA;
        Vector3 pointBWorld = mob.patrolPointB;

        // 첫 번째 순찰 지점에 대해 붉은색 핸들 그리기
        Handles.color = Color.red;
        pointAWorld = Handles.FreeMoveHandle(pointAWorld, _handleSize, Vector3.zero, Handles.SphereHandleCap);
        Handles.Label(pointAWorld, "Patrol Point A");

        // 두 번째 순찰 지점에 대해 파란색 핸들 그리기
        Handles.color = Color.blue;
        pointBWorld = Handles.FreeMoveHandle(pointBWorld, _handleSize, Vector3.zero, Handles.SphereHandleCap);
        Handles.Label(pointBWorld, "Patrol Point B");

        if (EditorGUI.EndChangeCheck()) // 변화 감지 종료 및 확인 (추가됨)
        {
            Undo.RecordObject(mob, "Change Patrol Points"); // Undo 기록 (추가됨)
            EditorUtility.SetDirty(mob); // 객체가 변경되었음을 알림 (추가됨)

            // 조작 결과를 순찰 지점 변수에 직접 저장 (World Space 좌표로 저장)
            mob.patrolPointA = pointAWorld;
            mob.patrolPointB = pointBWorld;
        }

        // 순찰 경로를 Scene View에 그림
        Handles.color = Color.black;
        Handles.DrawLine(pointAWorld, pointBWorld);

        // 플레이어 감지 범위 그리기 (항상 Mob의 현재 위치를 중심으로 그림)
        Handles.color = Color.yellow;
        float range = mob.CurrentState switch
        {
            MobController.MobPatrolState => mob.DetectionRange,
            MobController.MobChaseState => mob.ChasableRange,
            _ => 0
        };
        Handles.DrawWireArc(mob.transform.position, Vector3.forward, Vector3.up, 360, range);
        
        // 공격 범위 그리기 (항상 Mob의 현재 위치를 중심으로 그림)
        Handles.color = Color.red;
        Handles.DrawWireArc(mob.transform.position, Vector3.forward, Vector3.up, 360, mob.AttackRange);
    }
}