using UnityEngine;

public class Moving : MonoBehaviour
{
    [field:Header("Params")]
    [SerializeField] float timeDeleyGPS;
    [field: SerializeField]
    public float RotationSpeed { private set; get; }

    [field: Header("Creators")]
    [SerializeField] RoadCreator roadCreator;
    [SerializeField] DriveCreator driveCreator;

    [field: Header("GPS")]
    [SerializeField] Gps Gps;

    [field: Header("Debug")]
    [SerializeField] float targetSpeed;
    [field: SerializeField]
    public float Speed { private set; get; }
    [SerializeField] float acceleration;


    private Vector3 _target;
    public Vector3 Target
    {
        get { return _target; }
    }
    private RoadState _targetState;
    private bool _isThereTarget = false;

    private Vector3 _nextTarget;
    private RoadState _nextTragetState;
    private bool _isThereNextTarget = false;

    void Start()
    {
        if (!Gps.GetNext(out _target, out _targetState))
            return;

        roadCreator.road.MovePoint(2, _target);
        roadCreator.road.MovePoint(3, _target);
        _isThereTarget = true;
        driveCreator.state = _targetState;

        if (Gps.GetNext(out _nextTarget, out _nextTragetState))
        {
            roadCreator.road.AddSegment(_nextTarget);
            _isThereNextTarget = true;
        }

        driveCreator.UpdateRoad();
    }

    private void Update()
    {
        UpdateSpeed(); 
        Move();
    }

    private void UpdateSpeed()
    {
        if ((acceleration > 0 && targetSpeed - Speed > 0.05f) || (acceleration < 0 && Speed - targetSpeed > 0.05f))
            Speed += acceleration * Time.deltaTime;
    }

    private void Move()
    {
        if (_isThereTarget)
        {

            transform.position = Vector3.MoveTowards(transform.position, _target, Speed * Time.deltaTime);
            Rotate();

            if (IsOnTarget())
            {
                if (_isThereNextTarget)
                {
                    AdjustTargetSpeed();
                    _target = _nextTarget;
                    _targetState = _nextTragetState;
                    driveCreator.state = _targetState;
                    _isThereNextTarget = false;
                }
                else
                {
                    _isThereTarget = false;
                    return;
                }
            }

            if (!_isThereNextTarget && Gps.GetNext(out _nextTarget, out _nextTragetState))
            {
                _isThereNextTarget = true;
                roadCreator.road.AddSegment(_nextTarget);
                driveCreator.UpdateRoad();
            }
        }
        else if (Gps.GetNext(out _target, out _targetState))
        {
            _isThereTarget = true;
            driveCreator.state = _targetState;
            roadCreator.road.AddSegment(_target);
            driveCreator.UpdateRoad();
        }

        if (roadCreator.road.NumSegments > 3)
        {
            roadCreator.road.DeleteSegment(0);
            driveCreator.UpdateRoad();
        }
    }

    private void Rotate()
    {
        Vector3 dirMovement = _target - transform.position;
        if (dirMovement != Vector3.zero)
        {
            dirMovement.Normalize();
            Quaternion quaternion = Quaternion.LookRotation(dirMovement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, RotationSpeed * Speed * Time.deltaTime);
        }
    }

    private void AdjustTargetSpeed()
    {
        targetSpeed = ((_nextTarget - _target).magnitude) / timeDeleyGPS;
        acceleration = (targetSpeed - Speed) / (timeDeleyGPS / 2);
    }

    private bool IsOnTarget() => transform.position.x == _target.x && transform.position.z == _target.z;
}
