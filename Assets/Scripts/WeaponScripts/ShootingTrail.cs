using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting_Trail : MonoBehaviour
{

    private TrailRenderer trail;

    private void Start()
    {
        trail = gameObject.transform.Find("shoot_trail").GetComponent<TrailRenderer>();
        trail.emitting = false;
    }


    public IEnumerator CreateTrail(Vector3 startPos, RaycastHit ray, float trailTime)
    {
        trail.emitting = true;
        trail.SetPosition(0, startPos);
        //trail.time = trailTime;
        trail.SetPosition(1, ray.point);
        //Vector3.Lerp(startPos, ray.point, trailTime);
        //trailTime += Time.deltaTime / trailTime;
        
        yield return new WaitForSeconds(trailTime);
        trail.Clear();
        trail.emitting = false;
    }
}
