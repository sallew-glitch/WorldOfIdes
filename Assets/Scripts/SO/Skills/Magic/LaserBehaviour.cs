using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour : MonoBehaviour
{
    public void ActivateLaser(Transform origin, float duration, GameObject _laserPrefab)
    {
        StartCoroutine(InstantiateLaser(2, origin, duration, _laserPrefab));
    }
    private IEnumerator InstantiateLaser(float time, Transform origin, float duration, GameObject _laserPrefab)
    {
        yield return new WaitForSeconds(time);

        var shield = Instantiate(_laserPrefab, origin.position + origin.up * 1f + origin.forward * 1f, origin.rotation, origin);
        Destroy(shield, duration);
        PlayerManager.Instance._playerController._animator.SetBool("laserMagic", false);
    }

    public void ActivateFireball(Transform origin, float duration, GameObject _fireballPrefab)
    {
        StartCoroutine(InstantiateFireball(2, origin, duration, _fireballPrefab));
    }

    private IEnumerator InstantiateFireball(float time, Transform origin, float duration, GameObject _fireballPrefab)
    {
        yield return new WaitForSeconds(time);

        var fireball = Instantiate(_fireballPrefab, origin.position + origin.up * 1f + origin.forward * 1f, origin.rotation, origin);

        for (float timer = 0f; timer < time; timer++)
        {
            fireball.transform.position = fireball.transform.position + origin.transform.forward * 0.5f;
            timer += Time.deltaTime;
        }

        Destroy(fireball, duration);
        PlayerManager.Instance._playerController._animator.SetBool("fireballMagic", false);
    }
}
