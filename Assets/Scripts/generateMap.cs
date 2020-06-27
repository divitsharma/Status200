using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateMap : MonoBehaviour
{

    public GameObject routerPrefab;
    public GameObject pipePrefab1;
    public GameObject pipePrefab2;

    public float pipeLength = 20;

    public GameObject[] routers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 curPos = gameObject.transform.position;
        float curDistance = Mathf.Infinity;
        GameObject curRouter = null;

        routers = GameObject.FindGameObjectsWithTag("Router");

        // Find nearest router
        foreach (GameObject router in routers)
        {
            Vector3 routerPos = router.transform.position;
            float dist = Vector3.Distance(curPos, routerPos);

            if (dist < curDistance) 
            {
                curDistance = dist;
                curRouter = router;
            }
        }
        
        Vector3 curRouterPos = curRouter.transform.position;
        // Generate adjacent routers if they don't exist
        Vector3 top = new Vector3(curRouterPos.x + pipeLength, curRouterPos.y, curRouterPos.z);
        Vector3 bottom = new Vector3(curRouterPos.x - pipeLength, curRouterPos.y, curRouterPos.z);
        Vector3 left = new Vector3(curRouterPos.x, curRouterPos.y, curRouterPos.z - pipeLength);
        Vector3 right = new Vector3(curRouterPos.x, curRouterPos.y, curRouterPos.z + pipeLength);

        List<Vector3> points = new List<Vector3> {top, bottom, left, right};

        for (int i = 0; i < points.Count; i++) 
        {
            foreach (GameObject router in routers) 
            {
                Vector3 routerPos = router.transform.position;
                if (routerPos == points[i])
                {
                    points.RemoveAt(i);
                }
            }
        }

        foreach (Vector3 point in points) 
        {
            Instantiate(routerPrefab, point, curRouter.transform.rotation);
        }
    }
}
