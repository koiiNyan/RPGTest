// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Units/Players/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace RPG.Units.Player
{
    public class @PlayerControls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Unit"",
            ""id"": ""1d7b3360-0533-4993-b3d8-fc601c31545a"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""b73acc18-2648-41b6-a8fc-7c19d1841129"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MainAction"",
                    ""type"": ""Button"",
                    ""id"": ""1d875192-63eb-4c0a-bd2f-5fedb4dbe504"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AdditionalAction"",
                    ""type"": ""Button"",
                    ""id"": ""81bcd95e-c9aa-4bfc-a3f8-e90a648e80ef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LockTarget"",
                    ""type"": ""Button"",
                    ""id"": ""4b7b2dff-5b33-4b87-95c6-608217341f7a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Armed"",
                    ""type"": ""Button"",
                    ""id"": ""ebae046c-9d77-4abf-8b27-2db598f100b6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""1f2abf2d-a748-4394-a7d1-f6f875e5a34c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""e28bb2ee-d1fe-4f40-abc9-f4016f062396"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""c37fcb4f-d2a7-4732-af45-fa6e51b7d7fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Quests"",
                    ""type"": ""Button"",
                    ""id"": ""b83d0162-1354-4091-8c63-97b670818f39"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Levels"",
                    ""type"": ""Button"",
                    ""id"": ""ae1bfe7a-07be-429a-b6e3-068cb3e1b567"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory"",
                    ""type"": ""Button"",
                    ""id"": ""a5d885d4-8785-4ee5-8c13-f09c1c9469d4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Skill1"",
                    ""type"": ""Button"",
                    ""id"": ""d6fdb5fb-fe4b-4535-b2ea-013b8a5e696c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""b416cdff-5cc9-424b-8c31-b81ad5aeae4a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""0680218d-69c3-44db-bb9c-89ff31da298f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""547502e4-b70a-4519-b5fc-b571a8e95094"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""000108aa-5a11-4539-b23d-78e270586d18"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5c2ef8cc-510e-4067-8ff9-81ec72bc9954"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""122901db-d4bc-4368-acff-3df1e9a7e889"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""823b28a7-06f8-4956-a621-5bd31e126f69"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MainAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""117f3af2-2861-4452-91f7-1c5514b26528"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AdditionalAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a7934bbd-1931-4922-9b50-238e8582caf1"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LockTarget"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f5a6b70d-3490-49de-8074-5e9ec8368b5e"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Armed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0247e66b-7a70-4863-96e2-7a2a693500b6"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c9eb181-b9e8-4c8f-81cd-9c66edad6126"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2e3a607e-c252-4353-aa96-c7424e4c401a"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c0c2098c-e964-4072-8de9-7f5a95b76668"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Quests"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""42a55f26-eeab-46da-825c-8b41d85c2702"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Levels"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ae4e8950-df89-4b92-a6ce-a699d97fbb59"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0ee15950-ffbe-40a1-a24d-edae46ede8f3"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d6eb5ecc-db8a-4cc7-9319-304fa7bfbc0d"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Camera"",
            ""id"": ""094e6913-82df-4728-8d52-7aea18104c53"",
            ""actions"": [
                {
                    ""name"": ""Delta"",
                    ""type"": ""Value"",
                    ""id"": ""dfacba78-44f3-457c-abe4-c2200daa01e0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ab981fba-2dbe-4899-b0fd-53875d5190a2"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Delta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""a549ce8b-6de4-47a7-81c0-7c415c2a41f2"",
            ""actions"": [
                {
                    ""name"": ""Skip"",
                    ""type"": ""Button"",
                    ""id"": ""17cd8d04-1eab-491e-924f-6b3914ed7c9c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a41a7ff4-69ee-4bfb-a6b4-ede82d221a19"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Unit
            m_Unit = asset.FindActionMap("Unit", throwIfNotFound: true);
            m_Unit_Move = m_Unit.FindAction("Move", throwIfNotFound: true);
            m_Unit_MainAction = m_Unit.FindAction("MainAction", throwIfNotFound: true);
            m_Unit_AdditionalAction = m_Unit.FindAction("AdditionalAction", throwIfNotFound: true);
            m_Unit_LockTarget = m_Unit.FindAction("LockTarget", throwIfNotFound: true);
            m_Unit_Armed = m_Unit.FindAction("Armed", throwIfNotFound: true);
            m_Unit_Sprint = m_Unit.FindAction("Sprint", throwIfNotFound: true);
            m_Unit_Crouch = m_Unit.FindAction("Crouch", throwIfNotFound: true);
            m_Unit_Jump = m_Unit.FindAction("Jump", throwIfNotFound: true);
            m_Unit_Quests = m_Unit.FindAction("Quests", throwIfNotFound: true);
            m_Unit_Levels = m_Unit.FindAction("Levels", throwIfNotFound: true);
            m_Unit_Inventory = m_Unit.FindAction("Inventory", throwIfNotFound: true);
            m_Unit_Skill1 = m_Unit.FindAction("Skill1", throwIfNotFound: true);
            m_Unit_Pause = m_Unit.FindAction("Pause", throwIfNotFound: true);
            // Camera
            m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
            m_Camera_Delta = m_Camera.FindAction("Delta", throwIfNotFound: true);
            // UI
            m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
            m_UI_Skip = m_UI.FindAction("Skip", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Unit
        private readonly InputActionMap m_Unit;
        private IUnitActions m_UnitActionsCallbackInterface;
        private readonly InputAction m_Unit_Move;
        private readonly InputAction m_Unit_MainAction;
        private readonly InputAction m_Unit_AdditionalAction;
        private readonly InputAction m_Unit_LockTarget;
        private readonly InputAction m_Unit_Armed;
        private readonly InputAction m_Unit_Sprint;
        private readonly InputAction m_Unit_Crouch;
        private readonly InputAction m_Unit_Jump;
        private readonly InputAction m_Unit_Quests;
        private readonly InputAction m_Unit_Levels;
        private readonly InputAction m_Unit_Inventory;
        private readonly InputAction m_Unit_Skill1;
        private readonly InputAction m_Unit_Pause;
        public struct UnitActions
        {
            private @PlayerControls m_Wrapper;
            public UnitActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Unit_Move;
            public InputAction @MainAction => m_Wrapper.m_Unit_MainAction;
            public InputAction @AdditionalAction => m_Wrapper.m_Unit_AdditionalAction;
            public InputAction @LockTarget => m_Wrapper.m_Unit_LockTarget;
            public InputAction @Armed => m_Wrapper.m_Unit_Armed;
            public InputAction @Sprint => m_Wrapper.m_Unit_Sprint;
            public InputAction @Crouch => m_Wrapper.m_Unit_Crouch;
            public InputAction @Jump => m_Wrapper.m_Unit_Jump;
            public InputAction @Quests => m_Wrapper.m_Unit_Quests;
            public InputAction @Levels => m_Wrapper.m_Unit_Levels;
            public InputAction @Inventory => m_Wrapper.m_Unit_Inventory;
            public InputAction @Skill1 => m_Wrapper.m_Unit_Skill1;
            public InputAction @Pause => m_Wrapper.m_Unit_Pause;
            public InputActionMap Get() { return m_Wrapper.m_Unit; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(UnitActions set) { return set.Get(); }
            public void SetCallbacks(IUnitActions instance)
            {
                if (m_Wrapper.m_UnitActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnMove;
                    @MainAction.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnMainAction;
                    @MainAction.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnMainAction;
                    @MainAction.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnMainAction;
                    @AdditionalAction.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnAdditionalAction;
                    @AdditionalAction.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnAdditionalAction;
                    @AdditionalAction.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnAdditionalAction;
                    @LockTarget.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnLockTarget;
                    @LockTarget.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnLockTarget;
                    @LockTarget.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnLockTarget;
                    @Armed.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnArmed;
                    @Armed.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnArmed;
                    @Armed.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnArmed;
                    @Sprint.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnSprint;
                    @Sprint.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnSprint;
                    @Sprint.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnSprint;
                    @Crouch.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnCrouch;
                    @Crouch.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnCrouch;
                    @Crouch.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnCrouch;
                    @Jump.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnJump;
                    @Quests.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnQuests;
                    @Quests.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnQuests;
                    @Quests.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnQuests;
                    @Levels.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnLevels;
                    @Levels.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnLevels;
                    @Levels.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnLevels;
                    @Inventory.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnInventory;
                    @Inventory.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnInventory;
                    @Inventory.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnInventory;
                    @Skill1.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnSkill1;
                    @Skill1.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnSkill1;
                    @Skill1.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnSkill1;
                    @Pause.started -= m_Wrapper.m_UnitActionsCallbackInterface.OnPause;
                    @Pause.performed -= m_Wrapper.m_UnitActionsCallbackInterface.OnPause;
                    @Pause.canceled -= m_Wrapper.m_UnitActionsCallbackInterface.OnPause;
                }
                m_Wrapper.m_UnitActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @MainAction.started += instance.OnMainAction;
                    @MainAction.performed += instance.OnMainAction;
                    @MainAction.canceled += instance.OnMainAction;
                    @AdditionalAction.started += instance.OnAdditionalAction;
                    @AdditionalAction.performed += instance.OnAdditionalAction;
                    @AdditionalAction.canceled += instance.OnAdditionalAction;
                    @LockTarget.started += instance.OnLockTarget;
                    @LockTarget.performed += instance.OnLockTarget;
                    @LockTarget.canceled += instance.OnLockTarget;
                    @Armed.started += instance.OnArmed;
                    @Armed.performed += instance.OnArmed;
                    @Armed.canceled += instance.OnArmed;
                    @Sprint.started += instance.OnSprint;
                    @Sprint.performed += instance.OnSprint;
                    @Sprint.canceled += instance.OnSprint;
                    @Crouch.started += instance.OnCrouch;
                    @Crouch.performed += instance.OnCrouch;
                    @Crouch.canceled += instance.OnCrouch;
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                    @Quests.started += instance.OnQuests;
                    @Quests.performed += instance.OnQuests;
                    @Quests.canceled += instance.OnQuests;
                    @Levels.started += instance.OnLevels;
                    @Levels.performed += instance.OnLevels;
                    @Levels.canceled += instance.OnLevels;
                    @Inventory.started += instance.OnInventory;
                    @Inventory.performed += instance.OnInventory;
                    @Inventory.canceled += instance.OnInventory;
                    @Skill1.started += instance.OnSkill1;
                    @Skill1.performed += instance.OnSkill1;
                    @Skill1.canceled += instance.OnSkill1;
                    @Pause.started += instance.OnPause;
                    @Pause.performed += instance.OnPause;
                    @Pause.canceled += instance.OnPause;
                }
            }
        }
        public UnitActions @Unit => new UnitActions(this);

        // Camera
        private readonly InputActionMap m_Camera;
        private ICameraActions m_CameraActionsCallbackInterface;
        private readonly InputAction m_Camera_Delta;
        public struct CameraActions
        {
            private @PlayerControls m_Wrapper;
            public CameraActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Delta => m_Wrapper.m_Camera_Delta;
            public InputActionMap Get() { return m_Wrapper.m_Camera; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
            public void SetCallbacks(ICameraActions instance)
            {
                if (m_Wrapper.m_CameraActionsCallbackInterface != null)
                {
                    @Delta.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnDelta;
                    @Delta.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnDelta;
                    @Delta.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnDelta;
                }
                m_Wrapper.m_CameraActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Delta.started += instance.OnDelta;
                    @Delta.performed += instance.OnDelta;
                    @Delta.canceled += instance.OnDelta;
                }
            }
        }
        public CameraActions @Camera => new CameraActions(this);

        // UI
        private readonly InputActionMap m_UI;
        private IUIActions m_UIActionsCallbackInterface;
        private readonly InputAction m_UI_Skip;
        public struct UIActions
        {
            private @PlayerControls m_Wrapper;
            public UIActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Skip => m_Wrapper.m_UI_Skip;
            public InputActionMap Get() { return m_Wrapper.m_UI; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
            public void SetCallbacks(IUIActions instance)
            {
                if (m_Wrapper.m_UIActionsCallbackInterface != null)
                {
                    @Skip.started -= m_Wrapper.m_UIActionsCallbackInterface.OnSkip;
                    @Skip.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnSkip;
                    @Skip.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnSkip;
                }
                m_Wrapper.m_UIActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Skip.started += instance.OnSkip;
                    @Skip.performed += instance.OnSkip;
                    @Skip.canceled += instance.OnSkip;
                }
            }
        }
        public UIActions @UI => new UIActions(this);
        public interface IUnitActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnMainAction(InputAction.CallbackContext context);
            void OnAdditionalAction(InputAction.CallbackContext context);
            void OnLockTarget(InputAction.CallbackContext context);
            void OnArmed(InputAction.CallbackContext context);
            void OnSprint(InputAction.CallbackContext context);
            void OnCrouch(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnQuests(InputAction.CallbackContext context);
            void OnLevels(InputAction.CallbackContext context);
            void OnInventory(InputAction.CallbackContext context);
            void OnSkill1(InputAction.CallbackContext context);
            void OnPause(InputAction.CallbackContext context);
        }
        public interface ICameraActions
        {
            void OnDelta(InputAction.CallbackContext context);
        }
        public interface IUIActions
        {
            void OnSkip(InputAction.CallbackContext context);
        }
    }
}
