
using System;
using System.Collections;
using UnityEngine;

public enum HitSide
{
    Left,
    Right,
    Up,
    Down,
}


public class PlayerController : MonoBehaviour
{
    private const string AnimJumpField = "IsJump";
    private const string LeftHitField = "IsHitLeft";
    private const string RightHitField = "IsHitRight";
    private const string RollField = "IsRoll";
    private const string FallField = "IsFall";
    [SerializeField] private float _speedSwitchStep = 1f;
    [SerializeField] private float _speedJumpStep = 1f;
    [SerializeField] private float _moveToRollTime;
    [SerializeField] private float _jumpHeight = 5;
    [SerializeField] private float _defaultPosY = 0f;

    [SerializeField] private Vector3 _newVelocity;
    [SerializeField] private PlayerTrigger _playerTrigger;
    [SerializeField] private Animator _animator;

    [SerializeField] private Line _currentLine = Line.Middle;
    [SerializeField] private State _currentState = State.Moving;
    [SerializeField] private Swipe _swipe = Swipe.None;
    [SerializeField] private float _lengthToSwipe;
    [SerializeField] private float _sizeLine;

    [SerializeField] private bool _isGrounded, _isOnPhysics, _isDead;

    [SerializeField] private int _health = 2;
    [SerializeField] private float _timeToNextHit = 0.5f;

    private Rigidbody _rb;
    private Vector3 _oldMousePosition;

    //Для отслеживания. Потом убрать
    [SerializeField] private float _newPosX, _newPosY;
    private float _oldPosX;
    private Coroutine _jumpOrRollCoroutine, _regerateHealth;
    private float _currentNewX, _currentNewY, _timerHit;

    void Start() {
        Application.targetFrameRate = 60;
        _currentLine = Line.Middle;
        _swipe = Swipe.None;
        _rb = GetComponent<Rigidbody>();
        _animator.SetBool(FallField, false);
    }

    void Update() {
        if (_isGrounded) {
            SetCurrentState(State.Moving);
        }

        CheckSwipe();
        Switch();
        Jump();
        Roll();
        Fall();

        if (_isOnPhysics && _isGrounded && _currentState != State.Jumping) {
            _newPosY = _rb.position.y;
        }
        _currentNewY = Mathf.MoveTowards(transform.position.y, _newPosY, _speedJumpStep);
        _currentNewX = Mathf.MoveTowards(transform.position.x, _newPosX, _speedSwitchStep);
        //Move();
        _timerHit += Time.deltaTime;
    }

    private void LateUpdate() {
        Move();

        if (!_isDead) {
            _animator.SetBool(RightHitField, false);
            _animator.SetBool(LeftHitField, false);
        }
    }

    private void Move() {
        if (_currentState != State.OffMoving) {
            _newVelocity = new Vector3(_currentNewX, _currentNewY);
            transform.position = _newVelocity;
        }
    }

    private void CheckSwipe() {
        float directionX = 0;
        if (Input.GetMouseButtonDown(0)) {
            _oldMousePosition = Input.mousePosition;
        }

        //if (Input.GetMouseButton(0)) {
        //    var swipeVector = Input.mousePosition - _oldMousePosition;
        //    print("Length swipe vector = " + swipeVector.magnitude);
        //}

        if (Input.GetMouseButtonUp(0)) {
            var swipeVector = Input.mousePosition - _oldMousePosition;
            //print("Swipe Vector " + swipeVector);
            if (swipeVector.magnitude < _lengthToSwipe) return;

            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y)) {
                directionX = swipeVector.normalized.x;
                if (directionX > 0) _swipe = Swipe.Right;
                else if (directionX < 0) _swipe = Swipe.Left;
            }
            else {
                if (swipeVector.y > 0) {
                    _swipe = Swipe.Up;
                }
                else {
                    _swipe = Swipe.Down;
                }
            }

            //print(_swipe);
        }
    }

    private void Switch() {
        if (GameManager.Instance.IsGameOver) return;
        if (_swipe == Swipe.Left) {
            //print("Left side");
            SetDefaultSwipe();
            SwitchLine(Line.Left);
        }

        if (_swipe == Swipe.Right) {
            //print("Right Side");
            SetDefaultSwipe();
            SwitchLine(Line.Right);
        }
    }

    private void Fall() {

        if (_currentState == State.Falling) {
            _animator.SetBool(FallField, true);
            _newPosY -= _jumpHeight * Time.deltaTime;
            _newPosY = Mathf.Clamp(_newPosY, 0, _jumpHeight);
        }
        else {
            _animator.SetBool(FallField, false);
        }
    }

    private void Roll() {
        if (_swipe == Swipe.Down) {
            SetDefaultSwipe();
            _newPosY = _defaultPosY;
            _jumpOrRollCoroutine = StartCoroutine(DoRoll());

        }
    }

    private IEnumerator DoRoll() {
        SetCurrentState(State.Rolling);
        _playerTrigger.SetColliderToRoll(true);
        _animator.SetBool(RollField, true);
        yield return new WaitForSeconds(_moveToRollTime);
        _animator.SetBool(RollField, false);
        _playerTrigger.SetColliderToRoll(false);
        SetCurrentState(State.Moving);
    }

    private void Jump() {

        if (_isGrounded && _swipe == Swipe.Up) {

            SetDefaultSwipe();
            SetCurrentState(State.Jumping);
            StopRollCorutine();

            _newPosY = transform.position.y + _jumpHeight;
            _animator.SetBool(AnimJumpField, true);
        }

        if (!_isGrounded) {
            SetCurrentState(State.Falling);
            _animator.SetBool(AnimJumpField, false);
        }
    }

    private void StopRollCorutine() {
        if (_jumpOrRollCoroutine != null)
            StopCoroutine(_jumpOrRollCoroutine);
        _playerTrigger.SetColliderToRoll(false);
        _animator.SetBool(RollField, false);
    }


    private void SwitchLine(Line value) {
        _oldPosX = _newPosX;
        switch (value) {
            case Line.Left:
                if (_currentLine != Line.Left)

                    _newPosX -= _sizeLine;
                break;
            case Line.Right:
                if (_currentLine != Line.Right)
                    _newPosX += _sizeLine;
                break;
        }

        if (_newPosX > 0) {
            _currentLine = Line.Right;
        }
        else if (_newPosX < 0) {
            _currentLine = Line.Left;
        }
        else {
            _currentLine = Line.Middle;
        }

    }

    private void Hit(HitSide hitSide) {

        if (hitSide == HitSide.Down || hitSide == HitSide.Up) {
            Death();
            return;
        }
        
        if (hitSide == HitSide.Left) {
            _animator.SetBool(LeftHitField, true);
            print("Left Hit");
            _newPosX = _oldPosX;
        }
        else if (hitSide == HitSide.Right) {
            _animator.SetBool(RightHitField, true);
            print("Right Hit");
            _newPosX = _oldPosX;
        }

        if (_regerateHealth != null) {
            StopCoroutine(_regerateHealth);
        }

        _health--;
        if(_health <= 0) {
            
            Death();
        }
        else {
            _regerateHealth = StartCoroutine(RegenerateHealth());
        }
    }

    private void Death() {
        _isDead = true;
        _animator.SetBool("IsDeath", _isDead);
        GameManager.Instance.EventManager.PlayerDied();
    }

    IEnumerator RegenerateHealth() {
        yield return new WaitForSeconds(3f);
        _health++;
    }

    private void OnCollisionEnter(Collision collision) {
        foreach (var contact in collision.contacts) {

            if (_timerHit < _timeToNextHit) continue;
            _timerHit = 0f;
            

            var offsetX = contact.point.x - transform.position.x;
            var angle = Vector3.Angle(Vector3.down, contact.point);
            
            if (angle > 100f) {
                print("hit " + collision.rigidbody.name + " angle" + angle);
                if (offsetX < -0.2f) {
                    Hit(HitSide.Left);
                }
                else if (offsetX > 0.2f) {
                    Hit(HitSide.Right);
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Obstacle obstacle)) {
            Hit(HitSide.Down);
            return;
        }
    }

    private void OnCollisionStay(Collision collision) {
        _isGrounded = true;
        if (collision.collider.TryGetComponent(out Tramplin tramplin)) {
            _isOnPhysics = true;
        }

    }

    private void OnCollisionExit(Collision collision) {
        _isGrounded = false;
        if (collision.collider.TryGetComponent(out Tramplin tramplin)) {
            _isOnPhysics = false;
        }
    }

    private void SetDefaultSwipe() {
        _swipe = Swipe.None;
    }

    private void SetCurrentState(State value) {
        _currentState = value;
    }

    public void ResetPlayer() {
        _isDead = false;
        _currentLine = Line.Middle;
        _currentState = State.Moving;
        gameObject.SetActive(false);
    }
}
