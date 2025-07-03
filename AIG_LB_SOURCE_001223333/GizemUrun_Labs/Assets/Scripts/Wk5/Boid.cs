using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    [Header("Boid Settings")]

    [Tooltip("The maximum speed the boid can travel.")]
    public float maxSpeed = 5f;

    [Tooltip("The maximum steering force applied to the boid.")]
    public float maxForce = 10f;

    [Tooltip("The radius within which the boid can sense other boids.")]
    public float sensingDistance = 5f;

    [Tooltip("Weight of the cohesion behavior.")]
    public float cohesionStrength = 1f;

    [Tooltip("Weight of the separation behavior.")]
    public float separationStrength = 1.5f;

    [Tooltip("Weight of the alignment behavior.")]
    public float alignmentStrength = 1f;

    [Tooltip("Weight of the wander behavior.")]
    public float wanderStrength = 0.5f;

    [Tooltip("The object to which the boid is anchored.")]
    public Transform anchor; 

    [Tooltip("The object to which the boid is anchored.")]
    public float anchorStrength = 5f; // Anchor strength

    public GameObject anc;

    // Movement variables
    [HideInInspector]
    public Vector3 velocity;

    [HideInInspector]
    public Vector3 acceleration;

    public Rigidbody rb;

    // Exposed force vectors for inspection
    [Header("Force Vectors (Read-Only)")]
    [Tooltip("Cohesion steering force.")]
    public Vector3 cohesionForce;

    [Tooltip("Separation steering force.")]
    public Vector3 separationForce;

    [Tooltip("Alignment steering force.")]
    public Vector3 alignmentForce;

    [Tooltip("Wander steering force.")]
    public Vector3 wanderForce;

    [Tooltip("Anchor force to keep the boid near the anchor.")]
    public Vector3 anchorForce; 

    [Tooltip("Total steering force applied to the boid.")]
    public Vector3 totalForce;

    // Exposed magnitudes for inspection
    [Header("Force Magnitudes (Read-Only)")]
    [Tooltip("Magnitude of the cohesion force.")]
    public float cohesionMagnitude;

    [Tooltip("Magnitude of the separation force.")]
    public float separationMagnitude;

    [Tooltip("Magnitude of the alignment force.")]
    public float alignmentMagnitude;

    [Tooltip("Magnitude of the wander force.")]
    public float wanderMagnitude;

    [Tooltip("Magnitude of the anchor force.")]
    public float anchorMagnitude; 

    [Tooltip("Magnitude of the total force.")]
    public float totalMagnitude;

    void Awake()
    {
        // Initialize the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Set initial velocity randomly within a unit sphere scaled by max speed
        velocity = Random.insideUnitSphere * maxSpeed;
        rb.velocity = velocity;

        // Configure rigidbody properties to simulate boid behavior
        rb.useGravity = false; // Boids should not be affected by gravity
        rb.drag = 1f; // Set drag to simulate natural slowing of movement

        //gets the transform of the anhor in the scene
        if (anchor == null)
            anc = GameObject.Find("Anchor");
            anchor = anc.transform;
    }

    void FixedUpdate()
    {
        // Reset acceleration to zero at the start of each physics update
        acceleration = Vector3.zero;

        // Get a list of nearby boids within the sensing distance
        List<Boid> neighbors = GetNeighbors();

        // Calculate steering forces based on neighbor presence
        if (neighbors.Count > 0)
        {
            cohesionForce = Cohesion(neighbors) * cohesionStrength; // Calculate cohesion force to align with neighbors
            separationForce = Separation(neighbors) * separationStrength; // Calculate separation force to avoid crowding
            alignmentForce = Alignment(neighbors) * alignmentStrength; // Calculate alignment force to match velocity
        }
        else
        {
            // No neighbors means no force is applied for these behaviors
            cohesionForce = Vector3.zero;
            separationForce = Vector3.zero;
            alignmentForce = Vector3.zero;
        }

        // Calculate wander force to add random movement and unpredictability
        wanderForce = Wander() * wanderStrength;

        // If an anchor is defined, calculate the anchor force
        if (anchor != null)
        {
            anchorForce = AnchorForce() * anchorStrength; 
        }
        else
        {
            anchorForce = Vector3.zero; // No anchor, no force
        }

        // Sum all steering forces to calculate the total force acting on the boid
        totalForce = cohesionForce + separationForce + alignmentForce + wanderForce + anchorForce;

        // Clamp the total force so it doesn't exceed the maximum allowed steering force
        totalForce = Vector3.ClampMagnitude(totalForce, maxForce);

        // Apply the final calculated force to the Rigidbody
        ApplyForce(totalForce);

        // Limit the boid's speed to the maximum speed defined
        velocity = rb.velocity;
        if (velocity.magnitude > maxSpeed)
        {
            rb.velocity = velocity.normalized * maxSpeed;
        }

        // Rotate the boid to face the direction it is moving in
        if (rb.velocity != Vector3.zero)
        {
            transform.forward = rb.velocity.normalized;
        }

        // Update force magnitudes to display in inspector for debugging
        UpdateForceMagnitudes();
    }

    // Applies the given force to the Rigidbody to move the boid
    void ApplyForce(Vector3 force)
    {
        rb.AddForce(force);
    }

    // Finds all neighboring boids within the sensing distance
    List<Boid> GetNeighbors()
    {
        List<Boid> neighbors = new List<Boid>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, sensingDistance);

        foreach (Collider collider in colliders)
        {
            Boid boid = collider.GetComponent<Boid>();
            if (boid != null && boid != this)
            {
                neighbors.Add(boid); // Add boids that are not itself
            }
        }
        return neighbors;
    }

    // Calculates the cohesion force to steer towards the center of neighbors
    Vector3 Cohesion(List<Boid> neighbors)
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        // Calculate the average position of all neighboring boids
        foreach (Boid boid in neighbors)
        {
            if (boid != null)
            {
                centerOfMass += boid.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            centerOfMass /= count; // Calculate the center of mass
            Vector3 desired = centerOfMass - transform.position; // Direction towards center of mass
            desired = desired.normalized * maxSpeed; // Normalize and set to max speed
            Vector3 steering = desired - rb.velocity; // Calculate steering force
            return steering;
        }
        else
        {
            return Vector3.zero; // No neighbors means no cohesion force
        }
    }

    // Calculates the separation force to avoid crowding neighboring boids
    Vector3 Separation(List<Boid> neighbors)
    {
        Vector3 steering = Vector3.zero;
        int count = 0;

        // Calculate direction away from each neighbor, inversely proportional to distance
        foreach (Boid boid in neighbors)
        {
            if (boid != null)
            {
                Vector3 difference = transform.position - boid.transform.position;
                float distance = difference.magnitude;

                if (distance > 0)
                {
                    Vector3 away = difference.normalized / distance; // Weighted by inverse distance
                    steering += away;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            steering /= count; // Average the steering force
            steering = steering.normalized * maxSpeed - rb.velocity; // Calculate final steering direction
            return steering;
        }
        else
        {
            return Vector3.zero; // No neighbors means no separation force
        }
    }

    // Calculates the alignment force to match velocity with neighbors
    Vector3 Alignment(List<Boid> neighbors)
    {
        Vector3 averageVelocity = Vector3.zero;
        int count = 0;

        // Calculate the average velocity of all neighboring boids
        foreach (Boid boid in neighbors)
        {
            if (boid.rb != null)
            {
                averageVelocity += boid.rb.velocity;
                count++;
            }
        }

        if (count > 0)
        {
            averageVelocity /= count; // Average velocity of neighbors
            Vector3 desired = averageVelocity.normalized * maxSpeed; // Set desired velocity to match average
            Vector3 steering = desired - rb.velocity; // Calculate steering force to align with neighbors
            return steering;
        }
        else
        {
            return Vector3.zero; // No neighbors means no alignment force
        }
    }

    // Generates a wander force to add some randomness and variability to the movement
    Vector3 Wander()
    {
        float wanderRadius = 1f; // Radius of the circle within which to wander
        float wanderDistance = 2f; // Distance ahead of the boid for wandering
        float wanderJitter = 1f; // Amount of random fluctuation in wandering

        // Generate a random point within the sphere to add to the wandering
        Vector3 wanderTarget = Random.insideUnitSphere * wanderRadius;
        wanderTarget += transform.forward * wanderDistance;

        // Add jitter to create erratic, random movement
        wanderTarget += new Vector3(
            Random.Range(-wanderJitter, wanderJitter),
            Random.Range(-wanderJitter, wanderJitter),
            Random.Range(-wanderJitter, wanderJitter)
        );

        Vector3 desired = wanderTarget.normalized * maxSpeed; // Normalize and set to max speed
        Vector3 steering = desired - rb.velocity; // Calculate steering force
        return steering;
    }

    // Calculate anchor direction and force to pull boids towards the anchor
    Vector3 AnchorForce()
    {
        Vector3 desired = anchor.position - transform.position; // Direction towards the anchor
        return desired.normalized * maxSpeed - rb.velocity; // Calculate force
    }

    // Update force magnitudes to help with debugging and visualization in inspector
    void UpdateForceMagnitudes()
    {
        cohesionMagnitude = cohesionForce.magnitude;
        separationMagnitude = separationForce.magnitude;
        alignmentMagnitude = alignmentForce.magnitude;
        wanderMagnitude = wanderForce.magnitude;
        anchorMagnitude = anchorForce.magnitude;
        totalMagnitude = totalForce.magnitude;
    }

    // Visual representation in the editor for debugging
    void OnDrawGizmos()
    {
        // Draw a wire sphere to show the sensing distance of the boid
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sensingDistance);

        // Draw an arrow representing the forward direction of the boid
        Gizmos.color = Color.red;
        Vector3 forward = transform.forward * 2f;
        Gizmos.DrawLine(transform.position, transform.position + forward);

        // Draw arrowheads to indicate directionality
        Vector3 right = Quaternion.LookRotation(forward) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(forward) * Quaternion.Euler(0, -150, 0) * Vector3.forward;
        Gizmos.DrawLine(transform.position + forward, transform.position + forward + right * 0.5f);
        Gizmos.DrawLine(transform.position + forward, transform.position + forward + left * 0.5f);
    }
}
