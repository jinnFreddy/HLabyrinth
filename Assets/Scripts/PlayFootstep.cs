using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlayFootstep : MonoBehaviour
{
    public void PlayMonsterFootstepSounds()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 monsterPos = transform.position;
        Vector3 playerPos = player.transform.position;

        float pathDistance = NavMeshUtils.GetPathDistance(monsterPos, playerPos);

        if (float.IsPositiveInfinity(pathDistance) || pathDistance > 15f)
            return;

        float maxHearDistance = 15f;
        float minVolume = 0.1f;
        float normalized = 1f - Mathf.Clamp01(pathDistance / maxHearDistance);
        float curved = Mathf.Pow(normalized, 1.4f);
        float volume = Mathf.Lerp(minVolume, 1f, curved);

        if (Physics.Linecast(monsterPos, playerPos, out RaycastHit hit))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Default"))
            {
                volume *= 0.3f;
            }
        }

        SoundManager.PlaySound(SoundType.MWALK, volume);
    }

    public void PlayMonsterRunSounds()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 monsterPos = transform.position;
        Vector3 playerPos = player.transform.position;

        float pathDistance = NavMeshUtils.GetPathDistance(monsterPos, playerPos);

        if (float.IsPositiveInfinity(pathDistance) || pathDistance > 15f)
            return;

        float maxHearDistance = 15f;
        float minVolume = 0.1f;
        float normalized = 1f - Mathf.Clamp01(pathDistance / maxHearDistance);
        float curved = Mathf.Pow(normalized, 1.4f);
        float volume = Mathf.Lerp(minVolume, 1f, curved);

        if (Physics.Linecast(monsterPos, playerPos, out RaycastHit hit))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Default"))
            {
                volume *= 0.3f;
            }
        }

        SoundManager.PlaySound(SoundType.MRUN, volume);
    }
}
