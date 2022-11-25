using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum Line
{
    Left,
    Middle,
    Right
}

public enum State
{
    Moving,
    SwitchingLine,
    Jumping,
    Rolling,
    Falling,
    OffMoving
}

public enum Swipe
{
    None,
    Left,
    Right,
    Up,
    Down
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _timeToSwitchSide;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _moveToRollTime;

    [SerializeField] private PlayerTrigger _playerTrigger;

    [SerializeField] private Line _currentSide;
    [SerializeField] private State _currentState;
    [SerializeField] private float _lengthToSwipe = 10f;
    [SerializeField] private float _sizeLine = 5f;

    private Rigidbody _rb;
    private CapsuleCollider _collider;
    private Coroutine _switchingCoroutine;
    private Coroutine _jumpOrRollCoroutine;

    private Vector3 _oldMousePosition;
    private float _oldNextPosX, _defaultHeightCollider, _defaultPosY;
    private float _jumpToRollTime;
    private bool _isMoving;

    private void Start() {
        Application.targetFrameRate = 30;
        _currentSide = Line.Middle;
        _currentState = State.Moving;
        //_collider = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        //_defaultHeightCollider = _collider.height;
        _defaultPosY = transform.position.y;
        _jumpToRollTime = _moveToRollTime * 2;
    }

    private void Update() {
        
        if (_currentState != State.SwitchingLine) {
            CheckSwipe();
            transform.position += Vector3.forward * _speed * Time.deltaTime;
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
                CalculateLine(directionX);
            }
            else {
                if (swipeVector.y > 0) {
                    Jump();
                }
                else {
                    Roll();
                }
            }
        }
    }

    private void CalculateLine(float x) {
        if (x < 0 && _currentSide != Line.Left) {
            //print("Left side");
            SwitchLine(Line.Left);
        }

        if (x > 0 && _currentSide != Line.Right) {
            //print("Right Side");
            SwitchLine(Line.Right);
        }
    }

    private void Jump() {
        if (_currentState == State.SwitchingLine || _currentState == State.Jumping) return;

        print("Jump");
        _isMoving = false;

        if (_jumpOrRollCoroutine != null) {
            StopCoroutine(_jumpOrRollCoroutine);
            SetDefaultHeightHollider();
        }

        _jumpOrRollCoroutine = StartCoroutine(DoJump());
    }

    private IEnumerator DoJump() {
        SetCurrentState(State.Jumping);
        _rb.AddForce(transform.up * _jumpForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
        //Желательно прописать троекторию полета...
        yield return new WaitForSeconds(1f);
        SetDefaultHeightHollider();
        SetCurrentState(State.Moving);
    }

    private void Roll() {

        if (_currentState == State.SwitchingLine) return;

        
        print("Roll");

        if (_jumpOrRollCoroutine != null)
            StopCoroutine(_jumpOrRollCoroutine);

        _jumpOrRollCoroutine = StartCoroutine(DoRoll());
    }

    private IEnumerator DoRoll() {
        var rollTime = _moveToRollTime;
        if (_currentState == State.Jumping)
            rollTime = _jumpToRollTime; 

        SetCurrentState(State.Rolling);
        _playerTrigger.SetColliderToRoll(true);

        var endPosition = new Vector3(transform.position.x, _defaultPosY, transform.position.z);
        endPosition += GetNextForwardVectorForTime(_speed, rollTime);

        for (float t = 0; t <= 1f; t += Time.deltaTime / rollTime) {
            transform.position = Vector3.Lerp(transform.position, endPosition, t);
            yield return null;
        }
        //_rb.AddForce(-transform.up * _jumpForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
        //Желательно прописать троекторию полета...
        SetDefaultHeightHollider();
        SetCurrentState(State.Moving);
    }

    private void SetDefaultHeightHollider() {
        _playerTrigger.SetColliderToRoll(false);
    }

    

    private void SwitchLine(Line value) {

        var nextPos = new Vector3(_oldNextPosX, transform.position.y, transform.position.z);
        switch (value) {
            case Line.Left:
                nextPos += GetLineVector(Vector3.left);
                break;
            case Line.Right:
                nextPos += GetLineVector(Vector3.right);
                break;
        }

        if (_switchingCoroutine != null)
            StopCoroutine(_switchingCoroutine);

        _switchingCoroutine = StartCoroutine(Move(nextPos));

        if (nextPos.x > 0) {
            _currentSide = Line.Right;
        }
        else if (nextPos.x < 0) {
            _currentSide = Line.Left;
        }
        else {
            _currentSide = Line.Middle;
        }
        _oldNextPosX = nextPos.x;
    }


    private IEnumerator Move(Vector3 endPosition) {
        SetCurrentState(State.SwitchingLine);
        //var additionalVector = GetNextForwardVectorForTime(_speed, _timeToSwitchSide);
        endPosition += GetNextForwardVectorForTime(_speed, _timeToSwitchSide);
        for (float t = 0; t <= 1f; t+= Time.deltaTime / _timeToSwitchSide) {
            transform.position = Vector3.Lerp(transform.position, endPosition, t);
            yield return null;
        }
        _currentState = State.Moving;
    }

    private Vector3 GetLineVector(Vector3 direction) {
        return direction * _sizeLine;
    }

    private void SetCurrentState(State value) {
        _currentState = value;
    }

    private Vector3 GetNextForwardVectorForTime(float speed, float timeInSeconds) {
        return Vector3.forward * speed * Time.deltaTime * Application.targetFrameRate * timeInSeconds;
    }
}
