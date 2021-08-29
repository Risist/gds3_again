using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class BulletDirectionPointer : MonoBehaviour
{
    public float playerDistanceActivation;
    public float alphaSpeed;
    public float rotationSpeed;

    MeshRenderer _renderer;
    Transform _player;
    Collider _collider;
    HeadBulletController _headBulletController;

    void SetPlayer(Transform playerTransform)
    {
        if (!playerTransform)
        {
            _player = null;

            return;
        }

        bool hadPreviouslyPlayer = _player;

        _player = playerTransform.transform;

        if (!hadPreviouslyPlayer)
        {
            Vector2 toPlayer = (_player.position - transform.position).To2D();
            float desiredAngle = Vector2.SignedAngle(-toPlayer, Vector2.up);
            transform.eulerAngles = new Vector3(0, desiredAngle, 0);
        }
    }

    private void Start()
    {
        _headBulletController = GetComponentInParent<HeadBulletController>();
        _renderer = GetComponentInChildren<MeshRenderer>();
        _collider = transform.parent.GetComponent<Collider>();

        /*SetPlayer(GameObject.FindGameObjectWithTag("Player")?.transform);

        if (_player)
        {
            Vector2 toPlayer = (_player.position - transform.position).To2D();
            float desiredAngle = Vector2.SignedAngle(-toPlayer, Vector2.up);
            transform.eulerAngles = new Vector3(0, desiredAngle, 0);
        }*/

        Color cl = _renderer.material.color;
        cl.a = 0;
        _renderer.material.color = cl;
    }

    private void OnTriggerEnter(Collider other)
    {
        //var inputHolder = other.GetComponent<InputHolder>();
        //if (inputHolder && (_player == null || (_player != other.transform && _player.gameObject.tag == "Player")))
        if(other.tag == "Player")
        {
            SetPlayer(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var inputHolder = other.GetComponent<InputHolder>();
        if (inputHolder && _player == other.transform)
        {
            SetPlayer(null);
        }
    }

    private void LateUpdate()
    {
        bool activate = _collider.isTrigger && _player && _headBulletController.instigator != _player.gameObject;
        

        if(_player)
        {
            //Vector2 toPlayer = (_player.position - transform.position).To2D();
            Vector2 toPlayer = -_player.forward.To2D();
            float distanceSq = toPlayer.sqrMagnitude;
            activate &= distanceSq < playerDistanceActivation.Sq();

            float desiredAngle = Vector2.SignedAngle(-toPlayer, Vector2.up);
            //float angle = Mathf.LerpAngle(transform.eulerAngles.y, desiredAngle, rotationSpeed * Time.deltaTime);
            float angle = desiredAngle;
            transform.eulerAngles = new Vector3(0, angle, 0);
        }

        Color cl = _renderer.material.color;
        float desiredA = activate ? 1.0f: 0.0f;
        cl.a = Mathf.Lerp(cl.a, desiredA, alphaSpeed * Time.deltaTime);
        _renderer.material.color = cl;

        _renderer.enabled = cl.a > 0.05f;
        

    }
}

