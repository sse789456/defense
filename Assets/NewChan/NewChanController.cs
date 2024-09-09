using System.Collections;
using UnityEngine;

public class HashingValue
{
    public static readonly int SpeedStringHash = Animator.StringToHash("Speed");
    public static readonly int JumpStringHash = Animator.StringToHash("JUMP00_1");
    public static readonly int PunchLeftStringHash = Animator.StringToHash("PUNCH_L");
    public static readonly int PunchRightStringHash = Animator.StringToHash("PUNCH_R");
    public static readonly int KickLeftStringHash = Animator.StringToHash("KICK_1_L");
    public static readonly int KickRightStringHash = Animator.StringToHash("KICK_1_R");

    public static readonly int[] ComboAnimations = {
        PunchLeftStringHash,
        PunchRightStringHash,
        KickLeftStringHash,
        KickRightStringHash
    };

    public static int GetComboAnimNumber(int num) =>
        ComboAnimations.Length > num
            ? ComboAnimations[num]
            : ComboAnimations[^1];
}

public class NewChanController : MonoBehaviour
{
    public float Speed = 0.0f;
    public float RotSpeedDegree = 0.0f;
    public float JumpPower = 0.0f;

    private float horizontal = 0.0f;
    private float vertical = 0.0f;

    private Rigidbody _rigidbody;
    private Animator _animator;
    private AnimationWrapper _animationWrapper;

    private Coroutine _comboProcessCoroutine = null;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (!_rigidbody)
            Debug.LogError("Rigidbody component not found");

        _animator = GetComponent<Animator>();
        if (!_animator)
            Debug.LogError("Animator component not found");

        _animationWrapper = AnimationWrapper.NewAnimationWrapper(this);
        if (_animationWrapper == null)
            Debug.LogError("Failed to create AnimationWrapper");
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody?.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            PlayAnimation(HashingValue.JumpStringHash);
        }

        ComboProcess();
    }

    IEnumerator ComboProcess_Loop()
    {
        int currentComboNumber = 0;
        PlayAnimation(HashingValue.ComboAnimations[currentComboNumber]);

        yield return null;

        while (_animationWrapper != null)
        {
            if (_animationWrapper.IsName(HashingValue.GetComboAnimNumber(currentComboNumber)))
            {
                if (Input.GetKeyDown(KeyCode.P) &&
                    _animationWrapper.CanComboState(HashingValue.GetComboAnimNumber(currentComboNumber)))
                {
                    currentComboNumber++;
                    if (HashingValue.ComboAnimations.Length > currentComboNumber)
                        PlayAnimation(HashingValue.GetComboAnimNumber(currentComboNumber));
                }

                yield return null;
            }
            else
            {
                break;
            }
        }

        _comboProcessCoroutine = null;
    }

    void ComboProcess()
    {
        if (_comboProcessCoroutine == null)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _comboProcessCoroutine = StartCoroutine(ComboProcess_Loop());
            }
        }
    }

    void PlayAnimation(int animationNameHash, float speed = 1.0f)
    {
        _animationWrapper?.PlayAnimation(animationNameHash, speed);
    }

    private void FixedUpdate()
    {
        if (vertical != 0.0f)
        {
            float delta = vertical * Speed * Time.fixedDeltaTime;
            Vector3 curPos = transform.position;
            Vector3 nextPos = curPos + transform.forward * delta;
            _rigidbody?.MovePosition(nextPos);
        }

        _animationWrapper?.FixedUpdate(vertical);

        if (horizontal != 0.0f)
        {
            float delta = horizontal * RotSpeedDegree * Time.fixedDeltaTime;
            Quaternion curQuat = Quaternion.LookRotation(transform.forward);
            Quaternion nextQuat = Quaternion.RotateTowards(curQuat, curQuat * Quaternion.Euler(0, delta, 0), delta);
            _rigidbody?.MoveRotation(nextQuat);
        }
    }

    private class AnimationWrapper
    {
        private NewChanController _newChanController;
        private Animator _animator;
        private Coroutine _currentAnimationCoroutine;

        private AnimationWrapper(NewChanController newChanController)
        {
            _newChanController = newChanController;
            _animator = _newChanController.GetComponent<Animator>();
            if (!_animator)
                Debug.LogError("Animator component not found");
        }

        public static AnimationWrapper NewAnimationWrapper(NewChanController newChanController)
        {
            return new AnimationWrapper(newChanController);
        }

        public void FixedUpdate(float vertical)
        {
            _animator?.SetFloat(HashingValue.SpeedStringHash, vertical);
        }

        public void PlayAnimation(int animationNameHash, float speed = 1.0f)
        {
            if (_currentAnimationCoroutine != null)
            {
                _newChanController.StopCoroutine(_currentAnimationCoroutine);
                _currentAnimationCoroutine = null;
                if (_animator != null)
                {
                    _animator.speed = 1.0f;
                }
            }

            _currentAnimationCoroutine = _newChanController.StartCoroutine(PlayAnimation_Internal(animationNameHash, speed));
        }

        public bool CanComboState(int checkAnimationName)
        {
            if (IsName(checkAnimationName))
            {
                var stateInfo = _animator?.GetCurrentAnimatorStateInfo(0);
                return stateInfo?.normalizedTime >= 0.7f;
            }

            return false;
        }

        public bool IsName(int targetAnimationHash)
        {
            var stateInfo = _animator?.GetCurrentAnimatorStateInfo(0);
            var nextStateInfo = _animator?.GetNextAnimatorStateInfo(0);
            return stateInfo?.fullPathHash == targetAnimationHash || nextStateInfo?.fullPathHash == targetAnimationHash;
        }

        IEnumerator PlayAnimation_Internal(int animationNameHash, float speed)
        {
            if (_animator != null)
            {
                _animator.CrossFade(animationNameHash, 0.1f, 0);
                _animator.speed = speed;
            }

            yield return null;

            while (true)
            {
                var stateInfo = _animator?.GetCurrentAnimatorStateInfo(0);
                if (stateInfo?.fullPathHash == animationNameHash)
                {
                    yield return null;
                }
                else
                {
                    break;
                }
            }

            if (_animator != null)
            {
                _animator.speed = 1.0f;
            }
            _currentAnimationCoroutine = null;
        }
    }
}
