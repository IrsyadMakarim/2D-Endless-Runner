using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveAccel;
    public float MaxSpeed;
    public float JumpAccel;
    public float GroundRaycastDistance;
    public LayerMask GroundLayerMask;

    [Header("Scoring")]
    public ScoreController Score;
    public float ScoringRatio;
    private float _lastPositionX;

    private bool _isJumping;
    private bool _isOnGround;
    private Rigidbody2D _rig;
    private Animator _anim;
    private CharacterSoundController _sound;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(_isOnGround)
            {
                _isJumping = true;

                _sound.PlayJumpSound();
            }
        }

        //change anim
        _anim.SetBool("IsOnGround", _isOnGround);

        //calculate score
        int distancePassed = Mathf.FloorToInt(transform.position.x - _lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / ScoringRatio);

        if (scoreIncrement > 0)
        {
            Score.IncreaseCurrentScore(scoreIncrement);
            _lastPositionX += distancePassed;
        }
    }
    private void Start()
    {
        _rig = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _sound = GetComponent<CharacterSoundController>();
    }

    private void FixedUpdate() 
    {
        //raycast ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, GroundRaycastDistance, GroundLayerMask);

        if (hit)
        {
            if (!_isOnGround && _rig.velocity.y <= 0)
            {
                _isOnGround = true;
            }
        }
        else
        {
            _isOnGround = false;
        }

        //calculate velocity vector
        Vector2 velocityVector = _rig.velocity;

        if (_isJumping)
        {
            velocityVector.y += JumpAccel;
            _isJumping = false;
        }
        velocityVector.x = Mathf.Clamp(velocityVector.x + MoveAccel * Time.deltaTime, 0.0f, MaxSpeed);

        _rig.velocity = velocityVector;
    }

    private void OnDrawGizmos() 
    {
        Debug.DrawLine(transform.position, transform.position + -(Vector3.down * GroundRaycastDistance), Color.white);
    }
}
