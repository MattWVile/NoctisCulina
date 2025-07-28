using UnityEngine;

public class PhotonBeamChargeBar : MonoBehaviour
{
    public GameObject foreground; 
    public float maxCharge = 10.16f; // Maximum charge level for the beam

    public bool isCharged;
    public void FillBar(float chargeRate)
    {
        float currentScale = foreground.transform.localScale.x;
        float increment = Time.deltaTime * chargeRate;
        float newScale;

        if (currentScale + increment >= maxCharge)
        {
            newScale = maxCharge;
            isCharged = true; // Set charged state to true when max charge is reached
        }
        else
        {
            newScale = currentScale + increment;
            if (newScale >= maxCharge) { // If we reach max charge, set charged state to true
                isCharged = true;
            }
        }

        foreground.transform.localScale = new Vector3(newScale, foreground.transform.localScale.y, foreground.transform.localScale.z);
    }
    public void EmptyBar()
    {
        foreground.transform.localScale = new Vector3(0, foreground.transform.localScale.y, foreground.transform.localScale.z);
        isCharged = false;
    }
}
