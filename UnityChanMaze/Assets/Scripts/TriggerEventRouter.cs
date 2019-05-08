/* 
 * written by Ninoslav Kjireski 05/2019
 * parts of this project were provided by Joseph Hocking 2017
 * and the Unity-Chan Asset Package 05/2019 from the Unity Asset Store.
 * Written for DTT as an application test
 * released under MIT license (https://opensource.org/licenses/MIT)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TriggerEventHandler(GameObject trigger, GameObject other);

public class TriggerEventRouter : MonoBehaviour
{
    public TriggerEventHandler callback;

    void OnTriggerEnter(Collider other)
    {
        if (callback != null)
        {
            callback(this.gameObject, other.gameObject);
        }
    }
}
