using UnityEngine;
using UnityEngine.InputSystem;
using Controls;
using System;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "inputReader")]
    public class InputReader : ScriptableObject, GameInput.IGameplayActions, GameInput.IUIActions
    {
        private Controls.GameInput _gameInput;

        private void OnEnable()
        {
            if (_gameInput == null)
            {
                _gameInput = new GameInput();

                _gameInput.Gameplay.SetCallbacks(this);
                _gameInput.UI.SetCallbacks(this);

                SetGameplayActions();
            }
        }
        /// <summary>
        /// Set UI controls uit en gameplay controls aan.
        /// </summary>
        public void SetGameplayActions()
        {
            _gameInput.Gameplay.Enable();
            _gameInput.UI.Disable();
        }
        /// <summary>
        /// Set Gameplay controls uit en UI controls aan.
        /// </summary>
        public void SetUIActions()
        {
            _gameInput.UI.Enable();
            _gameInput.Gameplay.Disable();
        }

        // Gameplay Action Events
        public event Action<Vector2> MoveEvent;
        public event Action<Vector2> LookEvent;
        public event Action<float> NextPreviousEvent;
        public event Action<float> HotbarSelectEvent;
        public event Action UseEvent;
        public event Action CrouchEvent;
        public event Action CrouchCancelEvent;
        public event Action InteractEvent;
        public event Action SprintEvent;
        public event Action SprintCancelEvent;
        public event Action PauseEvent;

        // UI Action Events
        public event Action<Vector2> NavigateEvent;
        public event Action<Vector2> ScrollEvent;
        public event Action ResumeEvent;
        public event Action CancelEvent;
        public event Action ClickEvent;
        public event Action SubmitEvent;


        // Gameplay Actions
        public void OnUse(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                UseEvent?.Invoke();
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                CrouchEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                CrouchCancelEvent.Invoke();
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                PauseEvent?.Invoke();
                SetUIActions();
            }
        }

        public void OnHotbarSelect(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                HotbarSelectEvent?.Invoke(context.ReadValue<float>());
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                InteractEvent?.Invoke();
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                LookEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
           if (context.phase == InputActionPhase.Performed)
           {
               MoveEvent?.Invoke(context.ReadValue<Vector2>());
           }
           else
           {
               MoveEvent?.Invoke(context.ReadValue<Vector2>());
           }
        }
        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                SprintEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                SprintCancelEvent.Invoke();
            }
        }
        public void OnNextPrevious(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                NextPreviousEvent?.Invoke(context.ReadValue<float>());
            }
        }


        // UI Actions
        public void OnNavigate(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                NavigateEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }
        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                ResumeEvent?.Invoke();
                SetGameplayActions();
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                ClickEvent?.Invoke();
            }
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                NavigateEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                ScrollEvent?.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                SubmitEvent?.Invoke();
            }
        }
        void OnDisable()
        {
            _gameInput.UI.Disable();
            _gameInput.Gameplay.Disable();
        }
        void OnDestroy()
        {
            _gameInput.UI.Disable();
            _gameInput.Gameplay.Disable();
        }
    }
}
