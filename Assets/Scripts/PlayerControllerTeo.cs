using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControllerTeo : MonoBehaviour
{
    // inputs
    public float horizontalInput;
    public float verticalInput;

    // physics
    public float motorPower = 1100;
    public float mass = 800;
    public float jetForce = 100;
    public float jumpMultiplier = 64;
    public float xTorq = 100;
    public float bulletSpeed = 100;

    // energy management
    private float reactorPower = 5;
    private float generateFrequency = 0.2f;
    private float accGunMax = 100;
    private float accGunCurrent = 100;
    private float accJetMax = 100;
    private float accJetCurrent = 100;
    private float accDroidMax = 100;
    private float accDroidCurrent = 100;

    // object components
    private Rigidbody mechRb;
    public WheelCollider[] wheels;
    public GameObject centerOfMass;
    public GameObject centerOfWeapon;
    public GameObject bullet;

    void Start()
    {
        Physics.gravity = new Vector3(0, -9.81f * 2f, 0);
        mechRb = GetComponent<Rigidbody>();
        mechRb.mass = mass;
        mechRb.centerOfMass = centerOfMass.transform.localPosition;

        InvokeRepeating("GenerateEnergy", 0, generateFrequency);
        InvokeRepeating("Test", 1, 1);
    }

    private void FixedUpdate()
    {
        // shooting
        if (Input.GetKey(KeyCode.E))
        {
            GameObject newBullet = Instantiate(bullet, centerOfWeapon.transform.position + new Vector3(0, 0, 0), centerOfWeapon.transform.rotation) as GameObject;
            Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();
            bulletRb.velocity = transform.forward * bulletSpeed;
        }

        // set inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // mechanics
        if (isOnGround())
        {
            // add motor torque on wheels
            foreach (var wheel in wheels)
            {
                wheel.motorTorque = horizontalInput * ((motorPower * 8) / wheels.Length);
            }

            // jump
            if (Input.GetKey(KeyCode.Space) && accJetCurrent > 20)
            {
                mechRb.AddForce(Vector3.up * jetForce * jumpMultiplier, ForceMode.Impulse);
            }
        }
        else
        {
            // disable motor when in air
            foreach (var wheel in wheels)
            {
                wheel.motorTorque = 0;
            }

            // enable jet for fly or slow fall
            if (Input.GetKey(KeyCode.Space) && accJetCurrent > 0)
            {
                mechRb.AddForce(Vector3.up * (jetForce * 3), ForceMode.Impulse);
                accJetCurrent -= (0.02f * 10);
            }

            // enable jet for horisontal movement in air
            mechRb.AddForce(Vector3.forward * (jetForce) * horizontalInput, ForceMode.Impulse);
        }

        // test for mechanics
        if (Input.GetKey(KeyCode.Q))
        {
            Debug.Log("Q");
            mechRb.AddTorque(xTorq, 0, 0, ForceMode.Impulse);
        }

        // stabilization in air
        if (!isOnGround())
        {
            if (transform.rotation.x > 0.01)
            {
                mechRb.AddTorque(-xTorq, 0, 0, ForceMode.Impulse);
            }
            if (transform.rotation.x < -0.01)
            {
                mechRb.AddTorque(xTorq, 0, 0, ForceMode.Impulse);
            }
            if (transform.rotation.x < 0.01 && transform.rotation.x > -0.01)
            {
                mechRb.angularVelocity = Vector3.zero;
            }
        }

    }
    private void Update()
    {

    }

    // check for one of wheel in ground
    private bool isOnGround()
    {
        bool onGround = false;
        foreach (var wheel in wheels)
        {
            if (wheel.isGrounded)
            {
                onGround = true;
            }
        }
        return onGround;
    }
    private void GenerateEnergy()
    {
        float chargeUnit = reactorPower / (1 / generateFrequency);
        if (accGunCurrent < accGunMax)
        {
            accGunCurrent += chargeUnit;
        }
        else if (accJetCurrent < accJetMax)
        {
            accJetCurrent += chargeUnit;
        }
        else if (accDroidCurrent < accDroidMax)
        {
            accDroidCurrent += chargeUnit;
        }
    }
    private void Test()
    {
        Debug.Log($"Gun: {accGunCurrent}, Jet: {accJetCurrent}, Droid: {accDroidCurrent}");
    }
}
