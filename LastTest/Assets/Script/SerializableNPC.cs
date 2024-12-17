using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableNPC
{
    public string npcName;
    public bool question1Solved;
    public bool question2Solved;
    public bool question3Solved;
    public List<int> visitedLocations; // 방문한 장소 추가

    public SerializableNPC(NPCManager.NPCInfo npc)
    {
        npcName = npc.npcName;
        question1Solved = npc.question1Solved;
        question2Solved = npc.question2Solved;
        question3Solved = npc.question3Solved;
        visitedLocations = new List<int>(npc.visitedLocations); // 리스트 복사
    }

    public void ApplyTo(NPCManager.NPCInfo npc)
    {
        npc.npcName = npcName;
        npc.question1Solved = question1Solved;
        npc.question2Solved = question2Solved;
        npc.question3Solved = question3Solved;
        npc.visitedLocations = new List<int>(visitedLocations); // 리스트 복사
    }
}
