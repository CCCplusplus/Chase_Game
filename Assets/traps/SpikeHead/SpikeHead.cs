using UnityEngine;
using Mirror;

public class SpikeHead : NetworkBehaviour
{
    [Header("SpikeHead Attributes")]
    [SerializeField] float speed;
    [SerializeField] float range;
    [SerializeField] float checkDelay;
    [SerializeField] LayerMask playerLayer;
    public Transform runnerTransform;
    public Transform chaserTransform;
    private Vector3[] directions = new Vector3[4];
    private Vector3 destination;
    private Vector3 startingPosition;
    private float checkTimer;
    private bool attacking;


    //private void Start()
    //{
    //    startingPosition = transform.position;
    //}
    private void OnEnable()
    {
        startingPosition = transform.position;
        Stop();
    }
    private void Update()
    {
        //Move spikehead to destination only if attacking
        if (attacking)
            transform.Translate(destination * Time.deltaTime * speed);
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > checkDelay)
                CheckForPlayer();
        }
    }
    
    private void CheckForPlayer()
    {
        CheckForPlayerRPC();
    }
    //[Command]
    private void CheckForPlayerCMD()
    {
        CheckForPlayerRPC();
    }
    [Client]
    private void CheckForPlayerRPC()
    {
        CalculateDirections();

        //Check if spikehead sees player in all 4 directions
        for (int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i], Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);

            if (hit.collider != null && !attacking)
            {
                attacking = true;
                destination = directions[i];
                checkTimer = 0;
            }
        }
    }
    private void CalculateDirections()
    {
        CalculateDirectionsRPC();
    }
    //[Command]
    //private void CalculateDirectionsCMD()
    //{
    //    CalculateDirectionsRPC();
    //}
    [Client]
    private void CalculateDirectionsRPC()
    {
        directions[0] = transform.right * range; //Right direction
        directions[1] = -transform.right * range; //Left direction
        directions[2] = transform.up * range; //Up direction
        directions[3] = -transform.up * range; //Down direction
    }
    private void Stop()
    {
        StopRPC();
    }

    //[Command]

    //private void StopCMD()
    //{
    //    StopRPC();
    //}

    [Client]

    private void StopRPC()
    {
        transform.position = startingPosition; 
        attacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        player.died = true;
        if (collision.gameObject.tag == "Runner" || collision.gameObject.tag == "Invencible")
          collision.gameObject.transform.position = runnerTransform.position;
      else if (collision.gameObject.tag == "Chaser")
            collision.gameObject.transform.position = chaserTransform.position;
        
        Stop(); //Stop spikehead once he hits something
    }
}
