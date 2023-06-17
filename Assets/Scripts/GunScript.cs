using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public GameObject gun_module;
    public GameObject bullet;
    public Material trail_mat;
    public Texture trail_tex;
    public float trail_time = 0.1f;
    public float impulse = 20f;
    public float bullet_per_sec = 5;
    public float lifetime = 1;
    private float _timer;

    void Start()
    {
        bullet.GetComponent<TrailRenderer>().time = trail_time;
        trail_mat.mainTexture = trail_tex;
    }

    void Fire()
    {
        _timer += Time.deltaTime;
        if (_timer < 1 / bullet_per_sec) return;
        GameObject b = Instantiate(bullet, gun_module.transform.position, gun_module.transform.rotation);
        b.GetComponent<Rigidbody>().AddForce(transform.forward * impulse, ForceMode.Impulse);
        Destroy(b, lifetime);
        _timer = 0;
    }

    void GunLookAtMouse()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = transform.position.z - Camera.main.transform.position.z; 
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition); 
        var angle = Vector2.Angle(Vector2.right, mousePosition - transform.position);
        transform.eulerAngles = new Vector3(transform.position.y < mousePosition.y ? -angle : angle, 90f, 0f);
    }
    
    void Update()
    {
        GunLookAtMouse();
        if (Input.GetMouseButton(0)) Fire();
    }
}

