using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject crosshair;
    [SerializeField] float baseTurnSpeed = 3;
    [SerializeField] GameObject gun;
    [SerializeField] Transform turretBase;
    [SerializeField] Transform barrelEnd;
    [SerializeField] LineRenderer line;

    List<Vector3> laserPoints = new List<Vector3>();

    //new
    int maxBounce = 1;//max num for lazer to bounce

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TrackMouse();
        TurnBase();

        DrawLaser();//display laser 
        /*laserPoints.Clear();
        laserPoints.Add(barrelEnd.position);

        if(Physics.Raycast(barrelEnd.position, barrelEnd.forward, out RaycastHit hit, 1000.0f, targetLayer))
        {
            laserPoints.Add(hit.point);
        }

        line.positionCount = laserPoints.Count;
        for(int i = 0; i < line.positionCount; i++)
        {
            line.SetPosition(i, laserPoints[i]);
        }*/
    }

    void TrackMouse()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if(Physics.Raycast(cameraRay, out hit, 1000, targetLayer ))
        {
            crosshair.transform.forward = hit.normal;
            crosshair.transform.position = hit.point + hit.normal * 0.1f;
        }
    }

    void TurnBase()
    {
        Vector3 directionToTarget = (crosshair.transform.position - turretBase.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, directionToTarget.y, directionToTarget.z));
        turretBase.transform.rotation = Quaternion.Slerp(turretBase.transform.rotation, lookRotation, Time.deltaTime * baseTurnSpeed);
    }

    //new for laser bounce 
    void DrawLaser()
    {
        laserPoints.Clear();//clear prev points 
        Vector3 startPoint = barrelEnd.position;//get starting point 
        Vector3 direction = barrelEnd.forward;//direction laser points 
        laserPoints.Add(startPoint);//starting pt as first pt

        for (int bounces = 0; bounces <= maxBounce; bounces++)
        {
            //laser hits some surface
            if (Physics.Raycast(startPoint, direction, out RaycastHit hit, 1000f, targetLayer))
            {
                laserPoints.Add(hit.point);//add hit point
                direction = direction - 2 * Vector3.Dot(direction, hit.normal) * hit.normal;//calc reflection w/ dot R = I -2(I*N) N -> reflected dir = laser dir -2(laser dir dot surface norm) surface norm
                direction.Normalize();
                startPoint = hit.point + direction * .01f;//avoid double hit
            }

            else
            {
                laserPoints.Add(startPoint + direction * 1000f);//laser in direction it bounced in w/o other hit point
                break; //stop if theres no surface hit 
            }
        }
        line.positionCount= laserPoints.Count;//set num of pts
        line.SetPositions(laserPoints.ToArray());//connect pts to draw line
    } 
}
