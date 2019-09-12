using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class partHandler : MonoBehaviour
{
    GameObject currentArms;
    GameObject currentLegs;
    public GameObject BasicArms;
    public GameObject LifterArms;
    public GameObject ShieldArms;
    public GameObject BasicLegs;
    public GameObject GroundbreakerLegs;
    public GameObject AfterburnerLegs;

    public void HandleParts(int partConfig , int armConfig , int legConfig)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag == "Arms")
            {
                currentArms = transform.GetChild(i).gameObject;
            }
            else if (transform.GetChild(i).tag == "Legs")
            {
                currentLegs = transform.GetChild(i).gameObject;
            }
        }


        if(partConfig == 1) // no arms or legs
        {
            if(currentArms != null)
            {
                currentArms.GetComponent<Arms>().Detached();
                Destroy(currentArms);
            }
            if(currentLegs != null)
            {
                currentLegs.GetComponent<Legs>().Detached( 0 , 0);
                Destroy(currentLegs);
            }
        }

        if(partConfig == 2)
        {

        }

        if(partConfig == 3)
        {

        }

        if(partConfig == 4)
        {

        }

        gameObject.GetComponent<playerScript>().UpdateParts();
    }

    public void ArmCheck(int armConfig , int partConfig)
    {
        if(partConfig == 2 || partConfig == 4)
        {
            if(armConfig == 1)
            {
                Destroy(currentArms);
                currentArms = Instantiate(BasicArms , transform.position , Quaternion.identity);
            }
            else if(armConfig == 2)
            {
                Destroy(currentArms);
                currentArms = Instantiate(LifterArms , transform.position , Quaternion.identity);
            }
            else if(armConfig == 3)
            {
                Destroy(currentArms);
                currentArms = Instantiate(ShieldArms , transform.position , Quaternion.identity);
            }

            currentArms.transform.parent = gameObject.transform;
            currentArms.transform.localPosition = Vector2.zero;
        }
        else
        {
            Destroy(currentArms);
        }
    }

    public void LegCheck(int legConfig , int partConfig)
    {
        if(partConfig == 3 || partConfig == 4)
        {
            if(legConfig == 1)
            {
                Destroy(currentLegs);
                currentLegs = Instantiate(BasicLegs , transform.position , Quaternion.identity);
            }
            else if(legConfig == 2)
            {
                Destroy(currentLegs);
                currentLegs = Instantiate(GroundbreakerLegs , transform.position , Quaternion.identity);
            }
            else if(legConfig == 3)
            {
                Destroy(currentLegs);
                currentLegs = Instantiate(GroundbreakerLegs , transform.position , Quaternion.identity);
            }
            
            currentLegs.transform.parent = gameObject.transform;
            currentLegs.transform.localPosition = Vector2.zero;
        }
        else
        {
            Destroy(currentLegs);
        } 
    }
}
