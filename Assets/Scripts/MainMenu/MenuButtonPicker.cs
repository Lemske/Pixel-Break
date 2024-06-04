using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuButtonPicker : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private AudioSource change;
    private int currentSelectedButton = 0;
    private Color normalColor;
    private float timer = 0f;
    void Start()
    {
        normalColor = buttons[0].GetComponent<Image>().color;
        UpdateSelectedButton(null, buttons[currentSelectedButton]);
    }

    void Update()
    {
        if (Input.GetButtonUp("Fire"))
        {
            buttons[currentSelectedButton].GetComponent<Button>().onClick.Invoke();
        }
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }
        if (Input.GetButton("Vertical"))
        {
            GameObject prev = buttons[currentSelectedButton];
            currentSelectedButton = (currentSelectedButton + 1) % buttons.Length;
            UpdateSelectedButton(prev, buttons[currentSelectedButton]);
            timer = 0.2f;
        }
    }

    private void UpdateSelectedButton(GameObject prev, GameObject next)
    {
        if (prev != null)
        {
            prev.GetComponent<Image>().color = normalColor;
        }
        change.Play();
        next.GetComponent<Image>().color = Color.green;
    }
}
