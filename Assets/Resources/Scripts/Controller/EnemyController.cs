/*
 * Al A.
 * Summer 2020 (c)
 */
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// Control the behavior of enemies.
/// </summary>
public class EnemyController : MonoBehaviour
{
    // COMPONENTS
    // Scene UI use since target is defined in Start()
    public Transform playerUI;
    /// <summary>
    /// Unity objects defined when game starts.
    /// </summary>
    Transform target;
    NavMeshAgent NMAgent;

    // VARIABLES
    // for gizmo drawing
    /// <summary>
    /// Maximum distance the enemy can see.
    /// Radius of FOV.
    /// </summary>
    [SerializeField]
    float viewRadiusMax = 10f;
    /// <summary>
    /// Angle between enemy's facing direction to where the FOV should end.
    /// </summary>
    [SerializeField]
    float FOVAngleMax = 60f;
    // Enemy FOV use
    readonly float heightMultiplayer = 1.0f;
    /// <summary>
    /// Refers to if the player's position is within the FOV of enemy, NOT if the enemy sees the player.
    /// </summary>
    bool isInFOV;

    // Enemy Behavior use
    enum enemyBehaviorType
    {
        Patrol,
        Stationary
    }
    /// <summary>
    /// Defines the behavior of the current instance of this enemy.
    /// </summary>
    [SerializeField]
    enemyBehaviorType enemyBehavior = enemyBehaviorType.Stationary;

    // PATROL BEHAVIOR USE
    /// <summary>
    /// A list of Waypoints that this enemy will travel to.
    /// </summary>
    [SerializeField]
    List<Waypoints> patrolPoints = new List<Waypoints>();
    /// <summary>
    /// Current waypoint the enemy is travelling to.
    /// </summary>
    int curPatrolIndex;

    // STATIONARY BEHAVIOR USE
    /// <summary>
    /// Time in seconds the enemy will look in one direction for.
    /// </summary>
    [SerializeField]
    float totalLookTime = 5f;
    /// <summary>
    /// State where the enemy is looking in one direction, i.e. NOT turning to look in another direction.
    /// </summary>
    bool looking = true;
    /// <summary>
    /// State where the enemy is turning to look in another direction.
    /// </summary>
    bool turning;
    /// <summary>
    /// Timer to monitor how long the enemy is in the "looking" state.
    /// </summary>
    float lookTimer = 0f;

    /// <summary>
    /// Compass directions in Unity. 
    /// North = +z, East = +x, South = -z, West = -x.
    /// </summary>
    enum compassDir
    {
        /// <summary>
        /// +z-axis
        /// </summary>
        North,
        /// <summary>
        /// +x-axis
        /// </summary>
        East,
        /// <summary>
        /// -z-axis
        /// </summary>
        South,
        /// <summary>
        /// -x-axis
        /// </summary>
        West
    }
    /// <summary>
    /// Initial starting direction of enemy.
    /// </summary>
    [SerializeField]
    compassDir startDirection = compassDir.North;
    /// <summary>
    /// Current direction the enemy is facing.
    /// </summary>
    int curDirectionIndex;
    /// <summary>
    /// Compass directions as vectors.
    /// Variables have same indexes as compassDir
    /// </summary>
    readonly Vector3[] compass = 
    {
        new(0, 0, 90f),    // North
        new(90f, 0, 0),    // East
        new(0, 0, -90f),   // South
        new(-90f, 0, 0)    // West
    };

    // FOV visualization use
    /// <summary>
    /// Resolution of MeshFilter
    /// </summary>
    public float meshResolution;
    /// <summary>
    /// Mesh Filter of enemy FOV
    /// </summary>
    public MeshFilter viewMeshFilter;
    /// <summary>
    /// Visual Mesh of Enemy FOV.
    /// </summary>
    Mesh viewMesh;

    // Footsteps use
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

    // Called once per frame.
    // LateUpdate is called after Update.
    private void LateUpdate()
    {
        DrawFOV();
    }

    // Called ince oer frame.
    // FixedUpdate should be used for physics.
    void FixedUpdate()
    {
        // Distance between the player and the enemy.
        float distance = Vector3.Distance(target.position, transform.position);
        // Sets if the player is currently within sight of the enemy.
        isInFOV = inFOV(transform, target, FOVAngleMax, viewRadiusMax);
        
        // Player enters enemy line-of-sight
        if (isInFOV)
        {
            // Add this enemy to the list of enemies that have currently spotted the player.
            PlayerManager.instance.enemySpotsPlayer(this.gameObject);
        }

        // Test if player is hidden
        if (!PlayerManager.instance.getHideStatus() && PlayerManager.instance.isEnemyFollowingPlayer(this.gameObject))
        {
            //Debug.Log("Chasing...");
            NMAgent.SetDestination(target.position);

            // If the enemy gets close to the player...
            if (distance <= NMAgent.stoppingDistance)
            {
                FaceTarget();

                // Update player losing status
                GameManager.instance.SetLossStatus(true);
            }

            // Player must escape viewing radius to become undetected again
            if (distance > viewRadiusMax)
            {
                // Remove this enemy from the list of enemies that have currently spotted the player.
                PlayerManager.instance.enemyLosesPlayer(this.gameObject);
            }
        }
        // Default behavior
        else
        {
            DefaultBehavior();
        }

        // If enemy is moving, play footstep sound.
        // FIXME: does not work
        /*
        if (NMAgent.velocity.magnitude > .1)
            PlayFootsteps();
        //*/
    }

    /// <summary>
    /// Tells enemy how to behave by default.
    /// </summary>
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

    /// <summary>
    /// Plays footstep noise. FIXME: BROKEN.
    /// </summary>
    void PlayFootsteps()
    {
        AS.Play();
    }

    /// <summary>
    /// Tells a "stationary" enemy how to behave.
    /// Go to a specified waypoint and stay there. Rotate along y-axis at waypoint, chase player if spotted. Return to waypoint if player escapes.
    /// </summary>
    void DefaultBehaviorStationary()
    {
        float distance = Vector3.Distance(patrolPoints[curPatrolIndex].transform.position, transform.position);
        Quaternion look = Quaternion.LookRotation(new Vector3(compass[curDirectionIndex].x, 0f, compass[curDirectionIndex].z));
        float angle = Quaternion.Angle(transform.rotation, look);

        // When enemy returns to position, start looking around.
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

    /// <summary>
    /// Updates the current direction that the enemy is facing.
    /// </summary>
    void SetDirection()
    {
        curDirectionIndex = (++curDirectionIndex) % compass.Length;
    }

    /// <summary>
    /// Triggers the enemy to look in the next direction.
    /// </summary>
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

    /// <summary>
    /// Tells a "patrol" enemy how to behave.
    /// Travel to each specified waypoint on a loop. Chase player if spotted, return to current waypoint if player escapes.
    /// </summary>
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

    /// <summary>
    /// Turn enemy to face the target if target is too close to move towards
    /// </summary>
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

    /// <summary>
    /// Checks if the enemy can see the player.
    /// </summary>
    /// <param name="checkingObject">Enemy's transform.</param>
    /// <param name="target">Target's transform.</param>
    /// <param name="viewAngle">Refers to the angle of enemy's FOV.</param>
    /// <param name="radius">Refers to the radius of enemy's FOV.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Draws visual mesh of enemy FOV.
    /// </summary>
    void DrawFOV()
    {
        // FOV full angle, from left peripheral to right peripheral.
        float viewAngle = FOVAngleMax * 2;
        // Number of triangles to draw, based on mesh resolution.
        int rayCount = Mathf.RoundToInt(viewAngle * meshResolution);
        // Angle of each triangle.
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

        // Draw mesh based on calculations.
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
    void OnDrawGizmos()
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

        // Displays direction from target to enemy
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (playerUI.position - transform.position).normalized * viewRadiusMax);
    }
}
