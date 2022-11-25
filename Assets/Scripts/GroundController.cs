using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public static GroundController Instance;
    public float Speed = 5f;
    public float MaxSpeed = 10f;
    [SerializeField] private PatternGround[] _prefabGroundPatterns;
    [SerializeField] private int _countGroundParts = 5;
    [SerializeField] private Vector3 _startOffset = new Vector3(0f,0f,-7f);
    [SerializeField] private float _deathLengthAfterPlayer = 20f;
    [SerializeField] private float _summaryOffset = 100;
    [SerializeField] private float _timeToBoostSpeed = 60f;
    [SerializeField] private float _boostSpeed = 0.5f;

    List<PatternGround> _road = new List<PatternGround>();

    private float _timer;
    private float _currentOffset;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start() {
        ResetRoad();
        GameManager.Instance.EventManager.OnGameStarted += OnGameStarted;
    }

    private void OnGameStarted() {
        ResetRoad();
        
    }

    private void Update() {

        
        if (GameManager.Instance.IsGameOver) return;

        _timer += Time.deltaTime;
        if(_timer > _timeToBoostSpeed) {
            _timer = 0;
            Speed += _boostSpeed;
        }

        if (Speed == 0) return;

        foreach (var part in _road) {
            //part.GetComponent<Rigidbody>().MovePosition(part.transform.position - Vector3.forward * Speed * Time.deltaTime);
            part.transform.position += Vector3.back * Speed * Time.deltaTime;
        }

        //if (!GameManager.Instance.IsStarted) return;
        if (_road.Count <= 0) return;
        if (_road[0].transform.position.z < -_deathLengthAfterPlayer) {
            Destroy(_road[0].gameObject);
            _currentOffset -= _road[0].OffsetToNextPatternSpawn;
            _road.RemoveAt(0);
        }
        if (_currentOffset < _summaryOffset) {
            CreateNextPartGround();
        }
    }

    public void ResetRoad() {
        Speed = 0;

        while (_road.Count > 0) {
            DeleteFirstPartRoad();
        }

        while (_currentOffset < _summaryOffset) {
            CreateNextPartGround();
        }

        Speed = MaxSpeed;
        //for (int i = 0; i < _countGroundParts; i++) {
        //    CreateNextPartGround();
        //}
    }

    private void DeleteFirstPartRoad() {
        _currentOffset -= _road[0].OffsetToNextPatternSpawn;
        Destroy(_road[0].gameObject);
        _road.RemoveAt(0);
    }

    private PatternGround CreateNextPartGround() {

        
        var randomId = Random.Range(1, _prefabGroundPatterns.Length);
        if (_road.Count == 0) {
            randomId = 0;
        }
        var newPatternGround = _prefabGroundPatterns[randomId];
        var oldPatternGroundOffset = 0f;
        if (_road.Count > 0) {
            oldPatternGroundOffset = _road[_road.Count - 1].GetComponent<PatternGround>().OffsetToNextPatternSpawn;
        }
        var newPos = _startOffset;
        if(_road.Count > 0) {
            newPos = _road[_road.Count - 1].transform.position + new Vector3(0f, 0f, oldPatternGroundOffset);
        }
        
        var newPart = Instantiate(newPatternGround, newPos, Quaternion.identity, transform);
        _road.Add(newPart);
        _currentOffset += newPart.OffsetToNextPatternSpawn;
        return newPart;
    }
}
