using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public class ButtonFieldAttribute : PropertyAttribute
{
    public string buttonCallbackName;
    public string popupTitle;
    public string popupMessage;
    public bool showConfirmPopup;

    public ButtonFieldAttribute(string callbackName, bool confirmPopup = false, string _popupTitle = "Are you sure ?", string _popupMessage = "Do you want to perform this action ?")
    {
        buttonCallbackName = callbackName;
        showConfirmPopup = confirmPopup;
        popupTitle = _popupTitle;
        popupTitle = _popupMessage;
    }
}
