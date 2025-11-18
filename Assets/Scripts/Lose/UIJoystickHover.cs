//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class UIJoystickHover : MonoBehaviour
//{
//    public Selectable retryButton;
//    public Selectable exitButton;

//    public HoverPopLose retryPop;
//    public HoverPopLose exitPop;

//    void Update()
//    {
//        float x = Input.GetAxis("Horizontal");
//        float y = Input.GetAxis("Vertical");

//        if (Mathf.Abs(x) > 0.15f || Mathf.Abs(y) > 0.15f)
//        {
//            GameObject selected = EventSystem.current.currentSelectedGameObject;

//            if (selected == retryButton.gameObject)
//            {
//                retryPop.Pop();
//                exitPop.Unpop();
//            }
//            else if (selected == exitButton.gameObject)
//            {
//                exitPop.Pop();
//                retryPop.Unpop();
//            }
//        }
//    }
//}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIJoystickHover : MonoBehaviour
{
    public Selectable retryButton;
    public Selectable exitButton;

    public HoverPopLose retryPop;
    public HoverPopLose exitPop;

    public GameObject firstButton;

    void Start()
    {
        // Set the initial selected UI element
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // only check hover changes when joystick moves
        if (Mathf.Abs(x) > 0.15f || Mathf.Abs(y) > 0.15f)
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;

            if (selected == retryButton.gameObject)
            {
                retryPop.Pop();
                exitPop.Unpop();
            }
            else if (selected == exitButton.gameObject)
            {
                exitPop.Pop();
                retryPop.Unpop();
            }
        }
    }
}
