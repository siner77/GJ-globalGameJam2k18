using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CardBase : MonoBehaviour
{
    public float Cooldown;
    [SerializeField]
    private Image _cooldownImage;

    private Button _button;
    private float _timer = 0.0f;
    private bool _used;

    private void Start()
    {
        _button = GetComponent<Button>();
        if(_button.onClick.GetPersistentEventCount() == 0)
        {
            _button.onClick.AddListener(Use);
        }

        _cooldownImage.enabled = false;
        _cooldownImage.type = Image.Type.Filled;
        _cooldownImage.fillAmount = 1.0f;
        _cooldownImage.fillMethod = Image.FillMethod.Radial360;
        _cooldownImage.fillClockwise = true;
    }

    private void Update()
    {
        if(_used)
        {
            _timer += Time.deltaTime;
            _cooldownImage.fillAmount = _timer / Cooldown;
            if(_timer >= Cooldown)
            {
                _used = false;
                _cooldownImage.enabled = false;
                _button.interactable = true;
            }
            return;
        }

        OnUpdate();
    }

    protected virtual void OnUpdate()
    {

    }

    protected virtual void Use()
    {
        OnUsed();
    }

    protected void OnUsed()
    {
        _used = true;
        _timer = 0.0f;
        _button.interactable = false;
        _cooldownImage.enabled = true;
        _cooldownImage.fillAmount = 0.0f;
    }
}
