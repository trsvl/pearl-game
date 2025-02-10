using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ChainedSphereDestroyer : MonoBehaviour
{
    [Header("Raycast Settings")] public float maxRayDistance = 100f;
    public LayerMask sphereLayer;
    public LayerMask ignoreRaycastLayer;

    [Header("Destruction Settings")] public float detectionRadius = 0.5f;
    public int maxSphereChecks = 50;
    public float fallingDelay = 1f;
    public float destructionDelay = 0.2f;

    private Collider[] nearbySpheres;
    private HashSet<GameObject> processedSpheres = new HashSet<GameObject>();

    void Start()
    {
        nearbySpheres = new Collider[maxSphereChecks];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);            
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            TryDestroySpheres();
        }
    }

    void TryDestroySpheres()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, sphereLayer))
        {
            GameObject hitSphere = hit.collider.gameObject;
            Color hitColor = hitSphere.GetComponent<Renderer>().material.color;

            processedSpheres.Clear();
            StartCoroutine(DestroyConnectedSpheres(hitSphere, hitColor));
        }
    }

    IEnumerator DestroyConnectedSpheres(GameObject sphere, Color color)
    {
        if (!sphere || !processedSpheres.Add(sphere)) yield break;

        yield return new WaitForSeconds(fallingDelay);

        
        int hitCount =
            Physics.OverlapSphereNonAlloc(sphere.transform.position, detectionRadius, nearbySpheres, sphereLayer);

        for (int i = 0; i < hitCount; i++)
        {
            GameObject neighbor = nearbySpheres[i].gameObject;

            if (neighbor && neighbor.GetComponent<Renderer>().material.color == color)
            {
                StartCoroutine(DestroyConnectedSpheres(neighbor, color));
            }
        }


        sphere.layer = ignoreRaycastLayer;
        var rb = sphere.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.excludeLayers += sphereLayer;

        yield return new WaitForSeconds(destructionDelay);

        Destroy(sphere);

     
    }
}