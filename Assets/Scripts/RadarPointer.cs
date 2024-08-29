using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class RadarPointer : NetworkBehaviour
{
    public Transform playerTransform;
    public Transform targetTransform; // Set when the opposite player connects

    [SerializeField]
    private float distance = 1.5f;

    private Renderer radarRenderer;



    void Start()
    {
        radarRenderer = GetComponent<Renderer>();

        
        radarRenderer.enabled = targetTransform != null;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            var inputActions = GetComponentInParent<PlayerInput>().actions;

            if (inputActions["HideShow"].triggered)
                ToggleRadarVisibility();
        }
        

        if (playerTransform != null && targetTransform == null)
        {
            GameObject player = playerTransform.gameObject;

            if (player.tag == "Runner")
            {
                GameObject target = GameObject.FindGameObjectWithTag("Chaser");

                if (target != null)
                    targetTransform = target.transform;
                    
                
            }
            else if (player.tag == "Chaser")
            {
                GameObject target = GameObject.FindGameObjectWithTag("Runner");
                if (target != null)
                    targetTransform = target.transform;
            }
            else { }

        }

        if (targetTransform == null || playerTransform == null || !radarRenderer.enabled)
            return;

        
        Vector3 directionToTarget = targetTransform.position - playerTransform.position;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.position = playerTransform.position + directionToTarget.normalized * distance;
    }

    private void ToggleRadarVisibility()
    {
        radarRenderer.enabled = !radarRenderer.enabled;
    }
}
