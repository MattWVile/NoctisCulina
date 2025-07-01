using UnityEngine;
using Debug = UnityEngine.Debug;

public class DebuggingTools : MonoBehaviour
{

    void Update()
    {
        HandleTimeControl();
    }

    private static void HandleTimeControl()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            Time.timeScale *= 0.998f;
            Debug.Log($"[DEBUG] Decreased Time Scale [{Time.timeScale}]");
        }
        if (Input.GetKey(KeyCode.F2))
        {
            Time.timeScale /= 0.998f;
            Debug.Log($"[DEBUG] Increased Time Scale [{Time.timeScale}]");
        }
        if (Input.GetKey(KeyCode.F3))
        {
            Time.timeScale = 1f;
            Debug.Log($"[DEBUG] Reset Time Scale [{Time.timeScale}]");
        }
    }
}