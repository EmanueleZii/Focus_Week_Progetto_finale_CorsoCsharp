using System;
using System.Collections.Generic;
using UnityEngine;

// Classe serializzabile che rappresenta una lista di task
[Serializable]
public class TaskList {
    public List<TaskData> tasks = new List<TaskData>(); // Lista di tutti i task della settimana
}
