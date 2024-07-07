using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

// Control the behavior of enemies
public class EnemyController : MonoBehaviour
{
    // Reference Unity objects
    public Transform playerUI;  // Scene UI use since target is defined in Start()

    // Unity objects defined when game starts
    Transform target;           // Game use
    NavMeshAgent NMAgent;

    // Variables for gizmo drawing
    [SerializeField]
    float viewRadiusMax = 10f;
    [SerializeField]
    float FOVAngleMax = 60f;

    // Enemy FOV use
    float heightMultiplayer = 1.0f;
    bool isInFOV;

    // Enemy Behavior use
    enum enemyBehaviorType
    {
        Patrol,
        Stationary
    }
    [SerializeField]
    enemyBehaviorType enemyBehavior = enemyBehaviorType.Stationary;

    // Patrol use
    [SerializeField]
    List<Waypoints> patrolPoints = new List<Waypoints>();
    int curPatrolIndex;

    // Stationary use
    [SerializeField]
    float totalLookTime = 5f;
    bool looking = true;
    bool turning;
    float lookTimer = 0f;
    enum compassDir
    {
        North,
        East, 
        South,
        West
    }
    [SerializeField]
    compassDir startDirection = compassDir.North;
    int curDirectionIndex;
    Vector3[] compass = 
    {
        new Vector3(0, 0, 90f),    // North
        new Vector3(90f, 0, 0),    // East
        new Vector3(0, 0, -90f),   // South
        new Vector3(-90f, 0, 0)    // West
    };

    // FOV visualization use
    public float meshResolution;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    // Footstep use
    //[SerializeField]
    //AudioMixer audioMixer;
    //[SerializeField]
    //AudioClip triggerSound;
    AudioSource AS;

    void Start()
    {
        target = PlayerManager.instance.player.transform;
        NMAgent = GetComponent<NavMeshAgent>();
        AS = GetComponent<AudioSource>();

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        if (NMAgent != null)
        {
            switch (enemyBehavior)
            {
                case enemyBehaviorType.Patrol:
                    if (patrolPoints != null && patrolPoints.Count >= 2)
                    {
                        //for (int i = 0; i < patrolPoints.Count; ++i)
                        // Debug.Log(patrolPoints[i].transform.position);

                        curPatrolIndex = 0;
                    }
                    else
                        Debug.Log("Insufficient patrol points for " + enemyBehavior + " behavior.");
                    break;

                case enemyBehaviorType.Stationary:
                    if (patrolPoints != null && patrolPoints.Count == 1)
                    {
                        //for (int i = 0; i < patrolPoints.Count; ++i)
                        // Debug.Log(patrolPoints[i].transform.position);

                        curPatrolIndex = 0;

                        //curDirectionIndex = (int)startDirection;

                        switch (startDirection) // FIXME: explicit cast instead of a switch
                        {
                            case compassDir.North:
                                curDirectionIndex = 0;
                                break;
                            case compassDir.East:
                                curDirectionIndex = 1;
                                break;
                            case compassDir.South:
                                curDirectionIndex = 2;
                                break;
                            case compassDir.West:
                                curDirectionIndex = 3;
                                break;
                            default:
                                Debug.Log("Start Direction has no value for object " + this.gameObject.name + ". Set to North (0).");
                                curDirectionIndex = 0;
                                break;
                        }
                    }
                    else
                        Debug.Log("Insufficient patrol points for " + enemyBehavior + " behavior.");
                    break;

                default:
                    Debug.Log("ERROR: Enemy " + this.gameObject.name + " does not have set behavior.");
                    break;
            }
        }
        else
        {
            Debug.LogError("The NavMeshAgent component is not attched to " + gameObject.name);
        }
    }

    private void LateUpdate()
    {
        DrawFOV();
    }

    void FixedUpdate()
    {
        // Distance between the player and the enemy
        float distance = Vector3.Distance(target.position, transform.position);
        isInFOV = inFOV(transform, target, FOVAngleMax, viewRadiusMax);
        
        // Player enters enemy line-of-sight
        if (isInFOV)
        {
            //PlayerManager.instance.setHideStatus(false);
            PlayerManager.instance.enemySpotsPlayer(this.gameObject);
        }
        // Test if player is hidden
        if (!PlayerManager.instance.getHideStatus() && PlayerManager.instance.isEnemyFollowingPlayer(this.gameObject))
        {
            //Debug.Log("Chasing...");
            NMAgent.SetDestination(target.position);

            // If the enemy gets close to the player
            if (distance <= NMAgent.stoppingDistance)
            {
                FaceTarget();

                // Update player losing status
                GameManager.instance.setLossStatus(true);
            }
            // Player must escape viewing radius to become undetected again
            if (distance > viewRadiusMax)
            {
                //PlayerManager.instance.setHideStatus(true);
                PlayerManager.instance.enemyLosesPlayer(this.gameObject);
            }
        }
        // Default behavior
        else
        {
            DefaultBehavior();
        }

        if (NMAgent.velocity.magnitude > .1)
            PlayFootsteps();
    }

    void DefaultBehavior()
    {
        switch (enemyBehavior)
        {
            case enemyBehaviorType.Patrol:
                SetDestination();
                DefaultBehaviorPatrol();
                break;

            case enemyBehaviorType.Stationary:
                SetDestination();
                DefaultBehaviorStationary();
                break;

            default:
                Debug.Log("ERROR: Enemy " + this.gameObject.name + " does not have set behavior.");
                break;
        }
    }

    void PlayFootsteps()
    {
        AS.Play();
    }

    void DefaultBehaviorStationary()
    {
        float distance = Vector3.Distance(patrolPoints[curPatrolIndex].transform.position, transform.position);
        Quaternion look = Quaternion.LookRotation(new Vector3(compass[curDirectionIndex].x, 0f, compass[curDirectionIndex].z));
        float angle = Quaternion.Angle(transform.rotation, look);

        // When enemy returns to position, start looking around
        if (distance <= 2f)
        {
            if (looking)
            {
                lookTimer += Time.deltaTime;
                if (lookTimer >= totalLookTime)
                {
                    looking = false;
                    SetDirection();
                    turning = true;
                }
            }
            if (turning && angle < 0.1f)
            {
                turning = false;
                looking = true;
                lookTimer = 0f;
            }
            else
            {
                FaceDirection();
            }
        }
    }

    void SetDirection()
    {
        curDirectionIndex = (++curDirectionIndex) % compass.Length;
    }

    void FaceDirection()
    {
        // Different method of rotation since enemies use NavMesh while player uses input.
        // Direction from the target to the enemy        
        Vector3 direction = compass[curDirectionIndex];

        // Rotation that points to target.
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

        // Quaternion.Slerp interpolates the rotation from its current rotation to the rotation it's changing to.
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void DefaultBehaviorPatrol()
    {
        // Distance from enemy's current position to the next point its travelling to
        float distance = Vector3.Distance(patrolPoints[curPatrolIndex].transform.position, transform.position);

        if (distance <= 2f)
        {
            ChangePatrolPoint();
            SetDestination();
        }
    }

    // Determines which waypoint to navigate to next
    void ChangePatrolPoint()
    {
        curPatrolIndex = (++curPatrolIndex) % patrolPoints.Count;
        //Debug.Log("Point changed to " + curPatrolIndex);
    }

    // Sets the destination to the next control point
    void SetDestination()
    {
        if (patrolPoints != null)
        {
            Vector3 targetVec = patrolPoints[curPatrolIndex].transform.position;
            NMAgent.SetDestination(targetVec);
            //Debug.Log("New destination at " + targetVec);
        }
    }

    // Turn enemy to face the target if target is too close to move towards
    void FaceTarget()
    {
        // Different method of rotation since enemies use NavMesh while player uses input.
        // Direction from the target to the enemy        
        Vector3 direction = (target.position - transform.position).normalized;

        // Rotation that points to target.
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

        // Quaternion.Slerp interpolates the rotation from its current rotation to the rotation it's changing to.
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Checks if the enemy can see the player
    public bool inFOV(Transform checkingObject, Transform target, float viewAngle, float radius)
    {
        Vector3 directionBetween = (target.position - checkingObject.position).normalized;
        directionBetween.y = 0;

        Ray r = new Ray(checkingObject.position + Vector3.up * heightMultiplayer, (target.position - checkingObject.position).normalized);
        RaycastHit hit;
        
        // Test if ray hits something
        if (Physics.Raycast(r, out hit, radius))
        {
            // Test if ray hit the player
            if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Player")
            {
                float angle = Vector3.Angle(checkingObject.forward, directionBetween);

                // Test if the player is within the FOV of the enemy
                if (angle <= viewAngle)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void DrawFOV()
    {
        float viewAngle = FOVAngleMax * 2;
        int rayCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float rayAngleSize = viewAngle / rayCount;
        List<Vector3> viewPoints = new List<Vector3>();

        for (int i = 0; i <= rayCount; ++i)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + rayAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadiusMax, Color.red);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; ++i)
        {
            if (i < vertexCount - 2)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
    Vector3 DirFromAngle(float angleInDegs, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegs += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegs * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegs * Mathf.Deg2Rad));
    }
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool hit, Vector3 point, float dist, float angle)
        {
            this.hit = hit;
            this.point = point;
            this.dist = dist;
            this.angle = angle;
        }
    }
    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadiusMax))
        {
            if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Environment")
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            }
        }
        return new ViewCastInfo(false, transform.position + dir * viewRadiusMax, viewRadiusMax, globalAngle);

    }

    // Draws Scene gizmos for AI detecting player
    public void OnDrawGizmos()
    {
        // Displays viewing radius in Unity editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadiusMax);

        // Displays FOV angles in Unity editor
        Vector3 FOVLine1 = Quaternion.AngleAxis(FOVAngleMax, transform.up) * transform.forward * viewRadiusMax;
        Vector3 FOVLine2 = Quaternion.AngleAxis(-FOVAngleMax, transform.up) * transform.forward * viewRadiusMax;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, FOVLine1);
        Gizmos.DrawRay(transform.position, FOVLine2);

        // Displays enemy's "forward" direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * viewRadiusMax);

        // Displays direction from player to enemy in respect to player
        if (!isInFOV)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (playerUI.position - transform.position).normalized * viewRadiusMax);
    }
}
