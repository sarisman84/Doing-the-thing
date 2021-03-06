using System;
using TPUModelerEditor;
using UnityEditor;
using UnityEngine;

namespace Editor.Scriptable_Wizards
{
    public class EventListenerDocumentation : ScriptableWizard
    {
        private bool usingEventsFoldout, createEventsFoldout;

        public static void CreateWizard()
        {
            DisplayWizard<EventListenerDocumentation>("Event Listener Documentation");
        }

        public void OnWizardCreate()
        {
        }

        private void OnEnable()
        {
        }

        private void OnGUI()
        {
            createEventsFoldout = EditorGUILayout.Toggle(new GUIContent("Creating events!"), createEventsFoldout,
                GUIStyles.helpButtonStyle);
            if (createEventsFoldout)
            {
                EditorGUILayout.HelpBox(new GUIContent(
                    "Creating events is relatively simple. \nAll you have to do is to Right click in the Project window > Create > Event > New Event. \nThis will create an event for you in the project window."));

                DrawPicture(Resources.Load<Texture2D>("ELDocs/createEventsTexture"));
            }

            usingEventsFoldout = EditorGUILayout.Toggle(new GUIContent("Using events!"), usingEventsFoldout,
                GUIStyles.helpButtonStyle);
            if (usingEventsFoldout)
            {
                EditorGUILayout.HelpBox(new GUIContent(
                    "There are two steps in order to use an event. \nThe first one is to assign the event to a Unity Event. To do so, you simply drag the event itself onto a Unity Event as seen below."));

                DrawPicture(Resources.Load<Texture2D>("ELDocs/useEvents_InvokeTexture"));
                EditorGUILayout.HelpBox(new GUIContent(
                    "In the event itself, you have the option of calling the 'OnInvokeEvent()' which will call whatever is defined within that the Event Listener as seen below. \nFurthermore, the 'OnInvokeEvent()' can be assigned with a GameObject variable to be used as a filter to trigger specific objects.  \nIn this case, we only want a specific platform to be affected, as such, the platform gets assigned as a variable to the method."));
                EditorGUILayout.HelpBox(new GUIContent(
                    "The Event Listener uses the assigned Event to define a behaviour for either specific objects or anything that has called the particular event. To determine what is event is global and what isnt global, one can edit the Event asset itself and change the 'Is Global' boolean to determine what event type the aforementioned event is. \nThe conditions list defines what objects are required to call the event itself, which results in ONLY those objects getting affected by the aforementioned event. \nIn this instance, since we are trying to affect a particular platform, we add said platform to the condition list and call its Move() function in order to move upon the invocation of the event."));

                DrawPicture(Resources.Load<Texture2D>("ELDocs/useEvents_CallMethod"));
            }
        }

        private void DrawPicture(Texture2D texture)
        {
            if (texture)
            {
                Rect rect = GUILayoutUtility.GetRect(texture.width, texture.height);
                EditorGUI.DrawPreviewTexture(rect, texture,
                    null, ScaleMode.ScaleAndCrop);
            }
        }


        public void OnWizardUpdate()
        {
        }

        public void OnWizardOtherButton()
        {
        }
    }
}