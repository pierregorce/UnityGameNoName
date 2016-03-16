using UnityEngine;
using System.Collections;


public class TileTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        ENTER, EXIT
    }

    public Room associateRoom { get; set; }
    public TriggerType triggerType;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {

        //Debug.Log("trig");
        if (triggerType == TriggerType.ENTER)
        {
            int currentRoomIndex = GameManager.instance.mapGenerator.GetCurrentRoomIndex() + 1;

            if (associateRoom != GameManager.instance.mapGenerator.GetCurrentRoom())
            {
                GameManager.instance.EnterInRoom(associateRoom);
            }
        }



        //if (triggerType == TriggerType.EXIT)
        //{
        //    GameManager.instance.uiManager.ShowInformationUI("START", "ROOM " + GameManager.instance.mapGenerator.GetCurrentRoomIndex(), "Destroy all ennemies. " + " Remaining.");
        //}



    }
}
