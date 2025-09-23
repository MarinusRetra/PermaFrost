using UnityEngine;
using UnityEngine.InputSystem;
using Controls;
using System;
using Unity.VisualScripting;

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

        public void SetGameplayActions()
        {
             _gameInput.Gameplay.Enable();
             _gameInput.UI.Disable();
        }

        public void SetUIActions()
        {
            _gameInput.Gameplay.Disable();
            _gameInput.UI.Enable();
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
        public event Action SprintCancelledEvent;
        
        // UI Action Events
        public event Action<Vector2> NavigateEvent;
        public event Action<Vector2> ScrollEvent;
        public event Action PauseEvent;
        public event Action ResumeEvent; 
        public event Action CancelEvent;
        public event Action ClickEvent;
        public event Action SubmitEvent;


        // Gameplay Actions
        public void OnUse(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}");
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}");
        }

        public void OnHotbarSelect(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}, Value: {context.ReadValue<float>()}");
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}");
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // Ik laat de jump event nog even staan voor in het geval we toch wel een jump willen.
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}, Value: {context.ReadValue<Vector2>()}");
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}, Value: {context.ReadValue<Vector2>()}");
        }
        public void OnSprint(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}");
        }
        public void OnNextPrevious(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}, Value: {context.ReadValue<float>()}");
        }


        // UI Actions
        public void OnNavigate(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}, Value: {context.ReadValue<Vector2>()}");

        }
        public void OnCancel(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}");
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}");
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}, Value: {context.ReadValue<Vector2>()}");
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}");
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}, Value: {context.ReadValue<Vector2>()}");
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            Debug.Log($"Phase: {context.phase}");
        }
    }
}
