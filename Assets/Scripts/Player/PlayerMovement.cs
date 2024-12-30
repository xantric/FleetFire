using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 12f;
    public CharacterController controller;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;

    public LayerMask groundLayer;

    Vector3 velocity;
    bool isGrounded;
    bool isSoundPlaying;
    AudioManager audioManager;

    public float cameraXoffset;
    public float cameraYoffset;
    public float cameraZoffset;

    // Mouse Movement
    public float senstivity;

    float xRotation = 0f;

    private Camera playerCamera;

    public float TopClamp = -60f;
    public float BottomClamp = 60f;

    public GameObject Pistol;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Crosshair _crosshair = FindObjectOfType<Crosshair>();
        if (!IsOwner)
        {
            enabled = false;
            controller.enabled = false;
        }
        else
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x + cameraXoffset, transform.position.y + cameraYoffset, transform.position.z + cameraZoffset);
            playerCamera.transform.SetParent(transform);
            Pistol.transform.SetParent(playerCamera.transform);
            Pistol.GetComponent<WeaponControl>().mainCamera = playerCamera;
            Pistol.GetComponent<WeaponControl>().crossHair = _crosshair.crossHairRectTransform;
        }
        if(base.IsServer)
        {
            controller.enabled = false;
            SetSpawnLocation();
            controller.enabled = true;

        }
    }

    public void SetSpawnLocation()
    {
        SpawnPositionManager _spawnPositionManager = FindObjectOfType<SpawnPositionManager>();
        //Debug.Log(_spawnPositionManager.spawnPoints.Count);
        if (_spawnPositionManager == null || _spawnPositionManager.spawnPoints.Count == 0)
        {
            Debug.LogError("No Spawn Points Available");
            return;
        }
        transform.position = _spawnPositionManager.spawnPoints[_spawnPositionManager.spawnIndex].transform.position;
//        Debug.Log(transform.position);
        _spawnPositionManager.spawnIndex++;
        if (_spawnPositionManager.spawnIndex >= _spawnPositionManager.spawnPoints.Count) _spawnPositionManager.spawnIndex = 0;
    }

    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        isSoundPlaying = false;
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        if (move != new Vector3 (0f,0f,0f) && !audioManager.movementSource.isPlaying) {
            audioManager.movementSource.Play();
            
        }
        else if (audioManager.movementSource.isPlaying)
        {
            audioManager.movementSource.Stop();
        }
       
        controller.Move(move * moveSpeed * Time.deltaTime);
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            audioManager.PlaySFX(audioManager.jump);
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //Mouse Movement Logic

        float mouseX = Input.GetAxis("Mouse X") * senstivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * senstivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, TopClamp, BottomClamp);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }
}
