using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float initialPositionZ;
    [SerializeField] float resetPositionZ;

    // StartMove 메서드를 UniTask로 변경
    public void StartMove(float moveDuration)
    {
        MoveCorridor(moveDuration).Forget();  // 비동기 작업을 실행하고 대기하지 않음
    }

    // UniTask로 변환된 MoveCorridor 메서드
    private async UniTaskVoid MoveCorridor(float moveDuration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            // 복도를 이동시키는 로직
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

            // 일정 위치에 도달하면 위치를 초기화
            if (transform.position.z <= resetPositionZ)
            {
                Vector3 newPosition = transform.position;
                newPosition.z = initialPositionZ;
                transform.position = newPosition;
            }

            elapsedTime += Time.deltaTime;

            // 다음 프레임까지 대기 (코루틴의 yield return null과 동일)
            await UniTask.Yield();
        }
    }
}
