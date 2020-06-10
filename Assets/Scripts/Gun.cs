using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 30f;
    public Camera fpsCam;
    public ParticleSystem MuzzleFlash;
    public GameObject impactEffect;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        MuzzleFlash.Play();
        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position,fpsCam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            GameObject ImpactGO = Instantiate(impactEffect, hit.point, Quaternion.identity);
            //Destroy(ImpactGO);
        }
    }
}
