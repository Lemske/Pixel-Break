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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        normalColor = buttons[0].GetComponent<Image>().color;
        UpdateSelectedButton(null, buttons[currentSelectedButton]);
        timer = 0.5f;
    }

    void Update()
    {
        Vector3 cornerPosition = new Vector3(0, 0);
        Vector3 screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(cornerPosition.x, cornerPosition.y, Camera.main.nearClipPlane));
        Cursor.SetCursor(null, new Vector2(screenPosition.x, screenPosition.y), CursorMode.ForceSoftware);

        if (Input.GetButtonUp("Fire"))
        {
            buttons[currentSelectedButton].GetComponent<Button>().onClick.Invoke();
        }
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }

        float vertical = Input.GetAxis("Vertical");
        if (vertical != 0)
        {
            Debug.Log("Vertical: " + vertical);
            GameObject prev = buttons[currentSelectedButton];
            currentSelectedButton = (currentSelectedButton + (vertical < 0 ? -1 : 1)) % buttons.Length;
            Debug.Log("Current Selected Button: " + currentSelectedButton);
            currentSelectedButton = currentSelectedButton < 0 ? buttons.Length - 1 : currentSelectedButton;
            UpdateSelectedButton(prev, buttons[currentSelectedButton]);
            timer = 0.5f;
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
