using System;
using UnityEngine;
// Classe serializzabile che rappresenta un singolo task
[Serializable]
public class TaskData
{
    public string giorno;              // Giorno della settimana associato al task
    public string titolo;              // Titolo del task
    public string descrizione;         // Descrizione dettagliata del task
    public string dataOraCreazione;    // Data e ora di creazione del task
    
    // Nuova proprietà priorità (0 = Bassa, 1 = Media, 2 = Alta)
    public int priorita = 0;
}
