using UnityEngine;
using TMPro;

public class DepthMeter : MonoBehaviour
{
    public Transform player;
    public Transform startpoint;
    public TextMeshProUGUI depthText; 

    private float StartPointY; 

    private void Start()
    {
       
        StartPointY = startpoint.position.y;
    }

    private void Update()
    {
        
        float depth = player.position.y - StartPointY;
        float depthfinal;

        if (depth > 0)
        {
            depthText.text = "Depth: 0M";
        }
        else
        {
            depthfinal = depth * -1;
            depthText.text = "Depth: " + depthfinal.ToString("F1") + "M";
        }

        // Update the depth text
        
    }
}