using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Client
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _movementSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 100f;
        [SerializeField] private ParticleSystem _attackEffect;
        [SerializeField] private ParticleSystem _areaAttackEffectPrefab;

        [field: SerializeField] public int ManaPoint { get; private set; } = 100;

        public event Action<float> ManaChanged;

        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _attackAction;
        private InputAction _areaAttackAction;
        private CharacterController _characterController;

        private const string MoveAction = "move";
        private const string AttackAction = "fire";
        private const string SecondAttackAction = "secondFire";
        private const float MoveThreshold = 0.01f;

        private static readonly int MoveValue = Animator.StringToHash("MoveValue");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int AreaAttack = Animator.StringToHash("AreaAttack");

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _characterController = GetComponent<CharacterController>();

            _moveAction = _playerInput.actions.FindAction(MoveAction);
            _attackAction = _playerInput.actions.FindAction(AttackAction);
            _areaAttackAction = _playerInput.actions.FindAction(SecondAttackAction);
        }

        private void OnEnable()
        {
            _attackAction.performed += OnAttackPerform;
            _areaAttackAction.performed += OnSecondAttackPerform;
        }

        private void OnDisable()
        {
            _attackAction.performed -= OnAttackPerform;
            _areaAttackAction.performed -= OnSecondAttackPerform;
        }

        private void Update()
        {
            var input = _moveAction.ReadValue<Vector2>();

            HandleMovement(input);
            UpdateMoveAnimation(input);
        }


        private void OnAttackPerform(InputAction.CallbackContext obj)
        {
            _animator.SetTrigger(Attack);
            
            if (_attackEffect.isPlaying)
                _attackEffect.Stop();

            _attackEffect.Play();

            //Todo: magic number -> ability value
            ManaPoint -= 10;
            ManaChanged?.Invoke(ManaPoint);
        }

        private void OnSecondAttackPerform(InputAction.CallbackContext obj)
        {
            _animator.SetTrigger(AreaAttack);

            var effect = Instantiate(_areaAttackEffectPrefab, transform.position, Quaternion.identity);
            effect.Play();

            //Todo: magic number -> ability value
            ManaPoint -= 20;
            ManaChanged?.Invoke(ManaPoint);
        }

        private void HandleMovement(Vector2 input)
        {
            if (Mathf.Abs(input.y) > MoveThreshold)
            {
                var moveDirection = transform.forward;
                _characterController.Move(moveDirection * (_movementSpeed * input.y * Time.deltaTime));
            }

            if (Mathf.Abs(input.x) > MoveThreshold)
            {
                transform.Rotate(Vector3.up, input.x * _rotationSpeed * Time.deltaTime);
            }
        }

        private void UpdateMoveAnimation(Vector2 input)
        {
            var magnitude = input.magnitude;
            _animator.SetFloat(MoveValue, magnitude);

            switch (input.y)
            {
                case < 0:
                    _animator.SetFloat(MoveValue, -1);
                    break;
                case > 0:
                    _animator.SetFloat(MoveValue, 1);
                    break;
                default:
                    _animator.SetFloat(MoveValue, 0);
                    break;
            }
        }
    }
}