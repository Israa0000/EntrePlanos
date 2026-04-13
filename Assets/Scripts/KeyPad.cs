using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyPad : MonoBehaviour
{
    [SerializeField] TMP_Text Ans; // Campo de texto donde se muestra el número ingresado
    [SerializeField] CodeDoorBehavior codeDoor; // Referencia al script CodeDoorBehavior
    public bool openTheDoor = false; // Indica si la puerta debe abrirse
    public string answer; // Código correcto
    
    void Start()
    {
        openTheDoor = false;
    }

    void Update()
    {
        
    }

    public void Number(int number)
    {
        if (Ans.text.Length < 4) {
            Ans.text += number.ToString();
        }
       
    }

    public void DeleteNumber() {
        if (Ans.text.Length > 0)
        {
            Ans.text = Ans.text.Remove(Ans.text.Length - 1);        
        }

    }
    public void Execute()
    {
        if (Ans.text == answer) // Verificar si el código ingresado es correcto
        {
            print("correcto");
            openTheDoor = true;
            if (codeDoor != null)
            {
                codeDoor.Toggle(); // Llamar a Toggle() para abrir/cerrar la puerta
            }
        }
        else
        {
            openTheDoor = false;
            Debug.Log("Código incorrecto");
            print(Ans.text);
        }
    }

    public void ClearInput() {
        Ans.text = "";
    }
}
