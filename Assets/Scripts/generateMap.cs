using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class generateMap : MonoBehaviour
{

    public GameObject routerPrefab;
    public GameObject pipePrefab1;
    public GameObject pipePrefab2;
    public float delta = 0;

    public float pipeLength = 20;

    public int numRoutersInQueue = 1;

    public static Queue routerQueue = new Queue();
    public static Queue pipeQueue = new Queue();

    public bool renderPipe = true;

    public float socketOffset = 5.0f;



    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < numRoutersInQueue; i++) {
            GameObject newRouter = Instantiate(routerPrefab, new Vector3(0.0f, 100.0f, 0.0f), Quaternion.identity);
            routerQueue.Enqueue(newRouter);
            if (renderPipe) 
            {
                GameObject newPipe = Instantiate(pipePrefab1, new Vector3(0.0f, 100.0f, 0.0f), Quaternion.identity);
                pipeQueue.Enqueue(newPipe);
            }
        }
        
    }

    public class NullVector
    {
        public Vector3 pos;
        public bool isValid;

        public NullVector(Vector3 newPos, bool valid) 
        {
            this.pos = newPos;
            this.isValid = valid;
        }

        public bool changeValid() 
        {
            this.isValid = !this.isValid;
            return this.isValid;
        }
    }

    GameObject findAndEnq(List<GameObject> objects, bool isRouter) 
    {
        GameObject curRouter = null;
        float curDistance = Mathf.Infinity;

        GameObject furthestRouter = null;
        float furthest = 0;

        // Find nearest router
        foreach (GameObject router in objects)
        {
            Vector3 curPos = gameObject.transform.position;
            Vector3 routerPos = router.transform.position;
            float dist = Vector3.Distance(curPos, routerPos);

            if (dist < curDistance) 
            {
                curDistance = dist;
                curRouter = router;
            } else if (dist > furthest) {
                furthest = dist;
                furthestRouter = router;
            }
        }

        if (isRouter) 
        {
            if (routerQueue.Count < 4) 
            {
                routerQueue.Enqueue(furthestRouter);
            }
        } else 
        {
            if (pipeQueue.Count < 4) 
            {
                pipeQueue.Enqueue(furthestRouter);
            }
        }

        return curRouter;
    }

    Vector3 midpoint(Vector3 a, Vector3 b) 
    {
        return (a + b) / 2;
    }

    // 0 = top
    // 1 = left
    // 2 = right
    Vector3 translateInDirection(Vector3 startingPos, Quaternion startingDirection, float translationDistance, int direction)
    {
        Vector3 forwardVector = startingDirection * Vector3.forward;
        Vector3 leftVector = startingDirection * Vector3.left;
        Vector3 rightVector = startingDirection * Vector3.right;
        Vector3 newSpot = new Vector3(0.0f, 0.0f, 0.0f);

        switch(direction) 
        {
            case 0:
                newSpot = startingPos + (forwardVector.normalized * translationDistance);
                break;
            case 1:
                newSpot = startingPos + (leftVector.normalized * translationDistance);
                break;
            case 2:
                newSpot = startingPos + (rightVector.normalized * translationDistance);
                break;
        }

        return newSpot;
    }

    void renderObjs(List<GameObject> objs, GameObject curRouter, bool isRouter) {

        Vector3 curRouterPos = curRouter.transform.position;
        Quaternion curRotation = curRouter.transform.rotation;

        Router rRouter = curRouter.GetComponent<Router>();
        Transform leftSocket = rRouter.leftSocket;
        Transform rightSocket = rRouter.rightSocket;
        Transform topSocket = rRouter.straightSocket;

        NullVector topPos = new NullVector(translateInDirection(curRouterPos, curRotation, pipeLength + socketOffset, 0), true);
        NullVector leftPos = new NullVector(translateInDirection(curRouterPos, curRotation, pipeLength + socketOffset, 1), true);
        NullVector rightPos = new NullVector(translateInDirection(curRouterPos, curRotation, pipeLength + socketOffset, 2), true);

        List<NullVector> points = new List<NullVector> { rightPos, leftPos };//, topPos};

        for (int i = 0; i < points.Count; i++) 
        {
            foreach (GameObject router in objs) 
            {
                Vector3 routerPos = router.transform.position;
                if (routerPos == points[i].pos)
                {
                    points[i].changeValid();
                }
            }
        }

        if (isRouter) 
        {
            for (int i = 0; i < points.Count; i++) 
            {
                if (points[i].isValid && routerQueue.Count >= 1) {
                    GameObject newRouter = (GameObject) routerQueue.Dequeue();
                    newRouter.transform.position = points[i].pos;
                    newRouter.transform.rotation = curRotation;

                    switch(i) 
                    {
                        case 0:
                            newRouter.transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
                            break;
                        case 1:
                            newRouter.transform.Rotate(0.0f, -90.0f, 0.0f, Space.Self);
                            break;
                    }
                }
            }
        } 
        else 
        {
            for (int i = 0; i < points.Count; i++) 
            {
                if (points[i].isValid && pipeQueue.Count >= 1) {
                    // GameObject newPipe = (GameObject) pipeQueue.Dequeue();
                    GameObject newPipe = Instantiate(pipePrefab1, new Vector3(0,0,0), Quaternion.identity);

                    switch(i) 
                    {
                        case 0:
                            newPipe.transform.position = translateInDirection(rightSocket.position, rightSocket.rotation, socketOffset, 0);
                            newPipe.transform.rotation = rightSocket.rotation;
                            break;
                        case 1:
                            newPipe.transform.position = translateInDirection(leftSocket.position, leftSocket.rotation, socketOffset, 0);
                            newPipe.transform.rotation = leftSocket.rotation;
                            break;
                        case 2:
                            newPipe.transform.position = translateInDirection(topSocket.position, topSocket.rotation, socketOffset, 0);
                            newPipe.transform.rotation = topSocket.rotation;
                            break;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Router[] routerObjs = FindObjectsOfType<Router>();
        List<GameObject> routers = new List<GameObject>();
        foreach (Router r in routerObjs) { routers.Add(r.gameObject); }

        GameObject[] pipeObjs = GameObject.FindGameObjectsWithTag("Pipe");
        List<GameObject> pipes = new List<GameObject>();
        foreach (GameObject p in pipeObjs) { pipes.Add(p.gameObject); }

        
        int routerCount = routers.Count;
        //Debug.Log("Router Count" + routerCount);
        
        // Generate adjacent routers if they don't exist
        GameObject curRouter = findAndEnq(routers, true);

        // Ignore value
        _ = findAndEnq(pipes, false);

        renderObjs(routers, curRouter, true);
        if (renderPipe) {
            renderObjs(pipes, curRouter, false);
        }
        
    }
}
