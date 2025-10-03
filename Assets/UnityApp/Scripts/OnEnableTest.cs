using UnityEngine;

public class OnEnableTest : MonoBehaviour
{
    public bool OnEnableLog;
    public bool OnDisableLog;
    private void OnEnable()
    {
        if (OnEnableLog)
        {
            Debug.Log("OnEnable Test : "+name, gameObject);
        }

    }
    private void OnDisable()
    {
        if (OnDisableLog)
        {
            Debug.Log("OnDisable Test : " + name, gameObject);
        }
    }
}
