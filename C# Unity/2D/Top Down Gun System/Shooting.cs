using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Gun gun;

    Camera cam;
    float bulletCount;

    bool canShoot;
    bool isReloading;
    private void Start()
    {
        cam = Camera.main;
        canShoot = true;
        bulletCount = gun.magSize;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot && !isReloading)
        {
            if (bulletCount == 0)
            {
                StartCoroutine(Reload());
                return;
            }

            ShootBullet();
            StartCoroutine(ShootingDelay());
            bulletCount--;
        }

        if(Input.GetMouseButton(0) && canShoot && !isReloading)
        {
            if(bulletCount == 0)
            {
                StartCoroutine(Reload());
                return;
            }

            ShootBullet();
            StartCoroutine(ShootingDelay());
            bulletCount--;
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    private void ShootBullet()
    {
        Vector2 bulletDirection = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float accuracyMod = Random.Range(-gun.accuracy, gun.accuracy);
        Vector2 finalDir = new Vector2(bulletDirection.normalized.x + accuracyMod, bulletDirection.normalized.y + accuracyMod);

        GameObject b = Instantiate(gun.projectile, transform.position, Quaternion.identity);
        b.GetComponent<Rigidbody2D>().velocity = finalDir * gun.bulletSpeed;
        b.GetComponent<Bullet>().damage = gun.damage;

        Destroy(b, gun.bulletTime);
    }

    IEnumerator ShootingDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(gun.shootDelay);
        canShoot = true;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(gun.reloadSpeed);
        bulletCount = gun.magSize;
        isReloading = false;
    }
}
